using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Extensions;
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

        private List<Standard.Models.Agency> GetAgenciesFromFeed()
        {
            return _feed.Agencies
                .Select(a => new Standard.Models.Agency()
                {
                    AgencyID = a.Id,
                    AgencyName = a.Name,
                    AgencyTimezone = a.Timezone
                })
                .ToList();
        }

        private List<Standard.Models.Agency> GetAgenciesFromFeedByConditionWithSpecialCasing(Func<Agency, bool> condition)
        {
            return _feed.Agencies
                .Where(a => condition(a))
                .Select(e => new Standard.Models.Agency()
                {
                    AgencyID = e.Id,
                    AgencyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.Name.ToLower()),
                    AgencyTimezone = e.Timezone
                })
                .ToList();
        }

        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesAsync()
        {
            return Task.FromResult(GetAgenciesFromFeed());
        }

        /// <summary>
        /// Gets the agencies by the given query and timezone.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => (a.Id.ToLower().Contains(query.ToLower()) || a.Name.ToLower().Contains(query.ToLower())) && a.Timezone.ToLower().Contains(timezone.ToLower())));
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByQueryAsync(string query)
        {
            return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => a.Id.ToLower().Contains(query.ToLower()) || a.Name.ToLower().Contains(query.ToLower())));
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Standard.Models.Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return Task.FromResult(GetAgenciesFromFeedByConditionWithSpecialCasing(a => a.Timezone.ToLower().Contains(timezone.ToLower())));
        }

        private List<Standard.Models.Departure> GetDeparturesFromFeedByCondition(Func<StopTime, bool> condition)
        {
            return _feed.StopTimes
                .Where(s => condition(s) && s.PickupType != PickupType.NoPickup)
                .Join(_feed.Trips, s => s.TripId, t => t.Id, (s, t) => (s, t))
                .Join(_feed.Routes, e => e.t.RouteId, r => r.Id, (e, r) => (e.s, e.t, r))
                .Join(_feed.Calendars, e => e.t.ServiceId, c => c.ServiceId, (e, c) => (e.s, e.t, e.r, c))
                .OrderBy(e => e.s.DepartureTime)
                .Select(e => new Standard.Models.Departure()
                {
                    DepartureTime = e.s.DepartureTime?.ToString(),
                    StopID = e.s.StopId,
                    ServiceID = e.t.ServiceId,
                    TripID = e.t.Id,
                    TripHeadsign = e.t.Headsign,
                    TripShortName = e.t.ShortName,
                    AgencyID = e.r.AgencyId,
                    RouteShortName = e.r.ShortName,
                    RouteLongName = e.r.LongName,
                    Monday = e.c.Monday ? "1" : "",
                    Tuesday = e.c.Tuesday ? "1" : "",
                    Wednesday = e.c.Wednesday ? "1" : "",
                    Thursday = e.c.Thursday ? "1" : "",
                    Friday = e.c.Friday ? "1" : "",
                    Saturday = e.c.Saturday ? "1" : "",
                    Sunday = e.c.Sunday ? "1" : "",
                    StartDate = e.c.StartDate.AsInteger().ToString(),
                    EndDate = e.c.EndDate.AsInteger().ToString()
                })
                .ToList();
        }

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Standard.Models.Departure>> GetDeparturesForStopAsync(string id)
        {
            return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.StopId.ToLower().Equals(id.ToLower())));
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Standard.Models.Departure>> GetDeparturesForTripAsync(string id)
        {
            return Task.FromResult(GetDeparturesFromFeedByCondition(s => s.TripId.ToLower().Equals(id.ToLower())));
        }

        private List<Standard.Models.Exception> GetExceptionsFromFeed()
        {
            return _feed.CalendarDates
                .Select(d => new Standard.Models.Exception()
                {
                    Date = d.Date.AsInteger().ToString(),
                    ExceptionType = ((int)d.ExceptionType).ToString(),
                    ServiceID = d.ServiceId
                })
                .ToList();
        }

        /// <summary>
        /// Gets all available exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        public Task<List<Standard.Models.Exception>> GetExceptionsAsync()
        {
            return Task.FromResult(GetExceptionsFromFeed());
        }

        private List<Standard.Models.Stop> GetStopsFromFeed()
        {
            return _feed.Stops
                .Select(s => new Standard.Models.Stop()
                {
                    StopID = s.Id,
                    StopCode = s.Code,
                    StopName = s.Name,
                    StopTimezone = s.Timezone
                })
                .ToList();
        }

        private List<Standard.Models.Stop> GetStopsFromFeedByConditionWithSpecialCasing(Func<Stop, bool> condition)
        {
            return _feed.Stops
                .Where(s => condition(s) && s.Latitude != 0 && s.Longitude != 0)
                .Select(e => new Standard.Models.Stop()
                {
                    StopID = e.Id,
                    StopCode = e.Code,
                    StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.Name.ToLower()),
                    StopTimezone = e.Timezone
                })
                .ToList();
        }

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsAsync()
        {
            return Task.FromResult(GetStopsFromFeed());
        }

        /// <summary>
        /// Gets the stops by the given area, query and timezone.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => (s.Id.ToLower().Contains(query.ToLower()) || s.Code.ToLower().Contains(query.ToLower()) || s.Name.ToLower().Contains(query.ToLower())) && s.Latitude >= minLat && s.Latitude <= maxLat && s.Longitude >= minLon && s.Longitude <= maxLon && s.Timezone.ToLower().Contains(timezone.ToLower())));
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Latitude >= minLat && s.Latitude <= maxLat && s.Longitude >= minLon && s.Longitude <= maxLon));
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByQueryAsync(string query)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Id.ToLower().Contains(query.ToLower()) || s.Code.ToLower().Contains(query.ToLower()) || s.Name.ToLower().Contains(query.ToLower())));
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Standard.Models.Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return Task.FromResult(GetStopsFromFeedByConditionWithSpecialCasing(s => s.Timezone.ToLower().Contains(timezone.ToLower())));
        }
    }
}