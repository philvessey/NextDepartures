using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Unit
    {
        [TestMethod]
        public void GetStopsByQuery()
        {
            Feed feed = new Feed("");
            List<Stop> results = feed.GetStopsByQuery("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByQueryAsync()
        {
            Feed feed = new Feed("");
            List<Stop> results = await feed.GetStopsByQueryAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void GetStopsByLocation()
        {
            Feed feed = new Feed("");
            List<Stop> results = feed.GetStopsByLocation(0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByLocationAsync()
        {
            Feed feed = new Feed("");
            List<Stop> results = await feed.GetStopsByLocationAsync(0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void GetStopsByQueryAndLocation()
        {
            Feed feed = new Feed("");
            List<Stop> results = feed.GetStopsByQueryAndLocation("", 0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByQueryAndLocationAsync()
        {
            Feed feed = new Feed("");
            List<Stop> results = await feed.GetStopsByQueryAndLocationAsync("", 0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void GetServicesByStop()
        {
            Feed feed = new Feed("");
            List<Service> results = feed.GetServicesByStop("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = new Feed("");
            List<Service> results = await feed.GetServicesByStopAsync("");

            Assert.IsNotNull(results);
        }
    }
}