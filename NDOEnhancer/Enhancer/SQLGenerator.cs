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
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für SQLGenerator.
	/// </summary>
	internal class SQLGenerator
	{
		public void Generate(string scriptLanguage, DataSet dsSchema, string filename, NDOMapping mappings, MessageAdapter messages)
		{
			StreamWriter sr = new StreamWriter(filename, false, System.Text.Encoding.Default);
			ISqlGenerator concreteGenerator = null;
			if (scriptLanguage == string.Empty)
			{
				messages.WriteLine("NDOEnhancer: No script language selected");
				return;
			}

			concreteGenerator = GeneratorFactory.Instance[scriptLanguage];
			if (concreteGenerator == null)
			{
				messages.WriteLine("NDOEnhancer: No provider for script language '" + scriptLanguage + "' found");
				return;
			}


			new GenericSqlGenerator(concreteGenerator, messages).Generate(dsSchema, sr, mappings);
			sr.Close();
		}

	}
}
