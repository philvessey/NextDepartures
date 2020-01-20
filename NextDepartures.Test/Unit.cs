using GTFS.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Test.Mocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Unit
    {
        [TestMethod]
        public async Task GetAgenciesByQueryAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Agency> results = await feed.GetAgenciesByQueryAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByTimezoneAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Agency> results = await feed.GetAgenciesByTimezoneAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Service> results = await feed.GetServicesByStopAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Service> results = await feed.GetServicesByTripAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByLocationAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByLocationAsync(0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByQueryAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByQueryAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByTimezoneAsync()
        {
            Feed feed = await Feed.Load(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByTimezoneAsync("");

            Assert.IsNotNull(results);
        }
    }
}