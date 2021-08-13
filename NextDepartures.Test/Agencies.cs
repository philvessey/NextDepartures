using GTFS.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextDepartures.Standard;
using NextDepartures.Storage.GTFS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test
{
    [TestClass]
    public class Agencies
    {
        [TestMethod]
        public async Task GetAgenciesByEmailAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByEmailAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByFareURLAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByFareURLAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByLanguageCodeAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByLanguageCodeAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByPhoneAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByPhoneAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByQueryAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByQueryAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByTimezoneAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByTimezoneAsync();

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task GetAgenciesByURLAsync()
        {
            Feed feed = await Feed.Load(GTFSStorage.Load("Data/gtfs.zip"));
            List<Agency> results = await feed.GetAgenciesByURLAsync();

            Assert.IsNotNull(results);
        }
    }
}