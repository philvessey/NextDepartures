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
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.AgencyProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.CalendarDateProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.CalendarProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.FareAttributeProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.FareRuleProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.FrequencyProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.LevelProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.PathwayProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.RouteProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.ShapeProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.StopProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.StopTimeProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.TransferProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP PROCEDURE IF EXISTS {0}.TripProcedure", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.AgencyType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.CalendarType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.CalendarDateType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.FareAttributeType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.FareRuleType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.FrequencyType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.LevelType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.PathwayType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.RouteType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.ShapeType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.StopType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.StopTimeType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.TransferType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TYPE IF EXISTS {0}.TripType", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Agency", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Calendar", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.CalendarDate", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.FareAttribute", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.FareRule", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Frequency", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Level", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Pathway", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Route", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Shape", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Stop", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.StopTime", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Transfer", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("DROP TABLE IF EXISTS {0}.Trip", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Agency (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Calendar (ServiceId nvarchar(255) PRIMARY KEY, Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.CalendarDate (ServiceId nvarchar(255), Date datetime, ExceptionType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.FareAttribute (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.FareRule (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Frequency (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Level (Id nvarchar(255), Indexes float, Name nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Pathway (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Route (Id nvarchar(255) PRIMARY KEY, AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Shape (Id nvarchar(255), Latitude float, Longitude float, Sequence int, DistanceTravelled float)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Stop (Id nvarchar(255) PRIMARY KEY, Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Latitude float, Longitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.StopTime (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Transfer (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TABLE {0}.Trip (Id nvarchar(255) PRIMARY KEY, RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.AgencyType AS TABLE (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.CalendarType AS TABLE (ServiceId nvarchar(255), Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.CalendarDateType AS TABLE (ServiceId nvarchar(255), Date datetime, ExceptionType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.FareAttributeType AS TABLE (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.FareRuleType AS TABLE (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.FrequencyType AS TABLE (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.LevelType AS TABLE (Id nvarchar(255), Indexes float, Name nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.PathwayType AS TABLE (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.RouteType AS TABLE (Id nvarchar(255), AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.ShapeType AS TABLE (Id nvarchar(255), Latitude float, Longitude float, Sequence int, DistanceTravelled float)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.StopType AS TABLE (Id nvarchar(255), Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Latitude float, Longitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.StopTimeType AS TABLE (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.TransferType AS TABLE (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE TYPE {0}.TripType AS TABLE (Id nvarchar(255), RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.AgencyProcedure (@table {0}.AgencyType READONLY) AS INSERT INTO {0}.Agency (Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email) SELECT Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.CalendarProcedure (@table {0}.CalendarType READONLY) AS INSERT INTO {0}.Calendar (ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) SELECT ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.CalendarDateProcedure (@table {0}.CalendarDateType READONLY) AS INSERT INTO {0}.CalendarDate (ServiceId, Date, ExceptionType) SELECT ServiceId, Date, ExceptionType FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.FareAttributeProcedure (@table {0}.FareAttributeType READONLY) AS INSERT INTO {0}.FareAttribute (FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration) SELECT FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.FareRuleProcedure (@table {0}.FareRuleType READONLY) AS INSERT INTO {0}.FareRule (FareId, RouteId, OriginId, DestinationId, ContainsId) SELECT FareId, RouteId, OriginId, DestinationId, ContainsId FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.FrequencyProcedure (@table {0}.FrequencyType READONLY) AS INSERT INTO {0}.Frequency (TripId, StartTime, EndTime, HeadwaySecs, ExactTimes) SELECT TripId, StartTime, EndTime, HeadwaySecs, ExactTimes FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.LevelProcedure (@table {0}.LevelType READONLY) AS INSERT INTO {0}.Level (Id, Indexes, Name) SELECT Id, Indexes, Name FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.PathwayProcedure (@table {0}.PathwayType READONLY) AS INSERT INTO {0}.Pathway (Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs) SELECT Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.RouteProcedure (@table {0}.RouteType READONLY) AS INSERT INTO {0}.Route (Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor) SELECT Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.ShapeProcedure (@table {0}.ShapeType READONLY) AS INSERT INTO {0}.Shape (Id, Latitude, Longitude, Sequence, DistanceTravelled) SELECT Id, Latitude, Longitude, Sequence, DistanceTravelled FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.StopProcedure (@table {0}.StopType READONLY) AS INSERT INTO {0}.Stop (Id, Code, Name, Description, Latitude, Longitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode) SELECT Id, Code, Name, Description, Latitude, Longitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.StopTimeProcedure (@table {0}.StopTimeType READONLY) AS INSERT INTO {0}.StopTime (TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType) SELECT TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.TransferProcedure (@table {0}.TransferType READONLY) AS INSERT INTO {0}.Transfer (FromStopId, ToStopId, TransferType, MinimumTransferTime) SELECT FromStopId, ToStopId, TransferType, MinimumTransferTime FROM @table", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE PROCEDURE {0}.TripProcedure (@table {0}.TripType READONLY) AS INSERT INTO {0}.Trip (Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType) SELECT Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType FROM @table", option.Prefix.ToUpper()));

                Console.WriteLine("CREATE: tables");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.AgencyProcedure", option.Prefix.ToUpper()), agency, feed.Agencies, (dt, agency) => dt.Rows.Add(agency.Id, agency.Name, agency.URL, agency.Timezone, agency.LanguageCode, agency.Phone, agency.FareURL, agency.Email));

                Console.WriteLine("INSERT: agency.txt");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.CalendarProcedure", option.Prefix.ToUpper()), calendar, feed.Calendars.GroupBy(c => c.ServiceId).Select(c => c.First()), (dt, calendar) => dt.Rows.Add(calendar.ServiceId, calendar.Monday, calendar.Tuesday, calendar.Wednesday, calendar.Thursday, calendar.Friday, calendar.Saturday, calendar.Sunday, calendar.StartDate, calendar.EndDate));

                Console.WriteLine("INSERT: calendar.txt");

                DataTable calendarDate = new DataTable();
                calendarDate.Columns.Add("ServiceId", typeof(string));
                calendarDate.Columns.Add("Date", typeof(DateTime));
                calendarDate.Columns.Add("ExceptionType", typeof(int));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.CalendarDateProcedure", option.Prefix.ToUpper()), calendarDate, feed.CalendarDates, (dt, calendarDate) => dt.Rows.Add(calendarDate.ServiceId, calendarDate.Date, calendarDate.ExceptionType));

                Console.WriteLine("INSERT: calendar_dates.txt");

                DataTable fareAttribute = new DataTable();
                fareAttribute.Columns.Add("FareId", typeof(string));
                fareAttribute.Columns.Add("Price", typeof(string));
                fareAttribute.Columns.Add("CurrencyType", typeof(string));
                fareAttribute.Columns.Add("PaymentMethod", typeof(int));
                fareAttribute.Columns.Add("Transfers", typeof(int));
                fareAttribute.Columns.Add("AgencyId", typeof(string));
                fareAttribute.Columns.Add("TransferDuration", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.FareAttributeProcedure", option.Prefix.ToUpper()), fareAttribute, feed.FareAttributes, (dt, fareAttribute) => dt.Rows.Add(fareAttribute.FareId, fareAttribute.Price, fareAttribute.CurrencyType, fareAttribute.PaymentMethod, fareAttribute.Transfers, fareAttribute.AgencyId, fareAttribute.TransferDuration));

                Console.WriteLine("INSERT: fare_attributes.txt");

                DataTable fareRule = new DataTable();
                fareRule.Columns.Add("FareId", typeof(string));
                fareRule.Columns.Add("RouteId", typeof(string));
                fareRule.Columns.Add("OriginId", typeof(string));
                fareRule.Columns.Add("DestinationId", typeof(string));
                fareRule.Columns.Add("ContainsId", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.FareRuleProcedure", option.Prefix.ToUpper()), fareRule, feed.FareRules, (dt, fareRule) => dt.Rows.Add(fareRule.FareId, fareRule.RouteId, fareRule.OriginId, fareRule.DestinationId, fareRule.ContainsId));

                Console.WriteLine("INSERT: fare_rules.txt");

                DataTable frequency = new DataTable();
                frequency.Columns.Add("TripId", typeof(string));
                frequency.Columns.Add("StartTime", typeof(string));
                frequency.Columns.Add("EndTime", typeof(string));
                frequency.Columns.Add("HeadwaySecs", typeof(string));
                frequency.Columns.Add("ExactTimes", typeof(bool));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.FrequencyProcedure", option.Prefix.ToUpper()), frequency, feed.Frequencies, (dt, frequency) => dt.Rows.Add(frequency.TripId, frequency.StartTime, frequency.EndTime, frequency.HeadwaySecs, frequency.ExactTimes));

                Console.WriteLine("INSERT: frequencies.txt");

                DataTable level = new DataTable();
                level.Columns.Add("Id", typeof(string));
                level.Columns.Add("Indexes", typeof(double));
                level.Columns.Add("Name", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.LevelProcedure", option.Prefix.ToUpper()), level, feed.Levels, (dt, level) => dt.Rows.Add(level.Id, level.Index, level.Name));

                Console.WriteLine("INSERT: levels.txt");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.PathwayProcedure", option.Prefix.ToUpper()), pathway, feed.Pathways, (dt, pathway) => dt.Rows.Add(pathway.Id, pathway.FromStopId, pathway.ToStopId, pathway.PathwayMode, pathway.IsBidirectional, pathway.Length, pathway.TraversalTime, pathway.StairCount, pathway.MaxSlope, pathway.MinWidth, pathway.SignpostedAs, pathway.ReversedSignpostedAs));

                Console.WriteLine("INSERT: pathways.txt");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.RouteProcedure", option.Prefix.ToUpper()), route, feed.Routes.GroupBy(r => r.Id).Select(r => r.First()), (dt, route) => dt.Rows.Add(route.Id, route.AgencyId, route.ShortName, route.LongName, route.Description, route.Type, route.Url, route.Color, route.TextColor));

                Console.WriteLine("INSERT: routes.txt");

                DataTable shape = new DataTable();
                shape.Columns.Add("Id", typeof(string));
                shape.Columns.Add("Latitude", typeof(double));
                shape.Columns.Add("Longitude", typeof(double));
                shape.Columns.Add("Sequence", typeof(int));
                shape.Columns.Add("DistanceTravelled", typeof(double));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.ShapeProcedure", option.Prefix.ToUpper()), shape, feed.Shapes, (dt, shape) => dt.Rows.Add(shape.Id, shape.Latitude, shape.Longitude, shape.Sequence, shape.DistanceTravelled));

                Console.WriteLine("INSERT: shapes.txt");

                DataTable stop = new DataTable();
                stop.Columns.Add("Id", typeof(string));
                stop.Columns.Add("Code", typeof(string));
                stop.Columns.Add("Name", typeof(string));
                stop.Columns.Add("Description", typeof(string));
                stop.Columns.Add("Latitude", typeof(double));
                stop.Columns.Add("Longitude", typeof(double));
                stop.Columns.Add("Zone", typeof(string));
                stop.Columns.Add("Url", typeof(string));
                stop.Columns.Add("LocationType", typeof(int));
                stop.Columns.Add("ParentStation", typeof(string));
                stop.Columns.Add("Timezone", typeof(string));
                stop.Columns.Add("WheelchairBoarding", typeof(string));
                stop.Columns.Add("LevelId", typeof(string));
                stop.Columns.Add("PlatformCode", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.StopProcedure", option.Prefix.ToUpper()), stop, feed.Stops.GroupBy(s => s.Id).Select(s => s.First()), (dt, stop) => dt.Rows.Add(stop.Id, stop.Code, stop.Name, stop.Description, stop.Latitude, stop.Longitude, stop.Zone, stop.Url, stop.LocationType, stop.ParentStation, stop.Timezone, stop.WheelchairBoarding, stop.LevelId, stop.PlatformCode));

                Console.WriteLine("INSERT: stops.txt");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.StopTimeProcedure", option.Prefix.ToUpper()), stopTime, feed.StopTimes, (dt, stopTime) => dt.Rows.Add(stopTime.TripId, stopTime.ArrivalTime, stopTime.DepartureTime, stopTime.StopId, stopTime.StopSequence, stopTime.StopHeadsign, stopTime.PickupType, stopTime.DropOffType, stopTime.ShapeDistTravelled, stopTime.TimepointType));

                Console.WriteLine("INSERT: stop_times.txt");

                DataTable transfer = new DataTable();
                transfer.Columns.Add("FromStopId", typeof(string));
                transfer.Columns.Add("ToStopId", typeof(string));
                transfer.Columns.Add("TransferType", typeof(int));
                transfer.Columns.Add("MinimumTransferTime", typeof(string));

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.TransferProcedure", option.Prefix.ToUpper()), transfer, feed.Transfers, (dt, transfer) => dt.Rows.Add(transfer.FromStopId, transfer.ToStopId, transfer.TransferType, transfer.MinimumTransferTime));

                Console.WriteLine("INSERT: transfers.txt");

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

                await connection.ExecuteStoredProcedureFromTableInBatchesAsync(string.Format("{0}.TripProcedure", option.Prefix.ToUpper()), trip, feed.Trips.GroupBy(t => t.Id).Select(t => t.First()), (dt, trip) => dt.Rows.Add(trip.Id, trip.RouteId, trip.ServiceId, trip.Headsign, trip.ShortName, trip.Direction, trip.BlockId, trip.ShapeId, trip.AccessibilityType));

                Console.WriteLine("INSERT: trips.txt");

                await connection.ExecuteCommandAsync(string.Format("CREATE NONCLUSTERED INDEX {0}.StopTimeIndexStop ON {0}.StopTime (StopId, PickupType) INCLUDE (TripId, ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType) WITH (ONLINE = ON)", option.Prefix.ToUpper()));
                await connection.ExecuteCommandAsync(string.Format("CREATE NONCLUSTERED INDEX {0}.StopTimeIndexTrip ON {0}.StopTime (TripId, PickupType) INCLUDE (ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType) WITH (ONLINE = ON)", option.Prefix.ToUpper()));

                Console.WriteLine("CREATE: indexes");
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