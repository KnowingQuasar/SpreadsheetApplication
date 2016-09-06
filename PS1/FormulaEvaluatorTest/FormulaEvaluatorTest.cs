using System;
using FormulaEvaluator;

namespace FormulaEvaluatorTest
{
    class FormulaEvaluatorTest
    {
        private static bool TestEvaluator(int expected, int actual)
        {
            if (expected == actual)
                return true;
            return false;
        }
        static void Main(string[] args)
        {
            string result;
            string[] testExpressions =
            {
                //  "expression", //expected value
                "2", // 2
                "2 + 3", // 5
                "3 - 2", // 1
                "2 * 3", // 6
                "4 / 2", // 2
                "1 + 2 + 3", // 6
                "2 + 3 - 1", // 4
                "2 + 3 * 2", // 8
                "2 + 3 / 3", // 3
                "3 - 2 - 1", // 0
                "3 - 2 + 1", // 2
                "3 - 1 * 2", // 1
                "3 - 2 / 1", // 1
                "3 * 2 + 1", // 7
                "3 * 2 - 1", // 5
                "3 * 2 * 2", // 12
                "3 * 2 / 2", // 3
                "4 / 2 + 1", // 3
                "4 / 2 - 1", // 1
                "4 / 2 * 2", // 4
                "4 / 2 / 2", // 1
                "(2 + 3)", // 5
                "(3 - 2)", // 1
                "(2 * 3)", // 6
                "(4 / 2)", // 2
                "(1 + 2 + 3)", // 6
                "(1 + 2 - 3)", // 0
                "(1 + 2 * 3)", // 7
                "(1 + 2 / 2)", // 2
                "(1 - 2 + 3)", // 2
                "(3 - 2 - 1)", // 0
                "(3 - 2 * 1)", // 1
                "(3 - 2 / 2)", // 2
                "1 + (2 + 3)", // 6
                "10 - (2 + 3)", // 5
                "2 * (2 + 3)", // 10
                "10 / (2 + 3)", // 2
            };
            TestEvaluator(2, Evaluator.Evaluate("(2)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("2 + 3", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("2 - 3", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.WriteLine(Evaluator.Evaluate("(0)", VarLookup));
            Console.Read();
        }

        static int VarLookup(string val)
        {
            if (val.Equals("A3"))
            {
                return 2;
            }
            else return 5;
        }

    }
}
