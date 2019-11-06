using NextDepartures.Standard.Models;
using System.Collections.Generic;
using System.Linq;

namespace NextDepartures.Standard.Utils
{
    public static class TimezoneUtils
    {
        public static string GetTimezoneFromEntities(List<Agency> workingAgencies, List<Stop> workingStops, Departure departure, string defaultTimezone = "Etc/UTC")
        {
            string timezone = "";

            if (string.IsNullOrEmpty(timezone))
            {
                timezone = workingStops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone;
            }

            if (string.IsNullOrEmpty(timezone))
            {
                timezone = workingAgencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone;
            }

            if (string.IsNullOrEmpty(timezone))
            {
                timezone = workingAgencies.FirstOrDefault()?.AgencyTimezone;
            }

            if (string.IsNullOrEmpty(timezone))
            {
                timezone = defaultTimezone;
            }

            return timezone;
        }
    }
}