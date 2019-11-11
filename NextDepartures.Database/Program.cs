using CsvHelper;
using NextDepartures.Database.Extensions;
using NextDepartures.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NextDepartures.Database
{
    internal class Program
    {
        static void Main(string[] args) => new Program().RunAsync(args).GetAwaiter().GetResult();

        public async Task RunAsync(string[] args)
        {
            Console.WriteLine("");

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(args[0]);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(string.Format("Connection string invalid. Error: {0}", exception.Message));
                Environment.Exit(1);
            }

            using (SqlConnection connection = new SqlConnection(args[0]))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Console.Error.WriteLine(string.Format("Could not connect to database. Error: {0}", exception.Message));
                    Environment.Exit(1);
                }

                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS AgencyProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarDateProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FareAttributeProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FareRuleProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS FeedInfoProcedure");
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
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS FeedInfoType");
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
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS FeedInfo");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Frequency");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Level");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Pathway");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Route");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Shape");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Stop");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS StopTime");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Transfer");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Trip");
                await connection.ExecuteCommandAsync("CREATE TABLE Agency (AgencyID nvarchar(255), AgencyName nvarchar(255), AgencyUrl nvarchar(255), AgencyTimezone nvarchar(255), AgencyLang nvarchar(255), AgencyPhone nvarchar(255), AgencyFareUrl nvarchar(255), AgencyEmail nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Calendar (ServiceID nvarchar(255) PRIMARY KEY, Monday nvarchar(255), Tuesday nvarchar(255), Wednesday nvarchar(255), Thursday nvarchar(255), Friday nvarchar(255), Saturday nvarchar(255), Sunday nvarchar(255), StartDate nvarchar(255), EndDate nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE CalendarDate (ServiceID nvarchar(255), Date nvarchar(255), ExceptionType nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE FareAttribute (FareID nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod nvarchar(255), Transfers nvarchar(255), AgencyID nvarchar(255), TransferDuration nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE FareRule (FareID nvarchar(255), RouteID nvarchar(255), OriginID nvarchar(255), DestinationID nvarchar(255), ContainsID nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE FeedInfo (FeedPublisherName nvarchar(255), FeedPublisherUrl nvarchar(255), FeedLang nvarchar(255), FeedStartDate nvarchar(255), FeedEndDate nvarchar(255), FeedVersion nvarchar(255), FeedContactEmail nvarchar(255), FeedContactUrl nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Frequency (TripID nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Level (LevelID nvarchar(255), LevelIndex nvarchar(255), LevelName nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Pathway (PathwayID nvarchar(255), FromStopID nvarchar(255), ToStopID nvarchar(255), PathwayMode nvarchar(255), IsBidirectional nvarchar(255), Length nvarchar(255), TraversalTime nvarchar(255), StairCount nvarchar(255), MaxSlope nvarchar(255), MinWidth nvarchar(255), SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Route (RouteID nvarchar(255) PRIMARY KEY, AgencyID nvarchar(255), RouteShortName nvarchar(255), RouteLongName nvarchar(255), RouteDesc nvarchar(255), RouteType nvarchar(255), RouteUrl nvarchar(255), RouteColor nvarchar(255), RouteTextColor nvarchar(255), RouteSortOrder nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Shape (ShapeID nvarchar(255), ShapePtLat nvarchar(255), ShapePtLon nvarchar(255), ShapePtSequence nvarchar(255), ShapeDistTraveled nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Stop (StopID nvarchar(255) PRIMARY KEY, StopCode nvarchar(255), StopName nvarchar(255), StopDesc nvarchar(255), StopLat nvarchar(255), StopLon nvarchar(255), ZoneID nvarchar(255), StopUrl nvarchar(255), LocationType nvarchar(255), ParentStation nvarchar(255), StopTimezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelID nvarchar(255), PlatformCode nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE StopTime (TripID nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopID nvarchar(255), StopSequence nvarchar(255), StopHeadsign nvarchar(255), PickupType nvarchar(255), DropOffType nvarchar(255), ShapeDistTraveled nvarchar(255), Timepoint nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Transfer (FromStopID nvarchar(255), ToStopID nvarchar(255), TransferType nvarchar(255), MinTransferTime nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Trip (RouteID nvarchar(255), ServiceID nvarchar(255), TripID nvarchar(255) PRIMARY KEY, TripHeadsign nvarchar(255), TripShortName nvarchar(255), DirectionID nvarchar(255), BlockID nvarchar(255), ShapeID nvarchar(255), WheelchairAccessible nvarchar(255), BikesAllowed nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE AgencyType AS TABLE (AgencyID nvarchar(255), AgencyName nvarchar(255), AgencyUrl nvarchar(255), AgencyTimezone nvarchar(255), AgencyLang nvarchar(255), AgencyPhone nvarchar(255), AgencyFareUrl nvarchar(255), AgencyEmail nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE CalendarType AS TABLE (ServiceID nvarchar(255), Monday nvarchar(255), Tuesday nvarchar(255), Wednesday nvarchar(255), Thursday nvarchar(255), Friday nvarchar(255), Saturday nvarchar(255), Sunday nvarchar(255), StartDate nvarchar(255), EndDate nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE CalendarDateType AS TABLE (ServiceID nvarchar(255), Date nvarchar(255), ExceptionType nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE FareAttributeType AS TABLE (FareID nvarchar(255), Price nvarchar(255), CurrencyType nvarchar(255), PaymentMethod nvarchar(255), Transfers nvarchar(255), AgencyID nvarchar(255), TransferDuration nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE FareRuleType AS TABLE (FareID nvarchar(255), RouteID nvarchar(255), OriginID nvarchar(255), DestinationID nvarchar(255), ContainsID nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE FeedInfoType AS TABLE (FeedPublisherName nvarchar(255), FeedPublisherUrl nvarchar(255), FeedLang nvarchar(255), FeedStartDate nvarchar(255), FeedEndDate nvarchar(255), FeedVersion nvarchar(255), FeedContactEmail nvarchar(255), FeedContactUrl nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE FrequencyType AS TABLE (TripID nvarchar(255), StartTime nvarchar(255), EndTime nvarchar(255), HeadwaySecs nvarchar(255), ExactTimes nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE LevelType AS TABLE (LevelID nvarchar(255), LevelIndex nvarchar(255), LevelName nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE PathwayType AS TABLE (PathwayID nvarchar(255), FromStopID nvarchar(255), ToStopID nvarchar(255), PathwayMode nvarchar(255), IsBidirectional nvarchar(255), Length nvarchar(255), TraversalTime nvarchar(255), StairCount nvarchar(255), MaxSlope nvarchar(255), MinWidth nvarchar(255), SignpostedAs nvarchar(255), ReversedSignpostedAs nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE RouteType AS TABLE (RouteID nvarchar(255), AgencyID nvarchar(255), RouteShortName nvarchar(255), RouteLongName nvarchar(255), RouteDesc nvarchar(255), RouteType nvarchar(255), RouteUrl nvarchar(255), RouteColor nvarchar(255), RouteTextColor nvarchar(255), RouteSortOrder nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE ShapeType AS TABLE (ShapeID nvarchar(255), ShapePtLat nvarchar(255), ShapePtLon nvarchar(255), ShapePtSequence nvarchar(255), ShapeDistTraveled nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE StopType AS TABLE (StopID nvarchar(255), StopCode nvarchar(255), StopName nvarchar(255), StopDesc nvarchar(255), StopLat nvarchar(255), StopLon nvarchar(255), ZoneID nvarchar(255), StopUrl nvarchar(255), LocationType nvarchar(255), ParentStation nvarchar(255), StopTimezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelID nvarchar(255), PlatformCode nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE StopTimeType AS TABLE (TripID nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopID nvarchar(255), StopSequence nvarchar(255), StopHeadsign nvarchar(255), PickupType nvarchar(255), DropOffType nvarchar(255), ShapeDistTraveled nvarchar(255), Timepoint nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE TransferType AS TABLE (FromStopID nvarchar(255), ToStopID nvarchar(255), TransferType nvarchar(255), MinTransferTime nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE TripType AS TABLE (RouteID nvarchar(255), ServiceID nvarchar(255), TripID nvarchar(255), TripHeadsign nvarchar(255), TripShortName nvarchar(255), DirectionID nvarchar(255), BlockID nvarchar(255), ShapeID nvarchar(255), WheelchairAccessible nvarchar(255), BikesAllowed nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE AgencyProcedure (@table AgencyType READONLY) AS INSERT INTO Agency (AgencyID, AgencyName, AgencyUrl, AgencyTimezone, AgencyLang, AgencyPhone, AgencyFareUrl, AgencyEmail) SELECT AgencyID, AgencyName, AgencyUrl, AgencyTimezone, AgencyLang, AgencyPhone, AgencyFareUrl, AgencyEmail FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarProcedure (@table CalendarType READONLY) AS INSERT INTO Calendar (ServiceID, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) SELECT ServiceID, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarDateProcedure (@table CalendarDateType READONLY) AS INSERT INTO CalendarDate (ServiceID, Date, ExceptionType) SELECT ServiceID, Date, ExceptionType FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE FareAttributeProcedure (@table FareAttributeType READONLY) AS INSERT INTO FareAttribute (FareID, Price, CurrencyType, PaymentMethod, Transfers, AgencyID, TransferDuration) SELECT FareID, Price, CurrencyType, PaymentMethod, Transfers, AgencyID, TransferDuration FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE FareRuleProcedure (@table FareRuleType READONLY) AS INSERT INTO FareRule (FareID, RouteID, OriginID, DestinationID, ContainsID) SELECT FareID, RouteID, OriginID, DestinationID, ContainsID FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE FeedInfoProcedure (@table FeedInfoType READONLY) AS INSERT INTO FeedInfo (FeedPublisherName, FeedPublisherUrl, FeedLang, FeedStartDate, FeedEndDate, FeedVersion, FeedContactEmail, FeedContactUrl) SELECT FeedPublisherName, FeedPublisherUrl, FeedLang, FeedStartDate, FeedEndDate, FeedVersion, FeedContactEmail, FeedContactUrl FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE FrequencyProcedure (@table FrequencyType READONLY) AS INSERT INTO Frequency (TripID, StartTime, EndTime, HeadwaySecs, ExactTimes) SELECT TripID, StartTime, EndTime, HeadwaySecs, ExactTimes FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE LevelProcedure (@table LevelType READONLY) AS INSERT INTO Level (LevelID, LevelIndex, LevelName) SELECT LevelID, LevelIndex, LevelName FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE PathwayProcedure (@table PathwayType READONLY) AS INSERT INTO Pathway (PathwayID, FromStopID, ToStopID, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs) SELECT PathwayID, FromStopID, ToStopID, PathwayMode, IsBidirectional, Length, TraversalTime, StairCount, MaxSlope, MinWidth, SignpostedAs, ReversedSignpostedAs FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE RouteProcedure (@table RouteType READONLY) AS INSERT INTO Route (RouteID, AgencyID, RouteShortName, RouteLongName, RouteDesc, RouteType, RouteUrl, RouteColor, RouteTextColor, RouteSortOrder) SELECT RouteID, AgencyID, RouteShortName, RouteLongName, RouteDesc, RouteType, RouteUrl, RouteColor, RouteTextColor, RouteSortOrder FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE ShapeProcedure (@table ShapeType READONLY) AS INSERT INTO Shape (ShapeID, ShapePtLat, ShapePtLon, ShapePtSequence, ShapeDistTraveled) SELECT ShapeID, ShapePtLat, ShapePtLon, ShapePtSequence, ShapeDistTraveled FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE StopProcedure (@table StopType READONLY) AS INSERT INTO Stop (StopID, StopCode, StopName, StopDesc, StopLat, StopLon, ZoneID, StopUrl, LocationType, ParentStation, StopTimezone, WheelchairBoarding, LevelID, PlatformCode) SELECT StopID, StopCode, StopName, StopDesc, StopLat, StopLon, ZoneID, StopUrl, LocationType, ParentStation, StopTimezone, WheelchairBoarding, LevelID, PlatformCode FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE StopTimeProcedure (@table StopTimeType READONLY) AS INSERT INTO StopTime (TripID, ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTraveled, Timepoint) SELECT TripID, ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTraveled, Timepoint FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE TransferProcedure (@table TransferType READONLY) AS INSERT INTO Transfer (FromStopID, ToStopID, TransferType, MinTransferTime) SELECT FromStopID, ToStopID, TransferType, MinTransferTime FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE TripProcedure (@table TripType READONLY) AS INSERT INTO Trip (RouteID, ServiceID, TripID, TripHeadsign, TripShortName, DirectionID, BlockID, ShapeID, WheelchairAccessible, BikesAllowed) SELECT RouteID, ServiceID, TripID, TripHeadsign, TripShortName, DirectionID, BlockID, ShapeID, WheelchairAccessible, BikesAllowed FROM @table");

                Console.WriteLine("CREATE: tables");

                using (HttpClient http = new HttpClient())
                {
                    http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                    using (ZipArchive archive = new ZipArchive(await http.GetStreamAsync(new Uri(args[1]))))
                    {
                        Console.WriteLine("GET: feed.zip");

                        foreach (ZipArchiveEntry archiveEntry in archive.Entries.OrderBy(x => x.Name))
                        {
                            if (archiveEntry.Name == "agency.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Agency> workingAgencies = feedReader.GetRecords<Agency>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("AgencyID", typeof(string));
                                        table.Columns.Add("AgencyName", typeof(string));
                                        table.Columns.Add("AgencyUrl", typeof(string));
                                        table.Columns.Add("AgencyTimezone", typeof(string));
                                        table.Columns.Add("AgencyLang", typeof(string));
                                        table.Columns.Add("AgencyPhone", typeof(string));
                                        table.Columns.Add("AgencyFareUrl", typeof(string));
                                        table.Columns.Add("AgencyEmail", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("AgencyProcedure", table, workingAgencies, (dt, agency) => dt.Rows.Add(agency.agency_id, agency.agency_name, agency.agency_url, agency.agency_timezone, agency.agency_lang, agency.agency_phone, agency.agency_fare_url, agency.agency_email));
                                    }
                                }

                                Console.WriteLine("INSERT: agency.txt");
                            }
                            else if (archiveEntry.Name == "calendar.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Calendar> workingCalendars = feedReader.GetRecords<Calendar>().GroupBy(c => c.service_id).Select(c => c.First());

                                        DataTable table = new DataTable();
                                        table.Columns.Add("ServiceID", typeof(string));
                                        table.Columns.Add("Monday", typeof(string));
                                        table.Columns.Add("Tuesday", typeof(string));
                                        table.Columns.Add("Wednesday", typeof(string));
                                        table.Columns.Add("Thursday", typeof(string));
                                        table.Columns.Add("Friday", typeof(string));
                                        table.Columns.Add("Saturday", typeof(string));
                                        table.Columns.Add("Sunday", typeof(string));
                                        table.Columns.Add("StartDate", typeof(string));
                                        table.Columns.Add("EndDate", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("CalendarProcedure", table, workingCalendars, (dt, calendar) => dt.Rows.Add(calendar.service_id, calendar.monday, calendar.tuesday, calendar.wednesday, calendar.thursday, calendar.friday, calendar.saturday, calendar.sunday, calendar.start_date, calendar.end_date));
                                    }
                                }

                                Console.WriteLine("INSERT: calendar.txt");
                            }
                            else if (archiveEntry.Name == "calendar_dates.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<CalendarDate> workingCalendarDates = feedReader.GetRecords<CalendarDate>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("ServiceID", typeof(string));
                                        table.Columns.Add("Date", typeof(string));
                                        table.Columns.Add("ExceptionType", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("CalendarDateProcedure", table, workingCalendarDates, (dt, calendarDate) => dt.Rows.Add(calendarDate.service_id, calendarDate.date, calendarDate.exception_type));
                                    }
                                }

                                Console.WriteLine("INSERT: calendar_dates.txt");
                            }
                            else if (archiveEntry.Name == "fare_attributes.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<FareAttribute> workingFareAttributes = feedReader.GetRecords<FareAttribute>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("FareID", typeof(string));
                                        table.Columns.Add("Price", typeof(string));
                                        table.Columns.Add("CurrencyType", typeof(string));
                                        table.Columns.Add("PaymentMethod", typeof(string));
                                        table.Columns.Add("Transfers", typeof(string));
                                        table.Columns.Add("AgencyID", typeof(string));
                                        table.Columns.Add("TransferDuration", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FareAttributeProcedure", table, workingFareAttributes, (dt, fareAttribute) => dt.Rows.Add(fareAttribute.fare_id, fareAttribute.price, fareAttribute.currency_type, fareAttribute.payment_method, fareAttribute.transfers, fareAttribute.agency_id, fareAttribute.transfer_duration));
                                    }
                                }

                                Console.WriteLine("INSERT: fare_attributes.txt");
                            }
                            else if (archiveEntry.Name == "fare_rules.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<FareRule> workingFareRules = feedReader.GetRecords<FareRule>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("FareID", typeof(string));
                                        table.Columns.Add("RouteID", typeof(string));
                                        table.Columns.Add("OriginID", typeof(string));
                                        table.Columns.Add("DestinationID", typeof(string));
                                        table.Columns.Add("ContainsID", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FareRuleProcedure", table, workingFareRules, (dt, fareRule) => dt.Rows.Add(fareRule.fare_id, fareRule.route_id, fareRule.origin_id, fareRule.destination_id, fareRule.contains_id));
                                    }
                                }

                                Console.WriteLine("INSERT: fare_rules.txt");
                            }
                            else if (archiveEntry.Name == "feed_info.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<FeedInfo> workingFeedInfos = feedReader.GetRecords<FeedInfo>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("FeedPublisherName", typeof(string));
                                        table.Columns.Add("FeedPublisherUrl", typeof(string));
                                        table.Columns.Add("FeedLang", typeof(string));
                                        table.Columns.Add("FeedStartDate", typeof(string));
                                        table.Columns.Add("FeedEndDate", typeof(string));
                                        table.Columns.Add("FeedVersion", typeof(string));
                                        table.Columns.Add("FeedContactEmail", typeof(string));
                                        table.Columns.Add("FeedContactUrl", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FeedInfoProcedure", table, workingFeedInfos, (dt, feedInfo) => dt.Rows.Add(feedInfo.feed_publisher_name, feedInfo.feed_publisher_url, feedInfo.feed_lang, feedInfo.feed_start_date, feedInfo.feed_end_date, feedInfo.feed_version, feedInfo.feed_contact_email, feedInfo.feed_contact_url));
                                    }
                                }

                                Console.WriteLine("INSERT: feed_info.txt");
                            }
                            else if (archiveEntry.Name == "frequencies.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Frequency> workingFrequencies = feedReader.GetRecords<Frequency>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("TripID", typeof(string));
                                        table.Columns.Add("StartTime", typeof(string));
                                        table.Columns.Add("EndTime", typeof(string));
                                        table.Columns.Add("HeadwaySecs", typeof(string));
                                        table.Columns.Add("ExactTimes", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("FrequencyProcedure", table, workingFrequencies, (dt, frequency) => dt.Rows.Add(frequency.trip_id, frequency.start_time, frequency.end_time, frequency.headway_secs, frequency.exact_times));
                                    }
                                }

                                Console.WriteLine("INSERT: frequencies.txt");
                            }
                            else if (archiveEntry.Name == "levels.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Level> workingLevels = feedReader.GetRecords<Level>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("LevelID", typeof(string));
                                        table.Columns.Add("LevelIndex", typeof(string));
                                        table.Columns.Add("LevelName", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("LevelProcedure", table, workingLevels, (dt, level) => dt.Rows.Add(level.level_id, level.level_index, level.level_name));
                                    }
                                }

                                Console.WriteLine("INSERT: levels.txt");
                            }
                            else if (archiveEntry.Name == "pathways.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Pathway> workingPathways = feedReader.GetRecords<Pathway>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("PathwayID", typeof(string));
                                        table.Columns.Add("FromStopID", typeof(string));
                                        table.Columns.Add("ToStopID", typeof(string));
                                        table.Columns.Add("PathwayMode", typeof(string));
                                        table.Columns.Add("IsBidirectional", typeof(string));
                                        table.Columns.Add("Length", typeof(string));
                                        table.Columns.Add("TraversalTime", typeof(string));
                                        table.Columns.Add("StairCount", typeof(string));
                                        table.Columns.Add("MaxSlope", typeof(string));
                                        table.Columns.Add("MinWidth", typeof(string));
                                        table.Columns.Add("SignpostedAs", typeof(string));
                                        table.Columns.Add("ReversedSignpostedAs", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("PathwayProcedure", table, workingPathways, (dt, pathway) => dt.Rows.Add(pathway.pathway_id, pathway.from_stop_id, pathway.to_stop_id, pathway.pathway_mode, pathway.is_bidirectional, pathway.length, pathway.traversal_time, pathway.stair_count, pathway.max_slope, pathway.min_width, pathway.signposted_as, pathway.reversed_signposted_as));
                                    }
                                }

                                Console.WriteLine("INSERT: pathways.txt");
                            }
                            else if (archiveEntry.Name == "routes.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Route> workingRoutes = feedReader.GetRecords<Route>().GroupBy(r => r.route_id).Select(r => r.First());

                                        DataTable table = new DataTable();
                                        table.Columns.Add("RouteID", typeof(string));
                                        table.Columns.Add("AgencyID", typeof(string));
                                        table.Columns.Add("RouteShortName", typeof(string));
                                        table.Columns.Add("RouteLongName", typeof(string));
                                        table.Columns.Add("RouteDesc", typeof(string));
                                        table.Columns.Add("RouteType", typeof(string));
                                        table.Columns.Add("RouteUrl", typeof(string));
                                        table.Columns.Add("RouteColor", typeof(string));
                                        table.Columns.Add("RouteTextColor", typeof(string));
                                        table.Columns.Add("RouteSortOrder", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("RouteProcedure", table, workingRoutes, (dt, route) => dt.Rows.Add(route.route_id, route.agency_id, route.route_short_name, route.route_long_name, route.route_desc, route.route_type, route.route_url, route.route_color, route.route_text_color, route.route_sort_order));
                                    }
                                }

                                Console.WriteLine("INSERT: routes.txt");
                            }
                            else if (archiveEntry.Name == "shapes.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Shape> workingShapes = feedReader.GetRecords<Shape>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("ShapeID", typeof(string));
                                        table.Columns.Add("ShapePtLat", typeof(string));
                                        table.Columns.Add("ShapePtLon", typeof(string));
                                        table.Columns.Add("ShapePtSequence", typeof(string));
                                        table.Columns.Add("ShapeDistTraveled", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("ShapeProcedure", table, workingShapes, (dt, shape) => dt.Rows.Add(shape.shape_id, shape.shape_pt_lat, shape.shape_pt_lon, shape.shape_pt_sequence, shape.shape_dist_traveled));
                                    }
                                }

                                Console.WriteLine("INSERT: shapes.txt");
                            }
                            else if (archiveEntry.Name == "stop_times.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<StopTime> workingStopTimes = feedReader.GetRecords<StopTime>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("TripID", typeof(string));
                                        table.Columns.Add("ArrivalTime", typeof(string));
                                        table.Columns.Add("DepartureTime", typeof(string));
                                        table.Columns.Add("StopID", typeof(string));
                                        table.Columns.Add("StopSequence", typeof(string));
                                        table.Columns.Add("StopHeadsign", typeof(string));
                                        table.Columns.Add("PickupType", typeof(string));
                                        table.Columns.Add("DropOffType", typeof(string));
                                        table.Columns.Add("ShapeDistTraveled", typeof(string));
                                        table.Columns.Add("Timepoint", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("StopTimeProcedure", table, workingStopTimes, (dt, stopTime) => dt.Rows.Add(stopTime.trip_id, stopTime.arrival_time, stopTime.departure_time, stopTime.stop_id, stopTime.stop_sequence, stopTime.stop_headsign, stopTime.pickup_type, stopTime.drop_off_type, stopTime.shape_dist_traveled, stopTime.timepoint));
                                    }
                                }

                                Console.WriteLine("INSERT: stop_times.txt");
                            }
                            else if (archiveEntry.Name == "stops.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Stop> workingStops = feedReader.GetRecords<Stop>().GroupBy(s => s.stop_id).Select(s => s.First());

                                        DataTable table = new DataTable();
                                        table.Columns.Add("StopID", typeof(string));
                                        table.Columns.Add("StopCode", typeof(string));
                                        table.Columns.Add("StopName", typeof(string));
                                        table.Columns.Add("StopDesc", typeof(string));
                                        table.Columns.Add("StopLat", typeof(string));
                                        table.Columns.Add("StopLon", typeof(string));
                                        table.Columns.Add("ZoneID", typeof(string));
                                        table.Columns.Add("StopUrl", typeof(string));
                                        table.Columns.Add("LocationType", typeof(string));
                                        table.Columns.Add("ParentStation", typeof(string));
                                        table.Columns.Add("StopTimezone", typeof(string));
                                        table.Columns.Add("WheelchairBoarding", typeof(string));
                                        table.Columns.Add("LevelID", typeof(string));
                                        table.Columns.Add("PlatformCode", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("StopProcedure", table, workingStops, (dt, stop) => dt.Rows.Add(stop.stop_id, stop.stop_code, stop.stop_name, stop.stop_desc, stop.stop_lat, stop.stop_lon, stop.zone_id, stop.stop_url, stop.location_type, stop.parent_station, stop.stop_timezone, stop.wheelchair_boarding, stop.level_id, stop.platform_code));
                                    }
                                }

                                Console.WriteLine("INSERT: stops.txt");
                            }
                            else if (archiveEntry.Name == "transfers.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Transfer> workingTransfers = feedReader.GetRecords<Transfer>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("FromStopID", typeof(string));
                                        table.Columns.Add("ToStopID", typeof(string));
                                        table.Columns.Add("TransferType", typeof(string));
                                        table.Columns.Add("MinTransferTime", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("TransferProcedure", table, workingTransfers, (dt, transfer) => dt.Rows.Add(transfer.from_stop_id, transfer.to_stop_id, transfer.transfer_type, transfer.min_transfer_time));
                                    }
                                }

                                Console.WriteLine("INSERT: transfers.txt");
                            }
                            else if (archiveEntry.Name == "trips.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = feedInput.GetCsvReader())
                                    {
                                        IEnumerable<Trip> workingTrips = feedReader.GetRecords<Trip>().GroupBy(t => t.trip_id).Select(t => t.First());

                                        DataTable table = new DataTable();
                                        table.Columns.Add("RouteID", typeof(string));
                                        table.Columns.Add("ServiceID", typeof(string));
                                        table.Columns.Add("TripID", typeof(string));
                                        table.Columns.Add("TripHeadsign", typeof(string));
                                        table.Columns.Add("TripShortName", typeof(string));
                                        table.Columns.Add("DirectionID", typeof(string));
                                        table.Columns.Add("BlockID", typeof(string));
                                        table.Columns.Add("ShapeID", typeof(string));
                                        table.Columns.Add("WheelchairAccessible", typeof(string));
                                        table.Columns.Add("BikesAllowed", typeof(string));

                                        await connection.ExecuteStoredProcedureFromTableInBatchesAsync("TripProcedure", table, workingTrips, (dt, trip) => dt.Rows.Add(trip.route_id, trip.service_id, trip.trip_id, trip.trip_headsign, trip.trip_short_name, trip.direction_id, trip.block_id, trip.shape_id, trip.wheelchair_accessible, trip.bikes_allowed));
                                    }
                                }

                                Console.WriteLine("INSERT: trips.txt");
                            }
                        }
                    }
                }

                await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexStop ON StopTime (StopID, PickupType) INCLUDE (TripID, ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTraveled, Timepoint) WITH (ONLINE = ON)");
                await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexTrip ON StopTime (TripID, PickupType) INCLUDE (ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, DropOffType, ShapeDistTraveled, Timepoint) WITH (ONLINE = ON)");

                Console.WriteLine("CREATE: indexes");
            }
        }
    }
}