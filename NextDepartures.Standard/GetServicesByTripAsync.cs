using GTFS.Entities;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByTripAsync(string id, int hours = 0, int count = 0)
        {
            return GetServicesByTripAsync(id, DateTime.Now, hours, count);
        }

        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByTripAsync(string id, DateTime now, int hours = 0, int count = 0)
        {
            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id);
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForTrip = new List<Departure>();

                departuresForTrip.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, hours, id))
                    );

                if (count > 0)
                {
                    return departuresForTrip
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return departuresForTrip
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}