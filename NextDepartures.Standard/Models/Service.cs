using GTFS.Entities;
using System;

namespace NextDepartures.Standard.Models
{
    public class Service
    {
        public string AgencyName { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public TimeOfDay? DepartureTime { get; set; }
        public string DestinationName { get; set; }
        public string RouteName { get; set; }
        public string StopName { get; set; }
        public string TripId { get; set; }
    }
}