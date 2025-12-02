using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Services
{
    public class ComputeFormulaEvaluator : IFormulaEvaluator
    {
        private readonly DataTable _table;
        private readonly DataRow _row;

        public ComputeFormulaEvaluator()
        {
            _table = new DataTable("FormulaTable");

            _table.Columns.Add("a", typeof(double));
            _table.Columns.Add("b", typeof(double));
            _table.Columns.Add("c", typeof(double));
            _table.Columns.Add("d", typeof(double));

            _row = _table.NewRow();
            _table.Rows.Add(_row);
        }

        private void SetRowValues(DataRecord data)
        {
            _row["a"] = data.A;
            _row["b"] = data.B;
            _row["c"] = data.C;
            _row["d"] = data.D;
        }

        public double EvaluateNumeric(string expression, DataRecord data)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Expression cannot be null or empty.", nameof(expression));
            }

            SetRowValues(data);

            object result = _table.Compute(expression, string.Empty);

            return Convert.ToDouble(result);
        }

        public bool EvaluateCondition(string expression, DataRecord data)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Condition expression cannot be null or empty.", nameof(expression));
            }

            SetRowValues(data);

            object result = _table.Compute(expression, string.Empty);

            return Convert.ToBoolean(result);
        }
    }
}
