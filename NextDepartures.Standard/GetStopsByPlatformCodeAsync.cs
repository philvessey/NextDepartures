using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Exceptions;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by platform code
    /// </summary>
    /// <param name="platformCode">The platform code of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByPlatformCodeAsync(
        string platformCode = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByPlatformCodeAsync(
                platformCode: platformCode,
                comparison: comparison);
            
            if (results > 0)
                return stopsFromStorage
                    .Take(count: results)
                    .ToList();
            
            return stopsFromStorage;
        }
        catch (Exception e)
        {
            throw new StopException(
                message: "An error occurred while retrieving stops by platform code.",
                innerException: e);
        }
    }
}