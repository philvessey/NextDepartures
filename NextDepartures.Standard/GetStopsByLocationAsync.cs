using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops in the given location.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, int count = 10)
        {
            try
            {
                List<Stop> stopsFromStorage = await _dataStorage.GetStopsByLocationAsync(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude);

                return stopsFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}