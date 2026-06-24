namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles Division Nodes.
    /// </summary>
    internal class DivisionOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionOperatorNode"/> class.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        public DivisionOperatorNode(Node left, Node right)
            : base(left, right)
        {
            this.OperatorSymbol = '/';
            this.Precedence = 2;
            this.Associtivy = true;

        }

        /// <summary>
        /// Abstract Evaluate class.
        /// </summary>
        /// <returns>The division of expression.</returns>
        /// <exception cref="DivideByZeroException">Can't Divide by zero.</exception>
        public override double Evaluate()
        {
            double right = this.rightNode.Evaluate();
            if (right == 0)
            {
                throw new DivideByZeroException("Wrong");
            }
            else
            {
                return this.leftNode.Evaluate() / this.rightNode.Evaluate();
            }

        }
    }
}
