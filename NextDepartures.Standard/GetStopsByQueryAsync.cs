﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard
{
    public partial class Feed
    {
        /// <summary>Gets a list of stops by query.</summary>
        public async Task<List<Stop>> GetStopsByQueryAsync(string query, int count = 10)
        {
            List<Stop> results = new List<Stop>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();

                    _command = new SqlCommand(string.Format("SELECT StopID, StopName, StopTimezone FROM Stop WHERE LOWER(StopID) LIKE '%{0}%' OR LOWER(StopName) LIKE '%{0}%' AND StopLat != '0' AND StopLon != '0'", query.ToLower()), connection)
                    {
                        CommandTimeout = 0,
                        CommandType = CommandType.Text
                    };

                    _dataReader = await _command.ExecuteReaderAsync();

                    while (await _dataReader.ReadAsync())
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