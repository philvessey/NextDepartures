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
            List<Service> resultsByParentStation = await feed.GetServicesByParentStationAsync("CIVC", new DateTime(2023, 8, 14, 18, 0, 0), TimeSpan.Zero, 1, 10);

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2023, 8, 14, 18, 0, 0), TimeSpan.Zero, 1, 10);

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2023, 8, 14, 18, 0, 0), TimeSpan.Zero, 1, 10);
            List<Service> resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.FirstOrDefault().TripId, new DateTime(2023, 8, 14, 18, 0, 0), TimeSpan.FromMinutes(10), 1, 10);

            Assert.IsNotNull(resultsByTrip);
        }
    }
}