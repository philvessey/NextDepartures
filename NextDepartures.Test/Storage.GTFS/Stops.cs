using GTFS.Entities.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System.Threading.Tasks;

namespace NextDepartures.Test.Storage.GTFS;

[TestClass]
public class Stops
{
    [TestMethod]
    public async Task GetStopsByDescriptionAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByDescriptionAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByLevelAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLevelAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByLocationAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLocationAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByLocationTypeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByLocationTypeAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByParentStationAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByParentStationAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByPlatformCodeAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByPlatformCodeAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByQueryAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByQueryAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByTimezoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByTimezoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByUrlAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByUrlAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByWheelchairBoardingAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByWheelchairBoardingAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }

    [TestMethod]
    public async Task GetStopsByZoneAsync()
    {
        var feed = await Feed.Load(GtfsStorage.Load("Data/feed.zip"));
        var results = await feed.GetStopsByZoneAsync();

        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.Stop or LocationType.Station or LocationType.EntranceExit))
                continue;
            
            Assert.IsNotNull(stop.Name);
            Assert.IsNotNull(stop.Latitude);
            Assert.IsNotNull(stop.Longitude);
        }
        
        foreach (var stop in results)
        {
            Assert.IsNotNull(stop.Id);

            if (stop.LocationType is not (LocationType.EntranceExit or LocationType.GenericNode or LocationType.BoardingArea))
                continue;

            Assert.IsNotNull(stop.ParentStation);
        }
    }
}