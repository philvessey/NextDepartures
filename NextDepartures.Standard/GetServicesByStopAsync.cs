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
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByStopAsync(string id, int count = 10)
        {
            return GetServicesByStopAsync(id, DateTime.Now, count);
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, DateTime now, int count = 10)
        {
            const int ToleranceInHours = 1;

            try
            {
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);

                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<Models.Exception> exceptions = await _dataStorage.GetExceptionsAsync();
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                return new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, exceptions, stops, departuresFromStorage, now, DayOffsetType.Yesterday, ToleranceInHours, id))
                    .Take(count)
                    .AddMultiple(GetDeparturesOnDay(agencies, exceptions, stops, departuresFromStorage, now, DayOffsetType.Today, ToleranceInHours, id))
                    .Take(count)
                    .AddMultiple(GetDeparturesOnDay(agencies, exceptions, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, ToleranceInHours, id))
                    .Take(count)
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