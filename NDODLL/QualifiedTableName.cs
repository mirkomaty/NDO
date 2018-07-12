﻿//
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
using System.Text;
using NDOInterfaces;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// Summary description for QualifiedTableName.
	/// </summary>
	internal class QualifiedTableName
	{
		public static string Get(Class cls)
		{
			return Get( cls.TableName, cls.Provider );
		}

		public static string Get(MappingTable mappingTable)
		{
			return Get( mappingTable.TableName, mappingTable.Parent.Parent.Provider );
		}

		public static string Get(string name, IProvider p)
		{
			if (name.IndexOf('.') == -1)
				return p.GetQuotedName(name);
			string[] strarr = name.Split('.');
			StringBuilder sb = new StringBuilder();

			foreach(string n in strarr)
			{
				sb.Append(p.GetQuotedName(n));
				sb.Append('.');
			}
			sb.Length--;
			return sb.ToString();
		}
	}
}
