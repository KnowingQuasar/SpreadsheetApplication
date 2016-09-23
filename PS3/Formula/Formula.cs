// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

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
        private List<string> parsedFormula = new List<string>();
        private int hashCode;
        private string[] allowedOperators = { "*", "/", "+", "-", "(", ")" };

        private string TokenValidator(string token)
        {
            double outToken;
            if (allowedOperators.Contains(token))
                return "Op";
            else
            {
                if (Regex.Match(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success || double.TryParse(token, out outToken))
                    return "Num/Var";
            }
            throw new FormulaFormatException("The token \"" + token + "\" is invalid. Please validate the formula contents.");
        }
        /// <summary>
        /// This method is used to add operands together and remove them and the associated operator from the stack.
        /// </summary>
        private void Add(Stack<string> operators, Stack<string> operands)
        {
            operators.Pop();
            if (operands.Count < 2)
                throw new ArgumentException("Cannot execute expression - not enough operands to execute addition.");
            double opResult = double.Parse(operands.Pop()) + double.Parse(operands.Pop());
            operands.Push(opResult.ToString());
        }

        /// <summary>
        /// This method is uesd to subtract one operand from another and remove them and the associated operator from the stack.
        /// </summary>
        private void Subtract(Stack<string> operators, Stack<string> operands)
        {
            operators.Pop();
            double op1;
            double op2;
            op2 = double.Parse(operands.Pop());
            op1 = double.Parse(operands.Pop());
            double opResult = op1 - op2;
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
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
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
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            List<string> tokens = GetTokens(formula).ToList<string>();
            if (tokens.Count <= 0)
                throw new FormulaFormatException("Cannot evaluate expression, no tokens are present.");
            int openingParanthesisCount = 0;
            int closingParanthesisCount = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens.ToArray()[i];
                string tokenType = TokenValidator(token);
                if ((i + 1) == tokens.Count)
                {
                    if (!token.Equals(")") && !tokenType.Equals("Num/Var"))
                        throw new FormulaFormatException("The given expression does not end in a number, variable, or closing paranthesis. Please validate that the given expression is correct and ends in a number, variable, or closing paranthesis.");
                }
                string nextToken = "";
                string nextTokenType = "";
                if (!((i + 1) == tokens.Count))
                {
                    nextToken = tokens.ToArray()[i + 1];
                    nextTokenType = TokenValidator(nextToken);
                }
                if (i == 0)
                    if (!tokenType.Equals("Num/Var") && !token.Equals("("))
                        throw new FormulaFormatException("The first token in the expression was not a number or variable");
                if (token.Equals("("))
                {
                    openingParanthesisCount++;
                    if (!nextTokenType.Equals("Num/Var") && !nextToken.Equals("("))
                        throw new FormulaFormatException("An opening paranthesis is followed by a non-number or variable! Please validate that the paranthetical expression has the correct format.");
                }
                else if (token.Equals(")"))
                {
                    closingParanthesisCount++;
                    if (closingParanthesisCount > openingParanthesisCount)
                        throw new FormulaFormatException("There is a \")\" without a \"(\". Please validate that the formula has enough opening parantheses as closing.");
                }
                else if (tokenType.Equals("Num/Var"))
                {
                    double outToken;
                    if (!double.TryParse(token, out outToken))
                    {
                        if (isValid != null)
                        {
                            if (normalize != null)
                            {
                                if (!isValid(normalize(token)))
                                    throw new FormulaFormatException("The validator failed to validate this token: \"" + token + "\". Please make sure that the validate function allows this variable.");
                                token = normalize(token);
                            }
                            else
                            {
                                if (!isValid(token))
                                    throw new FormulaFormatException("The validator failed to validate this token: \"" + token + "\". Please make sure that the validate function allows this variable.");
                            }
                        }
                        else if (normalize != null)
                            token = normalize(token);
                    }
                    else
                    {
                        token = double.Parse(token).ToString();
                    }
                    if (!nextToken.Equals(")") && !nextTokenType.Equals("Op") && (!((i + 1) == tokens.Count)))
                        throw new FormulaFormatException("The number \"" + token + "\" is not followed by a number or variable, but \"" + nextToken + "\". Please make sure that the given number is followed by a valid operator or closing parantheis.");
                }
                parsedFormula.Add(token);
            }
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
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<string> operators = new Stack<string>();
            Stack<string> operands = new Stack<string>();
            foreach (string token in parsedFormula)
            {
                double numberToken = -1;
                if (!allowedOperators.Contains(token))
                {
                    if (!Regex.Match(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*").Success && !double.TryParse(token, out numberToken))
                        return new FormulaError("There is an invalid character in the expression: \"" + token + "\". Please verify that the character entered is the desired one");

                    if (!(double.TryParse(token, out numberToken)))
                    {
                        if (lookup == null)
                            return new FormulaError("Cannot evaluate expression. The expression contains variables, but no Lookup function is defined! Please define a valid lookup function.");
                        numberToken = lookup(token);
                        if (numberToken == -1)
                            return new FormulaError("Cannot evaluate expression. The lookup function does not have a value for the variable \"" + token + "\". Please make sure that the lookup function has a value for the variable.");
                    }

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
                                return new FormulaError("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            int.TryParse(operands.Pop(), out op1);
                            operators.Pop();
                            operands.Push((op1 * numberToken).ToString());
                            continue;
                        }
                        else if (operators.Peek().Equals("/"))
                        {
                            if (operands.Count == 0)
                                return new FormulaError("Expression contains an illegal operation. There is no left-hand operand for the operation!");
                            if (numberToken == 0)
                                return new FormulaError("Cannot execute expression - dividing by 0!");
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
                                    return new FormulaError("Cannot execute expression - not enough arguments for the specified operators.");
                                if (operators.Peek().Equals("+"))
                                    Add(operators, operands);
                                else if (operators.Peek().Equals("-"))
                                    Subtract(operators, operands);
                            }
                            operators.Push(token);
                        }
                    }
                    else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                        operators.Push(token);
                    else if (token.Equals(")"))
                    {
                        if (operators.Count == 0)
                            return new FormulaError("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        else if (operators.Peek().Equals("+"))
                            Add(operators, operands);
                        else if (operators.Peek().Equals("-"))
                            Subtract(operators, operands);

                        if (operators.Count == 0)
                            return new FormulaError("Cannot execute expression - there is a \")\" without a matching \"(\".");
                        else if (!(operators.Peek().Equals("(")))
                            return new FormulaError("Cannot execute expression - a \"(\" was not found in the expected location or is missing.");
                        else
                            operators.Pop();

                        if (!(operators.Count == 0))
                        {
                            if (operators.Peek().Equals("*"))
                            {
                                if (operands.Count < 2)
                                    return new FormulaError("Cannot execute expression - there are not enough operands for multiplication.");
                                operands.Push((int.Parse(operands.Pop()) * int.Parse(operands.Pop())).ToString());
                                operators.Pop();
                            }
                            else if (operators.Peek().Equals("/"))
                            {
                                int op1;
                                int op2;
                                op2 = int.Parse(operands.Pop());
                                if (op2 <= 0)
                                    return new FormulaError("Cannot execute expression - one or more parts of this equation would result in a division by 0.");
                                op1 = int.Parse(operands.Pop());
                                operands.Push((op1 / op2).ToString());
                                operators.Pop();
                            }
                        }
                    }
                }
                else
                {
                    return new FormulaError("Cannot execute expression - it contains an illegal operator. Legal operators: +, -, /, *, (, ).");
                }
            }
            if (operators.Count == 0 && operands.Count == 1)
                return double.Parse(operands.Pop());
            else
            {
                if (operands.Count == 0)
                    return new FormulaError("Cannot execute expression - no expression has been given.");
                if (!(operators.Count == 1) || !(operands.Count == 2))
                    return new FormulaError("Cannot execute expression - the last operation does not have the needed amount of operands or operators.");
                else if (operators.Peek().Equals("+"))
                    Add(operators, operands);
                else if (operators.Peek().Equals("-"))
                    Subtract(operators, operands);
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
        public IEnumerable<String> GetVariables()
        {
            foreach (string token in parsedFormula)
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
        public override string ToString()
        {
            string retString = "";
            foreach (string token in parsedFormula)
            {
                if (token.Equals("("))
                {
                    retString += token;
                    continue;
                }
                else if (token.Equals(")"))
                {
                    retString = retString.Remove(retString.Length - 1);
                    retString += token + " ";
                    continue;
                }
                retString += token + " ";
            }
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
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formula))
                return false;
            if (GetHashCode() == obj.GetHashCode())
                return true;
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(null) && f2.Equals(null))
                return true;
            if (f1.Equals(null) && !f2.Equals(null))
                return false;
            return (f1.GetHashCode() == f2.GetHashCode());
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
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
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
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