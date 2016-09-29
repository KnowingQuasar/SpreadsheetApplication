using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Dictionary containing a group of associated names and their cell values.
        /// </summary>
        Dictionary<string, Cell> allCells;
        /// <summary>
        /// Dependency graph for the Spreadsheet. See the DependencyGraph documentation for details.
        /// </summary>
        DependencyGraph dg;

        public Spreadsheet()
        {
            allCells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// Internal function used broadly to validate that a name is non-null and fits the valid variable criteria.
        /// </summary>
        /// <param name="name">The name to be validated.</param>
        private void validateName(string name)
        {
            if (name == null)
                throw new InvalidNameException();
            if (!Regex.Match(name, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success)
                throw new InvalidNameException();
        }

        /// <summary>
        /// Gets the cell contents at the given name, if it exists. 
        /// </summary>
        /// <param name="name">The name of the cell to get the contents of.</param>
        /// <returns>Returns a string, double, or Formula object that represents the contents of the named cell.</returns>
        public override object GetCellContents(string name)
        {
            //Assure that the given name is valid
            validateName(name);
            //Create a container cell
            Cell retCell;
            //If the allCells dictionary is empty or doesn't contain the name in question, return an empty string.
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
                return "";
            //Otherwise get the value of the given name and return it.
            else
            {
                allCells.TryGetValue("name", out retCell);
                return retCell.value;
            }
                
        }

        /// <summary>
        /// Gets the names of all non-empty cells (all cells that do not have an empty string value).
        /// </summary>
        /// <returns>Returns an IEnumerable of all non-empty cells.</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Create a container cell
            Cell checkCell;
            //Look through all names in allCells:
            foreach(string name in allCells.Keys)
            {
                //If the cell has a value:
                if(allCells.TryGetValue(name, out checkCell))
                {
                    //If the value is not an empty string, add it to the return value:
                    if (!checkCell.value.Equals(""))
                        yield return checkCell.name;
                }
            }
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in Formula object.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The Formula object that will become the value of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            //If the Formula object is null, throw an ArgumentNullException:
            if (formula == null)
                throw new ArgumentNullException();
            //Assure that the given name is valid
            validateName(name);
            //Create container cell
            Cell editCell;
            //If the allCells Dictionary is empty or doesn't contain the named key, create a new cell and add it to the Dictionary.
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
            {
                editCell = new Cell(name, formula);
                allCells.Add(name, editCell);
            }
            //Otherwise, edit the current value of the named cell:
            else
            {
                allCells.TryGetValue(name, out editCell);
                editCell.value = formula;
            }
            //Return a HashSet of the named cell's dependees.
            return new HashSet<string>(dg.GetDependees(name));
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in string.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The string that will become the value of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            //Has the same logic as SetCellContents(string name, Formula formula), but uses a string instead of a Formula object. See the aforementioned method for details.

            if (text == null)
                throw new ArgumentNullException();
            validateName(name);
            Cell editCell;
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
            {
                editCell = new Cell(name, text);
                allCells.Add(name, editCell);
            }
            else
            {
                allCells.TryGetValue(name, out editCell);
                editCell.value = text;
            }
            return new HashSet<string>(dg.GetDependees(name));
        }

        /// <summary>
        /// Sets the contents of the named cell with the passed in double.
        /// </summary>
        /// <param name="name">The name of the cell to change the contents of.</param>
        /// <param name="formula">The double that will become the value of the named cell (if not null).</param>
        /// <returns>Returns a set of the names of the current cell and all its dependees.</returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            //Has the same logic as SetCellContents(string name, Formula formula), but uses a string instead of a Formula object. See the aforementioned method for details.
            //NOTE: does not check if the double is null, as the double data type is guaranteed to not be null.

            validateName(name);
            Cell editCell;
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
            {
                editCell = new Cell(name, number);
                allCells.Add(name, editCell);
            }
            else
            {
                allCells.TryGetValue(name, out editCell);
                editCell.value = number;
            }
            return new HashSet<string>(dg.GetDependees(name));
        }

        /// <summary>
        /// Gets the direct dependants of the given name, if it exists.
        /// </summary>
        /// <param name="name">The name of the cell to get the dependants of.</param>
        /// <returns>Returns an IEnumerable of the direct dependants of the given name.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
                throw new ArgumentNullException();
            validateName(name);
            if (allCells.Count == 0 || !allCells.ContainsKey(name))
                return null;
            return dg.GetDependents(name);
        }

        /// <summary>
        /// Container for a spreadsheet's cell. Contains both the name of the cell and its value.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// The value of the cell.
            /// </summary>
            public object value;
            /// <summary>
            /// The name of the cell.
            /// </summary>
            public string name;

            /// <summary>
            /// Common constructor that takes a string and an object. Sets the name to the string parameter and value to the object parameter.
            /// </summary>
            /// <param name="name">The to-be name of the cell.</param>
            /// <param name="value">The to-be value of the cell.</param>
            public Cell(string name, object value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}
