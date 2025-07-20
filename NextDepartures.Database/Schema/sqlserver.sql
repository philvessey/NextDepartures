DROP TABLE IF EXISTS GTFS_AGENCY;

CREATE TABLE GTFS_AGENCY (
    AgencyId VARCHAR(255),
    AgencyName VARCHAR(255) NOT NULL,
    AgencyUrl VARCHAR(255) NOT NULL,
    AgencyTimezone VARCHAR(255) NOT NULL,
    AgencyLang VARCHAR(255),
    AgencyPhone VARCHAR(255),
    AgencyFareUrl VARCHAR(255),
    AgencyEmail VARCHAR(255));

DROP TABLE IF EXISTS GTFS_CALENDAR;

CREATE TABLE GTFS_CALENDAR (
    ServiceId VARCHAR(255) NOT NULL PRIMARY KEY,
    Monday SMALLINT NOT NULL,
    Tuesday SMALLINT NOT NULL,
    Wednesday SMALLINT NOT NULL,
    Thursday SMALLINT NOT NULL,
    Friday SMALLINT NOT NULL,
    Saturday SMALLINT NOT NULL,
    Sunday SMALLINT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL);

DROP TABLE IF EXISTS GTFS_CALENDAR_DATES;

CREATE TABLE GTFS_CALENDAR_DATES (
    ServiceId VARCHAR(255) NOT NULL,
    ExceptionDate DATE NOT NULL,
    ExceptionType SMALLINT NOT NULL);

DROP TABLE IF EXISTS GTFS_FARE_ATTRIBUTES;

CREATE TABLE GTFS_FARE_ATTRIBUTES (
    FareId VARCHAR(255) NOT NULL,
    Price REAL NOT NULL,
    CurrencyType VARCHAR(255) NOT NULL,
    PaymentMethod SMALLINT NOT NULL,
    Transfers VARCHAR(255) NOT NULL,
    AgencyId VARCHAR(255),
    TransferDuration SMALLINT);

DROP TABLE IF EXISTS GTFS_FARE_RULES;

CREATE TABLE GTFS_FARE_RULES (
    FareId VARCHAR(255) NOT NULL,
    RouteId VARCHAR(255),
    OriginId VARCHAR(255),
    DestinationId VARCHAR(255),
    ContainsId VARCHAR(255));

DROP TABLE IF EXISTS GTFS_FREQUENCIES;

CREATE TABLE GTFS_FREQUENCIES (
    TripId VARCHAR(255) NOT NULL,
    StartTime VARCHAR(255) NOT NULL,
    EndTime VARCHAR(255) NOT NULL,
    HeadwaySecs SMALLINT NOT NULL,
    ExactTimes VARCHAR(255));

DROP TABLE IF EXISTS GTFS_LEVELS;

CREATE TABLE GTFS_LEVELS (
    LevelId VARCHAR(255) NOT NULL,
    LevelIndex REAL NOT NULL,
    LevelName VARCHAR(255));

DROP TABLE IF EXISTS GTFS_PATHWAYS;

CREATE TABLE GTFS_PATHWAYS (
    PathwayId VARCHAR(255) NOT NULL,
    FromStopId VARCHAR(255) NOT NULL,
    ToStopId VARCHAR(255) NOT NULL,
    PathwayMode SMALLINT NOT NULL,
    IsBidirectional SMALLINT NOT NULL,
    Length REAL,
    TraversalTime SMALLINT,
    StairCount SMALLINT,
    MaxSlope VARCHAR(255),
    MinWidth REAL,
    SignpostedAs VARCHAR(255),
    ReversedSignpostedAs VARCHAR(255));

DROP TABLE IF EXISTS GTFS_ROUTES;

CREATE TABLE GTFS_ROUTES (
    RouteId VARCHAR(255) NOT NULL PRIMARY KEY,
    AgencyId VARCHAR(255),
    RouteShortName VARCHAR(255),
    RouteLongName VARCHAR(255),
    RouteDesc VARCHAR(255),
    RouteType SMALLINT NOT NULL,
    RouteUrl VARCHAR(255),
    RouteColor VARCHAR(255),
    RouteTextColor VARCHAR(255),
    RouteSortOrder SMALLINT);

