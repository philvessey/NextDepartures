using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Storage.GTFS;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            List<Service> resultsByParentStation = await feed.GetServicesByParentStationAsync("CIVC", DateTime.Parse("01/04/2023 18:00:00", CultureInfo.InvariantCulture), 12);

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", DateTime.Parse("01/04/2023 18:00:00", CultureInfo.InvariantCulture), 12);

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", DateTime.Parse("01/04/2023 18:00:00", CultureInfo.InvariantCulture), 12);
            List<Service> resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.FirstOrDefault().TripId, DateTime.Parse("01/04/2023 18:00:00", CultureInfo.InvariantCulture));

            Assert.IsNotNull(resultsByTrip);
        }
    }
}