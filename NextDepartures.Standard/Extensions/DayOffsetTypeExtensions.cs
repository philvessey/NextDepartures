using NextDepartures.Standard.Types;

namespace NextDepartures.Standard.Extensions;

public static class DayOffsetTypeExtensions
{
    public static int GetNumeric(this DayOffsetType baseDayOffsetType)
    {
        return (int)baseDayOffsetType;
    }
}