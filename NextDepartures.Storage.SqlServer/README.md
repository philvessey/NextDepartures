# NextDepartures

NextDepartures is a .NET Library that queries GTFS (General Transit Feed Specification) data sets stored locally, in a SQLite database or in a SQL Server database. The library will work with any well-formed GTFS data set.

Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://dev.azure.com/philvessey/NextDepartures/_build/latest?definitionId=2&branchName=master)
NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## SQL Server Usage

Connect to the library:

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.SqlServer;

Feed feed = await Feed.Load(SqlServerStorage.Load([database], (prefix)));
```

## License

Licensed under the [MIT License](./LICENSE).