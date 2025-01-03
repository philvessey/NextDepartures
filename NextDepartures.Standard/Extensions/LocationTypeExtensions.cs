using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions;

public static class LocationTypeExtensions
{
    public static int ToInt32(this LocationType baseLocationType)
    {
        return baseLocationType switch
        {
            LocationType.Stop => 0,
            LocationType.Station => 1,
            LocationType.EntranceExit => 2,
            LocationType.GenericNode => 3,
            LocationType.BoardingArea => 4,
            _ => 0
        };
    }
}