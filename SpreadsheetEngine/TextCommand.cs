namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Will handle Undo/Redo for text in cells.
    /// </summary>
    public class TextCommand : ICommand
    {
        /// <summary>
        /// Gets the Cell of the Text Command.
        /// </summary>
        public Cell Cell { get; set; }

        /// <summary>
        /// Gets or Sets the undo command.
        /// </summary>
        public string UndoText { get; set; }

        /// <summary>
        /// Gets or Sets the redo command.
        /// </summary>
        public string RedoText { get; set; }

        /// <summary>
        /// Gets or Sets the Action of the command.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCommand"/> class.
        /// </summary>
        /// <param name="cell">Cell that we are changing.</param>
        /// <param name="undoText">The old text.</param>
        /// <param name="redoText">The new text needed when we redo.</param>
        public TextCommand(Cell cell, string undoText, string redoText)
        {
            this.Cell = cell;
            this.UndoText = undoText;
            this.RedoText = redoText;
            this.Action = "Text Change";
        }

        /// <summary>
        /// Will handle Undo logic for text command.
        /// </summary>
        public void Undo()
        {
            this.Cell._text = this.UndoText; // set the current cells text to undo text
            this.Cell.SetValue(this.UndoText); // set value for this cell to the undo text
            if (this.Cell is NewCell newcell) // check to see if this cell has the value updated event
            {
                newcell.OnCellValueUpdated(); // fire event
            }
        }

        /// <summary>
        /// Handles Redo logic for text commands.
        /// </summary>
        public void Redo()
        {
            this.Cell._text = this.RedoText; // set the current text to the redo text.
            this.Cell.SetValue(this.RedoText); // set the text to the redo text.
            if (this.Cell is NewCell newcell)
            {
                newcell.OnCellValueUpdated(); // fire event
            }
        }
    }
}
