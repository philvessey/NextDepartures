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
                            "Id, " + 
                            "Name, " + 
                            "URL, " + 
                            "Timezone, " + 
                            "LanguageCode, " + 
                            "Phone, " + 
                            "FareURL, " + 
                            "Email" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@name, " + 
                            "@url, " + 
                            "@timezone, " + 
                            "@languageCode, " + 
                            "@phone, " + 
                            "@fareUrl, " + 
                            "@email" + 
                            ")";

command.Parameters.Clear();

foreach (var a in feed.Agencies)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", a.Id != null ? a.Id : DBNull.Value);
    command.Parameters.AddWithValue("@name", a.Name);
    command.Parameters.AddWithValue("@url", a.URL);
    command.Parameters.AddWithValue("@timezone", a.Timezone);
    command.Parameters.AddWithValue("@languageCode", a.LanguageCode != null ? a.LanguageCode : DBNull.Value);
    command.Parameters.AddWithValue("@phone", a.Phone != null ? a.Phone : DBNull.Value);
    command.Parameters.AddWithValue("@fareUrl", a.FareURL != null ? a.FareURL : DBNull.Value);
    command.Parameters.AddWithValue("@email", a.Email != null ? a.Email : DBNull.Value);

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

command.CommandText = "INSERT INTO GTFS_CALENDAR_DATE (" + 
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

command.CommandText = "INSERT INTO GTFS_FARE_ATTRIBUTE (" + 
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

command.CommandText = "INSERT INTO GTFS_FARE_RULE (" + 
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

command.CommandText = "INSERT INTO GTFS_FREQUENCY (" + 
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

command.CommandText = "INSERT INTO GTFS_LEVEL (" + 
                            "Id, " + 
                            "Idx, " + 
                            "Name" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@idx, " + 
                            "@name" + 
                            ")";

command.Parameters.Clear();

