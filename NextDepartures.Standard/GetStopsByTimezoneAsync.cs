using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by timezone
    /// </summary>
    /// <param name="timezone">The timezone of the stop. Default is all.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByTimezoneAsync(string timezone = "", ComparisonType comparison = ComparisonType.Partial, int results = int.MaxValue)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByTimezoneAsync(timezone, comparison);
            
            return stopsFromStorage
                .Take(results)
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}