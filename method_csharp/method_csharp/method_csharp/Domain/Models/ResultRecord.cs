using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Domain.Models
{
    public class ResultRecord
    {
        public int DataId { get; set; }       
        public int TargilId { get; set; }     
        public string Method { get; set; } = "";  
        public double? Result { get; set; }   
    }
}
