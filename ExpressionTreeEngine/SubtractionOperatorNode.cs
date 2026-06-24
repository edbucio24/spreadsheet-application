namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles when we substtract Nodes.
    /// </summary>
    internal class SubtractionOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionOperatorNode"/> class.
        /// </summary>
        /// <param name="left">Left Node.</param>
        /// <param name="right">Right Node.</param>
        public SubtractionOperatorNode(Node left, Node right)
            : base(left, right)
        {
            this.OperatorSymbol = '-';
            this.Precedence = 1;
            this.Associtivy = false;

        }

        /// <summary>
        /// Abstract Evaluate Class.
        /// </summary>
        /// <returns>The difference between our Nodes.</returns>
        public override double Evaluate()
        {
            return this.leftNode.Evaluate() - this.rightNode.Evaluate();
        }
    }
}
