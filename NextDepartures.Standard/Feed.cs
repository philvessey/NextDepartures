using System;
using System.Data.SqlClient;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        private SqlCommand _command;
        private SqlDataReader _dataReader;
        private readonly string _connection;

        /// <summary>Creates a new feed.</summary>
        public Feed(string connection)
        {
            _connection = connection;
        }

        private Departure CreateWorkingDeparture(Departure tempDeparture, DateTime departureTime)
        {
            return new Departure()
            {
                AgencyID = tempDeparture.AgencyID,
                DepartureTime = departureTime.ToString(),
                EndDate = tempDeparture.EndDate,
                Friday = tempDeparture.Friday,
                Monday = tempDeparture.Monday,
                RouteLongName = tempDeparture.RouteLongName,
                RouteShortName = tempDeparture.RouteShortName,
                Saturday = tempDeparture.Saturday,
                ServiceID = tempDeparture.ServiceID,
                StartDate = tempDeparture.StartDate,
                StopID = tempDeparture.StopID,
                Sunday = tempDeparture.Sunday,
                Thursday = tempDeparture.Thursday,
                TripHeadsign = tempDeparture.TripHeadsign,
                TripID = tempDeparture.TripID,
                TripShortName = tempDeparture.TripShortName,
                Tuesday = tempDeparture.Tuesday,
                Wednesday = tempDeparture.Wednesday
            };
        }
    }

    class Agency
    {
        public string AgencyID { get; set; }
        public string AgencyName { get; set; }
        public string AgencyTimezone { get; set; }
    }

    class Departure
    {
        public string AgencyID { get; set; }
        public string DepartureTime { get; set; }
        public string EndDate { get; set; }
        public string Friday { get; set; }
        public string Monday { get; set; }
        public string RouteLongName { get; set; }
        public string RouteShortName { get; set; }
        public string Saturday { get; set; }
        public string ServiceID { get; set; }
        public string StartDate { get; set; }
        public string StopID { get; set; }
        public string Sunday { get; set; }
        public string Thursday { get; set; }
        public string TripHeadsign { get; set; }
        public string TripID { get; set; }
        public string TripShortName { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
    }

    class Exception
    {
        public string Date { get; set; }
        public string ExceptionType { get; set; }
        public string ServiceID { get; set; }
    }

    public class Service
    {
        public string AgencyName { get; set; }
        public string DepartureTime { get; set; }
        public string DestinationName { get; set; }
        public string RouteName { get; set; }
        public string StopName { get; set; }
        public string TripID { get; set; }
    }

    public class Stop
    {
        public string StopID { get; set; }
        public string StopName { get; set; }
        public string StopTimezone { get; set; }
    }
}