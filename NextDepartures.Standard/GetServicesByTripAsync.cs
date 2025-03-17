using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the services by trip
    /// </summary>
    /// <param name="id">The id of the trip. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByTripAsync(
        string id,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrEmpty(value: id))
            return [];
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForTrip = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(
                id: id,
                comparison: comparison);
            
            departuresForTrip.AddRange(collection: new List<Departure>()
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Yesterday,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Today,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Tomorrow,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: id))
            );
            
            if (results > 0)
                return departuresForTrip
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "trip"))
                    .ToList();
            
            return departuresForTrip
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "trip"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// Gets the services by trip
    /// </summary>
    /// <param name="id">The id of the trip. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByTripAsync(
        string id,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrEmpty(value: id))
            return [];
        
        if (target == DateTime.MinValue || target == DateTime.MaxValue)
            return [];
        
        if (offset == TimeSpan.MinValue || offset == TimeSpan.MaxValue)
            return [];
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForTrip = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(
                id: id,
                comparison: comparison);
            
            departuresForTrip.AddRange(collection: new List<Departure>()
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Yesterday,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Today,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Tomorrow,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: id))
            );
            
            if (results > 0)
                return departuresForTrip
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "trip"))
                    .ToList();
            
            return departuresForTrip
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "trip"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}