using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Interfaces;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TimeZoneConverter;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        private readonly IDataStorage _dataStorage;

        private List<Agency> _agencies;
        private List<Models.Exception> _exceptions;
        private List<Stop> _stops;

        private Feed()
        {
        }

        private Feed(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _agencies = new List<Agency>();
            _exceptions = new List<Models.Exception>();
            _stops = new List<Stop>();
        }

        /// <summary>
        /// Creates a new feed.
        /// </summary>
        /// <param name="dataStorage">The data storage to use.</param>
        /// <returns>A new feed instance.</returns>
        public static async Task<Feed> Load(IDataStorage dataStorage)
        {
            Feed feed = new Feed(dataStorage);
            await feed.PreloadFromStorage();
            return feed;
        }

        private async Task PreloadFromStorage()
        {
            // TODO: May be parallelize this if the storage is able to do so
            _agencies = await _dataStorage.GetAgenciesAsync();
            _exceptions = new List<Models.Exception>();
            _stops = new List<Stop>();
        }

        /// <summary>
        /// Creates a processed departure.
        /// </summary>
        /// <param name="tempDeparture">The temporary departure.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <remarks>A processed departure is a departure actually running on the date / time.</remarks>
        /// <returns>A departure.</returns>
        private Departure CreateProcessedDeparture(Departure tempDeparture, DateTime departureTime)
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

        private Service CreateService(Departure departure)
        {
            const string fallback = "Unknown";

            string agencyName = StringUtils.FindPossibleString(fallback,
                                    () => _agencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyName,
                                    () => _agencies.FirstOrDefault()?.AgencyName
                                    ).Trim().ToTitleCase();

            string destinationName = StringUtils.FindPossibleString(fallback,
                () => _stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            string routeName = StringUtils.FindPossibleString(fallback,
                () => _stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            string stopName = StringUtils.FindPossibleString(fallback,
                () => _stops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopName
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

        private string GetTimezone(Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => _stops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone,
                () => _agencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone,
                () => _agencies.FirstOrDefault()?.AgencyTimezone);
        }

        private List<Departure> GetDeparturesOnDay(List<Departure> departures, int dayOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper)
        {
            List<Departure> resultForDay = new List<Departure>();

            // TODO: May be calculate the three days in one loop so that the timezone calculated and so on can be reused?
            foreach (Departure departure in departures)
            {
                resultForDay.AddIfNotNull(TryProcessDeparture(dayOffset, toleranceInHours, id, dayOfWeekMapper, departure));
            }

            return resultForDay;
        }

        private Departure TryProcessDeparture(int dayOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper, Departure departure)
        {
            string timezone = GetTimezone(departure);

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

            if (IsDepartureValid(toleranceInHours, id, dayOfWeekMapper, departure, now, targetDate, departureTime, startDate, endDate))
            {
                return CreateProcessedDeparture(departure, departureTime);
            }
            else
            {
                return null;
            }
        }

        private bool IsDepartureValid(int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper, Departure departure, DateTime now, int targetDate, DateTime departureTime, int startDate, int endDate)
        {
            // TODO: Refactor the if inner code
            if (dayOfWeekMapper(now.DayOfWeek, departure) == "1" && startDate <= targetDate && endDate >= targetDate)
            {
                bool exclude = false;

                foreach (var exception in _exceptions)
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
                    return true;
                }
            }
            else if (startDate <= targetDate && endDate >= targetDate)
            {
                bool include = false;

                foreach (var exception in _exceptions)
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
                    return true;
                }
            }

            return false;
        }
    }
}