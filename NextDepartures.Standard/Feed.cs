﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using NextDepartures.Standard.Extensions;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Storage;
using NextDepartures.Standard.Types;
using NextDepartures.Standard.Utils;

namespace NextDepartures.Standard;

public partial class Feed
{
    private readonly IDataStorage _dataStorage;

    private Feed(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    /// <summary>
    /// Loads a new feed
    /// </summary>
    /// <param name="dataStorage">The data storage to load into the feed.</param>
    /// <param name="preload">Whether to preload the data. Default is true.</param>
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
        return new Departure
        {
            DepartureTime = new TimeOfDay
            {
                Hours = departureDateTime.Hour,
                Minutes = departureDateTime.Minute,
                Seconds = departureDateTime.Second
            },
            
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
            () => agencies.FirstOrDefault(a =>
                a.Id == departure.AgencyId)?.Id,
            () => agencies.FirstOrDefault()?.Id).Trim();

        var agencyName = StringUtils.FindPossibleString(fallback,
            () => agencies.FirstOrDefault(a =>
                a.Id == departure.AgencyId)?.Name,
            () => agencies.FirstOrDefault()?.Name).Trim().ToTitleCase();

        var destinationName = StringUtils.FindPossibleString(fallback,
            () => stops.FirstOrDefault(s =>
                departure.RouteShortName.Contains(s.Id.WithPrefix("_")) ||
                departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
            () => departure.TripHeadsign, () => departure.TripShortName, () => departure.RouteLongName).Trim().ToTitleCase();

        var routeName = StringUtils.FindPossibleString(fallback,
            () => stops.FirstOrDefault(s =>
                departure.RouteShortName.Contains(s.Id.WithPrefix("_")) ||
                departure.RouteShortName.Contains(s.Id.WithPrefix("->")))?.Name,
            () => departure.RouteShortName).Trim().ToTitleCase();

        var stopName = StringUtils.FindPossibleString(fallback,
            () => stops.FirstOrDefault(s =>
                s.Id == departure.StopId)?.Name).Trim().ToTitleCase();

        return new Service
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

    private static DateTime GetDateTimeFromDeparture(DateTime zonedDateTime, int dayOffset, TimeOfDay departureTime)
    {
        return departureTime.Hours switch
        {
            >= 144 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 144,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 6),
            >= 120 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 120,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 5),
            >= 96 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 96,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 4),
            >= 72 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 72,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 3),
            >= 48 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 48,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 2),
            >= 24 => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours - 24,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset + 1),
            _ => new DateTime(zonedDateTime.Year, zonedDateTime.Month, zonedDateTime.Day, departureTime.Hours,
                departureTime.Minutes, departureTime.Seconds).AddDays(dayOffset)
        };
    }

    private static List<Departure> GetDeparturesOnDay(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, List<Departure> departures, DateTime target, DayOffsetType dayOffset, TimeSpan timeOffset, TimeSpan tolerance, string id)
    {
        List<Departure> results = [];

        foreach (var departure in departures)
        {
            results.AddIfNotNull(TryProcessDeparture(agencies, calendarDates, stops, target, dayOffset, timeOffset, tolerance, id, departure));
        }

        return results;
    }

    private static string GetTimezone(List<Agency> agencies, List<Stop> stops, Departure departure, string defaultTimezone = "Etc/UTC")
    {
        return StringUtils.FindPossibleString(defaultTimezone,
            () => stops.FirstOrDefault(s =>
                s.Id == departure.StopId)?.Timezone,
            () => agencies.FirstOrDefault(a =>
                a.Id == departure.AgencyId)?.Timezone,
            () => agencies.FirstOrDefault()?.Timezone);
    }

    private static bool IsDepartureValid(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, DateTime target, TimeSpan timeOffset, TimeSpan tolerance, string id, Departure departure, DateTime targetDateTime, DateTime departureDateTime, Func<DayOfWeek, Departure, bool> dayOfWeekMapper)
    {
        var zonedDateTime = target.ToZonedDateTime(GetTimezone(agencies, stops, departure));
        
        if (departure.StartDate > targetDateTime.Date) return false;
        if (departure.EndDate < targetDateTime.Date) return false;
        
        if (dayOfWeekMapper(zonedDateTime.DayOfWeek, departure))
        {
            var include = !calendarDates.Any(d =>
                d.ServiceId == departure.ServiceId &&
                d.Date == targetDateTime.Date &&
                d.ExceptionType == ExceptionType.Removed);
            
            if (!include) return false;
        }
        else
        {
            var include = calendarDates.Any(d =>
                d.ServiceId == departure.ServiceId &&
                d.Date == targetDateTime.Date &&
                d.ExceptionType == ExceptionType.Added);
            
            if (!include) return false;
        }
        
        if (departure.RouteShortName.Contains(id.WithPrefix("_"), StringComparison.CurrentCultureIgnoreCase)) return false;
        if (departure.RouteShortName.Contains(id.WithPrefix("->"), StringComparison.CurrentCultureIgnoreCase)) return false;

        if (timeOffset > TimeSpan.Zero && tolerance > TimeSpan.Zero)
        {
            if (departureDateTime >= zonedDateTime.Add(timeOffset) && departureDateTime <= zonedDateTime.Add(timeOffset).Add(tolerance))
            {
                return true;
            }
        }
        else if (tolerance > TimeSpan.Zero)
        {
            if (departureDateTime >= zonedDateTime && departureDateTime <= zonedDateTime.Add(tolerance))
            {
                return true;
            }
        }
        else if (timeOffset > TimeSpan.Zero)
        {
            if (departureDateTime >= zonedDateTime.Add(timeOffset))
            {
                return true;
            }
        }
        else
        {
            if (departureDateTime >= zonedDateTime)
            {
                return true;
            }
        }
        
        return false;
    }

    private static Departure TryProcessDeparture(List<Agency> agencies, List<CalendarDate> calendarDates, List<Stop> stops, DateTime target, DayOffsetType dayOffset, TimeSpan timeOffset, TimeSpan tolerance, string id, Departure departure)
    {
        var targetDateTime = target.ToZonedDateTime(GetTimezone(agencies, stops, departure))
            .AddDays(dayOffset.GetNumeric());
        
        var departureDateTime = GetDateTimeFromDeparture(target.ToZonedDateTime(GetTimezone(agencies, stops, departure)),
            dayOffset.GetNumeric(),
            departure.DepartureTime);
        
        return IsDepartureValid(agencies, calendarDates, stops, target, timeOffset, tolerance, id, departure, targetDateTime, departureDateTime, 
            WeekdayUtils.GetUtilByDayType(dayOffset)) ? CreateProcessedDeparture(departure, departureDateTime) : null;
    }
}