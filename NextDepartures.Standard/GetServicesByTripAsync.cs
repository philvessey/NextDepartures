using NextDepartures.Standard.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>
        /// Gets the services for a specific trip.
        /// </summary>
        /// <param name="id">The id of the trip.</param>
        /// <returns>A list of services.</returns>
        public async Task<List<Service>> GetServicesByTripAsync(string id)
        {
            List<Service> results = new List<Service>();

            try
            {
                DateTime now = DateTime.UtcNow;
                int yesterdayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(-1).Year, now.AddDays(-1).Month.ToString("00"), now.AddDays(-1).Day.ToString("00")));
                int todayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.Year, now.Month.ToString("00"), now.Day.ToString("00")));
                int tomorrowDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(1).Year, now.AddDays(1).Month.ToString("00"), now.AddDays(1).Day.ToString("00")));

                List<Departure> tempDepartures = await _dataStorage.GetDeparturesForTripAsync(id);
                List<Agency> workingAgencies = await _dataStorage.GetAgenciesAsync();
                List<Models.Exception> workingExceptions = await _dataStorage.GetExceptionsAsync();
                List<Stop> workingStops = await _dataStorage.GetStopsAsync();

                /// Process data
                /// 1. Timezone is determined - stop is checked first, if no timezone available, look at agency, if still no timezone assume Etc/UTC.
                /// 2. Yesterday date is checked first incase its midnight and services from previous day are running date, then checks today date, finally tomorrow date.
                /// 3. For each departure the departureTime is converted to DateTime. GTFS can store hours as more than 24 so that is handled.
                /// 4. Looks for which day of the week we are on.
                /// 5. Checks to see if a service is excluded from running - if it is its ignored.
                /// 6. Checks to see if the stop is the trip destination - if it is its ignored as this is not a departure.
                /// 7. Working Departure is created if departureTime > now and < 12 hours from now.
                /// 8. If cant determine what day of week service runs on exceptions are checked to see if service running  - if it is its included.
                /// 9. Checks to see if the stop is the trip destination - if it is its ignored as this is not a departure.

                List<Departure> workingDepartures = new List<Departure>();

                foreach (Departure departure in tempDepartures)
                {
                    string timeZone = "";

                    if (timeZone == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (stop.StopID == departure.StopID)
                            {
                                timeZone = stop.StopTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            if (agency.AgencyID == departure.AgencyID)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            timeZone = agency.AgencyTimezone;

                            break;
                        }
                    }

                    if (timeZone == "")
                    {
                        timeZone = "Etc/UTC";
                    }

                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                    yesterdayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(-1).Year, now.AddDays(-1).Month.ToString("00"), now.AddDays(-1).Day.ToString("00")));

                    DateTime departureTime = new DateTime();

                    if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2]));
                    }
                    else
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(-1);
                    }

                    int startDate = yesterdayDate;
                    int endDate = yesterdayDate;

                    if (departure.StartDate != "")
                    {
                        startDate = Convert.ToInt32(departure.StartDate);
                    }

                    if (departure.EndDate != "")
                    {
                        endDate = Convert.ToInt32(departure.EndDate);
                    }

                    if (now.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (departure.Sunday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        if (departure.Monday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        if (departure.Tuesday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Thursday)
                    {
                        if (departure.Wednesday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Friday)
                    {
                        if (departure.Thursday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (departure.Friday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (departure.Saturday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                }

                foreach (Departure departure in tempDepartures)
                {
                    string timeZone = "";

                    if (timeZone == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (stop.StopID == departure.StopID)
                            {
                                timeZone = stop.StopTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            if (agency.AgencyID == departure.AgencyID)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            timeZone = agency.AgencyTimezone;

                            break;
                        }
                    }

                    if (timeZone == "")
                    {
                        timeZone = "Etc/UTC";
                    }

                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                    todayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.Year, now.Month.ToString("00"), now.Day.ToString("00")));

                    DateTime departureTime = new DateTime();

                    if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(3);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                    }
                    else
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2]));
                    }

                    int startDate = todayDate;
                    int endDate = todayDate;

                    if (departure.StartDate != "")
                    {
                        startDate = Convert.ToInt32(departure.StartDate);
                    }

                    if (departure.EndDate != "")
                    {
                        endDate = Convert.ToInt32(departure.EndDate);
                    }

                    if (now.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (departure.Monday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        if (departure.Tuesday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        if (departure.Wednesday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Thursday)
                    {
                        if (departure.Thursday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Friday)
                    {
                        if (departure.Friday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (departure.Saturday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (departure.Sunday == "1" && startDate <= todayDate && endDate >= todayDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= todayDate && endDate >= todayDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                }

                foreach (Departure departure in tempDepartures)
                {
                    string timeZone = "";

                    if (timeZone == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (stop.StopID == departure.StopID)
                            {
                                timeZone = stop.StopTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            if (agency.AgencyID == departure.AgencyID)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }
                    }

                    if (timeZone == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            timeZone = agency.AgencyTimezone;

                            break;
                        }
                    }

                    if (timeZone == "")
                    {
                        timeZone = "Etc/UTC";
                    }

                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                    tomorrowDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(1).Year, now.AddDays(1).Month.ToString("00"), now.AddDays(1).Day.ToString("00")));

                    DateTime departureTime = new DateTime();

                    if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(4);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(3);
                    }
                    else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                    }
                    else
                    {
                        departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                    }

                    int startDate = tomorrowDate;
                    int endDate = tomorrowDate;

                    if (departure.StartDate != "")
                    {
                        startDate = Convert.ToInt32(departure.StartDate);
                    }

                    if (departure.EndDate != "")
                    {
                        endDate = Convert.ToInt32(departure.EndDate);
                    }

                    if (now.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (departure.Tuesday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        if (departure.Wednesday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        if (departure.Thursday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Thursday)
                    {
                        if (departure.Friday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Friday)
                    {
                        if (departure.Saturday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (departure.Sunday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (departure.Monday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool exclude = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                {
                                    exclude = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                exclude = true;
                            }

                            if (!exclude && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                        else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                        {
                            bool include = false;

                            foreach (var exception in workingExceptions)
                            {
                                if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                {
                                    include = true;

                                    break;
                                }
                            }

                            if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                            {
                                include = false;
                            }

                            if (include && departureTime >= now && departureTime <= now.AddHours(12))
                            {
                                workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                            }
                        }
                    }
                }

                foreach (Departure departure in workingDepartures)
                {
                    string agencyName = "";

                    if (agencyName == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            if (agency.AgencyID == departure.AgencyID)
                            {
                                agencyName = agency.AgencyName;

                                break;
                            }
                        }
                    }

                    if (agencyName == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            agencyName = agency.AgencyName;

                            break;
                        }
                    }

                    if (agencyName == "")
                    {
                        agencyName = "Unknown";
                    }

                    string destinationName = "";

                    if (destinationName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (departure.RouteShortName.Contains(string.Format("_{0}", stop.StopID)) || departure.RouteShortName.Contains(string.Format("->{0}", stop.StopID)))
                            {
                                destinationName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.TripHeadsign;
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.TripShortName;
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.RouteLongName;
                    }

                    if (destinationName == "")
                    {
                        destinationName = "Unknown";
                    }

                    string routeName = "";

                    if (routeName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (departure.RouteShortName.Contains(string.Format("_{0}", stop.StopID)) || departure.RouteShortName.Contains(string.Format("->{0}", stop.StopID)))
                            {
                                routeName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (routeName == "")
                    {
                        routeName = departure.RouteShortName;
                    }

                    if (routeName == "")
                    {
                        routeName = "Unknown";
                    }

                    string stopName = "";

                    if (stopName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (stop.StopID == departure.StopID)
                            {
                                stopName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (stopName == "")
                    {
                        stopName = "Unknown";
                    }

                    agencyName = agencyName.Trim();
                    destinationName = destinationName.Trim();
                    routeName = routeName.Trim();
                    stopName = stopName.Trim();

                    results.Add(new Service()
                    {
                        AgencyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(agencyName.ToLower()),
                        DepartureTime = departure.DepartureTime,
                        DestinationName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(destinationName.ToLower()),
                        RouteName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(routeName.ToLower()),
                        StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stopName.ToLower()),
                        TripID = departure.TripID
                    });
                }

                return results;
            }
            catch
            {
                return null;
            }
        }
    }
}