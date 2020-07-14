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
using System.Linq;

namespace NDO.Linq
{
	/// <summary>
	/// This class transforms Linq queries to NDOql
	/// </summary>
	public class ExpressionTreeTransformer
	{
		class OperatorEntry
		{
			public OperatorEntry( ExpressionType exprType, string replacement, int prec )
			{
				this.ExpressionType = exprType;
				this.Precedence = prec;
				this.Replacement = replacement;
			}
			public ExpressionType ExpressionType;
			public string Replacement;
			public int Precedence;

			public static bool IsComparison( ExpressionType expType )
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
		Expression baseLeftSide;
		StringBuilder sb;
		List<object> parameters;
		string baseParameterName;
		int baseParameterLength;
		Stack<Expression> expressionStack;

		/// <summary>
		/// Returns the generated parameters
		/// </summary>
		public IEnumerable<object> Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// Constructs an ExpressionTreeTransformer objct
		/// </summary>
		/// <param name="ex"></param>
		public ExpressionTreeTransformer( LambdaExpression ex )
		{
			this.baseExpression = ex;
		}

		/// <summary>
		/// Transforms the lambda expression passed to the constructor into an NDOql string.
		/// </summary>
		/// <returns></returns>
		public string Transform()
		{
			baseParameterName = baseExpression.Parameters[0].Name;
			baseLeftSide = baseExpression.Parameters[0];
			baseParameterLength = baseParameterName.Length + 1;
			this.expressionStack = new Stack<Expression>();
			sb = new StringBuilder();
			parameters = new List<object>();
			Transform( baseExpression.Body );
			return sb.ToString();
		}

		bool IsNull( Expression ex )
		{
			if (ex is ConstantExpression ce)
			{
				if (ce.Value == null)
					return true;
			}

			if (ex.NodeType == ExpressionType.MemberAccess)
			{
				var me = ex as MemberExpression;
				if (me != null)
				{
					if (me.Type == typeof( DateTime ))
					{
						try
						{
							var d = Expression.Lambda( ex ).Compile();
							var dt = (DateTime)d.DynamicInvoke();
							if (dt == DateTime.MinValue)
								return true;
						}
						catch { }
					}
					else if (me.Type == typeof( Guid ))
					{
						try
						{
							var d = Expression.Lambda( ex ).Compile();
							var g = (Guid)d.DynamicInvoke();
							if (g == Guid.Empty)
								return true;
						}
						catch { }
					}
				}
			}

			return false;
		}

