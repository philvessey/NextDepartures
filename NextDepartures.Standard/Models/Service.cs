using GTFS.Entities;
using JetBrains.Annotations;
using System;

namespace NextDepartures.Standard.Models;

public class Service
{
    [UsedImplicitly]
    public string AgencyId { get; init; }
    
    [UsedImplicitly]
    public string AgencyName { get; init; }
    
    [UsedImplicitly]
    public DateTime DepartureDateTime { get; init; }
    
    [UsedImplicitly]
    public TimeOfDay? DepartureTime { get; init; }
    
    [UsedImplicitly]
    public string DestinationName { get; init; }
    
    [UsedImplicitly]
    public string RouteName { get; init; }
    
    [UsedImplicitly]
    public string StopId { get; init; }
    
    [UsedImplicitly]
    public string StopName { get; init; }
    
    [UsedImplicitly]
    public string TripId { get; init; }
    
    [UsedImplicitly]
    public string Type { get; init; }
        
    public override string ToString()
    {
        return Type != "trip" ? $"[{DepartureDateTime}] {DestinationName}" : $"[{DepartureDateTime}] {StopName}";
    }
}