using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by parent station
    /// </summary>
    /// <param name="id">The parent station id of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByParentStationAsync(
        string id = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByParentStationAsync(
                id: id,
                comparison: comparison);
            
            if (results > 0)
                return stopsFromStorage
                    .OrderBy(keySelector: s => s.Name)
                    .Take(count: results)
                    .ToList();
            
            return stopsFromStorage
                .OrderBy(keySelector: s => s.Name)
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}