foreach (var l in feed.Levels)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", l.Id);
    command.Parameters.AddWithValue("@idx", l.Index);
    command.Parameters.AddWithValue("@name", l.Name != null ? l.Name : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_PATHWAY (" + 
                            "Id, " + 
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
                            "@id, " + 
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
    
    command.Parameters.AddWithValue("@id", p.Id);
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

command.CommandText = "INSERT INTO GTFS_ROUTE (" + 
                            "Id, " + 
                            "AgencyId, " + 
                            "ShortName, " + 
                            "LongName, " + 
                            "Description, " + 
                            "Type, " + 
                            "Url, " + 
                            "Color, " + 
                            "TextColor" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@agencyId, " + 
                            "@shortName, " + 
                            "@longName, " + 
                            "@description, " + 
                            "@type, " + 
                            "@url, " + 
                            "@color, " + 
                            "@textColor" + 
                            ")";

command.Parameters.Clear();

foreach (var r in feed.Routes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", r.Id);
    command.Parameters.AddWithValue("@agencyId", r.AgencyId != null ? r.AgencyId : DBNull.Value);
    command.Parameters.AddWithValue("@shortName", r.ShortName != null ? r.ShortName != string.Empty ? r.ShortName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@longName", r.LongName != null ? r.LongName != string.Empty ? r.LongName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@description", r.Description != null ? r.Description : DBNull.Value);
    command.Parameters.AddWithValue("@type", r.Type);
    command.Parameters.AddWithValue("@url", r.Url != null ? r.Url : DBNull.Value);
    command.Parameters.AddWithValue("@color", r.Color != null ? r.Color.ToString() != string.Empty ? r.Color : 0xFFFFFF : DBNull.Value);
    command.Parameters.AddWithValue("@textColor", r.TextColor != null ? r.TextColor.ToString() != string.Empty ? r.TextColor : 0x000000 : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_SHAPE (" + 
                            "Id, " + 
                            "Longitude, " + 
                            "Latitude, " + 
                            "Sequence, " + 
                            "DistanceTravelled" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@longitude, " + 
                            "@latitude, " + 
                            "@sequence, " + 
                            "@distanceTravelled" + 
                            ")";

command.Parameters.Clear();

foreach (var s in feed.Shapes)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", s.Id);
    command.Parameters.AddWithValue("@longitude", s.Longitude);
    command.Parameters.AddWithValue("@latitude", s.Latitude);
    command.Parameters.AddWithValue("@sequence", s.Sequence);
    command.Parameters.AddWithValue("@distanceTravelled", s.DistanceTravelled != null ? s.DistanceTravelled : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_STOP (" + 
                            "Id, " + 
                            "Code, " + 
                            "Name, " + 
                            "Description, " + 
                            "Longitude, " + 
                            "Latitude, " + 
                            "Zone, " + 
                            "Url, " + 
                            "LocationType, " + 
                            "ParentStation, " + 
                            "Timezone, " + 
                            "WheelchairBoarding, " + 
                            "LevelId, " + 
                            "PlatformCode" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@code, " + 
                            "@name, " + 
                            "@description, " + 
                            "@longitude, " + 
                            "@latitude, " + 
                            "@zone, " + 
                            "@url, " + 
                            "@locationType, " + 
                            "@parentStation, " + 
                            "@timezone, " + 
                            "@wheelchairBoarding, " + 
                            "@levelId, " + 
                            "@platformCode" + 
                            ")";

command.Parameters.Clear();

foreach (var s in feed.Stops)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", s.Id);
    command.Parameters.AddWithValue("@code", s.Code != null ? s.Code != string.Empty ? s.Code : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@name", s.Name != null ? s.Name : DBNull.Value);
    command.Parameters.AddWithValue("@description", s.Description != null ? s.Description : DBNull.Value);
    command.Parameters.AddWithValue("@longitude", s.Longitude);
    command.Parameters.AddWithValue("@latitude", s.Latitude);
    command.Parameters.AddWithValue("@zone", s.Zone != null ? s.Zone : DBNull.Value);
    command.Parameters.AddWithValue("@url", s.Url != null ? s.Url : DBNull.Value);
    command.Parameters.AddWithValue("@locationType", s.LocationType != null ? s.LocationType.ToString() != string.Empty ? s.LocationType : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@parentStation", s.ParentStation != null ? s.ParentStation != string.Empty ? s.ParentStation : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@timezone", s.Timezone != null ? s.Timezone != string.Empty ? s.Timezone : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@wheelchairBoarding", s.WheelchairBoarding != null ? s.WheelchairBoarding != string.Empty ? s.WheelchairBoarding : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@levelId", s.LevelId != null ? s.LevelId : DBNull.Value);
    command.Parameters.AddWithValue("@platformCode", s.PlatformCode != null ? s.PlatformCode : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_STOP_TIME (" + 
                            "TripId, " + 
                            "ArrivalTime, " + 
                            "DepartureTime, " + 
                            "StopId, " + 
                            "StopSequence, " + 
                            "StopHeadsign, " + 
                            "PickupType, " + 
                            "DropOffType, " + 
                            "ShapeDistTravelled, " + 
                            "TimepointType" + 
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
                            "@timepointType" + 
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
    command.Parameters.AddWithValue("@timepointType", s.TimepointType.ToString() != string.Empty ? s.TimepointType : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_TRANSFER (" + 
                            "FromStopId, " + 
                            "ToStopId, " + 
                            "TransferType, " + 
                            "MinimumTransferTime" + 
                            ") " + 
                      "VALUES (" + 
                            "@fromStopId, " + 
                            "@toStopId, " + 
                            "@transferType, " + 
                            "@minimumTransferTime" + 
                            ")";

command.Parameters.Clear();

foreach (var t in feed.Transfers)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@fromStopId", t.FromStopId != null ? t.FromStopId : DBNull.Value);
    command.Parameters.AddWithValue("@toStopId", t.ToStopId != null ? t.ToStopId : DBNull.Value);
    command.Parameters.AddWithValue("@transferType", t.TransferType.ToString() != string.Empty ? t.TransferType : string.Empty);
    command.Parameters.AddWithValue("@minimumTransferTime", t.MinimumTransferTime != null ? t.MinimumTransferTime : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "INSERT INTO GTFS_TRIP (" + 
                            "Id, " + 
                            "RouteId, " + 
                            "ServiceId, " + 
                            "Headsign, " + 
                            "ShortName, " + 
                            "Direction, " + 
                            "BlockId, " + 
                            "ShapeId, " + 
                            "AccessibilityType" + 
                            ") " + 
                      "VALUES (" + 
                            "@id, " + 
                            "@routeId, " + 
                            "@serviceId, " + 
                            "@headsign, " + 
                            "@shortName, " + 
                            "@direction, " + 
                            "@blockId, " + 
                            "@shapeId, " + 
                            "@accessibilityType" + 
                            ")";

command.Parameters.Clear();

foreach (var t in feed.Trips)
{
    command.Parameters.Clear();
    
    command.Parameters.AddWithValue("@id", t.Id);
    command.Parameters.AddWithValue("@routeId", t.RouteId);
    command.Parameters.AddWithValue("@serviceId", t.ServiceId);
    command.Parameters.AddWithValue("@headsign", t.Headsign != null ? t.Headsign : DBNull.Value);
    command.Parameters.AddWithValue("@shortName", t.ShortName != null ? t.ShortName != string.Empty ? t.ShortName : string.Empty : DBNull.Value);
    command.Parameters.AddWithValue("@direction", t.Direction != null ? t.Direction : DBNull.Value);
    command.Parameters.AddWithValue("@blockId", t.BlockId != null ? t.BlockId : DBNull.Value);
    command.Parameters.AddWithValue("@shapeId", t.ShapeId != null ? t.ShapeId : DBNull.Value);
    command.Parameters.AddWithValue("@accessibilityType", t.AccessibilityType != null ? t.AccessibilityType.ToString() != string.Empty ? t.AccessibilityType : string.Empty : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "CREATE INDEX GTFS_STOP_TIME_INDEX ON GTFS_STOP_TIME (" + 
                            "TripId, " + 
                            "StopId, " + 
                            "PickupType, " + 
                            "ArrivalTime, " + 
                            "DepartureTime, " + 
                            "StopSequence, " + 
                            "StopHeadsign, " + 
                            "DropOffType, " + 
                            "ShapeDistTravelled, " + 
                            "TimepointType" + 
                            ")";

await command.ExecuteNonQueryAsync();