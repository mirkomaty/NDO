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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDO.Logging;

namespace NdoUnitTests
{
	class TestLogAdapter : ILogAdapter
	{
		StringBuilder text = new StringBuilder();

		public void Clear()
		{
			text = new StringBuilder();
		}

		public void Debug( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Error( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Info( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Warn( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public string Text
		{
			get
			{
				return this.text.ToString();
			}
		}

		public override string ToString()
		{
			return Text;
		}
	}
}