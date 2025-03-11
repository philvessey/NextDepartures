using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard.Storage;

public class PreloadDataStorage : IDataStorage
{
    private readonly IDataStorage _dataStorage;

    private List<Agency> _agencies;
    private List<CalendarDate> _calendarDates;
    private List<Stop> _stops;

    private PreloadDataStorage(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;

        _agencies = [];
        _calendarDates = [];
        _stops = [];
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
    
    public Task<List<Agency>> GetAgenciesAsync()
    {
        return Task.FromResult(_agencies);
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return Task.FromResult(_calendarDates);
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        return Task.FromResult(_stops);
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByEmailAsync(email, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByFareUrlAsync(fareUrl, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByIdAsync(id, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByLanguageCodeAsync(languageCode, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByNameAsync(name, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByPhoneAsync(phone, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByQueryAsync(search, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByTimezoneAsync(timezone, comparison);
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        return _dataStorage.GetAgenciesByUrlAsync(url, comparison);
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetDeparturesForStopAsync(id, comparison);
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetDeparturesForTripAsync(id, comparison);
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByCodeAsync(code, comparison);
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByDescriptionAsync(description, comparison);
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByIdAsync(id, comparison);
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByLevelAsync(id, comparison);
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByLocationAsync(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude, comparison);
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByLocationTypeAsync(locationType, comparison);
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByNameAsync(name, comparison);
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByParentStationAsync(id, comparison);
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByPlatformCodeAsync(platformCode, comparison);
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByQueryAsync(search, comparison);
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByTimezoneAsync(timezone, comparison);
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByUrlAsync(url, comparison);
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByWheelchairBoardingAsync(wheelchairBoarding, comparison);
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string id, ComparisonType comparison)
    {
        return _dataStorage.GetStopsByZoneAsync(id, comparison);
    }
}