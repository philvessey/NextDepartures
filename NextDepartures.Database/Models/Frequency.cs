namespace NextDepartures.Database.Models
{
    internal class Frequency
    {
        public string trip_id { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string headway_secs { get; set; }
        public string exact_times { get; set; }
    }
}