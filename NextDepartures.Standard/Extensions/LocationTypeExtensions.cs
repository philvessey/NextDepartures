using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions
{
    public static class LocationTypeExtensions
    {
        public static int? ToInt32(this LocationType locationType)
        {
            if (locationType == LocationType.Stop)
            {
                return 0;
            }
            else if (locationType == LocationType.Station)
            {
                return 1;
            }
            else if (locationType == LocationType.EntranceExit)
            {
                return 2;
            }
            else if (locationType == LocationType.GenericNode)
            {
                return 3;
            }
            else if (locationType == LocationType.BoardingArea)
            {
                return 4;
            }
            else
            {
                return null;
            }
        }
    }
}