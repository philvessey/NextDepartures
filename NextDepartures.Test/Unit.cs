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