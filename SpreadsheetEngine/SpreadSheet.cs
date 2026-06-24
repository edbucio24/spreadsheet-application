namespace SpreadsheetEngine
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using ExpressionTreeEngine;

    /// <summary>
    /// Class for our spreadsheet.
    /// </summary>
    public class Spreadsheet
    {
        private Cell[,] newExcelSheet;

        private int rowCount { get; set; }

        private int columnCount { get; set; }

        /// <summary>
        /// Event that will handle visual changes.
        /// </summary>
        public event Action<Cell>? CellValueUpdated;

        public event PropertyChangedEventHandler CellChanged = delegate { };

        /// <summary>
        /// Stack containing all our undos.
        /// </summary>
        private Stack<ICommand> Undos = new Stack<ICommand>();

        /// <summary>
        /// Stack containing all our redos.
        /// </summary>
        private Stack<ICommand> Redos = new Stack<ICommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows">The number of rows we want.</param>
        /// <param name="columns">The number of columns we want.</param>
        public Spreadsheet(int rows, int columns)
        {
            this.newExcelSheet = new Cell[rows, columns];
            this.rowCount = rows;
            this.columnCount = columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.newExcelSheet[i, j] = new NewCell(i, j);// create 2D array, make sure we make a cell
                    this.newExcelSheet[i, j].PropertyChanged += CellPropertyChanged;  // subscribe to the event change
                }
            }
        }

        /// <summary>
        /// Get Cell function that returns the current cell.
        /// </summary>
        /// <param name="row">Row fo the cell.</param>
        /// <param name="column">Column of the cel.</param>
        /// <returns>The cell we want.</returns>
        public Cell GetCell(int row, int column)
        {
            if ((Cell)this.newExcelSheet[row, column] == null) // IF it is null, ignore/return null  have to cast
            {
                return null;
            }
            else
            {
                return (Cell)this.newExcelSheet[row, column];// else, return the cell , make sure to downcast
            }
        }

        /// <summary>
        /// When a value of my cell changes.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">the sender.</param>
        private void CellPropertyChanged(object? sender, EventArgs e)
        {
            NewCell newCell = (NewCell)sender;
            char column;
            int columnIndex;
            int rowIndex;

            if (string.IsNullOrEmpty(newCell._text))// check to see if the ce;ll text is still empty (string.empty)
            {
                // the user did not change the input
                newCell.SetValue("");// set the value to ""/(empty string)
            }
            else if (newCell._text.StartsWith('=') == false)// hw problem, check to see that it doesn't start with '='
            {
                // if it doesnt start with '=' then that means that its not a formula
                // the text is the userinput, no formula means we can just set the value of the string
                // to the text
                // text = rawinput from the user
                newCell.SetValue(newCell._text);// since it does, set the value to the text
            }
            else // we have an =
            {
                try
                {
                    string CellText = newCell._text.Substring(1); // remove =
                    ExpressionTree newTree = new ExpressionTree(CellText); // make an expression tree

                    List<string> cellsNeeded = newTree.GetVariableName(); // we are geting the variable names

                    HashSet<NewCell> prevCells = new HashSet<NewCell>(); // initialize the hashset for circular reference
                    bool circular = this.CircularReferenceCheck(newCell, prevCells, cellsNeeded); // return whether we found a circular reference
                    if (circular) // if true
                    {
                        return; // no node to execute the rest of the function
                    }

                    string myCell = this.ConvertToCellName(newCell); // get current cell's name, e.g., "A1"
                    if (cellsNeeded.Any(cell => cell.Equals(myCell))) // check to see if my converted cell matches any in the list
                    {
                        newCell.SetValue("!self reference"); // set the cell to self reference
                        this.CellChanged?.Invoke(newCell, new PropertyChangedEventArgs(nameof(Cell._value))); // fire event
                        return;
                    }

                    foreach (string indexCell in cellsNeeded)
                    {
                        if (!this.ValidName(indexCell))
                        {
                            newCell.SetValue("!(bad reference)");
                            this.CellChanged?.Invoke(newCell, new PropertyChangedEventArgs(nameof(Cell._value))); // fire event
                            return;
                        }
                    }

                    foreach (string cell in cellsNeeded)
                    {
                         column = cell[0]; // if text = B2, then we are making our column, B
                         columnIndex = cell[0] - 'A';
                         rowIndex = int.Parse(cell.Substring(1)) - 1; // we now delete B so we have just 2 we have to parse the string 2 into a int 2

                         Cell Cell2 = this.GetCell(rowIndex, columnIndex); // copy Cell2

                         if (double.TryParse(Cell2._value, out double constantNumber))
                        {
                            newTree.SetVariable(cell, constantNumber);// set variable to this num
                        }
                        else
                        {
                            newTree.SetVariable(cell, 0);
                            // throw new Exception("Cell no good");
                        }

                        // Subscribe to referenced cell so if it changes, this cell also updates
                         Cell2.PropertyChanged += (sender, event2) => // cell changed
                        {
                            this.CellPropertyChanged(newCell, EventArgs.Empty); // fire up event
                        };
                    }

                    double result = newTree.Evaluate(); // find the result of the expression
                    newCell.SetValue(result.ToString());// set that value in the cell
                }
                catch
                {
                    newCell.SetValue("!(REF)"); // out of bounds
                }

                this.CellChanged?.Invoke(newCell, new PropertyChangedEventArgs(nameof(Cell._value))); // fire event change
            }

            if (sender is Cell newCellThatChangedColor)
            {
                this.CellChanged?.Invoke(newCellThatChangedColor,new PropertyChangedEventArgs(nameof(Cell.BGcolor))); // fire event that a cell changed color
            }
        }

        /// <summary>
        /// Handles adding to our undo stack.
        /// </summary>
        /// <param name="newCommand">Command that we undo.</param>
        public void AddUndo(ICommand newCommand)
        {
            this.Undos.Push(newCommand); // add the command to our undo stack
            this.Redos.Clear();
        }

        /// <summary>
        /// Handles our undo Logic.
        /// </summary>
        public void Undo()
        {
            if (this.Undos.Count > 0) // if we have anymore undos
            {
                ICommand command = this.Undos.Pop(); // get the most resent command from the stack
                command.Undo(); // undo the command
                this.Redos.Push(command); // add the undo command in to our redo stack

                if (command is TextCommand textCommand)
                {
                    this.CellValueUpdated?.Invoke(textCommand.Cell); // since we caused changed, fire event for all cells subscribed
                }
            }
        }

        /// <summary>
        /// Handles the Redo logic.
        /// </summary>
        public void Redo()
        {
            if (this.Redos.Count > 0)
            {
                ICommand command = this.Redos.Pop();
                command.Redo();
                this.Undos.Push(command);

                if (command is TextCommand textCommand)
                {
                    this.CellValueUpdated?.Invoke(textCommand.Cell);
                }
            }
        }

        /// <summary>
        /// Saves Content from cells using XML.
        /// </summary>
        /// <param name="sr">Streamer File.</param>
        public void SaveXML(Stream sr)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            {
                settings.Indent = true;
                settings.NewLineOnAttributes = true; // new line for each attribut
                settings.IndentChars = "  ";
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(sr, settings)) // create the write for xml
            {
                xmlWriter.WriteStartElement("Spreadsheet"); // makes the <spreadsheet>

                foreach (Cell cell in this.NonDefault()) // iterate through the cells that are nondefault. not empty/non white
                {
                    char collett = (char)('A' + cell.columnIndex); // column name 
                    string rownum = (cell.rowIndex + 1).ToString(); // row number
                    string name = collett + rownum; // get the name of the cell
                    xmlWriter.WriteStartElement("cell"); // cell name attribute
                    xmlWriter.WriteAttributeString("name", name); // name of the cell is where the cell is

                    if (cell.BGcolor != 0xFFFFFFFF) // if the cell is not white
                    {
                        xmlWriter.WriteElementString("bgcolor", cell.BGcolor.ToString("X8")); // add the color attribute
                    }

                    if(!string.IsNullOrEmpty(cell._text))
                    {
                        xmlWriter.WriteElementString("text", cell._text); // write the text of the cell
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement(); // final <spreadsheet>
                xmlWriter.WriteEndDocument(); // close the xml document
            }
        }

        /// <summary>
        /// Helper function to return NonDefault Cells.
        /// </summary>
        /// <returns>All cells not in a default state.</returns>
        private IEnumerable<Cell> NonDefault()
        {
            int rows = this.newExcelSheet.GetLength(0); // initialize this to make our for loop
            int columns = this.newExcelSheet.GetLength(1); // look cleaner 

            for (int i = 0;i<rows; i++) // iterate through our 2D array
            {
                for (int j = 0;j<columns; j++)
                {
                    Cell cell = this.newExcelSheet[i,j];
                    if(!string.IsNullOrEmpty(cell._text) || cell.BGcolor != 0xFFFFFFFF) // if the cell is not empty or white, it must be default
                    {
                        yield return cell; // return all the cells that are not default
                    }
                }
            }
        }

        /// <summary>
        /// Function that will handle Loading XML.
        /// </summary>
        /// <param name="sr">The stream.</param>
        public void LoadXML(Stream sr)
        {
            this.ClearSheet(); // reset all the cells back to empty strings and white cells

            XDocument xmlDoc = XDocument.Load(sr); // load in the file that we want

            foreach (XElement element in xmlDoc.Descendants("cell")) // loop through all the the elements that have "cell"
            {
                string? name = element.Attribute("name")?.Value; // gets the name of the cell
                if (string.IsNullOrEmpty(name)) continue; // if no name, just skip it

                int column = name[0] - 'A';
                int row = int.Parse(name.Substring(1)) - 1; // convert to e.g. A1

                Cell cell = this.GetCell(row,column);
                if(cell == null) continue;

                XElement? textElement = element.Element("text"); // get the element text
                if(textElement != null) // if it is not an empty string
                {
                    cell._text = textElement.Value; // set the cell text to this
                }
                XElement? colorElement = element.Element("bgcolor");
                if(colorElement != null)
                {
                    cell.BGcolor = Convert.ToUInt32(colorElement.Value, 16); // same as text, set the cell to the loaded color
                }
            }

            this.Recalculate(); // recalculate formulas from the file loaded
        }

        /// <summary>
        /// Function that recalculates the functions.
        /// </summary>
        public void Recalculate()
        {
           for (int i = 0; i < this.rowCount; i++) // iterate throught the rows
            {
                for (int j = 0; j < this.columnCount; j++) // iterate throught the columns
                {
                    Cell cell = this.GetCell(i,j);

                    if (cell != null) // if the cell is not empty/no state
                    {
                        NewCell newcell = (NewCell)cell;
                        newcell.SetValue(newcell._text); // set the value/implementation for cells that have "="
                    }
                }
            }
        }

        /// <summary>
        /// Clears the sheet and undo/redo stacks.
        /// </summary>
        public void ClearSheet()
        {
            for(int i = 0;i<this.rowCount;i++) // iterate through our 2D array
            {
                for(int j = 0; j < columnCount; j++) // the columns
                {
                    NewCell cell = (NewCell)this.GetCell(i, j);
                    cell._text = string.Empty; // make the text empty
                    cell.SetValue(string.Empty); // value empty
                    cell.BGcolor = 0xFFFFFFFF; // color to default white
                }
            }

            this.Undos.Clear(); // clear the undo stack
            this.Redos.Clear(); // clear the redo stack
        }

        /// <summary>
        /// Makes the the given cell into e.g. A1.
        /// </summary>
        /// <param name="cellWanted">The cell given.</param>
        /// <returns>The cell in the form of A1.</returns>
        public string ConvertToCellName(Cell cellWanted)
        {
            int rowNumber = cellWanted.RowIndex + 1; // add 1 to account for spreadsheet
            int column = cellWanted.ColumnIndex; // getting the index of our column
            char columnLetter = (char)('A' + column); // converting into the asci value
            return columnLetter.ToString() + rowNumber.ToString(); // return our given cell in the form A1

        }

        /// <summary>
        /// Function that determines whether the name of cell
        /// inputed is valid.
        /// </summary>
        /// <param name="cellString">Expresion put.</param>
        /// <returns>Valid name or not.</returns>
        public bool ValidName(string cellString)
        {
            char columnName = cellString[0]; // get the first index of our expression
            int columnIndex = columnName - 'A';
            if (string.IsNullOrEmpty(cellString))
            {
                return false; // if the expression is empty, return false
            }

            if (columnIndex < 0 || columnIndex >= this.columnCount)
            {
                return false; // check to see that we are in bounds of A and Z
            }

            if (!int.TryParse(cellString.Substring(1),out int row)) // check to see that column is followed by a number
            {
                return false;
            }

            if (row < 1 || row > this.rowCount)
            {
                return false; // check to see that we are in bounds of 1 and 50
            }

            return true; // if passed, valid name
        }

        /// <summary>
        /// Will find whether we have a circular reference
        /// using a hashset where we check if the cell in the formula already
        /// appears in the formula.
        /// </summary>
        /// <param name="cell">Cell that has the formula.</param>
        /// <param name="prevCells">Hashset containing previous cells.</param>
        /// <param name="cellsInFormula">Cells in the formula.</param>
        /// <returns>Whether circular reference or not.</returns>
        public bool CircularReferenceCheck(NewCell cell, HashSet<NewCell> prevCells,List<string> cellsInFormula)
        {
            if (prevCells.Contains(cell)) // check to see if this cell is already in the hash map
            {
                cell.SetValue("!(circular reference)"); // if it is, we have a circular reference
                this.CellChanged?.Invoke(cell, new PropertyChangedEventArgs(nameof(Cell._value))); // fire event
                return true;
            }

            if (cellsInFormula == null || cellsInFormula.Count == 0)
            {
                return false; // deals with letting us allow the cell to equal 0 when we reference an empty cell
            }

            prevCells.Add(cell); // if not, add this cell in to the hashset

            foreach (string cellIndex in cellsInFormula) // for every cell in the list of cells required for the formula
            {
                if (string.IsNullOrWhiteSpace(cellIndex))
                {
                    continue;
                }

                if (!this.ValidName(cellIndex))
                {
                    continue;
                }

                if (cellIndex == this.ConvertToCellName(cell))
                {
                    continue;
                }

                int row = int.Parse(cellIndex.Substring(1)) - 1; // convert the cell into its coordinate form
                int column = cellIndex[0] - 'A'; // A1 = 0,0

                Cell refCell = this.GetCell(row, column); // get the cell we are referencing

                if (refCell is NewCell newCell)
                {
                    string text = newCell._text;

                    if (string.IsNullOrWhiteSpace(text) || text[0] != '=')
                    {
                        continue; // we don't care if the cell does not reference another cell
                    }

                    ExpressionTree tree = new ExpressionTree(newCell._text.Substring(1));
                    List<string> cellsForFormula = tree.GetVariableName(); // extract the variables

                    if (this.CircularReferenceCheck(newCell, prevCells, cellsForFormula))
                    {
                        return true; // recursivly check if this cell is a circular reference
                    }
                }
            }

            prevCells.Remove(cell); // update required to not get false positives
            return false; // if we got here, not a reference cell
        }

        /// <summary>
        /// Checks the stack if we have available undos.
        /// </summary>
        /// <returns>Yes or no.</returns>
        public bool UndoValid() => this.Undos.Count > 0;

        /// <summary>
        /// Checks stack to see if we have more redos.
        /// </summary>
        /// <returns>1 or 0.</returns>
        public bool RedoValid() => this.Redos.Count > 0;

        /// <summary>
        /// Peeks into our stack to see our next undo command.
        /// </summary>
        /// <returns>The next undo command.</returns>
        public string? NextUndo() => this.Undos.Count > 0 ? this.Undos.Peek().Action : null;

        /// <summary>
        /// Checks to to see the next redo command.
        /// </summary>
        /// <returns>The next redo command.</returns>
        public string? NextRedo() => this.Redos.Count > 0 ? this.Redos.Peek().Action : null;
    }
}
