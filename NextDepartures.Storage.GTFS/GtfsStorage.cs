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
                Id = a.Id,
                Name = a.Name,
                URL = a.URL,
                Timezone = a.Timezone,
                LanguageCode = a.LanguageCode,
                Phone = a.Phone,
                FareURL = a.FareURL,
                Email = a.Email
            })
            .ToList();
    }

    private List<Agency> GetAgenciesFromFeedByCondition(Func<Agency, bool> condition)
    {
        return _feed.Agencies
            .Where(condition)
            .Select(a => new Agency
            {
                Id = a.Id,
                Name = a.Name.Trim().ToTitleCase(),
                URL = a.URL,
                Timezone = a.Timezone,
                LanguageCode = a.LanguageCode,
                Phone = a.Phone,
                FareURL = a.FareURL,
                Email = a.Email
            })
            .ToList();
    }
    
    private List<CalendarDate> GetCalendarDatesFromFeed()
    {
        return _feed.CalendarDates
            .Select(d => new CalendarDate
            {
                ServiceId = d.ServiceId,
                Date = d.Date,
                ExceptionType = d.ExceptionType
            })
            .ToList();
    }
    
    private List<Departure> GetDeparturesFromFeedByCondition(Func<StopTime, bool> condition)
    {
        return _feed.StopTimes
            .Where(s => condition(s) && (s.PickupType != null && s.PickupType.ToString() != string.Empty ? s.PickupType : PickupType.Regular) != PickupType.NoPickup)
            .Join(_feed.Trips, s => s.TripId, t => t.Id, (s, t) => (s, t))
            .Join(_feed.Routes, e => e.t.RouteId, r => r.Id, (e, r) => (e.s, e.t, r))
            .Join(_feed.Calendars, e => e.t.ServiceId, c => c.ServiceId, (e, c) => (e.s, e.t, e.r, c))
            .OrderBy(e => e.s.DepartureTime)
            .Select(e => new Departure
            {
                DepartureTime = new TimeOfDay
                {
                    Hours = e.s.DepartureTime?.Hours ?? 0,
                    Minutes = e.s.DepartureTime?.Minutes ?? 0,
                    Seconds = e.s.DepartureTime?.Seconds ?? 0
                },
                
                StopId = e.s.StopId,
                TripId = e.t.Id,
                ServiceId = e.t.ServiceId,
                TripHeadsign = e.t.Headsign,
                TripShortName = e.t.ShortName,
                AgencyId = e.r.AgencyId,
                RouteShortName = e.r.ShortName,
                RouteLongName = e.r.LongName,
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
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Zone = s.Zone,
                Url = s.Url,
                LocationType = s.LocationType,
                ParentStation = s.ParentStation,
                Timezone = s.Timezone,
                WheelchairBoarding = s.WheelchairBoarding,
                LevelId = s.LevelId,
                PlatformCode = s.PlatformCode
            })
            .ToList();
    }

    private List<Stop> GetStopsFromFeedByCondition(Func<Stop, bool> condition)
    {
        return _feed.Stops
            .Where(condition)
            .Select(s => new Stop
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name.Trim().ToTitleCase(),
                Description = s.Description,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Zone = s.Zone,
                Url = s.Url,
                LocationType = s.LocationType,
                ParentStation = s.ParentStation,
                Timezone = s.Timezone,
                WheelchairBoarding = s.WheelchairBoarding,
                LevelId = s.LevelId,
                PlatformCode = s.PlatformCode
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
                (a.Email ?? string.Empty).Equals(email, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).StartsWith(email, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).EndsWith(email, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Email ?? string.Empty).Contains(email, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).Equals(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).StartsWith(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).EndsWith(fareUrl, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.FareURL ?? string.Empty).Contains(fareUrl, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Id ?? string.Empty).Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).Equals(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).StartsWith(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).EndsWith(languageCode, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.LanguageCode ?? string.Empty).Contains(languageCode, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.EndsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).Equals(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).StartsWith(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).EndsWith(phone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                (a.Phone ?? string.Empty).Contains(phone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.URL.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                a.Timezone.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Id ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.LanguageCode ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Phone ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.FareURL ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (a.Email ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.Equals(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.StartsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.EndsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.Timezone.Contains(timezone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.Equals(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.StartsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.EndsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetAgenciesFromFeedByCondition(a =>
                a.URL.Contains(url, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Partial => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.Contains(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.StopId.Equals(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }

    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Partial => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.Contains(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetDeparturesFromFeedByCondition(s =>
                s.TripId.Equals(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).Equals(code, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).StartsWith(code, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).EndsWith(code, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Code ?? string.Empty).Contains(code, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).Equals(description, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).StartsWith(description, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).EndsWith(description, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Description ?? string.Empty).Contains(description, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.LevelId ?? string.Empty).Contains(id, StringComparison.CurrentCultureIgnoreCase)))
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
                (s.Name ?? string.Empty).Equals(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).StartsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).EndsWith(name, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Name ?? string.Empty).Contains(name, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).Equals(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).StartsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).EndsWith(id, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.ParentStation ?? string.Empty).Contains(id, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).Equals(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).StartsWith(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).EndsWith(platformCode, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.PlatformCode ?? string.Empty).Contains(platformCode, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).Equals(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).StartsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).EndsWith(search, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                s.Id.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Code ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Name ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Description ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Zone ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Url ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.ParentStation ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.Timezone ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.LevelId ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                (s.PlatformCode ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).Equals(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).StartsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).EndsWith(timezone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Timezone ?? string.Empty).Contains(timezone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).Equals(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).StartsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).EndsWith(url, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Url ?? string.Empty).Contains(url, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").Equals(wheelchairBoarding.ToString("D")))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").Equals(wheelchairBoarding.ToString("D")))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").Equals(wheelchairBoarding.ToString("D")))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.WheelchairBoarding != null && s.WheelchairBoarding.ToString() != string.Empty ? s.WheelchairBoarding : "0").Equals(wheelchairBoarding.ToString("D")))),
        };
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string zone, ComparisonType comparison)
    {
        return comparison switch
        {
            ComparisonType.Exact => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).Equals(zone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Starts => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).StartsWith(zone, StringComparison.CurrentCultureIgnoreCase))),
            
            ComparisonType.Ends => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).EndsWith(zone, StringComparison.CurrentCultureIgnoreCase))),
            
            _ => Task.FromResult(GetStopsFromFeedByCondition(s =>
                (s.Zone ?? string.Empty).Contains(zone, StringComparison.CurrentCultureIgnoreCase)))
        };
    }
}