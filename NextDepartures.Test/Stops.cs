using GTFS.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Stops
    {
        [TestMethod]
        public async Task GetStopsByDescriptionAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByDescriptionAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByLevelAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByLevelAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByLocationAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByLocationAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByLocationTypeAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByLocationTypeAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByParentStationAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByParentStationAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByPlatformCodeAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByPlatformCodeAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByQueryAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByQueryAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByTimezoneAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByTimezoneAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByURLAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByURLAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByWheelchairBoardingAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByWheelchairBoardingAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByZoneAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Stop> results = await feed.GetStopsByZoneAsync();

            Assert.IsNotNull(results);
        }
    }
}