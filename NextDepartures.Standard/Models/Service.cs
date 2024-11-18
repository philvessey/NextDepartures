using GTFS.Entities;
using System;

namespace NextDepartures.Standard.Models
{
    public class Service
    {
        public string AgencyId { get; init; }
        public string AgencyName { get; init; }
        public DateTime DepartureDateTime { get; init; }
        public TimeOfDay? DepartureTime { get; init; }
        public string DestinationName { get; init; }
        public string RouteName { get; init; }
        public string StopId { get; init; }
        public string StopName { get; init; }
        public string TripId { get; init; }
        public string Type { get; init; }
        
        public override string ToString()
        {
            return Type != "trip" ? $"[{DepartureDateTime}] {DestinationName}" : $"[{DepartureDateTime}] {StopName}";
        }
    }
}