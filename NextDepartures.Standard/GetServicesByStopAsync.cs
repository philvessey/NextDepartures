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
        /// <param name="hours">The maximum number of hours to search over. Default is 2 but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByStopAsync(string id, int hours = 2, int count = 0)
        {
            return GetServicesByStopAsync(id, DateTime.Now, hours, count);
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is 2 but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, DateTime now, int hours = 2, int count = 0)
        {
            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = new List<Departure>();

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, hours, id))
                    );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return departuresForStop
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="stop">The stop.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is 2 but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByStopAsync(Stop stop, int hours = 2, int count = 0)
        {
            return GetServicesByStopAsync(stop, DateTime.Now, hours, count);
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="stop">The stop.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is 2 but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(Stop stop, DateTime now, int hours = 2, int count = 0)
        {
            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id);
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = new List<Departure>();

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, hours, stop.Id))
                    );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return departuresForStop
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