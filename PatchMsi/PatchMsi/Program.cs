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
using WindowsInstaller;
using System.Runtime.InteropServices;
namespace PatchMsi
{
	/// <summary>
	/// Zusammenfassung für Class1.
	/// </summary>
	class Program
	{
		[ComImport, Guid("000c1090-0000-0000-c000-000000000046")]
			internal class WindowsInstallerClass{}
			/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			try
			{
				if (args.Length < 1)
					throw new Exception("usage: PatchMsi <msi-file>");

				if (!File.Exists(args[0]))
					throw new Exception("File doesn't exist: " + args[0]);


				WindowsInstaller.Installer installer = (Installer) new WindowsInstallerClass();
				Database db = installer.OpenDatabase(args[0], WindowsInstaller.MsiOpenDatabaseMode.msiOpenDatabaseModeDirect); 
                
				View view = db.OpenView("SELECT Control, Height, Width from Control");
				view.Execute(null);
				Record r;
				while((r = view.Fetch()) != null)
				{
					string name = r.get_StringData(1);
					if (name.StartsWith("AllUsers"))
					{
						Console.WriteLine("Patching: " + name);
						r.set_IntegerData(2, 1);
						r.set_IntegerData(3, 1);
						view.Modify(MsiViewModify.msiViewModifyUpdate, r);
					}
				}
				view.Close();
                
                view = db.OpenView("SELECT Property, Value from Property");
                view.Execute(null);
                while ((r = view.Fetch()) != null)
                {
                    string name = r.get_StringData(1);
                    if (name == "FolderForm_AllUsers")
                    {
                        r.set_StringData(2, "ALL");
                        Console.WriteLine("Patching: " + name);
                        view.Modify(MsiViewModify.msiViewModifyUpdate, r);
                    }
                }
                view.Close();
                db.Commit();
			}
			catch (Exception ex)
			{
				Console.WriteLine("PatchMsi error: " + ex.Message);
				return -1;
			}
			return 0;
		}
	}
}
