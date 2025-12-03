using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using method_csharp.Domain.Models;
using method_csharp.Services;
using method_csharp.Domain.Interfaces;

namespace method_csharp.Tests
{
    [TestClass]
/*
    מנגנון הריצה(FormulaRunner):
    בדיקות עם Repositories :
   • חישוב נוסחה על מספר רשומות ללא תנאי.
   • בחירת נוסחה מתאימה לפי תנאי(tnai / targil_false).
   • קריאה למחיקת תוצאות קודמות עבור השיטה לפני שמירה חדשה.
*/
    public class FormulaEvaluatorTests
    {
        private IFormulaEvaluator CreateEvaluator()
        {
            return new NCalcFormulaEvaluator();
        }

        [TestMethod]
        public void EvaluateNumeric_SimpleAddition_ReturnsCorrectResult()
        {
            // Arrange
            var evaluator = CreateEvaluator();
            var data = new DataRecord
            {
                DataId = 1,
                A = 2,
                B = 3,
                C = 0,
                D = 0
            };

            string expression = "a + b";

            // Act
            double result = evaluator.EvaluateNumeric(expression, data);

            // Assert
            Assert.AreEqual(5.0, result, 1e-9, "a + b אמור להחזיר 5");
        }

        [TestMethod]
        public void EvaluateNumeric_Pythagoras_PowerAndSqrt_ReturnsCorrectResult()
        {
            // Arrange
            var evaluator = CreateEvaluator();
            var data = new DataRecord
            {
                DataId = 1,
                A = 3,
                B = 4,
                C = 0,
                D = 0
            };

            string expression = "SQRT(POWER(a, 2) + POWER(b, 2))";

            // Act
            double result = evaluator.EvaluateNumeric(expression, data);

            // Assert
            Assert.AreEqual(5.0, result, 1e-9, "SQRT(POWER(a,2)+POWER(b,2)) אמור להחזיר 5");
        }

        [TestMethod]
        public void EvaluateCondition_EqualityCondition_TrueWhenCEqualsD()
        {
            // Arrange
            var evaluator = CreateEvaluator();
            var data = new DataRecord
            {
                DataId = 1,
                A = 0,
                B = 0,
                C = 10,
                D = 10
            };

            string condition = "c = d";

            // Act
            bool result = evaluator.EvaluateCondition(condition, data);

            // Assert
            Assert.IsTrue(result, "התנאי c = d אמור להיות true כאשר C ו-D שווים");
        }
    }
}
