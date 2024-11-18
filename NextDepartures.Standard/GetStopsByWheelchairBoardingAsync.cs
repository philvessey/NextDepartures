using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the stops by the given wheelchair boarding.
    /// </summary>
    /// <param name="wheelchairBoarding">The wheelchair boarding. Default is all but can be overridden.</param>
    /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
    /// <returns>A list of stops.</returns>
    public async Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding = "", int count = 0)
    {
        try
        {
            var stopsFromStorage = await _dataStorage.GetStopsByWheelchairBoardingAsync(wheelchairBoarding);
            return count > 0 ? stopsFromStorage.Take(count).ToList() : stopsFromStorage;
        }
        catch
        {
            return null;
        }
    }
}