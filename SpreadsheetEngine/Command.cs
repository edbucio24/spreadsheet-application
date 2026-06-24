namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Command interface for Undo/Redo.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the Action of the Text or Color command.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// Will handle Undo fucntions.
        /// </summary>
        void Undo();

        /// <summary>
        /// Will handle Redo Functions.
        /// </summary>
        void Redo();
    }
}
