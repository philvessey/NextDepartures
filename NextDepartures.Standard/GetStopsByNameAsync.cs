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
    /// Gets stops by name
    /// </summary>
    /// <param name="name">The name of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByNameAsync(
        string name = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        if (results < 0)
            throw new StopException(message: "Invalid results.");
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByNameAsync(
                name: name,
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
                message: "An error occurred while retrieving stops by name.",
                innerException: e);
        }
    }
}