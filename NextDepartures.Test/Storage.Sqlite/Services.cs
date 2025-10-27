using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Types;
using NextDepartures.Storage.Sqlite;

namespace NextDepartures.Test.Storage.Sqlite;

[TestClass]
public class Services
{
    [TestMethod]
    public async Task GetServicesByParentStationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: SqliteStorage.Load(connectionString: "Data Source=Data/feed.db;"));
        
        var resultsByParentStation = await feed.GetServicesByParentStationAsync(
            stops: ["16TH", "24TH"],
            target: new DateTime(
                year: 2025,
                month: 1,
                day: 21,
                hour: 18,
                minute: 0,
                second: 0),
            offset: TimeSpan.Zero,
            comparison: ComparisonType.Exact,
            tolerance: TimeSpan.FromHours(value: 1),
            results: 10);
        
        Assert.IsNotEmpty(collection: resultsByParentStation);
    }
    
    [TestMethod]
    public async Task GetServicesByStopAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: SqliteStorage.Load(connectionString: "Data Source=Data/feed.db;"));
        
        var resultsByStop = await feed.GetServicesByStopAsync(
            id: "16TH",
            target: new DateTime(
                year: 2025,
                month: 1,
                day: 21,
                hour: 18,
                minute: 0,
                second: 0),
            offset: TimeSpan.Zero,
            comparison: ComparisonType.Exact,
            tolerance: TimeSpan.FromHours(value: 1),
            results: 10);
        
        Assert.IsNotEmpty(collection: resultsByStop);
    }
    
    [TestMethod]
    public async Task GetServicesByTripAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: SqliteStorage.Load(connectionString: "Data Source=Data/feed.db;"));
        
        var resultsByStop = await feed.GetServicesByStopAsync(
            id: "24TH",
            target: new DateTime(
                year: 2025,
                month: 1,
                day: 21,
                hour: 18,
                minute: 0,
                second: 0),
            offset: TimeSpan.Zero,
            comparison: ComparisonType.Exact,
            tolerance: TimeSpan.FromHours(value: 1),
            results: 10);
        
        var resultsByTrip = await feed.GetServicesByTripAsync(
            id: resultsByStop.First().TripId,
            target: new DateTime(
                year: 2025,
                month: 1,
                day: 21,
                hour: 18,
                minute: 0,
                second: 0),
            offset: TimeSpan.FromMinutes(value: 5),
            comparison: ComparisonType.Exact,
            tolerance: TimeSpan.FromHours(value: 1),
            results: 2);
        
        Assert.IsNotEmpty(collection: resultsByTrip);
    }
}