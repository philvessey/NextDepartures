using System;
using GTFS.Entities;

namespace NextDepartures.Standard.Utils;

public static class DateTimeUtils
{
    public static DateTime GetFromDeparture(
        DateTime zonedDateTime,
        int dayOffset,
        TimeOfDay departureTime) {
        
        switch (departureTime.Hours)
        {
            case >= 144:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 144,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 6);
            }
            case >= 120:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 120,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 5);
            }
            case >= 96:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 96,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 4);
            }
            case >= 72:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 72,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 3);
            }
            case >= 48:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 48,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 2);
            }
            case >= 24:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours - 24,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset + 1);
            }
            default:
            {
                var value = new DateTime(
                    year: zonedDateTime.Year,
                    month: zonedDateTime.Month,
                    day: zonedDateTime.Day,
                    hour: departureTime.Hours,
                    minute: departureTime.Minutes,
                    second: departureTime.Seconds);
                
                return value.AddDays(value: dayOffset);
            }
        }
    }
}