//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq.Expressions;

namespace NDO.Linq
{
    public class ExpressionTreeTransformer
    {
        class OperatorEntry
        {
            public OperatorEntry(ExpressionType exprType, string replacement, int prec)
            {
                this.ExpressionType = exprType;
                this.Precedence = prec;
                this.Replacement = replacement;
            }
            public ExpressionType ExpressionType;
            public string Replacement;
            public int Precedence;

			public static bool IsComparison(ExpressionType expType)
			{
				return expType == ExpressionType.GreaterThanOrEqual || expType == ExpressionType.GreaterThan || expType == ExpressionType.LessThan || expType == ExpressionType.LessThanOrEqual || expType == ExpressionType.Equal || expType == ExpressionType.NotEqual;
			}
        }

        static OperatorEntry[] operators =
        {
            new OperatorEntry(ExpressionType.Modulo, "%", 0),
            new OperatorEntry(ExpressionType.Multiply, "*", 0),
            new OperatorEntry(ExpressionType.Divide, "/", 0),
            new OperatorEntry(ExpressionType.Add, "+", 1),
            new OperatorEntry(ExpressionType.Subtract, "-", 1),
            new OperatorEntry(ExpressionType.GreaterThanOrEqual, ">=", 2),
            new OperatorEntry(ExpressionType.GreaterThan, ">", 2),
            new OperatorEntry(ExpressionType.LessThanOrEqual, "<=", 2),
            new OperatorEntry(ExpressionType.LessThan, "<", 2),
            new OperatorEntry(ExpressionType.NotEqual, "<>", 2),
            new OperatorEntry(ExpressionType.Equal, "=", 3),
            new OperatorEntry(ExpressionType.And, "AND", 4),
            new OperatorEntry(ExpressionType.AndAlso, "AND", 4),
            new OperatorEntry(ExpressionType.Or, "OR", 5),
            new OperatorEntry(ExpressionType.OrElse, "OR", 5),
        };

        LambdaExpression baseExpression;
        StringBuilder sb;
        ArrayList parameters;
        string baseParameterName;
        int baseParameterLength;

        public ArrayList Parameters
        {
            get { return parameters; }
        }

        public ExpressionTreeTransformer(LambdaExpression ex)
        {
            this.baseExpression = ex;
        }
		
        public string Transform()
        {
			// If an expression consists only of a boolean expression like in o=>o.BooleanProperty
			// we alter the expression to an Equal expression like o=>o.BooleanProperty == true.
			// SqlServer syntax needs a binary expression which reads like WHERE BooleanProperty = 1.
			// Note that this solution doesn't cover all cases of unary boolean expressions like o=>o.BooleanProperty && o.Name == "Max".
			if (this.baseExpression.Body.NodeType == ExpressionType.MemberAccess && this.baseExpression.ReturnType == typeof(bool))
			{
				this.baseExpression = Expression.Lambda( Expression.Equal( baseExpression.Body, Expression.Constant(true) ), baseExpression.Parameters );
			}

            baseParameterName = baseExpression.Parameters[0].Name;
            baseParameterLength = baseParameterName.Length + 1;
            sb = new StringBuilder();
            parameters = new ArrayList();
            Transform(baseExpression.Body, false);
            return sb.ToString();
        }

        void TransformBinaryOperator(ExpressionType exprType, bool rightIsNull)
        {
            sb.Append(' ');
			if (rightIsNull)
			{
				if (exprType == ExpressionType.NotEqual)
					sb.Append( "IS NOT" );
				else if (exprType == ExpressionType.Equal)
					sb.Append( "IS" );
				else
				{
					OperatorEntry oe = FindOperator( exprType );
					sb.Append( oe.Replacement );
				}
			}
			else
			{
				OperatorEntry oe = FindOperator( exprType );
				sb.Append( oe.Replacement );
			}
            sb.Append(' ');
        }

        OperatorEntry FindOperator(ExpressionType exprType)
        {
            foreach(OperatorEntry oe in operators)
                if (oe.ExpressionType == exprType)
                    return oe;
            throw new Exception("ExpressionTreeTransformer: Unsupported operator: " + exprType);
        }

        int GetPrecedence(ExpressionType exprType)
        {
            OperatorEntry oe = FindOperator(exprType);
            return oe.Precedence;
        }

        void AddParameter(object value)
        {
            sb.Append('{');
            sb.Append(parameters.Count.ToString());
            sb.Append('}');
            parameters.Add(value);
        }

