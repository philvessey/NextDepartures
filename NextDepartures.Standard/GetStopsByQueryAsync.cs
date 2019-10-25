using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of stops by query.</summary>
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