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
            string[] invalidExpressions =
            {
                " ", //#1
                " & ", //#2
                " A ", //
                " * 2", //#3
                "1 / 0", //#4
                "B4 / 4", //#5
                " + + 2", //#6
                " - - 2", //#7
                "( + )", //#8
                " 2 + 3)", //#9
                " * (2 + 3)", //#10
                "1 / (2 - 2)", //#11
                "3 * ", //#12
                "3 + + 2" //#13
            };

            string[] testExpressions =
            {
                //  "expression", //expected value
                "2", // 2                                   #1
                "2 + 3", // 5                               #2
                "3 - 2", // 1                               #3
                "2 * 3", // 6                               #4
                "4 / 2", // 2                               #5
                "1 + 2 + 3", // 6                           #6
                "2 + 3 - 1", // 4                           #7
                "2 + 3 * 2", // 8                           #8
                "2 + 3 / 3", // 3                           #9
                "3 - 2 - 1", // 0                           #10
                "3 - 2 + 1", // 2                           #11
                "3 - 1 * 2", // 1                           #12
                "3 - 2 / 1", // 1                           #13
                "3 * 2 + 1", // 7                           #14
                "3 * 2 - 1", // 5                           #15
                "3 * 2 * 2", // 12                          #16
                "3 * 2 / 2", // 3                           #17
                "4 / 2 + 1", // 3                           #18
                "4 / 2 - 1", // 1                           #19
                "4 / 2 * 2", // 4                           #20
                "4 / 2 / 2", // 1                           #21
                "(2 + 3)", // 5                             #22
                "(3 - 2)", // 1                             #23
                "(2 * 3)", // 6                             #24
                "(4 / 2)", // 2                             #25
                "(1 + 2 + 3)", // 6                         #26
                "(1 + 2 - 3)", // 0                         #27
                "(1 + 2 * 3)", // 7                         #28
                "(1 + 2 / 2)", // 2                         #29
                "(1 - 2 + 3)", // 2                         #30
                "(3 - 2 - 1)", // 0                         #31
                "(3 - 2 * 1)", // 1                         #32
                "(3 - 2 / 2)", // 2                         #33
                "1 + (2 + 3)", // 6                         #34
                "10 - (2 + 3)", // 5                        #35
                "2 * (2 + 3)", // 10                        #36
                "10 / (2 + 3)", // 2                        #37
                "(10 + 2) + (10 + 1)", // 23                #38
                "(10 + 2) - (1 + 4)", // 7                  #39
                "(10 + 2) * (1 + 1)", // 24                 #40
                "(10 + 2) / (1 + 1)", // 6                  #41
                "(10 + (10 + 2))", // 22                    #42
                "(10 - (10 + 2))", // -2                    #43
                "(10 * (1 + 1))", // 20                     #44
                "(10 / (1 + 1))", // 5                      #45
                "(10 + (1 + 1)) + (10 + (1 + 1))", // 24    #46
                "(20 + (2 + 2)) - (10 + (1 + 1))", // 12    #47
                "(20 + (1 + 2)) * (1 + (1 + 0))", // 46     #48
                "(20 + (1 + 1)) / (9 + (1 + 1))", // 2      #49
                "A3 + 2", // 4                              #50

                //Super complex example:
                "(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2", // 28  #51
            };

            int[] testAnswers =
            {
                2, 5, 1, 6, 2, 6, 4, 8, 3, 0, 2, 1, 1, 7, 5, 12,
                3, 3, 1, 4, 1, 5, 1, 6, 2, 6, 0, 7, 2, 2, 0, 1,
                2, 6, 5, 10, 2, 23, 7, 24, 6, 22, -2, 20, 5, 24,
                12, 46, 2, 4, 28
            };


            for (int i = 0; i < testAnswers.Length; i++)
            {
                try
                {
                    if (!TestEvaluator(testAnswers[i], Evaluator.Evaluate(testExpressions[i], VarLookup)))
                        Console.WriteLine("FAIL - Test expression: " + testExpressions[i] + "\nExpected value: " + testAnswers[i] + " | Actual value: " + Evaluator.Evaluate(testExpressions[i], VarLookup));
                    else
                        Console.WriteLine("PASSED - Test expression: " + testExpressions[i]);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("FAIL - Test expression: " + testExpressions[i] + ". Threw ArgumentException: " + e.Message);
                }
            }
        
            foreach (string expression in invalidExpressions)
            {
                try
                {
                    Evaluator.Evaluate(expression, VarLookup);
                    Console.WriteLine("FAIL - Test with illegal expression: " + expression + " returned an actual integer value.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("PASSED - Expression \"" + expression + "\" passed with exception " + e.GetType() + ". Message: " + e.Message);
                }
            }
            Console.Read();
        }

        static int VarLookup(string val)
        {
            if (val.Equals("A3"))
            {
                return 2;
            }
            else throw new ArgumentException("Cannot execute expression - variable " + val + " is not defined.");
        }

    }
}
