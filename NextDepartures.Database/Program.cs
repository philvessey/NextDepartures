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

            using (SqlConnection connection = new SqlConnection(args[0]))
            {
                connection.Open();

                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS AgencyProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS CalendarDateProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS RouteProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS StopProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS StopTimeProcedure");
                await connection.ExecuteCommandAsync("DROP PROCEDURE IF EXISTS TripProcedure");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS AgencyType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS CalendarType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS CalendarDateType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS RouteType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS StopType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS StopTimeType");
                await connection.ExecuteCommandAsync("DROP TYPE IF EXISTS TripType");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Agency");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Calendar");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS CalendarDate");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Route");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Stop");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS StopTime");
                await connection.ExecuteCommandAsync("DROP TABLE IF EXISTS Trip");
                await connection.ExecuteCommandAsync("CREATE TABLE Agency (AgencyID nvarchar(255), AgencyName nvarchar(255), AgencyUrl nvarchar(255), AgencyTimezone nvarchar(255), AgencyLang nvarchar(255), AgencyPhone nvarchar(255), AgencyFareUrl nvarchar(255), AgencyEmail nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Calendar (ServiceID nvarchar(255) PRIMARY KEY, Monday nvarchar(255), Tuesday nvarchar(255), Wednesday nvarchar(255), Thursday nvarchar(255), Friday nvarchar(255), Saturday nvarchar(255), Sunday nvarchar(255), StartDate nvarchar(255), EndDate nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE CalendarDate (ServiceID nvarchar(255), Date nvarchar(255), ExceptionType nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Route (RouteID nvarchar(255) PRIMARY KEY, AgencyID nvarchar(255), RouteShortName nvarchar(255), RouteLongName nvarchar(255), RouteDesc nvarchar(255), RouteType nvarchar(255), RouteUrl nvarchar(255), RouteColor nvarchar(255), RouteTextColor nvarchar(255), RouteSortOrder nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Stop (StopID nvarchar(255) PRIMARY KEY, StopCode nvarchar(255), StopName nvarchar(255), StopDesc nvarchar(255), StopLat nvarchar(255), StopLon nvarchar(255), ZoneID nvarchar(255), StopUrl nvarchar(255), LocationType nvarchar(255), ParentStation nvarchar(255), StopTimezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelID nvarchar(255), PlatformCode nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE StopTime (TripID nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopID nvarchar(255), StopSequence nvarchar(255), StopHeadsign nvarchar(255), PickupType nvarchar(255), DropOffType nvarchar(255), ShapeDistTraveled nvarchar(255), Timepoint nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TABLE Trip (RouteID nvarchar(255), ServiceID nvarchar(255), TripID nvarchar(255) PRIMARY KEY, TripHeadsign nvarchar(255), TripShortName nvarchar(255), DirectionID nvarchar(255), BlockID nvarchar(255), ShapeID nvarchar(255), WheelchairAccessible nvarchar(255), BikesAllowed nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE AgencyType AS TABLE (AgencyID nvarchar(255), AgencyName nvarchar(255), AgencyUrl nvarchar(255), AgencyTimezone nvarchar(255), AgencyLang nvarchar(255), AgencyPhone nvarchar(255), AgencyFareUrl nvarchar(255), AgencyEmail nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE CalendarType AS TABLE (ServiceID nvarchar(255), Monday nvarchar(255), Tuesday nvarchar(255), Wednesday nvarchar(255), Thursday nvarchar(255), Friday nvarchar(255), Saturday nvarchar(255), Sunday nvarchar(255), StartDate nvarchar(255), EndDate nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE CalendarDateType AS TABLE (ServiceID nvarchar(255), Date nvarchar(255), ExceptionType nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE RouteType AS TABLE (RouteID nvarchar(255), AgencyID nvarchar(255), RouteShortName nvarchar(255), RouteLongName nvarchar(255), RouteDesc nvarchar(255), RouteType nvarchar(255), RouteUrl nvarchar(255), RouteColor nvarchar(255), RouteTextColor nvarchar(255), RouteSortOrder nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE StopType AS TABLE (StopID nvarchar(255), StopCode nvarchar(255), StopName nvarchar(255), StopDesc nvarchar(255), StopLat nvarchar(255), StopLon nvarchar(255), ZoneID nvarchar(255), StopUrl nvarchar(255), LocationType nvarchar(255), ParentStation nvarchar(255), StopTimezone nvarchar(255), WheelchairBoarding nvarchar(255), LevelID nvarchar(255), PlatformCode nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE StopTimeType AS TABLE (TripID nvarchar(255), ArrivalTime nvarchar(255), DepartureTime nvarchar(255), StopID nvarchar(255), StopSequence nvarchar(255), StopHeadsign nvarchar(255), PickupType nvarchar(255), DropOffType nvarchar(255), ShapeDistTraveled nvarchar(255), Timepoint nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE TYPE TripType AS TABLE (RouteID nvarchar(255), ServiceID nvarchar(255), TripID nvarchar(255), TripHeadsign nvarchar(255), TripShortName nvarchar(255), DirectionID nvarchar(255), BlockID nvarchar(255), ShapeID nvarchar(255), WheelchairAccessible nvarchar(255), BikesAllowed nvarchar(255))");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE AgencyProcedure (@table AgencyType READONLY) AS INSERT INTO Agency (AgencyID, AgencyName, AgencyUrl, AgencyTimezone, AgencyLang, AgencyPhone, AgencyFareUrl, AgencyEmail) SELECT AgencyID, AgencyName, AgencyUrl, AgencyTimezone, AgencyLang, AgencyPhone, AgencyFareUrl, AgencyEmail FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarProcedure (@table CalendarType READONLY) AS INSERT INTO Calendar (ServiceID, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate) SELECT ServiceID, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartDate, EndDate FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE CalendarDateProcedure (@table CalendarDateType READONLY) AS INSERT INTO CalendarDate (ServiceID, Date, ExceptionType) SELECT ServiceID, Date, ExceptionType FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE RouteProcedure (@table RouteType READONLY) AS INSERT INTO Route (RouteID, AgencyID, RouteShortName, RouteLongName, RouteDesc, RouteType, RouteUrl, RouteColor, RouteTextColor, RouteSortOrder) SELECT RouteID, AgencyID, RouteShortName, RouteLongName, RouteDesc, RouteType, RouteUrl, RouteColor, RouteTextColor, RouteSortOrder FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE StopProcedure (@table StopType READONLY) AS INSERT INTO Stop (StopID, StopCode, StopName, StopDesc, StopLat, StopLon, ZoneID, StopUrl, LocationType, ParentStation, StopTimezone, WheelchairBoarding, LevelID, PlatformCode) SELECT StopID, StopCode, StopName, StopDesc, StopLat, StopLon, ZoneID, StopUrl, LocationType, ParentStation, StopTimezone, WheelchairBoarding, LevelID, PlatformCode FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE StopTimeProcedure (@table StopTimeType READONLY) AS INSERT INTO StopTime (TripID, ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTraveled, Timepoint) SELECT TripID, ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, PickupType, DropOffType, ShapeDistTraveled, Timepoint FROM @table");
                await connection.ExecuteCommandAsync("CREATE PROCEDURE TripProcedure (@table TripType READONLY) AS INSERT INTO Trip (RouteID, ServiceID, TripID, TripHeadsign, TripShortName, DirectionID, BlockID, ShapeID, WheelchairAccessible, BikesAllowed) SELECT RouteID, ServiceID, TripID, TripHeadsign, TripShortName, DirectionID, BlockID, ShapeID, WheelchairAccessible, BikesAllowed FROM @table");

                Console.WriteLine("1/9 Tables created!");

                using (HttpClient http = new HttpClient())
                {
                    http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                    using (ZipArchive archive = new ZipArchive(await http.GetStreamAsync(new Uri(args[1]))))
                    {
                        foreach (ZipArchiveEntry archiveEntry in archive.Entries.OrderBy(x => x.Name))
                        {
                            if (archiveEntry.Name == "agency.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

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

                                        foreach (Agency agency in workingAgencies)
                                        {
                                            table.Rows.Add(agency.agency_id, agency.agency_name, agency.agency_url, agency.agency_timezone, agency.agency_lang, agency.agency_phone, agency.agency_fare_url, agency.agency_email);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("AgencyProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("AgencyProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("2/9 Agency inserted!");
                            }

                            if (archiveEntry.Name == "calendar.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

                                        IEnumerable<Calendar> workingCalendars = feedReader.GetRecords<Calendar>().GroupBy(x => x.service_id).Select(x => x.First());

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

                                        foreach (Calendar calendar in workingCalendars)
                                        {
                                            table.Rows.Add(calendar.service_id, calendar.monday, calendar.tuesday, calendar.wednesday, calendar.thursday, calendar.friday, calendar.saturday, calendar.sunday, calendar.start_date, calendar.end_date);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("CalendarProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("CalendarProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("3/9 Calendar inserted!");
                            }

                            if (archiveEntry.Name == "calendar_dates.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

                                        IEnumerable<CalendarDate> workingCalendarDates = feedReader.GetRecords<CalendarDate>();

                                        DataTable table = new DataTable();
                                        table.Columns.Add("ServiceID", typeof(string));
                                        table.Columns.Add("Date", typeof(string));
                                        table.Columns.Add("ExceptionType", typeof(string));

                                        foreach (CalendarDate calendarDate in workingCalendarDates)
                                        {
                                            table.Rows.Add(calendarDate.service_id, calendarDate.date, calendarDate.exception_type);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("CalendarDateProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("CalendarDateProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("4/9 CalendarDate inserted!");
                            }

                            if (archiveEntry.Name == "routes.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

                                        IEnumerable<Route> workingRoutes = feedReader.GetRecords<Route>().GroupBy(x => x.route_id).Select(x => x.First());

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

                                        foreach (Route route in workingRoutes)
                                        {
                                            table.Rows.Add(route.route_id, route.agency_id, route.route_short_name, route.route_long_name, route.route_desc, route.route_type, route.route_url, route.route_color, route.route_text_color, route.route_sort_order);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("RouteProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("RouteProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("5/9 Route inserted!");
                            }

                            if (archiveEntry.Name == "stop_times.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

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

                                        foreach (StopTime stopTime in workingStopTimes)
                                        {
                                            table.Rows.Add(stopTime.trip_id, stopTime.arrival_time, stopTime.departure_time, stopTime.stop_id, stopTime.stop_sequence, stopTime.stop_headsign, stopTime.pickup_type, stopTime.drop_off_type, stopTime.shape_dist_traveled, stopTime.timepoint);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("StopTimeProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("StopTimeProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("6/9 StopTime inserted!");
                            }

                            if (archiveEntry.Name == "stops.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

                                        IEnumerable<Stop> workingStops = feedReader.GetRecords<Stop>().GroupBy(x => x.stop_id).Select(x => x.First());

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

                                        foreach (Stop stop in workingStops)
                                        {
                                            table.Rows.Add(stop.stop_id, stop.stop_code, stop.stop_name, stop.stop_desc, stop.stop_lat, stop.stop_lon, stop.zone_id, stop.stop_url, stop.location_type, stop.parent_station, stop.stop_timezone, stop.wheelchair_boarding, stop.level_id, stop.platform_code);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("StopProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("StopProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("7/9 Stop inserted!");
                            }

                            if (archiveEntry.Name == "trips.txt")
                            {
                                using (StreamReader feedInput = new StreamReader(archiveEntry.Open()))
                                {
                                    using (CsvReader feedReader = new CsvReader(feedInput))
                                    {
                                        feedReader.Configuration.BadDataFound = null;
                                        feedReader.Configuration.HeaderValidated = null;
                                        feedReader.Configuration.MissingFieldFound = null;
                                        feedReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

                                        IEnumerable<Trip> workingTrips = feedReader.GetRecords<Trip>().GroupBy(x => x.trip_id).Select(x => x.First());

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

                                        foreach (Trip trip in workingTrips)
                                        {
                                            table.Rows.Add(trip.route_id, trip.service_id, trip.trip_id, trip.trip_headsign, trip.trip_short_name, trip.direction_id, trip.block_id, trip.shape_id, trip.wheelchair_accessible, trip.bikes_allowed);

                                            if (table.Rows.Count > 999999)
                                            {
                                                await connection.ExecuteCommandAsync("TripProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                                table.Rows.Clear();
                                            }
                                        }

                                        if (table.Rows.Count > 0)
                                        {
                                            await connection.ExecuteCommandAsync("TripProcedure", CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                                            table.Rows.Clear();
                                        }
                                    }
                                }

                                Console.WriteLine("8/9 Trip inserted!");
                            }
                        }
                    }
                }

                await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexStop ON StopTime (StopID, PickupType) INCLUDE (TripID, ArrivalTime, DepartureTime, StopSequence, StopHeadsign, DropOffType, ShapeDistTraveled, Timepoint) WITH (ONLINE = ON)");
                await connection.ExecuteCommandAsync("CREATE NONCLUSTERED INDEX StopTimeIndexTrip ON StopTime (TripID, PickupType) INCLUDE (ArrivalTime, DepartureTime, StopID, StopSequence, StopHeadsign, DropOffType, ShapeDistTraveled, Timepoint) WITH (ONLINE = ON)");

                Console.WriteLine("9/9 Indexes created!");
            }
        }
    }
}