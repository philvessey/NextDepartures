using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;
using NextDepartures.Storage.GTFS.Extensions;

namespace NextDepartures.Storage.GTFS;

public class GtfsStorage : IDataStorage
{
    private readonly GTFSFeed _feed;
    
    private GtfsStorage(GTFSFeed feed)
    {
        _feed = feed;
    }
    
    /// <summary>
    /// Loads a GTFS data storage.
    /// </summary>
    /// <param name="path">The path to GTFS data.</param>
    public static GtfsStorage Load(string path)
    {
        GTFSReader<GTFSFeed> reader = new();
        return new GtfsStorage(feed: reader.Read(path: path));
    }
    
    private List<Agency> GetAgenciesFromFeed()
    {
        return _feed.Agencies
            .Select(selector: a => new Agency
            {
                Id = a.Id?.TrimDoubleQuotes(),
                Name = a.Name.TrimDoubleQuotes(),
                URL = a.URL.TrimDoubleQuotes(),
                Timezone = a.Timezone.TrimDoubleQuotes(),
                LanguageCode = a.LanguageCode?.TrimDoubleQuotes(),
                Phone = a.Phone?.TrimDoubleQuotes(),
                FareURL = a.FareURL?.TrimDoubleQuotes(),
                Email = a.Email?.TrimDoubleQuotes()
            })
            .ToList();
    }
    
    private List<Agency> GetAgenciesFromFeedByCondition(Func<Agency, bool> condition)
    {
        return _feed.Agencies
            .Where(predicate: condition)
            .Select(selector: a => new Agency
            {
                Id = a.Id?.TrimDoubleQuotes(),
                Name = a.Name.TrimDoubleQuotes(),
                URL = a.URL.TrimDoubleQuotes(),
                Timezone = a.Timezone.TrimDoubleQuotes(),
                LanguageCode = a.LanguageCode?.TrimDoubleQuotes(),
                Phone = a.Phone?.TrimDoubleQuotes(),
                FareURL = a.FareURL?.TrimDoubleQuotes(),
                Email = a.Email?.TrimDoubleQuotes()
            })
            .ToList();
    }
    
    private List<CalendarDate> GetCalendarDatesFromFeed()
    {
        return _feed.CalendarDates
            .Select(selector: d => new CalendarDate
            {
                ServiceId = d.ServiceId.TrimDoubleQuotes(),
                Date = d.Date,
                ExceptionType = d.ExceptionType
            })
            .ToList();
    }
    
    private List<Departure> GetDeparturesFromFeedByCondition(Func<StopTime, bool> condition)
    {
        return _feed.StopTimes
            .Where(predicate: s => condition(s) && (s.PickupType is not null && s.PickupType.ToString() != string.Empty ? s.PickupType : PickupType.Regular) is not PickupType.NoPickup)
            .Join(inner: _feed.Trips, s => s.TripId.TrimDoubleQuotes(), t => t.Id.TrimDoubleQuotes(), (s, t) => (s, t))
            .Join(inner: _feed.Routes, e => e.t.RouteId.TrimDoubleQuotes(), r => r.Id.TrimDoubleQuotes(), (e, r) => (e.s, e.t, r))
            .Join(inner: _feed.Calendars, e => e.t.ServiceId.TrimDoubleQuotes(), c => c.ServiceId.TrimDoubleQuotes(), (e, c) => (e.s, e.t, e.r, c))
            .OrderBy(keySelector: e => e.s.DepartureTime)
            .Select(selector: e => new Departure
            {
                DepartureTime = new TimeOfDay
                {
                    Hours = e.s.DepartureTime?.Hours ?? 0,
                    Minutes = e.s.DepartureTime?.Minutes ?? 0,
                    Seconds = e.s.DepartureTime?.Seconds ?? 0
                },
                
                StopId = e.s.StopId?.TrimDoubleQuotes(),
                TripId = e.t.Id.TrimDoubleQuotes(),
                ServiceId = e.t.ServiceId.TrimDoubleQuotes(),
                TripHeadsign = e.t.Headsign?.TrimDoubleQuotes(),
                TripShortName = e.t.ShortName?.TrimDoubleQuotes(),
                AgencyId = e.r.AgencyId?.TrimDoubleQuotes(),
                RouteShortName = e.r.ShortName?.TrimDoubleQuotes(),
                RouteLongName = e.r.LongName?.TrimDoubleQuotes(),
                Monday = e.c.Monday,
                Tuesday = e.c.Tuesday,
                Wednesday = e.c.Wednesday,
                Thursday = e.c.Thursday,
                Friday = e.c.Friday,
                Saturday = e.c.Saturday,
                Sunday = e.c.Sunday,
                StartDate = e.c.StartDate,
                EndDate = e.c.EndDate
            })
            .ToList();
    }
    
    private List<Stop> GetStopsFromFeed()
    {
        return _feed.Stops
            .Select(selector: s => new Stop
            {
                Id = s.Id.TrimDoubleQuotes(),
                Code = s.Code?.TrimDoubleQuotes(),
                Name = s.Name?.TrimDoubleQuotes(),
                Description = s.Description?.TrimDoubleQuotes(),
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Zone = s.Zone?.TrimDoubleQuotes(),
                Url = s.Url?.TrimDoubleQuotes(),
                LocationType = s.LocationType,
                ParentStation = s.ParentStation?.TrimDoubleQuotes(),
                Timezone = s.Timezone?.TrimDoubleQuotes(),
                WheelchairBoarding = s.WheelchairBoarding?.TrimDoubleQuotes(),
                LevelId = s.LevelId?.TrimDoubleQuotes(),
                PlatformCode = s.PlatformCode?.TrimDoubleQuotes()
            })
            .ToList();
    }
    
