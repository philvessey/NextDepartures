using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Test.Mock;

namespace NextDepartures.Test.Storage.SqlServer;

[TestClass]
public class Stops
{
    [TestMethod]
    public async Task GetStopsByCodeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByCodeAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByDescriptionAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByDescriptionAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByIdAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByIdAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByLevelAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLevelAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByLocationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLocationAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByLocationTypeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLocationTypeAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByNameAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByNameAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByParentStationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByParentStationAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByPlatformCodeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByPlatformCodeAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByQueryAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByQueryAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByTimezoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByTimezoneAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByUrlAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByUrlAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByWheelchairBoardingAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByWheelchairBoardingAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetStopsByZoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByZoneAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
}