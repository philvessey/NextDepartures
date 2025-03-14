using System;

namespace NextDepartures.Standard.Utils;

public static class StringUtils
{
    public static string GetPossibleString(
        string fallback,
        params Func<string>[] steps) {
        
        if (steps == null || steps.Length == 0)
            return fallback;
        
        foreach (var t in steps)
        {
            var value = t();
            
            if (!string.IsNullOrEmpty(value: value))
                return value;
        }
        
        return fallback;
    }
}