    private List<Stop> GetStopsFromFeedByCondition(Func<Stop, bool> condition)
    {
        return _feed.Stops
            .Where(predicate: condition)
            .Select(selector: s => new Stop
            {
                Id = s.Id.TrimDoubleQuotes(),
                Code = s.Code?.TrimDoubleQuotes(),
                Name = s.Name?.TrimDoubleQuotes(),
                Description = s.Description?.TrimDoubleQuotes(),
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Zone = s.Zone?.TrimDoubleQuotes(),
                Url = s.Url?.TrimDoubleQuotes(),
                LocationType = s.LocationType,
                ParentStation = s.ParentStation?.TrimDoubleQuotes(),
                Timezone = s.Timezone?.TrimDoubleQuotes(),
                WheelchairBoarding = s.WheelchairBoarding?.TrimDoubleQuotes(),
                LevelId = s.LevelId?.TrimDoubleQuotes(),
                PlatformCode = s.PlatformCode?.TrimDoubleQuotes()
            })
            .ToList();
    }
    
    public Task<List<Agency>> GetAgenciesAsync()
    {
        return Task.FromResult(result: GetAgenciesFromFeed());
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return Task.FromResult(result: GetCalendarDatesFromFeed());
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        return Task.FromResult(result: GetStopsFromFeed());
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(
        string email,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: email ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: email ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: email ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: email ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(
        string fareUrl,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: fareUrl ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: fareUrl ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: fareUrl ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: fareUrl ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(
        string id,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(
        string languageCode,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: languageCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: languageCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: languageCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: languageCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(
        string name,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                a.Name.TrimDoubleQuotes()
                    .Equals(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                a.Name.TrimDoubleQuotes()
                    .StartsWith(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                a.Name.TrimDoubleQuotes()
                    .EndsWith(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                a.Name.TrimDoubleQuotes()
                    .Contains(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(
        string phone,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: phone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: phone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: phone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: phone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                a.Name.TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                a.Name.TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                a.Name.TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                a.Name.TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                a.Timezone.TrimDoubleQuotes()
                    .Equals(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                a.Timezone.TrimDoubleQuotes()
                    .StartsWith(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                a.Timezone.TrimDoubleQuotes()
                    .EndsWith(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                a.Timezone.TrimDoubleQuotes()
                    .Contains(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        Func<Agency, bool> condition = comparison switch
        {
            ComparisonType.Exact => a =>
                a.URL.TrimDoubleQuotes()
                    .Equals(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => a =>
                a.URL.TrimDoubleQuotes()
                    .StartsWith(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => a =>
                a.URL.TrimDoubleQuotes()
                    .EndsWith(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => a =>
                a.URL.TrimDoubleQuotes()
                    .Contains(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetAgenciesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(
        string id,
        ComparisonType comparison) {
        
        Func<StopTime, bool> condition = comparison switch
        {
            ComparisonType.Partial => s =>
                s.StopId.TrimDoubleQuotes()
                    .Contains(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                s.StopId.TrimDoubleQuotes()
                    .StartsWith(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                s.StopId.TrimDoubleQuotes()
                    .EndsWith(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                s.StopId.TrimDoubleQuotes()
                    .Equals(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetDeparturesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(
        string id,
        ComparisonType comparison) {
        
        Func<StopTime, bool> condition = comparison switch
        {
            ComparisonType.Partial => s =>
                s.TripId.TrimDoubleQuotes()
                    .Contains(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                s.TripId.TrimDoubleQuotes()
                    .StartsWith(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                s.TripId.TrimDoubleQuotes()
                    .EndsWith(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                s.TripId.TrimDoubleQuotes()
                    .Equals(
                        value: id,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetDeparturesFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(
        string code,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: code ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: code ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: code ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: code ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(
        string description,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: description ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: description ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: description ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: description ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(
        string id,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                s.Id.TrimDoubleQuotes()
                    .Equals(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                s.Id.TrimDoubleQuotes()
                    .StartsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                s.Id.TrimDoubleQuotes()
                    .EndsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                s.Id.TrimDoubleQuotes()
                    .Contains(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(
        string id,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(
        double minimumLongitude,
        double minimumLatitude,
        double maximumLongitude,
        double maximumLatitude,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            _ => s =>
                s.Longitude >= minimumLongitude &&
                s.Latitude >= minimumLatitude &&
                s.Longitude <= maximumLongitude &&
                s.Latitude <= maximumLatitude
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(
        LocationType locationType,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            _ => s =>
                (s.LocationType is not null && s.LocationType.ToString() != string.Empty ? s.LocationType : LocationType.Stop)
                    .Equals(other: locationType)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(
        string name,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: name ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(
        string id,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(
        string platformCode,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: platformCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: platformCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: platformCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: platformCode ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(
        string search,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                s.Id.TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                s.Id.TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                s.Id.TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                s.Id.TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: search ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(
        string timezone,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: timezone ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(
        string url,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: url ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(
        WheelchairAccessibilityType wheelchairBoarding,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            _ => s =>
                (s.WheelchairBoarding is not null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").TrimDoubleQuotes()
                    .Equals(
                        value: wheelchairBoarding.ToString(format: "D"),
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(
        string id,
        ComparisonType comparison) {
        
        Func<Stop, bool> condition = comparison switch
        {
            ComparisonType.Exact => s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .Equals(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Starts => s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .StartsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            ComparisonType.Ends => s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .EndsWith(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase),
            
            _ => s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes()
                    .Contains(
                        value: id ?? string.Empty,
                        comparisonType: StringComparison.InvariantCultureIgnoreCase)
        };
        
        return Task.FromResult(result: GetStopsFromFeedByCondition(condition: condition));
    }
}