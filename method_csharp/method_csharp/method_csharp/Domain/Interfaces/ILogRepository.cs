using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Domain.Interfaces
{
    public interface ILogRepository
    {
        void SaveRunTime(int targilId, string method, double runTimeSeconds);
    }
}
