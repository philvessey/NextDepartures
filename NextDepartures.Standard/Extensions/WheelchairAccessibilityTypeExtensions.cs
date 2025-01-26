using GTFS.Entities.Enumerations;

namespace NextDepartures.Standard.Extensions;

public static class WheelchairAccessibilityTypeExtensions
{
    public static int ToInt32(this WheelchairAccessibilityType wheelchairAccessibilityType)
    {
        return wheelchairAccessibilityType switch
        {
            WheelchairAccessibilityType.NoInformation => 0,
            WheelchairAccessibilityType.SomeAccessibility => 1,
            WheelchairAccessibilityType.NoAccessibility => 2,
            _ => 0
        };
    }
}