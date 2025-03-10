using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using Microsoft.Data.Sqlite;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;

namespace NextDepartures.Storage.Sqlite;

public class SqliteStorage : IDataStorage
{
    private readonly string _connection;
        
    private SqliteStorage(string connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Loads a SQLite data storage.
    /// </summary>
    /// <param name="connection">The database connection string.</param>
    public static SqliteStorage Load(string connection)
    {
        return new SqliteStorage(connection);
    }

    private async Task<List<T>> ExecuteCommand<T>(string sql, Func<SqliteDataReader, T> entryProcessor) where T : class
    {
        List<T> results = [];

        await using SqliteConnection connection = new(_connection);
        await connection.OpenAsync();

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

        await dataReader.CloseAsync();
        await command.DisposeAsync();

        return results;
    }
    
    private static Agency GetAgencyFromDataReader(SqliteDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(0) ? dataReader.GetString(0) : null,
            Name = dataReader.GetString(1),
            URL = dataReader.GetString(2),
            Timezone = dataReader.GetString(3),
            LanguageCode = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
            Phone = !dataReader.IsDBNull(5) ? dataReader.GetString(5) : null,
            FareURL = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
            Email = !dataReader.IsDBNull(7) ? dataReader.GetString(7) : null
        };
    }
    
    private static Agency GetAgencyFromDataReaderByCondition(SqliteDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(0) ? dataReader.GetString(0) : null,
            Name = dataReader.GetString(1).Trim().ToTitleCase(),
            URL = dataReader.GetString(2),
            Timezone = dataReader.GetString(3),
            LanguageCode = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
            Phone = !dataReader.IsDBNull(5) ? dataReader.GetString(5) : null,
            FareURL = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
            Email = !dataReader.IsDBNull(7) ? dataReader.GetString(7) : null
        };
    }
    
    private static CalendarDate GetCalendarDateFromDataReader(SqliteDataReader dataReader)
    {
        return new CalendarDate
        {
            ServiceId = dataReader.GetString(0),
            Date = dataReader.GetDateTime(1),
            ExceptionType = dataReader.GetInt32(2).ToExceptionType()
        };
    }
    
    private static Departure GetDepartureFromDataReaderByCondition(SqliteDataReader dataReader)
    {
        return new Departure
        {
            DepartureTime = new TimeOfDay
            {
                Hours = !dataReader.IsDBNull(0) ? dataReader.GetString(0).ToTimeOfDay().Hours : 0,
                Minutes = !dataReader.IsDBNull(0) ? dataReader.GetString(0).ToTimeOfDay().Minutes : 0,
                Seconds = !dataReader.IsDBNull(0) ? dataReader.GetString(0).ToTimeOfDay().Seconds : 0,
            },
            
            StopId = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
            TripId = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null,
            ServiceId = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
            TripHeadsign = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
            TripShortName = !dataReader.IsDBNull(5) ? dataReader.GetString(5) : null,
            AgencyId = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
            RouteShortName = !dataReader.IsDBNull(7) ? dataReader.GetString(7) : null,
            RouteLongName = !dataReader.IsDBNull(8) ? dataReader.GetString(8) : null,
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
    
    private static Stop GetStopFromDataReader(SqliteDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(0),
            Code = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
            Name = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null,
            Description = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
            Latitude = dataReader.GetDouble(4),
            Longitude = dataReader.GetDouble(5),
            Zone = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
            Url = !dataReader.IsDBNull(7) ? dataReader.GetString(7) : null,
            LocationType = !dataReader.IsDBNull(8) ? dataReader.GetInt32(8).ToLocationType() : null,
            ParentStation = !dataReader.IsDBNull(9) ? dataReader.GetString(9) : null,
            Timezone = !dataReader.IsDBNull(10) ? dataReader.GetString(10) : null,
            WheelchairBoarding = !dataReader.IsDBNull(11) ? dataReader.GetInt32(11).ToString() : null,
            LevelId = !dataReader.IsDBNull(12) ? dataReader.GetString(12) : null,
            PlatformCode = !dataReader.IsDBNull(13) ? dataReader.GetString(13) : null
        };
    }
    
    private static Stop GetStopFromDataReaderByCondition(SqliteDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(0),
            Code = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
            Name = !dataReader.IsDBNull(2) ? dataReader.GetString(2).Trim().ToTitleCase() : null,
            Description = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
            Latitude = dataReader.GetDouble(4),
            Longitude = dataReader.GetDouble(5),
            Zone = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
            Url = !dataReader.IsDBNull(7) ? dataReader.GetString(7) : null,
            LocationType = !dataReader.IsDBNull(8) ? dataReader.GetInt32(8).ToLocationType() : null,
            ParentStation = !dataReader.IsDBNull(9) ? dataReader.GetString(9) : null,
            Timezone = !dataReader.IsDBNull(10) ? dataReader.GetString(10) : null,
            WheelchairBoarding = !dataReader.IsDBNull(11) ? dataReader.GetInt32(11).ToString() : null,
            LevelId = !dataReader.IsDBNull(12) ? dataReader.GetString(12) : null,
            PlatformCode = !dataReader.IsDBNull(13) ? dataReader.GetString(13) : null
        };
    }
    
    public Task<List<Agency>> GetAgenciesAsync()
    {
        const string sql = "SELECT * " + 
                           "FROM GTFS_AGENCY";

        return ExecuteCommand(sql, GetAgencyFromDataReader);
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        const string sql = "SELECT * " + 
                           "FROM GTFS_CALENDAR_DATES";
        
        return ExecuteCommand(sql, GetCalendarDateFromDataReader);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        const string sql = "SELECT * " + 
                           "FROM GTFS_STOPS";
        
        return ExecuteCommand(sql, GetStopFromDataReader);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) = '{email.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '{email.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '%{email.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '%{email.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) = '{fareUrl.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '{fareUrl.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{fareUrl.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{fareUrl.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '%{id.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) = '{languageCode.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '{languageCode.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '%{languageCode.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '%{languageCode.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) LIKE '{name.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) LIKE '%{name.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(AgencyName) LIKE '%{name.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) = '{phone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '{phone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '%{phone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '%{phone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) = '{search.ToLower()}' " + 
                                           $"OR LOWER(AgencyUrl) = '{search.ToLower()}' " + 
                                           $"OR LOWER(AgencyTimezone) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyId, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyLang, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) = '{search.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(AgencyUrl) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(AgencyTimezone) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(AgencyId, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '{search.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyName) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(AgencyUrl) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(AgencyTimezone) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyId, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '%{search.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(AgencyName) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(AgencyUrl) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(AgencyTimezone) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(AgencyId, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '%{search.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyTimezone) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyTimezone) LIKE '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyTimezone) LIKE '%{timezone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(AgencyTimezone) LIKE '%{timezone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyUrl) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyUrl) LIKE '{url.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(AgencyUrl) LIKE '%{url.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(AgencyUrl) LIKE '%{url.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Partial => "SELECT s.DepartureTime, " + 
                                             "s.StopId, " + 
                                             "t.TripId, " + 
                                             "t.ServiceId, " + 
                                             "t.TripHeadsign, " + 
                                             "t.TripShortName, " + 
                                             "r.AgencyId, " + 
                                             "r.RouteShortName, " + 
                                             "r.RouteLongName, " + 
                                             "c.Monday, " + 
                                             "c.Tuesday, " + 
                                             "c.Wednesday, " + 
                                             "c.Thursday, " + 
                                             "c.Friday, " + 
                                             "c.Saturday, " + 
                                             "c.Sunday, " + 
                                             "c.StartDate, " + 
                                             "c.EndDate " + 
                                      "FROM GTFS_STOP_TIMES s " + 
                                      "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                      "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                      "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                             $"WHERE LOWER(s.StopId) LIKE '%{id.ToLower()}%' " + 
                                                "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                      "ORDER BY s.DepartureTime",

            ComparisonType.Starts => "SELECT s.DepartureTime, " + 
                                            "s.StopId, " + 
                                            "t.TripId, " + 
                                            "t.ServiceId, " + 
                                            "t.TripHeadsign, " + 
                                            "t.TripShortName, " + 
                                            "r.AgencyId, " + 
                                            "r.RouteShortName, " + 
                                            "r.RouteLongName, " + 
                                            "c.Monday, " + 
                                            "c.Tuesday, " + 
                                            "c.Wednesday, " + 
                                            "c.Thursday, " + 
                                            "c.Friday, " + 
                                            "c.Saturday, " + 
                                            "c.Sunday, " + 
                                            "c.StartDate, " + 
                                            "c.EndDate " + 
                                     "FROM GTFS_STOP_TIMES s " + 
                                     "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                     "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                     "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                            $"WHERE LOWER(s.StopId) LIKE '{id.ToLower()}%' " + 
                                               "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                     "ORDER BY s.DepartureTime",

            ComparisonType.Ends => "SELECT s.DepartureTime, " + 
                                          "s.StopId, " + 
                                          "t.TripId, " + 
                                          "t.ServiceId, " + 
                                          "t.TripHeadsign, " + 
                                          "t.TripShortName, " + 
                                          "r.AgencyId, " + 
                                          "r.RouteShortName, " + 
                                          "r.RouteLongName, " + 
                                          "c.Monday, " + 
                                          "c.Tuesday, " + 
                                          "c.Wednesday, " + 
                                          "c.Thursday, " + 
                                          "c.Friday, " + 
                                          "c.Saturday, " + 
                                          "c.Sunday, " + 
                                          "c.StartDate, " + 
                                          "c.EndDate " + 
                                   "FROM GTFS_STOP_TIMES s " + 
                                   "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                   "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                   "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                          $"WHERE LOWER(s.StopId) LIKE '%{id.ToLower()}' " + 
                                             "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                   "ORDER BY s.DepartureTime",

            _ => "SELECT s.DepartureTime, " + 
                        "s.StopId, " + 
                        "t.TripId, " + 
                        "t.ServiceId, " + 
                        "t.TripHeadsign, " + 
                        "t.TripShortName, " + 
                        "r.AgencyId, " + 
                        "r.RouteShortName, " + 
                        "r.RouteLongName, " + 
                        "c.Monday, " + 
                        "c.Tuesday, " + 
                        "c.Wednesday, " + 
                        "c.Thursday, " + 
                        "c.Friday, " + 
                        "c.Saturday, " + 
                        "c.Sunday, " + 
                        "c.StartDate, " + 
                        "c.EndDate " + 
                 "FROM GTFS_STOP_TIMES s " + 
                 "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                 "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                 "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                        $"WHERE LOWER(s.StopId) = '{id.ToLower()}' " + 
                           "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                 "ORDER BY s.DepartureTime"
        };
        
        return ExecuteCommand(sql, GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Partial => "SELECT s.DepartureTime, " + 
                                             "s.StopId, " + 
                                             "t.TripId, " + 
                                             "t.ServiceId, " + 
                                             "t.TripHeadsign, " + 
                                             "t.TripShortName, " + 
                                             "r.AgencyId, " + 
                                             "r.RouteShortName, " + 
                                             "r.RouteLongName, " + 
                                             "c.Monday, " + 
                                             "c.Tuesday, " + 
                                             "c.Wednesday, " + 
                                             "c.Thursday, " + 
                                             "c.Friday, " + 
                                             "c.Saturday, " + 
                                             "c.Sunday, " + 
                                             "c.StartDate, " + 
                                             "c.EndDate " + 
                                      "FROM GTFS_STOP_TIMES s " + 
                                      "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                      "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                      "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                             $"WHERE LOWER(s.TripId) LIKE '%{id.ToLower()}%' " + 
                                                "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                      "ORDER BY s.DepartureTime",

            ComparisonType.Starts => "SELECT s.DepartureTime, " + 
                                            "s.StopId, " + 
                                            "t.TripId, " + 
                                            "t.ServiceId, " + 
                                            "t.TripHeadsign, " + 
                                            "t.TripShortName, " + 
                                            "r.AgencyId, " + 
                                            "r.RouteShortName, " + 
                                            "r.RouteLongName, " + 
                                            "c.Monday, " + 
                                            "c.Tuesday, " + 
                                            "c.Wednesday, " + 
                                            "c.Thursday, " + 
                                            "c.Friday, " + 
                                            "c.Saturday, " + 
                                            "c.Sunday, " + 
                                            "c.StartDate, " + 
                                            "c.EndDate " + 
                                     "FROM GTFS_STOP_TIMES s " + 
                                     "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                     "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                     "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                            $"WHERE LOWER(s.TripId) LIKE '{id.ToLower()}%' " + 
                                               "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                     "ORDER BY s.DepartureTime",

            ComparisonType.Ends => "SELECT s.DepartureTime, " + 
                                          "s.StopId, " + 
                                          "t.TripId, " + 
                                          "t.ServiceId, " + 
                                          "t.TripHeadsign, " + 
                                          "t.TripShortName, " + 
                                          "r.AgencyId, " + 
                                          "r.RouteShortName, " + 
                                          "r.RouteLongName, " + 
                                          "c.Monday, " + 
                                          "c.Tuesday, " + 
                                          "c.Wednesday, " + 
                                          "c.Thursday, " + 
                                          "c.Friday, " + 
                                          "c.Saturday, " + 
                                          "c.Sunday, " + 
                                          "c.StartDate, " + 
                                          "c.EndDate " + 
                                   "FROM GTFS_STOP_TIMES s " + 
                                   "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                                   "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                                   "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                          $"WHERE LOWER(s.TripId) LIKE '%{id.ToLower()}' " + 
                                             "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                   "ORDER BY s.DepartureTime",

            _ => "SELECT s.DepartureTime, " + 
                        "s.StopId, " + 
                        "t.TripId, " + 
                        "t.ServiceId, " + 
                        "t.TripHeadsign, " + 
                        "t.TripShortName, " + 
                        "r.AgencyId, " + 
                        "r.RouteShortName, " + 
                        "r.RouteLongName, " + 
                        "c.Monday, " + 
                        "c.Tuesday, " + 
                        "c.Wednesday, " + 
                        "c.Thursday, " + 
                        "c.Friday, " + 
                        "c.Saturday, " + 
                        "c.Sunday, " + 
                        "c.StartDate, " + 
                        "c.EndDate " + 
                 "FROM GTFS_STOP_TIMES s " + 
                 "LEFT JOIN GTFS_TRIPS t ON (s.TripId = t.TripId) " + 
                 "LEFT JOIN GTFS_ROUTES r ON (t.RouteId = r.RouteId) " + 
                 "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                        $"WHERE LOWER(s.TripId) = '{id.ToLower()}' " + 
                           "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                 "ORDER BY s.DepartureTime"
        };
        
        return ExecuteCommand(sql, GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopCode, '')) = '{code.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '{code.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '%{code.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '%{code.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) = '{description.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '{description.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '%{description.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '%{description.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(StopId) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE StopLon >= {minimumLongitude} " + 
                                          $"AND StopLat >= {minimumLatitude} " + 
                                          $"AND StopLon <= {maximumLongitude} " + 
                                          $"AND StopLat <= {maximumLatitude}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE StopLon >= {minimumLongitude} " + 
                                          $"AND StopLat >= {minimumLatitude} " + 
                                          $"AND StopLon <= {maximumLongitude} " + 
                                          $"AND StopLat <= {maximumLatitude}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE StopLon >= {minimumLongitude} " + 
                                          $"AND StopLat >= {minimumLatitude} " + 
                                          $"AND StopLon <= {maximumLongitude} " + 
                                          $"AND StopLat <= {maximumLatitude}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE StopLon >= {minimumLongitude} " + 
                      $"AND StopLat >= {minimumLatitude} " + 
                      $"AND StopLon <= {maximumLongitude} " + 
                      $"AND StopLat <= {maximumLatitude}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopName, '')) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopName, '')) LIKE '{name.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopName, '')) LIKE '%{name.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(StopName, '')) LIKE '%{name.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) = '{platformCode.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '{platformCode.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{platformCode.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{platformCode.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopName, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) = '{search.ToLower()}'",

            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopName, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '{search.ToLower()}%'",

            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(StopId) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopName, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '%{search.ToLower()}'",

            _ => "SELECT * " +
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(StopId) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopCode, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopName, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopDesc, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(ZoneId, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopUrl, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(LevelId, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '%{search.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '%{timezone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '%{timezone.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '{url.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '%{url.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '%{url.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOPS " + 
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOPS " + 
                    $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
}