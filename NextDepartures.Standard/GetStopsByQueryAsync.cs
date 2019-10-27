using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByQueryAsync(string query, int count = 10)
        {
            try
            {
                List<Stop> results = await _dataStorage.GetStopsByQueryAsync(query);

                return results.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}