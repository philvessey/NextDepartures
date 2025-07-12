﻿using System;
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
    /// Gets agencies by id
    /// </summary>
    /// <param name="id">The id of the agency. Default is all.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of agencies.</returns>
    public async Task<List<Agency>> GetAgenciesByIdAsync(
        string id = null,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesByIdAsync(
                id: id,
                comparison: comparison);
            
            if (results > 0)
                return agenciesFromStorage
                    .Take(count: results)
                    .ToList();
            
            return agenciesFromStorage;
        }
        catch (Exception e)
        {
            throw new AgencyException(
                message: "An error occurred while getting agencies by id.",
                innerException: e);
        }
    }
}