using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Domain.Interfaces
{
    public interface IResultRepository
    {
        void SaveResults(IEnumerable<ResultRecord> results);

        void DeleteResultsForMethod(string method);
    }
}
