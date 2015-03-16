//
// Copyright (C) 2002-2015 Mirko Matytschak
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


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

        void Transform(Expression ex, bool isRightSide)
        {
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
                Transform(binex.Left, false);
                if (leftbracket)
                    sb.Append(')');
				TransformBinaryOperator( ex.NodeType, binex.Right.ToString() == "null" );
                if (rightbracket)
                    sb.Append('(');
				Transform( binex.Right, OperatorEntry.IsComparison( ex.NodeType ) );
                if (rightbracket)
                    sb.Append(')');
				return;
			  //-------
            }
            MethodCallExpression mcex = ex as MethodCallExpression;
            if (mcex !=  null)
            {
                string mname = mcex.Method.Name;
                if (mname == "op_Equality")
                {
                    Transform(mcex.Arguments[0], false);
                    sb.Append(" = ");
                    Transform(mcex.Arguments[1], true);
                }
				else if (mname == "Like")
				{
					Transform(mcex.Arguments[0], false);
					sb.Append(" LIKE ");
					Transform(mcex.Arguments[1], true);
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
					throw new Exception( "Indexer must habe Any.Index or Index.Any parameter." );
				}
				else if (mname == "Oid")
				{
					string exStr = ex.ToString();
					exStr = exStr.Substring( baseParameterLength, exStr.Length - 5 - baseParameterLength );
					if (exStr.Length > 0)
					{
						sb.Append( exStr );
					}
					sb.Append("oid ");
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
						sb.Append( '.' );
					}
					sb.Append( "oid" );
					return;
				}
				string exprString = memberex.ToString().Substring( baseParameterLength);
				sb.Append( exprString );
				return;
			  //-------
            }
			if (ex.NodeType == ExpressionType.Constant)
            {
                ConstantExpression constEx = (ConstantExpression) ex;
                AddParameter(constEx.Value);
				return;
			  //-------
            }
			if (ex.NodeType == ExpressionType.Convert)
			{
				Transform( ((UnaryExpression)ex).Operand, isRightSide );
				return;
			  //-------
			}
        }
    }
}