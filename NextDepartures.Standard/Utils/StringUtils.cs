using System;

namespace NextDepartures.Standard.Utils;

public static class StringUtils
{
    public static string FindPossibleString(string fallback, params Func<string>[] steps)
    {
        if (steps == null || steps.Length == 0)
        {
            return fallback;
        }

        foreach (var t in steps)
        {
            var returnValue = t();

            if (!string.IsNullOrEmpty(returnValue))
            {
                return returnValue;
            }
        }

        return fallback;
    }
}