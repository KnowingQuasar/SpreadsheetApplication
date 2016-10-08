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
        /// Test validator function to act as delegate
        /// </summary>
        /// <param name="name">The name to validate</param>
        /// <returns></returns>
        private bool validate(string name)
        {
            if(name.Length > 2)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Test normalize function to act as delegate
        /// </summary>
        /// <param name="name">The name to normalize</param>
        /// <returns></returns>
        private string normalize(string name)
        {
            return name.ToUpper();
        }

        /// <summary>
        /// Test normalize function that should invalidate the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string badNormalize(string name)
        {
            return name += "123456";
        }

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
        /// Tests the four argument constructor. NOTE: uses a file on MY system, will not work on yours unless the filepath is edited
        /// </summary>
        [TestMethod]
        public void ConstructorLoadingXmlTest()
        {
            //Constructor loading the spreadsheet at the specified filepath, without a normalizer or validator:
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet1.xml");
            //Create a Formula to Assert against
            Formula testFormula = new Formula("A2 + A3");

            //Make assertions to test that the XML file was loaded correctly
            Assert.AreEqual("test", sheet.GetCellContents("A1"));
            Assert.AreEqual(1.0, sheet.GetCellContents("A2"));
            Assert.AreEqual(2.0, sheet.GetCellContents("A3"));
            Assert.AreEqual(testFormula, sheet.GetCellContents("A4"));

            //Set the contents of a cell with a name that should be normalized to all uppercase lettering (i.e. "a7" -> "A7")
            sheet.SetContentsOfCell("a7", "3.0");
            //Verify that the uppercased name exists as a cell
            Assert.IsTrue((new HashSet<string>(sheet.GetNamesOfAllNonemptyCells())).Contains("A7"));
        }

        /// <summary>
        /// Tests the three argument constructor of the Spreadsheet class
        /// </summary>
        [TestMethod]
        public void ConstructorThreeArgTest()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "test version");

            //Assert that the sheet has been initialized correctly
            Assert.AreEqual("test version", sheet.Version);
            Assert.AreEqual(validate, sheet.IsValid);
            Assert.AreEqual(normalize, sheet.Normalize);
        }

        /// <summary>
        /// Tests to see if the passed in validator throws the correct exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameInCellTest()
        {
            //Make a new sheet that uses the isValid and Normalize delegates
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default");
            //Set the contents of the cell to something that would cause the Normalize delegate to return false
            sheet.SetContentsOfCell("A123", "test");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException is thrown when the version of the file does not match the inputted version in the constructor
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetReadWriteExceptionTest1()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "1.0", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet2.xml");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException is thrown when there are invalid tags present in the XML file
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetReadWriteExceptionTest2()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet2.xml");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException is thrown when no version attribute is present
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetReadWriteExceptionTest3()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet3.xml");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException is thrown when there are attributes in the spreadsheet tag that aren't "version"
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetReadWriteExceptionTest4()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet4.xml");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException is thrown when there are attributes in the spreadsheet tag that aren't "version"
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetReadWriteExceptionTest5()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet5.xml");
        }

        /// <summary>
        /// Tests to see if a SpreadsheetReadWriteException prints the correct message
        /// </summary>
        [TestMethod]
        public void SpreadsheetReadWriteExceptionTest()
        {
            try
            {
                Spreadsheet sheet = new Spreadsheet(validate, normalize, "default", "C:\\Users\\Ian\\Source\\Repos\\01071551\\PS4\\TestXmlSpreadsheet4.xml");
            }
            catch(SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("The spreadsheet tag has more than just a version attribute!", e.Message);
            }
        }

        /// <summary>
        /// Tests GetSavedVersion() to make sure it has full functionality
        /// </summary>
        [TestMethod]
        public void GetSavedVersionTest()
        {
            Spreadsheet sheet = new Spreadsheet(validate, normalize, "1.2");

            //Sets up a cell with the name "A1" and value "test"
            sheet.SetContentsOfCell("A1", "test");
            //Save the sheet
            sheet.Save("C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml");
            //Get the version of teh sheet just saved
            Assert.AreEqual("1.2", sheet.GetSavedVersion("C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml"));
        }

        /// <summary>
        /// Tests the Save method of the Spreadsheet object
        /// </summary>
        [TestMethod]
        public void SaveXmlTest()
        {
            //Create a blank sheet
            Spreadsheet sheet = new Spreadsheet();
            //Create a Formula to Assert against
            Formula testFormula = new Formula("A2 + A3");

            //Set a bunch of cells to string, doubles, and Formula
            sheet.SetContentsOfCell("A1", "test");
            sheet.SetContentsOfCell("A2", "1.0");
            sheet.SetContentsOfCell("A3", "2.0");
            sheet.SetContentsOfCell("A4", "=A2 + A3");
            //Save the file to the disk
            sheet.Save("C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml");

            //Load the file just saved to a new Spreadsheet
            Spreadsheet loadedSheet = new Spreadsheet(null, null, "default", "C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml");

            //Make assertions to test that the XML file was saved and reloaded successfully
            Assert.AreEqual("test", loadedSheet.GetCellContents("A1"));
            Assert.AreEqual(1.0, loadedSheet.GetCellContents("A2"));
            Assert.AreEqual(2.0, loadedSheet.GetCellContents("A3"));
            Assert.AreEqual(testFormula, loadedSheet.GetCellContents("A4"));
        }

        /// <summary>
        /// Tests the GetCellValue method's functionality
        /// </summary>
        [TestMethod]
        public void GetCellValueTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set 3 cells to the values 1.0, 2.0, and the formula defining the multiplication of those two
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "=A1*A2");

            //Assert that the values of the cells are as expected
            Assert.AreEqual(2d, sheet.GetCellValue("A3"));
            Assert.AreEqual(1d, sheet.GetCellValue("A1"));
            Assert.AreEqual(2d, sheet.GetCellValue("A2"));
        }

        /// <summary>
        /// Tests to see if an InvalidNameException is thrown when an invalid cell name is passed into GetCellValue()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueWithInvalidNameExceptionTest1()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up a cell
            sheet.SetContentsOfCell("A1", "1.0");

            //Attempt to evaluate a non-existent cell, should throw exception
            sheet.GetCellValue("A3");
        }

        /// <summary>
        /// Tests to see if an InvalidNameException is thrown when a null value is passed into GetCellValue()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueWithInvalidNameExceptionTest2()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up a cell
            sheet.SetContentsOfCell("A1", "1.0");

            //Attempt to evaluate a non-existent cell, should throw exception
            sheet.GetCellValue(null);
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
        /// Tests SetCellContents with a Formula that is changed to have different dependents
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithEditedFormulaDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up the cell contents needed for the test
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "4.0");
            sheet.SetContentsOfCell("A4", "=A1*A2");
            //Assert that the value of A4 is as expected
            Assert.AreEqual(2d, sheet.GetCellValue("A4"));
            //Change the value of A4
            sheet.SetContentsOfCell("A4", "=A2*A3");
            //Assert that the value of A4 is as expected
            Assert.AreEqual(8d, sheet.GetCellValue("A4"));
        }

        /// <summary>
        /// Tests SetCellContents to verify behavior when a cell that had a Formula is changed to a string
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithStringEditDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up test cells
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "=A1 + A2");
            sheet.SetContentsOfCell("A3", "test");
            //Assert that the cell has been changed correctly (GetCellValue("A3") also verifies that the dependencies have been removed)
            Assert.AreEqual("test", sheet.GetCellValue("A3"));
        }

        /// <summary>
        /// Tests SetContentsOfCell() with a passed in empty string
        /// </summary>
        [TestMethod]
        public void SetCellContentWithEmptyStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Attempt to set the contents of A1 to ""
            sheet.SetContentsOfCell("A1", "");
            //Assert that the count of Nonempty cells is equal to 0
            Assert.IsTrue(new HashSet<string>(sheet.GetNamesOfAllNonemptyCells()).Count == 0);
            //Set the cell contents of A2 to "test"
            sheet.SetContentsOfCell("A2", "test");
            //Assert that the count of Nonempty cells is equal to 1
            Assert.IsTrue(new HashSet<string>(sheet.GetNamesOfAllNonemptyCells()).Count == 1);
            //Set the cell contents of A2 to ""
            sheet.SetContentsOfCell("A2", "");
            //Assert that the count of Nonempty cells is equal to 0
            Assert.IsTrue(new HashSet<string>(sheet.GetNamesOfAllNonemptyCells()).Count == 0);
        }

        /// <summary>
        /// Tests SetCellContents to verify behavior when a cell that had a Formula is changed to a string
        /// </summary>
        [TestMethod]
        public void SetCellContentsWithDoubleEditDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up test cells
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "=A1 + A2");
            sheet.SetContentsOfCell("A3", "4.0");
            //Assert that the cell has been changed correctly (GetCellValue("A3") also verifies that the dependencies have been removed)
            Assert.AreEqual(4d, sheet.GetCellValue("A3"));
            Assert.IsTrue(new HashSet<string>(sheet.GetNamesOfAllNonemptyCells()).Count == 3);
        }

        /// <summary>
        /// Tests SetContentsOfCell() using a normalizer function, but no validator
        /// </summary>
        [TestMethod]
        public void SetCellContentsUsingNoValidator()
        {
            Spreadsheet sheet = new Spreadsheet(null, normalize, "1.0");
            sheet.SetContentsOfCell("a1", "test");
            Assert.IsTrue((new HashSet<string>(sheet.GetNamesOfAllNonemptyCells())).Contains("A1"));
        }


        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        /// <summary>
        /// Attempts to set a cell to a name that causes the normalizer to invalidate the name according to the validator's rules
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsWithNormalizeInvalidatesNameTest()
        {
            Spreadsheet sheet = new Spreadsheet(validate, badNormalize, "1.0");
            sheet.SetContentsOfCell("A1", "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsWithNullFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", null);
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
        /// Tests SetContentsOfCell with a long chain of connected cells
        /// </summary>
        [TestMethod]
        public void SetContentsOfCellStressTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up the cells for testing
            sheet.SetContentsOfCell("A1", "3.0");
            sheet.SetContentsOfCell("A2", "4.0");
            sheet.SetContentsOfCell("A3", "18");
            sheet.SetContentsOfCell("A4", "=A1+A2");
            sheet.SetContentsOfCell("A5", "=A1+A3");
            sheet.SetContentsOfCell("A6", "=A2+A3");
            sheet.SetContentsOfCell("A7", "=A4+A5");
            sheet.SetContentsOfCell("A8", "=A6+A7");

            //Assert that all values are as expected
            Assert.AreEqual(7d, sheet.GetCellValue("A4"));
            Assert.AreEqual(21d, sheet.GetCellValue("A5"));
            Assert.AreEqual(22d, sheet.GetCellValue("A6"));
            Assert.AreEqual(28d, sheet.GetCellValue("A7"));
            Assert.AreEqual(50d, sheet.GetCellValue("A8"));
        }

        /// <summary>
        /// Tets SetContentsOfCell with an invalid Formula
        /// </summary>
        [TestMethod]
        public void SetContentsOfCellInvalidFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up the cells for testing
            sheet.SetContentsOfCell("A1", "test");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "=A1+A2");

            Assert.IsTrue(sheet.GetCellValue("A3") is FormulaError);
        }


        [TestMethod]
        public void ChangedTest()
        {
            Spreadsheet sheet = new Spreadsheet();

            //Set up a spreadsheet
            sheet.SetContentsOfCell("A1", "test");
            sheet.SetContentsOfCell("A2", "2.0");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("C:\\Users\\Ian\\Desktop\\Tests\\Testing.xml");
            Assert.IsFalse(sheet.Changed);
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
