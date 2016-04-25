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
	class Class1
	{
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("usage: ManipulateOidColumns mapping-file Guid|Auto");
				return -1;
			}
			Console.WriteLine("ManipulateOidColums: args: " + args[0] + " " + args[1]);
			

			if (!File.Exists(args[0]))
			{
				Console.WriteLine("can't find file " + args[0]);
				return -2;
			}

			try
			{
				NDOMapping mapping = new NDOMapping(args[0]);
				foreach(Class cl in mapping.Classes)
				{
                    if (cl.Oid.OidColumns.Count == 1)
                    {
                        OidColumn oidCol = (OidColumn)cl.Oid.OidColumns[0];

                        if (cl.FullName == "NDOObjectIdTestClasses.HintOwner")
                        {
                            oidCol.AutoIncremented = false;
                            oidCol.NetType = "System.Int32,mscorlib";
                        }
                        else if (cl.FullName == "NDOObjectIdTestClasses.ClassWithHint")
                        {
                            oidCol.AutoIncremented = false;
                            oidCol.NetType = "System.Guid,mscorlib";
                        }
                        else if (cl.FullName == "NDOObjectIdTestClasses.AbstractBaseGuid" || cl.FullName == "NDOObjectIdTestClasses.DerivedGuid")
                        {
                            oidCol.AutoIncremented = false;
                            oidCol.NetType = "System.Guid,mscorlib";
                            oidCol.FieldName = "guid";
                        }
                        else if (cl.FullName == "NDOObjectIdTestClasses.NDOoidAndHandler")
                        {
                            oidCol.AutoIncremented = false;
                            oidCol.NetType = "System.Guid,mscorlib";
                            oidCol.FieldName = "myId";
                        }
                        else if (cl.FullName == "NDOObjectIdTestClasses.ObjectOwner")
                        {
                            oidCol.AutoIncremented = false;
                            oidCol.NetType = "System.Int32,mscorlib";
                        }
                        else
                        {
                            if (args[1] == "Guid")
                            {
                                oidCol.AutoIncremented = false;
                                oidCol.NetType = "System.Guid,mscorlib";
                            }
                            else
                            {
                                oidCol.AutoIncremented = true;
                                oidCol.NetType = null;
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
