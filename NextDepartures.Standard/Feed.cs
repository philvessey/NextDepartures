using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        private readonly IDataStorage _dataStorage;
        private readonly DataStorageProperties _dataStorageProperties;

        private Feed(IDataStorage dataStorage, DataStorageProperties dataStorageProperties)
        {
            _dataStorage = dataStorage;
            _dataStorageProperties = dataStorageProperties;
        }

        /// <summary>
        /// Creates a new feed with the given data storage.
        /// </summary>
        /// <param name="dataStorage">The data storage to use.</param>
        /// <param name="preload">Whether to preload the data.</param>
        /// <returns>A new feed instance.</returns>
        public static async Task<Feed> Load(IDataStorage dataStorage, bool preload = true)
        {
            DataStorageProperties storageProperties = new DataStorageProperties(dataStorage);
            IDataStorage storage = dataStorage;

            if (preload)
            {
                storage = await PreloadDataStorage.LoadAsync(storage, storageProperties);
            }

            // TODO: Jump in here with custom queries via LINQ or similar (when the data storage properties tell us the data storage does not support sorting or similar)

            return new Feed(storage, storageProperties);
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

        private Service CreateService(List<Agency> agencies, List<Stop> stops, Departure departure)
        {
            const string fallback = "Unknown";

            string agencyName = StringUtils.FindPossibleString(fallback,
                                    () => agencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyName,
                                    () => agencies.FirstOrDefault()?.AgencyName
                                    ).Trim().ToTitleCase();

            string destinationName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            string routeName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.StopID.WithPrefix("_")) || departure.RouteShortName.Contains(s.StopID.WithPrefix("->")))?.StopName,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            string stopName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopName
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

        private List<Departure> GetDeparturesOnDay(List<Agency> agencies, List<Stop> stops, List<Models.Exception> exceptions, List<Departure> departures, DateTime now, DayOffsetType dayOffset, int toleranceInHours, string id)
        {
            List<Departure> resultForDay = new List<Departure>();

            // TODO: May be calculate the three days in one loop so that the timezone calculated and so on can be reused?
            foreach (Departure departure in departures)
            {
                resultForDay.AddIfNotNull(TryProcessDeparture(agencies, stops, exceptions, now, dayOffset, toleranceInHours, id, departure));
            }

            return resultForDay;
        }

        private DateTime GetDepartureTimeFromDeparture(DateTime targetDateTime, int dayOffset, string departureTime)
        {
            int[] splittedDepartureTime = departureTime.Split(new string[] { ":" }, StringSplitOptions.None).Select(s => int.Parse(s)).ToArray();
            int departureHour = splittedDepartureTime[0];

            return new DateTime(targetDateTime.Year, targetDateTime.Month, targetDateTime.Day, departureHour % 24, splittedDepartureTime[1], splittedDepartureTime[2]).AddDays(((int) (departureHour / 24)) + dayOffset);
        }

        private string GetTimezone(List<Agency> agencies, List<Stop> stops, Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => stops.FirstOrDefault(s => s.StopID == departure.StopID)?.StopTimezone,
                () => agencies.FirstOrDefault(a => a.AgencyID == departure.AgencyID)?.AgencyTimezone,
                () => agencies.FirstOrDefault()?.AgencyTimezone);
        }

        private bool IsDepartureValid(List<Models.Exception> exceptions, int toleranceInHours, string id, Func<DayOfWeek, Departure, string> dayOfWeekMapper, Departure departure, DateTime targetDateTime, int targetDate, DateTime departureTime, int startDate, int endDate)
        {
            if (startDate <= targetDate && endDate >= targetDate)
            {
                bool include;

                if (dayOfWeekMapper(targetDateTime.DayOfWeek, departure) == "1")
                {
                    include = !exceptions.Any(e => departure.ServiceID == e.ServiceID && e.Date == targetDate.ToString() && e.ExceptionType == "2");
                }
                else
                {
                    include = exceptions.Any(e => departure.ServiceID == e.ServiceID && e.Date == targetDate.ToString() && e.ExceptionType == "1");
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

        private Departure TryProcessDeparture(List<Agency> agencies, List<Stop> stops, List<Models.Exception> exceptions, DateTime now, DayOffsetType dayOffset, int toleranceInHours, string id, Departure departure)
        {
            DateTime targetDateTime = now.AsZonedDateTime(GetTimezone(agencies, stops, departure));
            DateTime departureTime = GetDepartureTimeFromDeparture(targetDateTime, dayOffset.GetNumeric(), departure.DepartureTime);

            int targetDate = targetDateTime.AddDays(dayOffset.GetNumeric()).AsInteger();
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

            if (IsDepartureValid(exceptions, toleranceInHours, id, WeekdayUtils.GetUtilByDayType(dayOffset), departure, targetDateTime, targetDate, departureTime, startDate, endDate))
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