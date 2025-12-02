using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Infrastructure.Data
{
    public class TargilRepository : ITargilRepository
    {
        private readonly string _connectionString;

        public TargilRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<TargilRecord> GetAllTargilim()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
SELECT targil_id, targil, tnai, targil_false
FROM t_targil";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return new TargilRecord
                {
                    TargilId = reader.GetInt32(0),
                    Targil = reader.GetString(1),
                    Tnai = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TargilFalse = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }
        }

        IEnumerable<TargilRecord> ITargilRepository.GetAllTargilim()
        {
            throw new NotImplementedException();
        }
    }
}
