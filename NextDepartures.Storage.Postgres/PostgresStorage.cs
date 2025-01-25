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

namespace NextDepartures.Storage.Postgres;

public class PostgresStorage : IDataStorage
{
    private readonly string _connection;
        
    private PostgresStorage(string connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Loads a Postgres data storage.
    /// </summary>
    /// <param name="connection">The database connection string.</param>
    public static PostgresStorage Load(string connection)
    {
        return new PostgresStorage(connection);
    }

    private async Task<List<T>> ExecuteCommand<T>(string sql, Func<NpgsqlDataReader, T> entryProcessor) where T : class
    {
        List<T> results = [];

        await using NpgsqlConnection connection = new(_connection);
        await connection.OpenAsync();

        NpgsqlCommand command = new(sql, connection)
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

    private static Agency GetAgencyFromDataReader(NpgsqlDataReader dataReader)
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

    private static Agency GetAgencyFromDataReaderByCondition(NpgsqlDataReader dataReader)
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
    
    private static CalendarDate GetCalendarDateFromDataReader(NpgsqlDataReader dataReader)
    {
        return new CalendarDate
        {
            ServiceId = dataReader.GetString(0),
            Date = dataReader.GetDateTime(1),
            ExceptionType = dataReader.GetInt32(2).ToExceptionType()
        };
    }
    
    private static Departure GetDepartureFromDataReaderByCondition(NpgsqlDataReader dataReader)
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
    
    private static Stop GetStopFromDataReader(NpgsqlDataReader dataReader)
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
    
    private static Stop GetStopFromDataReaderByCondition(NpgsqlDataReader dataReader)
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
        const string sql = "select * " + 
                           "from gtfs_agency";

        return ExecuteCommand(sql, GetAgencyFromDataReader);
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        const string sql = "select * " + 
                           "from gtfs_calendar_date";
        
        return ExecuteCommand(sql, GetCalendarDateFromDataReader);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        const string sql = "select * " + 
                           "from gtfs_stop";
        
        return ExecuteCommand(sql, GetStopFromDataReader);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(email, '')) = '{email.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(email, '')) like '{email.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(email, '')) like '%{email.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " +
                    $"where lower(coalesce(email, '')) like '%{email.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(fare_url, '')) = '{fareUrl.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(fare_url, '')) like '{fareUrl.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(fare_url, '')) like '%{fareUrl.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " +
                    $"where lower(coalesce(fare_url, '')) like '%{fareUrl.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " +
                                        $"where lower(coalesce(id, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " +
                                        $"where lower(coalesce(id, '')) like '{id.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " +
                                        $"where lower(coalesce(id, '')) like '%{id.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " +
                    $"where lower(coalesce(id, '')) like '%{id.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(coalesce(language_code, '')) = '{languageCode.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(coalesce(language_code, '')) like '{languageCode.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " + 
                                        $"where lower(coalesce(language_code, '')) like '%{languageCode.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(coalesce(language_code, '')) like '%{languageCode.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(name) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(name) like '{name.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " + 
                                        $"where lower(name) like '%{name.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(name) like '%{name.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(coalesce(phone, '')) = '{phone.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(coalesce(phone, '')) like '{phone.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " + 
                                        $"where lower(coalesce(phone, '')) like '%{phone.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(coalesce(phone, '')) like '%{phone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(name) = '{search.ToLower()}' " + 
                                           $"or lower(url) = '{search.ToLower()}' " + 
                                           $"or lower(timezone) = '{search.ToLower()}' " + 
                                           $"or lower(coalesce(id, '')) = '{search.ToLower()}' " + 
                                           $"or lower(coalesce(language_code, '')) = '{search.ToLower()}' " + 
                                           $"or lower(coalesce(phone, '')) = '{search.ToLower()}' " + 
                                           $"or lower(coalesce(fare_url, '')) = '{search.ToLower()}' " + 
                                           $"or lower(coalesce(email, '')) = '{search.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(name) like '{search.ToLower()}%' " + 
                                           $"or lower(url) like '{search.ToLower()}%' " + 
                                           $"or lower(timezone) like '{search.ToLower()}%' " + 
                                           $"or lower(coalesce(id, '')) like '{search.ToLower()}%' " + 
                                           $"or lower(coalesce(language_code, '')) like '{search.ToLower()}%' " + 
                                           $"or lower(coalesce(phone, '')) like '{search.ToLower()}%' " + 
                                           $"or lower(coalesce(fare_url, '')) like '{search.ToLower()}%' " + 
                                           $"or lower(coalesce(email, '')) like '{search.ToLower()}%'",
            
            ComparisonType.Ends => "select * " +
                                   "from gtfs_agency " + 
                                        $"where lower(name) like '%{search.ToLower()}' " + 
                                           $"or lower(url) like '%{search.ToLower()}' " + 
                                           $"or lower(timezone) like '%{search.ToLower()}' " + 
                                           $"or lower(coalesce(id, '')) like '%{search.ToLower()}' " + 
                                           $"or lower(coalesce(language_code, '')) like '%{search.ToLower()}' " + 
                                           $"or lower(coalesce(phone, '')) like '%{search.ToLower()}' " + 
                                           $"or lower(coalesce(fare_url, '')) like '%{search.ToLower()}' " + 
                                           $"or lower(coalesce(email, '')) like '%{search.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(name) like '%{search.ToLower()}%' " + 
                       $"or lower(url) like '%{search.ToLower()}%' " + 
                       $"or lower(timezone) like '%{search.ToLower()}%' " + 
                       $"or lower(coalesce(id, '')) like '%{search.ToLower()}%' " + 
                       $"or lower(coalesce(language_code, '')) like '%{search.ToLower()}%' " + 
                       $"or lower(coalesce(phone, '')) like '%{search.ToLower()}%' " + 
                       $"or lower(coalesce(fare_url, '')) like '%{search.ToLower()}%' " + 
                       $"or lower(coalesce(email, '')) like '%{search.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(timezone) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(timezone) like '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " + 
                                        $"where lower(timezone) like '%{timezone.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(timezone) like '%{timezone.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_agency " + 
                                        $"where lower(url) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_agency " + 
                                        $"where lower(url) like '{url.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_agency " + 
                                        $"where lower(url) like '%{url.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_agency " + 
                    $"where lower(url) like '%{url.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetAgencyFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Partial => "select s.departure_time, " + 
                                             "s.stop_id, " + 
                                             "t.id, " + 
                                             "t.service_id, " + 
                                             "t.headsign, " + 
                                             "t.short_name, " + 
                                             "r.agency_id, " + 
                                             "r.short_name, " + 
                                             "r.long_name, " + 
                                             "c.monday, " + 
                                             "c.tuesday, " + 
                                             "c.wednesday, " + 
                                             "c.thursday, " + 
                                             "c.friday, " + 
                                             "c.saturday, " + 
                                             "c.sunday, " + 
                                             "c.start_date, " + 
                                             "c.end_date " + 
                                      "from gtfs_stop_time s " + 
                                      "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                      "left join gtfs_route r on (t.route_id = r.id) " + 
                                      "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                             $"where lower(s.stop_id) like '%{id.ToLower()}%' " + 
                                                "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                      "order by s.departure_time",

            ComparisonType.Starts => "select s.departure_time, " + 
                                            "s.stop_id, " + 
                                            "t.id, " + 
                                            "t.service_id, " + 
                                            "t.headsign, " + 
                                            "t.short_name, " + 
                                            "r.agency_id, " + 
                                            "r.short_name, " + 
                                            "r.long_name, " + 
                                            "c.monday, " + 
                                            "c.tuesday, " + 
                                            "c.wednesday, " + 
                                            "c.thursday, " + 
                                            "c.friday, " + 
                                            "c.saturday, " + 
                                            "c.sunday, " + 
                                            "c.start_date, " + 
                                            "c.end_date " + 
                                     "from gtfs_stop_time s " + 
                                     "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                     "left join gtfs_route r on (t.route_id = r.id) " + 
                                     "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                            $"where lower(s.stop_id) like '{id.ToLower()}%' " + 
                                               "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                     "order by s.departure_time",

            ComparisonType.Ends => "select s.departure_time, " + 
                                          "s.stop_id, " + 
                                          "t.id, " + 
                                          "t.service_id, " + 
                                          "t.headsign, " + 
                                          "t.short_name, " + 
                                          "r.agency_id, " + 
                                          "r.short_name, " + 
                                          "r.long_name, " + 
                                          "c.monday, " + 
                                          "c.tuesday, " + 
                                          "c.wednesday, " + 
                                          "c.thursday, " + 
                                          "c.friday, " + 
                                          "c.saturday, " + 
                                          "c.sunday, " + 
                                          "c.start_date, " + 
                                          "c.end_date " + 
                                   "from gtfs_stop_time s " + 
                                   "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                   "left join gtfs_route r on (t.route_id = r.id) " + 
                                   "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                          $"where lower(s.stop_id) like '%{id.ToLower()}' " + 
                                             "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                   "order by s.departure_time",

            _ => "select s.departure_time, " + 
                        "s.stop_id, " + 
                        "t.id, " + 
                        "t.service_id, " + 
                        "t.headsign, " + 
                        "t.short_name, " + 
                        "r.agency_id, " + 
                        "r.short_name, " + 
                        "r.long_name, " + 
                        "c.monday, " + 
                        "c.tuesday, " + 
                        "c.wednesday, " + 
                        "c.thursday, " + 
                        "c.friday, " + 
                        "c.saturday, " + 
                        "c.sunday, " + 
                        "c.start_date, " + 
                        "c.end_date " + 
                 "from gtfs_stop_time s " + 
                 "left join gtfs_trip t on (s.trip_id = t.id) " + 
                 "left join gtfs_route r on (t.route_id = r.id) " + 
                 "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                        $"where lower(s.stop_id) = '{id.ToLower()}' " + 
                           "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                 "order by s.departure_time"
        };
        
