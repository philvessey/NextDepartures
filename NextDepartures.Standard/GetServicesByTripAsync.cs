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
    /// <param name="id">The id of the trip.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByTripAsync(string id, ComparisonType comparison = ComparisonType.Exact, TimeSpan tolerance = default, int results = 0)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForTrip = [];

            departuresForTrip.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Yesterday, TimeSpan.Zero, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Today, TimeSpan.Zero, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, DateTime.Now, DayOffsetType.Tomorrow, TimeSpan.Zero, tolerance, id))
            );
            
            if (results > 0)
            {
                return departuresForTrip
                    .OrderBy(d => d.DepartureDateTime)
                    .Take(results)
                    .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "trip"))
                    .ToList();
            }
            
            return departuresForTrip
                .OrderBy(d => d.DepartureDateTime)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "trip"))
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
    /// <param name="id">The id of the trip.</param>
    /// <param name="target">The DateTime target to search from.</param>
    /// <param name="offset">The TimeSpan offset to filter by.</param>
    /// <param name="comparison">The ComparisonType to use when searching. Default is exact.</param>
    /// <param name="tolerance">The TimeSpan tolerance to search over. Default is all.</param>
    /// <param name="results">The number of results to return. Default is all.</param>
    /// <returns>A list of services.</returns>
    public async Task<List<Service>> GetServicesByTripAsync(string id, DateTime target, TimeSpan offset, ComparisonType comparison = ComparisonType.Exact, TimeSpan tolerance = default, int results = 0)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesAsync();
            var calendarDatesFromStorage = await _dataStorage.GetCalendarDatesAsync();
            var departuresFromStorage = await _dataStorage.GetDeparturesForTripAsync(id, comparison);
            var stopsFromStorage = await _dataStorage.GetStopsAsync();

            List<Departure> departuresForTrip = [];

            departuresForTrip.AddRange(new List<Departure>()
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Yesterday, offset, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Today, offset, tolerance, id))
                .AddMultiple(GetDeparturesOnDay(agenciesFromStorage, calendarDatesFromStorage, stopsFromStorage,
                    departuresFromStorage, target, DayOffsetType.Tomorrow, offset, tolerance, id))
            );
            
            if (results > 0)
            {
                return departuresForTrip
                    .OrderBy(d => d.DepartureDateTime)
                    .Take(results)
                    .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "trip"))
                    .ToList();
            }
            
            return departuresForTrip
                .OrderBy(d => d.DepartureDateTime)
                .Select(d => CreateService(agenciesFromStorage, stopsFromStorage, d, "trip"))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}