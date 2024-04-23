using GTFS.Entities;
using GTFS.Entities.Enumerations;
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

            _agencies = new();
            _calendarDates = new();
            _stops = new();
        }

        public static async Task<IDataStorage> LoadAsync(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            PreloadDataStorage preloaded = new(dataStorage);
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
        /// Gets the agencies by the given email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByEmailAsync(string email)
        {
            return _dataStorage.GetAgenciesByEmailAsync(email);
        }

        /// <summary>
        /// Gets the agencies by the given fare URL.
        /// </summary>
        /// <param name="fareURL">The fare URL.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByFareURLAsync(string fareURL)
        {
            return _dataStorage.GetAgenciesByFareURLAsync(fareURL);
        }

        /// <summary>
        /// Gets the agencies by the given language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode)
        {
            return _dataStorage.GetAgenciesByLanguageCodeAsync(languageCode);
        }

        /// <summary>
        /// Gets the agencies by the given phone.
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone)
        {
            return _dataStorage.GetAgenciesByPhoneAsync(phone);
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
        /// Gets the agencies by the given URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A list of agencies.</returns>
        public Task<List<Agency>> GetAgenciesByURLAsync(string url)
        {
            return _dataStorage.GetAgenciesByURLAsync(url);
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
        /// Gets the stops by the given description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByDescriptionAsync(string description)
        {
            return _dataStorage.GetStopsByDescriptionAsync(description);
        }

        /// <summary>
        /// Gets the stops by the given level.
        /// </summary>
        /// <param name="id">The id of the level.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLevelAsync(string id)
        {
            return _dataStorage.GetStopsByLevelAsync(id);
        }

        /// <summary>
        /// Gets the stops in the given location.
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
        /// Gets the stops by the given location type.
        /// </summary>
        /// <param name="locationType">The location type.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType)
        {
            return _dataStorage.GetStopsByLocationTypeAsync(locationType);
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
        /// Gets the stops by the given platform code.
        /// </summary>
        /// <param name="platformCode">The platform code.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode)
        {
            return _dataStorage.GetStopsByPlatformCodeAsync(platformCode);
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

        /// <summary>
        /// Gets the stops by the given URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByURLAsync(string url)
        {
            return _dataStorage.GetStopsByURLAsync(url);
        }

        /// <summary>
        /// Gets the stops by the given wheelchair boarding.
        /// </summary>
        /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding)
        {
            return _dataStorage.GetStopsByWheelchairBoardingAsync(wheelchairBoarding);
        }

        /// <summary>
        /// Gets the stops by the given zone.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>A list of stops.</returns>
        public Task<List<Stop>> GetStopsByZoneAsync(string zone)
        {
            return _dataStorage.GetStopsByZoneAsync(zone);
        }
    }
}