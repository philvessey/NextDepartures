using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Model;

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

        private async Task<List<T>> ExecuteCommand<T>(string sql, Func<SqlDataReader, T> entryProcessor) where T : class
        {
            var resultList = new List<T>();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();

                var command = new SqlCommand(sql, connection)
                {
                    CommandTimeout = 0,
                    CommandType = CommandType.Text
                };

                var dataReader = await command.ExecuteReaderAsync();

                while (await dataReader.ReadAsync())
                {
                    resultList.Add(entryProcessor(dataReader));
                }

                dataReader.Close();
                command.Dispose();
            }

            return resultList;
        }

        private Departure GetDepartureFromDataReader(SqlDataReader dataReader)
        {
            return new Departure()
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

        private Stop GetStopFromDataReaderWithSpecialCasing(SqlDataReader dataReader)
        {
            return new Stop()
            {
                StopID = dataReader.GetValue(0).ToString(),
                StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataReader.GetValue(1).ToString().ToLower()),
                StopTimezone = dataReader.GetValue(2).ToString()
            };
        }

        private Stop GetStopFromDataReader(SqlDataReader dataReader)
        {
            return new Stop()
            {
                StopID = dataReader.GetValue(0).ToString(),
                StopName = dataReader.GetValue(1).ToString(),
                StopTimezone = dataReader.GetValue(2).ToString()
            };
        }

        private Standard.Model.Exception GetExceptionFromDataReader(SqlDataReader dataReader)
        {
            return new Standard.Model.Exception()
            {
                Date = dataReader.GetValue(0).ToString(),
                ExceptionType = dataReader.GetValue(1).ToString(),
                ServiceID = dataReader.GetValue(2).ToString()
            };
        }

        private Agency GetAgencyFromDataReader(SqlDataReader dataReader)
        {
            return new Agency()
            {
                AgencyID = dataReader.GetValue(0).ToString(),
                AgencyName = dataReader.GetValue(1).ToString(),
                AgencyTimezone = dataReader.GetValue(2).ToString()
            };
        }

        public Task<List<Agency>> GetAgenciesAsync()
        {
            return ExecuteCommand("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency", GetAgencyFromDataReader);
        }

        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopID, t.ServiceID, t.TripID, t.TripHeadsign, t.TripShortName, r.AgencyID, r.RouteShortName, r.RouteLongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripID = t.TripID) LEFT JOIN Route r ON (t.RouteID = r.RouteID) LEFT JOIN Calendar c ON (t.ServiceID = c.ServiceID) WHERE LOWER(s.StopID) = '{0}' AND s.PickupType != '1' ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopID, t.ServiceID, t.TripID, t.TripHeadsign, t.TripShortName, r.AgencyID, r.RouteShortName, r.RouteLongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripID = t.TripID) LEFT JOIN Route r ON (t.RouteID = r.RouteID) LEFT JOIN Calendar c ON (t.ServiceID = c.ServiceID) WHERE LOWER(s.TripID) = '{0}' AND s.PickupType != '1' ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        public Task<List<Standard.Model.Exception>> GetExceptionsAsync()
        {
            return ExecuteCommand("SELECT Date, ExceptionType, ServiceID FROM CalendarDate", GetExceptionFromDataReader);
        }

        public Task<List<Stop>> GetStopsAsync()
        {
            return ExecuteCommand("SELECT StopID, StopName, StopTimezone FROM Stop", GetStopFromDataReader);
        }

        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopName, StopTimezone FROM Stop WHERE CAST(StopLat as REAL) >= {0} AND CAST(StopLat as REAL) <= {1} AND CAST(StopLon as REAL) >= {2} AND CAST(StopLon as REAL) <= {3} AND StopLat != '0' AND StopLon != '0'", minLat, maxLat, minLon, maxLon), GetStopFromDataReaderWithSpecialCasing);
        }

        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopName, StopTimezone FROM Stop WHERE LOWER(StopID) LIKE '%{0}%' OR LOWER(StopName) LIKE '%{0}%' AND StopLat != '0' AND StopLon != '0'", query.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        public Task<List<Stop>> GetStopsByLocationAndQueryAsync(double minLon, double minLat, double maxLon, double maxLat, string query)
        {
            return ExecuteCommand(string.Format("SELECT StopID, StopName, StopTimezone FROM Stop WHERE LOWER(StopID) LIKE '%{0}%' OR LOWER(StopName) LIKE '%{0}%' AND CAST(StopLat as REAL) >= {1} AND CAST(StopLat as REAL) <= {2} AND CAST(StopLon as REAL) >= {3} AND CAST(StopLon as REAL) <= {4} AND StopLat != '0' AND StopLon != '0'", query.ToLower(), minLat, maxLat, minLon, maxLon), GetStopFromDataReaderWithSpecialCasing);
        }
    }
}
