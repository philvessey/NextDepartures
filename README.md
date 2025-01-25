# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General
Transit Feed Specification) data sets stored locally, or in a
database such as PostgreSQL, SQLite or SQL Server. The library 
will work with any well-formed GTFS data set.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://dev.azure.com/philvessey/NextDepartures/_build/latest?definitionId=2&branchName=master)
* NextDepartures.Standard: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)
* NextDepartures.Storage.GTFS: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.GTFS.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.GTFS/)
* NextDepartures.Storage.Postgres: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.Postgres.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.Postgres/)
* NextDepartures.Storage.Sqlite: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.Sqlite.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.Sqlite/)
* NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## Local Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

var feed = await Feed.Load(GtfsStorage.Load());
```

## ProgreSQL Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.Postgres;

var feed = await Feed.Load(PostgresStorage.Load());
```

## SQLite Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.Sqlite;

var feed = await Feed.Load(SqliteStorage.Load());
```

## SQL Server Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.SqlServer;

var feed = await Feed.Load(SqlServerStorage.Load());
```

## Agencies Endpoints

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

## Services Endpoints

```csharp
var results = await feed.GetServicesByParentStationAsync();
var results = await feed.GetServicesByStopAsync();
var results = await feed.GetServicesByTripAsync();
```

## Stops Endpoints

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
var results = await feed.GetStopsByQueryAsync();
var results = await feed.GetStopsByTimezoneAsync();
var results = await feed.GetStopsByUrlAsync();
var results = await feed.GetStopsByWheelchairBoardingAsync();
var results = await feed.GetStopsByZoneAsync();
```

## License

Licensed under the [MIT License](./LICENSE).