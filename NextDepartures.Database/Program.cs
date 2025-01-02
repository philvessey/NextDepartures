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

command.CommandText = "DROP TABLE IF EXISTS GTFS_AGENCY";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_AGENCY (" + 
                            "Id NVARCHAR(255), " + 
                            "Name NVARCHAR(255), " + 
                            "URL NVARCHAR(255), " + 
                            "Timezone NVARCHAR(255), " + 
                            "LanguageCode NVARCHAR(255), " + 
                            "Phone NVARCHAR(255), " + 
                            "FareURL NVARCHAR(255), " + 
                            "Email NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(a.Id) ? a.Id : DBNull.Value);
    command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(a.Name) ? a.Name : DBNull.Value);
    command.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(a.URL) ? a.URL : DBNull.Value);
    command.Parameters.AddWithValue("@timezone", !string.IsNullOrEmpty(a.Timezone) ? a.Timezone : DBNull.Value);
    command.Parameters.AddWithValue("@languageCode", !string.IsNullOrEmpty(a.LanguageCode) ? a.LanguageCode : DBNull.Value);
    command.Parameters.AddWithValue("@phone", !string.IsNullOrEmpty(a.Phone) ? a.Phone : DBNull.Value);
    command.Parameters.AddWithValue("@fareUrl", !string.IsNullOrEmpty(a.FareURL) ? a.FareURL : DBNull.Value);
    command.Parameters.AddWithValue("@email", !string.IsNullOrEmpty(a.Email) ? a.Email : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_CALENDAR";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_CALENDAR (" + 
                            "ServiceId NVARCHAR(255) PRIMARY KEY, " + 
                            "Monday BIT, " + 
                            "Tuesday BIT, " + 
                            "Wednesday BIT, " + 
                            "Thursday BIT, " + 
                            "Friday BIT, " + 
                            "Saturday BIT, " + 
                            "Sunday BIT, " + 
                            "StartDate DATETIME, " + 
                            "EndDate DATETIME" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(c.ServiceId) ? c.ServiceId : DBNull.Value);
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

