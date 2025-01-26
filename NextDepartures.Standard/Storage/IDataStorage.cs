using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard.Storage;

public interface IDataStorage
{
    Task<List<Agency>> GetAgenciesAsync();
    Task<List<CalendarDate>> GetCalendarDatesAsync();
    Task<List<Stop>> GetStopsAsync();
    
    Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison);
    Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison);
    
    Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison);
    Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison);
    
    Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison);
    Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison);
    Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison);
    Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison);
    Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison);
    Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison);
    Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison);
    Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison);
    Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison);
    Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison);
    Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison);
    Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison);
    Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison);
    Task<List<Stop>> GetStopsByZoneAsync(string zone, ComparisonType comparison);
}