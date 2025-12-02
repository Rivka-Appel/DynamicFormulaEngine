using System;
using method_csharp.Domain.Interfaces;
using method_csharp.Infrastructure;
using method_csharp.Infrastructure.Data;
using method_csharp.Services;
namespace method_csharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DynamicFormulaEngine C# – starting run...");

            string connectionString = DbConfig.ConnectionString;

            const string methodName = "C_SHARP";

            IDataRepository dataRepository = new DataRepository(connectionString);
            ITargilRepository targilRepository = new TargilRepository(connectionString);
            IResultRepository resultRepository = new ResultRepository(connectionString);
            ILogRepository logRepository = new LogRepository(connectionString);

            IFormulaEvaluator evaluator = new NCalcFormulaEvaluator();

            var runner = new FormulaRunner(
                dataRepository,
                targilRepository,
                resultRepository,
                logRepository,
                evaluator);

            runner.RunAll(methodName);

            Console.WriteLine("Run completed. Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
