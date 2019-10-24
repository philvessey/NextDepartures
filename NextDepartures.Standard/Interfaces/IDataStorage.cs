using NextDepartures.Standard.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Standard.Interfaces
{
    /// <summary>
    /// Provides the abstractions for the data storage used by the departure engine.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Gets the agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesAsync();

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <returns>A list of departures.</returns>
        Task<List<Departure>> GetDeparturesForStopAsync(string id);

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <returns>A list of departures.</returns>
        Task<List<Departure>> GetDeparturesForTripAsync(string id);

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        Task<List<Exception>> GetExceptionsAsync();

        /// <summary>
        /// Gets the stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsAsync();

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
        /// Gets the stops by the given area and query.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByLocationAndQueryAsync(double minLon, double minLat, double maxLon, double maxLat, string query);
    }
}
