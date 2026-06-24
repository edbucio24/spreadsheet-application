namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Our Constant Node Class.
    /// </summary>
    internal class ConstantNode : Node // Inherit from our Node Base Class
    {
        // Private thing in our Constant Node
        private double constantValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="newoConstant">Constant Node for expression.</param>
        public ConstantNode(double newoConstant)
        {
            this.constantValue = newoConstant;
        }

        /// <summary>
        /// Override the evaluate function
        /// that way it returns this Node.
        /// </summary>
        /// <returns>Constant Value.</returns>
        public override double Evaluate()
        {
            return this.constantValue;
        }
    }
}
