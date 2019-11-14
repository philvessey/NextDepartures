using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Standard.Storage
{
    public class PreloadDataStorage : IDataStorage
    {
        private readonly IDataStorage _dataStorage;
        private readonly DataStorageProperties _dataStorageProperties;

        private List<Agency> _agencies;
        private List<Exception> _exceptions;
        private List<Stop> _stops;

        private PreloadDataStorage(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            _dataStorage = dataStorage;
            _dataStorageProperties = dataStorageProperties;

            _agencies = new List<Agency>();
            _exceptions = new List<Models.Exception>();
            _stops = new List<Stop>();
        }

        public static async Task<IDataStorage> LoadAsync(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            // TODO: Pass them with into the constructor here until everything can be preloaded and they are only needed within the preload process and not after that
            PreloadDataStorage preloaded = new PreloadDataStorage(dataStorage, dataStorageProperties);
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

        public Task<List<Agency>> GetAgenciesAsync()
        {
            return Task.FromResult(_agencies);
        }

        public Task<List<Agency>> GetAgenciesByAllAsync(string query, string timezone)
        {
            return _dataStorage.GetAgenciesByAllAsync(query, timezone);
        }

        public Task<List<Agency>> GetAgenciesByQueryAsync(string query)
        {
            return _dataStorage.GetAgenciesByQueryAsync(query);
        }

        public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone)
        {
            return _dataStorage.GetAgenciesByTimezoneAsync(timezone);
        }

        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return _dataStorage.GetDeparturesForStopAsync(id);
        }

        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return _dataStorage.GetDeparturesForTripAsync(id);
        }

        public Task<List<Exception>> GetExceptionsAsync()
        {
            return Task.FromResult(_exceptions);
        }

        public Task<List<Stop>> GetStopsAsync()
        {
            return Task.FromResult(_stops);
        }

        public Task<List<Stop>> GetStopsByAllAsync(double minLon, double minLat, double maxLon, double maxLat, string query, string timezone)
        {
            return _dataStorage.GetStopsByAllAsync(minLon, minLat, maxLon, maxLat, query, timezone);
        }

        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return _dataStorage.GetStopsByLocationAsync(minLon, minLat, maxLon, maxLat);
        }

        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return _dataStorage.GetStopsByQueryAsync(query);
        }

        public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone)
        {
            return _dataStorage.GetStopsByTimezoneAsync(timezone);
        }
    }
}
