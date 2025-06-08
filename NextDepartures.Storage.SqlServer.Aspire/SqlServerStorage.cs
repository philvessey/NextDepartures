using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using Microsoft.Data.SqlClient;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;

namespace NextDepartures.Storage.SqlServer.Aspire;

public class SqlServerStorage : IDataStorage
{
    private readonly SqlConnection _connection;
    private readonly string _connectionString;
    
    private SqlServerStorage(SqlConnection connection)
    {
        _connection = connection;
    }
    
    private SqlServerStorage(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    /// <summary>
    /// Loads a SQL Server data storage.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    public static SqlServerStorage Load(SqlConnection connection)
    {
        return new SqlServerStorage(connection: connection);
    }
    
    /// <summary>
    /// Loads a SQL Server data storage.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    public static SqlServerStorage Load(string connectionString)
    {
        return new SqlServerStorage(connectionString: connectionString);
    }
    
    private async Task<List<T>> ExecuteCommandAsync<T>(
        string sql,
        Func<SqlDataReader, T> entryProcessor) where T : class {
        
        List<T> results = [];
        
        await using var connection = _connection ?? new SqlConnection(connectionString: _connectionString);
        await connection.OpenAsync();
        
        SqlCommand command = new();
        command.CommandText = sql;
        command.CommandTimeout = 0;
        command.CommandType = CommandType.Text;
        command.Connection = connection;
        
        var dataReader = await command.ExecuteReaderAsync();
        
        while (await dataReader.ReadAsync())
            results.Add(item: entryProcessor(dataReader));
        
        await dataReader.CloseAsync();
        await command.DisposeAsync();
        
        return results;
    }
    
    private static Agency GetAgencyFromDataReader(SqlDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(i: 0)
                ? dataReader.GetString(i: 0)
                : null,
            
            Name = dataReader.GetString(i: 1),
            URL = dataReader.GetString(i: 2),
            Timezone = dataReader.GetString(i: 3),
            
            LanguageCode = !dataReader.IsDBNull(i: 4)
                ? dataReader.GetString(i: 4)
                : null,
            
            Phone = !dataReader.IsDBNull(i: 5)
                ? dataReader.GetString(i: 5)
                : null,
            
            FareURL = !dataReader.IsDBNull(i: 6)
                ? dataReader.GetString(i: 6)
                : null,
            
            Email = !dataReader.IsDBNull(i: 7)
                ? dataReader.GetString(i: 7)
                : null
        };
    }
    
