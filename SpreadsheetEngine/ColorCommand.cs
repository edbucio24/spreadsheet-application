namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Will handle color commands.
    /// </summary>
    public class ColorCommand:ICommand
    {
        // list to handle undo/redo multiple cells
        private List<(NewCell cell, uint oldColor, uint newColor)> cellsThatChanged;

        /// <summary>
        /// Gets the Action Title.
        /// </summary>
        public string Action { get; } = "Background Color Change";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorCommand"/> class.
        /// </summary>
        /// <param name="cells">List of the cells that were changed.</param>
        public ColorCommand(List<(NewCell cell, uint oldColor, uint newColor)> cells) // Cell that was changed, old color, new color
        {
            this.cellsThatChanged = cells;
        }

        /// <summary>
        /// Undo logic for our color command.
        /// </summary>
        public void Undo()
        {
            foreach (var (cell, oldColor, _) in this.cellsThatChanged) // for each cell in our list
                cell.BGcolor = oldColor; // change back to the old color
        }

        /// <summary>
        /// Redo logic for our color command.
        /// </summary>
        public void Redo()
        {
            foreach (var (cell, _, newColor) in this.cellsThatChanged)
                cell.BGcolor = newColor; // change back to the new color
        }
    }
}
