namespace NextDepartures.Database.Models
{
    internal class FareAttribute
    {
        public string fare_id { get; set; }
        public string price { get; set; }
        public string currency_type { get; set; }
        public string payment_method { get; set; }
        public string transfers { get; set; }
        public string agency_id { get; set; }
        public string transfer_duration { get; set; }
    }
}