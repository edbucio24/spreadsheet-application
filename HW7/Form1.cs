namespace HW7
{
    using SpreadsheetEngine;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Class that handles the UI.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet demoSpreadsheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.AddColumns();
            this.AddRows();
            this.demoSpreadsheet = new Spreadsheet(50, 26);// Inititalize our spreadsheet
            this.demoSpreadsheet.CellChanged += this.SpreadsheetChanged; // subscribe to event changes from class
            this.demoSpreadsheet.CellValueUpdated += (cell) =>
            {
                this.dataGridView1.Rows[cell.rowIndex].Cells[cell.columnIndex].Value = cell._value; // event that handles visual
            };
            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit; // subscribing to the event changes
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
            this.UpdateUndoRedoButtons();
        }

        /// <summary>
        /// Property changed from class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void SpreadsheetChanged(object? sender, PropertyChangedEventArgs e)
        {

            NewCell cell = (NewCell)sender;// Get the Cell that triggered the new event

            // check to see if we changed the value of the cell
            if (e.PropertyName == nameof(NewCell._value))
            {
                this.dataGridView1.Rows[cell.rowIndex].Cells[cell.columnIndex].Value = cell._value;
            }

            // check to see if we changed the color of the cell
            else if (e.PropertyName == nameof(NewCell.BGcolor))
            {
                this.dataGridView1.Rows[cell.rowIndex].Cells[cell.columnIndex].Style.BackColor = System.Drawing.Color.FromArgb((int)cell.BGcolor);
            }
        }

        /// <summary>
        /// Adds all 26 columns A-Z.
        /// </summary>
        public void AddColumns()
        {
            for (char columnName = 'A'; columnName <= 'Z'; columnName++) // traverse our alphabet
            {
                this.dataGridView1.Columns.Add(columnName.ToString(), columnName.ToString()); // add each column
            }
        }

        /// <summary>
        /// Adds our 50 rows to spreasheet.
        /// </summary>
        public void AddRows()
        {
            for (int rowName = 1; rowName <= 50; rowName++)// Traverse 50 times
            {
                this.dataGridView1.Rows.Add();// Add the Row
                this.dataGridView1.Rows[rowName - 1].HeaderCell.Value = rowName.ToString();// To ensure its not under first column ('A')
            }
        }

        /// <summary>
        /// When we begin editing the cell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Cell cell = this.demoSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex); // cell we are currentluy editing
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell._text; // get the text of the cell swap from the value
        }

        /// <summary>
        /// The end of the edit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string? userTextInput = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? ""; // Reads what the user inputed but can be empty
            Cell cell = this.demoSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex); // cell that we want to change

            // this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell._value;// switching back to the formualted value
            if (cell._text != userTextInput)
            {
                string old = cell._text; // store the current text as old text
                string newtext = userTextInput; // new text is the userinput

                TextCommand command = new TextCommand(cell, old, newtext); // track the old and new text of the cell
                this.demoSpreadsheet.AddUndo(command); // add the command to the undo stack
                this.UpdateUndoRedoButtons(); // update whether we can use the undo/redo buttons
                cell._text = newtext; // update the text
                cell.SetValue(newtext); // fire the event for any subcsribed classes
            }

            this.UpdateUndoRedoButtons(); // make sure undo/redo buttons are valid
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles changing the color of the cell.
        /// </summary>
        /// <param name="sender">The cender.</param>
        /// <param name="e">The object.</param>
        private void changeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog myDialog = new ColorDialog()) // used looking at the documentation
            {
                if (myDialog.ShowDialog() == DialogResult.OK)
                {
                    var cells = new List<(NewCell cell2, uint old, uint newcolor)>(); // list of cells that got their color changed
                    foreach (DataGridViewCell dgvCell in this.dataGridView1.SelectedCells)
                    {
                        int row = dgvCell.RowIndex;
                        int column = dgvCell.ColumnIndex;

                        NewCell cell = (NewCell)this.demoSpreadsheet.GetCell(row, column); // grab the cell that were celected
                        cells.Add((cell, cell.BGcolor, (uint)myDialog.Color.ToArgb())); // see what colors changed
                        cell.BGcolor = (uint)myDialog.Color.ToArgb(); // apply the color to the cell
                    }

                    ColorCommand command = new ColorCommand(cells); // create the command that we just made
                    this.demoSpreadsheet.AddUndo(command); // add the command to the undo stack
                    this.UpdateUndoRedoButtons(); // updates our toolstri[ undo/redo
                }
            }
        }

        /// <summary>
        /// Handles the undo Button.
        /// </summary>
        /// <param name="sender">Handles the sender.</param>
        /// <param name="e">The object.</param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.demoSpreadsheet.UndoValid())
            {
                this.demoSpreadsheet.Undo();
                this.UpdateUndoRedoButtons();
            }
        }

        /// <summary>
        /// Handles Redo button function.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The object.</param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.demoSpreadsheet.RedoValid())
            {
                this.demoSpreadsheet.Redo();
                this.UpdateUndoRedoButtons();
            }
        }

        /// <summary>
        /// Handles updating the button text.
        /// </summary>
        private void UpdateUndoRedoButtons()
        {
            this.undoToolStripMenuItem.Enabled = this.demoSpreadsheet.UndoValid();
            this.redoToolStripMenuItem.Enabled = this.demoSpreadsheet.RedoValid();

            string nextundo = this.demoSpreadsheet.NextUndo();
            string nextredo = this.demoSpreadsheet.NextRedo();

            if (nextundo != null)
            {
                this.undoToolStripMenuItem.Text = $"Undo {nextundo}";
            }
            else
            {
                this.undoToolStripMenuItem.Text = "Undo";
            }

            if (nextredo != null)
            {
                this.redoToolStripMenuItem.Text = $"Redo {nextredo}";
            }
            else
            {
                this.redoToolStripMenuItem.Text = "Redo";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Tool button that saves our cells.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Object.</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog.Title = "Save Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile()) // save functionality taken from HW3
                    {
                        this.demoSpreadsheet.SaveXML(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Load Button,
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The object.</param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML Files (*.xml)|*.xml";
                openFileDialog.Title = "Load Spreadsheet";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream s = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        this.demoSpreadsheet.LoadXML(s); // load functionality taken from HW3
                    }
                    this.demoSpreadsheet.Recalculate(); // recalculate the formulas needed
                    this.RefreshUI(); // needed to show the correct datagridview
                }
            }
        }

        /// <summary>
        /// Helper function that loads the datagridview.
        /// </summary>
        private void RefreshUI()
        {
            for (int i = 0;i<50;i++)
            {
                for (int j = 0;j<26;j++) // Iterate through our Excel Sheet
                {
                    Cell cell = this.demoSpreadsheet.GetCell(i, j);
                    this.dataGridView1.Rows[i].Cells[j].Value = cell._value; // set the value
                    this.dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.FromArgb((int)cell.BGcolor); // set the color
                }
            }

            this.dataGridView1.Refresh(); // makes sure the changes to the spreadsheet are visible.
        }
    }
}
