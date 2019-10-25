using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of stops by location and query.</summary>
        public async Task<List<Stop>> GetStopsByWildcardAsync(double minLon, double minLat, double maxLon, double maxLat, string query, int count = 10)
        {
            try
            {
                List<Stop> results = await _dataStorage.GetStopsByLocationAndQueryAsync(minLon, minLat, maxLon, maxLat, query);

                return results.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}