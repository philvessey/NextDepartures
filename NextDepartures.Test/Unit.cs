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
        public async Task GetServicesByStopAsync()
        {
            Feed feed = new Feed("");
            List<Service> results = await feed.GetServicesByStopAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = new Feed("");
            List<Service> results = await feed.GetServicesByTripAsync("");

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
        public async Task GetStopsByQueryAsync()
        {
            Feed feed = new Feed("");
            List<Stop> results = await feed.GetStopsByQueryAsync("");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetStopsByWildcardAsync()
        {
            Feed feed = new Feed("");
            List<Stop> results = await feed.GetStopsByWildcardAsync(0, 0, 0, 0, "");

            Assert.IsNotNull(results);
        }
    }
}