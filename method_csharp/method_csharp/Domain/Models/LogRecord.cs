using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Domain.Models
{
    public class LogRecord
    {
        public int TargilId { get; set; }     
        public string Method { get; set; } = ""; 
        public double RunTime { get; set; }   
    }
}
