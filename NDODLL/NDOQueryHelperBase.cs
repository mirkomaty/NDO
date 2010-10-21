//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
