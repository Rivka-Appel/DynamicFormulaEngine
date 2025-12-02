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
            var table = new DataTable();
            table.Columns.Add("data_id", typeof(int));
            table.Columns.Add("targil_id", typeof(int));
            table.Columns.Add("method", typeof(string));
            table.Columns.Add("result", typeof(double));

            foreach (var r in results)
            {
                var row = table.NewRow();
                row["data_id"] = r.DataId;
                row["targil_id"] = r.TargilId;
                row["method"] = r.Method;
                row["result"] = r.Result.HasValue
                    ? r.Result.Value
                    : DBNull.Value;
                table.Rows.Add(row);
            }

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = "t_results"
            };

            bulkCopy.ColumnMappings.Add("data_id", "data_id");
            bulkCopy.ColumnMappings.Add("targil_id", "targil_id");
            bulkCopy.ColumnMappings.Add("method", "method");
            bulkCopy.ColumnMappings.Add("result", "result");

            bulkCopy.WriteToServer(table);
        }

    }
}
