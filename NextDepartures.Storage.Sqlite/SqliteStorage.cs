﻿using GTFS.Entities;
using GTFS.Entities.Enumerations;
using Microsoft.Data.Sqlite;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace NextDepartures.Storage.Sqlite;

/// <summary>
/// Implements the data storage for the SQLite database
/// </summary>
public class SqliteStorage : IDataStorage
{
    private readonly string _connection;
    private readonly string _prefix;
        
    private SqliteStorage(string connection, string prefix)
    {
        _connection = connection;
        _prefix = prefix;
    }

    /// <summary>
    /// Loads a sqlite database connection.
    /// </summary>
    /// <param name="connection">The connection string to use when connecting to a database.</param>
    /// <param name="prefix">The table prefix to select. Default is GTFS but can be overridden.</param>
    public static SqliteStorage Load(string connection, string prefix = "GTFS")
    {
        return new SqliteStorage(connection, prefix);
    }

    private async Task<List<T>> ExecuteCommand<T>(string sql, Func<SqliteDataReader, T> entryProcessor) where T : class
    {
        List<T> results = [];

        await using SqliteConnection connection = new(_connection);
        connection.Open();

        SqliteCommand command = new(sql, connection)
        {
            CommandTimeout = 0,
            CommandType = CommandType.Text
        };

        var dataReader = await command.ExecuteReaderAsync();

        while (await dataReader.ReadAsync())
        {
            results.Add(entryProcessor(dataReader));
        }

        dataReader.Close();
        command.Dispose();

        return results;
    }

    private static Agency GetAgencyFromDataReader(SqliteDataReader dataReader)
    {
        return new Agency
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

    private static Agency GetAgencyFromDataReaderWithSpecialCasing(SqliteDataReader dataReader)
    {
        return new Agency
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
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY", GetAgencyFromDataReader);
    }

    /// <summary>
    /// Gets the agencies by the given email.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(Email, '')) LIKE '%{email.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies by the given fare url.
    /// </summary>
    /// <param name="fareUrl">The fare url.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(FareURL, '')) LIKE '%{fareUrl.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies by the given language code.
    /// </summary>
    /// <param name="languageCode">The language code.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(LanguageCode, '')) LIKE '%{languageCode.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies by the given phone.
    /// </summary>
    /// <param name="phone">The phone.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(Phone, '')) LIKE '%{phone.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies by the given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(Id, '')) LIKE '%{query.ToLower()}%' OR LOWER(IFNULL(Name, '')) LIKE '%{query.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies in the given timezone.
    /// </summary>
    /// <param name="timezone">The timezone.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(Timezone, '')) LIKE '%{timezone.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the agencies by the given url.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_AGENCY WHERE LOWER(IFNULL(URL, '')) LIKE '%{url.ToLower()}%'", GetAgencyFromDataReaderWithSpecialCasing);
    }

    private static CalendarDate GetCalendarDatesFromDataReader(SqliteDataReader dataReader)
    {
        return new CalendarDate
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
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_CALENDAR_DATE", GetCalendarDatesFromDataReader);
    }

    private static Departure GetDepartureFromDataReader(SqliteDataReader dataReader)
    {
        return new Departure
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
        return ExecuteCommand($"SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM {_prefix.ToUpper()}_STOP_TIME s LEFT JOIN {_prefix.ToUpper()}_TRIP t ON (s.TripId = t.Id) LEFT JOIN {_prefix.ToUpper()}_ROUTE r ON (t.RouteId = r.Id) LEFT JOIN {_prefix.ToUpper()}_CALENDAR c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.StopId) = '{id.ToLower()}' AND IFNULL(s.PickupType, 0) != 1 ORDER BY s.DepartureTime", GetDepartureFromDataReader);
    }

    /// <summary>
    /// Gets the departures for a specific trip.
    /// </summary>
    /// <param name="id">The id of the trip.</param>
    /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
    /// <returns>A list of departures.</returns>
    public Task<List<Departure>> GetDeparturesForTripAsync(string id)
    {
        return ExecuteCommand($"SELECT s.DepartureTime, s.StopId, t.Id, t.ServiceId, t.Headsign, t.ShortName, r.AgencyId, r.ShortName, r.LongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM {_prefix.ToUpper()}_STOP_TIME s LEFT JOIN {_prefix.ToUpper()}_TRIP t ON (s.TripId = t.Id) LEFT JOIN {_prefix.ToUpper()}_ROUTE r ON (t.RouteId = r.Id) LEFT JOIN {_prefix.ToUpper()}_CALENDAR c ON (t.ServiceId = c.ServiceId) WHERE LOWER(s.TripId) = '{id.ToLower()}' AND IFNULL(s.PickupType, 0) != 1 ORDER BY s.DepartureTime", GetDepartureFromDataReader);
    }

    private static Stop GetStopFromDataReader(SqliteDataReader dataReader)
    {
        return new Stop
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

    private static Stop GetStopFromDataReaderWithSpecialCasing(SqliteDataReader dataReader)
    {
        return new Stop
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
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP", GetStopFromDataReader);
    }

    /// <summary>
    /// Gets the stops by the given description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(Description, '')) LIKE '%{description.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given level.
    /// </summary>
    /// <param name="id">The id of the level.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByLevelAsync(string id)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(LevelId, '')) LIKE '%{id.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
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
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE Longitude >= {minimumLongitude} AND Latitude >= {minimumLatitude} AND Longitude <= {maximumLongitude} AND Latitude <= {maximumLatitude}", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given location type.
    /// </summary>
    /// <param name="locationType">The location type.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LocationType = {locationType.ToInt32()}", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given parent station.
    /// </summary>
    /// <param name="id">The id of the station.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByParentStationAsync(string id)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(ParentStation, '')) LIKE '%{id.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given platform code.
    /// </summary>
    /// <param name="platformCode">The platform code.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(PlatformCode, '')) LIKE '%{platformCode.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByQueryAsync(string query)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(Id, '')) LIKE '%{query.ToLower()}%' OR LOWER(IFNULL(Code, '')) LIKE '%{query.ToLower()}%' OR LOWER(IFNULL(Name, '')) LIKE '%{query.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops in the given timezone.
    /// </summary>
    /// <param name="timezone">The timezone.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(Timezone, '')) LIKE '%{timezone.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given url.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByUrlAsync(string url)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(Url, '')) LIKE '%{url.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given wheelchair boarding.
    /// </summary>
    /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(WheelchairBoarding, '')) LIKE '%{wheelchairBoarding.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }

    /// <summary>
    /// Gets the stops by the given zone.
    /// </summary>
    /// <param name="zone">The zone.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByZoneAsync(string zone)
    {
        return ExecuteCommand($"SELECT * FROM {_prefix.ToUpper()}_STOP WHERE LOWER(IFNULL(Zone, '')) LIKE '%{zone.ToLower()}%'", GetStopFromDataReaderWithSpecialCasing);
    }
}