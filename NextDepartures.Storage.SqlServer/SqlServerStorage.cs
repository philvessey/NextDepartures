using GTFS.Entities;
using GTFS.Entities.Enumerations;
using Microsoft.Data.SqlClient;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly string _prefix;

        /// <summary>
        /// Creates a new sql server storage.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        /// <param name="prefix">The table prefix to select.</param>
        public SqlServerStorage(string connection, string prefix)
        {
            _connection = connection;
            _prefix = prefix;
        }

        /// <summary>
        /// Loads a sql server connection.
        /// </summary>
        /// <param name="connection">The connection string to use when connecting to a database.</param>
        /// <param name="prefix">The table prefix to select. Default is GTFS but can be overridden.</param>
        public static SqlServerStorage Load(string connection, string prefix = "GTFS")
        {
            return new SqlServerStorage(connection, prefix);
        }

        private async Task<List<T>> ExecuteCommand<T>(string sql, Func<SqlDataReader, T> entryProcessor) where T : class
        {
            List<T> results = new List<T>();

            using SqlConnection connection = new SqlConnection(_connection);
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
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY", _prefix.ToUpper()), GetAgencyFromDataReader);
        }

        /// <summary>
        /// Gets the agencies by the given email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByEmailAsync(string email)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(ISNULL(Email, '')) LIKE '%{1}%'", _prefix.ToUpper(), email.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given fare URL.
        /// </summary>
        /// <param name="fareURL">The fare URL.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByFareURLAsync(string fareURL)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(ISNULL(FareURL, '')) LIKE '%{1}%'", _prefix.ToUpper(), fareURL.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(ISNULL(LanguageCode, '')) LIKE '%{1}%'", _prefix.ToUpper(), languageCode.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given phone.
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(ISNULL(Phone, '')) LIKE '%{1}%'", _prefix.ToUpper(), phone.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(ISNULL(Id, '')) LIKE '%{1}%' OR LOWER(Name) LIKE '%{1}%'", _prefix.ToUpper(), query.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(Timezone) LIKE '%{1}%'", _prefix.ToUpper(), timezone.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the agencies by the given URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByURLAsync(string url)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_AGENCY WHERE LOWER(URL) LIKE '%{1}%'", _prefix.ToUpper(), url.ToLower()), GetAgencyFromDataReaderWithSpecialCasing);
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
            return ExecuteCommand(string.Format("SELECT * FROM {0}_CALENDAR_DATE", _prefix.ToUpper()), GetCalendarDatesFromDataReader);
        }

        private Departure GetDepartureFromDataReader(SqlDataReader dataReader)
        {
            return new Departure()
            {
                DepartureTime = dataReader.IsDBNull(0) ? null : dataReader.GetString(0).ToTimeOfDay(),
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
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM {0}_STOP_TIME s LEFT JOIN {0}_TRIP t ON (s.TripId = t.Id) LEFT JOIN {0}_ROUTE r ON (t.RouteId = r.Id) LEFT JOIN {0}_CALENDAR c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.StopId) = '{1}' AND s.PickupType != 1 ORDER BY s.DepartureTime ASC", _prefix.ToUpper(), id.ToLower()), GetDepartureFromDataReader);
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM {0}_STOP_TIME s LEFT JOIN {0}_TRIP t ON (s.TripId = t.Id) LEFT JOIN {0}_ROUTE r ON (t.RouteId = r.Id) LEFT JOIN {0}_CALENDAR c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.TripId) = '{1}' AND s.PickupType != 1 ORDER BY s.DepartureTime ASC", _prefix.ToUpper(), id.ToLower()), GetDepartureFromDataReader);
        }

        private Stop GetStopFromDataReader(SqlDataReader dataReader)
        {
            return new Stop()
            {
                Id = dataReader.GetString(0),
                Code = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                Name = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                Description = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                Longitude = dataReader.GetDouble(4),
                Latitude = dataReader.GetDouble(5),
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
                Longitude = dataReader.GetDouble(4),
                Latitude = dataReader.GetDouble(5),
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
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP", _prefix.ToUpper()), GetStopFromDataReader);
        }

        /// <summary>
        /// Gets the stops by the given description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByDescriptionAsync(string description)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(Description, '')) LIKE '%{1}%'", _prefix.ToUpper(), description.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given level.
        /// </summary>
        /// <param name="id">The id of the level.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLevelAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(LevelId, '')) = '{1}'", _prefix.ToUpper(), id.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops in the given location.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE Longitude >= {1} AND Latitude >= {2} AND Longitude <= {3} AND Latitude <= {4}", _prefix.ToUpper(), minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given location type.
        /// </summary>
        /// <param name="locationType">The location type.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LocationType = {1}", _prefix.ToUpper(), locationType.ToInt32()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given parent station.
        /// </summary>
        /// <param name="id">The id of the station.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByParentStationAsync(string id)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(ParentStation, '')) = '{1}'", _prefix.ToUpper(), id.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given platform code.
        /// </summary>
        /// <param name="platformCode">The platform code.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(PlatformCode, '')) = '{1}'", _prefix.ToUpper(), platformCode.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(Id) LIKE '%{1}%' OR LOWER(ISNULL(Code, '')) LIKE '%{1}%' OR LOWER(ISNULL(Name, '')) LIKE '%{1}%'", _prefix.ToUpper(), query.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(Timezone, '')) LIKE '%{1}%'", _prefix.ToUpper(), timezone.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByURLAsync(string url)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(Url, '')) LIKE '%{1}%'", _prefix.ToUpper(), url.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given wheelchair boarding.
        /// </summary>
        /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(WheelchairBoarding, '')) = '{1}'", _prefix.ToUpper(), wheelchairBoarding.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }

        /// <summary>
        /// Gets the stops by the given zone.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByZoneAsync(string zone)
        {
            return ExecuteCommand(string.Format("SELECT * FROM {0}_STOP WHERE LOWER(ISNULL(Zone, '')) = '{1}'", _prefix.ToUpper(), zone.ToLower()), GetStopFromDataReaderWithSpecialCasing);
        }
    }
}