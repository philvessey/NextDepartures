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
        /// Gets the services for a parent station.
        /// </summary>
        /// <param name="id">The id of the parent station.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByParentStationAsync(string id, int hours = 0, int count = 0)
        {
            return GetServicesByParentStationAsync(id, DateTime.Now, TimeSpan.Zero, hours, count);
        }

        /// <summary>
        /// Gets the services for a parent station.
        /// </summary>
        /// <param name="id">The id of the parent station.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="offset">The TimeSpan offset to filter by.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByParentStationAsync(string id, DateTime now, TimeSpan offset, int hours = 0, int count = 0)
        {
            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Stop> stops = await _dataStorage.GetStopsAsync();
                List<Stop> station = await _dataStorage.GetStopsByParentStationAsync(id);

                List<Departure> departuresForStation = new List<Departure>();

                foreach (Stop stop in station)
                {
                    List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id);

                    departuresForStation.AddRange(new List<Departure>()
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, offset, hours, stop.Id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, offset, hours, stop.Id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, offset, hours, stop.Id))
                        );
                }

                if (count > 0)
                {
                    return departuresForStation
                        .OrderBy(d => d.DepartureDateTime)
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return departuresForStation
                        .OrderBy(d => d.DepartureDateTime)
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
        /// Gets the services for a parent station.
        /// </summary>
        /// <param name="station">A list of stops to group as a parent station.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public Task<List<Service>> GetServicesByParentStationAsync(List<Stop> station, int hours = 0, int count = 0)
        {
            return GetServicesByParentStationAsync(station, DateTime.Now, TimeSpan.Zero, hours, count);
        }

        /// <summary>
        /// Gets the services for a parent station.
        /// </summary>
        /// <param name="station">A list of stops to group as a parent station.</param>
        /// <param name="now">The DateTime target to search from.</param>
        /// <param name="offset">The TimeSpan offset to filter by.</param>
        /// <param name="hours">The maximum number of hours to search over. Default is all (0) but can be overridden.</param>
        /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByParentStationAsync(List<Stop> station, DateTime now, TimeSpan offset, int hours = 0, int count = 0)
        {
            try
            {
                List<Agency> agencies = await _dataStorage.GetAgenciesAsync();
                List<CalendarDate> calendarDates = await _dataStorage.GetCalendarDatesAsync();
                List<Stop> stops = await _dataStorage.GetStopsAsync();

                List<Departure> departuresForStation = new List<Departure>();

                foreach (Stop stop in station)
                {
                    List<Departure> departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id);

                    departuresForStation.AddRange(new List<Departure>()
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Yesterday, offset, hours, stop.Id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Today, offset, hours, stop.Id))
                        .AddMultiple(GetDeparturesOnDay(agencies, calendarDates, stops, departuresFromStorage, now, DayOffsetType.Tomorrow, offset, hours, stop.Id))
                        );
                }

                if (count > 0)
                {
                    return departuresForStation
                        .OrderBy(d => d.DepartureDateTime)
                        .Take(count)
                        .Select(d => CreateService(agencies, stops, d))
                        .ToList();
                }
                else
                {
                    return departuresForStation
                        .OrderBy(d => d.DepartureDateTime)
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