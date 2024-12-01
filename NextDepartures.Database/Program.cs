using GTFS;
using Microsoft.Data.Sqlite;
using System;
using System.Data;

GTFSReader<GTFSFeed> reader = new();
var feed = reader.Read("Data/feed.zip");

await using SqliteConnection connection = new("Data Source=Data/feed.db;");
connection.Open();

SqliteCommand createAgency = new($"CREATE TABLE GTFS_AGENCY (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertAgency = new($"INSERT INTO GTFS_AGENCY (Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email) VALUES (@id, @name, @url, @timezone, @languageCode, @phone, @fareUrl, @email)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createAgency.ExecuteNonQueryAsync();

foreach (var agency in feed.Agencies)
{
    insertAgency.Parameters.Clear();
    insertAgency.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(agency.Id) ? agency.Id : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(agency.Name) ? agency.Name : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(agency.URL) ? agency.URL : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@timezone", !string.IsNullOrEmpty(agency.Timezone) ? agency.Timezone : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@languageCode", !string.IsNullOrEmpty(agency.LanguageCode) ? agency.LanguageCode : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@phone", !string.IsNullOrEmpty(agency.Phone) ? agency.Phone : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@fareUrl", !string.IsNullOrEmpty(agency.FareURL) ? agency.FareURL : DBNull.Value);
    insertAgency.Parameters.AddWithValue("@email", !string.IsNullOrEmpty(agency.Email) ? agency.Email : DBNull.Value);

    await insertAgency.ExecuteNonQueryAsync();
}

SqliteCommand createCalendar = new($"CREATE TABLE GTFS_CALENDAR (ServiceId nvarchar(255) PRIMARY KEY, Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertCalendar = new($"INSERT INTO GTFS_CALENDAR (ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) VALUES (@serviceId, @monday, @tuesday, @wednesday, @thursday, @friday, @saturday, @sunday, @startDate, @endDate)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createCalendar.ExecuteNonQueryAsync();

foreach (var calendar in feed.Calendars)
{
    insertCalendar.Parameters.Clear();
    insertCalendar.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(calendar.ServiceId) ? calendar.ServiceId : DBNull.Value);
    insertCalendar.Parameters.AddWithValue("@monday", calendar.Monday);
    insertCalendar.Parameters.AddWithValue("@tuesday", calendar.Tuesday);
    insertCalendar.Parameters.AddWithValue("@wednesday", calendar.Wednesday);
    insertCalendar.Parameters.AddWithValue("@thursday", calendar.Thursday);
    insertCalendar.Parameters.AddWithValue("@friday", calendar.Friday);
    insertCalendar.Parameters.AddWithValue("@saturday", calendar.Saturday);
    insertCalendar.Parameters.AddWithValue("@sunday", calendar.Sunday);
    insertCalendar.Parameters.AddWithValue("@startDate", calendar.StartDate);
    insertCalendar.Parameters.AddWithValue("@endDate", calendar.EndDate);

    await insertCalendar.ExecuteNonQueryAsync();
}

SqliteCommand createCalendarDate = new($"CREATE TABLE GTFS_CALENDAR_DATE (ServiceId nvarchar(255), Date datetime, ExceptionType int)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertCalendarDate = new($"INSERT INTO GTFS_CALENDAR_DATE (ServiceId, Date, ExceptionType) VALUES (@serviceId, @date, @exceptionType)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createCalendarDate.ExecuteNonQueryAsync();

foreach (var calendarDate in feed.CalendarDates)
{
    insertCalendarDate.Parameters.Clear();
    insertCalendarDate.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(calendarDate.ServiceId) ? calendarDate.ServiceId : DBNull.Value);
    insertCalendarDate.Parameters.AddWithValue("@date", calendarDate.Date);
    insertCalendarDate.Parameters.AddWithValue("@exceptionType", calendarDate.ExceptionType);

    await insertCalendarDate.ExecuteNonQueryAsync();
}

