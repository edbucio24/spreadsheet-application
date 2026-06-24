namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Our Node Class.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Abstract class that we will
        /// use for all our Node classes.
        /// </summary>
        /// <returns>Evaluate Method for Node.</returns>
        public abstract double Evaluate();
    }
}
