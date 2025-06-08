using System;

namespace NextDepartures.Standard.Utils;

public static class StringUtils
{
    public static string GetPossibleString(
        string fallback,
        params Func<string>[] steps) {
        
        if (steps is null || steps.Length is 0)
            return fallback;
        
        foreach (var s in steps)
        {
            var value = s();
            
            if (!string.IsNullOrEmpty(value: value))
                return value;
        }
        
        return fallback;
    }
}