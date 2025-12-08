# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General 
Transit Feed Specification) data sets stored locally, or in a 
database such as MySQL, PostgreSQL, SQLite or SQL Server. The 
library will work with any well-formed GTFS data set.

[![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.GTFS.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.GTFS/)

## Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

var feed = await Feed.LoadAsync(GtfsStorage.Load());
```