command.CommandText = "DROP TABLE IF EXISTS GTFS_CALENDAR_DATE";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_CALENDAR_DATE (" + 
                            "ServiceId NVARCHAR(255), " + 
                            "ExceptionDate DATETIME, " + 
                            "ExceptionType INT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(d.ServiceId) ? d.ServiceId : DBNull.Value);
    command.Parameters.AddWithValue("@exceptionDate", d.Date);
    command.Parameters.AddWithValue("@exceptionType", d.ExceptionType);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_FARE_ATTRIBUTE";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_FARE_ATTRIBUTE (" + 
                            "FareId NVARCHAR(255), " + 
                            "Price NVARCHAR(255), " + 
                            "CurrencyType NVARCHAR(255), " + 
                            "PaymentMethod INT, " + 
                            "Transfers INT, " + 
                            "AgencyId NVARCHAR(255), " + 
                            "TransferDuration NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@fareId", !string.IsNullOrEmpty(f.FareId) ? f.FareId : DBNull.Value);
    command.Parameters.AddWithValue("@price", !string.IsNullOrEmpty(f.Price) ? f.Price : DBNull.Value);
    command.Parameters.AddWithValue("@currencyType", !string.IsNullOrEmpty(f.CurrencyType) ? f.CurrencyType : DBNull.Value);
    command.Parameters.AddWithValue("@paymentMethod", f.PaymentMethod);
    command.Parameters.AddWithValue("@transfers", f.Transfers != null ? f.Transfers : DBNull.Value);
    command.Parameters.AddWithValue("@agencyId", !string.IsNullOrEmpty(f.AgencyId) ? f.AgencyId : DBNull.Value);
    command.Parameters.AddWithValue("@transferDuration", !string.IsNullOrEmpty(f.TransferDuration) ? f.TransferDuration : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_FARE_RULE";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_FARE_RULE (" + 
                            "FareId NVARCHAR(255), " + 
                            "RouteId NVARCHAR(255), " + 
                            "OriginId NVARCHAR(255), " + 
                            "DestinationId NVARCHAR(255), " + 
                            "ContainsId NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@fareId", !string.IsNullOrEmpty(f.FareId) ? f.FareId : DBNull.Value);
    command.Parameters.AddWithValue("@routeId", !string.IsNullOrEmpty(f.RouteId) ? f.RouteId : DBNull.Value);
    command.Parameters.AddWithValue("@originId", !string.IsNullOrEmpty(f.OriginId) ? f.OriginId : DBNull.Value);
    command.Parameters.AddWithValue("@destinationId", !string.IsNullOrEmpty(f.DestinationId) ? f.DestinationId : DBNull.Value);
    command.Parameters.AddWithValue("@containsId", !string.IsNullOrEmpty(f.ContainsId) ? f.ContainsId : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_FREQUENCY";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_FREQUENCY (" + 
                            "TripId NVARCHAR(255), " + 
                            "StartTime NVARCHAR(255), " + 
                            "EndTime NVARCHAR(255), " + 
                            "HeadwaySecs NVARCHAR(255), " + 
                            "ExactTimes BIT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@tripId", !string.IsNullOrEmpty(f.TripId) ? f.TripId : DBNull.Value);
    command.Parameters.AddWithValue("@startTime", !string.IsNullOrEmpty(f.StartTime) ? f.StartTime : DBNull.Value);
    command.Parameters.AddWithValue("@endTime", !string.IsNullOrEmpty(f.EndTime) ? f.EndTime : DBNull.Value);
    command.Parameters.AddWithValue("@headwaySecs", !string.IsNullOrEmpty(f.HeadwaySecs) ? f.HeadwaySecs : DBNull.Value);
    command.Parameters.AddWithValue("@exactTimes", f.ExactTimes != null ? f.ExactTimes : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_LEVEL";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_LEVEL (" + 
                            "Id NVARCHAR(255), " + 
                            "Idx FLOAT, " + 
                            "Name NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(l.Id) ? l.Id : DBNull.Value);
    command.Parameters.AddWithValue("@idx", l.Index);
    command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(l.Name) ? l.Name : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_PATHWAY";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_PATHWAY (" + 
                            "Id NVARCHAR(255), " + 
                            "FromStopId NVARCHAR(255), " + 
                            "ToStopId NVARCHAR(255), " + 
                            "PathwayMode INT, " + 
                            "IsBidirectional INT, " + 
                            "Length FLOAT, " + 
                            "TraversalTime INT, " + 
                            "StairCount INT, " + 
                            "MaxSlope FLOAT, " + 
                            "MinWidth FLOAT, " + 
                            "SignpostedAs NVARCHAR(255), " + 
                            "ReversedSignpostedAs NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(p.Id) ? p.Id : DBNull.Value);
    command.Parameters.AddWithValue("@fromStopId", !string.IsNullOrEmpty(p.FromStopId) ? p.FromStopId : DBNull.Value);
    command.Parameters.AddWithValue("@toStopId", !string.IsNullOrEmpty(p.ToStopId) ? p.ToStopId : DBNull.Value);
    command.Parameters.AddWithValue("@pathwayMode", p.PathwayMode);
    command.Parameters.AddWithValue("@isBidirectional", p.IsBidirectional);
    command.Parameters.AddWithValue("@length", p.Length != null ? p.Length : DBNull.Value);
    command.Parameters.AddWithValue("@traversalTime", p.TraversalTime != null ? p.TraversalTime : DBNull.Value);
    command.Parameters.AddWithValue("@stairCount", p.StairCount != null ? p.StairCount : DBNull.Value);
    command.Parameters.AddWithValue("@maxSlope", p.MaxSlope != null ? p.MaxSlope : DBNull.Value);
    command.Parameters.AddWithValue("@minWidth", p.MinWidth != null ? p.MinWidth : DBNull.Value);
    command.Parameters.AddWithValue("@signpostedAs", !string.IsNullOrEmpty(p.SignpostedAs) ? p.SignpostedAs : DBNull.Value);
    command.Parameters.AddWithValue("@reversedSignpostedAs", !string.IsNullOrEmpty(p.ReversedSignpostedAs) ? p.ReversedSignpostedAs : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_ROUTE";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_ROUTE (" + 
                            "Id NVARCHAR(255) PRIMARY KEY, " + 
                            "AgencyId NVARCHAR(255), " + 
                            "ShortName NVARCHAR(255), " + 
                            "LongName NVARCHAR(255), " + 
                            "Description NVARCHAR(255), " + 
                            "Type INT, " + 
                            "Url NVARCHAR(255), " + 
                            "Color INT, " + 
                            "TextColor INT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(r.Id) ? r.Id : DBNull.Value);
    command.Parameters.AddWithValue("@agencyId", !string.IsNullOrEmpty(r.AgencyId) ? r.AgencyId : DBNull.Value);
    command.Parameters.AddWithValue("@shortName", !string.IsNullOrEmpty(r.ShortName) ? r.ShortName : DBNull.Value);
    command.Parameters.AddWithValue("@longName", !string.IsNullOrEmpty(r.LongName) ? r.LongName : DBNull.Value);
    command.Parameters.AddWithValue("@description", !string.IsNullOrEmpty(r.Description) ? r.Description : DBNull.Value);
    command.Parameters.AddWithValue("@type", r.Type);
    command.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(r.Url) ? r.Url : DBNull.Value);
    command.Parameters.AddWithValue("@color", r.Color != null ? r.Color : DBNull.Value);
    command.Parameters.AddWithValue("@textColor", r.TextColor != null ? r.TextColor : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_SHAPE";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_SHAPE (" + 
                            "Id NVARCHAR(255), " + 
                            "Longitude FLOAT, " + 
                            "Latitude FLOAT, " + 
                            "Sequence INT, " + 
                            "DistanceTravelled FLOAT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(s.Id) ? s.Id : DBNull.Value);
    command.Parameters.AddWithValue("@longitude", s.Longitude);
    command.Parameters.AddWithValue("@latitude", s.Latitude);
    command.Parameters.AddWithValue("@sequence", s.Sequence);
    command.Parameters.AddWithValue("@distanceTravelled", s.DistanceTravelled != null ? s.DistanceTravelled : DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_STOP";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_STOP (" + 
                            "Id NVARCHAR(255) PRIMARY KEY, " + 
                            "Code NVARCHAR(255), " + 
                            "Name NVARCHAR(255), " + 
                            "Description NVARCHAR(255), " + 
                            "Longitude FLOAT, " + 
                            "Latitude FLOAT, " + 
                            "Zone NVARCHAR(255), " + 
                            "Url NVARCHAR(255), " + 
                            "LocationType INT, " + 
                            "ParentStation NVARCHAR(255), " + 
                            "Timezone NVARCHAR(255), " + 
                            "WheelchairBoarding NVARCHAR(255), " + 
                            "LevelId NVARCHAR(255), " + 
                            "PlatformCode NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(s.Id) ? s.Id : DBNull.Value);
    command.Parameters.AddWithValue("@code", !string.IsNullOrEmpty(s.Code) ? s.Code : DBNull.Value);
    command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(s.Name) ? s.Name : DBNull.Value);
    command.Parameters.AddWithValue("@description", !string.IsNullOrEmpty(s.Description) ? s.Description : DBNull.Value);
    command.Parameters.AddWithValue("@longitude", s.Longitude);
    command.Parameters.AddWithValue("@latitude", s.Latitude);
    command.Parameters.AddWithValue("@zone", !string.IsNullOrEmpty(s.Zone) ? s.Zone : DBNull.Value);
    command.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(s.Url) ? s.Url : DBNull.Value);
    command.Parameters.AddWithValue("@locationType", s.LocationType != null ? s.LocationType : DBNull.Value);
    command.Parameters.AddWithValue("@parentStation", !string.IsNullOrEmpty(s.ParentStation) ? s.ParentStation : DBNull.Value);
    command.Parameters.AddWithValue("@timezone", !string.IsNullOrEmpty(s.Timezone) ? s.Timezone : DBNull.Value);
    command.Parameters.AddWithValue("@wheelchairBoarding", !string.IsNullOrEmpty(s.WheelchairBoarding) ? s.WheelchairBoarding : DBNull.Value);
    command.Parameters.AddWithValue("@levelId", !string.IsNullOrEmpty(s.LevelId) ? s.LevelId : DBNull.Value);
    command.Parameters.AddWithValue("@platformCode", !string.IsNullOrEmpty(s.PlatformCode) ? s.PlatformCode : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_STOP_TIME";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_STOP_TIME (" + 
                            "TripId NVARCHAR(255), " + 
                            "ArrivalTime NVARCHAR(255), " + 
                            "DepartureTime NVARCHAR(255), " + 
                            "StopId NVARCHAR(255), " + 
                            "StopSequence INT, " + 
                            "StopHeadsign NVARCHAR(255), " + 
                            "PickupType INT, " + 
                            "DropOffType INT, " + 
                            "ShapeDistTravelled FLOAT, " + 
                            "TimepointType INT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@tripId", !string.IsNullOrEmpty(s.TripId) ? s.TripId : DBNull.Value);
    command.Parameters.AddWithValue("@arrivalTime", s.ArrivalTime != null ? s.ArrivalTime.ToString() : DBNull.Value);
    command.Parameters.AddWithValue("@departureTime", s.DepartureTime != null ? s.DepartureTime.ToString() : DBNull.Value);
    command.Parameters.AddWithValue("@stopId", !string.IsNullOrEmpty(s.StopId) ? s.StopId : DBNull.Value);
    command.Parameters.AddWithValue("@stopSequence", s.StopSequence);
    command.Parameters.AddWithValue("@stopHeadsign", !string.IsNullOrEmpty(s.StopHeadsign) ? s.StopHeadsign : DBNull.Value);
    command.Parameters.AddWithValue("@pickupType", s.PickupType != null ? s.PickupType : DBNull.Value);
    command.Parameters.AddWithValue("@dropOffType", s.DropOffType != null ? s.DropOffType : DBNull.Value);
    command.Parameters.AddWithValue("@shapeDistTravelled", s.ShapeDistTravelled != null ? s.ShapeDistTravelled : DBNull.Value);
    command.Parameters.AddWithValue("@timepointType", s.TimepointType);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_TRANSFER";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_TRANSFER (" + 
                            "FromStopId NVARCHAR(255), " + 
                            "ToStopId NVARCHAR(255), " + 
                            "TransferType INT, " + 
                            "MinimumTransferTime NVARCHAR(255)" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@fromStopId", !string.IsNullOrEmpty(t.FromStopId) ? t.FromStopId : DBNull.Value);
    command.Parameters.AddWithValue("@toStopId", !string.IsNullOrEmpty(t.ToStopId) ? t.ToStopId : DBNull.Value);
    command.Parameters.AddWithValue("@transferType", t.TransferType);
    command.Parameters.AddWithValue("@minimumTransferTime", !string.IsNullOrEmpty(t.MinimumTransferTime) ? t.MinimumTransferTime : DBNull.Value);
    
    await command.ExecuteNonQueryAsync();
}

