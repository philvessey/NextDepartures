using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agency = NextDepartures.Standard.Models.Agency;
using Exception = NextDepartures.Standard.Models.Exception;
using Stop = NextDepartures.Standard.Models.Stop;

namespace NextDepartures.Storage.GTFS
{
    /// <summary>
    /// Implements the data storage for the GTFS library
    /// </summary>
    public class GtfsDbStorage : IDataStorage
    {
        private readonly GTFSFeed _feed;

        /// <summary>
        /// Creates a new instance of <see cref="GtfsDbStorage"/> based on the given feed.
        /// </summary>
        /// <param name="feed">The GTFSFeed to use.</param>
        public GtfsDbStorage(GTFSFeed feed)
        {
            _feed = feed;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GtfsDbStorage"/>.
        /// </summary>
        /// <param name="path">The directory path of the directory containing the feed or the path to the zip file.</param>
        public static GtfsDbStorage Load(string path)
        {
            GTFSReader<GTFSFeed> gtfsReader = new GTFSReader<GTFSFeed>();
            GTFSFeed feed = gtfsReader.Read(path);

            return new GtfsDbStorage(feed);
        }

        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesAsync()
        {
            List<Agency> agencies = _feed.Agencies
                .Select(a => new Agency()
                {
                    AgencyID = a.Id,
                    AgencyName = a.Name,
                    AgencyTimezone = a.Timezone
                })
                .ToList();

            return Task.FromResult(agencies);
        }

        /// <summary>
        /// Gets the agencies by the given query and timezone.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            throw new System.NotImplementedException();
        }

        private List<Departure> GetDeparturesByCondition(Func<StopTime, bool> condition)
        {
            return _feed.StopTimes
                .Where(s => condition(s) && s.PickupType != PickupType.NoPickup)
                .Join(_feed.Trips, s => s.TripId, t => t.Id, (s, t) => (s, t))
                .Join(_feed.Routes, e => e.t.RouteId, r => r.Id, (e, r) => (e.s, e.t, r))
                .Join(_feed.Calendars, e => e.t.ServiceId, c => c.ServiceId, (e, c) => (e.s, e.t, e.r, c))
                .OrderBy(e => e.s.DepartureTime)
                .Select(e => new Departure()
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
        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return Task.FromResult(GetDeparturesByCondition(s => s.StopId.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return Task.FromResult(GetDeparturesByCondition(s => s.TripId.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets all available exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        public Task<List<Exception>> GetExceptionsAsync()
        {
            List<Exception> exceptions = _feed.CalendarDates
                .Select(d => new Exception()
                {
                    Date = d.Date.AsInteger().ToString(),
                    ExceptionType = ((int)d.ExceptionType).ToString(),
                    ServiceID = d.ServiceId
                })
                .ToList();

            return Task.FromResult(exceptions);
        }

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsAsync()
        {
            List<Stop> stops = _feed.Stops
                .Select(s => new Stop()
                {
                    StopID = s.Id,
                    StopCode = s.Code,
                    StopName = s.Name,
                    StopTimezone = s.Timezone
                })
                .ToList();

            return Task.FromResult(stops);
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
        public Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            throw new System.NotImplementedException();
        }
    }
}