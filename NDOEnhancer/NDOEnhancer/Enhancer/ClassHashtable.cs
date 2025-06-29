﻿//
// Copyright (c) 2002-2022 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, frgee of charge, to any person obtaining a copy of this software and associated 
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

namespace NDOEnhancer
{
	/// <summary>
	/// Implements a specific dictionary, which alters the key before storing or retrieving
	/// data.
	/// </summary>
	internal class ClassDictionary<T> : Dictionary<string,T>
	{
		static string ComputeKey(string name)
		{
			return name.Substring( name.IndexOf( ']' ) + 1 ).Replace( "+", "/" );
		}

		public new T this[string key]
		{
			get
			{
				string clName = ComputeKey( key );
				if (!base.ContainsKey( clName ))
					return default(T);
				return base[clName];
			}
			set
			{
				string clName = ComputeKey( key );
				base[clName] = value;
			}			
		}

		public new bool ContainsKey(string key)
		{
			string clName = ComputeKey( key );
			return base.ContainsKey(clName);
		}
		
		public new void Add(string key, T value)
		{
			string clName = ComputeKey( key );
			base.Add (clName, value);
		}
	}
}
