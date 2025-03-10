using System;
using System.Data;
using GTFS;
using Microsoft.Data.Sqlite;

GTFSReader<GTFSFeed> reader = new();
var feed = reader.Read("Data/feed.zip");

await using SqliteConnection connection = new("Data Source=Data/feed.db;");
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
                            "AgencyEmail" + 
                            ") " + 
                      "VALUES (" + 
                            "@agencyId, " + 
                            "@agencyName, " + 
                            "@agencyUrl, " + 
                            "@agencyTimezone, " + 
                            "@agencyLang, " + 
                            "@agencyPhone, " + 
                            "@agencyFareUrl, " + 
                            "@agencyEmail" + 
                            ")";

command.Parameters.Clear();

foreach (var a in feed.Agencies)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@agencyId", a.Id != null ? a.Id : DBNull.Value);
    command.Parameters.AddWithValue("@agencyName", a.Name);
    command.Parameters.AddWithValue("@agencyUrl", a.URL);
    command.Parameters.AddWithValue("@agencyTimezone", a.Timezone);
    command.Parameters.AddWithValue("@agencyLang", a.LanguageCode != null ? a.LanguageCode : DBNull.Value);
    command.Parameters.AddWithValue("@agencyPhone", a.Phone != null ? a.Phone : DBNull.Value);
    command.Parameters.AddWithValue("@agencyFareUrl", a.FareURL != null ? a.FareURL : DBNull.Value);
    command.Parameters.AddWithValue("@agencyEmail", a.Email != null ? a.Email : DBNull.Value);

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
                            "EndDate" + 
                            ") " + 
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
                            "@endDate" + 
                            ")";

command.Parameters.Clear();

