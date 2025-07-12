//resharper disable All

using System;
using NextDepartures.Storage.Sqlite.Extensions;

namespace NextDepartures.Storage.Sqlite.Utils;

public static class DistanceUtils
{
    public static double GetFromPoint(
        double originLongitude,
        double originLatitude,
        double destinationLongitude,
        double destinationLatitude) {
        
        var deltaLatitude = (destinationLatitude - originLatitude).ToRadians();
        var deltaLongitude = (destinationLongitude - originLongitude).ToRadians();
        
        var a = GetHaversineFormula(
            deltaLatitude: deltaLatitude / 2,
            deltaLongitude: deltaLongitude / 2);
        
        var b = GetHaversineFormula(
            originLatitude: originLatitude.ToRadians(),
            destinationLatitude: destinationLatitude.ToRadians(),
            deltaLatitude: deltaLatitude / 2,
            deltaLongitude: deltaLongitude / 2);
        
        var angle = 2 * Math.Atan2(
            y: Math.Sqrt(d: a + b),
            x: Math.Sqrt(d: 1 - (a + b)));
        
        return angle * 6371;
    }
    
    private static double GetHaversineFormula(
        double deltaLatitude,
        double deltaLongitude) {
        
        return Math.Sin(a: deltaLatitude) *
               Math.Sin(a: deltaLatitude);
    }
    
    private static double GetHaversineFormula(
        double originLatitude,
        double destinationLatitude,
        double deltaLatitude,
        double deltaLongitude) {
        
        return Math.Cos(d: originLatitude) *
               Math.Cos(d: destinationLatitude) *
               Math.Sin(a: deltaLongitude) *
               Math.Sin(a: deltaLongitude);
    }
}