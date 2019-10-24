using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Model;

using System;

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
    }
}