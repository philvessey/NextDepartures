﻿using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Utils;
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
        public async Task<List<Service>> GetServicesByTripAsync(string id)
        {
            const int ToleranceInHours = 12;

            try
            {
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id);

                return new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, -1, ToleranceInHours, id, WeekdayUtils.GetPreviousDay))
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 0, ToleranceInHours, id, WeekdayUtils.GetTodayDay))
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 1, ToleranceInHours, id, WeekdayUtils.GetFollowingDay))
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