using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="id">The id of the parent station.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(string id, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            stopsForStation.AddRange(await _dataStorage.GetStopsByParentStationAsync(id, comparison));

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="id">The id of the parent station.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(string id, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            stopsForStation.AddRange(await _dataStorage.GetStopsByParentStationAsync(id, comparison));

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="ids">An array of stop id's to group as a parent station.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(string[] ids, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var id in ids)
            {
                stopsForStation.AddRange(await _dataStorage.GetStopsByIdAsync(id, comparison));
            }

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="ids">An array of stop id's to group as a parent station.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(string[] ids, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var id in ids)
            {
                stopsForStation.AddRange(await _dataStorage.GetStopsByIdAsync(id, comparison));
            }

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">A list of stop objects to group as a parent station.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(List<Stop> stops, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var stop in stops)
            {
                stopsForStation.AddRange(await _dataStorage.GetStopsByIdAsync(stop.Id, comparison));
            }

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">A list of stop objects to group as a parent station.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(List<Stop>stops, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var stop in stops)
            {
                stopsForStation.AddRange(await _dataStorage.GetStopsByIdAsync(stop.Id, comparison));
            }

            foreach (var stop in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);

                departuresForStation.AddRange(new List<Departure>()
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, stop.Id))
                    .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                        departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, stop.Id))
                );
            }
            
            return departuresForStation
                .OrderBy(d => d.DepartureDateTime)
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "station"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}