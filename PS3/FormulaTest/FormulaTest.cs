using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTest
    {
        private string normalizer(string token)
        {
            return token.ToUpper();
        }
        private bool validate(string token)
        {
            if (token.Equals("A"))
                return true;
            else
                return false;
        }
        private double lookup(string var)
        {
            if (var.Equals("A"))
                return 2d;
            else
                return 3d;
        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            Formula addTest = new Formula("2 + 3");
            Formula subTest = new Formula("2 - 3");
            Formula multTest = new Formula("2 * 3");
            Formula divTest = new Formula("2 / 3");
            Formula paraTest = new Formula("(2 + 3)");
            Formula varTest = new Formula("A_2 + 2");
            Formula manyOperations = new Formula("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2");

            Assert.AreEqual("2 + 3", addTest.ToString());
            Assert.AreEqual("2 - 3", subTest.ToString());
            Assert.AreEqual("2 * 3", multTest.ToString());
            Assert.AreEqual("2 / 3", divTest.ToString());
            Assert.AreEqual("(2 + 3)", paraTest.ToString());
            Assert.AreEqual("A_2 + 2", varTest.ToString());
            Assert.AreEqual("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2", manyOperations.ToString());
        }

        [TestMethod]
        public void NormalizerConstructorTest()
        {
            Formula normalizerTestWithLetters = new Formula("a2 + b3", normalizer, null);
            Formula normalizerTestManyLetters = new Formula("a3 + (b4) * ct_3", normalizer, null);
            Formula normalizerTestNoLetters = new Formula("2 + 3", normalizer, null);
            Assert.AreEqual("A2 + B3", normalizerTestWithLetters.ToString());
            Assert.AreEqual("A3 + (B4) * CT_3", normalizerTestManyLetters.ToString());
            Assert.AreEqual("2 + 3", normalizerTestNoLetters.ToString());
        }

        [TestMethod]
        public void ValidatorConstructorTestPass()
        {
            Formula validatorTestPass = new Formula("A + 2", null, validate);
            Assert.AreEqual("A + 2", validatorTestPass.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ValidatorConstructorTestThrowsException()
        {
            Formula validatorTestFail = new Formula("B + 2", null, validate);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest()
        {
            Formula malformedExp = new Formula("(2");
        }

        [TestMethod]
        public void EvaluateTestNoLookup()
        {
            Formula addTest = new Formula("2 + 3");
            Formula subTest = new Formula("2 - 3");
            Formula multTest = new Formula("2 * 3");
            Formula divTest = new Formula("4 / 8");
            Formula paraTest = new Formula("(2 + 3)");
            Formula manyOperations = new Formula("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2");

            Assert.AreEqual(5d, addTest.Evaluate(null));
            Assert.AreEqual(-1d, subTest.Evaluate(null));
            Assert.AreEqual(6d, multTest.Evaluate(null));
            Assert.AreEqual(0.5d, divTest.Evaluate(null));
            Assert.AreEqual(5d, paraTest.Evaluate(null));
            Assert.AreEqual(28d, manyOperations.Evaluate(null));
        }

        public void EvaluateTestWithLookup()
        {
            Formula lookupTest = new Formula("A + 3");
            Formula lookupTest2 = new Formula("C + 4");

            Assert.AreEqual(5d, lookupTest.Evaluate(lookup));
            Assert.AreEqual(7d, lookupTest.Evaluate(lookup));
        }

        [TestMethod]
        public void EqualityTest()
        {
            Formula equalTest = new Formula("2 + 3");
            Formula equalTest2 = new Formula("2 + 3");
            Formula equalTest3 = new Formula("2 + x4", normalizer, null);
            Formula equalTest4 = new Formula("2 + X4");

            Assert.IsTrue(equalTest.Equals(equalTest2));
            Assert.IsTrue(equalTest == equalTest2);
            Assert.IsTrue(equalTest3.Equals(equalTest4));
            Assert.IsTrue(equalTest3 == equalTest4);
        }

        [TestMethod]
        public void GetVariablesTest()
        {
            string[] vars = { "x2", "t_2", "Y56_5t" };
            Formula getVarsTest = new Formula("x2 + t_2 / Y56_5t");
            int i = 0;
            foreach (string var in getVarsTest.GetVariables())
            {
                Assert.AreEqual(vars[i], var);
                i++;
            }
        }

        [TestMethod]
        public void NotEqualsTest()
        {
            Formula equalTest = new Formula("2 + 3");
            Formula equalTest2 = new Formula("2 + 5");
            Formula equalTest3 = new Formula("2 + x4");
            Formula equalTest4 = new Formula("2 + X4");

            Assert.IsTrue(equalTest != equalTest2);
            Assert.IsTrue(equalTest3 != equalTest4);
        }

        [TestMethod]
        public void GetHashTest()
        {
            Formula hashTest = new Formula("2 + 4");

            Assert.AreEqual("2 + 4".GetHashCode(), hashTest.GetHashCode());
        }

        [TestMethod]
        public void FormulaErrorTest()
        {
            Formula errorTest1 = new Formula("2 / 0");
            Formula errorTest2 = new Formula("(2 + 2) / (2 - 2)");
            Formula errorMessageTest = new Formula("1 - 0 / 0");

            Assert.IsTrue(errorTest1.Evaluate(null) is FormulaError);
            Assert.IsTrue(errorTest2.Evaluate(null) is FormulaError);
            Assert.AreEqual("Cannot execute expression - dividing by 0!", ((FormulaError) errorMessageTest.Evaluate(null)).Reason);
        }

        [TestMethod]
        public void PrivateMethodTests()
        {
            Formula privateTest = new Formula("2 + 3");
            PrivateObject privateAccessor = new PrivateObject(privateTest);

            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "+"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "-"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "*"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "/"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "("));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", ")"));
            Assert.AreEqual("Num/Var", privateAccessor.Invoke("TokenValidator", "3"));
            Assert.AreEqual("Num/Var", privateAccessor.Invoke("TokenValidator", "x4"));

            Stack<string> operands = new Stack<string>();
            operands.Push("2");
            operands.Push("3");
            Stack<string> operators = new Stack<string>();
            operators.Push("+");
            privateAccessor.Invoke("Add", operators, operands);
            Assert.AreEqual("5", operands.Pop());

            operands.Push("3");
            operands.Push("2");
            operators.Push("-");
            privateAccessor.Invoke("Subtract", operators, operands);
            Assert.AreEqual("1", operands.Pop());
        }
    }
}
