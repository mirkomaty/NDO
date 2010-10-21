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
