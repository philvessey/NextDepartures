using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Types;
using NextDepartures.Storage.GTFS;

namespace NextDepartures.Test.Storage.GTFS;

[TestClass]
public class Services
{
    [TestMethod]
    public async Task GetServicesByParentStationAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var resultsByParentStation = await feed.GetServicesByParentStationAsync(["16TH", "24TH"], new DateTime(2025, 1, 21, 18, 0, 0), TimeSpan.Zero, ComparisonType.Exact, 1, 10);

        Assert.IsTrue(resultsByParentStation.Count > 0);
    }

    [TestMethod]
    public async Task GetServicesByStopAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var resultsByStop = await feed.GetServicesByStopAsync("16TH", new DateTime(2025, 1, 21, 18, 0, 0), TimeSpan.Zero, ComparisonType.Exact, 1, 10);

        Assert.IsTrue(resultsByStop.Count > 0);
    }

    [TestMethod]
    public async Task GetServicesByTripAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var resultsByStop = await feed.GetServicesByStopAsync("24TH", new DateTime(2025, 1, 21, 18, 0, 0), TimeSpan.Zero, ComparisonType.Exact, 1, 10);
        var resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.First().TripId, new DateTime(2025, 1, 21, 18, 0, 0), TimeSpan.FromMinutes(10), ComparisonType.Exact, 1, 10);

        Assert.IsTrue(resultsByTrip.Count > 0);
    }
}