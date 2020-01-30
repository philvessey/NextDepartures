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
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="count">The number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByStopAsync(string id, int count = 0)
        {
            return GetServicesByStopAsync(id, DateTime.Now, count);
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="count">The number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, DateTime now, int count = 0)
        {
            const int ToleranceInHours = 12;

            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);

                if (count > 0)
                {
                    return new List<Departure>()
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, ToleranceInHours, id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, ToleranceInHours, id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, ToleranceInHours, id))
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return new List<Departure>()
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, ToleranceInHours, id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, ToleranceInHours, id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, ToleranceInHours, id))
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