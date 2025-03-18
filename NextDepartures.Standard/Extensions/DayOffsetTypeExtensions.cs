using NextDepartures.Standard.Types;

namespace NextDepartures.Standard.Extensions;

public static class DayOffsetTypeExtensions
{
    public static int ToInt32(this DayOffsetType baseDayOffsetType)
    {
        return baseDayOffsetType switch
        {
            DayOffsetType.Yesterday => -1,
            DayOffsetType.Today => 0,
            DayOffsetType.Tomorrow => 1,
            _ => 0
        };
    }
}