SqliteCommand createFareAttribute = new($"CREATE TABLE GTFS_FARE_ATTRIBUTE (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertFareAttribute = new($"INSERT INTO GTFS_FARE_ATTRIBUTE (FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration) VALUES (@fareId, @price, @currencyType, @paymentMethod, @transfers, @agencyId, @transferDuration)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createFareAttribute.ExecuteNonQueryAsync();

foreach (var fareAttribute in feed.FareAttributes)
{
    insertFareAttribute.Parameters.Clear();
    insertFareAttribute.Parameters.AddWithValue("@fareId", !string.IsNullOrEmpty(fareAttribute.FareId) ? fareAttribute.FareId : DBNull.Value);
    insertFareAttribute.Parameters.AddWithValue("@price", !string.IsNullOrEmpty(fareAttribute.Price) ? fareAttribute.Price : DBNull.Value);
    insertFareAttribute.Parameters.AddWithValue("@currencyType", !string.IsNullOrEmpty(fareAttribute.CurrencyType) ? fareAttribute.CurrencyType : DBNull.Value);
    insertFareAttribute.Parameters.AddWithValue("@paymentMethod", fareAttribute.PaymentMethod);
    insertFareAttribute.Parameters.AddWithValue("@transfers", fareAttribute.Transfers != null ? fareAttribute.Transfers : DBNull.Value);
    insertFareAttribute.Parameters.AddWithValue("@agencyId", !string.IsNullOrEmpty(fareAttribute.AgencyId) ? fareAttribute.AgencyId : DBNull.Value);
    insertFareAttribute.Parameters.AddWithValue("@transferDuration", !string.IsNullOrEmpty(fareAttribute.TransferDuration) ? fareAttribute.TransferDuration : DBNull.Value);

    await insertFareAttribute.ExecuteNonQueryAsync();
}

SqliteCommand createFareRule = new($"CREATE TABLE GTFS_FARE_RULE (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertFareRule = new($"INSERT INTO GTFS_FARE_RULE (FareId, RouteId, OriginId, DestinationId, ContainsId) VALUES (@fareId, @routeId, @originId, @destinationId, @containsId)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createFareRule.ExecuteNonQueryAsync();

foreach (var fareRule in feed.FareRules)
{
    insertFareRule.Parameters.Clear();
    insertFareRule.Parameters.AddWithValue("@fareId", !string.IsNullOrEmpty(fareRule.FareId) ? fareRule.FareId : DBNull.Value);
    insertFareRule.Parameters.AddWithValue("@routeId", !string.IsNullOrEmpty(fareRule.RouteId) ? fareRule.RouteId : DBNull.Value);
    insertFareRule.Parameters.AddWithValue("@originId", !string.IsNullOrEmpty(fareRule.OriginId) ? fareRule.OriginId : DBNull.Value);
    insertFareRule.Parameters.AddWithValue("@destinationId", !string.IsNullOrEmpty(fareRule.DestinationId) ? fareRule.DestinationId : DBNull.Value);
    insertFareRule.Parameters.AddWithValue("@containsId", !string.IsNullOrEmpty(fareRule.ContainsId) ? fareRule.ContainsId : DBNull.Value);

    await insertFareRule.ExecuteNonQueryAsync();
}

SqliteCommand createFrequency = new($"CREATE TABLE GTFS_FREQUENCY (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertFrequency = new($"INSERT INTO GTFS_FREQUENCY (TripId, StartTime, EndTime, HeadwaySecs, ExactTimes) VALUES (@tripId, @startTime, @endTime, @headwaySecs, @exactTimes)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createFrequency.ExecuteNonQueryAsync();

foreach (var frequency in feed.Frequencies)
{
    insertFrequency.Parameters.Clear();
    insertFrequency.Parameters.AddWithValue("@tripId", !string.IsNullOrEmpty(frequency.TripId) ? frequency.TripId : DBNull.Value);
    insertFrequency.Parameters.AddWithValue("@startTime", !string.IsNullOrEmpty(frequency.StartTime) ? frequency.StartTime : DBNull.Value);
    insertFrequency.Parameters.AddWithValue("@endTime", !string.IsNullOrEmpty(frequency.EndTime) ? frequency.EndTime : DBNull.Value);
    insertFrequency.Parameters.AddWithValue("@headwaySecs", !string.IsNullOrEmpty(frequency.HeadwaySecs) ? frequency.HeadwaySecs : DBNull.Value);
    insertFrequency.Parameters.AddWithValue("@exactTimes", frequency.ExactTimes != null ? frequency.ExactTimes : DBNull.Value);

    await insertFrequency.ExecuteNonQueryAsync();
}

SqliteCommand createLevel = new($"CREATE TABLE GTFS_LEVEL (Id nvarchar(255), Indexes float, Name nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertLevel = new($"INSERT INTO GTFS_LEVEL (Id, Index, Name) VALUES (@id, @index, @name)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createLevel.ExecuteNonQueryAsync();

foreach (var level in feed.Levels)
{
    insertLevel.Parameters.Clear();
    insertLevel.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(level.Id) ? level.Id : DBNull.Value);
    insertLevel.Parameters.AddWithValue("@index", level.Index);
    insertLevel.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(level.Name) ? level.Name : DBNull.Value);

    await insertLevel.ExecuteNonQueryAsync();
}

SqliteCommand createPathway = new($"CREATE TABLE GTFS_PATHWAY (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertPathway = new($"INSERT INTO GTFS_PATHWAY (Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs) VALUES (@id, @fromStopId, @toStopId, @pathwayMode, @isBidirectional, @length, @traversalTime, @stairCount, @maxSlope, @minWidth, @signpostedAs, @reversedSignpostedAs)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createPathway.ExecuteNonQueryAsync();

foreach (var pathway in feed.Pathways)
{
    insertPathway.Parameters.Clear();
    insertPathway.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(pathway.Id) ? pathway.Id : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@fromStopId", !string.IsNullOrEmpty(pathway.FromStopId) ? pathway.FromStopId : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@toStopId", !string.IsNullOrEmpty(pathway.ToStopId) ? pathway.ToStopId : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@pathwayMode", pathway.PathwayMode);
    insertPathway.Parameters.AddWithValue("@isBidirectional", pathway.IsBidirectional);
    insertPathway.Parameters.AddWithValue("@length", pathway.Length != null ? pathway.Length : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@traversalTime", pathway.TraversalTime != null ? pathway.TraversalTime : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@stairCount", pathway.StairCount != null ? pathway.StairCount : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@maxSlope", pathway.MaxSlope != null ? pathway.MaxSlope : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@minWidth", pathway.MinWidth != null ? pathway.MinWidth : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@signpostedAs", !string.IsNullOrEmpty(pathway.SignpostedAs) ? pathway.SignpostedAs : DBNull.Value);
    insertPathway.Parameters.AddWithValue("@reversedSignpostedAs", !string.IsNullOrEmpty(pathway.ReversedSignpostedAs) ? pathway.ReversedSignpostedAs : DBNull.Value);
}

SqliteCommand createRoute = new($"CREATE TABLE GTFS_ROUTE (Id nvarchar(255) PRIMARY KEY, AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertRoute = new($"INSERT INTO GTFS_ROUTE (Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor) VALUES (@id, @agencyId, @shortName, @longName, @description, @type, @url, @color, @textColor)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createRoute.ExecuteNonQueryAsync();

foreach (var route in feed.Routes)
{
    insertRoute.Parameters.Clear();
    insertRoute.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(route.Id) ? route.Id : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@agencyId", !string.IsNullOrEmpty(route.AgencyId) ? route.AgencyId : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@shortName", !string.IsNullOrEmpty(route.ShortName) ? route.ShortName : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@longName", !string.IsNullOrEmpty(route.LongName) ? route.LongName : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@description", !string.IsNullOrEmpty(route.Description) ? route.Description : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@type", route.Type);
    insertRoute.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(route.Url) ? route.Url : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@color", route.Color != null ? route.Color : DBNull.Value);
    insertRoute.Parameters.AddWithValue("@textColor", route.TextColor != null ? route.TextColor : DBNull.Value);

    await insertRoute.ExecuteNonQueryAsync();
}

SqliteCommand createShape = new($"CREATE TABLE GTFS_SHAPE (Id nvarchar(255), Longitude float, Latitude float, Sequence int, DistanceTravelled float)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertShape = new($"INSERT INTO GTFS_SHAPE (Id, Longitude, Latitude, Sequence, DistanceTravelled) VALUES (@id, @longitude, @latitude, @sequence, @distanceTravelled)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createShape.ExecuteNonQueryAsync();

foreach (var shape in feed.Shapes)
{
    insertShape.Parameters.Clear();
    insertShape.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(shape.Id) ? shape.Id : DBNull.Value);
    insertShape.Parameters.AddWithValue("@longitude", shape.Longitude);
    insertShape.Parameters.AddWithValue("@latitude", shape.Latitude);
    insertShape.Parameters.AddWithValue("@sequence", shape.Sequence);
    insertShape.Parameters.AddWithValue("@distanceTravelled", shape.DistanceTravelled != null ? shape.DistanceTravelled : DBNull.Value);

    await insertShape.ExecuteNonQueryAsync();
}

SqliteCommand createStop = new($"CREATE TABLE GTFS_STOP (Id nvarchar(255) PRIMARY KEY, Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Longitude float, Latitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertStop = new($"INSERT INTO GTFS_STOP (Id, Code, Name, Description, Longitude, Latitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode) VALUES (@id, @code, @name, @description, @longitude, @latitude, @zone, @url, @locationType, @parentStation, @timezone, @wheelchairBoarding, @levelId, @platformCode)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createStop.ExecuteNonQueryAsync();

foreach (var stop in feed.Stops)
{
    insertStop.Parameters.Clear();
    insertStop.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(stop.Id) ? stop.Id : DBNull.Value);
    insertStop.Parameters.AddWithValue("@code", !string.IsNullOrEmpty(stop.Code) ? stop.Code : DBNull.Value);
    insertStop.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(stop.Name) ? stop.Name : DBNull.Value);
    insertStop.Parameters.AddWithValue("@description", !string.IsNullOrEmpty(stop.Description) ? stop.Description : DBNull.Value);
    insertStop.Parameters.AddWithValue("@longitude", stop.Longitude);
    insertStop.Parameters.AddWithValue("@latitude", stop.Latitude);
    insertStop.Parameters.AddWithValue("@zone", !string.IsNullOrEmpty(stop.Zone) ? stop.Zone : DBNull.Value);
    insertStop.Parameters.AddWithValue("@url", !string.IsNullOrEmpty(stop.Url) ? stop.Url : DBNull.Value);
    insertStop.Parameters.AddWithValue("@locationType", stop.LocationType != null ? stop.LocationType : DBNull.Value);
    insertStop.Parameters.AddWithValue("@parentStation", !string.IsNullOrEmpty(stop.ParentStation) ? stop.ParentStation : DBNull.Value);
    insertStop.Parameters.AddWithValue("@timezone", !string.IsNullOrEmpty(stop.Timezone) ? stop.Timezone : DBNull.Value);
    insertStop.Parameters.AddWithValue("@wheelchairBoarding", !string.IsNullOrEmpty(stop.WheelchairBoarding) ? stop.WheelchairBoarding : DBNull.Value);
    insertStop.Parameters.AddWithValue("@levelId", !string.IsNullOrEmpty(stop.LevelId) ? stop.LevelId : DBNull.Value);
    insertStop.Parameters.AddWithValue("@platformCode", !string.IsNullOrEmpty(stop.PlatformCode) ? stop.PlatformCode : DBNull.Value);
    
    await insertStop.ExecuteNonQueryAsync();
}

SqliteCommand createStopTime = new($"CREATE TABLE GTFS_STOP_TIME (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertStopTime = new($"INSERT INTO GTFS_STOP_TIME (TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType) VALUES (@tripId, @arrivalTime, @departureTime, @stopId, @stopSequence, @stopHeadsign, @pickupType, @dropOffType, @shapeDistTravelled, @timepointType)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createStopTime.ExecuteNonQueryAsync();

foreach (var stopTime in feed.StopTimes)
{
    insertStopTime.Parameters.Clear();
    insertStopTime.Parameters.AddWithValue("@tripId", !string.IsNullOrEmpty(stopTime.TripId) ? stopTime.TripId : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@arrivalTime", stopTime.ArrivalTime != null ? stopTime.ArrivalTime.ToString() : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@departureTime", stopTime.DepartureTime != null ? stopTime.DepartureTime.ToString() : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@stopId", !string.IsNullOrEmpty(stopTime.StopId) ? stopTime.StopId : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@stopSequence", stopTime.StopSequence);
    insertStopTime.Parameters.AddWithValue("@stopHeadsign", !string.IsNullOrEmpty(stopTime.StopHeadsign) ? stopTime.StopHeadsign : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@pickupType", stopTime.PickupType != null ? stopTime.PickupType : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@dropOffType", stopTime.DropOffType != null ? stopTime.DropOffType : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@shapeDistTravelled", stopTime.ShapeDistTravelled != null ? stopTime.ShapeDistTravelled : DBNull.Value);
    insertStopTime.Parameters.AddWithValue("@timepointType", stopTime.TimepointType);
    
    await insertStopTime.ExecuteNonQueryAsync();
}

SqliteCommand createTransfer = new($"CREATE TABLE GTFS_TRANSFER (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertTransfer = new($"INSERT INTO GTFS_TRANSFER (FromStopId, ToStopId, TransferType, MinimumTransferTime) VALUES (@fromStopId, @toStopId, @transferType, @minimumTransferTime)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createTransfer.ExecuteNonQueryAsync();

foreach (var transfer in feed.Transfers)
{
    insertTransfer.Parameters.Clear();
    insertTransfer.Parameters.AddWithValue("@fromStopId", !string.IsNullOrEmpty(transfer.FromStopId) ? transfer.FromStopId : DBNull.Value);
    insertTransfer.Parameters.AddWithValue("@toStopId", !string.IsNullOrEmpty(transfer.ToStopId) ? transfer.ToStopId : DBNull.Value);
    insertTransfer.Parameters.AddWithValue("@transferType", transfer.TransferType);
    insertTransfer.Parameters.AddWithValue("@minimumTransferTime", !string.IsNullOrEmpty(transfer.MinimumTransferTime) ? transfer.MinimumTransferTime : DBNull.Value);
    
    await insertTransfer.ExecuteNonQueryAsync();
}

SqliteCommand createTrip = new($"CREATE TABLE GTFS_TRIP (Id nvarchar(255) PRIMARY KEY, RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

SqliteCommand insertTrip = new($"INSERT INTO GTFS_TRIP (Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType) VALUES (@id, @routeId, @serviceId, @headsign, @shortName, @direction, @blockId, @shapeId, @accessibilityType)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await createTrip.ExecuteNonQueryAsync();

foreach (var trip in feed.Trips)
{
    insertTrip.Parameters.Clear();
    insertTrip.Parameters.AddWithValue("@id", !string.IsNullOrEmpty(trip.Id) ? trip.Id : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@routeId", !string.IsNullOrEmpty(trip.RouteId) ? trip.RouteId : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@serviceId", !string.IsNullOrEmpty(trip.ServiceId) ? trip.ServiceId : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@headsign", !string.IsNullOrEmpty(trip.Headsign) ? trip.Headsign : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@shortName", !string.IsNullOrEmpty(trip.ShortName) ? trip.ShortName : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@direction", trip.Direction != null ? trip.Direction : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@blockId", !string.IsNullOrEmpty(trip.BlockId) ? trip.BlockId : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@shapeId", !string.IsNullOrEmpty(trip.ShapeId) ? trip.ShapeId : DBNull.Value);
    insertTrip.Parameters.AddWithValue("@accessibilityType", trip.AccessibilityType != null ? trip.AccessibilityType : DBNull.Value);
    
    await insertTrip.ExecuteNonQueryAsync();
}

SqliteCommand indexStopTime = new($"CREATE INDEX GTFS_STOP_TIME_INDEX ON GTFS_STOP_TIME (TripId, StopId, PickupType, ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType)", connection)
{
    CommandTimeout = 0,
    CommandType = CommandType.Text
};

await indexStopTime.ExecuteNonQueryAsync();