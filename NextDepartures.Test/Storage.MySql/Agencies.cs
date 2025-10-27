using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Test.Mock;

namespace NextDepartures.Test.Storage.MySql;

[TestClass]
public class Agencies
{
    [TestMethod]
    public async Task GetAgenciesByEmailAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByEmailAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByFareUrlAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByFareUrlAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByIdAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByIdAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByLanguageCodeAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByLanguageCodeAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByNameAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByNameAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByPhoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByPhoneAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByQueryAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByQueryAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByTimezoneAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByTimezoneAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
    
    [TestMethod]
    public async Task GetAgenciesByUrlAsync()
    {
        var feed = await Feed.LoadAsync(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByUrlAsync();
        
        Assert.IsNotEmpty(collection: results);
    }
}