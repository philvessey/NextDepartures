using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;

namespace NextDepartures.Test.Storage.GTFS;

[TestClass]
public class Stops
{
    [TestMethod]
    public async Task GetStopsByCodeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByCodeAsync();

        Assert.IsTrue(results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByDescriptionAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByDescriptionAsync();

        Assert.IsTrue(results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByIdAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByIdAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLevelAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLevelAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLocationAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLocationAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByLocationTypeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLocationTypeAsync();

        Assert.IsTrue(results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByNameAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByNameAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByParentStationAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByParentStationAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByPlatformCodeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByPlatformCodeAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByQueryAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByQueryAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByTimezoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByTimezoneAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByUrlAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByUrlAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByWheelchairBoardingAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByWheelchairBoardingAsync();

        Assert.IsTrue(results.Count > 0);
    }

    [TestMethod]
    public async Task GetStopsByZoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByZoneAsync();

        Assert.IsTrue(results.Count > 0);
    }
}