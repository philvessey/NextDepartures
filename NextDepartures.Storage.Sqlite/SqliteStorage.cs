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
            Longitude = dataReader.GetDouble(4),
            Latitude = dataReader.GetDouble(5),
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
            Longitude = dataReader.GetDouble(4),
            Latitude = dataReader.GetDouble(5),
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
                           "FROM GTFS_CALENDAR_DATE";
        
        return ExecuteCommand(sql, GetCalendarDateFromDataReader);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        const string sql = "SELECT * " + 
                           "FROM GTFS_STOP";
        
        return ExecuteCommand(sql, GetStopFromDataReader);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Email, '')) = '{email.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Email, '')) LIKE '{email.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Email, '')) LIKE '%{email.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(Email, '')) LIKE '%{email.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(FareURL, '')) = '{fareUrl.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(FareURL, '')) LIKE '{fareUrl.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(FareURL, '')) LIKE '%{fareUrl.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(FareURL, '')) LIKE '%{fareUrl.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Id, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Id, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(Id, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(Id, '')) LIKE '%{id.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(LanguageCode, '')) = '{languageCode.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(LanguageCode, '')) LIKE '{languageCode.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(LanguageCode, '')) LIKE '%{languageCode.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(COALESCE(LanguageCode, '')) LIKE '%{languageCode.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) LIKE '{name.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) LIKE '%{name.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(Name) LIKE '%{name.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(Phone, '')) = '{phone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(Phone, '')) LIKE '{phone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(COALESCE(Phone, '')) LIKE '%{phone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(COALESCE(Phone, '')) LIKE '%{phone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) = '{search.ToLower()}' " + 
                                           $"OR LOWER(URL) = '{search.ToLower()}' " + 
                                           $"OR LOWER(Timezone) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Id, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(LanguageCode, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Phone, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(FareURL, '')) = '{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Email, '')) = '{search.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(URL) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(Timezone) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(Id, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(LanguageCode, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(Phone, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(FareURL, '')) LIKE '{search.ToLower()}%' " + 
                                           $"OR LOWER(COALESCE(Email, '')) LIKE '{search.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Name) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(URL) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(Timezone) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Id, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(LanguageCode, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Phone, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(FareURL, '')) LIKE '%{search.ToLower()}' " + 
                                           $"OR LOWER(COALESCE(Email, '')) LIKE '%{search.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(Name) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(URL) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(Timezone) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(Id, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(LanguageCode, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(Phone, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(FareURL, '')) LIKE '%{search.ToLower()}%' " + 
                       $"OR LOWER(COALESCE(Email, '')) LIKE '%{search.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Timezone) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Timezone) LIKE '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(Timezone) LIKE '%{timezone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(Timezone) LIKE '%{timezone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(URL) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(URL) LIKE '{url.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_AGENCY " + 
                                        $"WHERE LOWER(URL) LIKE '%{url.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_AGENCY " + 
                    $"WHERE LOWER(URL) LIKE '%{url.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Partial => "SELECT s.DepartureTime, " + 
                                             "s.StopId, " + 
                                             "t.Id, " + 
                                             "t.ServiceId, " + 
                                             "t.Headsign, " + 
                                             "t.ShortName, " + 
                                             "r.AgencyId, " + 
                                             "r.ShortName, " + 
                                             "r.LongName, " + 
                                             "c.Monday, " + 
                                             "c.Tuesday, " + 
                                             "c.Wednesday, " + 
                                             "c.Thursday, " + 
                                             "c.Friday, " + 
                                             "c.Saturday, " + 
                                             "c.Sunday, " + 
                                             "c.StartDate, " + 
                                             "c.EndDate " + 
                                      "FROM GTFS_STOP_TIME s " + 
                                      "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                      "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                      "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                             $"WHERE LOWER(s.StopId) LIKE '%{id.ToLower()}%' " + 
                                                "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                      "ORDER BY s.DepartureTime",

            ComparisonType.Starts => "SELECT s.DepartureTime, " + 
                                            "s.StopId, " + 
                                            "t.Id, " + 
                                            "t.ServiceId, " + 
                                            "t.Headsign, " + 
                                            "t.ShortName, " + 
                                            "r.AgencyId, " + 
                                            "r.ShortName, " + 
                                            "r.LongName, " + 
                                            "c.Monday, " + 
                                            "c.Tuesday, " + 
                                            "c.Wednesday, " + 
                                            "c.Thursday, " + 
                                            "c.Friday, " + 
                                            "c.Saturday, " + 
                                            "c.Sunday, " + 
                                            "c.StartDate, " + 
                                            "c.EndDate " + 
                                     "FROM GTFS_STOP_TIME s " + 
                                     "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                     "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                     "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                            $"WHERE LOWER(s.StopId) LIKE '{id.ToLower()}%' " + 
                                               "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                     "ORDER BY s.DepartureTime",

            ComparisonType.Ends => "SELECT s.DepartureTime, " + 
                                          "s.StopId, " + 
                                          "t.Id, " + 
                                          "t.ServiceId, " + 
                                          "t.Headsign, " + 
                                          "t.ShortName, " + 
                                          "r.AgencyId, " + 
                                          "r.ShortName, " + 
                                          "r.LongName, " + 
                                          "c.Monday, " + 
                                          "c.Tuesday, " + 
                                          "c.Wednesday, " + 
                                          "c.Thursday, " + 
                                          "c.Friday, " + 
                                          "c.Saturday, " + 
                                          "c.Sunday, " + 
                                          "c.StartDate, " + 
                                          "c.EndDate " + 
                                   "FROM GTFS_STOP_TIME s " + 
                                   "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                   "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                   "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                          $"WHERE LOWER(s.StopId) LIKE '%{id.ToLower()}' " + 
                                             "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                   "ORDER BY s.DepartureTime",

            _ => "SELECT s.DepartureTime, " + 
                        "s.StopId, " + 
                        "t.Id, " + 
                        "t.ServiceId, " + 
                        "t.Headsign, " + 
                        "t.ShortName, " + 
                        "r.AgencyId, " + 
                        "r.ShortName, " + 
                        "r.LongName, " + 
                        "c.Monday, " + 
                        "c.Tuesday, " + 
                        "c.Wednesday, " + 
                        "c.Thursday, " + 
                        "c.Friday, " + 
                        "c.Saturday, " + 
                        "c.Sunday, " + 
                        "c.StartDate, " + 
                        "c.EndDate " + 
                 "FROM GTFS_STOP_TIME s " + 
                 "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                 "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
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
                                             "t.Id, " + 
                                             "t.ServiceId, " + 
                                             "t.Headsign, " + 
                                             "t.ShortName, " + 
                                             "r.AgencyId, " + 
                                             "r.ShortName, " + 
                                             "r.LongName, " + 
                                             "c.Monday, " + 
                                             "c.Tuesday, " + 
                                             "c.Wednesday, " + 
                                             "c.Thursday, " + 
                                             "c.Friday, " + 
                                             "c.Saturday, " + 
                                             "c.Sunday, " + 
                                             "c.StartDate, " + 
                                             "c.EndDate " + 
                                      "FROM GTFS_STOP_TIME s " + 
                                      "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                      "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                      "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                             $"WHERE LOWER(s.TripId) LIKE '%{id.ToLower()}%' " + 
                                                "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                      "ORDER BY s.DepartureTime",

            ComparisonType.Starts => "SELECT s.DepartureTime, " + 
                                            "s.StopId, " + 
                                            "t.Id, " + 
                                            "t.ServiceId, " + 
                                            "t.Headsign, " + 
                                            "t.ShortName, " + 
                                            "r.AgencyId, " + 
                                            "r.ShortName, " + 
                                            "r.LongName, " + 
                                            "c.Monday, " + 
                                            "c.Tuesday, " + 
                                            "c.Wednesday, " + 
                                            "c.Thursday, " + 
                                            "c.Friday, " + 
                                            "c.Saturday, " + 
                                            "c.Sunday, " + 
                                            "c.StartDate, " + 
                                            "c.EndDate " + 
                                     "FROM GTFS_STOP_TIME s " + 
                                     "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                     "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                     "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                            $"WHERE LOWER(s.TripId) LIKE '{id.ToLower()}%' " + 
                                               "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                     "ORDER BY s.DepartureTime",

            ComparisonType.Ends => "SELECT s.DepartureTime, " + 
                                          "s.StopId, " + 
                                          "t.Id, " + 
                                          "t.ServiceId, " + 
                                          "t.Headsign, " + 
                                          "t.ShortName, " + 
                                          "r.AgencyId, " + 
                                          "r.ShortName, " + 
                                          "r.LongName, " + 
                                          "c.Monday, " + 
                                          "c.Tuesday, " + 
                                          "c.Wednesday, " + 
                                          "c.Thursday, " + 
                                          "c.Friday, " + 
                                          "c.Saturday, " + 
                                          "c.Sunday, " + 
                                          "c.StartDate, " + 
                                          "c.EndDate " + 
                                   "FROM GTFS_STOP_TIME s " + 
                                   "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                                   "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
                                   "LEFT JOIN GTFS_CALENDAR c ON (t.ServiceId = c.ServiceId) " + 
                                          $"WHERE LOWER(s.TripId) LIKE '%{id.ToLower()}' " + 
                                             "AND COALESCE(NULLIF(s.PickupType, ''), 0) != 1 " + 
                                   "ORDER BY s.DepartureTime",

            _ => "SELECT s.DepartureTime, " + 
                        "s.StopId, " + 
                        "t.Id, " + 
                        "t.ServiceId, " + 
                        "t.Headsign, " + 
                        "t.ShortName, " + 
                        "r.AgencyId, " + 
                        "r.ShortName, " + 
                        "r.LongName, " + 
                        "c.Monday, " + 
                        "c.Tuesday, " + 
                        "c.Wednesday, " + 
                        "c.Thursday, " + 
                        "c.Friday, " + 
                        "c.Saturday, " + 
                        "c.Sunday, " + 
                        "c.StartDate, " + 
                        "c.EndDate " + 
                 "FROM GTFS_STOP_TIME s " + 
                 "LEFT JOIN GTFS_TRIP t ON (s.TripId = t.Id) " + 
                 "LEFT JOIN GTFS_ROUTE r ON (t.RouteId = r.Id) " + 
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
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Code, '')) = '{code.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Code, '')) LIKE '{code.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Code, '')) LIKE '%{code.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Code, '')) LIKE '%{code.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Description, '')) = '{description.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Description, '')) LIKE '{description.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Description, '')) LIKE '%{description.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Description, '')) LIKE '%{description.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(Id) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE Longitude >= {minimumLongitude} " + 
                                          $"AND Latitude >= {minimumLatitude} " + 
                                          $"AND Longitude <= {maximumLongitude} " + 
                                          $"AND Latitude <= {maximumLatitude}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE Longitude >= {minimumLongitude} " + 
                                          $"AND Latitude >= {minimumLatitude} " + 
                                          $"AND Longitude <= {maximumLongitude} " + 
                                          $"AND Latitude <= {maximumLatitude}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE Longitude >= {minimumLongitude} " + 
                                          $"AND Latitude >= {minimumLatitude} " + 
                                          $"AND Longitude <= {maximumLongitude} " + 
                                          $"AND Latitude <= {maximumLatitude}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE Longitude >= {minimumLongitude} " + 
                      $"AND Latitude >= {minimumLatitude} " + 
                      $"AND Longitude <= {maximumLongitude} " + 
                      $"AND Latitude <= {maximumLatitude}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE COALESCE(NULLIF(LocationType, ''), 0) = {locationType.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Name, '')) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Name, '')) LIKE '{name.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Name, '')) LIKE '%{name.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Name, '')) LIKE '%{name.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '{id.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{id.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) = '{platformCode.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '{platformCode.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{platformCode.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{platformCode.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Code, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Name, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Description, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Zone, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Url, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Timezone, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) = '{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) = '{search.ToLower()}'",

            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Code, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Name, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Description, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Zone, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Url, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(Timezone, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '{search.ToLower()}%' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '{search.ToLower()}%'",

            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(Id) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Code, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Name, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Description, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Zone, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Url, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(Timezone, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '%{search.ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '%{search.ToLower()}'",

            _ => "SELECT * " +
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(Id) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Code, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Name, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Description, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Zone, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Url, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{search.ToLower()}%' " +
                       $"OR LOWER(COALESCE(Timezone, '')) LIKE '%{search.ToLower()}%' " +
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
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Timezone, '')) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Timezone, '')) LIKE '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Timezone, '')) LIKE '%{timezone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Timezone, '')) LIKE '%{timezone.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Url, '')) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Url, '')) LIKE '{url.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Url, '')) LIKE '%{url.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Url, '')) LIKE '%{url.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), 0) = {wheelchairBoarding.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string zone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " + 
                                    "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Zone, '')) = '{zone.ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " + 
                                     "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Zone, '')) LIKE '{zone.ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " + 
                                   "FROM GTFS_STOP " + 
                                        $"WHERE LOWER(COALESCE(Zone, '')) LIKE '%{zone.ToLower()}'",
            
            _ => "SELECT * " + 
                 "FROM GTFS_STOP " + 
                    $"WHERE LOWER(COALESCE(Zone, '')) LIKE '%{zone.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
}