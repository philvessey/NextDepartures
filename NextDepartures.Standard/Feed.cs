using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
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
        private readonly DataStorageProperties _dataStorageProperties;

        private List<Agency> _agencies;
        private List<Models.Exception> _exceptions;
        private List<Stop> _stops;

        private Feed(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _dataStorageProperties = new DataStorageProperties(dataStorage);

            _agencies = new List<Agency>();
            _exceptions = new List<Models.Exception>();
            _stops = new List<Stop>();
        }

        /// <summary>
        /// Creates a new feed with the given data storage.
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
            if (_dataStorageProperties.DoesSupportParallelPreload)
            {
                var agenciesTask = _dataStorage.GetAgenciesAsync();
                var exceptionsTask = _dataStorage.GetExceptionsAsync();
                var stopsTask = _dataStorage.GetStopsAsync();

                _agencies = await agenciesTask;
                _exceptions = await exceptionsTask;
                _stops = await stopsTask;
            }
            else
            {
                _agencies = await _dataStorage.GetAgenciesAsync();
                _exceptions = await _dataStorage.GetExceptionsAsync();
                _stops = await _dataStorage.GetStopsAsync();
            }
        }

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

        private List<Departure> GetDeparturesOnDay(List<Departure> departures, DateTime now, int dayOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper)
        {
            List<Departure> resultForDay = new List<Departure>();

            // TODO: May be calculate the three days in one loop so that the timezone calculated and so on can be reused?
            foreach (Departure departure in departures)
            {
                resultForDay.AddIfNotNull(TryProcessDeparture(now, dayOffset, toleranceInHours, id, dayOfWeekMapper, departure));
            }

            return resultForDay;
        }

        private DateTime GetDepartureTimeFromDeparture(DateTime targetDateTime, int dayOffset, string departureTime)
        {
            int[] splittedDepartureTime = departureTime.Split(new string[] { ":" }, StringSplitOptions.None).Select(s => int.Parse(s)).ToArray();
            int departureHour = splittedDepartureTime[0];

            return new DateTime(targetDateTime.Year, targetDateTime.Month, targetDateTime.Day, departureHour % 24, splittedDepartureTime[1], splittedDepartureTime[2]).AddDays(((int) (departureHour / 24)) + dayOffset);
        }

        private string GetTimezone(Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => _stops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone,
                () => _agencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone,
                () => _agencies.FirstOrDefault()?.AgencyTimezone);
        }

        private bool IsDepartureValid(int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper, Departure departure, DateTime targetDateTime, int targetDate, DateTime departureTime, int startDate, int endDate)
        {
            if (startDate <= targetDate && endDate >= targetDate)
            {
                bool include;

                if (dayOfWeekMapper(targetDateTime.DayOfWeek, departure) == "1")
                {
                    include = !_exceptions.Any(e => departure.ServiceID == e.ServiceID && e.Date == targetDate.ToString() && e.ExceptionType == "2");
                }
                else
                {
                    include = _exceptions.Any(e => departure.ServiceID == e.ServiceID && e.Date == targetDate.ToString() && e.ExceptionType == "1");
                }

                if (departure.RouteShortName.ToLower().Contains(id.WithPrefix("_")) || departure.RouteShortName.ToLower().Contains(id.WithPrefix("->")))
                {
                    include = false;
                }

                if (include && departureTime >= targetDateTime && departureTime <= targetDateTime.AddHours(toleranceInHours))
                {
                    return true;
                }
            }

            return false;
        }

        private Departure TryProcessDeparture(DateTime now, int dayOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper, Departure departure)
        {
            DateTime targetDateTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(GetTimezone(departure))));
            DateTime departureTime = GetDepartureTimeFromDeparture(targetDateTime, dayOffset, departure.DepartureTime);
            
            int targetDate = targetDateTime.AddDays(dayOffset).AsInteger();
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

            if (IsDepartureValid(toleranceInHours, id, dayOfWeekMapper, departure, targetDateTime, targetDate, departureTime, startDate, endDate))
            {
                return CreateProcessedDeparture(departure, departureTime);
            }
            else
            {
                return null;
            }
        }
    }
}