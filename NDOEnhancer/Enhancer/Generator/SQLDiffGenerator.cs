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
using System.IO;
using System.Data;
using System.Collections;
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

			if (!NDOProviderFactory.Instance.Generators.ContainsKey(scriptLanguage))
			{
				// The error message should have been written by the Sql generator
				messages.WriteLine("NDOEnhancer: No Sql code generator for script language '" + scriptLanguage + "' found");
				return;
			}
			concreteGenerator = (ISqlGenerator) NDOProviderFactory.Instance.Generators[scriptLanguage];

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
