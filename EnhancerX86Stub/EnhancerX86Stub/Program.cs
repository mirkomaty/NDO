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
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace EnhancerX86Stub
{
	class Program
	{
		/// <summary>
		/// This application launches the NDOEnhancer in x86 mode to enhance x86 assemblies.
		/// </summary>
		/// <param name="args"></param>
		public static int Main( string[] args )
		{
			int result = 0;

			try
			{
				if ( args.Length < 1 )
					throw new ArgumentException( "EnhancerX86Stub.exe needs at least one parameter" );
				string loadPath = typeof( Program ).Assembly.Location;
				if ( !File.Exists( loadPath ) )
					throw new Exception( "File not found: " + loadPath );
				loadPath = Path.Combine( Path.GetDirectoryName( loadPath ), "NDOEnhancer.exe" );
				result = AppDomain.CurrentDomain.ExecuteAssembly( loadPath, new string[] { args[0] } );
			}
			catch ( Exception ex )
			{
                Console.Error.WriteLine("Error: " + ex.Message);
				result = -2;
			}
			return result;
		}

	}
}
