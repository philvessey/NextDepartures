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
        /// <param name="locationType">The location type. Default is LocationType.Stop but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType = LocationType.Stop, int count = 0)
        {
            try
            {
                var stopsFromStorage = await _dataStorage.GetStopsByLocationTypeAsync(locationType);
                return count > 0 ? stopsFromStorage.Take(count).ToList() : stopsFromStorage;
            }
            catch
            {
                return null;
            }
        }
    }
}