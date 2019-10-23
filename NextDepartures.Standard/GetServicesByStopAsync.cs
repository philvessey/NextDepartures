using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of services.</summary>
        public async Task<List<Service>> GetServicesByStopAsync(string id, int count = 10)
        {
            List<Service> results = new List<Service>();

            try
            {
                DateTime now = DateTime.UtcNow;
                int yesterdayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(-1).Year, now.AddDays(-1).Month.ToString("00"), now.AddDays(-1).Day.ToString("00")));
                int todayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.Year, now.Month.ToString("00"), now.Day.ToString("00")));
                int tomorrowDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(1).Year, now.AddDays(1).Month.ToString("00"), now.AddDays(1).Day.ToString("00")));

                List<Departure> tempDepartures = new List<Departure>();
                List<Agency> workingAgencies = new List<Agency>();
                List<Exception> workingExceptions = new List<Exception>();
                List<Stop> workingStops = new List<Stop>();

                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();

                    _command = new SqlCommand(string.Format("SELECT s.DepartureTime, s.StopID, t.ServiceID, t.TripID, t.TripHeadsign, t.TripShortName, r.AgencyID, r.RouteShortName, r.RouteLongName, c.Monday, c.Tuesday, c.Wednesday, c.Thursday, c.Friday, c.Saturday, c.Sunday, c.StartDate, c.EndDate FROM StopTime s LEFT JOIN Trip t ON (s.TripID = t.TripID) LEFT JOIN Route r ON (t.RouteID = r.RouteID) LEFT JOIN Calendar c ON (t.ServiceID = c.ServiceID) WHERE LOWER(s.StopID) = '{0}' AND s.PickupType != '1' ORDER BY s.DepartureTime ASC", id.ToLower()), connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = await _command.ExecuteReaderAsync();

                    while (_dataReader.Read())
                    {
                        tempDepartures.Add(new Departure()
                        {
                            DepartureTime = _dataReader.GetValue(0).ToString(),
                            StopID = _dataReader.GetValue(1).ToString(),
                            ServiceID = _dataReader.GetValue(2).ToString(),
                            TripID = _dataReader.GetValue(3).ToString(),
                            TripHeadsign = _dataReader.GetValue(4).ToString(),
                            TripShortName = _dataReader.GetValue(5).ToString(),
                            AgencyID = _dataReader.GetValue(6).ToString(),
                            RouteShortName = _dataReader.GetValue(7).ToString(),
                            RouteLongName = _dataReader.GetValue(8).ToString(),
                            Monday = _dataReader.GetValue(9).ToString(),
                            Tuesday = _dataReader.GetValue(10).ToString(),
                            Wednesday = _dataReader.GetValue(11).ToString(),
                            Thursday = _dataReader.GetValue(12).ToString(),
                            Friday = _dataReader.GetValue(13).ToString(),
                            Saturday = _dataReader.GetValue(14).ToString(),
                            Sunday = _dataReader.GetValue(15).ToString(),
                            StartDate = _dataReader.GetValue(16).ToString(),
                            EndDate = _dataReader.GetValue(17).ToString()
                        });
                    }

                    _dataReader.Close();

                    _command = new SqlCommand("SELECT AgencyID, AgencyName, AgencyTimezone FROM Agency", connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = await _command.ExecuteReaderAsync();

                    while (await _dataReader.ReadAsync())
                    {
                        workingAgencies.Add(new Agency()
                        {
                            AgencyID = _dataReader.GetValue(0).ToString(),
                            AgencyName = _dataReader.GetValue(1).ToString(),
                            AgencyTimezone = _dataReader.GetValue(2).ToString()
                        });
                    }

                    _dataReader.Close();

                    _command = new SqlCommand("SELECT Date, ExceptionType, ServiceID FROM CalendarDate", connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = await _command.ExecuteReaderAsync();

                    while (await _dataReader.ReadAsync())
                    {
                        workingExceptions.Add(new Exception()
                        {
                            Date = _dataReader.GetValue(0).ToString(),
                            ExceptionType = _dataReader.GetValue(1).ToString(),
                            ServiceID = _dataReader.GetValue(2).ToString()
                        });
                    }

                    _dataReader.Close();

                    _command = new SqlCommand("SELECT StopID, StopName, StopTimezone FROM Stop", connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = await _command.ExecuteReaderAsync();

                    while (await _dataReader.ReadAsync())
                    {
                        workingStops.Add(new Stop()
                        {
                            StopID = _dataReader.GetValue(0).ToString(),
                            StopName = _dataReader.GetValue(1).ToString(),
                            StopTimezone = _dataReader.GetValue(2).ToString()
                        });
                    }

                    _dataReader.Close();
                }

                List<Departure> workingDepartures = new List<Departure>();

                if (workingDepartures.Count < count)
                {
                    foreach (Departure departure in tempDepartures)
                    {
                        string timeZone = "";

                        if (timeZone == "")
                        {
                            foreach (Stop stop in workingStops)
                            {
                                if (stop.StopID == departure.StopID)
                                {
                                    timeZone = stop.StopTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                if (agency.AgencyID == departure.AgencyID)
                                {
                                    timeZone = agency.AgencyTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }

                        if (timeZone == "")
                        {
                            timeZone = "Etc/UTC";
                        }

                        now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                        yesterdayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(-1).Year, now.AddDays(-1).Month.ToString("00"), now.AddDays(-1).Day.ToString("00")));

                        DateTime departureTime = new DateTime();

                        if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2]));
                        }
                        else
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(-1);
                        }

                        int startDate = yesterdayDate;
                        int endDate = yesterdayDate;

                        if (departure.StartDate != "")
                        {
                            startDate = Convert.ToInt32(departure.StartDate);
                        }

                        if (departure.EndDate != "")
                        {
                            endDate = Convert.ToInt32(departure.EndDate);
                        }

                        if (now.DayOfWeek == DayOfWeek.Monday)
                        {
                            if (departure.Sunday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            if (departure.Monday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            if (departure.Tuesday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Thursday)
                        {
                            if (departure.Wednesday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (departure.Thursday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (departure.Friday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (departure.Saturday == "1" && startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= yesterdayDate && endDate >= yesterdayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == yesterdayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                    }
                }

                workingDepartures = workingDepartures.Take(count).ToList();

                if (workingDepartures.Count < count)
                {
                    foreach (Departure departure in tempDepartures)
                    {
                        string timeZone = "";

                        if (timeZone == "")
                        {
                            foreach (Stop stop in workingStops)
                            {
                                if (stop.StopID == departure.StopID)
                                {
                                    timeZone = stop.StopTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                if (agency.AgencyID == departure.AgencyID)
                                {
                                    timeZone = agency.AgencyTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }

                        if (timeZone == "")
                        {
                            timeZone = "Etc/UTC";
                        }

                        now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                        todayDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.Year, now.Month.ToString("00"), now.Day.ToString("00")));

                        DateTime departureTime = new DateTime();

                        if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(3);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                        }
                        else
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2]));
                        }

                        int startDate = todayDate;
                        int endDate = todayDate;

                        if (departure.StartDate != "")
                        {
                            startDate = Convert.ToInt32(departure.StartDate);
                        }

                        if (departure.EndDate != "")
                        {
                            endDate = Convert.ToInt32(departure.EndDate);
                        }

                        if (now.DayOfWeek == DayOfWeek.Monday)
                        {
                            if (departure.Monday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            if (departure.Tuesday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            if (departure.Wednesday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Thursday)
                        {
                            if (departure.Thursday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (departure.Friday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (departure.Saturday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (departure.Sunday == "1" && startDate <= todayDate && endDate >= todayDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= todayDate && endDate >= todayDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == todayDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                    }
                }

                workingDepartures = workingDepartures.Take(count).ToList();

                if (workingDepartures.Count < count)
                {
                    foreach (Departure departure in tempDepartures)
                    {
                        string timeZone = "";

                        if (timeZone == "")
                        {
                            foreach (Stop stop in workingStops)
                            {
                                if (stop.StopID == departure.StopID)
                                {
                                    timeZone = stop.StopTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                if (agency.AgencyID == departure.AgencyID)
                                {
                                    timeZone = agency.AgencyTimezone;

                                    break;
                                }
                            }
                        }

                        if (timeZone == "")
                        {
                            foreach (Agency agency in workingAgencies)
                            {
                                timeZone = agency.AgencyTimezone;

                                break;
                            }
                        }

                        if (timeZone == "")
                        {
                            timeZone = "Etc/UTC";
                        }

                        now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone)));
                        tomorrowDate = Convert.ToInt32(string.Format("{0}{1}{2}", now.AddDays(1).Year, now.AddDays(1).Month.ToString("00"), now.AddDays(1).Day.ToString("00")));

                        DateTime departureTime = new DateTime();

                        if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 72)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 72, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(4);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 48)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 48, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(3);
                        }
                        else if (Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) >= 24)
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, Convert.ToInt32(departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0]) - 24, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(2);
                        }
                        else
                        {
                            departureTime = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", now.Year, now.Month, now.Day, departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[0], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[1], departure.DepartureTime.Split(new string[] { ":" }, StringSplitOptions.None)[2])).AddDays(1);
                        }

                        int startDate = tomorrowDate;
                        int endDate = tomorrowDate;

                        if (departure.StartDate != "")
                        {
                            startDate = Convert.ToInt32(departure.StartDate);
                        }

                        if (departure.EndDate != "")
                        {
                            endDate = Convert.ToInt32(departure.EndDate);
                        }

                        if (now.DayOfWeek == DayOfWeek.Monday)
                        {
                            if (departure.Tuesday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            if (departure.Wednesday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            if (departure.Thursday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Thursday)
                        {
                            if (departure.Friday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (departure.Saturday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (departure.Sunday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                        else if (now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (departure.Monday == "1" && startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool exclude = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "2")
                                    {
                                        exclude = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    exclude = true;
                                }

                                if (!exclude && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                            else if (startDate <= tomorrowDate && endDate >= tomorrowDate)
                            {
                                bool include = false;

                                foreach (Exception exception in workingExceptions)
                                {
                                    if (departure.ServiceID == exception.ServiceID && exception.Date == tomorrowDate.ToString() && exception.ExceptionType == "1")
                                    {
                                        include = true;

                                        break;
                                    }
                                }

                                if (departure.RouteShortName.ToLower().Contains(string.Format("_{0}", id.ToLower())) || departure.RouteShortName.ToLower().Contains(string.Format("->{0}", id.ToLower())))
                                {
                                    include = false;
                                }

                                if (include && departureTime >= now && departureTime <= now.AddHours(1))
                                {
                                    workingDepartures.Add(CreateWorkingDeparture(departure, departureTime));
                                }
                            }
                        }
                    }
                }

                workingDepartures = workingDepartures.Take(count).ToList();

                foreach (Departure departure in workingDepartures)
                {
                    string agencyName = "";

                    if (agencyName == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            if (agency.AgencyID == departure.AgencyID)
                            {
                                agencyName = agency.AgencyName;

                                break;
                            }
                        }
                    }

                    if (agencyName == "")
                    {
                        foreach (Agency agency in workingAgencies)
                        {
                            agencyName = agency.AgencyName;

                            break;
                        }
                    }

                    if (agencyName == "")
                    {
                        agencyName = "Unknown";
                    }

                    string destinationName = "";

                    if (destinationName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (departure.RouteShortName.Contains(string.Format("_{0}", stop.StopID)) || departure.RouteShortName.Contains(string.Format("->{0}", stop.StopID)))
                            {
                                destinationName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.TripHeadsign;
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.TripShortName;
                    }

                    if (destinationName == "")
                    {
                        destinationName = departure.RouteLongName;
                    }

                    if (destinationName == "")
                    {
                        destinationName = "Unknown";
                    }

                    string routeName = "";

                    if (routeName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (departure.RouteShortName.Contains(string.Format("_{0}", stop.StopID)) || departure.RouteShortName.Contains(string.Format("->{0}", stop.StopID)))
                            {
                                routeName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (routeName == "")
                    {
                        routeName = departure.RouteShortName;
                    }

                    if (routeName == "")
                    {
                        routeName = "Unknown";
                    }

                    string stopName = "";

                    if (stopName == "")
                    {
                        foreach (Stop stop in workingStops)
                        {
                            if (stop.StopID == departure.StopID)
                            {
                                stopName = stop.StopName;

                                break;
                            }
                        }
                    }

                    if (stopName == "")
                    {
                        stopName = "Unknown";
                    }

                    agencyName = agencyName.Trim();
                    destinationName = destinationName.Trim();
                    routeName = routeName.Trim();
                    stopName = stopName.Trim();

                    results.Add(new Service()
                    {
                        AgencyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(agencyName.ToLower()),
                        DepartureTime = departure.DepartureTime,
                        DestinationName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(destinationName.ToLower()),
                        RouteName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(routeName.ToLower()),
                        StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stopName.ToLower()),
                        TripID = departure.TripID
                    });
                }

                return results;
            }
            catch
            {
                return null;
            }
        }
    }
}