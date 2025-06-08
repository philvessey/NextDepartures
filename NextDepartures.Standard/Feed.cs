using System;
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
    /// <param name="dataStorage">The data storage to load into the feed. Required.</param>
    /// <param name="preload">Whether to preload the data. Default is true.</param>
    /// <returns>A new feed instance.</returns>
    public static async Task<Feed> LoadAsync(
        IDataStorage dataStorage,
        bool preload = true) {
        
        if (preload)
            return new Feed(dataStorage: await PreloadDataStorage.LoadAsync(
                dataStorage: dataStorage,
                dataStorageProperties: new DataStorageProperties(dataStorage: dataStorage)));
        
        return new Feed(dataStorage: dataStorage);
    }
    
    private static Departure CreateProcessedDeparture(
        Departure departure,
        DateTime departureDateTime) {
        
        return new Departure
        {
            DepartureTime = new TimeOfDay
            {
                Hours = departureDateTime.Hour,
                Minutes = departureDateTime.Minute,
                Seconds = departureDateTime.Second
            },
            
            DepartureDateTime = departureDateTime,
            StopId = departure.StopId,
            StopSequence = departure.StopSequence,
            TripId = departure.TripId,
            ServiceId = departure.ServiceId,
            TripHeadsign = departure.TripHeadsign,
            TripShortName = departure.TripShortName,
            AgencyId = departure.AgencyId,
            RouteShortName = departure.RouteShortName,
            RouteLongName = departure.RouteLongName,
            Monday = departure.Monday,
            Tuesday = departure.Tuesday,
            Wednesday = departure.Wednesday,
            Thursday = departure.Thursday,
            Friday = departure.Friday,
            Saturday = departure.Saturday,
            Sunday = departure.Sunday,
            StartDate = departure.StartDate,
            EndDate = departure.EndDate
        };
    }
    
    private static Service CreateProcessedService(
        List<Agency> agencies,
        List<Stop> stops,
        Departure departure,
        string type) {
        
        var steps = new[]
        {
            () => agencies.FirstOrDefault(predicate: a => a.Id == departure.AgencyId)?.Id,
            () => agencies.FirstOrDefault()?.Id
        };
        
        var agencyId = StringUtils.GetPossibleString(
            fallback: "unknown",
            steps: steps);
        
        steps =
        [
            () => agencies.FirstOrDefault(predicate: a => a.Id == departure.AgencyId)?.Name,
            () => agencies.FirstOrDefault()?.Name
        ];
        
        var agencyName = StringUtils.GetPossibleString(
            fallback: "unknown",
            steps: steps);
        
        var destination = stops.FirstOrDefault(predicate: s =>
            departure.RouteShortName.Contains(
                value: s.Id.ToIncludePrefix(prefix: "_"),
                comparisonType: StringComparison.InvariantCultureIgnoreCase) ||
            departure.RouteShortName.Contains(
                value: s.Id.ToIncludePrefix(prefix: "->"),
                comparisonType: StringComparison.InvariantCultureIgnoreCase));
        
        steps =
        [
            () => destination?.Name,
            () => departure.TripHeadsign,
            () => departure.TripShortName,
            () => departure.RouteLongName
        ];
        
        var destinationName = StringUtils.GetPossibleString(
            fallback: "unknown",
            steps: steps);
        
        steps =
        [
            () => destination?.Name,
            () => departure.RouteShortName
        ];
        
        var routeName = StringUtils.GetPossibleString(
            fallback: "unknown",
            steps: steps);
        
        steps =
        [
            () => stops.FirstOrDefault(predicate: s => s.Id == departure.StopId)?.Name
        ];
        
        var stopName = StringUtils.GetPossibleString(
            fallback: "unknown",
            steps: steps);
        
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
            StopSequence = departure.StopSequence,
            TripId = departure.TripId,
            Type = type
        };
    }
    
    private static string CreateProcessedTimezone(
        List<Agency> agencies,
        List<Stop> stops,
        Departure departure) {
        
        var steps = new[]
        {
            () => stops.FirstOrDefault(predicate: s => s.Id == departure.StopId)?.Timezone,
            () => agencies.FirstOrDefault(predicate: a => a.Id == departure.AgencyId)?.Timezone,
            () => agencies.FirstOrDefault()?.Timezone
        };
        
        return StringUtils.GetPossibleString(
            fallback: "Etc/UTC",
            steps: steps);
    }
    
    private static List<Departure> GetDeparturesOnDay(
        List<Agency> agencies,
        List<CalendarDate> calendarDates,
        List<Stop> stops,
        List<Departure> departures,
        DateTime target,
        DayOffsetType dayOffset,
        TimeSpan timeOffset,
        TimeSpan tolerance,
        string id) {
        
        List<Departure> results = [];
        
        foreach (var d in departures)
            results.AddIfNotNull(item: FetchProcessedDeparture(
                agencies: agencies,
                calendarDates: calendarDates,
                stops: stops,
                target: target,
                dayOffset: dayOffset,
                timeOffset: timeOffset,
                tolerance: tolerance,
                id: id,
                departure: d));
        
        return results;
    }
    
    private static bool CheckProcessedDeparture(
        List<Agency> agencies,
        List<CalendarDate> calendarDates,
        List<Stop> stops,
        DateTime target,
        TimeSpan timeOffset,
        TimeSpan tolerance,
        string id,
        Departure departure,
        DateTime targetDateTime,
        DateTime departureDateTime,
        Func<DayOfWeek, Departure, bool> dayOfWeekMapper) {
        
        if (departure.StartDate > targetDateTime.Date)
            return false;
        
        if (departure.EndDate < targetDateTime.Date)
            return false;
        
        var zonedDateTime = target.ToZonedDateTime(timezone: CreateProcessedTimezone(
            agencies: agencies,
            stops: stops,
            departure: departure));
        
        var runningToday = dayOfWeekMapper(zonedDateTime.DayOfWeek, departure);
        
        var isAdded = calendarDates.Any(predicate: d =>
            d.ServiceId == departure.ServiceId &&
            d.Date == targetDateTime.Date &&
            d.ExceptionType is ExceptionType.Added);
        
        if (!runningToday && !isAdded)
            return false;
        
        var isRemoved = calendarDates.Any(predicate: d =>
            d.ServiceId == departure.ServiceId &&
            d.Date == targetDateTime.Date &&
            d.ExceptionType is ExceptionType.Removed);
        
        if (runningToday && isRemoved)
            return false;
        
        var prefixMatch = departure.RouteShortName.Contains(
            value: id.ToIncludePrefix(prefix: "_"),
            comparisonType: StringComparison.InvariantCultureIgnoreCase);
        
        if (prefixMatch)
            return false;
        
        prefixMatch = departure.RouteShortName.Contains(
            value: id.ToIncludePrefix(prefix: "->"),
            comparisonType: StringComparison.InvariantCultureIgnoreCase);
        
        if (prefixMatch)
            return false;
        
        DateTime minimumDateTime;
        DateTime maximumDateTime;
        
        if (timeOffset > TimeSpan.Zero && tolerance > TimeSpan.Zero)
        {
            minimumDateTime = zonedDateTime.Add(value: timeOffset);
            maximumDateTime = zonedDateTime.Add(value: timeOffset + tolerance);
        }
        else if (tolerance > TimeSpan.Zero)
        {
            minimumDateTime = zonedDateTime;
            maximumDateTime = zonedDateTime.Add(value: tolerance);
        }
        else if (timeOffset > TimeSpan.Zero)
        {
            minimumDateTime = zonedDateTime.Add(value: timeOffset);
            maximumDateTime = DateTime.MaxValue;
        }
        else
        {
            minimumDateTime = zonedDateTime;
            maximumDateTime = DateTime.MaxValue;
        }
        
        return departureDateTime >= minimumDateTime && departureDateTime <= maximumDateTime;
    }
    
    private static Departure FetchProcessedDeparture(
        List<Agency> agencies,
        List<CalendarDate> calendarDates,
        List<Stop> stops,
        DateTime target,
        DayOffsetType dayOffset,
        TimeSpan timeOffset,
        TimeSpan tolerance,
        string id,
        Departure departure) {
        
        var targetDateTime = target.ToZonedDateTime(timezone: CreateProcessedTimezone(
            agencies: agencies,
            stops: stops,
            departure: departure));
        
        var departureDateTime = DateTimeUtils.GetFromDeparture(
            zonedDateTime: target.ToZonedDateTime(timezone: CreateProcessedTimezone(
                agencies: agencies,
                stops: stops,
                departure: departure)),
            dayOffset: dayOffset.ToInt32(),
            departureTime: departure.DepartureTime);
        
        var checkProcessedDeparture = CheckProcessedDeparture(
            agencies: agencies,
            calendarDates: calendarDates,
            stops: stops,
            target: target,
            timeOffset: timeOffset,
            tolerance: tolerance,
            id: id,
            departure: departure,
            targetDateTime: targetDateTime.AddDays(value: dayOffset.ToInt32()),
            departureDateTime: departureDateTime,
            dayOfWeekMapper: WeekdayUtils.GetFromOffset(dayOffsetType: dayOffset));
        
        var processedDeparture = CreateProcessedDeparture(
            departure: departure,
            departureDateTime: departureDateTime);
        
        return checkProcessedDeparture
            ? processedDeparture
            : null;
    }
}