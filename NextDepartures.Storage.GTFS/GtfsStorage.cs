using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Extensions;
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
        return new GtfsStorage(reader.Read(path));
    }

    private List<Agency> GetAgenciesFromFeed()
    {
        return _feed.Agencies
            .Select(a => new Agency
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
            .Where(condition)
            .Select(a => new Agency
            {
                Id = a.Id?.TrimDoubleQuotes(),
                Name = a.Name.TrimDoubleQuotes().ToTitleCase(),
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
            .Select(d => new CalendarDate
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
            .Where(s => condition(s) && (s.PickupType != null && s.PickupType.ToString() != string.Empty ? s.PickupType : PickupType.Regular) != PickupType.NoPickup)
            .Join(_feed.Trips, s => s.TripId.TrimDoubleQuotes(), t => t.Id.TrimDoubleQuotes(), (s, t) => (s, t))
            .Join(_feed.Routes, e => e.t.RouteId.TrimDoubleQuotes(), r => r.Id.TrimDoubleQuotes(), (e, r) => (e.s, e.t, r))
            .Join(_feed.Calendars, e => e.t.ServiceId.TrimDoubleQuotes(), c => c.ServiceId.TrimDoubleQuotes(), (e, c) => (e.s, e.t, e.r, c))
            .OrderBy(e => e.s.DepartureTime)
            .Select(e => new Departure
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
            .Select(s => new Stop
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
            .Where(condition)
            .Select(s => new Stop
            {
                Id = s.Id.TrimDoubleQuotes(),
                Code = s.Code?.TrimDoubleQuotes(),
                Name = s.Name?.TrimDoubleQuotes().ToTitleCase(),
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
        return Task.FromResult(GetAgenciesFromFeed());
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return Task.FromResult(GetCalendarDatesFromFeed());
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        return Task.FromResult(GetStopsFromFeed());
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes().Equals(email, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes().StartsWith(email, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes().EndsWith(email, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).TrimDoubleQuotes().Contains(email, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().Equals(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().StartsWith(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().EndsWith(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().Contains(fareUrl, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().Equals(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().StartsWith(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().EndsWith(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().Contains(languageCode, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().Equals(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().StartsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().EndsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().Contains(name, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes().Equals(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes().StartsWith(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes().EndsWith(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).TrimDoubleQuotes().Contains(phone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.TrimDoubleQuotes().Equals(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.TrimDoubleQuotes().StartsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.TrimDoubleQuotes().EndsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.TrimDoubleQuotes().Contains(timezone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.TrimDoubleQuotes().Equals(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.TrimDoubleQuotes().StartsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.TrimDoubleQuotes().EndsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.TrimDoubleQuotes().Contains(url, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Partial => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Partial => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes().Equals(code, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes().StartsWith(code, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes().EndsWith(code, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).TrimDoubleQuotes().Contains(code, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes().Equals(description, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes().StartsWith(description, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes().EndsWith(description, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).TrimDoubleQuotes().Contains(description, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Longitude >= minimumLongitude &&
                s.Latitude >= minimumLatitude &&
                s.Longitude <= maximumLongitude &&
                s.Latitude <= maximumLatitude)),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Longitude >= minimumLongitude &&
                s.Latitude >= minimumLatitude &&
                s.Longitude <= maximumLongitude &&
                s.Latitude <= maximumLatitude)),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Longitude >= minimumLongitude &&
                s.Latitude >= minimumLatitude &&
                s.Longitude <= maximumLongitude &&
                s.Latitude <= maximumLatitude)),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Longitude >= minimumLongitude &&
                s.Latitude >= minimumLatitude &&
                s.Longitude <= maximumLongitude &&
                s.Latitude <= maximumLatitude))
        };
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LocationType != null && s.LocationType.ToString() != string.Empty ? s.LocationType : LocationType.Stop).Equals(locationType))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LocationType != null && s.LocationType.ToString() != string.Empty ? s.LocationType : LocationType.Stop).Equals(locationType))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LocationType != null && s.LocationType.ToString() != string.Empty ? s.LocationType : LocationType.Stop).Equals(locationType))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LocationType != null && s.LocationType.ToString() != string.Empty ? s.LocationType : LocationType.Stop).Equals(locationType))),
        };
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes().Equals(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes().StartsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes().EndsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).TrimDoubleQuotes().Contains(name, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().Equals(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().StartsWith(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().EndsWith(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().Contains(platformCode, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().Equals(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().StartsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().EndsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).TrimDoubleQuotes().Contains(search, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().Equals(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().StartsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().EndsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).TrimDoubleQuotes().Contains(timezone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes().Equals(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes().StartsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes().EndsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).TrimDoubleQuotes().Contains(url, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").TrimDoubleQuotes().Equals(wheelchairBoarding.ToString("D")))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").TrimDoubleQuotes().Equals(wheelchairBoarding.ToString("D")))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").TrimDoubleQuotes().Equals(wheelchairBoarding.ToString("D")))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").TrimDoubleQuotes().Equals(wheelchairBoarding.ToString("D")))),
        };
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes().Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes().StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes().EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).TrimDoubleQuotes().Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
}