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
            new OperatorEntry(ExpressionType.GE, ">=", 2),
            new OperatorEntry(ExpressionType.GT, ">", 2),
            new OperatorEntry(ExpressionType.LE, "<=", 2),
            new OperatorEntry(ExpressionType.LT, "<", 2),
            new OperatorEntry(ExpressionType.NE, "<>", 2),
            new OperatorEntry(ExpressionType.EQ, "=", 3),
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
            }
            MethodCallExpression mcex = ex as MethodCallExpression;
            if (mcex !=  null)
            {
                string mname = mcex.Method.Name;
                if (mname == "op_Equality")
                {
                    Transform(mcex.Parameters[0]);
                    if (useLikeOperator)
                        sb.Append(" LIKE ");
                    else
                        sb.Append(" = ");
                    Transform(mcex.Parameters[1]);
                }
                else
                    throw new Exception("Method call not supported: " + mname);
            }
            if (ex.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberex = (MemberExpression) ex;
                sb.Append(memberex.ToString().Substring(baseParameterLength));
            }
            else if (ex.NodeType == ExpressionType.Constant)
            {
                ConstantExpression constEx = (ConstantExpression) ex;
                AddParameter(constEx.Value);
            }
        }

        public bool UseLikeOperator
        {
            get { return useLikeOperator; }
            set { useLikeOperator = value; }
        }

    }
}