﻿using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;

using System;
using System.Collections.Generic;
using System.Linq;
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
            const int ToleranceInHours = 12;

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
                string timezone = GetTimezone(workingAgencies, workingStops, departure);

                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timezone)));
                int yesterdayDate = now.AddDays(-1).AsInteger();

                DateTime departureTime = GetDepartureTimeFromDeparture(now, departure.DepartureTime);

                int startDate = yesterdayDate;
                int endDate = yesterdayDate;

                if (departure.StartDate != "")
                {
                    startDate = int.Parse(departure.StartDate);
                }

                if (departure.EndDate != "")
                {
                    endDate = int.Parse(departure.EndDate);
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
                        {
                            workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                        }
                    }
                }
            }

            foreach (Departure departure in tempDepartures)
            {
                string timezone = GetTimezone(workingAgencies, workingStops, departure);

                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timezone)));
                int todayDate = now.AsInteger();

                DateTime departureTime = GetDepartureTimeFromDeparture(now, departure.DepartureTime);

                int startDate = todayDate;
                int endDate = todayDate;

                if (departure.StartDate != "")
                {
                    startDate = int.Parse(departure.StartDate);
                }

                if (departure.EndDate != "")
                {
                    endDate = int.Parse(departure.EndDate);
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
                        {
                            workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                        }
                    }
                }
            }

            foreach (Departure departure in tempDepartures)
            {
                string timezone = GetTimezone(workingAgencies, workingStops, departure);

                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timezone)));
                int tomorrowDate = now.AddDays(1).AsInteger();

                DateTime departureTime = GetDepartureTimeFromDeparture(now, departure.DepartureTime);

                int startDate = tomorrowDate;
                int endDate = tomorrowDate;

                if (departure.StartDate != "")
                {
                    startDate = int.Parse(departure.StartDate);
                }

                if (departure.EndDate != "")
                {
                    endDate = int.Parse(departure.EndDate);
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (!exclude && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
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

                        if (include && departureTime >= now && departureTime <= now.AddHours(ToleranceInHours))
                        {
                            workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                        }
                    }
                }
            }

            return workingDepartures.Select(d => CreateService(d, workingStops, workingAgencies)).ToList();
        }
    }
}