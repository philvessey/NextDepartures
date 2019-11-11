using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat, int count = 10)
        {
            try
            {
                List<Stop> stopsFromStorage = await _dataStorage.GetStopsByLocationAsync(minLon, minLat, maxLon, maxLat);

                return stopsFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}