        return ExecuteCommand(sql, GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Partial => "select s.departure_time, " + 
                                             "s.stop_id, " + 
                                             "t.id, " + 
                                             "t.service_id, " + 
                                             "t.headsign, " + 
                                             "t.short_name, " + 
                                             "r.agency_id, " + 
                                             "r.short_name, " + 
                                             "r.long_name, " + 
                                             "c.monday, " + 
                                             "c.tuesday, " + 
                                             "c.wednesday, " + 
                                             "c.thursday, " + 
                                             "c.friday, " + 
                                             "c.saturday, " + 
                                             "c.sunday, " + 
                                             "c.start_date, " + 
                                             "c.end_date " + 
                                      "from gtfs_stop_time s " + 
                                      "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                      "left join gtfs_route r on (t.route_id = r.id) " + 
                                      "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                             $"where lower(s.trip_id) like '%{id.ToLower()}%' " + 
                                                "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                      "order by s.departure_time",

            ComparisonType.Starts => "select s.departure_time, " + 
                                            "s.stop_id, " + 
                                            "t.id, " + 
                                            "t.service_id, " + 
                                            "t.headsign, " + 
                                            "t.short_name, " + 
                                            "r.agency_id, " + 
                                            "r.short_name, " + 
                                            "r.long_name, " + 
                                            "c.monday, " + 
                                            "c.tuesday, " + 
                                            "c.wednesday, " + 
                                            "c.thursday, " + 
                                            "c.friday, " + 
                                            "c.saturday, " + 
                                            "c.sunday, " + 
                                            "c.start_date, " + 
                                            "c.end_date " + 
                                     "from gtfs_stop_time s " + 
                                     "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                     "left join gtfs_route r on (t.route_id = r.id) " + 
                                     "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                            $"where lower(s.trip_id) like '{id.ToLower()}%' " + 
                                               "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                     "order by s.departure_time",

            ComparisonType.Ends => "select s.departure_time, " + 
                                          "s.stop_id, " + 
                                          "t.id, " + 
                                          "t.service_id, " + 
                                          "t.headsign, " + 
                                          "t.short_name, " + 
                                          "r.agency_id, " + 
                                          "r.short_name, " + 
                                          "r.long_name, " + 
                                          "c.monday, " + 
                                          "c.tuesday, " + 
                                          "c.wednesday, " + 
                                          "c.thursday, " + 
                                          "c.friday, " + 
                                          "c.saturday, " + 
                                          "c.sunday, " + 
                                          "c.start_date, " + 
                                          "c.end_date " + 
                                   "from gtfs_stop_time s " + 
                                   "left join gtfs_trip t on (s.trip_id = t.id) " + 
                                   "left join gtfs_route r on (t.route_id = r.id) " + 
                                   "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                                          $"where lower(s.trip_id) like '%{id.ToLower()}' " + 
                                             "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                                   "order by s.departure_time",

            _ => "select s.departure_time, " + 
                        "s.stop_id, " + 
                        "t.id, " + 
                        "t.service_id, " + 
                        "t.headsign, " + 
                        "t.short_name, " + 
                        "r.agency_id, " + 
                        "r.short_name, " + 
                        "r.long_name, " + 
                        "c.monday, " + 
                        "c.tuesday, " + 
                        "c.wednesday, " + 
                        "c.thursday, " + 
                        "c.friday, " + 
                        "c.saturday, " + 
                        "c.sunday, " + 
                        "c.start_date, " + 
                        "c.end_date " + 
                 "from gtfs_stop_time s " + 
                 "left join gtfs_trip t on (s.trip_id = t.id) " + 
                 "left join gtfs_route r on (t.route_id = r.id) " + 
                 "left join gtfs_calendar c on (t.service_id = c.service_id) " + 
                        $"where lower(s.trip_id) = '{id.ToLower()}' " + 
                           "and coalesce(nullif(s.pickup_type, ''), 0) != 1 " + 
                 "order by s.departure_time"
        };
        
