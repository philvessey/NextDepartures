﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Exceptions;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by location type
    /// </summary>
    /// <param name="locationType">The location type of the stop. Default is stop.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByLocationTypeAsync(
        LocationType locationType = LocationType.Stop,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByLocationTypeAsync(
                locationType: locationType,
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
                message: "An error occurred while retrieving stops by location type.",
                innerException: e);
        }
    }
}