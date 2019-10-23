using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of stops by location.</summary>
        public List<Stop> GetStopsByLocation(double minLon, double minLat, double maxLon, double maxLat, int count = 10)
        {
            List<Stop> results = new List<Stop>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();

                    _command = new SqlCommand(string.Format("SELECT StopID, StopName, StopTimezone FROM Stop WHERE CAST(StopLat as REAL) >= {0} AND CAST(StopLat as REAL) <= {1} AND CAST(StopLon as REAL) >= {2} AND CAST(StopLon as REAL) <= {3} AND StopLat != '0' AND StopLon != '0'", minLat, maxLat, minLon, maxLon), connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = _command.ExecuteReader();

                    while (_dataReader.Read())
                    {
                        results.Add(new Stop()
                        {
                            StopID = _dataReader.GetValue(0).ToString(),
                            StopName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_dataReader.GetValue(1).ToString().ToLower()),
                            StopTimezone = _dataReader.GetValue(2).ToString()
                        });
                    }

                    _dataReader.Close();
                }

                results = results.Take(count).ToList();

                return results;
            }
            catch
            {
                return null;
            }
        }
    }
}