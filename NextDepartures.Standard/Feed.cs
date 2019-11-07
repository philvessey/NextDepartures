using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZoneConverter;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        private readonly IDataStorage _dataStorage;

        /// <summary>
        /// Creates a new feed.
        /// </summary>
        /// <param name="dataStorage">The data storage to use.</param>
        public Feed(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <summary>
        /// Creates a working departure.
        /// </summary>
        /// <param name="tempDeparture">The temporary departure.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <remarks>A working departure is a departure actually running on the date / time.</remarks>
        /// <returns>A departure.</returns>
        private Departure CreateWorkingDeparture(Departure tempDeparture, DateTime departureTime)
        {
            return new Departure()
            {
                AgencyID = tempDeparture.AgencyID,
                DepartureTime = departureTime.ToString(),
                EndDate = tempDeparture.EndDate,
                Friday = tempDeparture.Friday,
                Monday = tempDeparture.Monday,
                RouteLongName = tempDeparture.RouteLongName,
                RouteShortName = tempDeparture.RouteShortName,
                Saturday = tempDeparture.Saturday,
                ServiceID = tempDeparture.ServiceID,
                StartDate = tempDeparture.StartDate,
                StopID = tempDeparture.StopID,
                Sunday = tempDeparture.Sunday,
                Thursday = tempDeparture.Thursday,
                TripHeadsign = tempDeparture.TripHeadsign,
                TripID = tempDeparture.TripID,
                TripShortName = tempDeparture.TripShortName,
                Tuesday = tempDeparture.Tuesday,
                Wednesday = tempDeparture.Wednesday
            };
        }

        private Service CreateService(Departure departure, List<Stop> workingStops, List<Agency> workingAgencies)
        {
            const string fallback = "Unknown";

            string agencyName = StringUtils.FindPossibleString(fallback,
                                    () => workingAgencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyName,
                                    () => workingAgencies.FirstOrDefault()?.AgencyName
                                    ).Trim().ToTitleCase();

            string destinationName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            string routeName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            string stopName = StringUtils.FindPossibleString(fallback,
                () => workingStops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopName
                ).Trim().ToTitleCase();

            return new Service()
            {
                AgencyName = agencyName,
                DepartureTime = departure.DepartureTime,
                DestinationName = destinationName,
                RouteName = routeName,
                StopName = stopName,
                TripID = departure.TripID
            };
        }

        private DateTime GetDepartureTimeFromDeparture(DateTime now, string departureTime)
        {
            var splittedDepartureTime = departureTime.Split(new string[] { ":" }, StringSplitOptions.None).Select(s => int.Parse(s)).ToArray();
            var departureHour = splittedDepartureTime[0];

            // When hour >= 72 then 2 days should be added
            // When hour >= 48 then 1 day should be added
            // When hour >= 24 then no days should be added
            // When hour < 24 then -1 days should be added
            return new DateTime(now.Year, now.Month, now.Day, departureHour % 24, splittedDepartureTime[1], splittedDepartureTime[2]).AddDays(((int) (departureHour / 24)) - 1);
        }

        private string GetTimezone(List<Agency> workingAgencies, List<Stop> workingStops, Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => workingStops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone,
                () => workingAgencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone,
                () => workingAgencies.FirstOrDefault()?.AgencyTimezone);
        }

        private List<Departure> GetDeparturesOnDay(List<Agency> agencies, List<Stop> stops, List<Models.Exception> exceptions, List<Departure> departures, int dayOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper)
        {
            List<Departure> resultForDay = new List<Departure>();

            foreach (Departure departure in departures)
            {
                string timezone = GetTimezone(agencies, stops, departure);

                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timezone)));
                int targetDate = now.AddDays(dayOffset).AsInteger();

                DateTime departureTime = GetDepartureTimeFromDeparture(now, departure.DepartureTime);

                int startDate = targetDate;
                int endDate = targetDate;

                if (departure.StartDate != "")
                {
                    startDate = int.Parse(departure.StartDate);
                }

                if (departure.EndDate != "")
                {
                    endDate = int.Parse(departure.EndDate);
                }

                if (dayOfWeekMapper(now.DayOfWeek, departure) == "1" && startDate <= targetDate && endDate >= targetDate)
                {
                    bool exclude = false;

                    foreach (var exception in exceptions)
                    {
                        if (departure.ServiceID == exception.ServiceID && exception.Date == targetDate.ToString() && exception.ExceptionType == "2")
                        {
                            exclude = true;

                            break;
                        }
                    }

                    if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                    {
                        exclude = true;
                    }

                    if (!exclude && departureTime >= now && departureTime <= now.AddHours(toleranceInHours))
                    {
                        resultForDay.Add(CreateWorkingDeparture(departure, departureTime));
                    }
                }
                else if (startDate <= targetDate && endDate >= targetDate)
                {
                    bool include = false;

                    foreach (var exception in exceptions)
                    {
                        if (departure.ServiceID == exception.ServiceID && exception.Date == targetDate.ToString() && exception.ExceptionType == "1")
                        {
                            include = true;

                            break;
                        }
                    }

                    if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                    {
                        include = false;
                    }

                    if (include && departureTime >= now && departureTime <= now.AddHours(toleranceInHours))
                    {
                        resultForDay.Add(CreateWorkingDeparture(departure, departureTime));
                    }
                }
            }

            return resultForDay;
        }
    }
}