﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by zone
    /// </summary>
    /// <param name="zone">The zone of the stop. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByZoneAsync(string zone = "", ComparisonType comparison = ComparisonType.Partial, int results = 0)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByZoneAsync(zone, comparison);
            
            if (results > 0)
            {
                return stopsFromStorage
                    .Take(results)
                    .ToList();
            }
            
            return stopsFromStorage
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}