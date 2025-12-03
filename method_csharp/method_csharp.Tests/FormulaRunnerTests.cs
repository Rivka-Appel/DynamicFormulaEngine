using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using method_csharp.Services;

namespace method_csharp.Tests
{
    
    [TestClass]

    /*
    מנוע החישוב(IFormulaEvaluator):
    בדיקות שבודקות חישוב נוסחאות פשוטות ומורכבות - חיבור, חזקה ושורש,
    וכן תנאי השוואה בין שדות.
    */
    public class FormulaRunnerTests
    {
        private const string TestMethodName = "C_SHARP_TEST";

        private IFormulaEvaluator CreateEvaluator()
        {
            return new NCalcFormulaEvaluator();
        }

        #region Fakes (Repositories בזיכרון)

        private class InMemoryDataRepository : IDataRepository
        {
            private readonly List<DataRecord> _data;

            public InMemoryDataRepository(IEnumerable<DataRecord> data)
            {
                _data = data.ToList();
            }

            public IEnumerable<DataRecord> GetAllData() => _data;
        }

        private class InMemoryTargilRepository : ITargilRepository
        {
            private readonly List<TargilRecord> _targilim;

            public InMemoryTargilRepository(IEnumerable<TargilRecord> targilim)
            {
                _targilim = targilim.ToList();
            }

            public IEnumerable<TargilRecord> GetAllTargilim() => _targilim;
        }

        private class InMemoryResultRepository : IResultRepository
        {
            public readonly List<ResultRecord> SavedResults = new List<ResultRecord>();

            public bool DeleteCalled { get; private set; }
            public string? DeletedMethod { get; private set; }

            public void SaveResults(IEnumerable<ResultRecord> results)
            {
                SavedResults.AddRange(results);
            }

            public void DeleteResultsForMethod(string method)
            {
                DeleteCalled = true;
                DeletedMethod = method;
            }
        }

        private class InMemoryLogRepository : ILogRepository
        {
            public readonly List<(int TargilId, string Method, double Time)> Logs
                = new List<(int, string, double)>();

            public void SaveRunTime(int targilId, string method, double seconds)
            {
                Logs.Add((targilId, method, seconds));
            }
        }

        #endregion

        [TestMethod]
        public void RunAll_NoCondition_UsesTargilForAllRows()
        {
            // Arrange
            var data = new[]
            {
                new DataRecord { DataId = 1, A = 1, B = 2, C = 0, D = 0 },
                new DataRecord { DataId = 2, A = 3, B = 4, C = 0, D = 0 }
            };

            var targilim = new[]
            {
                new TargilRecord
                {
                    TargilId = 1,
                    Targil = "a + b",
                    Tnai = null,
                    TargilFalse = null
                }
            };

            var dataRepo = new InMemoryDataRepository(data);
            var targilRepo = new InMemoryTargilRepository(targilim);
            var resultRepo = new InMemoryResultRepository();
            var logRepo = new InMemoryLogRepository();
            var evaluator = CreateEvaluator();

            var runner = new FormulaRunner(
                dataRepo,
                targilRepo,
                resultRepo,
                logRepo,
                evaluator);

            // Act
            runner.RunAll(TestMethodName);

            // Assert
            Assert.AreEqual(2, resultRepo.SavedResults.Count, "מצופה 2 תוצאות");

            var r1 = resultRepo.SavedResults.Single(r => r.DataId == 1);
            var r2 = resultRepo.SavedResults.Single(r => r.DataId == 2);

            Assert.AreEqual(1, r1.TargilId);
            Assert.AreEqual(1, r2.TargilId);
            Assert.AreEqual(TestMethodName, r1.Method);
            Assert.AreEqual(TestMethodName, r2.Method);

            Assert.AreEqual(3.0, r1.Result ?? 0.0, 1e-9, "לרשומה 1 אמור לצאת 3");
            Assert.AreEqual(7.0, r2.Result ?? 0.0, 1e-9, "לרשומה 2 אמור לצאת 7");
        }

        [TestMethod]
        public void RunAll_WithCondition_UsesTrueAndFalseExpressions()
        {
            // Arrange
            var data = new[]
            {
                new DataRecord { DataId = 1, A = 10, B = 5, C = 0, D = 0 }, // a > 5 -> true
                new DataRecord { DataId = 2, A = 2,  B = 8, C = 0, D = 0 }  // a > 5 -> false
            };

            var targilim = new[]
            {
                new TargilRecord
                {
                    TargilId = 2,
                    Targil = "b * 2",      
                    Tnai = "a > 5",
                    TargilFalse = "b / 2" 
                }
            };

            var dataRepo = new InMemoryDataRepository(data);
            var targilRepo = new InMemoryTargilRepository(targilim);
            var resultRepo = new InMemoryResultRepository();
            var logRepo = new InMemoryLogRepository();
            var evaluator = CreateEvaluator();

            var runner = new FormulaRunner(
                dataRepo,
                targilRepo,
                resultRepo,
                logRepo,
                evaluator);

            // Act
            runner.RunAll(TestMethodName);

            // Assert
            Assert.AreEqual(2, resultRepo.SavedResults.Count, "מצופה 2 תוצאות");

            var r1 = resultRepo.SavedResults.Single(r => r.DataId == 1);
            var r2 = resultRepo.SavedResults.Single(r => r.DataId == 2);

            Assert.AreEqual(10.0, r1.Result ?? 0.0, 1e-9, "לרשומה 1 אמור לצאת 10");

            Assert.AreEqual(4.0, r2.Result ?? 0.0, 1e-9, "לרשומה 2 אמור לצאת 4");
        }

        [TestMethod]
        public void RunAll_CallsDeleteResultsForMethod_BeforeSaving()
        {
            // Arrange
            var data = new[]
            {
                new DataRecord { DataId = 1, A = 1, B = 1, C = 0, D = 0 }
            };

            var targilim = new[]
            {
                new TargilRecord
                {
                    TargilId = 3,
                    Targil = "a + b",
                    Tnai = null,
                    TargilFalse = null
                }
            };

            var dataRepo = new InMemoryDataRepository(data);
            var targilRepo = new InMemoryTargilRepository(targilim);
            var resultRepo = new InMemoryResultRepository();
            var logRepo = new InMemoryLogRepository();
            var evaluator = CreateEvaluator();

            var runner = new FormulaRunner(
                dataRepo,
                targilRepo,
                resultRepo,
                logRepo,
                evaluator);

            // Act
            runner.RunAll(TestMethodName);

            // Assert
            Assert.IsTrue(resultRepo.DeleteCalled, "מצופה ש-DeleteResultsForMethod ייקרא לפני שמירה");
            Assert.AreEqual(TestMethodName, resultRepo.DeletedMethod, "מצופה שמחיקת התוצאות תהיה עבור השיטה הנכונה");
        }
    }
}
