# NextDepartures

NextDepartures is a .NET Standard Library that queries GTFS (General Transit Feed Specification) data sets stored in an Azure SQL Database. The library will work with any well formed GTFS data set.

[![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)

## Usage

```
NextDepartures.Database > dotnet run "[connection]" "[url]"
```

```csharp
using NextDepartures.Standard;

Feed feed = new Feed([connection]);
List<Service> results = feed.GetServicesByStop([id]);
List<Service> results = await feed.GetServicesByStopAsync([id]);
List<Service> results = feed.GetServicesByTrip([id]);
List<Service> results = await feed.GetServicesByTripAsync([id]);

Feed feed = new Feed([connection]);
List<Stop> results = feed.GetStopsByLocation([minLon], [minLat], [maxLon], [maxLat]);
List<Stop> results = await feed.GetStopsByLocationAsync([minLon], [minLat], [maxLon], [maxLat]);
List<Stop> results = feed.GetStopsByQuery([query]);
List<Stop> results = await feed.GetStopsByQueryAsync([query]);
List<Stop> results = feed.GetStopsByWildcard([minLon], [minLat], [maxLon], [maxLat], [query]);
List<Stop> results = await feed.GetStopsByWildcardAsync([minLon], [minLat], [maxLon], [maxLat], [query]);
```

## License

Licensed under the [MIT License](./LICENSE).