        return ExecuteCommand(sql, GetDepartureFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(code, '')) = '{code.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(code, '')) like '{code.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(code, '')) like '%{code.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(code, '')) like '%{code.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(description, '')) = '{description.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(description, '')) like '{description.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(description, '')) like '%{description.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(description, '')) like '%{description.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(id) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(id) like '{id.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(id) like '%{id.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(id) like '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(level_id, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(level_id, '')) like '{id.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(level_id, '')) like '%{id.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(level_id, '')) like '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where longitude >= {minimumLongitude} " + 
                                          $"and latitude >= {minimumLatitude} " + 
                                          $"and longitude <= {maximumLongitude} " + 
                                          $"and latitude <= {maximumLatitude}",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where longitude >= {minimumLongitude} " + 
                                          $"and latitude >= {minimumLatitude} " + 
                                          $"and longitude <= {maximumLongitude} " + 
                                          $"and latitude <= {maximumLatitude}",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where longitude >= {minimumLongitude} " + 
                                          $"and latitude >= {minimumLatitude} " + 
                                          $"and longitude <= {maximumLongitude} " + 
                                          $"and latitude <= {maximumLatitude}",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where longitude >= {minimumLongitude} " + 
                      $"and latitude >= {minimumLatitude} " + 
                      $"and longitude <= {maximumLongitude} " + 
                      $"and latitude <= {maximumLatitude}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where coalesce(nullif(location_type, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where coalesce(nullif(location_type, ''), 0) = {locationType.ToInt32()}",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where coalesce(nullif(location_type, ''), 0) = {locationType.ToInt32()}",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where coalesce(nullif(location_type, ''), 0) = {locationType.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(name, '')) = '{name.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(name, '')) like '{name.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(name, '')) like '%{name.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(name, '')) like '%{name.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(parent_station, '')) = '{id.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(parent_station, '')) like '{id.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(parent_station, '')) like '%{id.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(parent_station, '')) like '%{id.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(platform_code, '')) = '{platformCode.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(platform_code, '')) like '{platformCode.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(platform_code, '')) like '%{platformCode.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(platform_code, '')) like '%{platformCode.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " +
                                    "from gtfs_stop " + 
                                        $"where lower(id) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(code, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(name, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(description, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(zone, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(url, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(parent_station, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(timezone, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(level_id, '')) = '{search.ToLower()}' " +
                                           $"or lower(coalesce(platform_code, '')) = '{search.ToLower()}'",

            ComparisonType.Starts => "select * " +
                                     "from gtfs_stop " + 
                                        $"where lower(id) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(code, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(name, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(description, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(zone, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(url, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(parent_station, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(timezone, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(level_id, '')) like '{search.ToLower()}%' " +
                                           $"or lower(coalesce(platform_code, '')) like '{search.ToLower()}%'",

            ComparisonType.Ends => "select * " +
                                   "from gtfs_stop " + 
                                        $"where lower(id) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(code, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(name, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(description, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(zone, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(url, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(parent_station, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(timezone, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(level_id, '')) like '%{search.ToLower()}' " +
                                           $"or lower(coalesce(platform_code, '')) like '%{search.ToLower()}'",

            _ => "select * " +
                 "from gtfs_stop " + 
                    $"where lower(id) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(code, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(name, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(description, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(zone, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(url, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(parent_station, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(timezone, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(level_id, '')) like '%{search.ToLower()}%' " +
                       $"or lower(coalesce(platform_code, '')) like '%{search.ToLower()}%'"
        };

        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(timezone, '')) = '{timezone.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(timezone, '')) like '{timezone.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(timezone, '')) like '%{timezone.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(timezone, '')) like '%{timezone.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(url, '')) = '{url.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(url, '')) like '{url.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(url, '')) like '%{url.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(url, '')) like '%{url.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where coalesce(nullif(wheelchair_boarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where coalesce(nullif(wheelchair_boarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where coalesce(nullif(wheelchair_boarding, ''), 0) = {wheelchairBoarding.ToInt32()}",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where coalesce(nullif(wheelchair_boarding, ''), 0) = {wheelchairBoarding.ToInt32()}"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string zone, ComparisonType comparison)
    {
        var sql = comparison switch
        {
            ComparisonType.Exact => "select * " + 
                                    "from gtfs_stop " + 
                                        $"where lower(coalesce(zone, '')) = '{zone.ToLower()}'",
            
            ComparisonType.Starts => "select * " + 
                                     "from gtfs_stop " + 
                                        $"where lower(coalesce(zone, '')) like '{zone.ToLower()}%'",
            
            ComparisonType.Ends => "select * " + 
                                   "from gtfs_stop " + 
                                        $"where lower(coalesce(zone, '')) like '%{zone.ToLower()}'",
            
            _ => "select * " + 
                 "from gtfs_stop " + 
                    $"where lower(coalesce(zone, '')) like '%{zone.ToLower()}%'"
        };
        
        return ExecuteCommand(sql, GetStopFromDataReaderByCondition);
    }
}