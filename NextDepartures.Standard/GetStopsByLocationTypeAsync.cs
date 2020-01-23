using GTFS.Entities;
using GTFS.Entities.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops by the given location type.
        /// </summary>
        /// <param name="locationType">The location type.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, int count = 10)
        {
            try
            {
                List<Stop> stopsFromStorage = await _dataStorage.GetStopsByLocationTypeAsync(locationType);

                return stopsFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}