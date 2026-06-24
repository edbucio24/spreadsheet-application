namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///  This will handle the variables in a given line e.g. A + B + 3 , variables = A & B.
    /// </summary>
    internal class VariableNode : Node // Inherit frmo our class
    {

        // Private Property in our class
        private string variableName;
        private static Dictionary<string, double> variables = ExpressionTree.variables;

        /// <summary>
        /// Gets the name of the variable for the cell.
        /// </summary>
        public string VariableName => this.variableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="newVariable">The variable name inputed.</param>
        public VariableNode(string newVariable)
        {
            this.variableName = newVariable;
        }

        /// <summary>
        /// Override the abstract class from
        /// our base Node class.
        /// </summary>
        /// <returns>The value of the Variable.</returns>
        public override double Evaluate() // This evaluate is what drives option 2 in the menu
        {
            if (variables.ContainsKey(this.variableName)) // if we did set the variable to a value
            {
                return variables[this.variableName];// return that value
            }
            else
            {
                return 0.0; // if we didn't set it, just return 0
            }
        }
    }
}
