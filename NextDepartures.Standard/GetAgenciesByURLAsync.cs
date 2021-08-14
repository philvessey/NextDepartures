using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given URL.
        /// </summary>
        /// <param name="url">The URL. Default is all but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByURLAsync(string url = "", int count = 0)
        {
            try
            {
                List<Agency> agenciesFromStorage = await _dataStorage.GetAgenciesByURLAsync(url);

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