		BinaryExpression FlipExpression(BinaryExpression binex)
		{
			if (binex.NodeType == ExpressionType.GreaterThan)
				return BinaryExpression.LessThan( binex.Right, binex.Left );
			if (binex.NodeType == ExpressionType.GreaterThanOrEqual)
				return BinaryExpression.LessThanOrEqual( binex.Right, binex.Left );
			if (binex.NodeType == ExpressionType.LessThan)
				return BinaryExpression.GreaterThan( binex.Right, binex.Left );
			if (binex.NodeType == ExpressionType.LessThanOrEqual)
				return BinaryExpression.GreaterThanOrEqual( binex.Right, binex.Left );
			return Expression.MakeBinary( binex.NodeType, binex.Right, binex.Left );
		}

		void FlipArguments(List<Expression>arguments)
		{
			var temp = arguments[0];
			arguments[0] = arguments[1];
			arguments[1] = temp;
		}

		bool IsReversedExpression( List<Expression> arguments )
		{
			return arguments.Count == 2 && arguments[0] is ConstantExpression && !( arguments[1] is ConstantExpression );
		}

        void Transform(Expression ex, bool isRightSide)
        {
			if (ex.ToString() == baseParameterName)
				return;
			  //-------

			if (ex.ToString() == "null")
			{
				sb.Append( "NULL" );
				return;
			  //-------
			}			
			if (isRightSide)
			{
				object o = Expression.Lambda( ex ).Compile().DynamicInvoke();
				AddParameter( o );
				return;
			  //-------
			}

            BinaryExpression binex = ex as BinaryExpression;
            if (binex !=  null)
            {
				Expression left = binex.Left;
				Expression right = binex.Right;
				if (left.NodeType == ExpressionType.Convert)
					left = ((UnaryExpression)left).Operand;
				if (right.NodeType == ExpressionType.Convert)
					right = ((UnaryExpression)right).Operand;
				string leftStr = left.ToString();
				string rightStr = right.ToString();
				if (!leftStr.StartsWith(baseParameterName + '.' ) && rightStr.StartsWith(baseParameterName + '.'))
				{
					Transform( FlipExpression( binex ), isRightSide );
					return;
				  //-------
				}
                int ownPrecedence = GetPrecedence(binex.NodeType);
                int childPrecedence = 0;
                BinaryExpression childBinex = binex.Left as BinaryExpression;
                bool leftbracket = false;
                if (childBinex != null)
                {
                    childPrecedence = Math.Max(childPrecedence, GetPrecedence(childBinex.NodeType));
                    leftbracket = childPrecedence > ownPrecedence;
                }
                childBinex = binex.Right as BinaryExpression;
                bool rightbracket = false;
                if (childBinex != null)
                {
                    childPrecedence = Math.Max(childPrecedence, GetPrecedence(childBinex.NodeType));
                    rightbracket = childPrecedence > ownPrecedence;
                }
                if (leftbracket)
                    sb.Append('(');
                Transform(left, false);
                if (leftbracket)
                    sb.Append(')');
				TransformBinaryOperator( ex.NodeType, rightStr == "null" );
                if (rightbracket)
                    sb.Append('(');
				Transform( right, OperatorEntry.IsComparison( ex.NodeType ) );
                if (rightbracket)
                    sb.Append(')');
				return;
			  //-------
            }

            MethodCallExpression mcex = ex as MethodCallExpression;
            if (mcex !=  null)
            {
				var arguments = new List<Expression>(mcex.Arguments);
                string mname = mcex.Method.Name;
                if (mname == "op_Equality")
                {
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
					}
					Transform( arguments[0], false);
                    sb.Append(" = ");
                    Transform(arguments[1], true);
                }
				else if (mname == "Equals")
				{
					Transform( mcex.Object, false );
					sb.Append( " = " );
					Transform( mcex.Arguments[0], true );
				}
				else if (mname == "Like")
				{
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
					}
					Transform( arguments[0], false);
					sb.Append(" LIKE ");
					Transform(arguments[1], true);
				}
				else if (mname == "GreaterEqual")
				{
					string op = " >= ";
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
						op = " <= ";
					}
					Transform( arguments[0], false );
					sb.Append( op );
					Transform( arguments[1], true );
				}
				else if (mname == "LowerEqual")
				{
					string op = " <= ";
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
						op = " >= ";
					}
					Transform( arguments[0], false );
					sb.Append( op );
					Transform( arguments[1], true );
				}
				else if (mname == "GreaterThan")
				{
					string op = " > ";
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
						op = " < ";
					}
					Transform( arguments[0], false );
					sb.Append( op );
					Transform( arguments[1], true );
				}
				else if (mname == "LowerThan")
				{
					string op = " < ";
					if (IsReversedExpression( arguments ))
					{
						FlipArguments( arguments );
						op = " > ";
					}
					Transform( arguments[0], false );
					sb.Append( op );
					Transform( arguments[1], true );
				}
				else if (mname == "Between")
				{
					Transform(mcex.Arguments[0], false);
					sb.Append(" BETWEEN ");
					Transform(mcex.Arguments[1], true);
					sb.Append(" AND ");
					Transform(mcex.Arguments[2], true);
				}
				else if (mname == "get_Item")
				{
                    string argStr = mcex.Arguments[0].ToString();
					if (argStr == "Any.Index" || "Index.Any" == argStr)
					{
						Transform( mcex.Object, false );
						return;
					  //-------
					}
					Transform( mcex.Object, false );
					sb.Append( '(' );
					Transform( mcex.Arguments[0], false );
					sb.Append( ')' );
				}
                else if (mname == "ElementAt")
                {
                    string argStr = mcex.Arguments[1].ToString();
                    if (argStr == "Any.Index" || "Index.Any" == argStr)
                    {
                        Transform(mcex.Arguments[0], false);
                        return;
                        //-------
                    }
                    Transform(mcex.Arguments[0], false);
                    sb.Append('(');
                    Transform(mcex.Arguments[1], false);
                    sb.Append(')');
                }
				else if (mname == "Any")
				{
					Transform( mcex.Arguments[0], false );
					var exprx = mcex.Arguments[1];
					sb.Append( '.' );
					Transform( ( (LambdaExpression) mcex.Arguments[1] ).Body, false );
				}
				else if (mname == "In")
                {
                    var arg = mcex.Arguments[1];
                    IEnumerable list = (IEnumerable)(Expression.Lambda(arg).Compile().DynamicInvoke());
                    Transform(mcex.Arguments[0], false);
                    sb.Append(" IN (");
                    var en = list.GetEnumerator();
                    en.MoveNext();
                    bool isString = en.Current.GetType() == typeof(string);
                    foreach(object obj in list)
                    {
                        if (isString)
                            sb.Append('\'');
                        sb.Append(obj);
                        if (isString)
                            sb.Append('\'');
                        sb.Append(',');
                    }
                    sb.Length -= 1;
                    sb.Append(')');
                }
                else if (mname == "Oid")
				{
					var sbLength = sb.Length;
					Transform( mcex.Arguments[0], false );
					if (sb.Length > sbLength)
						sb.Append( ".oid" );
					else
						sb.Append( "oid" );
					if (mcex.Arguments.Count > 1)
					{
						// Hier könnte man schon die Arrays / Multikeys auswerten
						sb.Append( '(' );
						Transform(mcex.Arguments[1], false ); // There should only be one argument
						sb.Append( ')' );
					}
				}
				else
                    throw new Exception("Method call not supported: " + mname);
				return;
			  //-------
            }

            if (ex.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberex = (MemberExpression) ex;
				if (memberex.Member.Name == "NDOObjectId")
				{
					if (memberex.Expression.ToString() != this.baseParameterName)
					{
						Transform( memberex.Expression, false );
						if (sb[sb.Length - 1] != '.')
							sb.Append( '.' );
					}
					sb.Append( "oid" );
					return;
				}
				int l = sb.Length;
				Transform( memberex.Expression, false );
				if (l != sb.Length)
					sb.Append( '.' );
				sb.Append( memberex.Member.Name );
				return;
			  //-------
            }

			else if (ex.NodeType == ExpressionType.Constant)
            {
                ConstantExpression constEx = (ConstantExpression) ex;
				if (isRightSide)
					AddParameter( constEx.Value );
				else
					sb.Append( constEx.Value );
				return;
			  //-------
            }

			else if (ex.NodeType == ExpressionType.Convert)
			{
				Transform( ((UnaryExpression)ex).Operand, isRightSide );
				return;
			  //-------
			}

			else if (ex.NodeType == ExpressionType.Not)
			{
				sb.Append( " NOT " );
				Expression inner = ((UnaryExpression)ex).Operand;
				var binary = (inner is BinaryExpression);
				if (binary)
					sb.Append( '(' );
				Transform( inner, isRightSide );
				if (binary)
					sb.Append( ')' );
				return;
				//-------
			}

		}
	}
}