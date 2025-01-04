# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General
Transit Feed Specification) data sets stored locally, in a SQLite
database or in a SQL Server database. The library will work with
any well-formed GTFS data set.

## Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

var feed = await Feed.Load(GtfsStorage.Load());
```