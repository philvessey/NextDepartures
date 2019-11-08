using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Standard.Storage
{
    /// <summary>
    /// Provides the abstractions for the data storage used by the departure engine.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesAsync();

        /// <summary>
        /// Gets the agencies by the given query and timezone.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByAllAsync(string query, string timezone);

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByQueryAsync(string query);

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone);

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        Task<List<Departure>> GetDeparturesForStopAsync(string id);

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        Task<List<Departure>> GetDeparturesForTripAsync(string id);

        /// <summary>
        /// Gets all available exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        Task<List<Exception>> GetExceptionsAsync();

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsAsync();

        /// <summary>
        /// Gets the stops by the given area, query and timezone.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone);

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat);

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByQueryAsync(string query);

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByTimezoneAsync(string timezone);
    }
}