// Written by Ian Rodriguez
// Date of Completion: 9/29/2016
// Purpose: general setup of future spreadsheet design.

using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Dictionary containing a group of associated names and their cell values.
        /// </summary>
        private Dictionary<string, Cell> allCells;

        /// <summary>
        /// Dependency graph for the Spreadsheet. See the DependencyGraph documentation for details.
        /// </summary>
        private DependencyGraph dg;

        private bool changed;

        /// <summary>
        /// 
        /// </summary>
        public override bool Changed
        {
            get
            {
                return changed;
            }

            protected set
            {
                changed = value;
            }
        }

        /// <summary>
        /// Default constructor that initializes an empty spreadsheet that normalizes to itself and has no validation conditions 
        /// </summary>
        public Spreadsheet() : base(null, null, "default")
        {
            //Initialize the dictionary holding cells and the DependencyGraph
            allCells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// Constructor that initializes an empty spreadsheet and sets validate and normalize functions and a "version"
        /// </summary>
        /// <param name="isValid">Validation function to use.</param>
        /// <param name="normalize">Normalizer function to use.</param>
        /// <param name="version">Version string to use.</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Initialize the dictionary holding cells and the DependencyGraph
            allCells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// Constructor that loads a spreadsheet from the specified file path and sets validate and normalize functions and a version string 
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        /// <param name="filePath"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version, string filePath) : base(isValid, normalize, version)
        {
            //Load the spreadsheet from the file
            LoadSpreadsheet(filePath);
        }

        /// <summary>
        /// Private method used to load a spreadsheet from the given filepath.
        /// </summary>
        /// <param name="file">Filepath to load the spreadsheet from.</param>
        private void LoadSpreadsheet(string file)
        {
            List<string> validTags = new List<string>(new string[] { "spreadsheet", "cell", "name", "contents" });
            Dictionary<string, string> cellsToAdd = new Dictionary<string, string>();
            //Use the XmlReader class to read the file
            using (XmlReader reader = XmlReader.Create(file))
            {
                //Attempt to read to the spreadsheet tag, throw a SpreadsheetReadWriteException if no spreadsheet tag is found:
                if (!reader.ReadToFollowing("spreadsheet"))
                {
                    throw new SpreadsheetReadWriteException("The spreadsheet tag was not found in the given XML file!");
                }
                //Attempt to read to the first attribute, if no attribute is found throw a SpreadsheetReadWriteException
                if (!reader.MoveToFirstAttribute())
                {
                    throw new SpreadsheetReadWriteException("The spreadsheet tag did not have a version attribute!");
                }
                //Validate that the attribute is named "version", throw exception otherwise
                if (!reader.Name.ToLower().Equals("version"))
                {
                    throw new SpreadsheetReadWriteException("The spreadsheet tag has more than just a version attribute!");
                }
                //Assign the version attribute to a string
                string ver = reader.Value;
                //If the file's version doesn't match the inputted version, throw exception
                if (!ver.Equals(Version))
                {
                    throw new SpreadsheetReadWriteException("The version of the read spreadsheet does not match the inputted version!");
                }
                //Otherwise parse the remainder of the file:
                else
                {
                    //Temporary strings used to determine the XML element and its contents
                    string elemName = "";
                    string name = "";
                    string value = "";
                    //If the read string is a value, not an element name, we need to SetCellContents
                    bool isValue = false;
                    //While there are more elements to read loop:
                    while (reader.Read())
                    {
                        //Switch through the various NodeTypes
                        switch (reader.NodeType)
                        {
                            //If the reader is at an element take note of the element name:
                            case XmlNodeType.Element:
                                elemName = reader.Name.ToLower();
                                //If the name isn't a valid tag name according to our scheme, throw a SpreadsheetReadWriteException
                                if (!validTags.Contains(elemName))
                                {
                                    throw new SpreadsheetReadWriteException("An invalid tag was present in the XML file! Cannot finish parsing...");
                                }
                                break;
                            //If the reader is at the contents of a an XML tag (i.e. <name>This portion</name>)
                            case XmlNodeType.Text:
                                //If the XML element is at the name of a cell, store the value
                                if (elemName.Equals("name"))
                                {
                                    name = reader.Value.Trim();
                                }
                                //If the XML element is at the contents tag, store the value
                                else if (elemName.Equals("contents"))
                                {
                                    value = reader.Value.Trim();
                                    isValue = true;
                                }
                                break;
                            //If the XML document is at the end element tag (i.e. </cell>) continue
                            case XmlNodeType.EndElement:
                                break;
                        }
                        //If the reader is at a <content> element, create a cell
                        if (isValue)
                        {
                            cellsToAdd.Add(name, value);
                            isValue = false;
                        }
                    }
                    //Initialize DependencyGraph and Dictionary holding cells
                    allCells = new Dictionary<string, Cell>();
                    dg = new DependencyGraph();
                    //Loop through all to-be added cells and call SetContentsOfCell()
                    foreach (KeyValuePair<string, string> toBeCell in cellsToAdd)
                    {
                        SetContentsOfCell(toBeCell.Key, toBeCell.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Internal function used broadly to validate that a name is non-null and fits the valid variable criteria.
        /// </summary>
        /// <param name="name">The name to be validated.</param>
        private void ValidateName(string name)
        {
            //If the name is null, first char is an int, or doesn't match the criteria, throw an InvalidNameException()

            int parsevar;
            if (name == null)
            {
                throw new InvalidNameException();
            }
            if (int.TryParse(name[0].ToString(), out parsevar))
            {
                throw new InvalidNameException();
            }
            if (!Regex.Match(name, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success)
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Gets the cell contents at the given name, if it exists. 
        /// </summary>
        /// <param name="name">The name of the cell to get the contents of.</param>
        /// <returns>Returns a string, double, or Formula object that represents the contents of the named cell.</returns>
        public override object GetCellContents(string name)
        {
            //Assure that the given name is valid
            ValidateName(name);
            //Create a container cell
            Cell retCell;
            //If the allCells dictionary is empty or doesn't contain the name in question, return an empty string.
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
            {
                return "";
            }
            //Otherwise get the contents of the given name and return it.
            else
            {
                allCells.TryGetValue(name, out retCell);
                return retCell.contents;
            }

        }

        /// <summary>
        /// Gets the names of all non-empty cells (all cells that do not have an empty string contents).
        /// </summary>
        /// <returns>Returns an IEnumerable of all non-empty cells.</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Create a container cell
            Cell checkCell;
            //Look through all names in allCells:
            foreach (string name in allCells.Keys)
            {
                //If the cell has a contents:
                if (allCells.TryGetValue(name, out checkCell))
                {
                    //If the contents is not an empty string, add it to the return contents:
                    if (!checkCell.contents.Equals(""))
                    {
                        yield return checkCell.name;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in Formula object.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The Formula object that will become the contents of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            //If the Formula object is null, throw an ArgumentNullException:
            if (formula == null)
            {
                throw new ArgumentNullException();
            }
            //Assure that the given name is valid
            ValidateName(name);
            object previousValue = "";

            //Add dependencies for each variable in the Formula
            foreach (string var in formula.GetVariables())
            {
                dg.AddDependency(var, name);
            }
            //If the cell hasn't already been added, create a new cell
            if (!allCells.ContainsKey(name))
            {
                allCells.Add(name, new Cell(name, formula));
            }
            //Otherwise, edit the cell
            else
            {
                previousValue = allCells[name].contents;
                allCells[name] = new Cell(name, formula);
            }
            //Try to get the cells to recalculate (essentially checking for CircularExceptions)
            try
            {
                GetCellsToRecalculate(name);
            }
            catch (CircularException e)
            {
                //Reset the cell that was to be added/edited and then throw the exception
                if (previousValue.Equals(""))
                {
                    allCells.Remove(name);
                }
                else
                {
                    allCells[name] = new Cell(name, previousValue);
                }
                throw e;
            }
            
            //Return a HashSet of the named cell's dependees and the name given.
            HashSet<string> retSet = GetAllDependents(name);
            retSet.Add(name);
            return retSet;
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in string.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The string that will become the contents of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            //Has the same logic as SetCellContents(string name, Formula formula), but uses a string instead of a Formula object. See the aforementioned method for details.

            bool isEmpty = false;
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            if (text.Equals(""))
            {
                isEmpty = true;
            }
            ValidateName(name);
            if (!allCells.ContainsKey(name))
            {
                if (isEmpty)
                {
                    return new HashSet<string>();
                }
                allCells.Add(name, new Cell(name, text));
            }
            else
            {
                if (allCells[name].contents is Formula)
                {
                    foreach (string var in ((Formula)allCells[name].contents).GetVariables())
                    {
                        dg.RemoveDependency(var, name);
                    }
                }
                if (text.Equals(""))
                {
                    allCells.Remove(name);
                    return new HashSet<string>();
                }
                allCells[name] = new Cell(name, text);
            }
            HashSet<string> retSet = GetAllDependents(name);
            retSet.Add(name);
            return retSet;
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in double.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The double that will become the contents of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            //Has the same logic as SetCellContents(string name, Formula formula), but uses a string instead of a Formula object. See the aforementioned method for details.
            //NOTE: does not check if the double is null, as the double data type is guaranteed to not be null.

            ValidateName(name);
            if (!allCells.ContainsKey(name))
            {
                allCells.Add(name, new Cell(name, number));
            }
            else
            {
                if (allCells[name].contents is Formula)
                {
                    foreach (string var in ((Formula)allCells[name].contents).GetVariables())
                    {
                        dg.RemoveDependency(var, name);
                    }
                }
                allCells[name] = new Cell(name, number);
            }
            HashSet<string> retSet = GetAllDependents(name);
            retSet.Add(name);
            return retSet;
        }

        /// <summary>
        /// Gets the direct dependants of the given name, if it exists.
        /// </summary>
        /// <param name="name">The name of the cell to get the dependants of.</param>
        /// <returns>Returns an IEnumerable of the direct dependants of the given name.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            ValidateName(name);
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
            {
                return new List<string>();
            }
            return dg.GetDependents(name);
        }

        /// <summary>
        /// Private method used to get ALL dependents (indirect and direct) of the specified cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns a set of all dependents, indirect and direct, of the specified cell</returns>
        private HashSet<string> GetAllDependents(string name)
        {
            //Set to return:
            HashSet<string> retSet = new HashSet<string>(GetDirectDependents(name));
            //Set to iterate through (make sure all indirect dependents are accounted for:
            HashSet<string> temp = new HashSet<string>(retSet);
            foreach (string dependent in temp)
            {
                //Add all dependents of the direct dependents of the cell:
                retSet.UnionWith(GetAllDependents(dependent));
            }
            //Return the set of all dependents
            return retSet;
        }

        /// <summary>
        /// Gets the saved version of the given filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Returns the version string from the given filename</returns>
        public override string GetSavedVersion(string filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                reader.ReadToFollowing("spreadsheet");
                reader.MoveToFirstAttribute();
                return reader.Value;
            }
        }

        /// <summary>
        /// Saves the spreadsheet to the specified filepath using the following format:
        /// 
        /// <spreadsheet version="version information here">
        ///   <cell>
        ///     <name>Cell_Name1</name>
        ///     <content>Cell_Content1</content>
        ///   </cell>
        ///   <cell>
        ///     <name>Cell_Name2</name>
        ///     <content>Cell_Content2</content>
        ///   </cell>
        /// </spreadsheet>
        /// 
        /// </summary>
        /// <param name="filename">The file path to save the sheet to</param>
        public override void Save(string filename)
        {
            //Settings for the XmlWriter class, used to allow indentation
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            //XmlWriter used to write the XML file
            XmlWriter xmlWriter = XmlWriter.Create(filename, settings);

            //Start Doc:
            xmlWriter.WriteStartDocument();
            //Write <spreadsheet version="">
            xmlWriter.WriteStartElement("spreadsheet");
            xmlWriter.WriteAttributeString("version", Version);

            //Write each cell that is empty:
            foreach (string name in GetNamesOfAllNonemptyCells())
            {
                //Write <cell>
                xmlWriter.WriteStartElement("cell");
                //Write <name>
                xmlWriter.WriteStartElement("name");
                //Write the stored name
                xmlWriter.WriteString(name);
                //Write </name>
                xmlWriter.WriteEndElement();
                //Write <content>
                xmlWriter.WriteStartElement("contents");
                //If the cell's contents is a formula, write "=" plus the formula
                if (allCells[name].contents is Formula)
                {
                    xmlWriter.WriteString("=" + allCells[name].contents.ToString());
                }
                //Othewise write the contents of the cell
                else
                {
                    xmlWriter.WriteString(allCells[name].contents.ToString());
                }
                //Write closing tags
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            //End the doc
            xmlWriter.WriteEndDocument();
            //Close the writer
            xmlWriter.Close();
            //Create an XmlDocument
            XmlDocument doc = new XmlDocument();
            //Save the written XML to the specified filepath:
            doc.Save(xmlWriter);
            Changed = false;
        }

        /// <summary>
        /// Gets the value, not contents, of the given cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the value of the cell or a FormulaError object</returns>
        public override object GetCellValue(string name)
        {
            //If the name is null or is an empty cell throw an exception
            if (name == null || !allCells.ContainsKey(name))
            {
                throw new InvalidNameException();
            }
            //If the contents of the cell is a Formula, evaluate it using the lookup function
            if (allCells[name].contents is Formula)
            {
                return ((Formula)allCells[name].contents).Evaluate(lookup);
            }
            //Otherwise, just return the contents of the cell (should be a double or string)
            else
            {
                return allCells[name].contents;
            }
        }

        /// <summary>
        /// General method for setting the contents of a cell, determines the type (Formula, double, or string) of the cell and calls the appropriate SetCellContents method.
        /// </summary>
        /// <param name="name">Name of the cell to set the contents of</param>
        /// <param name="content">Content to be set in the named cell</param>
        /// <returns>Returns the set of dependees of the named cell</returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            //If there is a validator function
            if (IsValid != null)
            {
                //Attempt to validate name, throw exception if it fails to validate
                if (!IsValid(name))
                {
                    throw new InvalidNameException();
                }
                //If there is a normalizer, attempt to normalize then validate again, throw an exception if it fails to validate
                if (Normalize != null)
                {
                    if (!IsValid(Normalize(name)))
                    {
                        throw new InvalidNameException();
                    }
                    name = Normalize(name);
                }
            }
            else
            {
                //If there is a normalizer, normalize the name
                if(Normalize != null)
                {
                    name = Normalize(name);
                }
            }

            //Set up a double to attempt to parse to
            double parseVal;

            //Set changed to true
            Changed = true;

            if(content == null)
            {
                throw new ArgumentNullException();
            }
            else if (content.Equals(""))
            {
                return SetCellContents(name, content);
            }
            //If the content is a double, use the SetCellContents(string, double) method and return its results
            else if (double.TryParse(content, out parseVal))
            {
                return SetCellContents(name, parseVal);
            }
            //If the content's first character is an '=', then assume it's a Formula and use the SetCellContents(string, Formula) method
            else if (content[0] == '=')
            {
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            //Otherwise use the SetCellContents(string, string) method
            else
            {
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// Lookup function used to find the values of variables or throw an ArgumentException if the variable is undefined or not calculable
        /// </summary>
        /// <param name="var">The variable to lookup</param>
        /// <returns>Returns the value of the variable or throws an ArgumentException.</returns>
        private double lookup(string var)
        {
            //If the cell exists in the spreadsheet, continue lookup
            if (allCells.ContainsKey(var))
            {
                //If the variable results to a formula, attempt to evaluate it:
                if (allCells[var].contents is Formula)
                {
                    object result = ((Formula)allCells[var].contents).Evaluate(lookup);
                    //If the formula is not calculable (i.e. returns a FormulaError), throw an ArgumentException
                    if (result is FormulaError)
                    {
                        throw new ArgumentException("The formula associated with " + var + " is invalid!");
                    }
                    else
                    {
                        return (double) result;
                    }
                }

                //If the cell is a string, throw an ArgumentException (shouldn't try to evaluate "test" + 2)
                else if (allCells[var].contents is string)
                {
                    throw new ArgumentException("The value of " + var + " is a string, and therefore, not calculable!");
                }
                //If the cell with the given variable contains a double return it:
                else if (allCells[var].contents is double)
                {
                    return (double)allCells[var].contents;
                }
                //If none of the above apply throw an ArgumentException
                else
                {
                    throw new ArgumentException("The value of " + var + " is not a string, double, or formula!");
                }
            }
            //If the cell is not in the sheet
            else
            {
                throw new ArgumentException("The variable " + var + " is not a cell in the spreadsheet!");
            }
        }

        /// <summary>
        /// Container for a spreadsheet's cell. Contains both the name of the cell and its contents.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// The contents of the cell.
            /// </summary>
            public object contents;

            /// <summary>
            /// The name of the cell.
            /// </summary>
            public string name;

            /// <summary>
            /// Common constructor that takes a string and an object. Sets the name to the string parameter and contents to the object parameter.
            /// </summary>
            /// <param name="name">The to-be name of the cell.</param>
            /// <param name="value">The to-be contents of the cell.</param>
            public Cell(string name, object contents)
            {
                this.name = name;
                this.contents = contents;
            }
        }
    }
}
