using NextDepartures.Standard.Extensions;
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
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, int count = 10)
        {
            const int ToleranceInHours = 1;

            try
            {
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);

                return new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, -1, ToleranceInHours, id, WeekdayUtils.GetPreviousDay))
                    .Take(count)
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 0, ToleranceInHours, id, WeekdayUtils.GetTodayDay))
                    .Take(count)
                    .AddMultiple(GetDeparturesOnDay(departuresFromStorage, 1, ToleranceInHours, id, WeekdayUtils.GetFollowingDay))
                    .Take(count)
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