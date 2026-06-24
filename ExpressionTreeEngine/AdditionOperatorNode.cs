namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles addition Operator.
    /// </summary>
    internal class AdditionOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionOperatorNode"/> class.
        /// </summary>
        /// <param name="left">Left Node.</param>
        /// <param name="right">Right Node.</param>
        public AdditionOperatorNode(Node left, Node right)
            : base(left, right)// initializes the left and right nodes like Operator Node
        {
            this.OperatorSymbol = '+';
            this.Precedence = 1;
            this.Associtivy = false;
        }

        /// <summary>
        /// Abstract evaluate method.
        /// </summary>
        /// <returns>The sum of our Nodes.</returns>
        public override double Evaluate()
        {
            return this.leftNode.Evaluate() + this.rightNode.Evaluate();
        }
    }
}
