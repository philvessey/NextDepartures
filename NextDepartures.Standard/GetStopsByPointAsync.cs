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
    /// Gets stops by point distance
    /// </summary>
    /// <param name="longitude">The longitude of the point. Default is -180.</param>
    /// <param name="latitude">The latitude of the point. Default is -90.</param>
    /// <param name="distance">The distance from the point in kilometres. Default is 20,000.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is partial.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByPointAsync(
        double longitude = -180,
        double latitude = -90,
        double distance = 20000,
        ComparisonType comparison = ComparisonType.Partial,
        int results = 0) {
        
        if (longitude is < -180 or > 180)
            throw new StopException(message: "Invalid longitude.");
        
        if (latitude is < -90 or > 90)
            throw new StopException(message: "Invalid latitude.");
        
        if (distance < 0)
            throw new StopException(message: "Invalid distance.");
        
        if (results < 0)
            throw new StopException(message: "Invalid results.");
        
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByPointAsync(
                longitude: longitude,
                latitude: latitude,
                distance: distance,
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
                message: "An error occurred while retrieving stops by point.",
                innerException: e);
        }
    }
}