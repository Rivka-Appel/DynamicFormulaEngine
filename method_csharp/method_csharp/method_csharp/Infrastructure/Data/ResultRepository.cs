using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Infrastructure.Data
{
    public class ResultRepository : IResultRepository
    {
        private readonly string _connectionString;

        public ResultRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveResults(IEnumerable<ResultRecord> results)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            const string sql = @"
INSERT INTO t_results (data_id, targil_id, method, result)
VALUES (@data_id, @targil_id, @method, @result);";

            using var command = new SqlCommand(sql, connection, transaction);

            var pDataId = command.Parameters.Add("@data_id", SqlDbType.Int);
            var pTargilId = command.Parameters.Add("@targil_id", SqlDbType.Int);
            var pMethod = command.Parameters.Add("@method", SqlDbType.VarChar, 50);
            var pResult = command.Parameters.Add("@result", SqlDbType.Float);

            try
            {
                foreach (var r in results)
                {
                    pDataId.Value = r.DataId;
                    pTargilId.Value = r.TargilId;
                    pMethod.Value = r.Method;
                    pResult.Value = (object?)r.Result ?? DBNull.Value;

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
