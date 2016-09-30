﻿using System;
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
        /// Tests SetCellContents() when a Formula object is passed into the function. Also, inadvertently tests the GetCellContents() method, when the name, value pair are present.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a test formula for later assertion:
            Formula testFormula = new Formula("1 + 2");
            //Attempt to set the cell contents:
            ISet<string> result = sheet.SetCellContents("A1", testFormula);
            //Assert that there are no dependants:
            Assert.AreEqual(0, result.Count);
            //Assert that the given cell contents are correct (this also inadvertantly tests GetCellContents() as stated in the summary):
            Assert.AreEqual(testFormula, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a string is passed into the function.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a string for later assertion:
            string testString = "test";
            //Attempt to set the cell contents:
            ISet<string> result = sheet.SetCellContents("A1", testString);
            //Assert that there are no dependants:
            Assert.AreEqual(0, result.Count);
            //Assert that the given cell contents are correct:
            Assert.AreEqual(testString, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a double is passed into the function.
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithDoubleTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a double for later assertion:
            double testDouble = 12.123;
            //Attempt to set the cell contents:
            ISet<string> result = sheet.SetCellContents("A1", testDouble);
            //Assert that there are no dependants:
            Assert.AreEqual(0, result.Count);
            //Assert that the given cell contents are correct:
            Assert.AreEqual(testDouble, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests SetCellContents() when a null Formula is passed into the function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentWithNullFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a null Formula
            Formula nullFormula = null;
            //Attempt to set the cell contents:
            sheet.SetCellContents("A1", nullFormula);
        }

        /// <summary>
        /// Tests SetCellContents() when a null String is passed into the function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentWithNullStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            //Create a null String
            String nullString = null;
            //Attempt to set the cell contents:
            sheet.SetCellContents("A1", nullString);
        }



    }
}
