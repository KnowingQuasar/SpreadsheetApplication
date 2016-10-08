using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace SpreadsheetTest
{
    /// <summary>
    /// Test class for the Spreadsheet class.
    /// </summary>
    [TestClass]
    public class SpreadSheetTest
    {
        /// <summary>
        /// Tests the Spreadsheet object's default constructor.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorTest()
        {
            //Create Spreadsheet objects using the regular Spreadsheet and AbstractSpreadsheet identifiers:
            Spreadsheet spreadSheet = new Spreadsheet();
            AbstractSpreadsheet absSpreadsheet = new Spreadsheet();
            //Assert that they are not null:
            Assert.IsNotNull(spreadSheet);
            Assert.IsNotNull(absSpreadsheet);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void LoadXmlTest()
        {
            Spreadsheet sheet = new Spreadsheet(null, null, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet1.xml");
            sheet.Save("C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml");
        }

        /// <summary>
        /// Tests GetCellContents() when there are no defined cells.
        /// </summary>
        [TestMethod]
        public void GetCellContentsWithNoDefinedCellsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Attempt to get the cell contents of the empty cell "A1"
            object result = sheet.GetCellContents("A1");
            //Assert that it returned an empty string
            Assert.AreEqual("", result);
            //Another empty cell "A_23x":
            result = sheet.GetCellContents("A_23x");
            //Assert that it returned an empty string once more
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Tests GetCellContents() when a null name is passed. Should throw an InvalidNameException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithNullNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Attempt to get the cell contents using a null name parameter:
            sheet.GetCellContents(null);
        }

        /// <summary>
        /// Tests GetCellContents() when a number is passed as a name. Should throw an InvalidNameException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithBadNameTest1()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Attempt to get the cell contents using a number as a cell name:
            sheet.GetCellContents("2");
        }

        /// <summary>
        /// Tests GetCellContents() when a number then letter combination is passed as a name. Should throw an InvalidNameException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithBadNameTest2()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Attempt to get the cell contents using a number then letter combination as a cell name:
            sheet.GetCellContents("2d");
        }

        /// <summary>
        /// Tests GetCellContents() when a non-number or letter is passed as a name. Should throw an InvalidNameException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithBadNameTest3()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Attempt to get the cell contents using a non-number or letter (in this case the "&" symbol)
            sheet.GetCellContents("&");
        }

        /// <summary>
        /// Tests SetCellContents() when a Formula object is passed into the function. Also, inadvertently tests the GetCellContents() method, when the name, contents pair are present.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a test formula for later assertion:
            Formula testFormula = new Formula("1 + 2");
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", testFormula);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct (this also inadvertantly tests GetCellContents() as stated in the summary):
            Assert.AreEqual(testFormula, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a Formula object is passed into the function, then later changed to a different Formula.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithFormulaEditTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a test formula for later assertion:
            Formula testFormula = new Formula("1 + 2");
            //Create another test formula for testing edits:
            Formula testFormula2 = new Formula("3 + 4");
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", testFormula);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct
            Assert.AreEqual(testFormula, sheet.GetCellContents("A1"));

            //Change the cell contents of A1:
            result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", testFormula2);
            //Assert that the new values have been entered:
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(testFormula2, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a string is passed into the function.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a string for later assertion:
            string testString = "test";
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", testString);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct:
            Assert.AreEqual(testString, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a string is passed into the function, then later changed to a different string.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithStringEditTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a test string for later assertion:
            string string1 = "string1";
            //Create another test string for testing edits:
            string string2 = "string2";
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", string1);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct
            Assert.AreEqual(string1, sheet.GetCellContents("A1"));

            //Change the cell contents of A1:
            result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", string2);
            //Assert that the new values have been entered:
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(string2, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a double is passed into the function.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithDoubleTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a double for later assertion:
            double testDouble = 12.123;
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", testDouble);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct:
            Assert.AreEqual(testDouble, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a double is passed into the function, then later changed to a different double.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithDoubleEditTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a test double for later assertion:
            double double1 = 1;
            //Create another test double for testing edits:
            double double2 = 2;
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", double1);
            //Assert that there are no dependants:
            Assert.AreEqual(1, result.Count);
            //Assert that the given cell contents are correct
            Assert.AreEqual(double1, sheet.GetCellContents("A1"));

            //Change the cell contents of A1:
            result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", double2);
            //Assert that the new values have been entered:
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(double2, sheet.GetCellContents("A1"));
        }

        // ================================================================
        // Please note that the following tests used in PS4 no longer work 
        // in PS5 as the PrivateObject structure's Invoke method does not
        // deal with null arguments very well. In this case, the three 
        // different versions of SetCellContents() cause a problem when
        // trying to call the method using a null argument. This is a 
        // limitation on Unit Testing using Visual C#, and something I 
        // will not be able to easily overcome.
        // ================================================================

        /*
        /// <summary>
        /// Tests SetCellContents() when a null Formula is passed into the function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentWithNullFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a null Formula
            Formula nullFormula = null;
            //Attempt to set the cell contents:
            privateSheetAccessor.Invoke("SetCellContents", "A1", nullFormula);
        }

        /// <summary>
        /// Tests SetCellContents() when a null String is passed into the function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentWithNullStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a null String
            String nullString = null;
            //Attempt to set the cell contents:
            privateSheetAccessor.Invoke("SetCellContents", "A1", nullString);
        }*/



        /// <summary>
        /// Tests SetCellContents() with a Formula containing multiple dependencies.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithFormulaHavingDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a Formula with dependencies
            Formula formulaWithDependencies = new Formula("A2 + 3");
            //Attempt to set the cell contents:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", formulaWithDependencies);
            //Assert that the resulting values are as expected:
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsFalse(result.Contains("A2"));
        }

        /// <summary>
        /// Tests SetCellContents() with a Formula containing multiple dependencies referencing each other (i.e. A1 contains 3 * A4 and A2 contains A1 + 3).
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithFormulaHavingDependenciesTest2()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create one Formula with dependencies
            Formula formulaWithDependencies1 = new Formula("A2 + 3");
            //Create another Formula with dependencies
            Formula formulaWithDependencies2 = new Formula("A1 * 4");
            //Attempt to set the cell contents of A4:
            privateSheetAccessor.Invoke("SetCellContents", "A4", formulaWithDependencies2);
            //Attempt to set the cell contents of A1:
            ISet<string> result = (ISet<string>)privateSheetAccessor.Invoke("SetCellContents", "A1", formulaWithDependencies1);
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue(result.Contains("A4"));
        }

        /// <summary>
        /// Tests SetCellContents() with two Formula that reference each other (cell A1 references A2 and A2 references A1)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWithCircularDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create another Formula with dependencies
            Formula formulaWithDependencies1 = new Formula("A1");
            //Create one Formula with dependencies
            Formula formulaWithDependencies2 = new Formula("A2");
            //Attempt to set the cell contents of A2:
            privateSheetAccessor.Invoke("SetCellContents", "A2", formulaWithDependencies1);
            //Attempt to set the cell contents of A1:
            privateSheetAccessor.Invoke("SetCellContents", "A1", formulaWithDependencies2);
        }

        /// <summary>
        /// Tests GetNamesOfAllNonemptyCells() functionality
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            PrivateObject privateSheetAccessor = new PrivateObject(sheet);
            //Create a bunch of test Formula
            Formula formula1 = new Formula("A10");
            Formula formula2 = new Formula("A11");
            Formula formula3 = new Formula("A12");
            Formula formula4 = new Formula("A13");
            Formula formula5 = new Formula("A14");
            Formula formula6 = new Formula("A15");

            //Initialize several cells to these formulae:
            privateSheetAccessor.Invoke("SetCellContents", "A1", formula1);
            privateSheetAccessor.Invoke("SetCellContents", "A2", formula2);
            privateSheetAccessor.Invoke("SetCellContents", "A3", formula3);
            privateSheetAccessor.Invoke("SetCellContents", "A4", formula4);
            privateSheetAccessor.Invoke("SetCellContents", "A5", formula5);
            privateSheetAccessor.Invoke("SetCellContents", "A6", formula6);

            //Expected values from GetNamesOfAllNonemptyCells() as an array (comparing Lists didn't work for some reason...)
            string[] expected = { "A1", "A2", "A3", "A4", "A5", "A6" };
            //List of the actual values generated by GetNamesOfAllNonemptyCells():
            List<string> actual = new List<string>(sheet.GetNamesOfAllNonemptyCells());

            int count = 0;
            //Iterate through the actual values and compare the expected values with the actual:
            foreach (string actualVal in actual)
            {
                Assert.AreEqual(expected[count], actualVal);
                count++;
            }
        }

        /// <summary>
        /// Tests GetDirectDependents() with a null name passed in. Should throw an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependentsWithNullNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Enumerate a null string
            String nullString = null;
            //Create the private accessor object of the Spreadsheet 
            PrivateObject sheetPrivateAccessor = new PrivateObject(sheet);
            //Attempt to invoke GetDirectDependents(). Should throw an ArgumentNullException
            sheetPrivateAccessor.Invoke("GetDirectDependents", nullString);
        }

        /// <summary>
        /// Tests GetDirectDependents() with a spreadsheet that contains no cells. Should return an empty List
        /// </summary>
        [TestMethod]
        public void GetDirectDependentsNoCellsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            String badString = "A1";
            PrivateObject sheetPrivateAccessor = new PrivateObject(sheet);
            List<string> result = (List<string>)sheetPrivateAccessor.Invoke("GetDirectDependents", badString);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>
        /// Tests ValidateName() with a valid name according to the variable rules
        /// </summary>
        [TestMethod]
        public void ValidateNameGoodNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create the private accessor for the spreadsheet
            PrivateObject sheetPrivateAccessor = new PrivateObject(sheet);
            try
            {
                //Invoke ValidateName() with "A1". Should NOT throw an exception.
                sheetPrivateAccessor.Invoke("ValidateName", "A1");
            }
            catch (Exception e)
            {
                //If there was an exception, fail the test
                Assert.Fail("Expected no exception, but got: " + e.Message);
            }
        }

        /// <summary>
        /// Tests ValidateName() with an invalid name according to the variable rules. Should throw an InvalidNameException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ValidateNameBadNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create the private accessor for the spreadsheet
            PrivateObject sheetPrivateAccessor = new PrivateObject(sheet);
            sheetPrivateAccessor.Invoke("ValidateName", "&");
        }

        /// <summary>
        /// Tests ValidateName() with a null name. Should throw an InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ValidateNameNullNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create the private accessor for the spreadsheet
            PrivateObject sheetPrivateAccessor = new PrivateObject(sheet);
            //Enumerate a null string
            String nullString = null;
            sheetPrivateAccessor.Invoke("ValidateName", nullString);
        }
    }
}