    private static Agency GetAgencyFromDataReaderByCondition(SqlDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(i: 0)
                ? dataReader.GetString(i: 0)
                : null,
            
            Name = dataReader.GetString(i: 1),
            URL = dataReader.GetString(i: 2),
            Timezone = dataReader.GetString(i: 3),
            
            LanguageCode = !dataReader.IsDBNull(i: 4)
                ? dataReader.GetString(i: 4)
                : null,
            
            Phone = !dataReader.IsDBNull(i: 5)
                ? dataReader.GetString(i: 5)
                : null,
            
            FareURL = !dataReader.IsDBNull(i: 6)
                ? dataReader.GetString(i: 6)
                : null,
            
            Email = !dataReader.IsDBNull(i: 7)
                ? dataReader.GetString(i: 7)
                : null
        };
    }
    
    private static CalendarDate GetCalendarDateFromDataReader(SqlDataReader dataReader)
    {
        return new CalendarDate
        {
            ServiceId = dataReader.GetString(i: 0),
            Date = dataReader.GetDateTime(i: 1),
            ExceptionType = dataReader.GetInt16(i: 2).ToExceptionType()
        };
    }
    
    private static Departure GetDepartureFromDataReaderByCondition(SqlDataReader dataReader)
    {
        return new Departure
        {
            DepartureTime = new TimeOfDay
            {
                Hours = !dataReader.IsDBNull(i: 0)
                    ? dataReader.GetString(i: 0).ToTimeOfDay().Hours
                    : 0,
                
                Minutes = !dataReader.IsDBNull(i: 0)
                    ? dataReader.GetString(i: 0).ToTimeOfDay().Minutes
                    : 0,
                
                Seconds = !dataReader.IsDBNull(i: 0)
                    ? dataReader.GetString(i: 0).ToTimeOfDay().Seconds
                    : 0
            },
            
            StopId = !dataReader.IsDBNull(i: 1)
                ? dataReader.GetString(i: 1)
                : null,
            
            StopSequence = dataReader.GetInt16(i: 2),
            TripId = dataReader.GetString(i: 3),
            ServiceId = dataReader.GetString(i: 4),
            
            TripHeadsign = !dataReader.IsDBNull(i: 5)
                ? dataReader.GetString(i: 5)
                : null,
            
            TripShortName = !dataReader.IsDBNull(i: 6)
                ? dataReader.GetString(i: 6)
                : null,
            
            AgencyId = !dataReader.IsDBNull(i: 7)
                ? dataReader.GetString(i: 7)
                : null,
            
            RouteShortName = !dataReader.IsDBNull(i: 8)
                ? dataReader.GetString(i: 8)
                : null,
            
            RouteLongName = !dataReader.IsDBNull(i: 9)
                ? dataReader.GetString(i: 9)
                : null,
            
            Monday = dataReader.GetInt16(i: 10).ToBool(),
            Tuesday = dataReader.GetInt16(i: 11).ToBool(),
            Wednesday = dataReader.GetInt16(i: 12).ToBool(),
            Thursday = dataReader.GetInt16(i: 13).ToBool(),
            Friday = dataReader.GetInt16(i: 14).ToBool(),
            Saturday = dataReader.GetInt16(i: 15).ToBool(),
            Sunday = dataReader.GetInt16(i: 16).ToBool(),
            StartDate = dataReader.GetDateTime(i: 17),
            EndDate = dataReader.GetDateTime(i: 18)
        };
    }
    
    private static Stop GetStopFromDataReader(SqlDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(i: 0),
            
            Code = !dataReader.IsDBNull(i: 1)
                ? dataReader.GetString(i: 1)
                : null,
            
            Name = !dataReader.IsDBNull(i: 2)
                ? dataReader.GetString(i: 2)
                : null,
            
            Description = !dataReader.IsDBNull(i: 3)
                ? dataReader.GetString(i: 3)
                : null,
            
            Latitude = !dataReader.IsDBNull(i: 4)
                ? dataReader.GetDouble(i: 4)
                : 0,
            
            Longitude = !dataReader.IsDBNull(i: 5)
                ? dataReader.GetDouble(i: 5)
                : 0,
            
            Zone = !dataReader.IsDBNull(i: 6)
                ? dataReader.GetString(i: 6)
                : null,
            
            Url = !dataReader.IsDBNull(i: 7)
                ? dataReader.GetString(i: 7)
                : null,
            
            LocationType = !dataReader.IsDBNull(i: 8)
                ? dataReader.GetString(i: 8).ToLocationType()
                : null,
            
            ParentStation = !dataReader.IsDBNull(i: 9)
                ? dataReader.GetString(i: 9)
                : null,
            
            Timezone = !dataReader.IsDBNull(i: 10)
                ? dataReader.GetString(i: 10)
                : null,
            
            WheelchairBoarding = !dataReader.IsDBNull(i: 11)
                ? dataReader.GetString(i: 11)
                : null,
            
            LevelId = !dataReader.IsDBNull(i: 12)
                ? dataReader.GetString(i: 12)
                : null,
            
            PlatformCode = !dataReader.IsDBNull(i: 13)
                ? dataReader.GetString(i: 13)
                : null
        };
    }
    
    private static Stop GetStopFromDataReaderByCondition(SqlDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(i: 0),
            
            Code = !dataReader.IsDBNull(i: 1)
                ? dataReader.GetString(i: 1)
                : null,
            
            Name = !dataReader.IsDBNull(i: 2)
                ? dataReader.GetString(i: 2)
                : null,
            
            Description = !dataReader.IsDBNull(i: 3)
                ? dataReader.GetString(i: 3)
                : null,
            
            Latitude = !dataReader.IsDBNull(i: 4)
                ? dataReader.GetDouble(i: 4)
                : 0,
            
            Longitude = !dataReader.IsDBNull(i: 5)
                ? dataReader.GetDouble(i: 5)
                : 0,
            
            Zone = !dataReader.IsDBNull(i: 6)
                ? dataReader.GetString(i: 6)
                : null,
            
            Url = !dataReader.IsDBNull(i: 7)
                ? dataReader.GetString(i: 7)
                : null,
            
            LocationType = !dataReader.IsDBNull(i: 8)
                ? dataReader.GetString(i: 8).ToLocationType()
                : null,
            
            ParentStation = !dataReader.IsDBNull(i: 9)
                ? dataReader.GetString(i: 9)
                : null,
            
            Timezone = !dataReader.IsDBNull(i: 10)
                ? dataReader.GetString(i: 10)
                : null,
            
            WheelchairBoarding = !dataReader.IsDBNull(i: 11)
                ? dataReader.GetString(i: 11)
                : null,
            
            LevelId = !dataReader.IsDBNull(i: 12)
                ? dataReader.GetString(i: 12)
                : null,
            
            PlatformCode = !dataReader.IsDBNull(i: 13)
                ? dataReader.GetString(i: 13)
                : null
        };
    }
    
    public Task<List<Agency>> GetAgenciesAsync()
    {
        const string sql = "SELECT * " +
                           "FROM GTFS_AGENCY";
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReader);
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        const string sql = "SELECT * " +
                           "FROM GTFS_CALENDAR_DATES";
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetCalendarDateFromDataReader);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        const string sql = "SELECT * " +
                           "FROM GTFS_STOPS";
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReader);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(
        string email,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) = '{(email ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '{(email ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '%{(email ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyEmail, '')) LIKE '%{(email ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(
        string fareUrl,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) = '{(fareUrl ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '{(fareUrl ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{(fareUrl ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{(fareUrl ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) = '{(id ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '{(id ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '%{(id ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyId, '')) LIKE '%{(id ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(
        string languageCode,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) = '{(languageCode ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '{(languageCode ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '%{(languageCode ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyLang, '')) LIKE '%{(languageCode ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(
        string name,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) = '{(name ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) LIKE '{(name ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) LIKE '%{(name ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(AgencyName) LIKE '%{(name ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(
        string phone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) = '{(phone ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '{(phone ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '%{(phone ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(COALESCE(AgencyPhone, '')) LIKE '%{(phone ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(AgencyUrl) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(AgencyTimezone) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyId, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyLang, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) = '{(search ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(AgencyUrl) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(AgencyTimezone) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(AgencyId, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '{(search ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyName) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(AgencyUrl) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(AgencyTimezone) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyId, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '%{(search ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(AgencyName) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(AgencyUrl) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(AgencyTimezone) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(AgencyId, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(AgencyLang, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(AgencyPhone, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(AgencyFareUrl, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(AgencyEmail, '')) LIKE '%{(search ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyTimezone) = '{(timezone ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyTimezone) LIKE '{(timezone ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyTimezone) LIKE '%{(timezone ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(AgencyTimezone) LIKE '%{(timezone ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyUrl) = '{(url ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyUrl) LIKE '{(url ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_AGENCY " +
                                        $"WHERE LOWER(AgencyUrl) LIKE '%{(url ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_AGENCY " +
                    $"WHERE LOWER(AgencyUrl) LIKE '%{(url ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Partial => "SELECT s.DepartureTime, " +
                                             "s.StopId, " +
                                             "s.StopSequence, " +
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
                                                "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                      "ORDER BY s.DepartureTime",
            
            ComparisonType.Starts => "SELECT s.DepartureTime, " +
                                            "s.StopId, " +
                                            "s.StopSequence, " +
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
                                               "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                     "ORDER BY s.DepartureTime",
            
            ComparisonType.Ends => "SELECT s.DepartureTime, " +
                                          "s.StopId, " +
                                          "s.StopSequence, " +
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
                                             "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                   "ORDER BY s.DepartureTime",
            
            _ => "SELECT s.DepartureTime, " +
                        "s.StopId, " +
                        "s.StopSequence, " +
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
                           "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                 "ORDER BY s.DepartureTime"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Partial => "SELECT s.DepartureTime, " +
                                             "s.StopId, " +
                                             "s.StopSequence, " +
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
                                                "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                      "ORDER BY s.DepartureTime",
            
            ComparisonType.Starts => "SELECT s.DepartureTime, " +
                                            "s.StopId, " +
                                            "s.StopSequence, " +
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
                                               "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                     "ORDER BY s.DepartureTime",
            
            ComparisonType.Ends => "SELECT s.DepartureTime, " +
                                          "s.StopId, " +
                                          "s.StopSequence, " +
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
                                             "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                                   "ORDER BY s.DepartureTime",
            
            _ => "SELECT s.DepartureTime, " +
                        "s.StopId, " +
                        "s.StopSequence, " +
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
                           "AND COALESCE(NULLIF(s.PickupType, ''), '0') <> '1' " +
                 "ORDER BY s.DepartureTime"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(
        string code,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopCode, '')) = '{(code ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '{(code ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '%{(code ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(StopCode, '')) LIKE '%{(code ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(
        string description,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) = '{(description ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '{(description ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '%{(description ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(StopDesc, '')) LIKE '%{(description ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) = '{(id ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) LIKE '{(id ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) LIKE '%{(id ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(StopId) LIKE '%{(id ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(LevelId, '')) = '{(id ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '{(id ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{(id ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(LevelId, '')) LIKE '%{(id ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(
        double minimumLongitude,
        double minimumLatitude,
        double maximumLongitude,
        double maximumLatitude,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE COALESCE(StopLon, 0) >= {minimumLongitude} " +
                      $"AND COALESCE(StopLat, 0) >= {minimumLatitude} " +
                      $"AND COALESCE(StopLon, 0) <= {maximumLongitude} " +
                      $"AND COALESCE(StopLat, 0) <= {maximumLatitude}"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(
        LocationType locationType,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE COALESCE(NULLIF(LocationType, ''), '0') = '{locationType.ToInt32()}'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(
        string name,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopName, '')) = '{(name ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopName, '')) LIKE '{(name ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopName, '')) LIKE '%{(name ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(StopName, '')) LIKE '%{(name ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(
        string id, ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) = '{(id ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '{(id ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{(id ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(ParentStation, '')) LIKE '%{(id ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(
        string platformCode,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) = '{(platformCode ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '{(platformCode ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{(platformCode ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(PlatformCode, '')) LIKE '%{(platformCode ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopName, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) = '{(search ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopName, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '{(search ?? string.Empty).ToLower()}%' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '{(search ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(StopId) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopCode, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopName, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopDesc, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(ZoneId, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopUrl, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(LevelId, '')) LIKE '%{(search ?? string.Empty).ToLower()}' " +
                                           $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '%{(search ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(StopId) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopCode, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopName, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopDesc, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(ZoneId, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopUrl, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(ParentStation, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(StopTimezone, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(LevelId, '')) LIKE '%{(search ?? string.Empty).ToLower()}%' " +
                       $"OR LOWER(COALESCE(PlatformCode, '')) LIKE '%{(search ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) = '{(timezone ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '{(timezone ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '%{(timezone ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(StopTimezone, '')) LIKE '%{(timezone ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) = '{(url ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '{(url ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '%{(url ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(StopUrl, '')) LIKE '%{(url ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(
        WheelchairAccessibilityType wheelchairBoarding,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE COALESCE(NULLIF(WheelchairBoarding, ''), '0') = '{wheelchairBoarding.ToInt32()}'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "SELECT * " +
                                    "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) = '{(id ?? string.Empty).ToLower()}'",
            
            ComparisonType.Starts => "SELECT * " +
                                     "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '{(id ?? string.Empty).ToLower()}%'",
            
            ComparisonType.Ends => "SELECT * " +
                                   "FROM GTFS_STOPS " +
                                        $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '%{(id ?? string.Empty).ToLower()}'",
            
            _ => "SELECT * " +
                 "FROM GTFS_STOPS " +
                    $"WHERE LOWER(COALESCE(ZoneId, '')) LIKE '%{(id ?? string.Empty).ToLower()}%'"
        };
        
        return ExecuteCommandAsync(sql: sql, entryProcessor: GetStopFromDataReaderByCondition);
    }
}