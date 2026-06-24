namespace ExpressionTreeEngine
{
    using System.Xml.Linq;

    /// <summary>
    /// Class for our Expression Tree.
    /// </summary>
    public class ExpressionTree
    {
        public string expression;
        private Node root;
        private char[] charactersAllowed = { '+', '-', '*', '/' };
        public static Dictionary<string, double> variables = new Dictionary<string, double>();
        private OperatorNodeFactory myFactory = new OperatorNodeFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression">Our expression.</param>
        public ExpressionTree(string expression)
        {
            variables.Clear();// Resets the variables.
            this.expression = expression; // Simple Constructor
            this.root = this.BuildTree(expression);
        }

        /// <summary>
        /// Method to parse our expression.
        /// </summary>
        /// <param name="newexpression">Expression Given.</param>
        /// <returns>A list of our expression.</returns>
        private List<string> ParseExpression(string newexpression)
        {
            List<string> tokens = new(); // Initialize our list where we will hold each element of our expression(Tokens)
            string current = ""; // Temp

            // Go through each element in the expression
            foreach (char item in newexpression)
            {
                if (char.IsWhiteSpace(item))// if its a white space, ignore it
                    continue;
                if (char.IsLetterOrDigit(item) || item == '.')// '.' so that we can handle double numbers like 1.1
                {
                    current += item;// if we hit a number or digit, add it to our temp string
                }
                else // we hit an operator
                {
                    if (current != "") // we have to see the element that we had
                    {
                        tokens.Add(current);// add this element to the list
                        current = "";// reset our temp string
                    }

                    tokens.Add(item.ToString()); // add the operator to the list
                }
            }

            if (current != "")
            {
                tokens.Add(current); // add that final element into our string
            }

            return tokens;// return our list of tokens
        }

        /// <summary>
        /// Shunting Alg required for the HW.
        /// </summary>
        /// <param name="elements">Expression Given.</param>
        /// <returns>List in correct way.</returns>
        private static List<string> ShuntingYardAlgorithm(List<string> elements)
        {
            List<string> output = new(); // our finalized expression
            Stack<string> operators = new Stack<string>(); // stack where we will keep our operators
            OperatorNodeFactory myFactory = new(); // factory of our function

            // iterate through our list of elements
            foreach (string token in elements)
            {
                if (double.TryParse(token, out _) || char.IsLetter(token[0]))// check to see if we have a number or variable
                {
                    output.Add(token); // into our finalized list
                }
                else if (token == "(")
                {
                    operators.Push(token); // if the next token is a left parantheses, push to our stack
                }
                else if (token == ")") // if our token is the right parantheses
                {
                    while (operators.Count > 0 && operators.Peek() != "(") // pop until we get to the left parantheses  LIFO
                        output.Add(operators.Pop()); // pop into our finalized list

                    if (operators.Count == 0) // handles mismatched parantheses
                        throw new InvalidOperationException("Odd number of parantheses.");
                    operators.Pop();
                }
                else // we have hit an operator
                {
                    OperatorNode currentOperator = myFactory.CreateOperatorNode(token[0]); // we create am operator node

                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        OperatorNode higheroperator = myFactory.CreateOperatorNode(operators.Peek()[0]);
                        if (higheroperator.Precedence >= currentOperator.Precedence) // check to see the precedence of our operators
                            output.Add(operators.Pop());
                        else
                            break;
                    }

                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop()); // add final parts of operators
            }

            return output; // return the finalized product
        }

        /// <summary>
        /// Build our Expression Tree.
        /// </summary>
        /// <param name="expression">Our given esxpression tree.</param>
        /// <returns>The tree.</returns>
        private Node BuildTree(string expression)
        {
            List<string> tokens = this.ParseExpression(expression); // parse our string
            List<string> shuntingresult = ShuntingYardAlgorithm(tokens); // put that string into the shunting alg

            Stack<Node> stack = new(); // stack

            // loop through our list in order
            foreach (string token in shuntingresult)
            {
                if (double.TryParse(token, out double val))
                {
                    stack.Push(new ConstantNode(val)); // if it is a number, make a constant node
                }
                else if (char.IsLetter(token[0]))
                {
                    if (!variables.ContainsKey(token))
                    {
                        variables[token] = 0.0;

                    }

                    stack.Push(new VariableNode(token)); // if it is a variable, make a variable node
                }
                else
                {
                    Node right = stack.Pop();
                    Node left = stack.Pop();

                    OperatorNode newNode = myFactory.CreateOperatorNode(token[0]); // create the node from the factory
                    newNode.setLeft(left); // setters for the node
                    newNode.setright(right);
                    stack.Push(newNode); // push it to the stack
                }
            }

            return stack.Pop(); // pop
        }

        /// <summary>
        /// Set the variable value
        /// to the variable name that we want to change.
        /// </summary>
        /// <param name="variableName">Variable that we will change.</param>
        /// <param name="variableValue">Value that we want to set it to./param>
        public void SetVariable(string variableName, double variableValue)
        {
            variables[variableName] = variableValue; // setting the variable name to the value inputed
                                                     // e.g. variables[Hello] = 42
        }

        /// <summary>
        /// Function to reutrn names in the expression.
        /// </summary>
        /// <returns>Variable names.</returns>
        public List<string> GetVariableName()
        {
            var variables = new HashSet<string>();
            this.Traverse(root, variables); // traverse through the tree and collect the variables
            return new List<string>(variables);// return our variables
        }

        /// <summary>
        /// Helper function for GetVariableName().
        /// </summary>
        /// <param name="node">Node of the Cell.</param>
        /// <param name="variables">HashSet of the variables.</param>
        private void Traverse(Node node, HashSet<string> variables)
        {
            // if our node is a variable node
            if (node is VariableNode varNode)
            {
                variables.Add(varNode.VariableName); // collect the variable that we found
            }
            else if (node is OperatorNode operators)
            {
                // traverse through the tree
                this.Traverse(operators.GetLeft(), variables);
                this.Traverse(operators.GetRight(), variables);
            }
        }

        /// <summary>
        /// Overides from our base class in Node.
        /// </summary>
        /// <returns>The total value of our ExpressionTree.</returns>
        public double Evaluate()
        {
            return this.root.Evaluate();// Evalutes the total Value of the tree for option menu 3
        }
    }
}
