using CommandLine;
using CommandLine.Text;
using GTFS;
using Microsoft.Data.SqlClient;
using NextDepartures.Database.Extensions;
using NextDepartures.Database.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Database
{
    internal class Program
    {
        static void Main(string[] args) => new Program().ParseAsync(args).GetAwaiter().GetResult();

        public async Task ParseAsync(string[] args)
        {
            Console.WriteLine("");
            await Parser.Default.ParseArguments<Option>(args).WithParsedAsync(RunAsync);
            Console.WriteLine("");
        }

        private async Task RunAsync(Option option)
        {
            Console.WriteLine(HeadingInfo.Default);
            Console.WriteLine(CopyrightInfo.Default);
            Console.WriteLine("");

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(option.Database);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(string.Format("ERROR: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }

            SqlConnection connection = new SqlConnection(option.Database);

            try
            {
                await connection.OpenAsync();
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine(string.Format("ERROR: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }

            try
            {
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_AGENCY_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_AGENCY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_CALENDAR_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_CALENDAR_DATE_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR_DATE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_FARE_ATTRIBUTE_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_ATTRIBUTE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_FARE_RULE_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_RULE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_FREQUENCY_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FREQUENCY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_LEVEL_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_LEVEL_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_PATHWAY_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_PATHWAY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_ROUTE_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_ROUTE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_SHAPE_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_SHAPE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_STOP_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_STOP_TIME_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP_TIME_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_TRANSFER_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRANSFER_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}_TRIP_PROCEDURE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRIP_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_AGENCY_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_AGENCY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_CALENDAR_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_CALENDAR_DATE_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR_DATE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_FARE_ATTRIBUTE_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_ATTRIBUTE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_FARE_RULE_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_RULE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_FREQUENCY_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FREQUENCY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_LEVEL_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_LEVEL_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_PATHWAY_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_PATHWAY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_ROUTE_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_ROUTE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_SHAPE_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_SHAPE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_STOP_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_STOP_TIME_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP_TIME_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_TRANSFER_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRANSFER_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}_TRIP_TYPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRIP_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_AGENCY", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_AGENCY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_CALENDAR", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_CALENDAR_DATE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_CALENDAR_DATE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_FARE_ATTRIBUTE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_ATTRIBUTE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_FARE_RULE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FARE_RULE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_FREQUENCY", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_FREQUENCY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_LEVEL", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_LEVEL", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_PATHWAY", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_PATHWAY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_ROUTE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_ROUTE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_SHAPE", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_SHAPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_STOP", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_STOP_TIME", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_STOP_TIME", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_TRANSFER", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRANSFER", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}_TRIP", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("DROP: {0}_TRIP", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_AGENCY (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_AGENCY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_CALENDAR (ServiceId nvarchar(255) PRIMARY KEY, Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_CALENDAR_DATE (ServiceId nvarchar(255), Date datetime, ExceptionType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR_DATE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_FARE_ATTRIBUTE (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_ATTRIBUTE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_FARE_RULE (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_RULE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_FREQUENCY (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FREQUENCY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_LEVEL (Id nvarchar(255), Indexes float, Name nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_LEVEL", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_PATHWAY (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_PATHWAY", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_ROUTE (Id nvarchar(255) PRIMARY KEY, AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_ROUTE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_SHAPE (Id nvarchar(255), Longitude float, Latitude float, Sequence int, DistanceTravelled float)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_SHAPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_STOP (Id nvarchar(255) PRIMARY KEY, Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Longitude float, Latitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_STOP_TIME (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_TIME", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_TRANSFER (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRANSFER", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}_TRIP (Id nvarchar(255) PRIMARY KEY, RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRIP", option.Prefix.ToUpper()));
                
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_AGENCY_TYPE AS TABLE (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_AGENCY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_CALENDAR_TYPE AS TABLE (ServiceId nvarchar(255), Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_CALENDAR_DATE_TYPE AS TABLE (ServiceId nvarchar(255), Date datetime, ExceptionType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR_DATE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_FARE_ATTRIBUTE_TYPE AS TABLE (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_ATTRIBUTE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_FARE_RULE_TYPE AS TABLE (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_RULE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_FREQUENCY_TYPE AS TABLE (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FREQUENCY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_LEVEL_TYPE AS TABLE (Id nvarchar(255), Indexes float, Name nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_LEVEL_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_PATHWAY_TYPE AS TABLE (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_PATHWAY_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_ROUTE_TYPE AS TABLE (Id nvarchar(255), AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_ROUTE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_SHAPE_TYPE AS TABLE (Id nvarchar(255), Longitude float, Latitude float, Sequence int, DistanceTravelled float)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_SHAPE_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_STOP_TYPE AS TABLE (Id nvarchar(255), Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Longitude float, Latitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_STOP_TIME_TYPE AS TABLE (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_TIME_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_TRANSFER_TYPE AS TABLE (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRANSFER_TYPE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}_TRIP_TYPE AS TABLE (Id nvarchar(255), RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRIP_TYPE", option.Prefix.ToUpper()));
                
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_AGENCY_PROCEDURE (@table {0}_AGENCY_TYPE READONLY) AS INSERT INTO {0}_AGENCY (Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email) SELECT Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_AGENCY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_CALENDAR_PROCEDURE (@table {0}_CALENDAR_TYPE READONLY) AS INSERT INTO {0}_CALENDAR (ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) SELECT ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_CALENDAR_DATE_PROCEDURE (@table {0}_CALENDAR_DATE_TYPE READONLY) AS INSERT INTO {0}_CALENDAR_DATE (ServiceId, Date, ExceptionType) SELECT ServiceId, Date, ExceptionType FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_CALENDAR_DATE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_FARE_ATTRIBUTE_PROCEDURE (@table {0}_FARE_ATTRIBUTE_TYPE READONLY) AS INSERT INTO {0}_FARE_ATTRIBUTE (FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration) SELECT FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_ATTRIBUTE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_FARE_RULE_PROCEDURE (@table {0}_FARE_RULE_TYPE READONLY) AS INSERT INTO {0}_FARE_RULE (FareId, RouteId, OriginId, DestinationId, ContainsId) SELECT FareId, RouteId, OriginId, DestinationId, ContainsId FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FARE_RULE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_FREQUENCY_PROCEDURE (@table {0}_FREQUENCY_TYPE READONLY) AS INSERT INTO {0}_FREQUENCY (TripId, StartTime, EndTime, HeadwaySecs, ExactTimes) SELECT TripId, StartTime, EndTime, HeadwaySecs, ExactTimes FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_FREQUENCY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_LEVEL_PROCEDURE (@table {0}_LEVEL_TYPE READONLY) AS INSERT INTO {0}_LEVEL (Id, Indexes, Name) SELECT Id, Indexes, Name FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_LEVEL_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_PATHWAY_PROCEDURE (@table {0}_PATHWAY_TYPE READONLY) AS INSERT INTO {0}_PATHWAY (Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs) SELECT Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_PATHWAY_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_ROUTE_PROCEDURE (@table {0}_ROUTE_TYPE READONLY) AS INSERT INTO {0}_ROUTE (Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor) SELECT Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_ROUTE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_SHAPE_PROCEDURE (@table {0}_SHAPE_TYPE READONLY) AS INSERT INTO {0}_SHAPE (Id, Longitude, Latitude, Sequence, DistanceTravelled) SELECT Id, Longitude, Latitude, Sequence, DistanceTravelled FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_SHAPE_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_STOP_PROCEDURE (@table {0}_STOP_TYPE READONLY) AS INSERT INTO {0}_STOP (Id, Code, Name, Description, Longitude, Latitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode) SELECT Id, Code, Name, Description, Longitude, Latitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_STOP_TIME_PROCEDURE (@table {0}_STOP_TIME_TYPE READONLY) AS INSERT INTO {0}_STOP_TIME (TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType) SELECT TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_TIME_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_TRANSFER_PROCEDURE (@table {0}_TRANSFER_TYPE READONLY) AS INSERT INTO {0}_TRANSFER (FromStopId, ToStopId, TransferType, MinimumTransferTime) SELECT FromStopId, ToStopId, TransferType, MinimumTransferTime FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRANSFER_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}_TRIP_PROCEDURE (@table {0}_TRIP_TYPE READONLY) AS INSERT INTO {0}_TRIP (Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType) SELECT Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType FROM @table", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_TRIP_PROCEDURE", option.Prefix.ToUpper()));

                GTFSReader<GTFSFeed> reader = new GTFSReader<GTFSFeed>();
                GTFSFeed feed = reader.Read(option.Gtfs);

                DataTable agency = new DataTable();
                agency.Columns.Add("Id", typeof(string));
                agency.Columns.Add("Name", typeof(string));
                agency.Columns.Add("URL", typeof(string));
                agency.Columns.Add("Timezone", typeof(string));
                agency.Columns.Add("LanguageCode", typeof(string));
                agency.Columns.Add("Phone", typeof(string));
                agency.Columns.Add("FareURL", typeof(string));
                agency.Columns.Add("Email", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_AGENCY_PROCEDURE", option.Prefix.ToUpper()), agency, feed.Agencies, (dt, agency) => dt.Rows.Add(agency.Id, agency.Name, agency.URL, agency.Timezone, agency.LanguageCode, agency.Phone, agency.FareURL, agency.Email));
                Console.WriteLine(string.Format("INSERT: {0}_AGENCY_PROCEDURE", option.Prefix.ToUpper()));

                DataTable calendar = new DataTable();
                calendar.Columns.Add("ServiceId", typeof(string));
                calendar.Columns.Add("Monday", typeof(bool));
                calendar.Columns.Add("Tuesday", typeof(bool));
                calendar.Columns.Add("Wednesday", typeof(bool));
                calendar.Columns.Add("Thursday", typeof(bool));
                calendar.Columns.Add("Friday", typeof(bool));
                calendar.Columns.Add("Saturday", typeof(bool));
                calendar.Columns.Add("Sunday", typeof(bool));
                calendar.Columns.Add("StartDate", typeof(DateTime));
                calendar.Columns.Add("EndDate", typeof(DateTime));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_CALENDAR_PROCEDURE", option.Prefix.ToUpper()), calendar, feed.Calendars.GroupBy(c => c.ServiceId).Select(c => c.First()), (dt, calendar) => dt.Rows.Add(calendar.ServiceId, calendar.Monday, calendar.Tuesday, calendar.Wednesday, calendar.Thursday, calendar.Friday, calendar.Saturday, calendar.Sunday, calendar.StartDate, calendar.EndDate));
                Console.WriteLine(string.Format("INSERT: {0}_CALENDAR_PROCEDURE", option.Prefix.ToUpper()));

                DataTable calendarDate = new DataTable();
                calendarDate.Columns.Add("ServiceId", typeof(string));
                calendarDate.Columns.Add("Date", typeof(DateTime));
                calendarDate.Columns.Add("ExceptionType", typeof(int));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_CALENDAR_DATE_PROCEDURE", option.Prefix.ToUpper()), calendarDate, feed.CalendarDates, (dt, calendarDate) => dt.Rows.Add(calendarDate.ServiceId, calendarDate.Date, calendarDate.ExceptionType));
                Console.WriteLine(string.Format("INSERT: {0}_CALENDAR_DATE_PROCEDURE", option.Prefix.ToUpper()));

                DataTable fareAttribute = new DataTable();
                fareAttribute.Columns.Add("FareId", typeof(string));
                fareAttribute.Columns.Add("Price", typeof(string));
                fareAttribute.Columns.Add("CurrencyType", typeof(string));
                fareAttribute.Columns.Add("PaymentMethod", typeof(int));
                fareAttribute.Columns.Add("Transfers", typeof(int));
                fareAttribute.Columns.Add("AgencyId", typeof(string));
                fareAttribute.Columns.Add("TransferDuration", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_FARE_ATTRIBUTE_PROCEDURE", option.Prefix.ToUpper()), fareAttribute, feed.FareAttributes, (dt, fareAttribute) => dt.Rows.Add(fareAttribute.FareId, fareAttribute.Price, fareAttribute.CurrencyType, fareAttribute.PaymentMethod, fareAttribute.Transfers, fareAttribute.AgencyId, fareAttribute.TransferDuration));
                Console.WriteLine(string.Format("INSERT: {0}_FARE_ATTRIBUTE_PROCEDURE", option.Prefix.ToUpper()));

                DataTable fareRule = new DataTable();
                fareRule.Columns.Add("FareId", typeof(string));
                fareRule.Columns.Add("RouteId", typeof(string));
                fareRule.Columns.Add("OriginId", typeof(string));
                fareRule.Columns.Add("DestinationId", typeof(string));
                fareRule.Columns.Add("ContainsId", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_FARE_RULE_PROCEDURE", option.Prefix.ToUpper()), fareRule, feed.FareRules, (dt, fareRule) => dt.Rows.Add(fareRule.FareId, fareRule.RouteId, fareRule.OriginId, fareRule.DestinationId, fareRule.ContainsId));
                Console.WriteLine(string.Format("INSERT: {0}_FARE_RULE_PROCEDURE", option.Prefix.ToUpper()));

                DataTable frequency = new DataTable();
                frequency.Columns.Add("TripId", typeof(string));
                frequency.Columns.Add("StartTime", typeof(string));
                frequency.Columns.Add("EndTime", typeof(string));
                frequency.Columns.Add("HeadwaySecs", typeof(string));
                frequency.Columns.Add("ExactTimes", typeof(bool));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_FREQUENCY_PROCEDURE", option.Prefix.ToUpper()), frequency, feed.Frequencies, (dt, frequency) => dt.Rows.Add(frequency.TripId, frequency.StartTime, frequency.EndTime, frequency.HeadwaySecs, frequency.ExactTimes));
                Console.WriteLine(string.Format("INSERT: {0}_FREQUENCY_PROCEDURE", option.Prefix.ToUpper()));

                DataTable level = new DataTable();
                level.Columns.Add("Id", typeof(string));
                level.Columns.Add("Indexes", typeof(double));
                level.Columns.Add("Name", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_LEVEL_PROCEDURE", option.Prefix.ToUpper()), level, feed.Levels, (dt, level) => dt.Rows.Add(level.Id, level.Index, level.Name));
                Console.WriteLine(string.Format("INSERT: {0}_LEVEL_PROCEDURE", option.Prefix.ToUpper()));

                DataTable pathway = new DataTable();
                pathway.Columns.Add("Id", typeof(string));
                pathway.Columns.Add("FromStopId", typeof(string));
                pathway.Columns.Add("ToStopId", typeof(string));
                pathway.Columns.Add("PathwayMode", typeof(int));
                pathway.Columns.Add("IsBidirectional", typeof(int));
                pathway.Columns.Add("Length", typeof(double));
                pathway.Columns.Add("TraversalTime", typeof(int));
                pathway.Columns.Add("StairCount", typeof(int));
                pathway.Columns.Add("MaxSlope", typeof(double));
                pathway.Columns.Add("MinWidth", typeof(double));
                pathway.Columns.Add("SignpostedAs", typeof(string));
                pathway.Columns.Add("ReversedSignpostedAs", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_PATHWAY_PROCEDURE", option.Prefix.ToUpper()), pathway, feed.Pathways, (dt, pathway) => dt.Rows.Add(pathway.Id, pathway.FromStopId, pathway.ToStopId, pathway.PathwayMode, pathway.IsBidirectional, pathway.Length, pathway.TraversalTime, pathway.StairCount, pathway.MaxSlope, pathway.MinWidth, pathway.SignpostedAs, pathway.ReversedSignpostedAs));
                Console.WriteLine(string.Format("INSERT: {0}_PATHWAY_PROCEDURE", option.Prefix.ToUpper()));

                DataTable route = new DataTable();
                route.Columns.Add("Id", typeof(string));
                route.Columns.Add("AgencyId", typeof(string));
                route.Columns.Add("ShortName", typeof(string));
                route.Columns.Add("LongName", typeof(string));
                route.Columns.Add("Description", typeof(string));
                route.Columns.Add("Type", typeof(int));
                route.Columns.Add("Url", typeof(string));
                route.Columns.Add("Color", typeof(int));
                route.Columns.Add("TextColor", typeof(int));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_ROUTE_PROCEDURE", option.Prefix.ToUpper()), route, feed.Routes.GroupBy(r => r.Id).Select(r => r.First()), (dt, route) => dt.Rows.Add(route.Id, route.AgencyId, route.ShortName, route.LongName, route.Description, route.Type, route.Url, route.Color, route.TextColor));
                Console.WriteLine(string.Format("INSERT: {0}_ROUTE_PROCEDURE", option.Prefix.ToUpper()));

                DataTable shape = new DataTable();
                shape.Columns.Add("Id", typeof(string));
                shape.Columns.Add("Longitude", typeof(double));
                shape.Columns.Add("Latitude", typeof(double));
                shape.Columns.Add("Sequence", typeof(int));
                shape.Columns.Add("DistanceTravelled", typeof(double));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_SHAPE_PROCEDURE", option.Prefix.ToUpper()), shape, feed.Shapes, (dt, shape) => dt.Rows.Add(shape.Id, shape.Longitude, shape.Latitude, shape.Sequence, shape.DistanceTravelled));
                Console.WriteLine(string.Format("INSERT: {0}_SHAPE_PROCEDURE", option.Prefix.ToUpper()));

                DataTable stop = new DataTable();
                stop.Columns.Add("Id", typeof(string));
                stop.Columns.Add("Code", typeof(string));
                stop.Columns.Add("Name", typeof(string));
                stop.Columns.Add("Description", typeof(string));
                stop.Columns.Add("Longitude", typeof(double));
                stop.Columns.Add("Latitude", typeof(double));
                stop.Columns.Add("Zone", typeof(string));
                stop.Columns.Add("Url", typeof(string));
                stop.Columns.Add("LocationType", typeof(int));
                stop.Columns.Add("ParentStation", typeof(string));
                stop.Columns.Add("Timezone", typeof(string));
                stop.Columns.Add("WheelchairBoarding", typeof(string));
                stop.Columns.Add("LevelId", typeof(string));
                stop.Columns.Add("PlatformCode", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_STOP_PROCEDURE", option.Prefix.ToUpper()), stop, feed.Stops.GroupBy(s => s.Id).Select(s => s.First()), (dt, stop) => dt.Rows.Add(stop.Id, stop.Code, stop.Name, stop.Description, stop.Longitude, stop.Latitude, stop.Zone, stop.Url, stop.LocationType, stop.ParentStation, stop.Timezone, stop.WheelchairBoarding, stop.LevelId, stop.PlatformCode));
                Console.WriteLine(string.Format("INSERT: {0}_STOP_PROCEDURE", option.Prefix.ToUpper()));

                DataTable stopTime = new DataTable();
                stopTime.Columns.Add("TripId", typeof(string));
                stopTime.Columns.Add("ArrivalTime", typeof(string));
                stopTime.Columns.Add("DepartureTime", typeof(string));
                stopTime.Columns.Add("StopId", typeof(string));
                stopTime.Columns.Add("StopSequence", typeof(int));
                stopTime.Columns.Add("StopHeadsign", typeof(string));
                stopTime.Columns.Add("PickupType", typeof(int));
                stopTime.Columns.Add("DropOffType", typeof(int));
                stopTime.Columns.Add("ShapeDistTravelled", typeof(double));
                stopTime.Columns.Add("TimepointType", typeof(int));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_STOP_TIME_PROCEDURE", option.Prefix.ToUpper()), stopTime, feed.StopTimes, (dt, stopTime) => dt.Rows.Add(stopTime.TripId, stopTime.ArrivalTime, stopTime.DepartureTime, stopTime.StopId, stopTime.StopSequence, stopTime.StopHeadsign, stopTime.PickupType, stopTime.DropOffType, stopTime.ShapeDistTravelled, stopTime.TimepointType));
                Console.WriteLine(string.Format("INSERT: {0}_STOP_TIME_PROCEDURE", option.Prefix.ToUpper()));

                DataTable transfer = new DataTable();
                transfer.Columns.Add("FromStopId", typeof(string));
                transfer.Columns.Add("ToStopId", typeof(string));
                transfer.Columns.Add("TransferType", typeof(int));
                transfer.Columns.Add("MinimumTransferTime", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_TRANSFER_PROCEDURE", option.Prefix.ToUpper()), transfer, feed.Transfers, (dt, transfer) => dt.Rows.Add(transfer.FromStopId, transfer.ToStopId, transfer.TransferType, transfer.MinimumTransferTime));
                Console.WriteLine(string.Format("INSERT: {0}_TRANSFER_PROCEDURE", option.Prefix.ToUpper()));

                DataTable trip = new DataTable();
                trip.Columns.Add("Id", typeof(string));
                trip.Columns.Add("RouteId", typeof(string));
                trip.Columns.Add("ServiceId", typeof(string));
                trip.Columns.Add("Headsign", typeof(string));
                trip.Columns.Add("ShortName", typeof(string));
                trip.Columns.Add("Direction", typeof(int));
                trip.Columns.Add("BlockId", typeof(string));
                trip.Columns.Add("ShapeId", typeof(string));
                trip.Columns.Add("AccessibilityType", typeof(int));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}_TRIP_PROCEDURE", option.Prefix.ToUpper()), trip, feed.Trips.GroupBy(t => t.Id).Select(t => t.First()), (dt, trip) => dt.Rows.Add(trip.Id, trip.RouteId, trip.ServiceId, trip.Headsign, trip.ShortName, trip.Direction, trip.BlockId, trip.ShapeId, trip.AccessibilityType));
                Console.WriteLine(string.Format("INSERT: {0}_TRIP_PROCEDURE", option.Prefix.ToUpper()));

                await connection.ExecuteCommandAsync(string.Format("CREATE NONCLUSTERED INDEX {0}_STOP_TIME_INDEX ON {0}_STOP_TIME (TripId, StopId, PickupType) INCLUDE (ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType) WITH (ONLINE = ON)", option.Prefix.ToUpper()));
                Console.WriteLine(string.Format("CREATE: {0}_STOP_TIME_INDEX", option.Prefix.ToUpper()));
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(string.Format("ERROR: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }
        }
    }
}