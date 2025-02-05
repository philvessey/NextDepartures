using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;

namespace NextDepartures.Test.Mock;

public class MockStorage : IDataStorage
{
    public Task<List<Agency>> GetAgenciesAsync()
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return Task.FromResult(new List<CalendarDate>
        {
            new()
            {
                ServiceId = "2025_01_21-DX-MVS",
                Date = new DateTime(2025, 12, 25),
                ExceptionType = ExceptionType.Removed
            }
        });
    }
    
    public Task<List<Stop>> GetStopsAsync()
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByEmailAsync(string email, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByFareUrlAsync(string fareUrl, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByIdAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByLanguageCodeAsync(string languageCode, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByNameAsync(string name, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByPhoneAsync(string phone, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByQueryAsync(string search, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Agency>> GetAgenciesByUrlAsync(string url, ComparisonType comparison)
    {
        return Task.FromResult(new List<Agency>
        {
            new()
            {
                Id = "BART",
                Name = "Bay Area Rapid Transit",
                URL = "https://www.bart.gov/",
                Timezone = "America/Los_Angeles",
                Phone = "510-464-6000"
            }
        });
    }
    
    public Task<List<Departure>> GetDeparturesForStopAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Departure>
        {
            new()
            {
                DepartureTime = new TimeOfDay
                {
                    Hours = 10,
                    Minutes = 30,
                    Seconds = 0
                },
                
                StopId = "16TH",
                TripId = "1561190",
                ServiceId = "2025_01_21-DX-MVS",
                TripHeadsign = "San Francisco / Antioch",
                RouteShortName = "Yellow-N",
                RouteLongName = "Millbrae/SFIA to Antioch",
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = true,
                Sunday = true,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            }
        });
    }
    
    public Task<List<Departure>> GetDeparturesForTripAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Departure>
        {
            new()
            {
                DepartureTime = new TimeOfDay()
                {
                    Hours = 10,
                    Minutes = 30,
                    Seconds = 0
                },
                
                StopId = "16TH",
                TripId = "1561190",
                ServiceId = "2025_01_21-DX-MVS",
                TripHeadsign = "San Francisco / Antioch",
                RouteShortName = "Yellow-N",
                RouteLongName = "Millbrae/SFIA to Antioch",
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = true,
                Sunday = true,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByCodeAsync(string code, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByDescriptionAsync(string description, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByIdAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByLevelAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByLocationAsync(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByLocationTypeAsync(LocationType locationType, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByNameAsync(string name, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByParentStationAsync(string id, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByPlatformCodeAsync(string platformCode, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByQueryAsync(string search, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByTimezoneAsync(string timezone, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByUrlAsync(string url, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByWheelchairBoardingAsync(WheelchairAccessibilityType wheelchairBoarding, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
    
    public Task<List<Stop>> GetStopsByZoneAsync(string zone, ComparisonType comparison)
    {
        return Task.FromResult(new List<Stop>
        {
            new()
            {
                Id = "16TH",
                Name = "16th Street / Mission",
                Latitude = 37.765176,
                Longitude = -122.419755,
                Zone = "16TH",
                LocationType = LocationType.Stop
            }
        });
    }
}