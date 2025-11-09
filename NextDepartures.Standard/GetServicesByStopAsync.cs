using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using NextDepartures.Standard.Exceptions;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="id">The id of the stop. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(
        string id,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrWhiteSpace(value: id))
            throw new ServiceException(message: "Invalid id.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForStop = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                id: id,
                comparison: comparison);
            
            departuresForStop.AddRange(collection: new List<Departure>()
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
                return departuresForStop
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "stop"))
                    .ToList();
            
            return departuresForStop
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "stop"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by stop.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="id">The id of the stop. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(
        string id,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrWhiteSpace(value: id))
            throw new ServiceException(message: "Invalid id.");
        
        if (target < DateTime.MinValue || target > DateTime.MaxValue)
            throw new ServiceException(message: "Invalid target.");
        
        if (offset < TimeSpan.MinValue || offset > TimeSpan.MaxValue)
            throw new ServiceException(message: "Invalid offset.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForStop = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                id: id,
                comparison: comparison);
            
            departuresForStop.AddRange(collection: new List<Departure>()
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
                return departuresForStop
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "stop"))
                    .ToList();
            
            return departuresForStop
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "stop"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by stop.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="stop">The stop object. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(
        Stop stop,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stop is null)
            throw new ServiceException(message: "Invalid stop.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForStop = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                id: stop.Id,
                comparison: comparison);
            
            departuresForStop.AddRange(collection: new List<Departure>()
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Yesterday,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: stop.Id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Today,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: stop.Id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: DateTime.Now,
                    dayOffset: DayOffsetType.Tomorrow,
                    timeOffset: TimeSpan.Zero,
                    tolerance: tolerance,
                    id: stop.Id))
            );
            
            if (results > 0)
                return departuresForStop
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "stop"))
                    .ToList();
            
            return departuresForStop
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "stop"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by stop.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by stop
    /// </summary>
    /// <param name="stop">The stop object. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByStopAsync(
        Stop stop,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stop is null)
            throw new ServiceException(message: "Invalid stop.");
        
        if (target < DateTime.MinValue || target > DateTime.MaxValue)
            throw new ServiceException(message: "Invalid target.");
        
        if (offset < TimeSpan.MinValue || offset > TimeSpan.MaxValue)
            throw new ServiceException(message: "Invalid offset.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Departure> departuresForStop = [];
            
            var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                id: stop.Id,
                comparison: comparison);
            
            departuresForStop.AddRange(collection: new List<Departure>()
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Yesterday,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: stop.Id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Today,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: stop.Id))
                .AddMultiple(items: GetDeparturesOnDay(
                    agencies: agenciesFromStorage,
                    calendarDates: calendarDatesFromStorage,
                    stops: stopsFromStorage,
                    departures: departuresFromStorage,
                    target: target,
                    dayOffset: DayOffsetType.Tomorrow,
                    timeOffset: offset,
                    tolerance: tolerance,
                    id: stop.Id))
            );
            
            if (results > 0)
                return departuresForStop
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "stop"))
                    .ToList();
            
            return departuresForStop
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "stop"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by stop.",
                innerException: e);
        }
    }
}