		void TransformBinaryOperator( ExpressionType exprType, ref Expression right )
		{
			sb.Append( ' ' );
			if (IsNull( right ))
			{
				if (exprType == ExpressionType.NotEqual)
				{
					sb.Append( "IS NOT" );
					right = Expression.Constant(null);
				}
				else if (exprType == ExpressionType.Equal)
				{
					sb.Append( "IS" );
					right = Expression.Constant( null );
				}
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
			sb.Append( ' ' );
		}

		OperatorEntry FindOperator( ExpressionType exprType )
		{
			foreach (OperatorEntry oe in operators)
				if (oe.ExpressionType == exprType)
					return oe;
			throw new Exception( "ExpressionTreeTransformer: Unsupported operator: " + exprType );
		}

		int GetPrecedence( ExpressionType exprType )
		{
			OperatorEntry oe = FindOperator(exprType);
			return oe.Precedence;
		}

		void AddParameter( object value )
		{
			var ix = parameters.FindIndex(p => p.Equals(value));
			if (ix > -1)
			{
				sb.Append( '{' );
				sb.Append( ix.ToString() );
				sb.Append( '}' );
			}
			else
			{
				sb.Append( '{' );
				sb.Append( parameters.Count.ToString() );
				sb.Append( '}' );
				parameters.Add( value );
			}
		}

		BinaryExpression FlipExpression( BinaryExpression binex )
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

		void FlipArguments( List<Expression> arguments )
		{
			var temp = arguments[0];
			arguments[0] = arguments[1];
			arguments[1] = temp;
		}

		bool IsReversedExpression( List<Expression> arguments )
		{
			return arguments.Count == 2 && arguments[0] is ConstantExpression && !( arguments[1] is ConstantExpression );
		}

		void TransformEquality(Expression ex1, Expression ex2)
		{
			Transform( ex1 );
			TransformBinaryOperator( ExpressionType.Equal, ref ex2 );
			Transform( ex2 );
		}

		bool IsPropertyOfBaseExpression( Expression ex )
		{
			if (ex is MemberExpression pe)
			{
				return pe.Expression == baseLeftSide;
			}

			return false;
		}

		void Transform( Expression ex )
		{
			expressionStack.Push( ex );  // This helps determining parent expressions
			try
			{
				if (ex == this.baseLeftSide)
					return;
				  //-------

				string exStr = ex.ToString();
				if (exStr == "null")
				{
					sb.Append( "NULL" );
					return;
				  //-------
				}

				if (ex.NodeType == ExpressionType.MemberAccess )
				{
					// We try to compile the expression.
					// If it can be compiled, we have a value,
					// which should be added as a parameter.
					// We are safe to examine MemberAccess nodes only,
					// because local variables will be wrapped in
					// display classes and accessed as members of 
					// these classes. All other cases are anyway 
					// members of arbitrary types (like String.Empty).
					try
					{
						var d = Expression.Lambda( ex ).Compile();
						AddParameter( d.DynamicInvoke() );
						return;
					  //-------
					}
					catch
					{  // Can't be compiled, so pass on.
					}
				}

				BinaryExpression binex = ex as BinaryExpression;
				if (binex != null)
				{
					Expression left = binex.Left;
					Expression right = binex.Right;
					if (left.NodeType == ExpressionType.Convert)
						left = ( (UnaryExpression) left ).Operand;
					if (right.NodeType == ExpressionType.Convert)
						right = ( (UnaryExpression) right ).Operand;

					if (!IsPropertyOfBaseExpression( binex.Left ) && IsPropertyOfBaseExpression( binex.Right ))
					{
						Transform( FlipExpression( binex ) );
						return;
						//-------
					}

					int ownPrecedence = GetPrecedence(binex.NodeType);
					int childPrecedence = 0;
					BinaryExpression childBinex = binex.Left as BinaryExpression;
					bool leftbracket = false;
					if (childBinex != null)
					{
						childPrecedence = Math.Max( childPrecedence, GetPrecedence( childBinex.NodeType ) );
						leftbracket = childPrecedence > ownPrecedence;
					}
					childBinex = binex.Right as BinaryExpression;
					bool rightbracket = false;
					if (childBinex != null)
					{
						childPrecedence = Math.Max( childPrecedence, GetPrecedence( childBinex.NodeType ) );
						rightbracket = childPrecedence > ownPrecedence;
					}
					if (leftbracket)
						sb.Append( '(' );
					Transform( left );
					if (leftbracket)
						sb.Append( ')' );
					TransformBinaryOperator( ex.NodeType, ref right );
					if (rightbracket)
						sb.Append( '(' );
					Transform( right );
					if (rightbracket)
						sb.Append( ')' );
					return;
					//-------
				}

				MethodCallExpression mcex = ex as MethodCallExpression;
				if (mcex != null)
				{
					var arguments = new List<Expression>(mcex.Arguments);
					string mname = mcex.Method.Name;
					if (mname == "op_Equality")
					{
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
						}
						TransformEquality( arguments[0], arguments[1] );
					}
					else if (mname == "Equals")
					{
						TransformEquality( mcex.Object, arguments[0] );
					}
					else if (mname == "Like")
					{
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
						}
						Transform( arguments[0] );
						sb.Append( " LIKE " );
						Transform( arguments[1] );
					}
					else if (mname == "GreaterEqual")
					{
						string op = " >= ";
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
							op = " <= ";
						}
						Transform( arguments[0] );
						sb.Append( op );
						Transform( arguments[1] );
					}
					else if (mname == "LowerEqual")
					{
						string op = " <= ";
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
							op = " >= ";
						}
						Transform( arguments[0] );
						sb.Append( op );
						Transform( arguments[1] );
					}
					else if (mname == "GreaterThan")
					{
						string op = " > ";
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
							op = " < ";
						}
						Transform( arguments[0] );
						sb.Append( op );
						Transform( arguments[1] );
					}
					else if (mname == "LowerThan")
					{
						string op = " < ";
						if (IsReversedExpression( arguments ))
						{
							FlipArguments( arguments );
							op = " > ";
						}
						Transform( arguments[0] );
						sb.Append( op );
						Transform( arguments[1] );
					}
					else if (mname == "Between")
					{
						Transform( mcex.Arguments[0] );
						sb.Append( " BETWEEN " );
						Transform( mcex.Arguments[1] );
						sb.Append( " AND " );
						Transform( mcex.Arguments[2] );
					}
					else if (mname == "get_Item")
					{
						string argStr = mcex.Arguments[0].ToString();
						if (argStr == "Any.Index")
						{
							Transform( mcex.Object );
							return;
							//-------
						}
						Transform( mcex.Object );
						if (mcex.Object.Type.FullName == "NDO.ObjectId")
						{
							TransformOidIndex( mcex.Arguments[0] );
							return;
							//-------
						}
						sb.Append( '(' );
						Transform( mcex.Arguments[0] );
						sb.Append( ')' );
					}
					else if (mname == "ElementAt")
					{
						string argStr = mcex.Arguments[1].ToString();
						if (argStr == "Any.Index" || "Index.Any" == argStr)
						{
							Transform( mcex.Arguments[0] );
							return;
							//-------
						}
						Transform( mcex.Arguments[0] );
						sb.Append( '(' );
						Transform( mcex.Arguments[1] );
						sb.Append( ')' );
					}
					else if (mname == "Any")
					{
						Transform( mcex.Arguments[0] );
						var exprx = mcex.Arguments[1];
						if (sb[sb.Length - 1] != '.')
							sb.Append( '.' );
						Transform( ( (LambdaExpression) mcex.Arguments[1] ).Body );
					}
					else if (mname == "In")
					{
						var arg = mcex.Arguments[1];
						IEnumerable list = (IEnumerable)(Expression.Lambda(arg).Compile().DynamicInvoke());
						Transform( mcex.Arguments[0] );
						sb.Append( " IN (" );
						var en = list.GetEnumerator();
						en.MoveNext();
						bool doQuote = en.Current is string || en.Current is Guid || en.Current is DateTime;
						foreach (object obj in list)
						{
							if (doQuote)
								sb.Append( '\'' );
							sb.Append( obj );
							if (doQuote)
								sb.Append( '\'' );
							sb.Append( ',' );
						}
						sb.Length -= 1;
						sb.Append( ')' );
					}
					else if (mname == "Oid")
					{
						var sbLength = sb.Length;
						Transform( mcex.Arguments[0] );
						if (sb.Length > sbLength)
							sb.Append( ".oid" );
						else
							sb.Append( "oid" );
						if (mcex.Arguments.Count > 1)
						{
							TransformOidIndex( mcex.Arguments[1] );
						}
					}
					else
						throw new Exception( "Method call not supported: " + mname );
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
							Transform( memberex.Expression );
							if (sb[sb.Length - 1] != '.')
								sb.Append( '.' );
						}
						sb.Append( "oid" );
						return;
					}
					if (ex.Type == typeof( bool ))
					{
						var top = expressionStack.Pop();
						bool transformIt = expressionStack.Count == 0;  // The boolean expression is the top
						if (expressionStack.Count > 0)
						{
							var parent = expressionStack.Peek();
							if (parent.NodeType != ExpressionType.Equal && parent.NodeType != ExpressionType.NotEqual)
							{
								// A Boolean Expression, which is not part of an Equals or NotEquals expression,
								// must be unary. Since Sql Server doesn't support unary boolean expressions, we add an Equals Expression which compares with 1.
								// Fortunately this works with other databases, too.
								transformIt = true;
							}
						}
						if (transformIt)
						{
							sb.Append( memberex.Member.Name );
							sb.Append( " = 1" );
						}
						expressionStack.Push( top );
						if (transformIt)
							return;
						//-------
					}
					if (memberex.Expression != baseLeftSide)
					{
						// This happens while navigating through relations.
						Transform( memberex.Expression );
						if (sb[sb.Length - 1] != '.')
							sb.Append( '.' );
					}
					sb.Append( memberex.Member.Name );
					return;
					//-------
				}

				else if (ex.NodeType == ExpressionType.Constant)
				{
					ConstantExpression constEx = (ConstantExpression) ex;
					AddParameter( constEx.Value );
					return;
					//-------
				}

				else if (ex.NodeType == ExpressionType.Convert)
				{
					Transform( ( (UnaryExpression) ex ).Operand );
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
					Transform( inner );
					if (binary)
						sb.Append( ')' );
					return;
					//-------
				}
			}
			finally
			{				
				expressionStack.Pop();
			}
		}

		private void TransformEquality( List<Expression> arguments )
		{
			Transform( arguments[0] );
			sb.Append( " = " );
			Transform( arguments[1] );
		}

		private void TransformOidIndex( Expression ex )
		{
			var ce = ex as ConstantExpression;
			if (ce == null)
				throw new Exception( "Oid index expression must be a ConstantExpression" );
			if (!( ce.Value is System.Int32 ))
				throw new Exception( "Oid index expression must be an Int32" );

			if ((int) ce.Value != 0)
			{
				sb.Append( '(' );
				sb.Append( ce.Value );
				sb.Append( ')' );
			}
		}
	}
}