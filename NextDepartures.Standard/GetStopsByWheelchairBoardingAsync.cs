using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the stops by the given wheelchair boarding.
        /// </summary>
        /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
        /// <param name="count">The number of results to return. Default is 10 but can be overridden.</param>
        /// <returns>A list of stops.</returns>
        public async Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding, int count = 10)
        {
            try
            {
                List<Stop> stopsFromStorage = await _dataStorage.GetStopsByWheelchairBoardingAsync(wheelchairBoarding);

                return stopsFromStorage.Take(count).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}