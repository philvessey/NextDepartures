using System;
using System.Data;
using GTFS;
using Microsoft.Data.Sqlite;
using NextDepartures.Database.Extensions;

GTFSReader<GTFSFeed> reader = new();
var feed = reader.Read(path: "Data/feed.zip");

await using SqliteConnection connection = new(connectionString: "Data Source=Data/feed.db;");
connection.Open();

var command = new SqliteCommand
{
    CommandTimeout = 0,
    CommandType = CommandType.Text,
    Connection = connection
};

command.CommandText = "INSERT INTO GTFS_AGENCY (" +
                            "AgencyId, " +
                            "AgencyName, " +
                            "AgencyUrl, " +
                            "AgencyTimezone, " +
                            "AgencyLang, " +
                            "AgencyPhone, " +
                            "AgencyFareUrl, " +
                            "AgencyEmail) " +
                      "VALUES (" +
                            "@agencyId, " +
                            "@agencyName, " +
                            "@agencyUrl, " +
                            "@agencyTimezone, " +
                            "@agencyLang, " +
                            "@agencyPhone, " +
                            "@agencyFareUrl, " +
                            "@agencyEmail)";

command.Parameters.Clear();

foreach (var a in feed.Agencies)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyId",
        value: a.Id is not null
            ? a.Id.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyName",
        value: a.Name.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyUrl",
        value: a.URL.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyTimezone",
        value: a.Timezone.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyLang",
        value: a.LanguageCode is not null
            ? a.LanguageCode.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyPhone",
        value: a.Phone is not null
            ? a.Phone.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyFareUrl",
        value: a.FareURL is not null
            ? a.FareURL.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyEmail",
        value: a.Email is not null
            ? a.Email.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_CALENDAR (" +
                            "ServiceId, " +
                            "Monday, " +
                            "Tuesday, " +
                            "Wednesday, " +
                            "Thursday, " +
                            "Friday, " +
                            "Saturday, " +
                            "Sunday, " +
                            "StartDate, " +
                            "EndDate) " +
                      "VALUES (" +
                            "@serviceId, " +
                            "@monday, " +
                            "@tuesday, " +
                            "@wednesday, " +
                            "@thursday, " +
                            "@friday, " +
                            "@saturday, " +
                            "@sunday, " +
                            "@startDate, " +
                            "@endDate)";

command.Parameters.Clear();

foreach (var c in feed.Calendars)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@serviceId",
        value: c.ServiceId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@monday",
        value: c.Monday);
    
    command.Parameters.AddWithValue(
        parameterName: "@tuesday",
        value: c.Tuesday);
    
    command.Parameters.AddWithValue(
        parameterName: "@wednesday",
        value: c.Wednesday);
    
    command.Parameters.AddWithValue(
        parameterName: "@thursday",
        value: c.Thursday);
    
    command.Parameters.AddWithValue(
        parameterName: "@friday",
        value: c.Friday);
    
    command.Parameters.AddWithValue(
        parameterName: "@saturday",
        value: c.Saturday);
    
    command.Parameters.AddWithValue(
        parameterName: "@sunday",
        value: c.Sunday);
    
    command.Parameters.AddWithValue(
        parameterName: "@startDate",
        value: c.StartDate);
    
    command.Parameters.AddWithValue(
        parameterName: "@endDate",
        value: c.EndDate);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_CALENDAR_DATES (" +
                            "ServiceId, " +
                            "ExceptionDate, " +
                            "ExceptionType) " +
                      "VALUES (" +
                            "@serviceId, " +
                            "@exceptionDate, " +
                            "@exceptionType)";

command.Parameters.Clear();

foreach (var d in feed.CalendarDates)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@serviceId",
        value: d.ServiceId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@exceptionDate",
        value: d.Date);
    
    command.Parameters.AddWithValue(
        parameterName: "@exceptionType",
        value: d.ExceptionType);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_FARE_ATTRIBUTES (" +
                            "FareId, " +
                            "Price, " +
                            "CurrencyType, " +
                            "PaymentMethod, " +
                            "Transfers, " +
                            "AgencyId, " +
                            "TransferDuration) " +
                      "VALUES (" +
                            "@fareId, " +
                            "@price, " +
                            "@currencyType, " +
                            "@paymentMethod, " +
                            "@transfers, " +
                            "@agencyId, " +
                            "@transferDuration)";

command.Parameters.Clear();

foreach (var f in feed.FareAttributes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@fareId",
        value: f.FareId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@price",
        value: f.Price.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@currencyType",
        value: f.CurrencyType.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@paymentMethod",
        value: f.PaymentMethod);
    
    command.Parameters.AddWithValue(
        parameterName: "@transfers",
        value: f.Transfers.ToString() != string.Empty
            ? f.Transfers
            : string.Empty);
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyId",
        value: f.AgencyId is not null
            ? f.AgencyId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@transferDuration",
        value: f.TransferDuration is not null
            ? f.TransferDuration.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_FARE_RULES (" +
                            "FareId, " +
                            "RouteId, " +
                            "OriginId, " +
                            "DestinationId, " +
                            "ContainsId) " +
                      "VALUES (" +
                            "@fareId, " +
                            "@routeId, " +
                            "@originId, " +
                            "@destinationId, " +
                            "@containsId)";

command.Parameters.Clear();

foreach (var f in feed.FareRules)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@fareId",
        value: f.FareId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@routeId",
        value: f.RouteId is not null
            ? f.RouteId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@originId",
        value: f.OriginId is not null
            ? f.OriginId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@destinationId",
        value: f.DestinationId is not null
            ? f.DestinationId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@containsId",
        value: f.ContainsId is not null
            ? f.ContainsId.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_FREQUENCIES (" +
                            "TripId, " +
                            "StartTime, " +
                            "EndTime, " +
                            "HeadwaySecs, " +
                            "ExactTimes) " +
                      "VALUES (" +
                            "@tripId, " +
                            "@startTime, " +
                            "@endTime, " +
                            "@headwaySecs, " +
                            "@exactTimes)";

command.Parameters.Clear();

foreach (var f in feed.Frequencies)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@tripId",
        value: f.TripId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@startTime",
        value: f.StartTime.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@endTime",
        value: f.EndTime.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@headwaySecs",
        value: f.HeadwaySecs.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@exactTimes",
        value: f.ExactTimes is not null
            ? f.ExactTimes.ToString() != string.Empty
                ? f.ExactTimes
                : string.Empty
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_LEVELS (" +
                            "LevelId, " +
                            "LevelIndex, " +
                            "LevelName) " +
                      "VALUES (" +
                            "@levelId, " +
                            "@levelIndex, " +
                            "@levelName)";

command.Parameters.Clear();

foreach (var l in feed.Levels)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@levelId",
        value: l.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@levelIndex",
        value: l.Index);
    
    command.Parameters.AddWithValue(
        parameterName: "@levelName",
        value: l.Name is not null
            ? l.Name.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_PATHWAYS (" +
                            "PathwayId, " +
                            "FromStopId, " +
                            "ToStopId, " +
                            "PathwayMode, " +
                            "IsBidirectional, " +
                            "Length, " +
                            "TraversalTime, " +
                            "StairCount, " +
                            "MaxSlope, " +
                            "MinWidth, " +
                            "SignpostedAs, " +
                            "ReversedSignpostedAs) " +
                      "VALUES (" +
                            "@pathwayId, " +
                            "@fromStopId, " +
                            "@toStopId, " +
                            "@pathwayMode, " +
                            "@isBidirectional, " +
                            "@length, " +
                            "@traversalTime, " +
                            "@stairCount, " +
                            "@maxSlope, " +
                            "@minWidth, " +
                            "@signpostedAs, " +
                            "@reversedSignpostedAs)";

command.Parameters.Clear();

foreach (var p in feed.Pathways)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@pathwayId",
        value: p.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@fromStopId",
        value: p.FromStopId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@toStopId",
        value: p.ToStopId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@pathwayMode",
        value: p.PathwayMode);
    
    command.Parameters.AddWithValue(
        parameterName: "@isBidirectional",
        value: p.IsBidirectional);
    
    command.Parameters.AddWithValue(
        parameterName: "@length",
        value: p.Length is not null
            ? p.Length
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@traversalTime",
        value: p.TraversalTime is not null
            ? p.TraversalTime
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stairCount",
        value: p.StairCount is not null
            ? p.StairCount
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@maxSlope",
        value: p.MaxSlope is not null
            ? p.MaxSlope.ToString() != string.Empty
                ? p.MaxSlope
                : string.Empty
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@minWidth",
        value: p.MinWidth is not null
            ? p.MinWidth
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@signpostedAs",
        value: p.SignpostedAs is not null
            ? p.SignpostedAs.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@reversedSignpostedAs",
        value: p.ReversedSignpostedAs is not null
            ? p.ReversedSignpostedAs.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_ROUTES (" +
                            "RouteId, " +
                            "AgencyId, " +
                            "RouteShortName, " +
                            "RouteLongName, " +
                            "RouteDesc, " +
                            "RouteType, " +
                            "RouteUrl, " +
                            "RouteColor, " +
                            "RouteTextColor) " +
                      "VALUES (" +
                            "@routeId, " +
                            "@agencyId, " +
                            "@routeShortName, " +
                            "@routeLongName, " +
                            "@routeDesc, " +
                            "@routeType, " +
                            "@routeUrl, " +
                            "@routeColor, " +
                            "@routeTextColor)";

command.Parameters.Clear();

foreach (var r in feed.Routes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@routeId",
        value: r.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@agencyId",
        value: r.AgencyId is not null
            ? r.AgencyId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeShortName",
        value: r.ShortName is not null
            ? r.ShortName.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeLongName",
        value: r.LongName is not null
            ? r.LongName.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeDesc",
        value: r.Description is not null
            ? r.Description.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeType",
        value: r.Type);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeUrl",
        value: r.Url is not null
            ? r.Url.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeColor",
        value: r.Color is not null
            ? r.Color.ToString() != string.Empty
                ? r.Color
                : string.Empty
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@routeTextColor",
        value: r.TextColor is not null
            ? r.TextColor.ToString() != string.Empty
                ? r.TextColor
                : string.Empty
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_SHAPES (" +
                            "ShapeId, " +
                            "ShapePtLat, " +
                            "ShapePtLon, " +
                            "ShapePtSequence, " +
                            "ShapeDistanceTravelled) " +
                      "VALUES (" +
                            "@shapeId, " +
                            "@shapePtLat, " +
                            "@shapePtLon, " +
                            "@shapePtSequence, " +
                            "@shapeDistanceTravelled)";

command.Parameters.Clear();

foreach (var s in feed.Shapes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@shapeId",
        value: s.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@shapePtLat",
        value: s.Latitude);
    
    command.Parameters.AddWithValue(
        parameterName: "@shapePtLon",
        value: s.Longitude);
    
    command.Parameters.AddWithValue(
        parameterName: "@shapePtSequence",
        value: s.Sequence);
    
    command.Parameters.AddWithValue(
        parameterName: "@shapeDistanceTravelled",
        value: s.DistanceTravelled is not null
            ? s.DistanceTravelled
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_STOPS (" +
                            "StopId, " +
                            "StopCode, " +
                            "StopName, " +
                            "StopDesc, " +
                            "StopLat, " +
                            "StopLon, " +
                            "ZoneId, " +
                            "StopUrl, " +
                            "LocationType, " +
                            "ParentStation, " +
                            "StopTimezone, " +
                            "WheelchairBoarding, " +
                            "LevelId, " +
                            "PlatformCode) " +
                      "VALUES (" +
                            "@stopId, " +
                            "@stopCode, " +
                            "@stopName, " +
                            "@stopDesc, " +
                            "@stopLat, " +
                            "@stopLon, " +
                            "@zoneId, " +
                            "@stopUrl, " +
                            "@locationType, " +
                            "@parentStation, " +
                            "@stopTimezone, " +
                            "@wheelchairBoarding, " +
                            "@levelId, " +
                            "@platformCode)";

command.Parameters.Clear();

foreach (var s in feed.Stops)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@stopId",
        value: s.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@stopCode",
        value: s.Code is not null
            ? s.Code.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopName",
        value: s.Name is not null
            ? s.Name.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopDesc",
        value: s.Description is not null
            ? s.Description.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopLat",
        value: s.Latitude);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopLon",
        value: s.Longitude);
    
    command.Parameters.AddWithValue(
        parameterName: "@zoneId",
        value: s.Zone is not null
            ? s.Zone.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopUrl",
        value: s.Url is not null
            ? s.Url.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@locationType",
        value: s.LocationType is not null
            ? s.LocationType.ToString() != string.Empty
                ? s.LocationType
                : string.Empty
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@parentStation",
        value: s.ParentStation is not null
            ? s.ParentStation.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopTimezone",
        value: s.Timezone is not null
            ? s.Timezone.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@wheelchairBoarding",
        value: s.WheelchairBoarding is not null
            ? s.WheelchairBoarding.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@levelId",
        value: s.LevelId is not null
            ? s.LevelId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@platformCode",
        value: s.PlatformCode is not null
            ? s.PlatformCode.TrimDoubleQuotes()
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_STOP_TIMES (" +
                            "TripId, " +
                            "ArrivalTime, " +
                            "DepartureTime, " +
                            "StopId, " +
                            "StopSequence, " +
                            "StopHeadsign, " +
                            "PickupType, " +
                            "DropOffType, " +
                            "ShapeDistTravelled, " +
                            "Timepoint) " +
                      "VALUES (" +
                            "@tripId, " +
                            "@arrivalTime, " +
                            "@departureTime, " +
                            "@stopId, " +
                            "@stopSequence, " +
                            "@stopHeadsign, " +
                            "@pickupType, " +
                            "@dropOffType, " +
                            "@shapeDistTravelled, " +
                            "@timepoint)";

command.Parameters.Clear();

foreach (var s in feed.StopTimes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@tripId",
        value: s.TripId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@arrivalTime",
        value: s.ArrivalTime is not null
            ? s.ArrivalTime.ToString()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@departureTime",
        value: s.DepartureTime is not null
            ? s.DepartureTime.ToString()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopId",
        value: s.StopId is not null
            ? s.StopId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopSequence",
        value: s.StopSequence);
    
    command.Parameters.AddWithValue(
        parameterName: "@stopHeadsign",
        value: s.StopHeadsign is not null
            ? s.StopHeadsign.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@pickupType",
        value: s.PickupType is not null
            ? s.PickupType.ToString() != string.Empty
                ? s.PickupType
                : string.Empty
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@dropOffType",
        value: s.DropOffType is not null
            ? s.DropOffType.ToString() != string.Empty
                ? s.DropOffType
                : string.Empty
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@shapeDistTravelled",
        value: s.ShapeDistTravelled is not null
            ? s.ShapeDistTravelled
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@timepoint",
        value: s.TimepointType.ToString() != string.Empty
            ? s.TimepointType
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_TRANSFERS (" +
                            "FromStopId, " +
                            "ToStopId, " +
                            "TransferType, " +
                            "MinTransferTime) " +
                      "VALUES (" +
                            "@fromStopId, " +
                            "@toStopId, " +
                            "@transferType, " +
                            "@minTransferTime)";

command.Parameters.Clear();

foreach (var t in feed.Transfers)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@fromStopId",
        value: t.FromStopId is not null
            ? t.FromStopId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@toStopId",
        value: t.ToStopId is not null
            ? t.ToStopId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@transferType",
        value: t.TransferType.ToString() != string.Empty
            ? t.TransferType
            : string.Empty);
    
    command.Parameters.AddWithValue(
        parameterName: "@minTransferTime",
        value: t.MinimumTransferTime is not null
            ? t.MinimumTransferTime
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_TRIPS (" +
                            "RouteId, " +
                            "ServiceId, " +
                            "TripId, " +
                            "TripHeadsign, " +
                            "TripShortName, " +
                            "DirectionId, " +
                            "BlockId, " +
                            "ShapeId, " +
                            "WheelchairAccessible) " +
                      "VALUES (" +
                            "@routeId, " +
                            "@serviceId, " +
                            "@tripId, " +
                            "@tripHeadsign, " +
                            "@tripShortName, " +
                            "@directionId, " +
                            "@blockId, " +
                            "@shapeId, " +
                            "@wheelchairAccessible)";

command.Parameters.Clear();

foreach (var t in feed.Trips)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue(
        parameterName: "@routeId",
        value: t.RouteId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@serviceId",
        value: t.ServiceId.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@tripId",
        value: t.Id.TrimDoubleQuotes());
    
    command.Parameters.AddWithValue(
        parameterName: "@tripHeadsign",
        value: t.Headsign is not null
            ? t.Headsign.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@tripShortName",
        value: t.ShortName is not null
            ? t.ShortName.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@directionId",
        value: t.Direction is not null
            ? t.Direction
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@blockId",
        value: t.BlockId is not null
            ? t.BlockId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@shapeId",
        value: t.ShapeId is not null
            ? t.ShapeId.TrimDoubleQuotes()
            : DBNull.Value);
    
    command.Parameters.AddWithValue(
        parameterName: "@wheelchairAccessible",
        value: t.AccessibilityType is not null
            ? t.AccessibilityType.ToString() != string.Empty
                ? t.AccessibilityType
                : string.Empty
            : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "CREATE INDEX GTFS_STOP_TIMES_INDEX ON GTFS_STOP_TIMES (" +
                            "TripId, " +
                            "StopId, " +
                            "PickupType, " +
                            "ArrivalTime, " +
                            "DepartureTime, " +
                            "StopSequence, " +
                            "StopHeadsign, " +
                            "DropOffType, " +
                            "ShapeDistTravelled, " +
                            "Timepoint)";

await command.ExecuteNonQueryAsync();

await command.DisposeAsync();
await connection.CloseAsync();