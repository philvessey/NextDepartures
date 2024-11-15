using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Agencies
    {
        [TestMethod]
        public async Task GetAgenciesByEmailAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByEmailAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByFareUrlAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByFareURLAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByLanguageCodeAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByLanguageCodeAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByPhoneAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByPhoneAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByQueryAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByQueryAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByTimezoneAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByTimezoneAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByUrlAsync()
        {
            var feed = await Feed.Load(GtfsStorage.Load("Data/gtfs.zip"));
            var results = await feed.GetAgenciesByURLAsync();

            Assert.IsNotNull(results);
        }
    }
}