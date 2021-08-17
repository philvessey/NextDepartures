using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Storage.GTFS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Services
    {
        [TestMethod]
        public async Task GetServicesByParentStationAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByParentStation = await feed.GetServicesByParentStationAsync("370G105120", new DateTime(2021, 8, 17, 12, 0, 0));

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("370040413", new DateTime(2021, 8, 17, 12, 0, 0));

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("370040413", new DateTime(2021, 8, 17, 12, 0, 0));
            List<Service> resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop[0].TripId, new DateTime(2021, 8, 17, 12, 0, 0));

            Assert.IsNotNull(resultsByTrip);
        }
    }
}