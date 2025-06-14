using System.Linq;
using GTFS.Entities;
using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions;

public static class StringExtensions
{
    public static string ToExcludePrefix(
        this string baseString,
        string prefix) {
        
        return baseString.Replace(
            oldValue: prefix,
            newValue: string.Empty);
    }
    
    public static string ToIncludePrefix(
        this string baseString,
        string prefix) {
        
        return prefix + baseString;
    }
    
    public static LocationType ToLocationType(this string baseString)
    {
        return baseString switch
        {
            "0" => LocationType.Stop,
            "1" => LocationType.Station,
            "2" => LocationType.EntranceExit,
            "3" => LocationType.GenericNode,
            "4" => LocationType.BoardingArea,
            _ => LocationType.Stop
        };
    }
    
    public static TimeOfDay ToTimeOfDay(this string baseString)
    {
        var value = baseString
            .Split(separator: ":")
            .Select(selector: int.Parse)
            .ToArray();
        
        return new TimeOfDay
        {
            Hours = value.ElementAt(index: 0),
            Minutes = value.ElementAt(index: 1),
            Seconds = value.ElementAt(index: 2)
        };
    }
}