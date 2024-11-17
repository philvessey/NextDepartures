using GTFS.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace NextDepartures.Standard.Extensions
{
    public static class StringExtensions
    {
        private static readonly string[] Separator = [":"];

        public static TimeOfDay? ToTimeOfDay(this string baseString)
        {
            var splitTime = baseString.Split(Separator, StringSplitOptions.None).Select(int.Parse).ToArray();

            return new TimeOfDay()
            {
                Hours = splitTime[0],
                Minutes = splitTime[1],
                Seconds = splitTime[2]
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
}