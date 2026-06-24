namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to create operator nodes.
    /// </summary>
    internal class OperatorNodeFactory
    {
        /// <summary>
        /// Determines which operator we Need.
        /// </summary>
        /// <param name = "op" > Our given operator.</param>
        /// <returns>The correct operator Node.</returns>
        /// <exception cref = "InvalidOperationException" > Handles unwanted operators/</exception>
        public OperatorNode CreateOperatorNode(char op)
        {
            switch (op)
            {
                case '+':
                    return new AdditionOperatorNode(null, null);
                case '-':
                    return new SubtractionOperatorNode(null, null);
                case '*':
                    return new MultiplicationOperartorNode(null, null);
                case '/':
                    return new DivisionOperatorNode(null, null);
                default:
                    throw new InvalidOperationException("Wrong operator");
            }
        }
    }
}