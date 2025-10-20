using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Test.Mock;

namespace NextDepartures.Test.Storage.MySql;

[TestClass]
public class Stops
{
    [TestMethod]
    public async Task GetStopsByCodeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByCodeAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByDescriptionAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByDescriptionAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByIdAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByIdAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByLevelAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLevelAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByLocationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLocationAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByLocationTypeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByLocationTypeAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByNameAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByNameAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByParentStationAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByParentStationAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByPlatformCodeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByPlatformCodeAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByPointAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByPointAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByQueryAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByQueryAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByTimezoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByTimezoneAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByUrlAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByUrlAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByWheelchairBoardingAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByWheelchairBoardingAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetStopsByZoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetStopsByZoneAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
}