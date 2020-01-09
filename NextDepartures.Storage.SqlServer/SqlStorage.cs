using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;

namespace NextDepartures.Storage.SqlServer
{
    /// <summary>
    /// Implements the data storage for the SQL Server
    /// </summary>
    public class SqlStorage : IDataStorage
    {
        private readonly string _connection;

        /// <summary>
        /// Creates a new sql storage.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        public SqlStorage(string connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Loads a sql server connection.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        public static SqlStorage Load(string connection)
        {
            return new SqlStorage(connection);
        }

        private async Task<List<T>> ExecuteCommand<T>(string sql, Func<SqlDataReader, T> entryProcessor) where T : class
        {
            List<T> results = new List<T>();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection)
                {
                    CommandTimeout = 0,
                    CommandType = CommandType.Text
                };

                SqlDataReader dataReader = await command.ExecuteReaderAsync();

                while (await dataReader.ReadAsync())
                {
                    results.Add(entryProcessor(dataReader));
                }

                dataReader.Close();
                command.Dispose();
            }

            return results;
        }

        private Standard.Models.Agency GetAgencyFromDataReader(SqlDataReader dataReader)
        {
            return new Standard.Models.Agency()
            {
                AgencyID = dataReader.GetValue(0).ToString(),
                AgencyName = dataReader.GetValue(1).ToString(),
                AgencyTimezone = dataReader.GetValue(2).ToString()
            };
        }

        private Standard.Models.Agency GetAgencyFromDataReaderWithSpecialCasing(SqlDataReader dataReader)
        {
            return new Standard.Models.Agency()
            {
                AgencyID = dataReader.GetValue(0).ToString(),
                AgencyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataReader.GetValue(1).ToString().ToLower()),
                AgencyTimezone = dataReader.GetValue(2).ToString()
            };
        }

        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesAsync()
        {
            return ExecuteCommand("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency", GetAgencyFromDataReader);
        }

        /// <summary>
        /// Gets the agencies by the given query and timezone.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            return ExecuteCommand(string.Format("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency WHERE (LOWER(AgencyID) LIKE '%{0}%' OR LOWER(AgencyName) LIKE '%{0}%') AND LOWER(AgencyTimezone) LIKE '%{1}%'", query.ToLower(), timezone.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency WHERE LOWER(AgencyID) LIKE '%{0}%' OR LOWER(AgencyName) LIKE '%{0}%'", query.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency WHERE LOWER(AgencyTimezone) LIKE '%{0}%'", timezone.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        private Standard.Models.Departure GetDepartureFromDataReader(SqlDataReader dataReader)
        {
            return new Standard.Models.Departure()
            {
                DepartureTime = dataReader.GetValue(0).ToString(),
                StopID = dataReader.GetValue(1).ToString(),
                ServiceID = dataReader.GetValue(2).ToString(),
                TripID = dataReader.GetValue(3).ToString(),
                TripHeadsign = dataReader.GetValue(4).ToString(),
                TripShortName = dataReader.GetValue(5).ToString(),
                AgencyID = dataReader.GetValue(6).ToString(),
                RouteShortName = dataReader.GetValue(7).ToString(),
                RouteLongName = dataReader.GetValue(8).ToString(),
                Monday = dataReader.GetValue(9).ToString(),
                Tuesday = dataReader.GetValue(10).ToString(),
                Wednesday = dataReader.GetValue(11).ToString(),
                Thursday = dataReader.GetValue(12).ToString(),
                Friday = dataReader.GetValue(13).ToString(),
                Saturday = dataReader.GetValue(14).ToString(),
                Sunday = dataReader.GetValue(15).ToString(),
                StartDate = dataReader.GetValue(16).ToString(),
                EndDate = dataReader.GetValue(17).ToString()
            };
        }

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Standard.Models.Departure>> GetDeparturesForStopAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopID, t.ServiceID, t.TripID, t.TripHeadsign, t.TripShortName, r.AgencyID, r.RouteShortName, r.RouteLongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripID = t.TripID) LEFT JOIN Route r ON (t.RouteID = r.RouteID) LEFT JOIN Calendar c ON (t.ServiceID = c.ServiceID) WHERE LOWER(s.StopID) = '{0}' AND s.PickupType != '1' ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Standard.Models.Departure>> GetDeparturesForTripAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopID, t.ServiceID, t.TripID, t.TripHeadsign, t.TripShortName, r.AgencyID, r.RouteShortName, r.RouteLongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripID = t.TripID) LEFT JOIN Route r ON (t.RouteID = r.RouteID) LEFT JOIN Calendar c ON (t.ServiceID = c.ServiceID) WHERE LOWER(s.TripID) = '{0}' AND s.PickupType != '1' ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        private Standard.Models.Exception GetExceptionFromDataReader(SqlDataReader dataReader)
        {
            return new Standard.Models.Exception()
            {
                Date = dataReader.GetValue(0).ToString(),
                ExceptionType = dataReader.GetValue(1).ToString(),
                ServiceID = dataReader.GetValue(2).ToString()
            };
        }

        /// <summary>
        /// Gets all available exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        public Task<List<Standard.Models.Exception>> GetExceptionsAsync()
        {
            return ExecuteCommand("SELECT Date, ExceptionType, ServiceID FROM CalendarDate", GetExceptionFromDataReader);
        }

        private Standard.Models.Stop GetStopFromDataReader(SqlDataReader dataReader)
        {
            return new Standard.Models.Stop()
            {
                StopID = dataReader.GetValue(0).ToString(),
                StopCode = dataReader.GetValue(1).ToString(),
                StopName = dataReader.GetValue(2).ToString(),
                StopTimezone = dataReader.GetValue(3).ToString()
            };
        }

        private Standard.Models.Stop GetStopFromDataReaderWithSpecialCasing(SqlDataReader dataReader)
        {
            return new Standard.Models.Stop()
            {
                StopID = dataReader.GetValue(0).ToString(),
                StopCode = dataReader.GetValue(1).ToString(),
                StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataReader.GetValue(2).ToString().ToLower()),
                StopTimezone = dataReader.GetValue(3).ToString()
            };
        }

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsAsync()
        {
            return ExecuteCommand("SELECT StopID, StopCode, StopName, StopTimezone FROM Stop", GetStopFromDataReader);
        }

        /// <summary>
        /// Gets the stops by the given area, query and timezone.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopCode, StopName, StopTimezone FROM Stop WHERE (LOWER(StopID) LIKE '%{0}%' OR LOWER(StopCode) LIKE '%{0}%' OR LOWER(StopName) LIKE '%{0}%') AND (CAST(StopLat as REAL) >= {1} AND CAST(StopLat as REAL) <= {2} AND CAST(StopLon as REAL) >= {3} AND CAST(StopLon as REAL) <= {4}) AND (StopLat != '0' AND StopLon != '0') AND LOWER(StopTimezone) LIKE '%{5}%'", query.ToLower(), minLat, maxLat, minLon, maxLon, timezone.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopCode, StopName, StopTimezone FROM Stop WHERE (CAST(StopLat as REAL) >= {0} AND CAST(StopLat as REAL) <= {1} AND CAST(StopLon as REAL) >= {2} AND CAST(StopLon as REAL) <= {3}) AND (StopLat != '0' AND StopLon != '0')", minLat, maxLat, minLon, maxLon), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopCode, StopName, StopTimezone FROM Stop WHERE (LOWER(StopID) LIKE '%{0}%' OR LOWER(StopCode) LIKE '%{0}%' OR LOWER(StopName) LIKE '%{0}%') AND (StopLat != '0' AND StopLon != '0')", query.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopCode, StopName, StopTimezone FROM Stop WHERE (StopLat != '0' AND StopLon != '0') AND LOWER(StopTimezone) LIKE '%{0}%'", timezone.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }
    }
}