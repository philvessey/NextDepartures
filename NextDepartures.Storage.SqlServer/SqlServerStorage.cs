using GTFS.Entities;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
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
    public class SqlServerStorage : IDataStorage
    {
        private readonly string _connection;

        /// <summary>
        /// Creates a new sql server storage.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        public SqlServerStorage(string connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Loads a sql server connection.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        public static SqlServerStorage Load(string connection)
        {
            return new SqlServerStorage(connection);
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

        private Agency GetAgencyFromDataReader(SqlDataReader dataReader)
        {
            return new Agency()
            {
                Id = dataReader.IsDBNull(0) ? null : dataReader.GetString(0),
                Name = dataReader.GetString(1),
                URL = dataReader.GetString(2),
                Timezone = dataReader.GetString(3),
                LanguageCode = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                Phone = dataReader.IsDBNull(5) ? null : dataReader.GetString(5),
                FareURL = dataReader.IsDBNull(6) ? null : dataReader.GetString(6),
                Email = dataReader.IsDBNull(7) ? null : dataReader.GetString(7)
            };
        }

        private Agency GetAgencyFromDataReaderWithSpecialCasing(SqlDataReader dataReader)
        {
            return new Agency()
            {
                Id = dataReader.IsDBNull(0) ? null : dataReader.GetString(0),
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataReader.GetString(1).ToLower()),
                URL = dataReader.GetString(2),
                Timezone = dataReader.GetString(3),
                LanguageCode = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                Phone = dataReader.IsDBNull(5) ? null : dataReader.GetString(5),
                FareURL = dataReader.IsDBNull(6) ? null : dataReader.GetString(6),
                Email = dataReader.IsDBNull(7) ? null : dataReader.GetString(7)
            };
        }

        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesAsync()
        {
            return ExecuteCommand("SELECT * FROM Agency", GetAgencyFromDataReader);
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT * FROM Agency WHERE LOWER(Id) LIKE '%{0}%' OR LOWER(Name) LIKE '%{0}%'", query.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM Agency WHERE LOWER(Timezone) LIKE '%{0}%'", timezone.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        private CalendarDate GetCalendarDatesFromDataReader(SqlDataReader dataReader)
        {
            return new CalendarDate()
            {
                ServiceId = dataReader.GetString(0),
                Date = dataReader.GetDateTime(1),
                ExceptionType = dataReader.GetInt32(2).ToExceptionType()
            };
        }

        /// <summary>
        /// Gets all available calendar dates.
        /// </summary>
        /// <returns>A list of calendar dates.</returns>
        public Task<List<CalendarDate>> GetCalendarDatesAsync()
        {
            return ExecuteCommand("SELECT * FROM CalendarDate", GetCalendarDatesFromDataReader);
        }

        private Departure GetDepartureFromDataReader(SqlDataReader dataReader)
        {
            return new Departure()
            {
                DepartureTime = dataReader.GetString(0),
                StopId = dataReader.GetString(1),
                TripId = dataReader.GetString(2),
                ServiceId = dataReader.GetString(3),
                TripHeadsign = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                TripShortName = dataReader.IsDBNull(5) ? null : dataReader.GetString(5),
                AgencyId = dataReader.IsDBNull(6) ? null : dataReader.GetString(6),
                RouteShortName = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                RouteLongName = dataReader.IsDBNull(8) ? null : dataReader.GetString(8),
                Monday = dataReader.GetBoolean(9),
                Tuesday = dataReader.GetBoolean(10),
                Wednesday = dataReader.GetBoolean(11),
                Thursday = dataReader.GetBoolean(12),
                Friday = dataReader.GetBoolean(13),
                Saturday = dataReader.GetBoolean(14),
                Sunday = dataReader.GetBoolean(15),
                StartDate = dataReader.GetDateTime(16),
                EndDate = dataReader.GetDateTime(17)
            };
        }

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripId = t.Id) LEFT JOIN Route r ON (t.RouteId = r.Id) LEFT JOIN Calendar c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.StopId) = '{0}' AND s.PickupType != 1 ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripId = t.Id) LEFT JOIN Route r ON (t.RouteId = r.Id) LEFT JOIN Calendar c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.TripId) = '{0}' AND s.PickupType != 1 ORDER BY s.DepartureTime ASC", id.ToLower()), GetDepartureFromDataReader);
        }

        private Stop GetStopFromDataReader(SqlDataReader dataReader)
        {
            return new Stop()
            {
                Id = dataReader.GetString(0),
                Code = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                Name = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                Description = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                Latitude = dataReader.GetDouble(4),
                Longitude = dataReader.GetDouble(5),
                Zone = dataReader.IsDBNull(6) ? null : dataReader.GetString(6),
                Url = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                LocationType = dataReader.IsDBNull(8) ? null : dataReader.GetInt32(8).ToLocationType(),
                ParentStation = dataReader.IsDBNull(9) ? null : dataReader.GetString(9),
                Timezone = dataReader.IsDBNull(10) ? null : dataReader.GetString(10),
                WheelchairBoarding = dataReader.IsDBNull(11) ? null : dataReader.GetString(11),
                LevelId = dataReader.IsDBNull(12) ? null : dataReader.GetString(12),
                PlatformCode = dataReader.IsDBNull(13) ? null : dataReader.GetString(13)
            };
        }

        private Stop GetStopFromDataReaderWithSpecialCasing(SqlDataReader dataReader)
        {
            return new Stop()
            {
                Id = dataReader.GetString(0),
                Code = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                Name = dataReader.IsDBNull(2) ? null : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataReader.GetString(2).ToLower()),
                Description = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                Latitude = dataReader.GetDouble(4),
                Longitude = dataReader.GetDouble(5),
                Zone = dataReader.IsDBNull(6) ? null : dataReader.GetString(6),
                Url = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                LocationType = dataReader.IsDBNull(8) ? null : dataReader.GetInt32(8).ToLocationType(),
                ParentStation = dataReader.IsDBNull(9) ? null : dataReader.GetString(9),
                Timezone = dataReader.IsDBNull(10) ? null : dataReader.GetString(10),
                WheelchairBoarding = dataReader.IsDBNull(11) ? null : dataReader.GetString(11),
                LevelId = dataReader.IsDBNull(12) ? null : dataReader.GetString(12),
                PlatformCode = dataReader.IsDBNull(13) ? null : dataReader.GetString(13)
            };
        }

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsAsync()
        {
            return ExecuteCommand("SELECT * FROM Stop", GetStopFromDataReader);
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude)
        {
            return ExecuteCommand(string.Format("SELECT * FROM Stop WHERE Latitude >= {0} AND Latitude <= {1} AND Longitude >= {2} AND Longitude <= {3}", minimumLatitude, maximumLatitude, minimumLongitude, maximumLongitude), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT * FROM Stop WHERE LOWER(Id) LIKE '%{0}%' OR LOWER(Code) LIKE '%{0}%' OR LOWER(Name) LIKE '%{0}%'", query.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM Stop WHERE LOWER(Timezone) LIKE '%{0}%'", timezone.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }
    }
}