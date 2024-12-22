using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.Sqlite;
using System.Threading.Tasks;

namespace NextDepartures.Test.Storage.Sqlite;

[TestClass]
public class Stops
{
    [TestMethod]
    public async Task GetStopsByDescriptionAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByDescriptionAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLevelAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByLevelAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLocationAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByLocationAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLocationTypeAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByLocationTypeAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByParentStationAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByParentStationAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByPlatformCodeAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByPlatformCodeAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByQueryAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByQueryAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByTimezoneAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByTimezoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByUrlAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByUrlAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByWheelchairBoardingAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByWheelchairBoardingAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByZoneAsync()
    {
        var feed = await Feed.Load(SqliteStorage.Load("Data Source=Data/feed.db;"));
        var results = await feed.GetStopsByZoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
    }
}