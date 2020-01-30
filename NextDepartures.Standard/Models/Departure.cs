using GTFS.Entities;
using System;

namespace NextDepartures.Standard.Models
{
    public class Departure
    {
        public TimeOfDay? DepartureTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string StopId { get; set; }
        public string TripId { get; set; }
        public string ServiceId { get; set; }
        public string TripHeadsign { get; set; }
        public string TripShortName { get; set; }
        public string AgencyId { get; set; }
        public string RouteLongName { get; set; }
        public string RouteShortName { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", DepartureTime, ServiceId);
        }
    }
}