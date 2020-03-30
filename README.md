# NextDepartures

NextDepartures is a .NET Standard Library that queries GTFS (General Transit Feed Specification) data sets stored locally or in an Azure SQL Database. The library will work with any well formed GTFS data set.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/NextDepartures/_apis/build/status/philvessey.NextDepartures?branchName=master)](https://dev.azure.com/philvessey/NextDepartures/_build/latest?definitionId=2&branchName=master)
* NextDepartures.Standard: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Standard.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Standard/)
* NextDepartures.Storage.GTFS: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.GTFS.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.GTFS/)
* NextDepartures.Storage.SqlServer: [![NuGet Version](https://img.shields.io/nuget/v/NextDepartures.Storage.SqlServer.svg?style=flat)](https://www.nuget.org/packages/NextDepartures.Storage.SqlServer/)

## Usage

```
NextDepartures.Database > dotnet run "[connection]" "[path]"
```

```csharp
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using NextDepartures.Storage.SqlServer;

Feed feed = await Feed.Load(GTFSStorage.Load([path]));
Feed feed = await Feed.Load(SqlServerStorage.Load([connection]));
```

If you are working with a UNIX based system and using the NextDepartures.Storage.SqlServer library you may need to include the following setting in your database connection string:

```
MultipleActiveResultSets=True;
```

## License

Licensed under the [MIT License](./LICENSE).