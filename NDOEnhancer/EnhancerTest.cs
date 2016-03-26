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
using System.Runtime.InteropServices;
using System.Collections;
using NDOEnhancer;
using System.IO;
using System.Xml;

namespace EnhancerTest
{
	[Serializable]
	public class EnhancerTest
	{
	
        bool verboseMode;

        void CopyFile(string source, string dest)
        {
            if (this.verboseMode)
			    Console.WriteLine("Copying: " + source + "->" + dest);

            File.Copy(source, dest, true);
        }

		int DomainLaunch(string arg)
		{
			ProjectDescription pd;
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}
			pd = new ProjectDescription(arg);
			options = pd.ConfigurationOptions;

			if (!options.EnableAddIn)
				return 0;

			this.verboseMode = options.VerboseMode;
			string appDomainDir = Path.GetDirectoryName(pd.BinFile);
			AppDomain cd = AppDomain.CurrentDomain;
			AppDomain ad = AppDomain.CreateDomain("NDOEnhancerDomain", cd.Evidence, appDomainDir, "", false);
			ad.SetData("arg", arg);
			string loadPath = this.GetType().Assembly.Location;
			if (!File.Exists(loadPath))
				throw new Exception("File not found: " + loadPath);
			int result = ad.ExecuteAssembly(loadPath);
			AppDomain.Unload(ad);
			if (options.EnableEnhancer)
			{
                string tempDir = Path.Combine(pd.ObjPath, "ndotemp");
				string enhObjFile = Path.Combine(tempDir, Path.GetFileName(pd.BinFile));
				string enhPdbFile = Path.ChangeExtension(enhObjFile, ".pdb");
				string binPdbFile = Path.ChangeExtension(pd.BinFile, ".pdb");
				string objFile = Path.Combine(pd.ObjPath, Path.GetFileName(pd.BinFile));
                string objPdbFile = Path.Combine(pd.ObjPath, Path.GetFileName(binPdbFile));
                bool objPathDifferent = String.Compare(objFile, pd.BinFile, true) != 0;
                if (File.Exists(enhObjFile))
				{
					CopyFile(enhObjFile, pd.BinFile);

                    if (objPathDifferent)
                        CopyFile(enhObjFile, objFile);
                    File.Delete(enhObjFile);

					if (File.Exists(enhPdbFile))
					{
                        CopyFile(enhPdbFile, binPdbFile);

                        if (objPathDifferent)
                            CopyFile(enhPdbFile, objPdbFile);
                        try
                        {
                            File.Delete(enhPdbFile);
                        }
                        catch (Exception ex)
                        {
                            if (verboseMode)
                                Console.WriteLine("Warning: Ignored Exception: " + ex.ToString());
                        }
					}
				}
			}
			return result;
		}

		public void InternalStart(string arg)
		{
			ProjectDescription pd;
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}
#if DEBUG
            Console.WriteLine("Loading Project Description...");
#endif
			pd = new ProjectDescription(arg);
            options = pd.ConfigurationOptions;

			if (!options.EnableAddIn)
				return;

#if DEBUG
            Console.WriteLine("Loading Project Description ready.");

			this.verboseMode = true;
#else
            this.verboseMode = options.VerboseMode;

            // In Debug Mode the base directory is printed in the Main method
            if (this.verboseMode)
                Console.WriteLine("Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory);
#endif
            Console.WriteLine(EnhDate.String.Replace("NDO", "NDO Enhancer"));

            if (!pd.IsWebProject)
			    pd.References.Add(pd.AssemblyName, new NDOReference(pd.AssemblyName, pd.BinFile, true));

			MessageAdapter messages = new MessageAdapter();

			new NDOEnhancer.Enhancer(pd, messages).doIt();

			if (options.EnableEnhancer)
				Console.WriteLine("Enhancer ready");
		}

		public static int Main(string[] args)
		{
			int result = 0;

            try
            {
				string locationDir = Path.GetDirectoryName(typeof(EnhancerTest).Assembly.Location);
				string appDomainDir = AppDomain.CurrentDomain.BaseDirectory;
				if (appDomainDir.EndsWith("\\"))
					appDomainDir = appDomainDir.Substring(0, appDomainDir.Length - 1);

				if (string.Compare(appDomainDir, locationDir, true) == 0)
				{
					if (args.Length < 1)
						throw new Exception("usage: NDOEnhancer <file_name>\n");
					string arg = args[0];
					arg = Path.GetFullPath(arg);

					Console.WriteLine( "Enhancer executable: " + typeof(EnhancerTest).Assembly.Location );

					result = new EnhancerTest().DomainLaunch(arg);
				}
				else
				{
#if DEBUG
					Console.WriteLine( "Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory );
					Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
#endif
					string newarg = (string) AppDomain.CurrentDomain.GetData("arg");
					new EnhancerTest().InternalStart(newarg);
				}
            }
            catch(Exception ex)
            {
#if DEBUG
                Console.Error.WriteLine("Error: " + ex.ToString());
#else
                Console.Error.WriteLine("Error: " + ex.Message);
#endif
                result = -1;
            }
			return result;
		}

		public EnhancerTest()
		{
		}

	}
}
