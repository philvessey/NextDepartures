namespace NextDepartures.Storage.GTFS.Extensions;

public static class StringExtensions
{
    public static string TrimDoubleQuotes(this string baseString)
    {
        return baseString.Trim(trimChar: '"');
    }
}