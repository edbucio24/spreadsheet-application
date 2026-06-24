namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Formats.Asn1;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Cell class that is abstract for the spreadsheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        public int RowIndex;
        public int ColumnIndex;
        protected string Text;
        protected string Value;
        private uint BGColor = 0xFFFFFFFF; // Color property of Cell sit to default white.

        /// <summary>
        /// Event that will handle properties of
        /// cells changing.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="newRowIndex">Index row of the cell.</param>
        /// <param name="newColumnIndex">Column of the cell.</param>
        protected Cell(int newRowIndex, int newColumnIndex)
        {
            this.RowIndex = newRowIndex;
            this.ColumnIndex = newColumnIndex;
            this.Text = string.Empty;
            this.Value = string.Empty;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        public int rowIndex
        {
            get { return this.RowIndex; }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        public int columnIndex
        {
            get { return this.ColumnIndex; }
        }

        /// <summary>
        /// Gets or Sets the color of
        /// the cell.
        /// </summary>
        public uint BGcolor
        {
            get
            {
                return this.BGColor;
            }

            set
            {
                if (this.BGColor != value)
                {
                    this.BGColor = value;
                    this.OnPropertychanged(nameof(this.BGColor)); // copy the properties of text and value
                }
            }
        }

        /// <summary>
        /// Gets or Sets the text
        /// of the cell.
        /// </summary>
        public string _text
        {
            get { return this.Text; } // getter for text property

            set
            {
                if (this.Text != value)// make sure that the text isn't the same
                {
                    this.Text = value;// update the protexted field
                    this.OnPropertychanged(nameof(this._text));// if different text, fire the event
                }
            }
        }

        /// <summary>
        /// Gets or Sets the value
        /// of the cell.
        /// </summary>
        public string _value
        {
            get
            {
                return this.Value;// getter statement
            }

            protected set// only the spreadsheet can set it
            {
                if (this.Value != value)
                {
                    this.Value = value;
                    this.OnPropertychanged(nameof(this._value));// firing up property changed event
                }
            }
        }

        /// <summary>
        /// Handles firing event when we see a new event.
        /// </summary>
        /// <param name="newvalue">New value in the cell.</param>
        public void SetValue(string newvalue)
        {
            if (newvalue == null)
            {
                throw new ArgumentNullException(nameof(newvalue));
            }

            if (this.Value != newvalue)
            {
                this.Value=newvalue;
                this.OnPropertychanged(nameof(this._value));
            }
        }

        /// <summary>
        /// Fires the change in event
        /// autofilled by visual studios.
        /// </summary>
        /// <param name="newName">The new name.</param>
        protected void OnPropertychanged(string newName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(newName));
        }
    }
}
