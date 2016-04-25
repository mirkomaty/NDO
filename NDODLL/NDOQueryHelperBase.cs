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

namespace NDO
{
	/// <summary>
	/// Base Class for the QueryHelper Classes which help to write
	/// Expressions in Query Strings which are checked at compile time
	/// The Enhancer will inherit QueryHelper classes from this class.
	/// If the name of a user class is MyClass, the query helper is 
	/// available with the name MyClass.QueryHelper.
	/// </summary>
	public abstract class NDOQueryHelperBase
	{
		/// <summary>
		/// This constructor is used in nested scenarios
		/// </summary>
		/// <param name="n"></param>
		public NDOQueryHelperBase (string n) 
		{ 
			name = n; 
		}

		/// <summary>
		/// Default constructor 
		/// </summary>
		public NDOQueryHelperBase()
		{
		}

		const string dot = ".";
		string name;

		/// <summary>
		/// This dummy function is used to verify names of query strings.
		/// Use qh.Verify(qh.varname1.varname2.varname3) to verify a query string
		/// like "varname1.varname2.varname3 = someExpression"
		/// </summary>
		/// <param name="s">The QueryHelper fields are strings representing the name of fields</param>
		public void Verify(string s){}

		/// <summary>
		/// Override of ToString
		/// </summary>
		/// <returns>The name of the field, the current instance relates to</returns>
		public override string ToString()
		{
			return name;
		}

		/// <summary>
		/// Used to construct suery strings with query helpers
		/// </summary>
		/// <param name="a">Name of a field - typically a QueryHelper field</param>
		/// <param name="b">Name of a field - typically a QueryHelper field</param>
		/// <returns></returns>
		public string Concat(string a, string b)
		{
			return a + dot + b;
		}

		/// <summary>
		/// Used to construct suery strings with query helpers
		/// </summary>
		/// <param name="a">Name of a field - typically a QueryHelper field</param>
		/// <param name="b">Name of a field - typically a QueryHelper field</param>
		/// <param name="c">Name of a field - typically a QueryHelper field</param>
		/// <returns></returns>
		public string Concat(string a, string b, string c)
		{
			return a + dot + b + dot + c;
		}

		/// <summary>
		/// Used to construct suery strings with query helpers
		/// </summary>
		/// <param name="arr">Names of fields - typically QueryHelper objects or fields of QueryHelpers</param>
		/// <returns></returns>
		public string Concat(object[] arr)
		{
			string result = "";
			for (int i = 0; i < arr.Length; i++)
			{
				result += arr[i].ToString();

				if (i < arr.Length - 1)
					result += dot;
			}
			return result;
		}
	}
}
