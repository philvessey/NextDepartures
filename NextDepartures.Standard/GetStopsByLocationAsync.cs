using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the stops in the given location.
    /// </summary>
    /// <param name="minimumLongitude">The minimum longitude. Default is -180 but can be overridden.</param>
    /// <param name="minimumLatitude">The minimum latitude. Default is -90 but can be overridden.</param>
    /// <param name="maximumLongitude">The maximum longitude. Default is 180 but can be overridden.</param>
    /// <param name="maximumLatitude">The maximum latitude. Default is 90 but can be overridden.</param>
    /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude = -180, double minimumLatitude = -90, double maximumLongitude = 180, double maximumLatitude = 90, int count = 0)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByLocationAsync(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude);
            return count > 0 ? stopsFromStorage.Take(count).ToList() : stopsFromStorage;
        }
        catch
        {
            return null;
        }
    }
}