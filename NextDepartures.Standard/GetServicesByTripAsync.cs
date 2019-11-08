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

            List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id);

            /// Process data
            /// 1. Timezone is determined - stop is checked first, if no timezone available, look at agency, if still no timezone assume Etc/UTC.
            /// 2. Yesterday date is checked first incase its midnight and services from previous day are running date, then checks today date, finally tomorrow date.
            /// 3. For each departure the departureTime is converted to DateTime. GTFS can store hours as more than 24 so that is handled.
            /// 4. Looks for which day of the week we are on.
            /// 5. Checks to see if a service is excluded from running - if it is its ignored.
            /// 6. Checks to see if the stop is the trip destination - if it is its ignored as this is not a departure.
            /// 7. Processed Departure is created if departureTime > now and < 12 hours from now.
            /// 8. If cant determine what day of week service runs on exceptions are checked to see if service running  - if it is its included.
            /// 9. Checks to see if the stop is the trip destination - if it is its ignored as this is not a departure.

            return new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(departuresFromStorage, -1, ToleranceInHours, id, WeekdayUtils.GetPreviousDay)) // Yesterday
                .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 0, ToleranceInHours, id, WeekdayUtils.GetTodayDay)) // Today
                .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 1, ToleranceInHours, id, WeekdayUtils.GetFollowingDay)) // Tomorrow
                .Select(CreateService)
                .ToList();
        }
    }
}