using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given phone.
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, int count = 10)
        {
            try
            {
                List<Agency> agenciesFromStorage = await _dataStorage.GetAgenciesByPhoneAsync(phone);

                return agenciesFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}