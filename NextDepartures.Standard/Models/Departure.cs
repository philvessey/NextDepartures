using GTFS.Entities;
using JetBrains.Annotations;
using System;

namespace NextDepartures.Standard.Models;

public class Departure
{
    [UsedImplicitly]
    public TimeOfDay? DepartureTime { get; init; }
    
    [UsedImplicitly]
    public DateTime DepartureDateTime { get; init; }
    
    [UsedImplicitly]
    public string StopId { get; init; }
    
    [UsedImplicitly]
    public string TripId { get; init; }
    
    [UsedImplicitly]
    public string ServiceId { get; init; }
    
    [UsedImplicitly]
    public string TripHeadsign { get; init; }
    
    [UsedImplicitly]
    public string TripShortName { get; init; }
    
    [UsedImplicitly]
    public string AgencyId { get; init; }
    
    [UsedImplicitly]
    public string RouteLongName { get; init; }
    
    [UsedImplicitly]
    public string RouteShortName { get; init; }
    
    [UsedImplicitly]
    public bool Monday { get; init; }
    
    [UsedImplicitly]
    public bool Tuesday { get; init; }
    
    [UsedImplicitly]
    public bool Wednesday { get; init; }
    
    [UsedImplicitly]
    public bool Thursday { get; init; }
    
    [UsedImplicitly]
    public bool Friday { get; init; }
    
    [UsedImplicitly]
    public bool Saturday { get; init; }
    
    [UsedImplicitly]
    public bool Sunday { get; init; }
    
    [UsedImplicitly]
    public DateTime EndDate { get; init; }
    
    [UsedImplicitly]
    public DateTime StartDate { get; init; }

    public override string ToString()
    {
        return $"[{DepartureDateTime}] {ServiceId}";
    }
}