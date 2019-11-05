using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, int count = 10)
        {
            try
            {
                List<Agency> results = await _dataStorage.GetAgenciesByTimezoneAsync(timezone);

                return results.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}