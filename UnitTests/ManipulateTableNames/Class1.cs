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
using System.IO;
using NDO.Mapping;

namespace ManipulateTableNames
{
	/// <summary>
	/// Zusammenfassung fï¿½r Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt fï¿½r die Anwendung.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("usage: ManipulateTableNames mapping-file [TablePrefix]");
				return -1;
			}
			Console.Write("ManipulateTableNames: args: " + args[0]);
			
			if (args.Length > 1)
				Console.WriteLine(" " + args[1]);
			else
				Console.WriteLine();

			if (!File.Exists(args[0]))
			{
				Console.WriteLine("can't find file " + args[0]);
				return -2;
			}

			string prefix = null;
			bool hasPrefix = args.Length > 1;
			if (hasPrefix)
				prefix= args[1];
			try
			{
				NDOMapping mapping = new NDOMapping(args[0]);
				if (hasPrefix)
				{
					foreach(Class cl in mapping.Classes)
					{
						if (!cl.TableName.StartsWith(prefix))
							cl.TableName = prefix + "." + cl.TableName;
						foreach(Relation r in cl.Relations)
						{
							if (r.MappingTable != null)
							{
								if (!r.MappingTable.TableName.StartsWith(prefix))
									r.MappingTable.TableName = prefix + "." + r.MappingTable.TableName;
							}
						}
					}
				}
				else
				{
					foreach(Class cl in mapping.Classes)
					{
						if (cl.TableName.IndexOf('.') > -1)
							cl.TableName = cl.TableName.Substring(cl.TableName.IndexOf('.') + 1);
						foreach(Relation r in cl.Relations)
						{
							if (r.MappingTable != null)
							{
								if (r.MappingTable.TableName.IndexOf('.') > -1)
									r.MappingTable.TableName = r.MappingTable.TableName.Substring(r.MappingTable.TableName.IndexOf('.') + 1);
							}
						}
					}
				}
				mapping.Save();
			}
			catch(Exception ex)
			{
				Console.WriteLine("ManipulateTableNames: " + ex.Message);
				return -3;
			}
			return 0;
		}
	}
}
