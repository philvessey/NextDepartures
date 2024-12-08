# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General Transit Feed Specification) data sets stored locally, in a SQLite database or in a SQL Server database. The library will work with any well-formed GTFS data set.

## SQL Server Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.SqlServer;

Feed feed = await Feed.Load(SqlServerStorage.Load([database], (prefix)));
```