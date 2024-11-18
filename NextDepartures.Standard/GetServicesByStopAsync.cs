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
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, int hours = 0, int count = 0)
        {
            try
            {
                var agencies = await _dataStorage.GetAgenciesAsync();
                var calendarDates = await _dataStorage.GetCalendarDatesAsync();
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);
                var stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = [];

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, hours, id))
                );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d, "stop"))
                        .ToList();
                }

                return departuresForStop
                    .Select(d => CreateService(agencies, stops, d, "stop"))
                    .ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="offset">The TimeSpan offset to filter by.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(string id, DateTime now, TimeSpan offset, int hours = 0, int count = 0)
        {
            try
            {
                var agencies = await _dataStorage.GetAgenciesAsync();
                var calendarDates = await _dataStorage.GetCalendarDatesAsync();
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id);
                var stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = [];

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, offset, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, offset, hours, id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, offset, hours, id))
                    );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d, "stop"))
                        .ToList();
                }

                return departuresForStop
                    .Select(d => CreateService(agencies, stops, d, "stop"))
                    .ToList();
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
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(Stop stop, int hours = 0, int count = 0)
        {
            try
            {
                var agencies = await _dataStorage.GetAgenciesAsync();
                var calendarDates = await _dataStorage.GetCalendarDatesAsync();
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id);
                var stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = [];

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, hours, stop.Id))
                );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d, "stop"))
                        .ToList();
                }

                return departuresForStop
                    .Select(d => CreateService(agencies, stops, d, "stop"))
                    .ToList();
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
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="offset">The TimeSpan offset to filter by.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByStopAsync(Stop stop, DateTime now, TimeSpan offset, int hours = 0, int count = 0)
        {
            try
            {
                var agencies = await _dataStorage.GetAgenciesAsync();
                var calendarDates = await _dataStorage.GetCalendarDatesAsync();
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id);
                var stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStop = [];

                departuresForStop.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, offset, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, offset, hours, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, offset, hours, stop.Id))
                    );

                if (count > 0)
                {
                    return departuresForStop
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d, "stop"))
                        .ToList();
                }

                return departuresForStop
                    .Select(d => CreateService(agencies, stops, d, "stop"))
                    .ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}