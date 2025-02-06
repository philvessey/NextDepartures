using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by id
    /// </summary>
    /// <param name="id">The id of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByIdAsync(string id = "", ComparisonType comparison = ComparisonType.Partial, int results = 0)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByIdAsync(id, comparison);
            
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