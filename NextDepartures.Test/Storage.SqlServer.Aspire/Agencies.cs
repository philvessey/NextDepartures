using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Test.Mock;

namespace NextDepartures.Test.Storage.SqlServer.Aspire;

[TestClass]
public class Agencies
{
    [TestMethod]
    public async Task GetAgenciesByEmailAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByEmailAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByFareUrlAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByFareUrlAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByIdAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByIdAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByLanguageCodeAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByLanguageCodeAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByNameAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByNameAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByPhoneAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByPhoneAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByQueryAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByQueryAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByTimezoneAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByTimezoneAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
    
    [TestMethod]
    public async Task GetAgenciesByUrlAsync()
    {
        var feed = await Feed.Load(dataStorage: new MockStorage());
        var results = await feed.GetAgenciesByUrlAsync();
        
        Assert.IsTrue(condition: results.Count > 0);
    }
}