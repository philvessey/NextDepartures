using System.Globalization;

namespace NextDepartures.Standard.Extensions
{
    public static class StringExtensions
    {
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