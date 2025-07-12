# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General 
Transit Feed Specification) data sets stored locally, or in a 
database such as MySQL, PostgreSQL, SQLite or SQL Server. The 
library will work with any well-formed GTFS data set.

## Agencies

```csharp
var results = await feed.GetAgenciesByEmailAsync();
var results = await feed.GetAgenciesByFareUrlAsync();
var results = await feed.GetAgenciesByIdAsync();
var results = await feed.GetAgenciesByLanguageCodeAsync();
var results = await feed.GetAgenciesByNameAsync();
var results = await feed.GetAgenciesByPhoneAsync();
var results = await feed.GetAgenciesByQueryAsync();
var results = await feed.GetAgenciesByTimezoneAsync();
var results = await feed.GetAgenciesByUrlAsync();
```

## Services

```csharp
var results = await feed.GetServicesByParentStationAsync();
var results = await feed.GetServicesByStopAsync();
var results = await feed.GetServicesByTripAsync();
```

## Stops

```csharp
var results = await feed.GetStopsByCodeAsync();
var results = await feed.GetStopsByDescriptionAsync();
var results = await feed.GetStopsByIdAsync();
var results = await feed.GetStopsByLevelAsync();
var results = await feed.GetStopsByLocationAsync();
var results = await feed.GetStopsByLocationTypeAsync();
var results = await feed.GetStopsByNameAsync();
var results = await feed.GetStopsByParentStationAsync();
var results = await feed.GetStopsByPlatformCodeAsync();
var results = await feed.GetStopsByPointAsync();
var results = await feed.GetStopsByQueryAsync();
var results = await feed.GetStopsByTimezoneAsync();
var results = await feed.GetStopsByUrlAsync();
var results = await feed.GetStopsByWheelchairBoardingAsync();
var results = await feed.GetStopsByZoneAsync();
```