using NodaTime;
using NodaTime.Extensions;
using System;

namespace NextDepartures.Standard.Extensions
{
    public static class DateTimeExtensions
    {
        public static int AsInteger(this DateTime dateTime)
        {
            return int.Parse(string.Format("{0}{1}{2}", dateTime.Year, dateTime.Month.ToString("00"), dateTime.Day.ToString("00")));
        }

        public static DateTime AsZonedDateTime(this DateTime dateTime, string timezone)
        {
            Instant instant = dateTime.ToUniversalTime().ToInstant();
            ZonedDateTime zoned = instant.InZone(DateTimeZoneProviders.Tzdb[timezone]);

            return zoned.ToDateTimeUnspecified();
        }
    }
}