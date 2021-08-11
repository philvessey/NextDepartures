using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the agencies by the given language code.
        /// </summary>
        /// <param name="languageCode">The language code. Default is all but can be overridden.</param>
        /// <param name="count">The number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of agencies.</returns>
        public async Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode = "", int count = 0)
        {
            try
            {
                List<Agency> agenciesFromStorage = await _dataStorage.GetAgenciesByLanguageCodeAsync(languageCode);

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