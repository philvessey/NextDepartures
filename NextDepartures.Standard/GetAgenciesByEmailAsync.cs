using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByEmailAsync(string email, int count = 10)
        {
            try
            {
                List<Agency> agenciesFromStorage = await _dataStorage.GetAgenciesByEmailAsync(email);

                return agenciesFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}