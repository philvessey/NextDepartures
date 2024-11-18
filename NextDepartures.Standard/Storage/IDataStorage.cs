using GTFS.Entities;
using GTFS.Entities.Enumerations;
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
        /// Gets the agencies by the given email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByEmailAsync(string email);

        /// <summary>
        /// Gets the agencies by the given fare url.
        /// </summary>
        /// <param name="fareUrl">The fare url.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl);

        /// <summary>
        /// Gets the agencies by the given language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode);

        /// <summary>
        /// Gets the agencies by the given phone.
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByPhoneAsync(string phone);

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
        /// Gets the agencies by the given url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>A list of agencies.</returns>
        Task<List<Agency>> GetAgenciesByUrlAsync(string url);

        /// <summary>
        /// Gets all available calendar dates.
        /// </summary>
        /// <returns>A list of calendar dates.</returns>
        Task<List<CalendarDate>> GetCalendarDatesAsync();

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
        /// Gets all available stops.
        /// </summary>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsAsync();

        /// <summary>
        /// Gets the stops by the given description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByDescriptionAsync(string description);

        /// <summary>
        /// Gets the stops by the given level.
        /// </summary>
        /// <param name="id">The id of the level.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByLevelAsync(string id);

        /// <summary>
        /// Gets the stops in the given location.
        /// </summary>
        /// <param name="minimumLongitude">The minimum longitude.</param>
        /// <param name="minimumLatitude">The minimum latitude.</param>
        /// <param name="maximumLongitude">The maximum longitude.</param>
        /// <param name="maximumLatitude">The maximum latitude.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude);

        /// <summary>
        /// Gets the stops by the given location type.
        /// </summary>
        /// <param name="locationType">The location type.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType);

        /// <summary>
        /// Gets the stops by the given parent station.
        /// </summary>
        /// <param name="id">The id of the station.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByParentStationAsync(string id);

        /// <summary>
        /// Gets the stops by the given platform code.
        /// </summary>
        /// <param name="platformCode">The platform code.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode);

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

        /// <summary>
        /// Gets the stops by the given url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByUrlAsync(string url);

        /// <summary>
        /// Gets the stops by the given wheelchair boarding.
        /// </summary>
        /// <param name="wheelchairBoarding">The wheelchair boarding.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByWheelchairBoardingAsync(string wheelchairBoarding);

        /// <summary>
        /// Gets the stops by the given zone.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>A list of stops.</returns>
        Task<List<Stop>> GetStopsByZoneAsync(string zone);
    }
}