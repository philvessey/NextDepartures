using GTFS.Entities;
using GTFS.Entities.Enumerations;
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

        private Feed(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <summary>
        /// Creates a new feed with the given data storage.
        /// </summary>
        /// <param name="dataStorage">The data storage to use.</param>
        /// <param name="preload">Whether to preload the data.</param>
        /// <returns>A new feed instance.</returns>
        public static async Task<Feed> Load(IDataStorage dataStorage, bool preload = true)
        {
            DataStorageProperties storageProperties = new(dataStorage);
            
            var storage = dataStorage;

            if (preload)
            {
                storage = await PreloadDataStorage.LoadAsync(storage, storageProperties);
            }

            return new Feed(storage);
        }

        private static Departure CreateProcessedDeparture(Departure tempDeparture, DateTime departureDateTime)
        {
            return new Departure()
            {
                DepartureTime = new TimeOfDay() { Hours = departureDateTime.Hour, Minutes = departureDateTime.Minute, Seconds = departureDateTime.Second },
                DepartureDateTime = departureDateTime,
                StopId = tempDeparture.StopId,
                TripId = tempDeparture.TripId,
                ServiceId = tempDeparture.ServiceId,
                TripHeadsign = tempDeparture.TripHeadsign,
                TripShortName = tempDeparture.TripShortName,
                AgencyId = tempDeparture.AgencyId,
                RouteShortName = tempDeparture.RouteShortName,
                RouteLongName = tempDeparture.RouteLongName,
                Monday = tempDeparture.Monday,
                Tuesday = tempDeparture.Tuesday,
                Wednesday = tempDeparture.Wednesday,
                Thursday = tempDeparture.Thursday,
                Friday = tempDeparture.Friday,
                Saturday = tempDeparture.Saturday,
                Sunday = tempDeparture.Sunday,
                StartDate = tempDeparture.StartDate,
                EndDate = tempDeparture.EndDate
            };
        }

        private static Service CreateService(List<Agency> agencies, List<Stop> stops, Departure departure, string type)
        {
            const string fallback = "unknown";

            var agencyId = StringUtils.FindPossibleString(fallback,
                () => agencies.FirstOrDefault(a => a.Id == departure.AgencyId)?.Id,
                () => agencies.FirstOrDefault()?.Id
                ).Trim();

            var agencyName = StringUtils.FindPossibleString(fallback,
                () => agencies.FirstOrDefault(a => a.Id == departure.AgencyId)?.Name,
                () => agencies.FirstOrDefault()?.Name
                ).Trim().ToTitleCase();

            var destinationName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.Id.WithPrefix("_")) || departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            var routeName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.Id.WithPrefix("_")) || departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            var stopName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => s.Id == departure.StopId)?.Name
                ).Trim().ToTitleCase();

            return new Service()
            {
                AgencyId = agencyId,
                AgencyName = agencyName,
                DepartureDateTime = departure.DepartureDateTime,
                DepartureTime = departure.DepartureTime,
                DestinationName = destinationName,
                RouteName = routeName,
                StopId = departure.StopId,
                StopName = stopName,
                TripId = departure.TripId,
                Type = type
            };
        }

        private static DateTime GetDateTimeFromDeparture(DateTime zonedDateTime, int dayOffset, TimeOfDay? departureTime)
        {
            return departureTime != null ? new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Value.Hours % 24, departureTime.Value.Minutes, departureTime.Value.Seconds).AddDays((departureTime.Value.Hours / 24.0) + dayOffset) : DateTime.MinValue;
        }

        private static List<Departure> GetDeparturesOnDay(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, List<Departure> departures, DateTime now, DayOffsetType dayOffset, TimeSpan timeOffset, int toleranceInHours, string id)
        {
            List<Departure> resultForDay = [];

            foreach (var departure in departures)
            {
                resultForDay.AddIfNotNull(TryProcessDeparture(agencies, calendarDates, stops, now, dayOffset, timeOffset, toleranceInHours, id, departure));
            }

            return resultForDay;
        }

        private static string GetTimezone(List<Agency> agencies, List<Stop> stops, Departure departure, string defaultTimezone = "Etc/UTC")
        {
            return StringUtils.FindPossibleString(defaultTimezone,
                () => stops.FirstOrDefault(s => s.Id == departure.StopId)?.Timezone,
                () => agencies.FirstOrDefault(a => a.Id == departure.AgencyId)?.Timezone,
                () => agencies.FirstOrDefault()?.Timezone);
        }

        private static bool IsDepartureValid(List<CalendarDate> calendarDates, TimeSpan timeOffset, int toleranceInHours, string id, Func<DayOfWeek, Departure, bool> dayOfWeekMapper, Departure departure, DateTime zonedDateTime, DateTime departureDateTime, DateTime targetDateTime, DateTime startDate, DateTime endDate)
        {
            bool include;
            
            if (startDate > targetDateTime.Date || endDate < targetDateTime.Date) return false;

            if (dayOfWeekMapper(zonedDateTime.DayOfWeek, departure))
            {
                include = !calendarDates.Any(d => d.ServiceId == departure.ServiceId && d.Date == targetDateTime.Date && d.ExceptionType == ExceptionType.Removed);
            }
            else
            {
                include = calendarDates.Any(d => d.ServiceId == departure.ServiceId && d.Date == targetDateTime.Date && d.ExceptionType == ExceptionType.Added);
            }

            switch (include)
            {
                case true when (departure.RouteShortName.Contains(id.WithPrefix("_"), StringComparison.CurrentCultureIgnoreCase) || departure.RouteShortName.Contains(id.WithPrefix("->"), StringComparison.CurrentCultureIgnoreCase)):
                    return false;
                case true when timeOffset > TimeSpan.Zero && toleranceInHours > 0 && departureDateTime >= zonedDateTime.Add(timeOffset) && departureDateTime <= zonedDateTime.AddHours(toleranceInHours) && departureDateTime >= zonedDateTime.Add(timeOffset) && departureDateTime <= zonedDateTime.AddHours(toleranceInHours):
                case true when toleranceInHours > 0 && departureDateTime >= zonedDateTime && departureDateTime <= zonedDateTime.AddHours(toleranceInHours) && departureDateTime >= zonedDateTime && departureDateTime <= zonedDateTime.AddHours(toleranceInHours):
                case true when timeOffset > TimeSpan.Zero && departureDateTime >= zonedDateTime.Add(timeOffset) && departureDateTime >= zonedDateTime.Add(timeOffset):
                case true when departureDateTime >= zonedDateTime:
                    return true;
            }

            return false;
        }

        private static Departure TryProcessDeparture(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, DateTime now, DayOffsetType dayOffset, TimeSpan timeOffset, int toleranceInHours, string id, Departure departure)
        {
            return IsDepartureValid(calendarDates, timeOffset, toleranceInHours, id, WeekdayUtils.GetUtilByDayType(dayOffset), departure, now.ToZonedDateTime(GetTimezone(agencies, stops, departure)), GetDateTimeFromDeparture(now.ToZonedDateTime(GetTimezone(agencies, stops, departure)), dayOffset.GetNumeric(), departure.DepartureTime), now.ToZonedDateTime(GetTimezone(agencies, stops, departure)).AddDays(dayOffset.GetNumeric()), departure.StartDate, departure.EndDate) ? CreateProcessedDeparture(departure, GetDateTimeFromDeparture(now.ToZonedDateTime(GetTimezone(agencies, stops, departure)), dayOffset.GetNumeric(), departure.DepartureTime)) : null;
        }
    }
}