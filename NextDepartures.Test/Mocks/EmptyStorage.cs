using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDepartures.Test.Mocks
{
    /// <summary>
    /// Represents an empty storage.
    /// </summary>
    public class EmptyStorage : IDataStorage
    {
        public Task<List<Agency>> GetAgenciesAsync()
        {
            return Task.FromResult(new List<Agency>());
        }

        public Task<List<Departure>> GetDeparturesForStopAsync(string id)
        {
            return Task.FromResult(new List<Departure>());
        }

        public Task<List<Departure>> GetDeparturesForTripAsync(string id)
        {
            return Task.FromResult(new List<Departure>());
        }

        public Task<List<Exception>> GetExceptionsAsync()
        {
            return Task.FromResult(new List<Exception>());
        }

        public Task<List<Stop>> GetStopsAsync()
        {
            return Task.FromResult(new List<Stop>());
        }

        public Task<List<Stop>> GetStopsByLocationAndQueryAsync(double minLon, double minLat, double maxLon, double maxLat, string query)
        {
            return Task.FromResult(new List<Stop>());
        }

        public Task<List<Stop>> GetStopsByLocationAsync(double minLon, double minLat, double maxLon, double maxLat)
        {
            return Task.FromResult(new List<Stop>());
        }

        public Task<List<Stop>> GetStopsByQueryAsync(string query)
        {
            return Task.FromResult(new List<Stop>());
        }
    }
}
