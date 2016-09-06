//Author: Ian Rodriguez (u1071551)
//Date: 09/06/2016
//Purpose: This function is designed to evaluate infix expressions. It allows any combination
//         of numbers and variables of the form (in Regex) [a-zA-Z]+[0-9]+ and the following
//         operators: +, -, /, *, ), (. Any and all arithmetic and number format errors are 
//         handled by throwing a System.ArgumentException.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Contains methods used to evaluate infix expressions and return an integer result.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// A Stack data structure used to hold expression numbers and variables. 
        /// </summary>
        private static Stack<string> operands = new Stack<string>();

        /// <summary>
        /// A Stack data Structure used to hold operators. Acceptable operators: *, /, +, -, (, ).
        /// </summary>
        private static Stack<string> operators = new Stack<string>();

        /// <summary>
        /// Array used to determine if a given token is a valid operator, and conversely used to tell if a token is a number.
        /// </summary>
        private static string[] allowedOperators = { "*", "/", "+", "-", "(", ")" };

        /// <summary>
        /// Delegate used to lookup the values of variables in expressions using the passed in method.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The looked up value for the passed in variable.</returns>
        public delegate int Lookup(string value);

        /// <summary>
        /// This method is used to add operands together and remove them and the associated operator from the stack.
        /// </summary>
        private static void add()
        {
            operators.Pop();
            if (operands.Count < 2)
                throw new ArgumentException("Cannot execute expression - not enough operands to execute addition.");
            int opResult = int.Parse(operands.Pop()) + int.Parse(operands.Pop());
            operands.Push(opResult.ToString());
        }

        /// <summary>
        /// This method is uesd to subtract one operand from another and remove them and the associated operator from the stack.
        /// </summary>
        private static void subtract()
        {
            operators.Pop();
            int op1;
            int op2;
            op2 = int.Parse(operands.Pop());
            op1 = int.Parse(operands.Pop());
            int opResult = op1 - op2;
            operands.Push(opResult.ToString());
        }

        /// <summary>
        /// A method used for evaluating infix exprssions with respect to the standard order of operations. Accepts non-negative integers and variables. Legal operators: *, /, -, +, (, ).
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>Returns the result of evaluating the expression as an integer.</returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            operands.Clear();
            operators.Clear();

            string exp_nowhitespace = Regex.Replace(exp, @"\s+", "");
            string[] tokens = Regex.Split(exp_nowhitespace, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (string token in tokens)
            {
                if (token.Equals(""))
                    continue;

                int numberToken;
                if (!allowedOperators.Contains(token))
                {
                    if (!Regex.Match(exp, @"^[a-zA-Z]+[0-9]+").Success && !int.TryParse(token, out numberToken))
                        throw new ArgumentException("Cannot execute expression - an illegal operator is present.");

                    if (!(int.TryParse(token, out numberToken)))
                        numberToken = variableEvaluator(token);

                    if (operators.Count == 0)
                    {
                        operands.Push(numberToken.ToString());
                        continue;
                    }
                    else
                    {
                        int op1;
                        if (operators.Peek().Equals("*"))
                        {
                            if (operands.Count == 0)
                                throw new ArgumentException("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            int.TryParse(operands.Pop(), out op1);
                            operators.Pop();
                            operands.Push((op1 * numberToken).ToString());
                            continue;
                        }
                        else if (operators.Peek().Equals("/"))
                        {
                            if (operands.Count == 0)
                                throw new ArgumentException("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            if (numberToken == 0)
                                throw new ArgumentException("Cannot execute expression - dividing by 0!");
                            int.TryParse(operands.Pop(), out op1);
                            operators.Pop();
                            operands.Push((op1 / numberToken).ToString());
                            continue;
                        }
                        else
                            operands.Push(numberToken.ToString());
                    }
                }
                else if (allowedOperators.Contains(token))
                {

                    if (token.Equals("+") || token.Equals("-"))
                    {
                        if (operators.Count == 0)
                            operators.Push(token);
                        else
                        {
                            if (operators.Peek().Equals("+") || operators.Peek().Equals("-"))
                            {
                                if (operands.Count < 2)
                                    throw new ArgumentException("Cannot execute expression - not enough arguments for the specified operators.");
                                if (operators.Peek().Equals("+"))
                                    add();
                                else if (operators.Peek().Equals("-"))
                                    subtract();
                            }
                            operators.Push(token);
                        }
                    }
                    else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                        operators.Push(token);
                    else if (token.Equals(")"))
                    {
                        if (operators.Count == 0)
                            throw new ArgumentException("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        else if (operators.Peek().Equals("+"))
                            add();
                        else if (operators.Peek().Equals("-"))
                            subtract();

                        if (operators.Count == 0)
                            throw new ArgumentException("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        else if (!(operators.Peek().Equals("(")))
                            throw new ArgumentException("Cannot execute expression - a \"(\" was not found in the expected location or is missing.");
                        else
                            operators.Pop();

                        if (!(operators.Count == 0))
                        {
                            if (operators.Peek().Equals("*"))
                            {
                                if (operands.Count < 2)
                                    throw new ArgumentException("Cannot execute expression - there are not enough operands for multiplication.");
                                operands.Push((int.Parse(operands.Pop()) * int.Parse(operands.Pop())).ToString());
                                operators.Pop();
                            }
                            else if (operators.Peek().Equals("/"))
                            {
                                int op1;
                                int op2;
                                op2 = int.Parse(operands.Pop());
                                if (op2 <= 0)
                                    throw new ArgumentException("Cannot execute expression - one or more parts of this equation would result in a division by 0.");
                                op1 = int.Parse(operands.Pop());
                                operands.Push((op1 / op2).ToString());
                                operators.Pop();
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot execute expression - it contains an illegal operator. Legal operators: +, -, /, *, (, ).");
                }
            }
            if (operators.Count == 0 && operands.Count == 1)
                return int.Parse(operands.Pop());
            else
            {
                if (operands.Count == 0)
                    throw new ArgumentException("Cannot execute expression - no expression has been given.");
                if (!(operators.Count == 1) || !(operands.Count == 2))
                    throw new ArgumentException("Cannot execute expression - the last operation does not have the needed amount of operands or operators.");
                else if (operators.Peek().Equals("+"))
                    add();
                else if (operators.Peek().Equals("-"))
                    subtract();
                return int.Parse(operands.Pop());
            }
        }
    }
}



