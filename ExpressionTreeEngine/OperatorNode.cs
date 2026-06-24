namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles operator nodes.
    /// </summary>
    internal abstract class OperatorNode : Node
    {
        /// <summary>
        /// Symbol of the operator.
        /// </summary>
        public char OperatorSymbol { get; set; }// Symbol that we are given

        /// <summary>
        /// Will handle PEMDAS
        /// </summary>
        public int Precedence { get; set; } // Precedence = PEMDAS

        /// <summary>
        /// Gets the Associtivy of the expression.
        /// </summary>
        public bool Associtivy { get; set; }

        protected Node leftNode;
        protected Node rightNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="leftNode">The left node.</param>
        /// <param name="rightNode">The right node.</param>
        protected OperatorNode(Node leftNode, Node rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        internal void setLeft(Node left) => this.leftNode = left;

        internal void setright(Node right) => this.rightNode = right;

        /// <summary>
        /// Gets the left Node.
        /// </summary>
        /// <returns>The left node.</returns>
        internal Node GetLeft() => this.leftNode;

        /// <summary>
        /// Gets the right node.
        /// </summary>
        /// <returns>The right node.</returns>
        internal Node GetRight() => this.rightNode;

        /// <summary>
        /// Evaluate method abstact from Node.
        /// </summary>
        /// <returns>Evaluation of the nodes.</returns>
        public abstract override double Evaluate();
    }
}
