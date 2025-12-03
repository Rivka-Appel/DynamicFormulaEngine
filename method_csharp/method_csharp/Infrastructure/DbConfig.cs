using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Infrastructure
{
    public static class DbConfig
    {
        // example
        private const string ConnectionStringInternal =
            "Server=DESKTOP-MG2PNTU\\SQLEXPRESS;Database=DynamicFormulaDB;Trusted_Connection=True;TrustServerCertificate=True";

        public static string ConnectionString => ConnectionStringInternal;
    }
}
