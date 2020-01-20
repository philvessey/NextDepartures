using GTFS.Entities;
using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Standard.Storage
{
    public class PreloadDataStorage : IDataStorage
    {
        private readonly IDataStorage _dataStorage;

        private List<Agency> _agencies;
        private List<CalendarDate> _calendarDates;
        private List<Stop> _stops;

        private PreloadDataStorage(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _agencies = new List<Agency>();
            _calendarDates = new List<CalendarDate>();
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
                var calendarDatesTask = dataStorage.GetCalendarDatesAsync();
                var stopsTask = dataStorage.GetStopsAsync();

                _agencies = await agenciesTask;
                _calendarDates = await calendarDatesTask;
                _stops = await stopsTask;
            }
            else
            {
                _agencies = await _dataStorage.GetAgenciesAsync();
                _calendarDates = await _dataStorage.GetCalendarDatesAsync();
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
        /// Gets all available calendar dates.
        /// </summary>
        /// <returns>A list of calendar dates.</returns>
        public Task<List<CalendarDate>> GetCalendarDatesAsync()
        {
            return Task.FromResult(_calendarDates);
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
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsAsync()
        {
            return Task.FromResult(_stops);
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude)
        {
            return _dataStorage.GetStopsByLocationAsync(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude);
        }

        /// <summary>
        /// Gets the stops by the given parent station.
        /// </summary>
        /// <param name="id">The id of the station.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByParentStationAsync(string id)
        {
            return _dataStorage.GetStopsByParentStationAsync(id);
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