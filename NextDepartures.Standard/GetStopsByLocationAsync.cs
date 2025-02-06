using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by location bounding box
    /// </summary>
    /// <param name="minimumLongitude">The minimum longitude of the bounding box. Default is -180.</param>
    /// <param name="minimumLatitude">The minimum latitude of the bounding box. Default is -90.</param>
    /// <param name="maximumLongitude">The maximum longitude of the bounding box. Default is 180.</param>
    /// <param name="maximumLatitude">The maximum latitude of the bounding box. Default is 90.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude = -180, double minimumLatitude = -90, double maximumLongitude = 180, double maximumLatitude = 90, ComparisonType comparison = ComparisonType.Partial, int results = 0)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByLocationAsync(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude, comparison);
            
            if (results > 0)
            {
                return stopsFromStorage
                    .OrderBy(s => s.Name)
                    .Take(results)
                    .ToList();
            }
            
            return stopsFromStorage
                .OrderBy(s => s.Name)
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}