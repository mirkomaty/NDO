﻿//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using NDO;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für SQLGenerator.
	/// </summary>
	internal class SQLGenerator
	{
		public void Generate(string scriptLanguage, bool utf8Encoding, DataSet dsSchema, DataSet dsOld, string filename, NDOMapping mappings, MessageAdapter messages, TypeManager typeManager, bool generateConstraints)
		{
			StreamWriter sw = new StreamWriter(filename, false, utf8Encoding ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default);
			ISqlGenerator concreteGenerator = null;
			if (scriptLanguage == string.Empty)
			{
				messages.WriteLine("NDOEnhancer: No script language selected");
				return;
			}

			concreteGenerator = (ISqlGenerator) NDOProviderFactory.Instance.Generators[scriptLanguage];
			if (concreteGenerator == null)
			{
				messages.WriteLine("NDOEnhancer: No Sql code generator for script language '" + scriptLanguage + "' found");
				return;
			}


			new GenericSqlGenerator(concreteGenerator, messages, mappings).Generate(dsSchema, dsOld, sw, typeManager, generateConstraints);
			sw.Close();
		}

	}
}
