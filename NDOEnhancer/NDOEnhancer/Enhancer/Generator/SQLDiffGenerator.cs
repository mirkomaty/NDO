//
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
using System.IO;
using System.Data;
using NDO;
using NDO.Mapping;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für SQLDiffGenerator.
	/// </summary>
	internal class SQLDiffGenerator
	{
		public void Generate(string scriptLanguage, bool utf8Encoding, DataSet dsSchema, DataSet dsBak, string filename, NDOMapping mappings, MessageAdapter messages)
		{
			string diffFile = filename.Replace(".ndo.sql", ".ndodiff." + mappings.SchemaVersion + ".sqltemp");
            bool isNewDiffFile = !File.Exists(diffFile.Replace(".sqltemp", ".sql"));
			StreamWriter sw = new StreamWriter(diffFile, true, utf8Encoding ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default);
            long initialLength = 0;
            if (isNewDiffFile)
            {
                sw.WriteLine(@"-- NDO accumulates all schema changes in this diff file.
-- Note: If you change the mapping schema version, a new diff file will be created.
-- You can change the mapping schema version in the NDO configuration dialog.
-- Don't use the Mapping Tool to change the schema information, because it will be
-- overwritten by the value set in the configuration. For automatic builds set
-- the schema version value in the .ndoproj file.
");
                sw.Flush();
                initialLength = sw.BaseStream.Length;
            }
			ISqlGenerator concreteGenerator = null;
			if (scriptLanguage == string.Empty)
			{
				messages.WriteLine("NDOEnhancer: No script language selected");
				return;
			}

			if (!EnhancerProviderFactory.Instance.Generators.ContainsKey(scriptLanguage))
			{
				// The error message should have been written by the Sql generator
				messages.WriteLine("NDOEnhancer: No Sql code generator for script language '" + scriptLanguage + "' found");
				return;
			}
			concreteGenerator = (ISqlGenerator) EnhancerProviderFactory.Instance.Generators[scriptLanguage];

			new GenericDiffGenerator(concreteGenerator, messages, mappings).Generate(dsSchema, dsBak, sw);
			sw.Close();
            bool delete = false;
            if (isNewDiffFile)
            {
                FileStream fs = new FileStream(diffFile, FileMode.Open, FileAccess.Read);
                delete = fs.Length < initialLength + 2L;
                fs.Close();
            }
            if (delete)
                File.Delete(diffFile);
            else
                File.Copy(diffFile, diffFile.Replace(".sqltemp", ".sql"), true);
			
		}
	}
}
