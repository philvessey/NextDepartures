namespace NextDepartures.Database.Models
{
    internal class FeedInfo
    {
        public string feed_publisher_name { get; set; }
        public string feed_publisher_url { get; set; }
        public string feed_lang { get; set; }
        public string feed_start_date { get; set; }
        public string feed_end_date { get; set; }
        public string feed_version { get; set; }
        public string feed_contact_email { get; set; }
        public string feed_contact_url { get; set; }
    }
}