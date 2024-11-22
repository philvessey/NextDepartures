using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System.Threading.Tasks;

namespace NextDepartures.Test;

[TestClass]
public class Agencies
{
    [TestMethod]
    public async Task GetAgenciesByEmailAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByEmailAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByFareUrlAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByFareUrlAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByLanguageCodeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByLanguageCodeAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByPhoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByPhoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByQueryAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByQueryAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByTimezoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByTimezoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }

    [TestMethod]
    public async Task GetAgenciesByUrlAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
        var results = await feed.GetAgenciesByUrlAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var agency in results)
        {
            Assert.IsNotNull(agency.Id);
            Assert.IsNotNull(agency.Name);
            Assert.IsNotNull(agency.URL);
            Assert.IsNotNull(agency.Timezone);
        }
    }
}