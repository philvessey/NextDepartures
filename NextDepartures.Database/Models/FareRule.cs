namespace NextDepartures.Database.Models
{
    internal class FareRule
    {
        public string fare_id { get; set; }
        public string route_id { get; set; }
        public string origin_id { get; set; }
        public string destination_id { get; set; }
        public string contains_id { get; set; }
    }
}