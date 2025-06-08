using System;
using GTFS.Entities;
using JetBrains.Annotations;

namespace NextDepartures.Standard.Models;

public class Service
{
    [UsedImplicitly]
    public string AgencyId { get; set; }
    
    [UsedImplicitly]
    public string AgencyName { get; set; }
    
    [UsedImplicitly]
    public DateTime DepartureDateTime { get; set; }
    
    [UsedImplicitly]
    public TimeOfDay? DepartureTime { get; set; }
    
    [UsedImplicitly]
    public string DestinationName { get; set; }
    
    [UsedImplicitly]
    public string RouteName { get; set; }
    
    [UsedImplicitly]
    public string StopId { get; set; }
    
    [UsedImplicitly]
    public string StopName { get; set; }
    
    [UsedImplicitly]
    public string TripId { get; set; }
    
    [UsedImplicitly]
    public string Type { get; set; }
    
    public override string ToString()
    {
        return Type is not "trip" ? $"[{DepartureDateTime}] {DestinationName}" : $"[{DepartureDateTime}] {StopName}";
    }
}