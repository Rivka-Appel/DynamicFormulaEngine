using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Domain.Interfaces
{
    public interface IFormulaEvaluator
    {
        double EvaluateNumeric(string expression, DataRecord data);
        bool EvaluateCondition(string expression, DataRecord data);
    }
}
