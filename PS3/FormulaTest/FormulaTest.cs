using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTest
    {
        private string normalizer(string token)
        {
            return token.ToUpper();
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
            Formula normalizerTest = new Formula("2 + 3", normalizer, null);

        }
    }
}
