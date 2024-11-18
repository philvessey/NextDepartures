using NodaTime;
using NodaTime.Extensions;
using System;

namespace NextDepartures.Standard.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToZonedDateTime(this DateTime dateTime, string timezone)
        {
            return dateTime.ToUniversalTime().ToInstant().InZone(DateTimeZoneProviders.Tzdb[timezone]).ToDateTimeUnspecified();
        }
    }
}