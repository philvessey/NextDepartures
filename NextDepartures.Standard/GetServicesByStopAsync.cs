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
    /// Gets the services by stop
    /// </summary>
    /// <param name="id">The id of the stop.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(string id, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForStop = [];

            departuresForStop.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, id))
            );
            
            return departuresForStop
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "stop"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="id">The id of the stop.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(string id, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForStop = [];

            departuresForStop.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, id))
            );
            
            return departuresForStop
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "stop"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="stop">The stop object.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(Stop stop, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForStop = [];

            departuresForStop.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, stop.Id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, stop.Id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, stop.Id))
            );
            
            return departuresForStop
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "stop"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="stop">The stop object.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The comparison type to use when searching. Default is exact.</param>
    /// <param name="tolerance">The number of hours to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(Stop stop, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, int tolerance = int.MaxValue, int results = int.MaxValue)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(stop.Id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForStop = [];

            departuresForStop.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, stop.Id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, stop.Id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, stop.Id))
            );
            
            return departuresForStop
                .Take(results)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "stop"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}