foreach (var c in feed.Calendars)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@serviceId", c.ServiceId);
    command.Parameters.AddWithValue("@monday", c.Monday);
    command.Parameters.AddWithValue("@tuesday", c.Tuesday);
    command.Parameters.AddWithValue("@wednesday", c.Wednesday);
    command.Parameters.AddWithValue("@thursday", c.Thursday);
    command.Parameters.AddWithValue("@friday", c.Friday);
    command.Parameters.AddWithValue("@saturday", c.Saturday);
    command.Parameters.AddWithValue("@sunday", c.Sunday);
    command.Parameters.AddWithValue("@startDate", c.StartDate);
    command.Parameters.AddWithValue("@endDate", c.EndDate);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_CALENDAR_DATES (" + 
                            "ServiceId, " + 
                            "ExceptionDate, " + 
                            "ExceptionType" + 
                            ") " + 
                      "VALUES (" + 
                            "@serviceId, " + 
                            "@exceptionDate, " + 
                            "@exceptionType" + 
                            ")";

command.Parameters.Clear();

foreach (var d in feed.CalendarDates)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@serviceId", d.ServiceId);
    command.Parameters.AddWithValue("@exceptionDate", d.Date);
    command.Parameters.AddWithValue("@exceptionType", d.ExceptionType);

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
                            "TransferDuration" + 
                            ") " + 
                      "VALUES (" + 
                            "@fareId, " + 
                            "@price, " + 
                            "@currencyType, " + 
                            "@paymentMethod, " + 
                            "@transfers, " + 
                            "@agencyId, " + 
                            "@transferDuration" + 
                            ")";

command.Parameters.Clear();

foreach (var f in feed.FareAttributes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@fareId", f.FareId);
    command.Parameters.AddWithValue("@price", f.Price);
    command.Parameters.AddWithValue("@currencyType", f.CurrencyType);
    command.Parameters.AddWithValue("@paymentMethod", f.PaymentMethod);
    command.Parameters.AddWithValue("@transfers", f.Transfers.ToString() != string.Empty ? f.Transfers : string.Empty);
    command.Parameters.AddWithValue("@agencyId", f.AgencyId != null ? f.AgencyId : DBNull.Value);
    command.Parameters.AddWithValue("@transferDuration", f.TransferDuration != null ? f.TransferDuration != string.Empty ? f.TransferDuration : string.Empty : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_FARE_RULES (" + 
                            "FareId, " + 
                            "RouteId, " + 
                            "OriginId, " + 
                            "DestinationId, " + 
                            "ContainsId" + 
                            ") " + 
                      "VALUES (" + 
                            "@fareId, " + 
                            "@routeId, " + 
                            "@originId, " + 
                            "@destinationId, " + 
                            "@containsId" + 
                            ")";

command.Parameters.Clear();

foreach (var f in feed.FareRules)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@fareId", f.FareId);
    command.Parameters.AddWithValue("@routeId", f.RouteId != null ? f.RouteId : DBNull.Value);
    command.Parameters.AddWithValue("@originId", f.OriginId != null ? f.OriginId : DBNull.Value);
    command.Parameters.AddWithValue("@destinationId", f.DestinationId != null ? f.DestinationId : DBNull.Value);
    command.Parameters.AddWithValue("@containsId", f.ContainsId != null ? f.ContainsId : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_FREQUENCIES (" + 
                            "TripId, " + 
                            "StartTime, " + 
                            "EndTime, " + 
                            "HeadwaySecs, " + 
                            "ExactTimes" + 
                            ") " + 
                      "VALUES (" + 
                            "@tripId, " + 
                            "@startTime, " + 
                            "@endTime, " + 
                            "@headwaySecs, " + 
                            "@exactTimes" + 
                            ")";

command.Parameters.Clear();

foreach (var f in feed.Frequencies)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@tripId", f.TripId);
    command.Parameters.AddWithValue("@startTime", f.StartTime);
    command.Parameters.AddWithValue("@endTime", f.EndTime);
    command.Parameters.AddWithValue("@headwaySecs", f.HeadwaySecs);
    command.Parameters.AddWithValue("@exactTimes", f.ExactTimes != null ? f.ExactTimes.ToString() != string.Empty ? f.ExactTimes : string.Empty : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_LEVELS (" + 
                            "LevelId, " + 
                            "LevelIndex, " + 
                            "LevelName" + 
                            ") " + 
                      "VALUES (" + 
                            "@levelId, " + 
                            "@levelIndex, " + 
                            "@levelName" + 
                            ")";

command.Parameters.Clear();

foreach (var l in feed.Levels)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@levelId", l.Id);
    command.Parameters.AddWithValue("@levelIndex", l.Index);
    command.Parameters.AddWithValue("@levelName", l.Name != null ? l.Name : DBNull.Value);

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
                            "ReversedSignpostedAs" + 
                            ") " + 
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
                            "@reversedSignpostedAs" + 
                            ")";

command.Parameters.Clear();

foreach (var p in feed.Pathways)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@pathwayId", p.Id);
    command.Parameters.AddWithValue("@fromStopId", p.FromStopId);
    command.Parameters.AddWithValue("@toStopId", p.ToStopId);
    command.Parameters.AddWithValue("@pathwayMode", p.PathwayMode);
    command.Parameters.AddWithValue("@isBidirectional", p.IsBidirectional);
    command.Parameters.AddWithValue("@length", p.Length != null ? p.Length : DBNull.Value);
    command.Parameters.AddWithValue("@traversalTime", p.TraversalTime != null ? p.TraversalTime : DBNull.Value);
    command.Parameters.AddWithValue("@stairCount", p.StairCount != null ? p.StairCount : DBNull.Value);
    command.Parameters.AddWithValue("@maxSlope", p.MaxSlope != null ? p.MaxSlope.ToString() != string.Empty ? p.MaxSlope : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@minWidth", p.MinWidth != null ? p.MinWidth : DBNull.Value);
    command.Parameters.AddWithValue("@signpostedAs", p.SignpostedAs != null ? p.SignpostedAs : DBNull.Value);
    command.Parameters.AddWithValue("@reversedSignpostedAs", p.ReversedSignpostedAs != null ? p.ReversedSignpostedAs : DBNull.Value);
    
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
                            "RouteTextColor" +
                            ") " + 
                      "VALUES (" + 
                            "@routeId, " + 
                            "@agencyId, " + 
                            "@routeShortName, " + 
                            "@routeLongName, " + 
                            "@routeDesc, " + 
                            "@routeType, " + 
                            "@routeUrl, " + 
                            "@routeColor, " + 
                            "@routeTextColor" +
                            ")";

command.Parameters.Clear();

foreach (var r in feed.Routes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@routeId", r.Id);
    command.Parameters.AddWithValue("@agencyId", r.AgencyId != null ? r.AgencyId : DBNull.Value);
    command.Parameters.AddWithValue("@routeShortName", r.ShortName != null ? r.ShortName != string.Empty ? r.ShortName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@routeLongName", r.LongName != null ? r.LongName != string.Empty ? r.LongName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@routeDesc", r.Description != null ? r.Description : DBNull.Value);
    command.Parameters.AddWithValue("@routeType", r.Type);
    command.Parameters.AddWithValue("@routeUrl", r.Url != null ? r.Url : DBNull.Value);
    command.Parameters.AddWithValue("@routeColor", r.Color != null ? r.Color.ToString() != string.Empty ? r.Color : 0xFFFFFF : DBNull.Value);
    command.Parameters.AddWithValue("@routeTextColor", r.TextColor != null ? r.TextColor.ToString() != string.Empty ? r.TextColor : 0x000000 : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_SHAPES (" + 
                            "ShapeId, " + 
                            "ShapePtLat, " + 
                            "ShapePtLon, " + 
                            "ShapePtSequence, " + 
                            "ShapeDistanceTravelled" + 
                            ") " + 
                      "VALUES (" + 
                            "@shapeId, " + 
                            "@shapePtLat, " + 
                            "@shapePtLon, " + 
                            "@shapePtSequence, " + 
                            "@shapeDistanceTravelled" + 
                            ")";

command.Parameters.Clear();

foreach (var s in feed.Shapes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@shapeId", s.Id);
    command.Parameters.AddWithValue("@shapePtLat", s.Latitude);
    command.Parameters.AddWithValue("@shapePtLon", s.Longitude);
    command.Parameters.AddWithValue("@shapePtSequence", s.Sequence);
    command.Parameters.AddWithValue("@shapeDistanceTravelled", s.DistanceTravelled != null ? s.DistanceTravelled : DBNull.Value);

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
                            "PlatformCode" + 
                            ") " + 
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
                            "@platformCode" + 
                            ")";

command.Parameters.Clear();

foreach (var s in feed.Stops)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@stopId", s.Id);
    command.Parameters.AddWithValue("@stopCode", s.Code != null ? s.Code != string.Empty ? s.Code : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@stopName", s.Name != null ? s.Name : DBNull.Value);
    command.Parameters.AddWithValue("@stopDesc", s.Description != null ? s.Description : DBNull.Value);
    command.Parameters.AddWithValue("@stopLat", s.Latitude);
    command.Parameters.AddWithValue("@stopLon", s.Longitude);
    command.Parameters.AddWithValue("@zoneId", s.Zone != null ? s.Zone : DBNull.Value);
    command.Parameters.AddWithValue("@stopUrl", s.Url != null ? s.Url : DBNull.Value);
    command.Parameters.AddWithValue("@locationType", s.LocationType != null ? s.LocationType.ToString() != string.Empty ? s.LocationType : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@parentStation", s.ParentStation != null ? s.ParentStation != string.Empty ? s.ParentStation : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@stopTimezone", s.Timezone != null ? s.Timezone != string.Empty ? s.Timezone : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@wheelchairBoarding", s.WheelchairBoarding != null ? s.WheelchairBoarding != string.Empty ? s.WheelchairBoarding : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@levelId", s.LevelId != null ? s.LevelId : DBNull.Value);
    command.Parameters.AddWithValue("@platformCode", s.PlatformCode != null ? s.PlatformCode : DBNull.Value);
    
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
                            "Timepoint" + 
                            ") " + 
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
                            "@timepoint" + 
                            ")";

command.Parameters.Clear();

foreach (var s in feed.StopTimes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@tripId", s.TripId);
    command.Parameters.AddWithValue("@arrivalTime", s.ArrivalTime != null ? s.ArrivalTime.ToString() : DBNull.Value);
    command.Parameters.AddWithValue("@departureTime", s.DepartureTime != null ? s.DepartureTime.ToString() : DBNull.Value);
    command.Parameters.AddWithValue("@stopId", s.StopId != null ? s.StopId : DBNull.Value);
    command.Parameters.AddWithValue("@stopSequence", s.StopSequence);
    command.Parameters.AddWithValue("@stopHeadsign", s.StopHeadsign != null ? s.StopHeadsign : DBNull.Value);
    command.Parameters.AddWithValue("@pickupType", s.PickupType != null ? s.PickupType.ToString() != string.Empty ? s.PickupType : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@dropOffType", s.DropOffType != null ? s.DropOffType.ToString() != string.Empty ? s.DropOffType : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@shapeDistTravelled", s.ShapeDistTravelled != null ? s.ShapeDistTravelled : DBNull.Value);
    command.Parameters.AddWithValue("@timepoint", s.TimepointType.ToString() != string.Empty ? s.TimepointType : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_TRANSFERS (" + 
                            "FromStopId, " + 
                            "ToStopId, " + 
                            "TransferType, " + 
                            "MinTransferTime" + 
                            ") " + 
                      "VALUES (" + 
                            "@fromStopId, " + 
                            "@toStopId, " + 
                            "@transferType, " + 
                            "@minTransferTime" + 
                            ")";

command.Parameters.Clear();

foreach (var t in feed.Transfers)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@fromStopId", t.FromStopId != null ? t.FromStopId : DBNull.Value);
    command.Parameters.AddWithValue("@toStopId", t.ToStopId != null ? t.ToStopId : DBNull.Value);
    command.Parameters.AddWithValue("@transferType", t.TransferType.ToString() != string.Empty ? t.TransferType : string.Empty);
    command.Parameters.AddWithValue("@minTransferTime", t.MinimumTransferTime != null ? t.MinimumTransferTime : DBNull.Value);
    
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
                            "WheelchairAccessible" +
                            ") " + 
                      "VALUES (" + 
                            "@routeId, " + 
                            "@serviceId, " +
                            "@tripId, " +
                            "@tripHeadsign, " + 
                            "@tripShortName, " + 
                            "@directionId, " + 
                            "@blockId, " + 
                            "@shapeId, " + 
                            "@wheelchairAccessible" +
                            ")";

command.Parameters.Clear();

foreach (var t in feed.Trips)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@routeId", t.RouteId);
    command.Parameters.AddWithValue("@serviceId", t.ServiceId);
    command.Parameters.AddWithValue("@tripId", t.Id);
    command.Parameters.AddWithValue("@tripHeadsign", t.Headsign != null ? t.Headsign : DBNull.Value);
    command.Parameters.AddWithValue("@tripShortName", t.ShortName != null ? t.ShortName != string.Empty ? t.ShortName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@directionId", t.Direction != null ? t.Direction : DBNull.Value);
    command.Parameters.AddWithValue("@blockId", t.BlockId != null ? t.BlockId : DBNull.Value);
    command.Parameters.AddWithValue("@shapeId", t.ShapeId != null ? t.ShapeId : DBNull.Value);
    command.Parameters.AddWithValue("@wheelchairAccessible", t.AccessibilityType != null ? t.AccessibilityType.ToString() != string.Empty ? t.AccessibilityType : string.Empty : DBNull.Value);
    
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
                            "Timepoint" + 
                            ")";

await command.ExecuteNonQueryAsync();