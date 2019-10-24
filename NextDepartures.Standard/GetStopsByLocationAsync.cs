using NextDepartures.Standard.Model;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of stops by location.</summary>
        public async Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat, int count = 10)
        {
            try
            {
                List<Stop> results = await _dataStorage.GetStopsByLocationAsync(minLon, minLat, maxLon, maxLat);

                return results.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}