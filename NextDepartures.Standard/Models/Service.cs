using GTFS.Entities;
using System;

namespace NextDepartures.Standard.Models
{
    public class Service
    {
        public string AgencyId { get; set; }
        public string AgencyName { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public TimeOfDay? DepartureTime { get; set; }
        public string DestinationName { get; set; }
        public string RouteName { get; set; }
        public string StopId { get; set; }
        public string StopName { get; set; }
        public string TripId { get; set; }
        public string Type { get; set; }
        
        public override string ToString()
        {
            return Type != "trip" ? string.Format("[{0}] {1}", DepartureDateTime, DestinationName) : string.Format("[{0}] {1}", DepartureDateTime, StopName);
        }
    }
}