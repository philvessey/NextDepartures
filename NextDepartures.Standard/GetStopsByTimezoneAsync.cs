using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone. Default is all but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByTimezoneAsync(string timezone = "", int count = 0)
        {
            try
            {
                List<Stop> stopsFromStorage = await _dataStorage.GetStopsByTimezoneAsync(timezone);

                if (count > 0)
                {
                    return stopsFromStorage.Take(count).ToList();
                }
                else
                {
                    return stopsFromStorage;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}