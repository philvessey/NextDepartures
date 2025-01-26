﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets stops by wheelchair boarding
    /// </summary>
    /// <param name="wheelchairBoarding">The wheelchair boarding status of the stop. Default is no information.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding = WheelchairAccessibilityType.NoInformation, ComparisonType comparison = ComparisonType.Partial, int results = int.MaxValue)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByWheelchairBoardingAsync(wheelchairBoarding, comparison);
            
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