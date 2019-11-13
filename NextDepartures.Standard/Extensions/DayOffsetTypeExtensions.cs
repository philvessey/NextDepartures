using NextDepartures.Standard.Models;

namespace NextDepartures.Standard.Extensions
{
    public static class DayOffsetTypeExtensions
    {
        public static int GetNumeric(this DayOffsetType dayOffsetType)
        {
            return (int) dayOffsetType;
        }
    }
}