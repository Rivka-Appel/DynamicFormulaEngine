using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace method_csharp.Services
{

    public class FormulaRunner
    {
        private readonly IDataRepository _dataRepository;
        private readonly ITargilRepository _targilRepository;
        private readonly IResultRepository _resultRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFormulaEvaluator _evaluator;

        public FormulaRunner(
            IDataRepository dataRepository,
            ITargilRepository targilRepository,
            IResultRepository resultRepository,
            ILogRepository logRepository,
            IFormulaEvaluator evaluator)
        {
            _dataRepository = dataRepository;
            _targilRepository = targilRepository;
            _resultRepository = resultRepository;
            _logRepository = logRepository;
            _evaluator = evaluator;
        }

        public void RunAll(string methodName)
        {
            _resultRepository.DeleteResultsForMethod(methodName);

            var data = _dataRepository.GetAllData().ToList();
            var targilim = _targilRepository.GetAllTargilim().ToList();

            Console.WriteLine($"Loaded {data.Count} data rows and {targilim.Count} formulas.");

            foreach (var targil in targilim)
            {
                Console.WriteLine($"Running targil #{targil.TargilId} ...");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<ResultRecord>(data.Count);

                foreach (var row in data)
                {
                    string expressionToUse;

                    if (!string.IsNullOrWhiteSpace(targil.Tnai))
                    {
                        bool condition = _evaluator.EvaluateCondition(targil.Tnai, row);

                        if (condition)
                        {
                            expressionToUse = targil.Targil;  
                        }
                        else
                        {
                            expressionToUse = string.IsNullOrWhiteSpace(targil.TargilFalse)
                                ? targil.Targil
                                : targil.TargilFalse!;
                        }
                    }
                    else
                    {
                        expressionToUse = targil.Targil;
                    }

                    double value = _evaluator.EvaluateNumeric(expressionToUse, row);

                    results.Add(new ResultRecord
                    {
                        DataId = row.DataId,
                        TargilId = targil.TargilId,
                        Method = methodName,
                        Result = value
                    });
                }

                stopwatch.Stop();
                double seconds = stopwatch.Elapsed.TotalSeconds;

                Console.WriteLine($"Targil {targil.TargilId} finished in {seconds:F2} seconds.");

                _resultRepository.SaveResults(results);
                _logRepository.SaveRunTime(targil.TargilId, methodName, seconds);
            }

        }
    }

}
