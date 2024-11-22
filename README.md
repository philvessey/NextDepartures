# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General Transit Feed Specification) data sets stored locally or in an Azure SQL Database. The library will work with any well-formed GTFS data set.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://dev.azure.com/philvessey/NextDepartures/_build/latest?definitionId=2&branchName=master)
* NextDepartures.Standard: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)
* NextDepartures.Storage.GTFS: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.GTFS.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.GTFS/)
* NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## Local GTFS Usage

Connect to the library:

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

Feed feed = await Feed.Load(GtfsStorage.Load([path]));
```

* [path] > Path to GTFS data set .zip or directory. Required.

## SQL Server Usage

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
var results = await feed.GetAgenciesByEmailAsync();
var results = await feed.GetAgenciesByFareUrlAsync();
var results = await feed.GetAgenciesByLanguageCodeAsync();
var results = await feed.GetAgenciesByPhoneAsync();
var results = await feed.GetAgenciesByQueryAsync();
var results = await feed.GetAgenciesByTimezoneAsync();
var results = await feed.GetAgenciesByUrlAsync();
```

## Services Endpoints

Once connected to the library:

```csharp
var results = await feed.GetServicesByParentStationAsync();
var results = await feed.GetServicesByStopAsync();
var results = await feed.GetServicesByTripAsync();
```

## Stops Endpoints

Once connected to the library:

```csharp
var results = await feed.GetStopsByDescriptionAsync();
var results = await feed.GetStopsByLevelAsync();
var results = await feed.GetStopsByLocationAsync();
var results = await feed.GetStopsByLocationTypeAsync();
var results = await feed.GetStopsByParentStationAsync();
var results = await feed.GetStopsByPlatformCodeAsync();
var results = await feed.GetStopsByQueryAsync();
var results = await feed.GetStopsByTimezoneAsync();
var results = await feed.GetStopsByUrlAsync();
var results = await feed.GetStopsByWheelchairBoardingAsync();
var results = await feed.GetStopsByZoneAsync();
```

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/philvessey"><img src="https://avatars.githubusercontent.com/u/814882?v=4?s=100" width="100px;" alt="Phil Vessey"/><br /><sub><b>Phil Vessey</b></sub></a><br /><a href="#code-philvessey" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/hypervtechnics"><img src="https://avatars.githubusercontent.com/u/10027956?v=4?s=100" width="100px;" alt="hypervtechnics"/><br /><sub><b>hypervtechnics</b></sub></a><br /><a href="#code-hypervtechnics" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

## License

Licensed under the [MIT License](./LICENSE).