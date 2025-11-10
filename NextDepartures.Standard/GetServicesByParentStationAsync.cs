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
    /// Gets the services by parent station
    /// </summary>
    /// <param name="id">The id of the parent station. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        string id,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrEmpty(value: id))
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
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            stopsForStation.AddRange(collection: await _dataStorage.GetStopsByParentStationAsync(
                id: id,
                comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="id">The id of the parent station. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        string id,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (string.IsNullOrEmpty(value: id))
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
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            stopsForStation.AddRange(collection: await _dataStorage.GetStopsByParentStationAsync(
                id: id,
                comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">An array of stop id's to group as a parent station. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        string[] stops,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stops is null || stops.Length is 0)
            throw new ServiceException(message: "Invalid ids.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var s in stops)
                stopsForStation.AddRange(collection: await _dataStorage.GetStopsByIdAsync(
                    id: s,
                    comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">An array of stop id's to group as a parent station. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        string[] stops,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stops is null || stops.Length is 0)
            throw new ServiceException(message: "Invalid ids.");
        
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
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var s in stops)
                stopsForStation.AddRange(collection: await _dataStorage.GetStopsByIdAsync(
                    id: s,
                    comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">A list of stop objects to group as a parent station. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        List<Stop> stops,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stops is null || stops.Count is 0)
            throw new ServiceException(message: "Invalid stops.");
        
        if (tolerance < TimeSpan.Zero)
            throw new ServiceException(message: "Invalid tolerance.");
        
        if (results < 0)
            throw new ServiceException(message: "Invalid results.");
        
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var stopsFromStorage = await _dataStorage.GetStopsAsync();
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var s in stops)
                stopsForStation.AddRange(collection: await _dataStorage.GetStopsByIdAsync(
                    id: s.Id,
                    comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: DateTime.Now,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: TimeSpan.Zero,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
    
    /// <summary>
    /// Gets the services by parent station
    /// </summary>
    /// <param name="stops">A list of stop objects to group as a parent station. Required.</param>
    /// <param name="target">The DateTime target to search from. Required.</param>
    /// <param name="offset">The TimeSpan offset to filter by. Required.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByParentStationAsync(
        List<Stop> stops,
        DateTime target,
        TimeSpan offset,
        ComparisonType comparison = ComparisonType.Exact,
        TimeSpan tolerance = default,
        int results = 0) {
        
        if (stops is null || stops.Count is 0)
            throw new ServiceException(message: "Invalid stops.");
        
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
            
            List<Stop> stopsForStation = [];
            List<Departure> departuresForStation = [];
            
            foreach (var s in stops)
                stopsForStation.AddRange(collection: await _dataStorage.GetStopsByIdAsync(
                    id: s.Id,
                    comparison: comparison));
            
            foreach (var s in stopsForStation)
            {
                var departuresFromStorage = await _dataStorage.GetDeparturesForStopAsync(
                    id: s.Id,
                    comparison: comparison);
                
                departuresForStation.AddRange(collection: new List<Departure>()
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Yesterday,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Today,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                    .AddMultiple(items: GetDeparturesOnDay(
                        agencies: agenciesFromStorage,
                        calendarDates: calendarDatesFromStorage,
                        stops: stopsFromStorage,
                        departures: departuresFromStorage,
                        target: target,
                        dayOffset: DayOffsetType.Tomorrow,
                        timeOffset: offset,
                        tolerance: tolerance,
                        id: s.Id))
                );
            }
            
            if (results > 0)
                return departuresForStation
                    .OrderBy(keySelector: d => d.DepartureDateTime)
                    .Take(count: results)
                    .Select(selector: d =>
                        CreateProcessedService(
                            agencies: agenciesFromStorage,
                            stops: stopsFromStorage,
                            departure: d,
                            type: "station"))
                    .ToList();
            
            return departuresForStation
                .OrderBy(keySelector: d => d.DepartureDateTime)
                .Select(selector: d =>
                    CreateProcessedService(
                        agencies: agenciesFromStorage,
                        stops: stopsFromStorage,
                        departure: d,
                        type: "station"))
                .ToList();
        }
        catch (Exception e)
        {
            throw new ServiceException(
                message: "An error occurred while retrieving services by parent station.",
                innerException: e);
        }
    }
}