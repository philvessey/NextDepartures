using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions;

public static class IntExtensions
{
    public static ExceptionType ToExceptionType(this int baseInt)
    {
        return baseInt == 0 ? ExceptionType.Added : ExceptionType.Removed;
    }

    public static LocationType ToLocationType(this int baseInt)
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