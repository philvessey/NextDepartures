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
                Console.Error.WriteLine(string.Format("Connection string invalid. Error: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }

            SqlConnection connection = new SqlConnection(option.Database);

            try
            {
                connection.Open();
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine(string.Format("Could not connect to database. Error: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }

            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS AgencyProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarDateProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FareAttributeProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FareRuleProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FrequencyProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS LevelProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS PathwayProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS RouteProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS ShapeProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS StopProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS StopTimeProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS TransferProcedure");
            await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS TripProcedure");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS AgencyType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS CalendarType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS CalendarDateType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS FareAttributeType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS FareRuleType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS FrequencyType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS LevelType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS PathwayType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS RouteType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS ShapeType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS StopType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS StopTimeType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS TransferType");
            await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS TripType");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Agency");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Calendar");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS CalendarDate");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS FareAttribute");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS FareRule");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Frequency");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Level");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Pathway");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Route");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Shape");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Stop");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS StopTime");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Transfer");
            await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Trip");
            await connection.ExecuteCommandAsync("CREATE TABLE Agency (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE Calendar (ServiceId nvarchar(255) PRIMARY KEY, Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)");
            await connection.ExecuteCommandAsync("CREATE TABLE CalendarDate (ServiceId nvarchar(255), Date datetime, ExceptionType int)");
            await connection.ExecuteCommandAsync("CREATE TABLE FareAttribute (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE FareRule (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE Frequency (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)");
            await connection.ExecuteCommandAsync("CREATE TABLE Level (Id nvarchar(255), Indexes float, Name nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE Pathway (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE Route (Id nvarchar(255) PRIMARY KEY, AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)");
            await connection.ExecuteCommandAsync("CREATE TABLE Shape (Id nvarchar(255), Latitude float, Longitude float, Sequence int, DistanceTravelled float)");
            await connection.ExecuteCommandAsync("CREATE TABLE Stop (Id nvarchar(255) PRIMARY KEY, Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Latitude float, Longitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE StopTime (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)");
            await connection.ExecuteCommandAsync("CREATE TABLE Transfer (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TABLE Trip (Id nvarchar(255) PRIMARY KEY, RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)");
            await connection.ExecuteCommandAsync("CREATE TYPE AgencyType AS TABLE (Id nvarchar(255), Name nvarchar(255), URL nvarchar(255), Timezone nvarchar(255), LanguageCode nvarchar(255), Phone nvarchar(255), FareURL nvarchar(255), Email nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE CalendarType AS TABLE (ServiceId nvarchar(255), Monday bit, Tuesday bit, Wednesday bit, Thursday bit, Friday bit, Saturday bit, Sunday bit, StartDate datetime, EndDate datetime)");
            await connection.ExecuteCommandAsync("CREATE TYPE CalendarDateType AS TABLE (ServiceId nvarchar(255), Date datetime, ExceptionType int)");
            await connection.ExecuteCommandAsync("CREATE TYPE FareAttributeType AS TABLE (FareId nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod int, Transfers int, AgencyId nvarchar(255), TransferDuration nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE FareRuleType AS TABLE (FareId nvarchar(255), RouteId nvarchar(255), OriginId nvarchar(255), DestinationId nvarchar(255), ContainsId nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE FrequencyType AS TABLE (TripId nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes bit)");
            await connection.ExecuteCommandAsync("CREATE TYPE LevelType AS TABLE (Id nvarchar(255), Indexes float, Name nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE PathwayType AS TABLE (Id nvarchar(255), FromStopId nvarchar(255), ToStopId nvarchar(255), PathwayMode int, IsBidirectional int, Length float, TraversalTime int, StairCount int, MaxSlope float, MinWidth float, SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE RouteType AS TABLE (Id nvarchar(255), AgencyId nvarchar(255), ShortName nvarchar(255), LongName nvarchar(255), Description nvarchar(255), Type int, Url nvarchar(255), Color int, TextColor int)");
            await connection.ExecuteCommandAsync("CREATE TYPE ShapeType AS TABLE (Id nvarchar(255), Latitude float, Longitude float, Sequence int, DistanceTravelled float)");
            await connection.ExecuteCommandAsync("CREATE TYPE StopType AS TABLE (Id nvarchar(255), Code nvarchar(255), Name nvarchar(255), Description nvarchar(255), Latitude float, Longitude float, Zone nvarchar(255), Url nvarchar(255), LocationType int, ParentStation nvarchar(255), Timezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelId nvarchar(255), PlatformCode nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE StopTimeType AS TABLE (TripId nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopId nvarchar(255), StopSequence int, StopHeadsign nvarchar(255), PickupType int, DropOffType int, ShapeDistTravelled float, TimepointType int)");
            await connection.ExecuteCommandAsync("CREATE TYPE TransferType AS TABLE (FromStopId nvarchar(255), ToStopId nvarchar(255), TransferType int, MinimumTransferTime nvarchar(255))");
            await connection.ExecuteCommandAsync("CREATE TYPE TripType AS TABLE (Id nvarchar(255), RouteId nvarchar(255), ServiceId nvarchar(255), Headsign nvarchar(255), ShortName nvarchar(255), Direction int, BlockId nvarchar(255), ShapeId nvarchar(255), AccessibilityType int)");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE AgencyProcedure (@table AgencyType READONLY) AS INSERT INTO Agency (Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email) SELECT Id, Name, URL, Timezone, LanguageCode, Phone, FareURL, Email FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarProcedure (@table CalendarType READONLY) AS INSERT INTO Calendar (ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) SELECT ServiceId, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarDateProcedure (@table CalendarDateType READONLY) AS INSERT INTO CalendarDate (ServiceId, Date, ExceptionType) SELECT ServiceId, Date, ExceptionType FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE FareAttributeProcedure (@table FareAttributeType READONLY) AS INSERT INTO FareAttribute (FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration) SELECT FareId, Price, CurrencyType, PaymentMethod, Transfers, AgencyId, TransferDuration FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE FareRuleProcedure (@table FareRuleType READONLY) AS INSERT INTO FareRule (FareId, RouteId, OriginId, DestinationId, ContainsId) SELECT FareId, RouteId, OriginId, DestinationId, ContainsId FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE FrequencyProcedure (@table FrequencyType READONLY) AS INSERT INTO Frequency (TripId, StartTime, EndTime, HeadwaySecs, ExactTimes) SELECT TripId, StartTime, EndTime, HeadwaySecs, ExactTimes FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE LevelProcedure (@table LevelType READONLY) AS INSERT INTO Level (Id, Indexes, Name) SELECT Id, Indexes, Name FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE PathwayProcedure (@table PathwayType READONLY) AS INSERT INTO Pathway (Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs) SELECT Id, FromStopId, ToStopId, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE RouteProcedure (@table RouteType READONLY) AS INSERT INTO Route (Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor) SELECT Id, AgencyId, ShortName, LongName, Description, Type, Url, Color, TextColor FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE ShapeProcedure (@table ShapeType READONLY) AS INSERT INTO Shape (Id, Latitude, Longitude, Sequence, DistanceTravelled) SELECT Id, Latitude, Longitude, Sequence, DistanceTravelled FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE StopProcedure (@table StopType READONLY) AS INSERT INTO Stop (Id, Code, Name, Description, Latitude, Longitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode) SELECT Id, Code, Name, Description, Latitude, Longitude, Zone, Url, LocationType, ParentStation, Timezone, WheelchairBoarding, LevelId, PlatformCode FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE StopTimeProcedure (@table StopTimeType READONLY) AS INSERT INTO StopTime (TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType) SELECT TripId, ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTravelled, TimepointType FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE TransferProcedure (@table TransferType READONLY) AS INSERT INTO Transfer (FromStopId, ToStopId, TransferType, MinimumTransferTime) SELECT FromStopId, ToStopId, TransferType, MinimumTransferTime FROM @table");
            await connection.ExecuteCommandAsync("CREATE PROCEDURE TripProcedure (@table TripType READONLY) AS INSERT INTO Trip (Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType) SELECT Id, RouteId, ServiceId, Headsign, ShortName, Direction, BlockId, ShapeId, AccessibilityType FROM @table");

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("AgencyProcedure", agency, feed.Agencies, (dt, agency) => dt.Rows.Add(agency.Id, agency.Name, agency.URL, agency.Timezone, agency.LanguageCode, agency.Phone, agency.FareURL, agency.Email));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("CalendarProcedure", calendar, feed.Calendars.GroupBy(c => c.ServiceId).Select(c => c.First()), (dt, calendar) => dt.Rows.Add(calendar.ServiceId, calendar.Monday, calendar.Tuesday, calendar.Wednesday, calendar.Thursday, calendar.Friday, calendar.Saturday, calendar.Sunday, calendar.StartDate, calendar.EndDate));

            Console.WriteLine("INSERT: calendar.txt");

            DataTable calendarDate = new DataTable();
            calendarDate.Columns.Add("ServiceId", typeof(string));
            calendarDate.Columns.Add("Date", typeof(DateTime));
            calendarDate.Columns.Add("ExceptionType", typeof(int));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("CalendarDateProcedure", calendarDate, feed.CalendarDates, (dt, calendarDate) => dt.Rows.Add(calendarDate.ServiceId, calendarDate.Date, calendarDate.ExceptionType));

            Console.WriteLine("INSERT: calendar_dates.txt");

            DataTable fareAttribute = new DataTable();
            fareAttribute.Columns.Add("FareId", typeof(string));
            fareAttribute.Columns.Add("Price", typeof(string));
            fareAttribute.Columns.Add("CurrencyType", typeof(string));
            fareAttribute.Columns.Add("PaymentMethod", typeof(int));
            fareAttribute.Columns.Add("Transfers", typeof(int));
            fareAttribute.Columns.Add("AgencyId", typeof(string));
            fareAttribute.Columns.Add("TransferDuration", typeof(string));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FareAttributeProcedure", fareAttribute, feed.FareAttributes, (dt, fareAttribute) => dt.Rows.Add(fareAttribute.FareId, fareAttribute.Price, fareAttribute.CurrencyType, fareAttribute.PaymentMethod, fareAttribute.Transfers, fareAttribute.AgencyId, fareAttribute.TransferDuration));

            Console.WriteLine("INSERT: fare_attributes.txt");

            DataTable fareRule = new DataTable();
            fareRule.Columns.Add("FareId", typeof(string));
            fareRule.Columns.Add("RouteId", typeof(string));
            fareRule.Columns.Add("OriginId", typeof(string));
            fareRule.Columns.Add("DestinationId", typeof(string));
            fareRule.Columns.Add("ContainsId", typeof(string));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FareRuleProcedure", fareRule, feed.FareRules, (dt, fareRule) => dt.Rows.Add(fareRule.FareId, fareRule.RouteId, fareRule.OriginId, fareRule.DestinationId, fareRule.ContainsId));

            Console.WriteLine("INSERT: fare_rules.txt");

            DataTable frequency = new DataTable();
            frequency.Columns.Add("TripId", typeof(string));
            frequency.Columns.Add("StartTime", typeof(string));
            frequency.Columns.Add("EndTime", typeof(string));
            frequency.Columns.Add("HeadwaySecs", typeof(string));
            frequency.Columns.Add("ExactTimes", typeof(bool));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FrequencyProcedure", frequency, feed.Frequencies, (dt, frequency) => dt.Rows.Add(frequency.TripId, frequency.StartTime, frequency.EndTime, frequency.HeadwaySecs, frequency.ExactTimes));

            Console.WriteLine("INSERT: frequencies.txt");

            DataTable level = new DataTable();
            level.Columns.Add("Id", typeof(string));
            level.Columns.Add("Indexes", typeof(double));
            level.Columns.Add("Name", typeof(string));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("LevelProcedure", level, feed.Levels, (dt, level) => dt.Rows.Add(level.Id, level.Index, level.Name));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("PathwayProcedure", pathway, feed.Pathways, (dt, pathway) => dt.Rows.Add(pathway.Id, pathway.FromStopId, pathway.ToStopId, pathway.PathwayMode, pathway.IsBidirectional, pathway.Length, pathway.TraversalTime, pathway.StairCount, pathway.MaxSlope, pathway.MinWidth, pathway.SignpostedAs, pathway.ReversedSignpostedAs));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("RouteProcedure", route, feed.Routes.GroupBy(r => r.Id).Select(r => r.First()), (dt, route) => dt.Rows.Add(route.Id, route.AgencyId, route.ShortName, route.LongName, route.Description, route.Type, route.Url, route.Color, route.TextColor));

            Console.WriteLine("INSERT: routes.txt");

            DataTable shape = new DataTable();
            shape.Columns.Add("Id", typeof(string));
            shape.Columns.Add("Latitude", typeof(double));
            shape.Columns.Add("Longitude", typeof(double));
            shape.Columns.Add("Sequence", typeof(int));
            shape.Columns.Add("DistanceTravelled", typeof(double));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("ShapeProcedure", shape, feed.Shapes, (dt, shape) => dt.Rows.Add(shape.Id, shape.Latitude, shape.Longitude, shape.Sequence, shape.DistanceTravelled));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("StopProcedure", stop, feed.Stops.GroupBy(s => s.Id).Select(s => s.First()), (dt, stop) => dt.Rows.Add(stop.Id, stop.Code, stop.Name, stop.Description, stop.Latitude, stop.Longitude, stop.Zone, stop.Url, stop.LocationType, stop.ParentStation, stop.Timezone, stop.WheelchairBoarding, stop.LevelId, stop.PlatformCode));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("StopTimeProcedure", stopTime, feed.StopTimes, (dt, stopTime) => dt.Rows.Add(stopTime.TripId, stopTime.ArrivalTime, stopTime.DepartureTime, stopTime.StopId, stopTime.StopSequence, stopTime.StopHeadsign, stopTime.PickupType, stopTime.DropOffType, stopTime.ShapeDistTravelled, stopTime.TimepointType));

            Console.WriteLine("INSERT: stop_times.txt");

            DataTable transfer = new DataTable();
            transfer.Columns.Add("FromStopId", typeof(string));
            transfer.Columns.Add("ToStopId", typeof(string));
            transfer.Columns.Add("TransferType", typeof(int));
            transfer.Columns.Add("MinimumTransferTime", typeof(string));

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("TransferProcedure", transfer, feed.Transfers, (dt, transfer) => dt.Rows.Add(transfer.FromStopId, transfer.ToStopId, transfer.TransferType, transfer.MinimumTransferTime));

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

            await connection.ExecuteStoredProcedureFromTableInBatchesAsync("TripProcedure", trip, feed.Trips.GroupBy(t => t.Id).Select(t => t.First()), (dt, trip) => dt.Rows.Add(trip.Id, trip.RouteId, trip.ServiceId, trip.Headsign, trip.ShortName, trip.Direction, trip.BlockId, trip.ShapeId, trip.AccessibilityType));

            Console.WriteLine("INSERT: trips.txt");

            await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexStop ON StopTime (StopId, PickupType) INCLUDE (TripId, ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType) WITH (ONLINE = ON)");
            await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexTrip ON StopTime (TripId, PickupType) INCLUDE (ArrivalTime, DepartureTime, StopId, StopSequence, StopHeadsign, DropOffType, ShapeDistTravelled, TimepointType) WITH (ONLINE = ON)");

            Console.WriteLine("CREATE: indexes");
        }
    }
}