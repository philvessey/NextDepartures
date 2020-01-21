using GTFS.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace NextDepartures.Standard.Extensions
{
    public static class StringExtensions
    {
        public static TimeOfDay? ToTimeOfDay(this string baseString)
        {
            int[] splittedString = baseString.Split(new string[] { ":" }, StringSplitOptions.None).Select(s => int.Parse(s)).ToArray();

            return new TimeOfDay()
            {
                Hours = splittedString[0],
                Minutes = splittedString[1],
                Seconds = splittedString[2]
            };
        }

        public static string ToTitleCase(this string baseString)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(baseString.ToLower());
        }

        public static string WithPrefix(this string baseString, string prefix)
        {
            return string.Format("{0}{1}", prefix, baseString);
        }
    }
}