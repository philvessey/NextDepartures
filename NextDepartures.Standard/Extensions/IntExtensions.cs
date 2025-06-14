using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions;

public static class IntExtensions
{
    public static bool ToBool(this short baseInt)
    {
        return baseInt is 1;
    }
    
    public static ExceptionType ToExceptionType(this short baseInt)
    {
        return baseInt is 0
            ? ExceptionType.Added
            : ExceptionType.Removed;
    }
    
    public static LocationType ToLocationType(this short baseInt)
    {
        return baseInt switch
        {
            0 => LocationType.Stop,
            1 => LocationType.Station,
            2 => LocationType.EntranceExit,
            3 => LocationType.GenericNode,
            4 => LocationType.BoardingArea,
            _ => LocationType.Stop
        };
    }
}