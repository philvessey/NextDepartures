using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;
using Npgsql;

namespace NextDepartures.Storage.Postgres.Aspire;

public class PostgresStorage : IDataStorage
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _connectionString;
    
    private PostgresStorage(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    private PostgresStorage(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    /// <summary>
    /// Loads a Postgres data storage.
    /// </summary>
    /// <param name="dataSource">The database data source.</param>
    public static PostgresStorage Load(NpgsqlDataSource dataSource)
    {
        return new PostgresStorage(dataSource: dataSource);
    }
    
    /// <summary>
    /// Loads a Postgres data storage.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    public static PostgresStorage Load(string connectionString)
    {
        return new PostgresStorage(connectionString: connectionString);
    }
    
    private async Task<List<T>> ExecuteCommandAsync<T>(
        string sql,
        Func<NpgsqlDataReader, T> entryProcessor) where T : class {
        
        List<T> results = [];
        
        await using var connection = _dataSource is not null
            ? _dataSource.CreateConnection()
            : new NpgsqlConnection(connectionString: _connectionString);
        
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand();
        command.CommandText = sql;
        command.CommandTimeout = 0;
        command.CommandType = CommandType.Text;
        command.Connection = connection;
        
        await using var dataReader = await command.ExecuteReaderAsync();
        
        while (await dataReader.ReadAsync())
            results.Add(item: entryProcessor(dataReader));
        
        return results;
    }
    
    private static Agency GetAgencyFromDataReader(NpgsqlDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(ordinal: 0)
                ? dataReader.GetString(ordinal: 0)
                : null,
            
            Name = dataReader.GetString(ordinal: 1),
            URL = dataReader.GetString(ordinal: 2),
            Timezone = dataReader.GetString(ordinal: 3),
            
            LanguageCode = !dataReader.IsDBNull(ordinal: 4)
                ? dataReader.GetString(ordinal: 4)
                : null,
            
            Phone = !dataReader.IsDBNull(ordinal: 5)
                ? dataReader.GetString(ordinal: 5)
                : null,
            
            FareURL = !dataReader.IsDBNull(ordinal: 6)
                ? dataReader.GetString(ordinal: 6)
                : null,
            
            Email = !dataReader.IsDBNull(ordinal: 7)
                ? dataReader.GetString(ordinal: 7)
                : null
        };
    }
    
    private static Agency GetAgencyFromDataReaderByCondition(NpgsqlDataReader dataReader)
    {
        return new Agency
        {
            Id = !dataReader.IsDBNull(ordinal: 0)
                ? dataReader.GetString(ordinal: 0)
                : null,
            
            Name = dataReader.GetString(ordinal: 1),
            URL = dataReader.GetString(ordinal: 2),
            Timezone = dataReader.GetString(ordinal: 3),
            
            LanguageCode = !dataReader.IsDBNull(ordinal: 4)
                ? dataReader.GetString(ordinal: 4)
                : null,
            
            Phone = !dataReader.IsDBNull(ordinal: 5)
                ? dataReader.GetString(ordinal: 5)
                : null,
            
            FareURL = !dataReader.IsDBNull(ordinal: 6)
                ? dataReader.GetString(ordinal: 6)
                : null,
            
            Email = !dataReader.IsDBNull(ordinal: 7)
                ? dataReader.GetString(ordinal: 7)
                : null
        };
    }
    
    private static CalendarDate GetCalendarDateFromDataReader(NpgsqlDataReader dataReader)
    {
        return new CalendarDate
        {
            ServiceId = dataReader.GetString(ordinal: 0),
            Date = dataReader.GetDateTime(ordinal: 1),
            ExceptionType = dataReader.GetInt16(ordinal: 2).ToExceptionType()
        };
    }
    
    private static Departure GetDepartureFromDataReaderByCondition(NpgsqlDataReader dataReader)
    {
        return new Departure
        {
            DepartureTime = new TimeOfDay
            {
                Hours = !dataReader.IsDBNull(ordinal: 0)
                    ? dataReader.GetString(ordinal: 0).ToTimeOfDay().Hours
                    : 0,
                
                Minutes = !dataReader.IsDBNull(ordinal: 0)
                    ? dataReader.GetString(ordinal: 0).ToTimeOfDay().Minutes
                    : 0,
                
                Seconds = !dataReader.IsDBNull(ordinal: 0)
                    ? dataReader.GetString(ordinal: 0).ToTimeOfDay().Seconds
                    : 0
            },
            
            StopId = !dataReader.IsDBNull(ordinal: 1)
                ? dataReader.GetString(ordinal: 1)
                : null,
            
            StopSequence = dataReader.GetInt16(ordinal: 2),
            TripId = dataReader.GetString(ordinal: 3),
            ServiceId = dataReader.GetString(ordinal: 4),
            
            TripHeadsign = !dataReader.IsDBNull(ordinal: 5)
                ? dataReader.GetString(ordinal: 5)
                : null,
            
            TripShortName = !dataReader.IsDBNull(ordinal: 6)
                ? dataReader.GetString(ordinal: 6)
                : null,
            
            AgencyId = !dataReader.IsDBNull(ordinal: 7)
                ? dataReader.GetString(ordinal: 7)
                : null,
            
            RouteShortName = !dataReader.IsDBNull(ordinal: 8)
                ? dataReader.GetString(ordinal: 8)
                : null,
            
            RouteLongName = !dataReader.IsDBNull(ordinal: 9)
                ? dataReader.GetString(ordinal: 9)
                : null,
            
            Monday = dataReader.GetInt16(ordinal: 10).ToBool(),
            Tuesday = dataReader.GetInt16(ordinal: 11).ToBool(),
            Wednesday = dataReader.GetInt16(ordinal: 12).ToBool(),
            Thursday = dataReader.GetInt16(ordinal: 13).ToBool(),
            Friday = dataReader.GetInt16(ordinal: 14).ToBool(),
            Saturday = dataReader.GetInt16(ordinal: 15).ToBool(),
            Sunday = dataReader.GetInt16(ordinal: 16).ToBool(),
            StartDate = dataReader.GetDateTime(ordinal: 17),
            EndDate = dataReader.GetDateTime(ordinal: 18)
        };
    }
    
    private static Stop GetStopFromDataReader(NpgsqlDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(ordinal: 0),
            
            Code = !dataReader.IsDBNull(ordinal: 1)
                ? dataReader.GetString(ordinal: 1)
                : null,
            
            Name = !dataReader.IsDBNull(ordinal: 2)
                ? dataReader.GetString(ordinal: 2)
                : null,
            
            Description = !dataReader.IsDBNull(ordinal: 3)
                ? dataReader.GetString(ordinal: 3)
                : null,
            
            Latitude = !dataReader.IsDBNull(ordinal: 4)
                ? dataReader.GetDouble(ordinal: 4)
                : 0,
            
            Longitude = !dataReader.IsDBNull(ordinal: 5)
                ? dataReader.GetDouble(ordinal: 5)
                : 0,
            
            Zone = !dataReader.IsDBNull(ordinal: 6)
                ? dataReader.GetString(ordinal: 6)
                : null,
            
            Url = !dataReader.IsDBNull(ordinal: 7)
                ? dataReader.GetString(ordinal: 7)
                : null,
            
            LocationType = !dataReader.IsDBNull(ordinal: 8)
                ? dataReader.GetString(ordinal: 8).ToLocationType()
                : null,
            
            ParentStation = !dataReader.IsDBNull(ordinal: 9)
                ? dataReader.GetString(ordinal: 9)
                : null,
            
            Timezone = !dataReader.IsDBNull(ordinal: 10)
                ? dataReader.GetString(ordinal: 10)
                : null,
            
            WheelchairBoarding = !dataReader.IsDBNull(ordinal: 11)
                ? dataReader.GetString(ordinal: 11)
                : null,
            
            LevelId = !dataReader.IsDBNull(ordinal: 12)
                ? dataReader.GetString(ordinal: 12)
                : null,
            
            PlatformCode = !dataReader.IsDBNull(ordinal: 13)
                ? dataReader.GetString(ordinal: 13)
                : null
        };
    }
    
    private static Stop GetStopFromDataReaderByCondition(NpgsqlDataReader dataReader)
    {
        return new Stop
        {
            Id = dataReader.GetString(ordinal: 0),
            
            Code = !dataReader.IsDBNull(ordinal: 1)
                ? dataReader.GetString(ordinal: 1)
                : null,
            
            Name = !dataReader.IsDBNull(ordinal: 2)
                ? dataReader.GetString(ordinal: 2)
                : null,
            
            Description = !dataReader.IsDBNull(ordinal: 3)
                ? dataReader.GetString(ordinal: 3)
                : null,
            
            Latitude = !dataReader.IsDBNull(ordinal: 4)
                ? dataReader.GetDouble(ordinal: 4)
                : 0,
            
            Longitude = !dataReader.IsDBNull(ordinal: 5)
                ? dataReader.GetDouble(ordinal: 5)
                : 0,
            
            Zone = !dataReader.IsDBNull(ordinal: 6)
                ? dataReader.GetString(ordinal: 6)
                : null,
            
            Url = !dataReader.IsDBNull(ordinal: 7)
                ? dataReader.GetString(ordinal: 7)
                : null,
            
            LocationType = !dataReader.IsDBNull(ordinal: 8)
                ? dataReader.GetString(ordinal: 8).ToLocationType()
                : null,
            
            ParentStation = !dataReader.IsDBNull(ordinal: 9)
                ? dataReader.GetString(ordinal: 9)
                : null,
            
            Timezone = !dataReader.IsDBNull(ordinal: 10)
                ? dataReader.GetString(ordinal: 10)
                : null,
            
            WheelchairBoarding = !dataReader.IsDBNull(ordinal: 11)
                ? dataReader.GetString(ordinal: 11)
                : null,
            
            LevelId = !dataReader.IsDBNull(ordinal: 12)
                ? dataReader.GetString(ordinal: 12)
                : null,
            
            PlatformCode = !dataReader.IsDBNull(ordinal: 13)
                ? dataReader.GetString(ordinal: 13)
                : null
        };
    }
    
    public Task<List<Agency>> GetAgenciesAsync()
    {
        const string sql = "select * " +
                           "from gtfs_agency";
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReader);
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        const string sql = "select * " +
                           "from gtfs_calendar_dates";
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetCalendarDateFromDataReader);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        const string sql = "select * " +
                           "from gtfs_stops";
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReader);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(
        string email,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(agency_email, '')) = '{(email ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(agency_email, '')) like '{(email ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(agency_email, '')) like '%{(email ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(coalesce(agency_email, '')) like '%{(email ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(
        string fareUrl,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(agency_fare_url, '')) = '{(fareUrl ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(agency_fare_url, '')) like '{(fareUrl ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(agency_fare_url, '')) like '%{(fareUrl ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(coalesce(agency_fare_url, '')) like '%{(fareUrl ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(agency_id, '')) = '{(id ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(agency_id, '')) like '{(id ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(agency_id, '')) like '%{(id ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(coalesce(agency_id, '')) like '%{(id ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(
        string languageCode,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(agency_lang, '')) = '{(languageCode ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(agency_lang, '')) like '{(languageCode ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(agency_lang, '')) like '%{(languageCode ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(coalesce(agency_lang, '')) like '%{(languageCode ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(
        string name,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(agency_name) = '{(name ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(agency_name) like '{(name ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(agency_name) like '%{(name ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(agency_name) like '%{(name ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(
        string phone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(agency_phone, '')) = '{(phone ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(agency_phone, '')) like '{(phone ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(agency_phone, '')) like '%{(phone ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(coalesce(agency_phone, '')) like '%{(phone ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(agency_name) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(agency_url) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(agency_timezone) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_id, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_lang, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_phone, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_fare_url, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_email, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(agency_name) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(agency_url) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(agency_timezone) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(agency_id, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(agency_lang, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(agency_phone, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(agency_fare_url, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(agency_email, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(agency_name) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(agency_url) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(agency_timezone) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_id, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_lang, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_phone, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_fare_url, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(agency_email, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(agency_name) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(agency_url) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(agency_timezone) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(agency_id, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(agency_lang, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(agency_phone, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(agency_fare_url, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(agency_email, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(agency_timezone) = '{(timezone ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(agency_timezone) like '{(timezone ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(agency_timezone) like '%{(timezone ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(agency_timezone) like '%{(timezone ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_agency " +
                                        $"where lower(agency_url) = '{(url ?? string.Empty).ToLower()}' " +
                                    "order by agency_name",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_agency " +
                                        $"where lower(agency_url) like '{(url ?? string.Empty).ToLower()}%' " +
                                     "order by agency_name",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " +
                                        $"where lower(agency_url) like '%{(url ?? string.Empty).ToLower()}' " +
                                   "order by agency_name",
            
            _ => "select * " +
                 "from gtfs_agency " +
                    $"where lower(agency_url) like '%{(url ?? string.Empty).ToLower()}%' " +
                 "order by agency_name"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Partial => "select s.departure_time, " +
                                             "s.stop_id, " +
                                             "s.stop_sequence, " +
                                             "t.trip_id, " +
                                             "t.service_id, " +
                                             "t.trip_headsign, " +
                                             "t.trip_short_name, " +
                                             "r.agency_id, " +
                                             "r.route_short_name, " +
                                             "r.route_long_name, " +
                                             "c.monday, " +
                                             "c.tuesday, " +
                                             "c.wednesday, " +
                                             "c.thursday, " +
                                             "c.friday, " +
                                             "c.saturday, " +
                                             "c.sunday, " +
                                             "c.start_date, " +
                                             "c.end_date " +
                                      "from gtfs_stop_times s " +
                                      "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                      "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                      "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                             $"where lower(s.stop_id) like '%{id.ToLower()}%' " +
                                                "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                      "order by s.departure_time",
            
            ComparisonType.Starts => "select s.departure_time, " +
                                            "s.stop_id, " +
                                            "s.stop_sequence, " +
                                            "t.trip_id, " +
                                            "t.service_id, " +
                                            "t.trip_headsign, " +
                                            "t.trip_short_name, " +
                                            "r.agency_id, " +
                                            "r.route_short_name, " +
                                            "r.route_long_name, " +
                                            "c.monday, " +
                                            "c.tuesday, " +
                                            "c.wednesday, " +
                                            "c.thursday, " +
                                            "c.friday, " +
                                            "c.saturday, " +
                                            "c.sunday, " +
                                            "c.start_date, " +
                                            "c.end_date " +
                                     "from gtfs_stop_times s " +
                                     "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                     "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                     "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                            $"where lower(s.stop_id) like '{id.ToLower()}%' " +
                                               "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                     "order by s.departure_time",
            
            ComparisonType.Ends => "select s.departure_time, " +
                                          "s.stop_id, " +
                                          "s.stop_sequence, " +
                                          "t.trip_id, " +
                                          "t.service_id, " +
                                          "t.trip_headsign, " +
                                          "t.trip_short_name, " +
                                          "r.agency_id, " +
                                          "r.route_short_name, " +
                                          "r.route_long_name, " +
                                          "c.monday, " +
                                          "c.tuesday, " +
                                          "c.wednesday, " +
                                          "c.thursday, " +
                                          "c.friday, " +
                                          "c.saturday, " +
                                          "c.sunday, " +
                                          "c.start_date, " +
                                          "c.end_date " +
                                   "from gtfs_stop_times s " +
                                   "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                   "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                   "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                          $"where lower(s.stop_id) like '%{id.ToLower()}' " +
                                             "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                   "order by s.departure_time",
            
            _ => "select s.departure_time, " +
                        "s.stop_id, " +
                        "s.stop_sequence, " +
                        "t.trip_id, " +
                        "t.service_id, " +
                        "t.trip_headsign, " +
                        "t.trip_short_name, " +
                        "r.agency_id, " +
                        "r.route_short_name, " +
                        "r.route_long_name, " +
                        "c.monday, " +
                        "c.tuesday, " +
                        "c.wednesday, " +
                        "c.thursday, " +
                        "c.friday, " +
                        "c.saturday, " +
                        "c.sunday, " +
                        "c.start_date, " +
                        "c.end_date " +
                 "from gtfs_stop_times s " +
                 "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                 "left join gtfs_routes r on (t.route_id = r.route_id) " +
                 "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                        $"where lower(s.stop_id) = '{id.ToLower()}' " +
                           "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                 "order by s.departure_time"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Partial => "select s.departure_time, " +
                                             "s.stop_id, " +
                                             "s.stop_sequence, " +
                                             "t.trip_id, " +
                                             "t.service_id, " +
                                             "t.trip_headsign, " +
                                             "t.trip_short_name, " +
                                             "r.agency_id, " +
                                             "r.route_short_name, " +
                                             "r.route_long_name, " +
                                             "c.monday, " +
                                             "c.tuesday, " +
                                             "c.wednesday, " +
                                             "c.thursday, " +
                                             "c.friday, " +
                                             "c.saturday, " +
                                             "c.sunday, " +
                                             "c.start_date, " +
                                             "c.end_date " +
                                      "from gtfs_stop_times s " +
                                      "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                      "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                      "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                             $"where lower(s.trip_id) like '%{id.ToLower()}%' " +
                                                "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                      "order by s.departure_time",
            
            ComparisonType.Starts => "select s.departure_time, " +
                                            "s.stop_id, " +
                                            "s.stop_sequence, " +
                                            "t.trip_id, " +
                                            "t.service_id, " +
                                            "t.trip_headsign, " +
                                            "t.trip_short_name, " +
                                            "r.agency_id, " +
                                            "r.route_short_name, " +
                                            "r.route_long_name, " +
                                            "c.monday, " +
                                            "c.tuesday, " +
                                            "c.wednesday, " +
                                            "c.thursday, " +
                                            "c.friday, " +
                                            "c.saturday, " +
                                            "c.sunday, " +
                                            "c.start_date, " +
                                            "c.end_date " +
                                     "from gtfs_stop_times s " +
                                     "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                     "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                     "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                            $"where lower(s.trip_id) like '{id.ToLower()}%' " +
                                               "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                     "order by s.departure_time",
            
            ComparisonType.Ends => "select s.departure_time, " +
                                          "s.stop_id, " +
                                          "s.stop_sequence, " +
                                          "t.trip_id, " +
                                          "t.service_id, " +
                                          "t.trip_headsign, " +
                                          "t.trip_short_name, " +
                                          "r.agency_id, " +
                                          "r.route_short_name, " +
                                          "r.route_long_name, " +
                                          "c.monday, " +
                                          "c.tuesday, " +
                                          "c.wednesday, " +
                                          "c.thursday, " +
                                          "c.friday, " +
                                          "c.saturday, " +
                                          "c.sunday, " +
                                          "c.start_date, " +
                                          "c.end_date " +
                                   "from gtfs_stop_times s " +
                                   "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                                   "left join gtfs_routes r on (t.route_id = r.route_id) " +
                                   "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                                          $"where lower(s.trip_id) like '%{id.ToLower()}' " +
                                             "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                                   "order by s.departure_time",
            
            _ => "select s.departure_time, " +
                        "s.stop_id, " +
                        "s.stop_sequence, " +
                        "t.trip_id, " +
                        "t.service_id, " +
                        "t.trip_headsign, " +
                        "t.trip_short_name, " +
                        "r.agency_id, " +
                        "r.route_short_name, " +
                        "r.route_long_name, " +
                        "c.monday, " +
                        "c.tuesday, " +
                        "c.wednesday, " +
                        "c.thursday, " +
                        "c.friday, " +
                        "c.saturday, " +
                        "c.sunday, " +
                        "c.start_date, " +
                        "c.end_date " +
                 "from gtfs_stop_times s " +
                 "left join gtfs_trips t on (s.trip_id = t.trip_id) " +
                 "left join gtfs_routes r on (t.route_id = r.route_id) " +
                 "left join gtfs_calendar c on (t.service_id = c.service_id) " +
                        $"where lower(s.trip_id) = '{id.ToLower()}' " +
                           "and coalesce(nullif(s.pickup_type, ''), '0') <> '1' " +
                 "order by s.departure_time"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(
        string code,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(stop_code, '')) = '{(code ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(stop_code, '')) like '{(code ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(stop_code, '')) like '%{(code ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(stop_code, '')) like '%{(code ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(
        string description,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(stop_desc, '')) = '{(description ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(stop_desc, '')) like '{(description ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(stop_desc, '')) like '%{(description ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(stop_desc, '')) like '%{(description ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(stop_id) = '{(id ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(stop_id) like '{(id ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(stop_id) like '%{(id ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(stop_id) like '%{(id ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(level_id, '')) = '{(id ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(level_id, '')) like '{(id ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(level_id, '')) like '%{(id ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(level_id, '')) like '%{(id ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(
        double minimumLongitude,
        double minimumLatitude,
        double maximumLongitude,
        double maximumLatitude,
        ComparisonType comparison) {
        
        var longitude = (minimumLongitude + maximumLongitude) / 2;
        var latitude = (minimumLatitude + maximumLatitude) / 2;
        
        var sql = comparison switch
        {
            _ => $"select *, " +
                    $"get_from_point({longitude}, {latitude}, coalesce(stop_lon, 0), coalesce(stop_lat, 0)) AS distance " +
                 $"from gtfs_stops " +
                    $"where coalesce(stop_lon, 0) >= {minimumLongitude} " +
                      $"and coalesce(stop_lat, 0) >= {minimumLatitude} " +
                      $"and coalesce(stop_lon, 0) <= {maximumLongitude} " +
                      $"and coalesce(stop_lat, 0) <= {maximumLatitude} " +
                 $"order by distance"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(
        LocationType locationType,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where coalesce(nullif(location_type, ''), '0') = '{locationType.ToInt32()}' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(
        string name,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(stop_name, '')) = '{(name ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(stop_name, '')) like '{(name ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(stop_name, '')) like '%{(name ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(stop_name, '')) like '%{(name ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(parent_station, '')) = '{(id ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(parent_station, '')) like '{(id ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(parent_station, '')) like '%{(id ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(parent_station, '')) like '%{(id ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(
        string platformCode,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(platform_code, '')) = '{(platformCode ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(platform_code, '')) like '{(platformCode ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(platform_code, '')) like '%{(platformCode ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(platform_code, '')) like '%{(platformCode ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPointAsync(
        double longitude,
        double latitude,
        double distance,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => $"select *, " +
                    $"get_from_point({longitude}, {latitude}, coalesce(stop_lon, 0), coalesce(stop_lat, 0)) AS distance " +
                 $"from gtfs_stops " +
                    $"where get_from_point({longitude}, {latitude}, coalesce(stop_lon, 0), coalesce(stop_lat, 0)) <= {distance} " +
                 $"order by distance"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(stop_id) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_code, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_name, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_desc, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(zone_id, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_url, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(parent_station, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_timezone, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(level_id, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(platform_code, '')) = '{(search ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(stop_id) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(stop_code, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(stop_name, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(stop_desc, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(zone_id, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(stop_url, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(parent_station, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(stop_timezone, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(level_id, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                           $"or lower(coalesce(platform_code, '')) like '{(search ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(stop_id) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_code, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_name, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_desc, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(zone_id, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_url, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(parent_station, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(stop_timezone, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(level_id, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                           $"or lower(coalesce(platform_code, '')) like '%{(search ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(stop_id) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(stop_code, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(stop_name, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(stop_desc, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(zone_id, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(stop_url, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(parent_station, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(stop_timezone, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(level_id, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                       $"or lower(coalesce(platform_code, '')) like '%{(search ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(stop_timezone, '')) = '{(timezone ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(stop_timezone, '')) like '{(timezone ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(stop_timezone, '')) like '%{(timezone ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(stop_timezone, '')) like '%{(timezone ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(stop_url, '')) = '{(url ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(stop_url, '')) like '{(url ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(stop_url, '')) like '%{(url ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(stop_url, '')) like '%{(url ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(
        WheelchairAccessibilityType wheelchairBoarding,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where coalesce(nullif(wheelchair_boarding, ''), '0') = '{wheelchairBoarding.ToInt32()}' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(
        string id,
        ComparisonType comparison) {
        
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stops " +
                                        $"where lower(coalesce(zone_id, '')) = '{(id ?? string.Empty).ToLower()}' " +
                                    "order by stop_name, stop_id",
            
            ComparisonType.Starts => "select * " +
                                     "from gtfs_stops " +
                                        $"where lower(coalesce(zone_id, '')) like '{(id ?? string.Empty).ToLower()}%' " +
                                     "order by stop_name, stop_id",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_stops " +
                                        $"where lower(coalesce(zone_id, '')) like '%{(id ?? string.Empty).ToLower()}' " +
                                   "order by stop_name, stop_id",
            
            _ => "select * " +
                 "from gtfs_stops " +
                    $"where lower(coalesce(zone_id, '')) like '%{(id ?? string.Empty).ToLower()}%' " +
                 "order by stop_name, stop_id"
        };
        
        return ExecuteCommandAsync(
            sql: sql,
            entryProcessor: GetStopFromDataReaderByCondition);
    }
}