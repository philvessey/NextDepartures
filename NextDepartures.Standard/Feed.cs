using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        private readonly IDataStorage _dataStorage;

        /// <summary>
        /// Creates a new feed.
        /// </summary>
        /// <param name="dataStorage">The data storage to use.</param>
        public Feed(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <summary>
        /// Creates a working departure.
        /// </summary>
        /// <param name="tempDeparture">The temporary departure.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <remarks>A working departure is a departure actually running on the date / time.</remarks>
        /// <returns>A departure.</returns>
        private Departure CreateWorkingDeparture(Departure tempDeparture, DateTime departureTime)
        {
            return new Departure()
            {
                AgencyID = tempDeparture.AgencyID,
                DepartureTime = departureTime.ToString(),
                EndDate = tempDeparture.EndDate,
                Friday = tempDeparture.Friday,
                Monday = tempDeparture.Monday,
                RouteLongName = tempDeparture.RouteLongName,
                RouteShortName = tempDeparture.RouteShortName,
                Saturday = tempDeparture.Saturday,
                ServiceID = tempDeparture.ServiceID,
                StartDate = tempDeparture.StartDate,
                StopID = tempDeparture.StopID,
                Sunday = tempDeparture.Sunday,
                Thursday = tempDeparture.Thursday,
                TripHeadsign = tempDeparture.TripHeadsign,
                TripID = tempDeparture.TripID,
                TripShortName = tempDeparture.TripShortName,
                Tuesday = tempDeparture.Tuesday,
                Wednesday = tempDeparture.Wednesday
            };
        }

        private Service CreateService(Departure departure, List<Stop> workingStops, List<Agency> workingAgencies)
        {
            const string fallback = "Unknown";

            string agencyName = StringUtils.FindPossibleString(fallback,
                                    () => workingAgencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyName,
                                    () => workingAgencies.FirstOrDefault()?.AgencyName
                                    ).Trim().ToTitleCase();

            string destinationName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            string routeName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            string stopName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopName
                ).Trim().ToTitleCase();

            return new Service()
            {
                AgencyName = agencyName,
                DepartureTime = departure.DepartureTime,
                DestinationName = destinationName,
                RouteName = routeName,
                StopName = stopName,
                TripID = departure.TripID
            };
        }

        private string GetTimezone(List<Agency> workingAgencies, List<Stop> workingStops, Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => workingStops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone,
                () => workingAgencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone,
                () => workingAgencies.FirstOrDefault()?.AgencyTimezone
                );
        }
    }
}