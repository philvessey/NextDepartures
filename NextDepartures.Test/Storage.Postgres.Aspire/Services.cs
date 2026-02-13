using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Types;
using NextDepartures.Test.Mock;

namespace NextDepartures.Test.Storage.Postgres.Aspire;

[TestClass]
public class Services
{
    [TestMethod]
    public async Task GetServicesByParentStationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        
        var resultsByParentStation = await feed.GetServicesByParentStationAsync(
            stops: ["M50-1", "M50-2"],
            target: new DateTime(
                year: 2026,
                month: 1,
                day: 12,
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
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        
        var resultsByStop = await feed.GetServicesByStopAsync(
            id: "M50-1",
            target: new DateTime(
                year: 2026,
                month: 1,
                day: 12,
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
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        
        var resultsByStop = await feed.GetServicesByStopAsync(
            id: "M50-1",
            target: new DateTime(
                year: 2026,
                month: 1,
                day: 12,
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
                year: 2026,
                month: 1,
                day: 12,
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