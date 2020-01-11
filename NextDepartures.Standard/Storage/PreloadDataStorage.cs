using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Standard.Storage
{
    public class PreloadDataStorage : IDataStorage
    {
        private readonly IDataStorage _dataStorage;

        private List<Agency> _agencies;
        private List<Exception> _exceptions;
        private List<Stop> _stops;

        private PreloadDataStorage(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _agencies = new List<Agency>();
            _exceptions = new List<Models.Exception>();
            _stops = new List<Stop>();
        }

        public static async Task<IDataStorage> LoadAsync(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            // TODO: Pass the original storage with into the constructor here until everything can be preloaded and it is only needed within the preload process and not after that
            PreloadDataStorage preloaded = new PreloadDataStorage(dataStorage);
            await preloaded.PreloadAsync(dataStorage, dataStorageProperties);

            return preloaded;
        }

        private async Task PreloadAsync(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            if (dataStorageProperties.DoesSupportParallelPreload)
            {
                var agenciesTask = dataStorage.GetAgenciesAsync();
                var exceptionsTask = dataStorage.GetExceptionsAsync();
                var stopsTask = dataStorage.GetStopsAsync();

                _agencies = await agenciesTask;
                _exceptions = await exceptionsTask;
                _stops = await stopsTask;
            }
            else
            {
                _agencies = await _dataStorage.GetAgenciesAsync();
                _exceptions = await _dataStorage.GetExceptionsAsync();
                _stops = await _dataStorage.GetStopsAsync();
            }
        }

        /// <summary>
        /// Gets all available agencies.
        /// </summary>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesAsync()
        {
            return Task.FromResult(_agencies);
        }

        /// <summary>
        /// Gets the agencies by the given query and timezone.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            return _dataStorage.GetAgenciesByAllAsync(query, timezone);
        }

        /// <summary>
        /// Gets the agencies by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            return _dataStorage.GetAgenciesByQueryAsync(query);
        }

        /// <summary>
        /// Gets the agencies in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return _dataStorage.GetAgenciesByTimezoneAsync(timezone);
        }

        /// <summary>
        /// Gets the departures for a specific stop.
        /// </summary>
        /// <param name="id">The id of the stop.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return _dataStorage.GetDeparturesForStopAsync(id);
        }

        /// <summary>
        /// Gets the departures for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <remarks>The list should be ordered ascending by the departure time. Also stop times with a pickup type of 1 should be ignored.</remarks>
        /// <returns>A list of departures.</returns>
        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return _dataStorage.GetDeparturesForTripAsync(id);
        }

        /// <summary>
        /// Gets all available exceptions.
        /// </summary>
        /// <returns>A list of exceptions.</returns>
        public Task<List<Exception>> GetExceptionsAsync()
        {
            return Task.FromResult(_exceptions);
        }

        /// <summary>
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsAsync()
        {
            return Task.FromResult(_stops);
        }

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
        public Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            return _dataStorage.GetStopsByAllAsync(minLon, minLat, maxLon, maxLat, query, timezone);
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minLon">The minimum longitude.</param>
        /// <param name="minLat">The minimum latitude.</param>
        /// <param name="maxLon">The maximum longitude.</param>
        /// <param name="maxLat">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return _dataStorage.GetStopsByLocationAsync(minLon, minLat, maxLon, maxLat);
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return _dataStorage.GetStopsByQueryAsync(query);
        }

        /// <summary>
        /// Gets the stops in the given timezone.
        /// </summary>
        /// <param name="timezone">The timezone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return _dataStorage.GetStopsByTimezoneAsync(timezone);
        }
    }
}