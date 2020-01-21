using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Storage.GTFS
{
    /// <summary>
    /// Implements the data storage for the GTFS library
    /// </summary>
    public class GTFSStorage : IDataStorage
    {
        private readonly GTFSFeed _feed;

        /// <summary>
        /// Creates a new GTFS storage.
        /// </summary>
        /// <param name="feed">The GTFSFeed to use.</param>
        public GTFSStorage(GTFSFeed feed)
        {
            _feed = feed;
        }

        /// <summary>
        /// Loads a GTFS data set.
        /// </summary>
        /// <param name="path">The path of the directory containing the feed or the path to the zip file.</param>
        public static GTFSStorage Load(string path)
        {
            GTFSReader<GTFSFeed> reader = new GTFSReader<GTFSFeed>();
            GTFSFeed feed = reader.Read(path);

            return new GTFSStorage(feed);
        }

        private List<Agency> GetAgenciesFromFeed()
        {
            return _feed.Agencies
                .Select(a => new Agency()
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
                .Where(a => condition(a))
                .Select(e => new Agency()
                {
                    Id = e.Id,
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.Name.ToLower()),
                    URL = e.URL,
                    Timezone = e.Timezone,
                    LanguageCode = e.LanguageCode,
                    Phone = e.Phone,
                    FareURL = e.FareURL,
                    Email = e.Email
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
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => a.Id.ToLower().Contains(query.ToLower()) || a.Name.ToLower().Contains(query.ToLower())));
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => a.Timezone.ToLower().Contains(timezone.ToLower())));
        }

        private List<CalendarDate> GetCalendarDatesFromFeed()
        {
            return _feed.CalendarDates
                .Select(d => new CalendarDate()
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
                .Select(e => new Departure()
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
            return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.StopId.ToLower().Equals(id.ToLower())));
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.TripId.ToLower().Equals(id.ToLower())));
        }

        private List<Stop> GetStopsFromFeed()
        {
            return _feed.Stops
                .Select(s => new Stop()
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

        private List<Stop> GetStopsFromFeedByConditionWithSpecialCasing(Func<Stop, bool> condition)
        {
            return _feed.Stops
                .Where(s => condition(s))
                .Select(e => new Stop()
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.Name.ToLower()),
                    Description = e.Description,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Zone = e.Zone,
                    Url = e.Url,
                    LocationType = e.LocationType,
                    ParentStation = e.ParentStation,
                    Timezone = e.Timezone,
                    WheelchairBoarding = e.WheelchairBoarding,
                    LevelId = e.LevelId,
                    PlatformCode = e.PlatformCode
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
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Latitude >= minimumLatitude && s.Latitude <= maximumLatitude && s.Longitude >= minimumLongitude && s.Longitude <= maximumLongitude));
        }

        /// <summary>
        /// Gets the stops by the given parent station.
        /// </summary>
        /// <param name="id">The id of the station.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByParentStationAsync(string id)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.ParentStation.ToLower().Equals(id.ToLower())));
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Id.ToLower().Contains(query.ToLower()) || s.Name.ToLower().Contains(query.ToLower())));
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Timezone.ToLower().Contains(timezone.ToLower())));
        }
    }
}