namespace NextDepartures.Database.Models
{
    internal class Pathway
    {
        public string pathway_id { get; set; }
        public string from_stop_id { get; set; }
        public string to_stop_id { get; set; }
        public string pathway_mode { get; set; }
        public string is_bidirectional { get; set; }
        public string length { get; set; }
        public string traversal_time { get; set; }
        public string stair_count { get; set; }
        public string max_slope { get; set; }
        public string min_width { get; set; }
        public string signposted_as { get; set; }
        public string reversed_signposted_as { get; set; }
    }
}