using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets agencies by timezone
    /// </summary>
    /// <param name="timezone">The timezone of the agency. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of agencies.</returns>
    public async Task<List<Agency>> GetAgenciesByTimezoneAsync(
        string timezone = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesByTimezoneAsync(
                timezone: timezone,
                comparison: comparison);
            
            if (results > 0)
                return agenciesFromStorage
                    .OrderBy(keySelector: a => a.Name)
                    .Take(count: results)
                    .ToList();
            
            return agenciesFromStorage
                .OrderBy(keySelector: a => a.Name)
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}