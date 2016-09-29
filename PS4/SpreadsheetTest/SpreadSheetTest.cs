using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

namespace SpreadsheetTest
{
    [TestClass]
    public class SpreadSheetTest
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            Spreadsheet spreadSheet = new Spreadsheet();
            AbstractSpreadsheet absSpreadsheet = new Spreadsheet();
            Assert.IsNotNull(spreadSheet);
            Assert.IsNotNull(absSpreadsheet);
        }

        [TestMethod]
        public void GetCellContentsWithNoDefinedCellsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            object result = sheet.GetCellContents("A1");
            Assert.AreEqual("", result);
            result = sheet.GetCellContents("A_23x");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithNullNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWithBadNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("2");
        }
    }
}
