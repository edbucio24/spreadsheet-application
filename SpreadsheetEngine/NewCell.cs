namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to initialize a cell
    /// visual studios autofilled.
    /// </summary>
    public class NewCell : Cell // needed because Cell is an abstract class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewCell"/> class.
        /// </summary>
        /// <param name="row">Row of cell.</param>
        /// <param name="column">Column of cell.</param>
        public NewCell(int row, int column)
            : base(row, column)
        { }

        /// <summary>
        /// Event that will handle cell changing.
        /// </summary>
        public event Action<NewCell>? CellValueUpdated; // event used to handle changes in value/color

        /// <summary>
        /// Event handles value in cell changing.
        /// </summary>
        public void OnCellValueUpdated()
        {
            this.CellValueUpdated?.Invoke(this); // invoke/fire the event
        }

        /// <summary>
        /// Setter for Value.
        /// </summary>
        /// <param name="newvalue">The new value.</param>
        public void SetValue(string newvalue)
        {
            this._value = newvalue;
        }
    }
}
