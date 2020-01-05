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
    public class GtfsDbStorage : IDataStorage
    {
        private GTFSFeed feed;

        /// <summary>
        /// Creates a new instance of <see cref="GtfsDbStorage"/> based on the given feed.
        /// </summary>
        /// <param name="feed"></param>
        public GtfsDbStorage(GTFSFeed feed)
        {
            this.feed = feed;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GtfsDbStorage"/>.
        /// </summary>
        /// <param name="path">The directory path of the directory containing the feed or the path to the zip file.</param>
        public static GtfsDbStorage Load(string path)
        {
            var gtfsReader = new GTFSReader<GTFSFeed>();
            var feed = gtfsReader.Read(path);

            return new GtfsDbStorage(feed);
        }

        public Task<List<Agency>> GetAgenciesAsync()
        {
            var agencies = feed.Agencies
                .Select(a => new Agency()
                {
                    AgencyID = a.Id,
                    AgencyName = a.Name,
                    AgencyTimezone = a.Timezone
                })
                .ToList();

            return Task.FromResult(agencies);
        }

        public Task<List<Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return Task.FromResult(GetDeparturesByCondition(s => s.StopId.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return Task.FromResult(GetDeparturesByCondition(s => s.TripId.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        private List<Departure> GetDeparturesByCondition(Func<StopTime, bool> condition)
        {
            return feed.StopTimes
                .Where(s => condition(s) && s.PickupType != PickupType.NoPickup)
                .Join(feed.Trips, s => s.TripId, t => t.Id, (s, t) => (s, t))
                .Join(feed.Routes, e => e.t.RouteId, r => r.Id, (e, r) => (e.s, e.t, r))
                .Join(feed.Calendars, e => e.t.ServiceId, c => c.ServiceId, (e, c) => (e.s, e.t, e.r, c))
                .OrderBy(e => e.s.DepartureTime)
                .Select(e => new Departure()
                {
                    AgencyID = e.r.AgencyId,
                    DepartureTime = e.s.DepartureTime?.ToString(),
                    TripID = e.t.Id,
                    TripHeadsign = e.t.Headsign,
                    TripShortName = e.t.ShortName,
                    RouteLongName = e.r.LongName,
                    RouteShortName = e.r.ShortName,
                    StopID = e.s.StopId,
                    ServiceID = e.t.ServiceId,
                    StartDate = e.c.StartDate.AsInteger().ToString(),
                    EndDate = e.c.EndDate.AsInteger().ToString(),
                    Monday = e.c.Monday ? "1" : "",
                    Tuesday = e.c.Tuesday ? "1" : "",
                    Wednesday = e.c.Wednesday ? "1" : "",
                    Thursday = e.c.Thursday ? "1" : "",
                    Friday = e.c.Friday ? "1" : "",
                    Saturday = e.c.Saturday ? "1" : "",
                    Sunday = e.c.Sunday ? "1" : ""
                })
                .ToList();
        }

        public Task<List<Exception>> GetExceptionsAsync()
        {
            var exceptions = feed.CalendarDates
                .Select(d => new Exception()
                {
                    ExceptionType = ((int) d.ExceptionType).ToString(),
                    ServiceID = d.ServiceId,
                    Date = d.Date.AsInteger().ToString()
                })
                .ToList();

            return Task.FromResult(exceptions);
        }

        public Task<List<Stop>> GetStopsAsync()
        {
            var stops = feed.Stops
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

        public Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            throw new System.NotImplementedException();
        }
    }
}
