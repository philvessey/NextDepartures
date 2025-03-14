using System;
using NodaTime;
using NodaTime.Extensions;

namespace NextDepartures.Standard.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToZonedDateTime(
        this DateTime baseDateTime,
        string timezone) {
        
        return baseDateTime
            .ToUniversalTime()
            .ToInstant()
            .InZone(zone: DateTimeZoneProviders.Tzdb[timezone])
            .ToDateTimeUnspecified();
    }
}