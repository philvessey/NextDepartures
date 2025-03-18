using System;
using GTFS.Entities;
using JetBrains.Annotations;

namespace NextDepartures.Standard.Models;

public class Departure
{
    [UsedImplicitly]
    public TimeOfDay DepartureTime { get; set; }
    
    [UsedImplicitly]
    public DateTime DepartureDateTime { get; set; }
    
    [UsedImplicitly]
    public string StopId { get; set; }
    
    [UsedImplicitly]
    public string TripId { get; set; }
    
    [UsedImplicitly]
    public string ServiceId { get; set; }
    
    [UsedImplicitly]
    public string TripHeadsign { get; set; }
    
    [UsedImplicitly]
    public string TripShortName { get; set; }
    
    [UsedImplicitly]
    public string AgencyId { get; set; }
    
    [UsedImplicitly]
    public string RouteLongName { get; set; }
    
    [UsedImplicitly]
    public string RouteShortName { get; set; }
    
    [UsedImplicitly]
    public bool Monday { get; set; }
    
    [UsedImplicitly]
    public bool Tuesday { get; set; }
    
    [UsedImplicitly]
    public bool Wednesday { get; set; }
    
    [UsedImplicitly]
    public bool Thursday { get; set; }
    
    [UsedImplicitly]
    public bool Friday { get; set; }
    
    [UsedImplicitly]
    public bool Saturday { get; set; }
    
    [UsedImplicitly]
    public bool Sunday { get; set; }
    
    [UsedImplicitly]
    public DateTime StartDate { get; set; }
    
    [UsedImplicitly]
    public DateTime EndDate { get; set; }
    
    public override string ToString()
    {
        return $"[{DepartureDateTime}] {ServiceId}";
    }
}