DROP TABLE IF EXISTS GTFS_SHAPES;

CREATE TABLE GTFS_SHAPES (
    ShapeId VARCHAR(255) NOT NULL,
    ShapePtLat REAL NOT NULL,
    ShapePtLon REAL NOT NULL,
    ShapePtSequence SMALLINT NOT NULL,
    ShapeDistanceTravelled REAL);

DROP TABLE IF EXISTS GTFS_STOPS;

CREATE TABLE GTFS_STOPS (
    StopId VARCHAR(255) NOT NULL PRIMARY KEY,
    StopCode VARCHAR(255),
    StopName VARCHAR(255),
    StopDesc VARCHAR(255),
    StopLat REAL,
    StopLon REAL,
    ZoneId VARCHAR(255),
    StopUrl VARCHAR(255),
    LocationType VARCHAR(255),
    ParentStation VARCHAR(255),
    StopTimezone VARCHAR(255),
    WheelchairBoarding VARCHAR(255),
    LevelId VARCHAR(255),
    PlatformCode VARCHAR(255));

DROP TABLE IF EXISTS GTFS_STOP_TIMES;

CREATE TABLE GTFS_STOP_TIMES (
    TripId VARCHAR(255) NOT NULL,
    ArrivalTime VARCHAR(255),
    DepartureTime VARCHAR(255),
    StopId VARCHAR(255),
    StopSequence SMALLINT NOT NULL,
    StopHeadsign VARCHAR(255),
    PickupType VARCHAR(255),
    DropOffType VARCHAR(255),
    ShapeDistTravelled REAL,
    Timepoint SMALLINT);

DROP TABLE IF EXISTS GTFS_TRANSFERS;

CREATE TABLE GTFS_TRANSFERS (
    FromStopId VARCHAR(255),
    ToStopId VARCHAR(255),
    TransferType VARCHAR(255) NOT NULL,
    MinTransferTime SMALLINT);

DROP TABLE IF EXISTS GTFS_TRIPS;

CREATE TABLE GTFS_TRIPS (
    RouteId VARCHAR(255) NOT NULL,
    ServiceId VARCHAR(255) NOT NULL,
    TripId VARCHAR(255) NOT NULL PRIMARY KEY,
    TripHeadsign VARCHAR(255),
    TripShortName VARCHAR(255),
    DirectionId SMALLINT,
    BlockId VARCHAR(255),
    ShapeId VARCHAR(255),
    WheelchairAccessible VARCHAR(255),
    BikesAllowed VARCHAR(255));

DROP FUNCTION IF EXISTS GET_FROM_POINT

CREATE FUNCTION GET_FROM_POINT (
    @originLongitude REAL,
    @originLatitude REAL,
    @destinationLongitude REAL,
    @destinationLatitude REAL
) RETURNS REAL AS

BEGIN
    DECLARE @angle REAL;
    DECLARE @deltaLatitude REAL;
    DECLARE @deltaLongitude REAL;
    DECLARE @distance REAL;
    
    DECLARE @a REAL;
    DECLARE @b REAL;
    DECLARE @x REAL;
    DECLARE @y REAL;
    
    SET @deltaLatitude = RADIANS(@destinationLatitude - @originLatitude);
    SET @deltaLongitude = RADIANS(@destinationLongitude - @originLongitude);
    
    SET @a = SIN(@deltaLatitude / 2) *
             SIN(@deltaLatitude / 2);
    SET @b = COS(RADIANS(@originLatitude)) *
             COS(RADIANS(@destinationLatitude)) *
             SIN(@deltaLongitude / 2) *
             SIN(@deltaLongitude / 2);
    
    SET @y = SQRT(@a + @b);
    SET @x = SQRT(1 - (@a + @b));
    
    SET @angle = 2 * ATN2(@y, @x);
    SET @distance = @angle * 6371;
    
    RETURN @distance;
END