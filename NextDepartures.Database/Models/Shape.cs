﻿namespace NextDepartures.Database.Models
{
    internal class Shape
    {
        public string shape_id { get; set; }
        public string shape_pt_lat { get; set; }
        public string shape_pt_lon { get; set; }
        public string shape_pt_sequence { get; set; }
        public string shape_dist_traveled { get; set; }
    }
}