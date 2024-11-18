using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given fare url.
        /// </summary>
        /// <param name="fareUrl">The fare url. Default is all but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl = "", int count = 0)
        {
            try
            {
                var agenciesFromStorage = await _dataStorage.GetAgenciesByFareUrlAsync(fareUrl);
                return count > 0 ? agenciesFromStorage.Take(count).ToList() : agenciesFromStorage;
            }
            catch
            {
                return null;
            }
        }
    }
}