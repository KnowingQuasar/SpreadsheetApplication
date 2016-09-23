using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace UnitTestProject1
{
    /// <summary>
    /// Test classes for the Formula class. The ToString() method is tested throughout each test and does not have its own test.
    /// </summary>
    [TestClass]
    public class FormulaTest
    {
        /// <summary>
        /// Test normalize function that converts the token to uppercase.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns an upppercase version of the parameter token.</returns>
        private string normalizer(string token)
        {
            return token.ToUpper();
        }

        /// <summary>
        /// Test validator function that returns true if the token is an "A" and false if it is anything else.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns a boolean value representing the result of validating the token parameter.</returns>
        private bool validate(string token)
        {
            if (token.Equals("A"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Test lookup function. Returns 2 if the token is an "A" and 3 if it is anything else.
        /// </summary>
        /// <param name="var"></param>
        /// <returns>Returns the double value of the looked up value for the var parameter.</returns>
        private double lookup(string var)
        {
            if (var.Equals("A"))
                return 2d;
            else
                return 3d;
        }

        /// <summary>
        /// Tests the constructor that takes a string only.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorTest()
        {
            //Tests creating a formula with an expression containing addition
            Formula addTest = new Formula("2 + 3");
            //Tests creating a formula with an expression containing subtraction
            Formula subTest = new Formula("2 - 3");
            //Tests creating a formula with an expression containing multiplication
            Formula multTest = new Formula("2 * 3");
            //Tests creating a formula with an expression containing addition division
            Formula divTest = new Formula("2 / 3");
            //Tests creating a formula with an expression containing parantheses
            Formula paraTest = new Formula("(2 + 3)");
            //Tests creating a formula with an expression containing variables
            Formula varTest = new Formula("A_2 + 2");
            //Tests creating a formula with an expression containing many different operations. Stress test.
            Formula manyOperations = new Formula("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2");

            //Assertions to validate that the constructor created the formula correctly. Also tests the ToString() method incidentally.
            Assert.AreEqual("2 + 3", addTest.ToString());
            Assert.AreEqual("2 - 3", subTest.ToString());
            Assert.AreEqual("2 * 3", multTest.ToString());
            Assert.AreEqual("2 / 3", divTest.ToString());
            Assert.AreEqual("(2 + 3)", paraTest.ToString());
            Assert.AreEqual("A_2 + 2", varTest.ToString());
            Assert.AreEqual("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2", manyOperations.ToString());
        }

        /// <summary>
        /// Tests the Formula constructor with a string and a normalize function, but no validator
        /// </summary>
        [TestMethod]
        public void NormalizerConstructorTest()
        {
            //Tests with only a few variables to make uppercase
            Formula normalizerTestWithLetters = new Formula("a2 + b3", normalizer, null);
            //Tests with several different operators and several variables
            Formula normalizerTestManyLetters = new Formula("a3 + (b4) * ct_3", normalizer, null);
            //Tests with an expression that contains no variables. The normalize function should do nothing here.
            Formula normalizerTestNoLetters = new Formula("2 + 3", normalizer, null);

            //Assertions to validate that the Formula constructor passes variables to the normalize function correctly (or that it does nothing in the case of no variables being present).
            Assert.AreEqual("A2 + B3", normalizerTestWithLetters.ToString());
            Assert.AreEqual("A3 + (B4) * CT_3", normalizerTestManyLetters.ToString());
            Assert.AreEqual("2 + 3", normalizerTestNoLetters.ToString());
        }

        /// <summary>
        /// Tests the Formula constructor with a string containing valid variables and a validate function, but no normalize function. 
        /// </summary>
        [TestMethod]
        public void ValidatorConstructorTestPass()
        {
            Formula validatorTestPass = new Formula("A + 2", null, validate);
            Assert.AreEqual("A + 2", validatorTestPass.ToString());
        }

        /// <summary>
        /// Tests the formula constructor with string containing valid variables, a validate function, and a normalize function.
        /// </summary>
        [TestMethod]
        public void NormalizerAndValidatorConstructorTestPass()
        {
            Formula normalizerAndValidatorTest = new Formula("a + 2", normalizer, validate);
            Assert.AreEqual("A + 2", normalizerAndValidatorTest.ToString());
        }

        /// <summary>
        /// Tests the Formula constructor with a string that doesn't contain any valid variables, a validate function, and a normalize function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NormalizerAndValidatorConstructorTestFail()
        {
            Formula normalizerAndValidatorTest = new Formula("b + 2", normalizer, validate);
        }

        /// <summary>
        /// Tests the Formula constructor with a string that doesn't contain any valid variables and a validate function, but no normalize function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ValidatorConstructorTestThrowsException()
        {
            Formula validatorTestFail = new Formula("B + 2", null, validate);
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "(2".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest1()
        {
            Formula malformedExp = new Formula("(2");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "+ 2".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest2()
        {
            Formula malformedExp = new Formula("+ 2");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using an empty expression.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest3()
        {
            Formula malformedExp = new Formula("");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "(+".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest4()
        {
            Formula malformedExp = new Formula("(+");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "2 + 3) / 2".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest5()
        {
            Formula malformedExp = new Formula("2 + 3) / 2");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "2 2".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest6()
        {
            Formula malformedExp = new Formula("2 2");
        }

        /// <summary>
        /// Tests the Formula constructor to raise a FormulaFormatException using the expression "(2 + 3) *".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MalformedExpTest7()
        {
            Formula malformedExp = new Formula("(2 + 3) *");
        }

        /// <summary>
        /// Tests the Evaluate() method without a lookup function.
        /// </summary>
        [TestMethod]
        public void EvaluateTestNoLookup()
        {
            //Basic addition test
            Formula addTest = new Formula("2 + 3");
            //Basic subtraction test
            Formula subTest = new Formula("2 - 3");
            //Basic multiplication test
            Formula multTest = new Formula("2 * 3");
            //Basic division test
            Formula divTest = new Formula("4 / 8");
            //Paranthetical expression test
            Formula paraTest = new Formula("(2 + 3)");
            //Stress test, many operations
            Formula manyOperations = new Formula("(20 / (1 * (2 / 1))) / (1 + (2 * 2 / 1)) / 2 * 17 + (42 - 31) * 2 / 2");

            //Assertions used to check that both the returned values are doubles and that they are the correct values. 
            Assert.AreEqual(5d, addTest.Evaluate(null));
            Assert.AreEqual(-1d, subTest.Evaluate(null));
            Assert.AreEqual(6d, multTest.Evaluate(null));
            Assert.AreEqual(0.5d, divTest.Evaluate(null));
            Assert.AreEqual(5d, paraTest.Evaluate(null));
            Assert.AreEqual(28d, manyOperations.Evaluate(null));
        }

        /// <summary>
        /// Tests the Evaluate() function with an expression containing a variable, but no lookup function.
        /// </summary>
        [TestMethod]
        public void EvaluateTestNoLookupWithException()
        {
            //Expression with a variable
            Formula noLookupEvaluateWithVar = new Formula("2 + A2");
            //Evaluation done with no lookup function
            object result = noLookupEvaluateWithVar.Evaluate(null);

            //Check if returned value is a FormulaError object
            Assert.IsTrue(result is FormulaError);
        }

        /// <summary>
        /// Tests Evaluate with an expression containing variables and a lookup function
        /// </summary>
        [TestMethod]
        public void EvaluateTestWithLookup()
        {
            //Two expressions containing variables
            Formula lookupTest = new Formula("A + 3");
            Formula lookupTest2 = new Formula("C + 4");

            //Assert that the values are what they are expected to be
            Assert.AreEqual(5d, lookupTest.Evaluate(lookup));
            Assert.AreEqual(7d, lookupTest2.Evaluate(lookup));
        }


        /// <summary>
        /// Tests the .Equals method and == operator
        /// </summary>
        [TestMethod]
        public void EqualityTest()
        {
            //Test if two identical Formula objects are equivalent
            Formula equalTest = new Formula("2 + 3");
            Formula equalTest2 = new Formula("2 + 3");
            //Tests if two Formula objects are identical when a normalizer is used
            Formula equalTest3 = new Formula("2 + x4", normalizer, null);
            Formula equalTest4 = new Formula("2 + X4", normalizer, null);
            //Tests to see if false is returned correctly for .Equals and ==
            Formula notEqualTest = new Formula("2 + 4");
            Formula notEqualTest2 = new Formula("2");
            //Tests null behavior
            Formula nullTest = null;

            //Identical Formula object assertions:
            Assert.IsTrue(equalTest.Equals(equalTest2));
            Assert.IsTrue(equalTest == equalTest2);
            //Identical when normalizer is used:
            Assert.IsTrue(equalTest3.Equals(equalTest4));
            Assert.IsTrue(equalTest3 == equalTest4);
            //Assertions for false if not equal:
            Assert.IsFalse(notEqualTest.Equals(notEqualTest2));
            Assert.IsFalse(notEqualTest == notEqualTest2);
            //Assertions for null behavior
            Assert.IsFalse(equalTest.Equals(null));
            Assert.IsFalse(equalTest == null);
            Assert.IsFalse(null == equalTest);
            Assert.IsTrue(nullTest == null);
        }

        /// <summary>
        /// Tests GetVariables() function
        /// </summary>
        [TestMethod]
        public void GetVariablesTest()
        {
            //Correct values that should be returned by GetVariables function
            string[] vars = { "x2", "t_2", "Y56_5t" };

            //Test Formula object with expression containing variables
            Formula getVarsTest = new Formula("x2 + t_2 / Y56_5t");
            int i = 0;
            //Tests to assert that each variable is returned by GetVariables() in order from left to right
            foreach (string var in getVarsTest.GetVariables())
            {
                Assert.AreEqual(vars[i], var);
                i++;
            }
        }

        /// <summary>
        /// Tests the != operator
        /// </summary>
        [TestMethod]
        public void NotEqualsTest()
        {
            //Tests using two unequal expressions:
            Formula equalTest = new Formula("2 + 3");
            Formula equalTest2 = new Formula("2 + 5");
            //Tests using two unequal exprssions with variables:
            Formula equalTest3 = new Formula("2 + x4");
            Formula equalTest4 = new Formula("2 + X4");

            //Assert that the != operator is working correctly
            Assert.IsTrue(equalTest != equalTest2);
            Assert.IsTrue(equalTest3 != equalTest4);
        }

        /// <summary>
        /// Tests GetHashCode() function
        /// </summary>
        [TestMethod]
        public void GetHashTest()
        {
            //Create test Formula object
            Formula hashTest = new Formula("2 + 4");

            //Assert that the correct hash is being made (see source for description)
            Assert.AreEqual("2 + 4".GetHashCode(), hashTest.GetHashCode());
        }

        /// <summary>
        /// Tests the FormulaError class
        /// </summary>
        [TestMethod]
        public void FormulaErrorTest()
        {
            //Create test Formula object that should generate a specific FormulaError message
            Formula errorMessageTest = new Formula("1 - 0 / 0");

            //Assert that the FormulaError class is working correctly.
            Assert.AreEqual("Cannot execute expression - dividing by 0!", ((FormulaError) errorMessageTest.Evaluate(null)).Reason);
        }



        /// <summary>
        /// Tests various private methods
        /// </summary>
        [TestMethod]
        public void PrivateMethodTests()
        {
            //Create test Formula object
            Formula privateTest = new Formula("2 + 3");
            //Create private member accessor
            PrivateObject privateAccessor = new PrivateObject(privateTest);

            //Test that the TokenValidator method validates operators correctly and returns their type ("Op" for operator)
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "+"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "-"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "*"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "/"));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", "("));
            Assert.AreEqual("Op", privateAccessor.Invoke("TokenValidator", ")"));
            //Test that the TokenValidator method validates numbers and variables correctly and returns their type ("Num/Var" for number/variable)
            Assert.AreEqual("Num/Var", privateAccessor.Invoke("TokenValidator", "3"));
            Assert.AreEqual("Num/Var", privateAccessor.Invoke("TokenValidator", "x4"));
            //Tests that the TokenValidator class throws a FormulaFormatException if the token is not a valid number, operator, or variable
            try
            {
                privateAccessor.Invoke("TokenValidator", "&");
            }
            catch(FormulaFormatException f)
            {
                Assert.AreEqual("The token \"&\" is invalid. Please validate the formula contents.", f.Message);
            }

            //Tests used to test the Add and Subtract methods

            //Create an operands Stack holding 2 and 3
            Stack<string> operands = new Stack<string>();
            operands.Push("2");
            operands.Push("3");
            //Create an operators Stack holding the + operator
            Stack<string> operators = new Stack<string>();
            operators.Push("+");
            //Add 2 and 3 and assert that the associated Stacks are empty
            privateAccessor.Invoke("Add", operators, operands);
            Assert.AreEqual("5", operands.Pop());
            Assert.AreEqual(0, operands.Count);
            Assert.AreEqual(0, operators.Count);

            //Attempt to generate a FormulaFormatException:
            operands.Push("2");
            operators.Push("+");
            try
            {
                //Attemp to evaluate "2 +"
                privateAccessor.Invoke("Add", operators, operands);
            }
            catch (FormulaFormatException f)
            {
                Assert.AreEqual("Cannot execute expression - not enough operands to execute addition. Please verify your expression's addition.", f.Message);
            }

            //Clear the stacks
            operators.Clear();
            operands.Clear();

            //Create the expression "3 - 2"
            operands.Push("3");
            operands.Push("2");
            operators.Push("-");
            //Assert that the expression 3 - 2 is completed correctly
            privateAccessor.Invoke("Subtract", operators, operands);
            Assert.AreEqual("1", operands.Pop());
        }
    }
}
