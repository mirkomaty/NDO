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

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for TypeName.
	/// </summary>
	internal class TypeName
	{
		string ilname;

		public TypeName(string ilname)
		{
			this.ilname = ilname;
		}

		public TypeName(string name, bool isLibType)
		{
			if (isLibType)
				ilname = ILFromLibType(name);
			else
				ilname = name;
		}


		public bool IsPrimitive
		{
			get { return isPrimitive(); }
		}

		public string LibType
		{
			get { return (LibTypeFromIL(ilname)); }
		}

		public string ILType 
		{
			get { return ilname; }
		}

		private string
		LibTypeFromIL( string typeName )
		{
			typeName = typeName.Trim();

			if ( typeName == "bool" )
				return "System.Boolean";
			else if ( typeName == "byte" )
				return "System.Byte";
			else if ( typeName == "sbyte" )
				return "System.SByte";
			else if ( typeName == "char" )
				return "System.Char";
			else if ( typeName == "unsigned char" )
				return "System.UChar";
			else if ( typeName == "short" || typeName == "int16" )
				return "System.Int16";
			else if ( typeName == "unsigned int16" )
				return "System.UInt16";
			else if ( typeName == "int" || typeName == "int32" )
				return "System.Int32";
			else if ( typeName == "unsigned int32" )
				return "System.UInt32";
			else if ( typeName == "long" || typeName == "int64" )
				return "System.Int64";
			else if ( typeName == "unsigned int64" )
				return "System.UInt64";
			else if ( typeName == "float" || typeName == "single" )
				return "System.Single";
			else if ( typeName == "double" )
				return "System.Double";
			else if ( typeName == "string" )
				return "System.String";
			else return typeName;
		}


		private bool
			isPrimitive()
		{
			ilname = ilname.Trim();

			if ( ilname == "bool" )
				return true;
			if ( ilname == "byte" )
				return true;
			if ( ilname == "sbyte" )
				return true;
			if ( ilname == "char" )
				return true;
			if ( ilname == "unsigned char" )
				return true;
			if ( ilname == "short" || ilname == "int16" )
				return true;
			if ( ilname == "unsigned int16" )
				return true;
			if ( ilname == "int" || ilname == "int32" )
				return true;
			if ( ilname == "unsigned int32" )
				return true;
			if ( ilname == "long" || ilname == "int64" )
				return true;
			if ( ilname == "unsigned int64" )
				return true;
			if ( ilname == "float" || ilname == "single" )
				return true;
			if ( ilname == "double" )
				return true;
			return false;
		}

		public bool IsString
		{
			get 
			{
				return ( ilname == "string" );
			}
		}
		
		private string
		ILFromLibType( string tName )
		{
			string typeName = tName.Substring(tName.IndexOf("]") + 1).Trim();

			if ( typeName == "System.Boolean" )
				return "bool";
			else if ( typeName == "System.Byte")
				return "byte";
			else if ( typeName == "System.SByte" )
				return "sbyte";
			else if ( typeName == "System.Char" )
				return "char";
			else if ( typeName == "System.UChar" )
				return "unsigned char";
			else if ( typeName == "System.Int16" )
				return "int16";
			else if ( typeName == "System.UInt16" )
				return "unsigned int16";
			else if ( typeName == "System.Int32" )
				return "int32";
			else if ( typeName == "System.UInt32" )
				return "unsigned int32";
			else if ( typeName == "System.Int64" )
				return "int64";
			else if ( typeName == "System.UInt64" )
				return "unsigned int64";
			else if ( typeName == "System.Single" )
				return "single";  // evtl. float
			else if ( typeName == "System.Double" )
				return "double";
			else if ( typeName == "System.String" )
				return "string";
			else return typeName;
		}

	}
}
