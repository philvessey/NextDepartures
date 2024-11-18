using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query. Default is all but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByQueryAsync(string query = "", int count = 0)
        {
            try
            {
                var agenciesFromStorage = await _dataStorage.GetAgenciesByQueryAsync(query);
                return count > 0 ? agenciesFromStorage.Take(count).ToList() : agenciesFromStorage;
            }
            catch
            {
                return null;
            }
        }
    }
}