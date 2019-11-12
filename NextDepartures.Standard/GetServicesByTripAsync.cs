using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByTripAsync(string id)
        {
            return GetServicesByTripAsync(id, DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByTripAsync(string id, DateTime now)
        {
            const int ToleranceInHours = 12;

            try
            {
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id);

                return new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, now, DayOffsetType.Yesterday, ToleranceInHours, id))
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, now, DayOffsetType.Today, ToleranceInHours, id))
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, now, DayOffsetType.Tomorrow, ToleranceInHours, id))
                    .Select(CreateService)
                    .ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}