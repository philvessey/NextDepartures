using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Storage.GTFS;
using System;
using System.Collections.Generic;
using System.Linq;
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
            List<Service> resultsByParentStation = await feed.GetServicesByParentStationAsync("CIVC", new DateTime(2023, 1, 4, 18, 0, 0), 1);

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2023, 1, 4, 18, 0, 0), 1);

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2023, 1, 4, 18, 0, 0), 1);
            List<Service> resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.FirstOrDefault().TripId, new DateTime(2023, 1, 4, 18, 0, 0), 1);

            Assert.IsNotNull(resultsByTrip);
        }
    }
}