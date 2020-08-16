using CommandLine;

namespace NextDepartures.Database.Models
{
    public class Option
    {
        [Option('d', "database", Required = true, HelpText = "Database connection string.")]
        public string Database { get; set; }

        [Option('g', "gtfs", Required = true, HelpText = "Path to GTFS data set .zip or directory.")]
        public string Gtfs { get; set; }
    }
}