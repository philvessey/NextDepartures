# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General Transit Feed Specification) data sets stored locally or in an Azure SQL Database. The library will work with any well formed GTFS data set.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://dev.azure.com/philvessey/NextDepartures/_build/latest?definitionId=2&branchName=master)
* NextDepartures.Standard: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)
* NextDepartures.Storage.GTFS: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.GTFS.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.GTFS/)
* NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## Local GTFS Usage

Connect to the library:

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

Feed feed = await Feed.Load(GTFSStorage.Load([path]));
```

* [path] > Path to GTFS data set .zip or directory. Required.

## SQL Server Usage

Build the data from the database project:

```
dotnet run -d [--database] -g [--gtfs] (--prefix)
```

* [database] > Database connection string. Required.
* [gtfs] > Path to GTFS data set .zip or directory. Required.
* (prefix) > Specify database table prefix. Default (GTFS). Optional.

Connect to the library:

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.SqlServer;

Feed feed = await Feed.Load(SqlServerStorage.Load([database], (prefix)));
```

* [database] > Database connection string. Required.
* (prefix) > Specify database table prefix. Default (GTFS). Optional.

## Agencies Endpoints

Once connected to the library:

```csharp
List<Agency> results = await feed.GetAgenciesByEmailAsync();
List<Agency> results = await feed.GetAgenciesByFareURLAsync();
List<Agency> results = await feed.GetAgenciesByLanguageCodeAsync();
List<Agency> results = await feed.GetAgenciesByPhoneAsync();
List<Agency> results = await feed.GetAgenciesByQueryAsync();
List<Agency> results = await feed.GetAgenciesByTimezoneAsync();
List<Agency> results = await feed.GetAgenciesByURLAsync();
```

## Services Endpoints

Once connected to the library:

```csharp
List<Service> results = await feed.GetServicesByParentStationAsync();
List<Service> results = await feed.GetServicesByStopAsync();
List<Service> results = await feed.GetServicesByTripAsync();
```

## Stops Endpoints

Once connected to the library:

```csharp
List<Stop> results = await feed.GetStopsByDescriptionAsync();
List<Stop> results = await feed.GetStopsByLevelAsync();
List<Stop> results = await feed.GetStopsByLocationAsync();
List<Stop> results = await feed.GetStopsByLocationTypeAsync();
List<Stop> results = await feed.GetStopsByParentStationAsync();
List<Stop> results = await feed.GetStopsByPlatformCodeAsync();
List<Stop> results = await feed.GetStopsByQueryAsync();
List<Stop> results = await feed.GetStopsByTimezoneAsync();
List<Stop> results = await feed.GetStopsByURLAsync();
List<Stop> results = await feed.GetStopsByWheelchairBoardingAsync();
List<Stop> results = await feed.GetStopsByZoneAsync();
```

## License

Licensed under the [MIT License](./LICENSE).