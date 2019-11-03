# NextDepartures

NextDepartures is a .NET Standard Library that queries GTFS (General Transit Feed Specification) data sets stored in an Azure SQL Database. The library will work with any well formed GTFS data set.

[![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)

## Usage

```
NextDepartures.Database > dotnet run "[connection]" "[url]"
```

```csharp
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Storage.SqlServer;

Feed feed = new Feed(new SqlStorage([connection]));
List<Service> results = await feed.GetServicesByStopAsync([id]);
List<Service> results = await feed.GetServicesByTripAsync([id]);

Feed feed = new Feed(new SqlStorage([connection]));
List<Stop> results = await feed.GetStopsByAllAsync([minLon], [minLat], [maxLon], [maxLat], [query], [timezone]);
List<Stop> results = await feed.GetStopsByLocationAsync([minLon], [minLat], [maxLon], [maxLat]);
List<Stop> results = await feed.GetStopsByQueryAsync([query]);
List<Stop> results = await feed.GetStopsByTimezoneAsync([timezone]);
```

## License

Licensed under the [MIT License](./LICENSE).