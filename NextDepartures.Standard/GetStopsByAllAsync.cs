using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops by the given area and query.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="query">The query.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, int count = 10)
        {
            try
            {
                List<Stop> results = await _dataStorage.GetStopsByAllAsync(minLon, minLat, maxLon, maxLat, query);

                return results.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}