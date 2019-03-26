using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathyStuff
{

    /// <summary>
    /// 
    /// This calss should contain all the things we need to interpret algebric and return a message to the user 
    /// that is going to be eigher
    /// the solution we need. Or an error mesage that tell the user exactly what is wrong. 
    /// </summary>
    public class Interpretor
    {

        #region fields
        public static readonly char[] OPERATORS={'+','-','*','/'};
        public static readonly IReadOnlyCollection<char> OPERATORSlist = new List<char>(OPERATORS);
        
        //The expression put in by the user. 
        public readonly String inputexpression;
        
        //public readonly String output;
        public readonly String outputresult; 

        // If this is true, it means the output result is an string 
        // that is the error message, default is true, because 
        //There is more ways to go wrong than go correctly. 
        protected bool  ErrorOccured =true;
        #endregion
        #region interpretor Constructor
        public Interpretor(String expression)
        {
            this.inputexpression = expression;

            try //Try to compute and display result onto the console. 
            {
                ExpressionDiegest ed = new ExpressionDiegest(inputexpression);
                IList<Object> explist = ed.getCastSplittedExpression();
                Queue<Object> postfix = infixtToPostFix(explist);

                //Evaluate the post fix and out put. 
                outputresult = evaluatePostFix(postfix).ToString();

                Utilities.Stuff.ConsoleLog(outputresult);

                ErrorOccured = false; 
            }
            catch (System.DivideByZeroException ohshit)
            {
                this.outputresult = "Infinity caused by dividing by 0.";
                Console.WriteLine();
            }
            catch (Exception e)
            {
                if (e is ExpressionDiegestError)
                {
                    ExpressionDiegestError ede = e as ExpressionDiegestError;
                    this.outputresult = ede.getErrorComments();
                    Console.WriteLine(ede.getErrorComments());
                }
                else
                {
                    this.outputresult = "Some Unknow error Occured...";
                    Utilities.Stuff.ConsoleLog(e.ToString());
                    Utilities.Stuff.ConsoleLog(e.HelpLink);
                    Utilities.Stuff.ConsoleLog(e.Message);
                    Utilities.Stuff.ConsoleLog(e.StackTrace.ToString());
                }
            }


        }
        #endregion

        /// <summary>
        /// True then there is error and the output message will 
        /// an string description of the errors instead
        /// of the correct result. 
        /// </summary>
        /// <returns></returns>
        public bool HasErrorOccured()
        {
            return this.ErrorOccured;
        }


        #region static methods cluster

        //***************Below Are all Static Methods*************************************************

        /// <summary>
        /// This method should be able to catch possible invalide expreession and sub expression and throw an exception.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete("Please don't use this method. ")]
        public static ProperFraction evaluateSimple(IList<Object> expression)
        {
            Stack<ProperFraction> numstack = new Stack<ProperFraction>();
            Stack<char> opstack = new Stack<char>();
            //Queue q = new Queue();
           

            //iterate through a queue of number, operator,or a sub expression.
            foreach(Object ele in expression)
            {
                //if is number
                if(ele is ProperFraction)
                {
                    //put into numberstack.
                    numstack.Push(ele as ProperFraction);
                }
                // elseif operator
                else if(ele is char)
                {
                    
                    if(opstack.Count==0)// if operator stack is empty, put it in. 
                    {
                        opstack.Push((char)ele);
                    }
                    else // else that stack is not empty. 
                    {
                        char opt = (char)ele;

                        if (compare(opt, opstack.Peek()) == 1)//if it's bigger to the most recent opt in stack
                        {
                            // put it in op stack. 
                            opstack.Push(opt);
                        }

                        else //else: it's leq to the top opt.
                        {
                            while (compare(opt, opstack.Peek()) < 1 || opstack.Count != 0)//while current op leq than top one on stack || operator stack is not empty.
                            {
                                // pop top 
                                char topoperator = opstack.Pop();
                                // pop two numbers if there is no more number on top, use zero
                                ProperFraction secondnumber = numstack.Pop();
                                ProperFraction firstnumber = null;
                                if (numstack.Count != 0) firstnumber = numstack.Pop();
                                //compute the top two number using the operator, the order matters. 
                                ProperFraction res = compute(firstnumber, secondnumber, opt);
                                //put the number back to the number stack. 
                                numstack.Push(res);
                            }
                            //put that operator in the opt stack? 
                            opstack.Push(opt);
                        }
                    }
                }
                else//else it must be sub expression
                {
                    if (!(ele is IList<Object>))// This shit if not a list
                    {
                        throw new Exception("Bad element: "+ele);
                    }

                    //Turn the sub expression into a number. 
                    ProperFraction subexpressionnumber = evaluateSimple((IList<Object>)ele);
                    numstack.Push(subexpressionnumber);
                }  
            }
            // Evaluate the remaining expression empty the stack;
            // return result. 

            ProperFraction subexpressionresult = evalueteStacks(numstack,opstack);
            if (subexpressionresult == null) throw new Exception("Expression or sub expression cannot be evaluated. "); 
            return subexpressionresult;

        }


        /// <summary>
        /// Method function properly under certain scope. 
        /// </summary>
        /// <param name="infix">
        /// List of type In64 and type char, the element should be integers, operators, parenthesis. 
        /// </param>
        /// <returns>
        /// null if error occured. 
        /// </returns>
        public static Queue<Object> infixtToPostFix(ICollection<Object> infix)
        {
            Queue<Object> result = new Queue<Object>();
            Stack<char> optstack = new Stack<char>();

            char unaryoperator = '\0';//If this is not null, it marks the presencce of an unary operator. 
            Object lasttoken = null;//This obejct is need to determine if the current operatier is a unary operator. 
            foreach (Object obj in infix)
            {
                if(obj is Int64)// If the token is number, add to output queue. 
                {
                    Int64 number = (Int64)obj;
                    if (unaryoperator != '\0')// The last token read is an unary operator on this token, a number. 
                    {
                        if (unaryoperator == '-' || unaryoperator == '+')
                        {
                            result.Enqueue(0L);//Force it to be a unary operator. 
                            unaryoperator = '\0'; // Rest it for the next unary operator that's going to be read. 
                        }
                        else return null; // Invalid Unary Operator. 
                    }
                    result.Enqueue(number);
                }
                else if(obj is char && (char)obj=='(')//else if the token is '(', put it into the operator stack. 
                {
                    optstack.Push('(');
                }
                else if(obj is char && (char)obj == ')')//else if the token is ')'
                {
                    while
                        (
                        optstack.Count!=0
                        &&!optstack.Peek().Equals((Object)'(')
                        )//while stack is not empty and the most recent element in the stack is not '('
                    {
                        result.Enqueue(optstack.Pop());
                        //pop it out and add it to output queue. 
                    }
                    //The stack is empty or the most recent element is '('
                    //discard '(' from the stack, or if stack is empty, return null; 
                    if (optstack.Count == 0)  throw new Exception();
                    optstack.Pop();
                }
                else//Else this token must be an operator. 
                {
                    if (!(obj is char)||!OPERATORSlist.Contains((char)obj) ) throw new Exception();
                    char currentoperator = (char)obj;

                    //If the operator appear right after ( or at the very beginning of the expression, 
                    //It will be a unary operator. 
                    if (lasttoken == null || (lasttoken is char && (char)lasttoken == '('))
                    {
                        unaryoperator = (char)obj;
                        optstack.Push(currentoperator);
                        lasttoken = currentoperator;
                        continue;
                       //read next token.
                    }
                   
                    // while stack is not empty and the operator is less than or equal to the most recent operator 
                    //on the stack
                    while
                        (
                        optstack.Count != 0
                        &&
                        compare(currentoperator, optstack.Peek()) <= 0 //This used to be a <=
                        )
                    {
                        result.Enqueue(optstack.Pop());
                        //pop the operator and add it to the output queue. 
                    }
                    optstack.Push(currentoperator);
                }
                

                lasttoken = obj;

            }

            //pop all remaining element from the stack to the queue. 
            while (optstack.Count != 0)
            {
                result.Enqueue(optstack.Pop());
            }
            return result; 
        }


        /// <summary>
        /// Return result of exact fraction from queue of postfix. 
        /// </summary>
        /// <param name="postFix"></param>
        /// <returns></returns>
        public static ProperFraction evaluatePostFix(Queue<Object> postFix)
        {
            if (postFix == null) return null;
            Queue<Object> Fractionlist = new Queue<Object>();
            //Cast all int64 into properFraction. 
            foreach (Object element in postFix)
            {
                if (element is Int64)
                {
                    Fractionlist.Enqueue(new ProperFraction((Int64)element));
                }
                else
                {
                    Fractionlist.Enqueue(element);
                }
            }

            Stack<ProperFraction> nstack = new Stack<ProperFraction>();
            // while it's more than one element in the queue. 
            while (Fractionlist.Count!=0)
            {
                Object obj = Fractionlist.Dequeue();
                //Retrieve oldest element.  
                if(obj is ProperFraction)//If it's a properfraction 
                {
                    nstack.Push((ProperFraction)obj);//add to the stack. 
                }
                else//else it must be operator
                {
                    if (!(obj is char)||nstack.Count==0) throw new Exception("Please Debug when this error happend.");
                    //pop two numbers from the statck and evaluate. 
                    ProperFraction secondnumber = nstack.Pop();
                    ProperFraction firstnumber = null;
                    if (nstack.Count != 0) firstnumber = nstack.Pop();
                    //if cannot get the second one and there is only one element, then it must be unary operator. 

                    // add the result to the stack.
                    nstack.Push(compute(firstnumber,secondnumber,(char)obj));
                }

            }

            return nstack.Pop();
        }


        public static bool isUnaryOpt(char arg)
        {
            return arg == '/';
        }
        /// <summary>
        /// If -1 is returned, the operator is invalid. 
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static int getOptRank(char opt)
        {
            switch (opt)
            {
                case '+':
                    return 1;
                case '-':
                    return 1;
                case '*':
                    return 2;
                case '/':
                    return 2;
            }

            //Parenthesis has the lowest rank. 
            return -1; 
        }

        /// <summary>
        /// If first bigger than second, return 1, return 0 iff equal, smaller iff -1 returned. 
        /// </summary>
        /// <returns></returns>
        public static int compare(char arg1, char arg2)
        {
            return getOptRank(arg1) - getOptRank(arg2);
        }

        /// <summary>
        /// method is tested. 
        /// <para>
        /// This method takes in two number and an operator and return the result. 
        /// </para>
        /// If there is only one number is not null, this method will assume the operator is '-' or '+';  
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// Null is the computation cannot be done or error has occured. 
        /// </returns>
        public static ProperFraction compute(ProperFraction a, ProperFraction b, char opt)
        {
            // the operator is valide: 
            if (getOptRank(opt) == -1) return null;
            // If there is one number at least: 
            if (a == null && b == null) return null;
            //At this point, the operator will be valide and there will be at least one numebr. 
            if (a != null && b != null)//Both bumber is not null
            {
                switch (opt)
                {
                    case '+':
                        return a + b;
                    case '-':
                        return a - b;
                    case '*':
                        return a * b;
                    case '/':
                        return a / b;
                }
                throw new Exception("Something wrong, please check consistency on logic.");
            }
            //One of the number is null.s
            if(opt == '-') return a == null ? -b:-a;
            if (opt == '+') return a == null ? b : a;
            return null;
        }


        /// <summary>
        /// 
        /// This method is an extension of evaluate simple. 
        /// </summary>
        /// <param name="numberstack"></param>
        /// <param name="optstack"></param>
        /// <returns>
        /// null if the input cannot evalueted. 
        /// </returns>
        [Obsolete("Outside the scope.")]
        public static ProperFraction evalueteStacks(Stack<ProperFraction> numberstack, Stack<char> optstack)
        {
            //make sure the input is both not null: 
            if (numberstack == null || optstack == null) return null;

            
             //If there is no more operator in the stack, that means there is only one number in the number stack, and it's 
            if(optstack.Count==0)// it's the base case:
            {
                //check if there is really one number in the stack: 
                if (numberstack.Count != 1) throw new Exception("Something shouldn't happen hapepened.");
                return numberstack.Pop();
            }

            //Comfirm that there is a Operator in the stack. 
            char opt = optstack.Pop();
            ProperFraction secondnumber = numberstack.Pop();
            ProperFraction firstnumber = null;
            if(numberstack.Count!=0) firstnumber = numberstack.Pop();
            ProperFraction computedresult = compute(firstnumber,secondnumber,opt);

            if (computedresult == null) return null; //This is hit if the number and the operator cannot be computed. 
            numberstack.Push(computedresult);
            return evalueteStacks(numberstack, optstack);
        }

        #endregion


        /// <summary>
        /// <para>
        /// This class take in a string and validate whether it's a 
        /// algebaric expression or not, if it is, it will parse into a list containing 
        /// integers or operator. </para>
        /// It also checks if the parenthesises are balanced. 
        /// </summary>
        public class ExpressionDiegest
        {
            #region ExpressionDiegest Fields
            protected String sourceexpression;
            protected String diagestedexpression;
            protected String[] splitedexpression ;

            protected IList<Object> castplistedexpression;
            #endregion

            public ExpressionDiegest(String expression)
            {
                if (expression == null || expression.Length < 1) throw new ExpressionDiegestError(1);
                sourceexpression = expression;
                if (!checkChar()) throw new ExpressionDiegestError(2);
                if (!checkParenthesis()) throw new ExpressionDiegestError(3);
                if (!varifyExpression()) throw new ExpressionDiegestError(4);
                addSpace();
                parseExpression();
                if (!castElements()) throw new ExpressionDiegestError(5); ;
            }

            public IList<Object> getCastSplittedExpression()
            {
                return castplistedexpression;
            }


            #region validation methods cluster

            /// <summary>
            /// <para>
            /// Only integers, parenthesis and elementary operators are allowed.
            /// </para>
            /// <para>
            /// The prenthesis must be matched. 
            /// </para>
            /// </summary>
            /// <returns></returns>
            protected bool checkChar()
            {
                foreach (char c in sourceexpression)
                {
                    if (c < '0'|| c > '9')//not a number
                    {
                        if (!OPERATORSlist.Contains(c))//not a operator. 
                        {
                            return c == ')' || c == '(';
                        }
                    }
                }
                return true;
            }

            protected bool checkParenthesis()
            {
                Int32 sum = 0;
                foreach (char c in this.sourceexpression)//foreach char in the string
                {
                    if (c != ')' || c != '(') //Not '(' or ')' skip
                        continue;
                    if (c == '(') // if it's left (, plus one
                        sum++;
                    else sum--;// if it's right ), minus one
                    if (sum < 0)// if -1, return false; 
                        return false;
                }
                return true;
            }


            /// <summary>
            /// This method is totally wrong. 
            /// </summary>
            /// <returns></returns>
            [Obsolete("This method has significant errors.")]
            protected bool checkNumberAndOperator()
            {
                Int32 sum = 1;
                foreach (char c in this.sourceexpression)
                {
                    if (c <= '9' || c >= '0') sum++;//add one if this is a number.
                    else if (OPERATORSlist.Contains(c)) sum--;//else minus one if this is a operator.
                    else sum = 1;//else this must be parenthesis, it will clear the sum to 1. 
                    if (sum < 0) return false; //if it's smaller than zero, there is something wrong with the expression. 
                }
                return true;
            }

            /// <summary>
            /// This method will try to see if the expression is valid, expression such as(--),--6+ will be 
            /// identified as this point.
            /// </summary>
            /// <returns>
            /// true if the expression is legit, false if it's not. 
            /// </returns>
            protected bool varifyExpression()
            {
                //For each of the characters in the string
                for (int i = 0; i < sourceexpression.Length; i++)
                {
                    char currenttoken = sourceexpression[i];
                    char nexttoken = i+1 < sourceexpression.Length ? sourceexpression[i + 1] : '\0';
                    char lasttoken = i == 0 ? '\0' : sourceexpression[i - 1];
                    // if the charactor is an operator.
                    if(getOptRank(currenttoken)>0)
                    {
                        // assert the right side is a number, or (
                        if (!(
                                (nexttoken <= '9' && nexttoken >= '0')||nexttoken=='('
                              )
                            )
                        {
                            return false;
                        }

                        //if the left side is either null(Last token not presented), or (. 
                        if (lasttoken == '\0' || lasttoken == '(')
                        {
                            //assert this is a unary opertor. 
                            if (currenttoken != '+' && currenttoken != '-')
                             return false;

                        }
                        else//else Assert the left side is a number or )
                        {
                            if (
                                !(
                                (lasttoken<='9'&&lasttoken>='0')
                                ||
                                lasttoken ==')'
                                )
                                )
                            return false; 
                        }
          
                    }

                }
                return true;
            }

            #endregion


            #region parsing and diegesting methods cluster
            /// <summary>
            /// 
            /// This method modify the current expression by adding space to left side and right side of the 
            /// operators or parenthesis making it easier to parse using split. 
            /// </summary>
            protected void addSpace()
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in sourceexpression)
                {
                    if (!OPERATORSlist.Contains(c) && c != ')' && c != '(')//if it's not an operator or parenthesis
                    {
                        sb.Append(c);
                    }
                    else//It is a operator or prenthesis or something unexpected. 
                    {
                        sb.Append(' ');
                        sb.Append(c);
                        sb.Append(' ');
                    }
                }
                this.diagestedexpression = sb.ToString();
            }
            /// <summary>
            /// Parse the expression into units of operator, integers, or parenthesis. 
            /// </summary>
            protected void parseExpression()
            {
                String[] splitedexpression = Regex.Split(diagestedexpression, @"\s+");
                this.splitedexpression = splitedexpression;
            }


            /// <summary>
            /// 
            /// Cast the splitted string into a list of integers and operators. 
            /// </summary>
            protected bool castElements()
            {
                IList<Object> expression = new List<Object>();
                foreach (String str in splitedexpression)
                {

                    if (str.Length == 0) continue; // Skip empty string. 

                    bool isinteger = Regex.IsMatch(str, @"\d+")&&str.Length<13;
                    bool OptOrParen = str.Length == 1
                                      &&
                                        ( 
                                        OPERATORSlist.Contains((char)str.ElementAt(0))
                                        || (char)str.ElementAt(0) == '(' || (char)str.ElementAt(0) == ')'
                                         );
                    if (isinteger) expression.Add(Int64.Parse(str));
                    else if (OptOrParen) expression.Add(str.ElementAt(0));
                    else return false ; // something is wrong is this line is executed. 
                }
                this.castplistedexpression = expression;
                return true;
            }
            #endregion

            public override string ToString()
            {
                String s = this.sourceexpression;
                s += '\n';
                s += this.diagestedexpression;
                s += "\nCast Splitted Expression: ";
                s += String.Join(" , ",this.castplistedexpression);
                return s;
            }
        }
                       
    }

    [Serializable]
    internal class ExpressionDiegestError : Exception
    {
        private int commentcode = 0;
        public ExpressionDiegestError()
        {
        }

        public ExpressionDiegestError(string message) : base(message)
        {
        }

        /// <summary>
        /// Give a code so that a comment message wrriten source code will be displayed through a method
        /// in this class when the exception is catched elsewhere. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commentcode"></param>
        public ExpressionDiegestError(int commentcode) : base("")
        {
            this.commentcode = commentcode;
        }

        public ExpressionDiegestError(string message, Exception innerException) : base(message, innerException)
        {
        }


        protected ExpressionDiegestError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        /// <summary>
        /// This method takes into a code and return  a comment that is supposed to be read by user in the user interface. 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public String getErrorComments()
        {
            switch (commentcode)
            {
                case 1:
                    return "The expression are two short or it's given as a null type.";
                case 2:
                    return "Character Issue, the input must only consists of number and primary operators[+,-,*,/] and spaces. ";
                case 3:
                    return "Parentesis Issue.Please check if your lost a parenthesis.";
                case 4:
                    return "Please check if the operators are in valid position and they are numbers or parenthesis besides it.";
                case 5:
                    return "The number cannot be longer than 12 digits, I know it's a bummer, but it's going to overflow if I allow that. ";
            }
            return commentcode.ToString(); 
        }
    }
}
