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
	/// Summary description for MessageAdapter.
	/// </summary>
	internal class MessageAdapter
	{
		int indent = 0;

		public MessageAdapter()
		{
		}


		public void Indent() { indent += 2; }
		public void Unindent()
		{
			if (indent >= 2)
				indent -= 2;
		}


		public void
			Write( String text )
		{
			string indentStr = string.Empty;
			for (int i = 0; i < indent; i++)
				indentStr += ' ';
			
			Console.Write ( indentStr + text );
		}

		
		public void
			WriteInsertedLine( String text )
		{
			Indent();
			WriteLine(text);
			Unindent();
		}
		

		public void
			WriteLine( String text )
		{
			string indentStr = string.Empty;
			for (int i = 0; i < indent; i++)
				indentStr += ' ';
			
			Console.WriteLine( indentStr + text );
		}

		public void ShowError(string text)
		{
			Console.Error.WriteLine("Error: " + text);
		}


	}
}
