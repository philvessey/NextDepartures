# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General Transit Feed Specification) data sets stored locally, in a SQLite database or in a SQL Server database. The library will work with any well-formed GTFS data set.

Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)]

## SQLite Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.Sqlite;

Feed feed = await Feed.Load(SqliteStorage.Load([database], (prefix)));
```