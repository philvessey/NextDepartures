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
    /// Gets stops by description
    /// </summary>
    /// <param name="description">The description of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByDescriptionAsync(
        string description = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByDescriptionAsync(
                description: description,
                comparison: comparison);
            
            if (results > 0)
                return stopsFromStorage
                    .OrderBy(keySelector: s => s.Name)
                    .ThenBy(keySelector: s => s.Id)
                    .Take(count: results)
                    .ToList();
            
            return stopsFromStorage
                .OrderBy(keySelector: s => s.Name)
                .ThenBy(keySelector: s => s.Id)
                .ToList();
        }
        catch (Exception e)
        {
            throw new StopException(
                message: "An error occurred while retrieving stops by description.",
                innerException: e);
        }
    }
}