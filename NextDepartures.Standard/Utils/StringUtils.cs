using System;

namespace NextDepartures.Standard.Utils
{
    public static class StringUtils
    {
        public static string FindPossibleString(string fallback, params Func<string>[] steps)
        {
            if (steps == null || steps.Length == 0)
            {
                return fallback;
            }

            string returnValue = string.Empty;

            for (int i = 0; i < steps.Length; i++)
            {
                returnValue = steps[i]();

                if (!string.IsNullOrEmpty(returnValue))
                {
                    return returnValue;
                }
            }

            return fallback;
        }
    }
}