command.Parameters.Clear();

command.CommandText = "DROP TABLE IF EXISTS GTFS_TRIP";
await command.ExecuteNonQueryAsync();

command.CommandText = "CREATE TABLE GTFS_TRIP (" + 
                            "Id NVARCHAR(255) PRIMARY KEY, " + 
                            "RouteId NVARCHAR(255), " + 
                            "ServiceId NVARCHAR(255), " + 
                            "Headsign NVARCHAR(255), " + 
                            "ShortName NVARCHAR(255), " + 
                            "Direction INT, " + 
                            "BlockId NVARCHAR(255), " + 
                            "ShapeId NVARCHAR(255), " + 
                            "AccessibilityType INT" + 
                            ")";

await command.ExecuteNonQueryAsync();

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
    
    command.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(t.Id) ? t.Id : DBNull.Value);
    command.Parameters.AddWithValue("@routeId", !string.IsNullOrEmpty(t.RouteId) ? t.RouteId : DBNull.Value);
    command.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(t.ServiceId) ? t.ServiceId : DBNull.Value);
    command.Parameters.AddWithValue("@headsign", !string.IsNullOrEmpty(t.Headsign) ? t.Headsign : DBNull.Value);
    command.Parameters.AddWithValue("@shortName", !string.IsNullOrEmpty(t.ShortName) ? t.ShortName : DBNull.Value);
    command.Parameters.AddWithValue("@direction", t.Direction != null ? t.Direction : DBNull.Value);
    command.Parameters.AddWithValue("@blockId", !string.IsNullOrEmpty(t.BlockId) ? t.BlockId : DBNull.Value);
    command.Parameters.AddWithValue("@shapeId", !string.IsNullOrEmpty(t.ShapeId) ? t.ShapeId : DBNull.Value);
    command.Parameters.AddWithValue("@accessibilityType", t.AccessibilityType != null ? t.AccessibilityType : DBNull.Value);
    
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