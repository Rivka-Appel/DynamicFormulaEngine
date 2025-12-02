using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using method_csharp.Domain.Interfaces;

namespace method_csharp.Infrastructure.Data
{

    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<DataRecord> GetAllData()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            const string sql = "SELECT data_id, a, b, c, d FROM t_data";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

            while (reader.Read())
            {
                yield return new DataRecord
                {
                    DataId = reader.GetInt32(0),
                    A = reader.GetDouble(1),
                    B = reader.GetDouble(2),
                    C = reader.GetDouble(3),
                    D = reader.GetDouble(4)
                };
            }
        }
    }
}
