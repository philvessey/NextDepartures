using System;
using System.Data;
using System.Data.SqlClient;
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
    }
}
