# NextDepartures

NextDepartures is a .NET Standard Library that queries GTFS (General Transit Feed Specification) data sets stored in an Azure SQL Database. The library will work with any well formed GTFS data set.

* Build Status: [![Build Status](https://philvessey.visualstudio.com/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://philvessey.visualstudio.com/NextDepartures/_build/latest?definitionId=2&branchName=master)
* NextDepartures.Standard: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)
* NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## Usage

```
NextDepartures.Database > dotnet run "[connection]" "[url]"
```

```csharp
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Storage.SqlServer;

Feed feed = await Feed.Load(new SqlStorage([connection]));
List<Agency> results = await feed.GetAgenciesByAllAsync([query], [timezone]);
List<Agency> results = await feed.GetAgenciesByQueryAsync([query]);
List<Agency> results = await feed.GetAgenciesByTimezoneAsync([timezone]);

Feed feed = await Feed.Load(new SqlStorage([connection]));
List<Service> results = await feed.GetServicesByStopAsync([id]);
List<Service> results = await feed.GetServicesByStopAsync([id], [now]);
List<Service> results = await feed.GetServicesByTripAsync([id]);
List<Service> results = await feed.GetServicesByTripAsync([id], [now]);

Feed feed = await Feed.Load(new SqlStorage([connection]));
List<Stop> results = await feed.GetStopsByAllAsync([minLon], [minLat], [maxLon], [maxLat], [query], [timezone]);
List<Stop> results = await feed.GetStopsByLocationAsync([minLon], [minLat], [maxLon], [maxLat]);
List<Stop> results = await feed.GetStopsByQueryAsync([query]);
List<Stop> results = await feed.GetStopsByTimezoneAsync([timezone]);
```

## License

Licensed under the [MIT License](./LICENSE).