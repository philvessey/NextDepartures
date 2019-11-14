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
            return GetServicesByTripAsync(id, DateTime.Now);
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

                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<Stop> stops = await _dataStorage.GetStopsAsync();
                List<Models.Exception> exceptions = await _dataStorage.GetExceptionsAsync();

                return new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, stops, exceptions, departuresFromStorage, now, DayOffsetType.Yesterday, ToleranceInHours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, stops, exceptions, departuresFromStorage, now, DayOffsetType.Today, ToleranceInHours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, stops, exceptions, departuresFromStorage, now, DayOffsetType.Tomorrow, ToleranceInHours, id))
                    .Select(d => CreateService(agencies, stops, d))
                    .ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}