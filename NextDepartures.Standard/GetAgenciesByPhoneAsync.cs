﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets agencies by phone number
    /// </summary>
    /// <param name="phone">The phone number of the agency. Default is all.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of agencies.</returns>
    public async Task<List<Agency>> GetAgenciesByPhoneAsync(string phone = "", ComparisonType comparison = ComparisonType.Partial, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesByPhoneAsync(phone, comparison);
            
            return agenciesFromStorage
                .Take(results)
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}