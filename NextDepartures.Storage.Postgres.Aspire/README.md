# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General 
Transit Feed Specification) data sets stored locally, or in a 
database such as MySQL, PostgreSQL, SQLite or SQL Server. The 
library will work with any well-formed GTFS data set.

## Usage

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.Postgres.Aspire;

var feed = await Feed.LoadAsync(PostgresStorage.Load());
```