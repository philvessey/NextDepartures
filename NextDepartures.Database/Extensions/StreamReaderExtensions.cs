using CsvHelper;
using System.IO;

namespace NextDepartures.Database.Extensions
{
    public static class StreamReaderExtensions
    {
        public static CsvReader GetCsvReader(this StreamReader streamReader)
        {
            CsvReader csvReader = new CsvReader(streamReader);

            csvReader.Configuration.BadDataFound = null;
            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Replace(" ", "");

            return csvReader;
        }
    }
}