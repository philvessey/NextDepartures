using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions
{
    public static class IntExtensions
    {
        public static ExceptionType ToExceptionType(this int baseInt)
        {
            if (baseInt == 0)
            {
                return ExceptionType.Added;
            }
            else
            {
                return ExceptionType.Removed;
            }
        }

        public static LocationType? ToLocationType(this int baseInt)
        {
            if (baseInt == 0)
            {
                return LocationType.Stop;
            }
            else if (baseInt == 1)
            {
                return LocationType.Station;
            }
            else if (baseInt == 2)
            {
                return LocationType.EntranceExit;
            }
            else if (baseInt == 3)
            {
                return LocationType.GenericNode;
            }
            else if (baseInt == 4)
            {
                return LocationType.BoardingArea;
            }
            else
            {
                return null;
            }
        }
    }
}