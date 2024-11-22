using GTFS.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace NextDepartures.Standard.Extensions;

public static class StringExtensions
{
    private static readonly string[] Separator = [":"];

    public static TimeOfDay? ToTimeOfDay(this string baseString)
    {
        var value = baseString.Split(Separator, StringSplitOptions.None).Select(int.Parse).ToArray();

        return new TimeOfDay
        {
            Hours = value[0],
            Minutes = value[1],
            Seconds = value[2]
        };
    }

    public static string ToTitleCase(this string baseString)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(baseString.ToLower());
    }

    public static string WithPrefix(this string baseString, string prefix)
    {
        return $"{prefix}{baseString}";
    }
}