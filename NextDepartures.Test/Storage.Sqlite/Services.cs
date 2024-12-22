using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.Sqlite;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Test.Storage.Sqlite;

[TestClass]
public class Services
{
    [TestMethod]
    public async Task GetServicesByParentStationAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var resultsByStop = await feed.GetStopsByQueryAsync("Mission");
        var resultsByParentStation = await feed.GetServicesByParentStationAsync(resultsByStop, new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);

        Assert.IsNotNull(resultsByParentStation);
        Assert.IsTrue(resultsByParentStation.Count > 0);
    }

    [TestMethod]
    public async Task GetServicesByStopAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var resultsByStop = await feed.GetServicesByStopAsync("16TH", new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);

        Assert.IsNotNull(resultsByStop);
        Assert.IsTrue(resultsByStop.Count > 0);
    }

    [TestMethod]
    public async Task GetServicesByTripAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var resultsByStop = await feed.GetServicesByStopAsync("24TH", new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);
        var resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.First().TripId, new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.FromMinutes(10), 1, 10);

        Assert.IsNotNull(resultsByTrip);
        Assert.IsTrue(resultsByTrip.Count > 0);
    }
}