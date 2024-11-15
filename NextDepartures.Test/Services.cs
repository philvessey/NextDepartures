using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System;
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
            var feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            var resultsByParentStation = await feed.GetServicesByParentStationAsync("CIVC", new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            var feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            var resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            var feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            var resultsByStop = await feed.GetServicesByStopAsync("CIVC", new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.Zero, 1, 10);
            var resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.First().TripId, new DateTime(2024, 11, 7, 18, 0, 0), TimeSpan.FromMinutes(10), 1, 10);

            Assert.IsNotNull(resultsByTrip);
        }
    }
}