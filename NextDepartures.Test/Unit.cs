using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Standard.Models;
using NextDepartures.Test.Mocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    /// <summary>
    /// Represents unit tests.
    /// </summary>
    [TestClass]
    public class Unit
    {
        /// <summary>
        /// Gets the services for a specific stop.
        /// </summary>
        [TestMethod]
        public async Task GetServicesByStopAsync()
        {
            Feed feed = new Feed(new EmptyStorage());
            List<Service> results = await feed.GetServicesByStopAsync("");

            Assert.IsNotNull(results);
        }

        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        [TestMethod]
        public async Task GetServicesByTripAsync()
        {
            Feed feed = new Feed(new EmptyStorage());
            List<Service> results = await feed.GetServicesByTripAsync("");

            Assert.IsNotNull(results);
        }

        /// <summary>
        /// Gets the stops by the given area and query.
        /// </summary>
        [TestMethod]
        public async Task GetStopsByAllAsync()
        {
            Feed feed = new Feed(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByAllAsync(0, 0, 0, 0, "");

            Assert.IsNotNull(results);
        }

        /// <summary>
        /// Gets the stops in the given area.
        /// </summary>
        [TestMethod]
        public async Task GetStopsByLocationAsync()
        {
            Feed feed = new Feed(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByLocationAsync(0, 0, 0, 0);

            Assert.IsNotNull(results);
        }

        /// <summary>
        /// Gets the stops by the given query.
        /// </summary>
        [TestMethod]
        public async Task GetStopsByQueryAsync()
        {
            Feed feed = new Feed(new EmptyStorage());
            List<Stop> results = await feed.GetStopsByQueryAsync("");

            Assert.IsNotNull(results);
        }
    }
}