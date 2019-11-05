using NextDepartures.Standard.Models;
using System.Collections.Generic;

namespace NextDepartures.Standard.Utils
{
    public static class TimezoneUtils
    {
        public static string GetTimezoneFromEntities(List<Agency> workingAgencies, List<Stop> workingStops, Departure departure)
        {
            string timezone = "";

            if (timezone == "")
            {
                foreach (Stop stop in workingStops)
                {
                    if (stop.StopID == departure.StopID)
                    {
                        timezone = stop.StopTimezone;

                        break;
                    }
                }
            }

            if (timezone == "")
            {
                foreach (Agency agency in workingAgencies)
                {
                    if (agency.AgencyID == departure.AgencyID)
                    {
                        timezone = agency.AgencyTimezone;

                        break;
                    }
                }
            }

            if (timezone == "")
            {
                foreach (Agency agency in workingAgencies)
                {
                    timezone = agency.AgencyTimezone;

                    break;
                }
            }

            if (timezone == "")
            {
                timezone = "Etc/UTC";
            }

            return timezone;
        }
    }
}