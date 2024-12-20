﻿using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Storage.GTFS;

/// <summary>
/// Implements the data storage for the GTFS library
/// </summary>
public class GtfsStorage : IDataStorage
{
    private readonly GTFSFeed _feed;
        
    private GtfsStorage(GTFSFeed feed)
    {
        _feed = feed;
    }

    /// <summary>
    /// Loads a GTFS data set.
    /// </summary>
    /// <param name="path">The path of the directory containing the feed or the path to the zip file.</param>
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

    private List<Agency> GetAgenciesFromFeedByConditionWithSpecialCasing(Func<Agency, bool> condition)
    {
        return _feed.Agencies
            .Where(condition)
            .Select(a => new Agency
            {
                Id = a.Id,
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.Name.ToLower()),
                URL = a.URL,
                Timezone = a.Timezone,
                LanguageCode = a.LanguageCode,
                Phone = a.Phone,
                FareURL = a.FareURL,
                Email = a.Email
            })
            .ToList();
    }

    /// <summary>
    /// Gets all available agencies.
    /// </summary>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesAsync()
    {
        return Task.FromResult(GetAgenciesFromFeed());
    }

    /// <summary>
    /// Gets the agencies by the given email.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.Email ?? "").Contains(email, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies by the given fare url.
    /// </summary>
    /// <param name="fareUrl">The fare url.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.FareURL ?? "").Contains(fareUrl, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies by the given language code.
    /// </summary>
    /// <param name="languageCode">The language code.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.LanguageCode ?? "").Contains(languageCode, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies by the given phone.
    /// </summary>
    /// <param name="phone">The phone.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.Phone ?? "").Contains(phone, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies by the given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.Id ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase) || (a.Name ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies in the given timezone.
    /// </summary>
    /// <param name="timezone">The timezone.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.Timezone ?? "").Contains(timezone, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the agencies by the given url.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>A list of agencies.</returns>
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url)
    {
        return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.URL ?? "").Contains(url, StringComparison.CurrentCultureIgnoreCase)));
    }

    private List<CalendarDate> GetCalendarDatesFromFeed()
    {
        return _feed.CalendarDates
            .Select(d => new CalendarDate
            {
                Date = d.Date,
                ExceptionType = d.ExceptionType,
                ServiceId = d.ServiceId
            })
            .ToList();
    }

    /// <summary>
    /// Gets all available calendar dates.
    /// </summary>
    /// <returns>A list of calendar dates.</returns>
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return Task.FromResult(GetCalendarDatesFromFeed());
    }

    private List<Departure> GetDeparturesFromFeedByCondition(Func<StopTime, bool> condition)
    {
        return _feed.StopTimes
            .Where(s => condition(s) && s.PickupType != PickupType.NoPickup)
            .Join(_feed.Trips, s => s.TripId, t => t.Id, (s, t) => (s, t))
            .Join(_feed.Routes, e => e.t.RouteId, r => r.Id, (e, r) => (e.s, e.t, r))
            .Join(_feed.Calendars, e => e.t.ServiceId, c => c.ServiceId, (e, c) => (e.s, e.t, e.r, c))
            .OrderBy(e => e.s.DepartureTime)
            .Select(e => new Departure
            {
                DepartureTime = e.s.DepartureTime,
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

    /// <summary>
    /// Gets the departures for a specific stop.
    /// </summary>
    /// <param name="id">The id of the stop.</param>
    /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
    /// <returns>A list of departures.</returns>
    public Task<List<Departure>> GetDeparturesForStopAsync(string id)
    {
        return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.StopId.Equals(id, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the departures for a specific trip.
    /// </summary>
    /// <param name="id">The id of the trip.</param>
    /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
    /// <returns>A list of departures.</returns>
    public Task<List<Departure>> GetDeparturesForTripAsync(string id)
    {
        return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.TripId.Equals(id, StringComparison.CurrentCultureIgnoreCase)));
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
                Longitude = s.Longitude,
                Latitude = s.Latitude,
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

    private List<Stop> GetStopsFromFeedByConditionWithSpecialCasing(Func<Stop, bool> condition)
    {
        return _feed.Stops
            .Where(condition)
            .Select(s => new Stop
            {
                Id = s.Id,
                Code = s.Code,
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.Name.ToLower()),
                Description = s.Description,
                Longitude = s.Longitude,
                Latitude = s.Latitude,
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

    /// <summary>
    /// Gets all available stops.
    /// </summary>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsAsync()
    {
        return Task.FromResult(GetStopsFromFeed());
    }

    /// <summary>
    /// Gets the stops by the given description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Description ?? "").Contains(description, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given level.
    /// </summary>
    /// <param name="id">The id of the level.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByLevelAsync(string id)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.LevelId ?? "").Contains(id, StringComparison.CurrentCultureIgnoreCase)));
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
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Longitude >= minimumLongitude && s.Latitude >= minimumLatitude && s.Longitude <= maximumLongitude && s.Latitude <= maximumLatitude));
    }

    /// <summary>
    /// Gets the stops by the given location type.
    /// </summary>
    /// <param name="locationType">The location type.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.LocationType.Equals(locationType)));
    }

    /// <summary>
    /// Gets the stops by the given parent station.
    /// </summary>
    /// <param name="id">The id of the station.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByParentStationAsync(string id)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.ParentStation ?? "").Contains(id, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given platform code.
    /// </summary>
    /// <param name="platformCode">The platform code.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.PlatformCode ?? "").Contains(platformCode, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByQueryAsync(string query)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Id ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase) || (s.Code ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase) || (s.Name ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops in the given timezone.
    /// </summary>
    /// <param name="timezone">The timezone.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Timezone ?? "").Contains(timezone, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given url.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByUrlAsync(string url)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Url ?? "").Contains(url, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given wheelchair boarding.
    /// </summary>
    /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.WheelchairBoarding ?? "").Contains(wheelchairBoarding, StringComparison.CurrentCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the stops by the given zone.
    /// </summary>
    /// <param name="zone">The zone.</param>
    /// <returns>A list of stops.</returns>
    public Task<List<Stop>> GetStopsByZoneAsync(string zone)
    {
        return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Zone ?? "").Contains(zone, StringComparison.CurrentCultureIgnoreCase)));
    }
}