using GTFS.Entities;
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
        /// <param name="count">The number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, int count = 0)
        {
            try
            {
                List<Agency> agenciesFromStorage = await _dataStorage.GetAgenciesByTimezoneAsync(timezone);

                if (count > 0)
                {
                    return agenciesFromStorage.Take(count).ToList();
                }
                else
                {
                    return agenciesFromStorage;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}