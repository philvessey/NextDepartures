﻿using GTFS.Entities;
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
            IDataStorage storage = dataStorage;
            DataStorageProperties storageProperties = new(dataStorage);

            if (preload)
            {
                storage = await PreloadDataStorage.LoadAsync(storage, storageProperties);
            }

            return new Feed(storage, storageProperties);
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

            string agencyId = StringUtils.FindPossibleString(fallback,
                                    () => agencies.FirstOrDefault(a => a.Id == departure.AgencyId)?.Id,
                                    () => agencies.FirstOrDefault()?.Id
                                    ).Trim();

            string agencyName = StringUtils.FindPossibleString(fallback,
                                    () => agencies.FirstOrDefault(a => a.Id == departure.AgencyId)?.Name,
                                    () => agencies.FirstOrDefault()?.Name
                                    ).Trim().ToTitleCase();

            string destinationName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.Id.WithPrefix("_")) || departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
                () => departure.TripHeadsign,
                () => departure.TripShortName,
                () => departure.RouteLongName
                ).Trim().ToTitleCase();

            string routeName = StringUtils.FindPossibleString(fallback,
                () => stops.FirstOrDefault(s => departure.RouteShortName.Contains(s.Id.WithPrefix("_")) || departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
                () => departure.RouteShortName
                ).Trim().ToTitleCase();

            string stopName = StringUtils.FindPossibleString(fallback,
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
            return new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Value.Hours % 24, departureTime.Value.Minutes, departureTime.Value.Seconds).AddDays((departureTime.Value.Hours / 24) + dayOffset);
        }

        private static List<Departure> GetDeparturesOnDay(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, List<Departure> departures, DateTime now, DayOffsetType dayOffset, TimeSpan timeOffset, int toleranceInHours, string id)
        {
            List<Departure> resultForDay = new();

            foreach (Departure departure in departures)
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
            if (startDate <= targetDateTime.Date && endDate >= targetDateTime.Date)
            {
                bool include = false;

                if (dayOfWeekMapper(zonedDateTime.DayOfWeek, departure))
                {
                    include = !calendarDates.Any(c => c.ServiceId == departure.ServiceId && c.Date == targetDateTime.Date && c.ExceptionType == ExceptionType.Removed);
                }
                else
                {
                    include = calendarDates.Any(c => c.ServiceId == departure.ServiceId && c.Date == targetDateTime.Date && c.ExceptionType == ExceptionType.Added);
                }

                if (include && (departure.RouteShortName.Contains(id.WithPrefix("_"), StringComparison.CurrentCultureIgnoreCase) || departure.RouteShortName.Contains(id.WithPrefix("->"), StringComparison.CurrentCultureIgnoreCase)))
                {
                    return false;
                }

                if (include && timeOffset > TimeSpan.Zero && toleranceInHours > 0)
                {
                    if (departureDateTime >= zonedDateTime.Add(timeOffset) && departureDateTime <= zonedDateTime.AddHours(toleranceInHours))
                    {
                        return true;
                    }
                }
                else if (include && toleranceInHours > 0)
                {
                    if (departureDateTime >= zonedDateTime && departureDateTime <= zonedDateTime.AddHours(toleranceInHours))
                    {
                        return true;
                    }
                }
                else if (include && timeOffset > TimeSpan.Zero)
                {
                    if (departureDateTime >= zonedDateTime.Add(timeOffset))
                    {
                        return true;
                    }
                }
                else if (include)
                {
                    if (departureDateTime >= zonedDateTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Departure TryProcessDeparture(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, DateTime now, DayOffsetType dayOffset, TimeSpan timeOffset, int toleranceInHours, string id, Departure departure)
        {
            DateTime zonedDateTime = now.ToZonedDateTime(GetTimezone(agencies, stops, departure));
            DateTime departureDateTime = GetDateTimeFromDeparture(zonedDateTime, dayOffset.GetNumeric(), departure.DepartureTime);
            DateTime targetDateTime = zonedDateTime.AddDays(dayOffset.GetNumeric());

            if (IsDepartureValid(calendarDates, timeOffset, toleranceInHours, id, WeekdayUtils.GetUtilByDayType(dayOffset), departure, zonedDateTime, departureDateTime, targetDateTime, departure.StartDate, departure.EndDate))
            {
                return CreateProcessedDeparture(departure, departureDateTime);
            }
            else
            {
                return null;
            }
        }
    }
}