# NextDepartures

NextDepartures is a .NET Standard Library that queries GTFS (General Transit Feed Specification) data sets stored in an Azure SQL Database. The library will work with any well formed GTFS data set.

## Usage

```csharp
using NextDepartures.Standard;

Feed feed = new Feed([connection]);
List<Stop> results = feed.GetStopsByQuery([query]);
List<Stop> results = await feed.GetStopsByQueryAsync([query]);

Feed feed = new Feed([connection]);
List<Service> results = feed.GetServicesByStop([id]);
List<Service> results = await feed.GetServicesByStopAsync([id]);
```

## License

Licensed under the [MIT License](./LICENSE).