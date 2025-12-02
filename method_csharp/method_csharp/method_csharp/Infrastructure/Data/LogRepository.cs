using method_csharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Infrastructure.Data
{

    public class LogRepository : ILogRepository
    {
        private readonly string _connectionString;

        public LogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveRunTime(int targilId, string method, double runTimeSeconds)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
INSERT INTO t_log (targil_id, method, run_time)
VALUES (@targil_id, @method, @run_time);";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@targil_id", targilId);
            command.Parameters.AddWithValue("@method", method);
            command.Parameters.AddWithValue("@run_time", runTimeSeconds);

            command.ExecuteNonQuery();
        }
    }
}
