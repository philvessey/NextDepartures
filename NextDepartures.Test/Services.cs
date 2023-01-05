using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Extensions;
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
            List<Service> resultsByParentStation = await feed.GetServicesByParentStationAsync("CIVC", DateTime.Parse("04/01/2023 18:00:00").ToZonedDateTime("Europe/London"), 12);

            Assert.IsNotNull(resultsByParentStation);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", DateTime.Parse("04/01/2023 18:00:00").ToZonedDateTime("Europe/London"), 12);

            Assert.IsNotNull(resultsByStop);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Service> resultsByStop = await feed.GetServicesByStopAsync("CIVC", DateTime.Parse("04/01/2023 18:00:00").ToZonedDateTime("Europe/London"), 12);
            List<Service> resultsByTrip = await feed.GetServicesByTripAsync(resultsByStop.FirstOrDefault().TripId, DateTime.Parse("04/01/2023 18:00:00").ToZonedDateTime("Europe/London"));

            Assert.IsNotNull(resultsByTrip);
        }
    }
}