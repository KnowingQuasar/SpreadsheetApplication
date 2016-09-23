//Written by Ian Rodriguez 09/22/2016
//Purpose: Generic Formula class to evaluate infix expressions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// Represents the formula after being parsed through the constructor
        /// </summary>
        private List<string> parsedFormula = new List<string>();

        /// <summary>
        /// Array representing the allowed operators for infix expressions
        /// </summary>
        private string[] allowedOperators = { "*", "/", "+", "-", "(", ")" };

        /// <summary>
        /// Validates tokens and returns their type. "Op" for operator and "Num/Var" for number/variable. 
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="FormulaFormatException">Thrown if the token is not an operator or a number/variable.</exception>
        /// <returns>Returns the type of the token. "Op" for operator and "Num/Var" for number/variable.</returns>
        private string TokenValidator(string token)
        {
            //Used to satisfy the double.TryParse parameters
            double outToken;

            //If the token is an operator, return "Op"
            if (allowedOperators.Contains(token))
                return "Op";
            else
            {
                //If the token is a variable or number return "Num/Var"
                if (Regex.Match(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success || double.TryParse(token, out outToken))
                    return "Num/Var";
            }
            //Throw a FormulaFormatException if it is not an operator or number/variable
            throw new FormulaFormatException("The token \"" + token + "\" is invalid. Please validate the formula contents.");
        }
        /// <summary>
        /// This method is used to add operands together and remove them and the associated operator from the stack.
        /// </summary>
        /// <param name="operands">The stack containing the operands at the current stage of evaluation</param>
        /// <param name="operators">The stack containing the operators at the current stage of evaluation</param>
        private void Add(Stack<string> operators, Stack<string> operands)
        {
            //Remove the addition operator
            operators.Pop();
            //Check if there are enough operands for addition
            if (operands.Count < 2)
                throw new FormulaFormatException("Cannot execute expression - not enough operands to execute addition. Please verify your expression's addition.");
            //Do the addition and remove the operands
            double opResult = double.Parse(operands.Pop()) + double.Parse(operands.Pop());
            //Put the result in the operands Stack
            operands.Push(opResult.ToString());
        }

        /// <summary>
        /// This method is uesd to subtract one operand from another and remove them and the associated operator from the stack.
        /// </summary>
        /// <param name="operands">The stack containing the operands at the current stage of evaluation</param>
        /// <param name="operators">The stack containing the operators at the current stage of evaluation</param>
        private void Subtract(Stack<string> operators, Stack<string> operands)
        {
            //Remove the subtraction operator
            operators.Pop();
            //Doubles to house the operands. Used to preserve the order of numbers as a Stack is a LIFO structure.
            double op1;
            double op2;
            op2 = double.Parse(operands.Pop());
            op1 = double.Parse(operands.Pop());
            //Do the subtraction
            double opResult = op1 - op2;
            //Push the result onto the operands Stack
            operands.Push(opResult.ToString());
        }
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        /// <param name="formula">String containing the infix expression</param>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            //Essentially this calls the second constructor with no validator or normalizer
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        /// <param name="formula">The string containing the infix expresstion</param>
        /// <param name="isValid">The validator function used to validate variables</param>
        /// <param name="normalize">The normalize function used to normalize variables</param>
        /// <exception cref="FormulaFormatException">If at any point the standard rules of arithmetic are seen to be broken, this type of exception will be thrown.</exception>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            //List of tokens retrieved by the GetTokens() function
            List<string> tokens = GetTokens(formula).ToList<string>();

            //If the expression is empty throw an exception
            if (tokens.Count <= 0)
                throw new FormulaFormatException("Cannot evaluate expression, no tokens are present.");

            //Counts of the number of opening/closing parantheses
            int openingParanthesisCount = 0;
            int closingParanthesisCount = 0;

            //Validate and parse tokens:
            for (int i = 0; i < tokens.Count; i++)
            {
                //Get the current token
                string token = tokens.ToArray()[i];
                //Get the current token's type and validate it
                string tokenType = TokenValidator(token);

                //If the token is the last in the list
                if ((i + 1) == tokens.Count)
                {
                    //If the token is not an end paranthesis or a number/variable, throw a FormulaFormatException
                    if (!token.Equals(")") && !tokenType.Equals("Num/Var"))
                        throw new FormulaFormatException("The given expression does not end in a number, variable, or closing paranthesis. Please validate that the given expression is correct and ends in a number, variable, or closing paranthesis.");
                }

                //Create an empty string to hold the next token in the list (or remain empty if the token is the last one in the list)
                string nextToken = "";
                string nextTokenType = "";

                //If the token isn't the last token, get the next token and its type 
                if (!((i + 1) == tokens.Count))
                {
                    nextToken = tokens.ToArray()[i + 1];
                    nextTokenType = TokenValidator(nextToken);
                }

                //If the token is the first one in the list
                if (i == 0)
                    if (!tokenType.Equals("Num/Var") && !token.Equals("(")) //Assure the first token is a number or opening paranthesis
                        throw new FormulaFormatException("The first token in the expression was not a number or variable");

                //If the token is an opening paranthesis
                if (token.Equals("("))
                {
                    //Increase the count of opening parantheses
                    openingParanthesisCount++;
                    //Make sure that the next token is either a number or opening paranthesis
                    if (!nextTokenType.Equals("Num/Var") && !nextToken.Equals("("))
                        throw new FormulaFormatException("An opening paranthesis is followed by a non-number or variable! Please validate that the paranthetical expression has the correct format.");
                }
                //If the token is a closing paranthesis
                else if (token.Equals(")"))
                {
                    //Increase the count of closing parantheses
                    closingParanthesisCount++;
                    //Make sure that the amount of closing parantheses isn't greate than opening parantheses
                    if (closingParanthesisCount > openingParanthesisCount)
                        throw new FormulaFormatException("There is a \")\" without a \"(\". Please validate that the formula has enough opening parantheses as closing.");
                }
                //If the token is a number/variable
                else if (tokenType.Equals("Num/Var"))
                {
                    //Used to satisfy double.TryParse
                    double outToken;

                    //If the token is a number
                    if (!double.TryParse(token, out outToken))
                    {
                        //If there is a validator function
                        if (isValid != null)
                        {
                            //If there is a normalize function
                            if (normalize != null)
                            {
                                //If the token isn't valid, throw a FormulaFormatException
                                if (!isValid(normalize(token)))
                                    throw new FormulaFormatException("The validator failed to validate this token: \"" + token + "\". Please make sure that the validate function allows this variable.");
                                //Otherwise, normalize the token and prepare it to be put into the parsedFormula list
                                token = normalize(token);
                            }
                            //If there isn't a normalize function
                            else
                            {
                                //If the token isn't valid, throw a FormulaFormatException
                                if (!isValid(token))
                                    throw new FormulaFormatException("The validator failed to validate this token: \"" + token + "\". Please make sure that the validate function allows this variable.");
                            }
                        }
                        //If there is not validator function, but there is a normalize function
                        else if (normalize != null)
                            //Normalize the function and store it in the parsedFormula list
                            token = normalize(token);
                    }
                    //If there is no validator 
                    else
                    {
                        //Parse the token as a double and prepare it to be put into the parsedFormula list
                        token = double.Parse(token).ToString();
                    }
                    //If the token is a number or variable, make sure that the next token is an operator or closing paranthesis
                    if (!nextToken.Equals(")") && !nextTokenType.Equals("Op") && (!((i + 1) == tokens.Count)))
                        throw new FormulaFormatException("The number \"" + token + "\" is not followed by a number or variable, but \"" + nextToken + "\". Please make sure that the given number is followed by a valid operator or closing parantheis.");
                }
                //Add the processed token to the parsedFormula list
                parsedFormula.Add(token);
            }
            //If the amount of opening parantheses and closing parantheses doesn't match, throw a FormulaFormatException
            if (openingParanthesisCount != closingParanthesisCount)
                throw new FormulaFormatException("The expression is not balanced. There are " + openingParanthesisCount + " opening parantheses and " + closingParanthesisCount + " closing parantheses.");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        /// <param name="lookup">Function used to lookup values of variables</param>
        /// <returns>Returns either the double result from evaluating the expression or a FormulaError object if the evaluation fails.</returns>
        public object Evaluate(Func<string, double> lookup)
        {
            //Stacks containing operators and operands used for calculation
            Stack<string> operators = new Stack<string>();
            Stack<string> operands = new Stack<string>();

            //Evaluate the expression:
            foreach (string token in parsedFormula)
            {
                //Used to get the double from the double.TryParse method
                double numberToken;

                //If the token is a number/variable
                if (!allowedOperators.Contains(token))
                {
                    //If the token is not a variable or number return a FormulaError object
                    if (!Regex.Match(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success && !double.TryParse(token, out numberToken))
                        return new FormulaError("There is an invalid character in the expression: \"" + token + "\". Please verify that the character entered is the desired one");
                    //If the token is a variable
                    if (!(double.TryParse(token, out numberToken)))
                    {
                        //If there is no lookup function return a FormulaError object
                        if (lookup == null)
                            return new FormulaError("Cannot evaluate expression. The expression contains variables, but no Lookup function is defined! Please define a valid lookup function.");
                        //Attempt to lookup the value for the variable and store it for calculation
                        numberToken = lookup(token);
                    }

                    //If there are not operators
                    if (operators.Count == 0)
                    {
                        //Push the number/variable onto the operands stack and exit loop
                        operands.Push(numberToken.ToString());
                        continue;
                    }
                    //If there are operators
                    else
                    {
                        //Used to preserve order of operations (i.e. 3 - 2 doesn't become 2 - 3)
                        double op1;
                        //If the top operator on the stack is multiplication
                        if (operators.Peek().Equals("*"))
                        {
                            //If there are no operands for multiplication, return a FormulaError object
                            if (operands.Count == 0)
                                return new FormulaError("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            //Parse the top value of the operands stack into op1
                            double.TryParse(operands.Pop(), out op1);
                            //Pop the multiplication operator from the stack
                            operators.Pop();
                            //Multiply and push onto the operands stack. Exit loop.
                            operands.Push((op1 * numberToken).ToString());
                            continue;
                        }
                        //If the top operator on the stack is division
                        else if (operators.Peek().Equals("/"))
                        {
                            //If there are no operands for division, return a FormulaError object
                            if (operands.Count == 0)
                                return new FormulaError("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            //If attempting to divide by 0, return a FormulaError object
                            if (numberToken == 0)
                                return new FormulaError("Cannot execute expression - dividing by 0!");
                            //Parse the top value of operands into op1
                            double.TryParse(operands.Pop(), out op1);
                            //Pop the division from the operators stack
                            operators.Pop();
                            //Divide and push onto operands stack. Exit loop.
                            operands.Push((op1 / numberToken).ToString());
                            continue;
                        }
                        //Otherwise, push the number/variable onto the stack
                        else
                            operands.Push(numberToken.ToString());
                    }
                }
                //If the token is an operator
                else if (allowedOperators.Contains(token))
                {
                    //If the token is + or -
                    if (token.Equals("+") || token.Equals("-"))
                    {
                        //If there are no operators, push the token onto the stack
                        if (operators.Count == 0)
                            operators.Push(token);
                        else
                        {
                            //If the top operator is a + or -
                            if (operators.Peek().Equals("+") || operators.Peek().Equals("-"))
                            {
                                //If there are not enough operands for addition or subtraction, return a FormulaError object
                                if (operands.Count < 2)
                                    return new FormulaError("Cannot execute expression - not enough arguments for the specified operators.");
                                //If the operator is a +, add the operands
                                if (operators.Peek().Equals("+"))
                                    Add(operators, operands);
                                //If the operator is a -, subtract the operands
                                else if (operators.Peek().Equals("-"))
                                    Subtract(operators, operands);
                            }
                            //Push the operator onto the stack
                            operators.Push(token);
                        }
                    }
                    //If the token is a *, / or ( push it onto the stack
                    else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                        operators.Push(token);
                    //If the token is a )
                    else if (token.Equals(")"))
                    {
                        //If there is not a (, return a FormulaError object
                        if (operators.Count == 0)
                            return new FormulaError("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        //If the top operator is a +, add the operands
                        else if (operators.Peek().Equals("+"))
                            Add(operators, operands);
                        //If the top operator is a -, subtract the operands
                        else if (operators.Peek().Equals("-"))
                            Subtract(operators, operands);
                        //If there are no operators, return a FormulaError object
                        if (operators.Count == 0)
                            return new FormulaError("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        //If the top operators is not a (, return a FormulaError object
                        else if (!(operators.Peek().Equals("(")))
                            return new FormulaError("Cannot execute expression - a \"(\" was not found in the expected location or is missing.");
                        //Otherwise, remove the top operator from the stack
                        else
                            operators.Pop();

                        //If there is at least one operator in the stack
                        if (!(operators.Count == 0))
                        {
                            //If the top operator is a *
                            if (operators.Peek().Equals("*"))
                            {
                                //If there are not enough operands for multiplication, return a FormulaError object
                                if (operands.Count < 2)
                                    return new FormulaError("Cannot execute expression - there are not enough operands for multiplication.");
                                //Pop the top two operands from the stack and push the product onto the stack
                                operands.Push((int.Parse(operands.Pop()) * int.Parse(operands.Pop())).ToString());
                                //Remove the top operator from the stack
                                operators.Pop();
                            }
                            //If the top operator is a /
                            else if (operators.Peek().Equals("/"))
                            {
                                //Doubles used to preserve order of operations
                                double op1;
                                double op2;
                                //Parse the top operand into op2 (as it is the denominator)
                                op2 = double.Parse(operands.Pop());
                                //If the divisor is 0, return a FormulaError object
                                if (op2 == 0)
                                    return new FormulaError("Cannot execute expression - one or more parts of this equation would result in a division by 0.");
                                //Parse the next operand into op1 (numerator)
                                op1 = double.Parse(operands.Pop());
                                //Divde and push onto the stack
                                operands.Push((op1 / op2).ToString());
                                //Pop the top operator from the stack
                                operators.Pop();
                            }
                        }
                    }
                }
                //If the token is not a number/variable or an operator, return a FormulaError object
                else
                {
                    return new FormulaError("Cannot execute expression - it contains an illegal operator. Legal operators: +, -, /, *, (, ).");
                }
            }

            //After evaluating all tokens in the expression:

            //If there is only one value in the operands stack, that is the result. Return the value.
            if (operators.Count == 0 && operands.Count == 1)
                return double.Parse(operands.Pop());
            else
            {
                //If the operands stack is empty after evaluation, return a FormulaError objet
                if (operands.Count == 0)
                    return new FormulaError("Cannot execute expression - no expression has been given.");
                //If there isn't exactly one operator (must be a + or -) and two operands, return a FormulaError object
                if (!(operators.Count == 1) || !(operands.Count == 2))
                    return new FormulaError("Cannot execute expression - the last operation does not have the needed amount of operands or operators.");
                //If the top operator is a +, Add the values
                else if (operators.Peek().Equals("+"))
                    Add(operators, operands);
                //If the top operator is a -, Subtract the values
                else if (operators.Peek().Equals("-"))
                    Subtract(operators, operands);
                //Return the final result
                return double.Parse(operands.Pop());
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        /// <returns>Returns an IEnumerable of the normalized (if a normalize function is present) variables in the exprssion.</returns>
        public IEnumerable<String> GetVariables()
        {
            //Check each token in the parsedFormula list
            foreach (string token in parsedFormula)
                //If the token is a variable, add it to the IEnumerable return value
                if (Regex.Match(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success)
                    yield return token;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        /// <returns>Returns a string representation of the Formula object</returns>
        public override string ToString()
        {
            //String to be filled out and returned
            string retString = "";

            //Add each token in parsedFormula to the return string
            foreach (string token in parsedFormula)
            {
                //If the token is a (, don't add a space after putting the token into the return string
                if (token.Equals("("))
                {
                    retString += token;
                    continue;
                }
                //If the token is a ), remove the previous space and add the token and a space to the return string
                else if (token.Equals(")"))
                {
                    retString = retString.Remove(retString.Length - 1);
                }
                //Otherwise add the token and a space to the return string
                retString += token + " ";
            }
            //Return a trimmed return string
            return retString.Trim();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        /// <returns>A boolean representing if one Formula object equals another.</returns>
        public override bool Equals(object obj)
        {
            //If the obj is null or not a Formula object, return false
            if (obj == null || !(obj is Formula))
                return false;
            //For simplicity equality is done by comparing hash codes. If one hash code is equal to another, return true
            if (GetHashCode() == obj.GetHashCode())
                return true;
            //Otherwise, return false
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        /// <returns>A boolean representing if one Formula object equals another.</returns>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(null, f1) && ReferenceEquals(null, f2))
                return true;
            if (ReferenceEquals(null, f1) || ReferenceEquals(null, f2))
                return false;
            return (f1.GetHashCode() == f2.GetHashCode());
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        /// <returns>A boolean representing if one Formula object does NOT equal another.</returns>
        public static bool operator !=(Formula f1, Formula f2)
        {
            //Negate the == operator for return value
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// 
        /// For the Formula class, hashing is done by hashing the ToString() of the object. Instead of making my own hashing function,
        /// the C# string.GetHashCode() is used. This ensures that they are very unique, yet equal if the Formula are equivalent as the ToString()
        /// method is directly related to the numbers, variables, and operators in the object.
        /// </summary>
        /// <returns>The hash code for the Formula object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        /// <returns>Returns an IEnumberable of all tokens in a given formula.</returns>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method. Generally houses an error message detailing how
    /// the Evaluate method failed.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason">The message detailing the failure generated by Formula.Evaluate</param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}