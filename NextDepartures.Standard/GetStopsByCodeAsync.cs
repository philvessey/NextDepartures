﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by code
    /// </summary>
    /// <param name="code">The code of the stop. Default is all.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByCodeAsync(string code = "", ComparisonType comparison = ComparisonType.Partial, int results = int.MaxValue)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByCodeAsync(code, comparison);
            
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