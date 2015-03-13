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
        bool useLikeOperator;

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
            Transform(baseExpression.Body);
            return sb.ToString();
        }


        void TransformBinaryOperator(ExpressionType exprType)
        {
            sb.Append(' ');
            OperatorEntry oe = FindOperator(exprType);
            sb.Append(oe.Replacement);
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

        void Transform(Expression ex)
        {
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
                Transform(binex.Left);
                if (leftbracket)
                    sb.Append(')');
                TransformBinaryOperator(ex.NodeType);
                if (rightbracket)
                    sb.Append('(');
                Transform(binex.Right);
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
                    Transform(mcex.Arguments[0]);
                    if (useLikeOperator)
                        sb.Append(" LIKE ");
                    else
                        sb.Append(" = ");
                    Transform(mcex.Arguments[1]);
                }
				else if (mname == "Like")
				{
					Transform(mcex.Arguments[0]);
					sb.Append(" LIKE ");
					Transform(mcex.Arguments[1]);
				}
                else
                    throw new Exception("Method call not supported: " + mname);
				return;
			  //-------
            }
            if (ex.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberex = (MemberExpression) ex;
				if ((memberex.Expression as ConstantExpression) == null)
				{
					string exprString = memberex.ToString().Substring( baseParameterLength).Replace(".get_Item(Index.Any).", ".").Replace(".get_Item(Any.Index).", ".");
					sb.Append( exprString );
				}
				else
				{
					object o = Expression.Lambda( memberex ).Compile().DynamicInvoke();
					AddParameter( o );
				}
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
				Transform( ((UnaryExpression)ex).Operand );
				return;
			  //-------
			}
        }
    }
}