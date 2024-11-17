using GTFS.Entities;
using System;

namespace NextDepartures.Standard.Models
{
    public class Departure
    {
        public TimeOfDay? DepartureTime { get; init; }
        public DateTime DepartureDateTime { get; init; }
        public string StopId { get; init; }
        public string TripId { get; init; }
        public string ServiceId { get; init; }
        public string TripHeadsign { get; init; }
        public string TripShortName { get; init; }
        public string AgencyId { get; init; }
        public string RouteLongName { get; init; }
        public string RouteShortName { get; init; }
        public bool Monday { get; init; }
        public bool Tuesday { get; init; }
        public bool Wednesday { get; init; }
        public bool Thursday { get; init; }
        public bool Friday { get; init; }
        public bool Saturday { get; init; }
        public bool Sunday { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime StartDate { get; init; }

        public override string ToString()
        {
            return $"[{DepartureDateTime}] {ServiceId}";
        }
    }
}