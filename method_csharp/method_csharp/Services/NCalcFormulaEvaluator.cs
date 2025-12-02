using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using method_csharp.Domain.Interfaces;
using method_csharp.Domain.Models;
using NCalc;

namespace method_csharp.Services
{
    public class NCalcFormulaEvaluator : IFormulaEvaluator
    {
        // Cache של אקספרשנים לפי מחרוזת מנורמלת
        private readonly Dictionary<string, Expression> _expressionCache = new();

        public double EvaluateNumeric(string expression, DataRecord data)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be null or empty.", nameof(expression));

            string normalized = NormalizeExpression(expression);

            Expression expr = GetOrCreateExpression(normalized);

            SetParameters(expr, data);

            object result = expr.Evaluate();
            return Convert.ToDouble(result);
        }

        public bool EvaluateCondition(string expression, DataRecord data)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Condition expression cannot be null or empty.", nameof(expression));

            string normalized = NormalizeExpression(expression);

            Expression expr = GetOrCreateExpression(normalized);

            SetParameters(expr, data);

            object result = expr.Evaluate();
            return Convert.ToBoolean(result);
        }

        private Expression GetOrCreateExpression(string normalizedExpression)
        {
            if (_expressionCache.TryGetValue(normalizedExpression, out var existing))
            {
                return existing;
            }

            var expr = new Expression(normalizedExpression);

            // פונקציות מותאמות: POWER, SQRT, ABS, LOG
            expr.EvaluateFunction += (name, args) =>
            {
                switch (name.ToUpperInvariant())
                {
                    case "POWER":
                        {
                            double x = Convert.ToDouble(args.Parameters[0].Evaluate());
                            double y = Convert.ToDouble(args.Parameters[1].Evaluate());
                            args.Result = Math.Pow(x, y);
                            break;
                        }
                    case "SQRT":
                        {
                            double x = Convert.ToDouble(args.Parameters[0].Evaluate());
                            args.Result = Math.Sqrt(x);
                            break;
                        }
                    case "ABS":
                        {
                            double x = Convert.ToDouble(args.Parameters[0].Evaluate());
                            args.Result = Math.Abs(x);
                            break;
                        }
                    case "LOG":
                        {
                            double x = Convert.ToDouble(args.Parameters[0].Evaluate());
                            args.Result = Math.Log(x);
                            break;
                        }
                }
            };

            _expressionCache[normalizedExpression] = expr;
            return expr;
        }

        private static void SetParameters(Expression expr, DataRecord data)
        {
            expr.Parameters["a"] = data.A;
            expr.Parameters["b"] = data.B;
            expr.Parameters["c"] = data.C;
            expr.Parameters["d"] = data.D;
        }

        private static string NormalizeExpression(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
                return expr;

            string s = expr;

            s = s.Replace("<>", "!=");

            // c = d -> c == d  (רק בין מזהים, לא ב-log(c = 1) וכו')
            s = Regex.Replace(s, @"(?<=\w)\s*=\s*(?=\w)", " == ");

            return s;
        }
    }
}
