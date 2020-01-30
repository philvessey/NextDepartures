using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NextDepartures.Database.Extensions
{
    public static class SqlConnectionExtensions
    {
        public static async Task ExecuteCommandAsync(this SqlConnection connection, string sql, CommandType commandType = CommandType.Text, Action<SqlCommand> commandHandler = null)
        {
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandTimeout = 0,
                CommandType = commandType
            };

            commandHandler?.Invoke(command);

            await command.ExecuteNonQueryAsync();
            command.Dispose();
        }

        public static async Task ExecuteStoredProcedureFromTableInBatchesAsync<T>(this SqlConnection connection, string sql, DataTable table, IEnumerable<T> batch, Action<DataTable, T> step, int batchSize = 999999)
        {
            foreach (T batchElement in batch)
            {
                step(table, batchElement);

                if (table.Rows.Count > batchSize)
                {
                    await connection.ExecuteCommandAsync(sql, CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                    table.Rows.Clear();
                }
            }

            if (table.Rows.Count > 0)
            {
                await connection.ExecuteCommandAsync(sql, CommandType.StoredProcedure, (cmd) => cmd.Parameters.AddWithValue("@table", table));

                table.Rows.Clear();
            }
        }
    }
}