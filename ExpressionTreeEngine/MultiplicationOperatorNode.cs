namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles when we multiply.
    /// </summary>
    internal class MultiplicationOperartorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplicationOperartorNode"/> class.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The Right Node.</param>
        public MultiplicationOperartorNode(Node left, Node right)
            : base(left, right)
        {
            this.OperatorSymbol = '*';
            this.Precedence = 2;
            this.Associtivy = true;
        }

        /// <summary>
        /// Abstract Evaluate class.
        /// </summary>
        /// <returns>The multiplication of our Nodes.</returns>
        public override double Evaluate()
        {
            return this.leftNode.Evaluate() * this.rightNode.Evaluate();
        }
    }
}
