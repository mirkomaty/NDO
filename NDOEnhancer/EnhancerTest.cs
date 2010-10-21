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
	
		string arg;
        bool verboseMode;

		/// <summary>
		/// This function will be called by the host application NDOEnhancer.exe 
		/// via Assembly::raw_Invoke_3()
		/// </summary>
		/// <param name="arg"></param>
		public static void DoIt(string extarg)
		{
			try
			{
				new EnhancerTest(extarg).InternalStart();
			}
			catch (Exception ex)
			{
                bool verboseMode = false;
                try
                {
                    ProjectDescription pd = new ProjectDescription(extarg);
                    verboseMode = pd.ConfigurationOptions.VerboseMode;
                }
                catch    // The outer exception might have been caused
                {                                   // by a project description load error.
                }                                   // So this will be the second failure. We just ignore it.
                if (verboseMode)
				    Console.Error.WriteLine("Error: " + ex.ToString());
                else
				    Console.Error.WriteLine("Error: " + ex.Message);
			}
		}

		EnhancerTest(string extarg)
		{
			this.arg = extarg;
		}

        void CopyFile(string source, string dest)
        {
            if (this.verboseMode)
			    Console.WriteLine("Copying: " + source + "->" + dest);

            File.Copy(source, dest, true);
        }

		int DomainLaunch()
		{
			ProjectDescription pd;
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}
			pd = new ProjectDescription(arg);
			options = pd.ConfigurationOptions;
			this.verboseMode = options.VerboseMode;
			string appDomainDir = Path.GetDirectoryName(pd.BinFile);
			AppDomain cd = AppDomain.CurrentDomain;
			AppDomain ad = AppDomain.CreateDomain("new Domain", cd.Evidence, appDomainDir, "", false);
			string loadPath = this.GetType().Assembly.Location; //Path.Combine(cd.BaseDirectory, "Enhancer.exe");
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

		public void InternalStart()
		{
#if DEBUG
			Console.WriteLine("Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory);
#endif
			ProjectDescription pd;
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}
            Console.WriteLine("Loading Project Description...");
			pd = new ProjectDescription(arg);
            options = pd.ConfigurationOptions;

            Console.WriteLine("Loading Project Description ready.");
#if DEBUG   
            this.verboseMode = true;
#else
            this.verboseMode = options.VerboseMode;

            // In Debug Mode the base directory is printed at the beginning of InternalStart
            if (this.verboseMode)
                Console.WriteLine("Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory);
#endif
            if (this.verboseMode)
                Console.WriteLine(EnhDate.String.Replace("NDO", "Enhancer"));

            if (!pd.IsWebProject)
			    pd.References.Add(pd.AssemblyName, new NDOReference(pd.AssemblyName, pd.BinFile, true));

			MessageAdapter messages = new MessageAdapter();

			new NDOEnhancer.Enhancer(pd, messages).doIt();

			if (options.EnableEnhancer)
				Console.WriteLine("Enhancer ready");
		}

		public static int Main(string[] args)
		{
			string tempArgFile;
			int result = 0;

            try
            {
				string locationDir = Path.GetDirectoryName(typeof(EnhancerTest).Assembly.Location);
				string appDomainDir = AppDomain.CurrentDomain.BaseDirectory;
				if (appDomainDir.EndsWith("\\"))
					appDomainDir = appDomainDir.Substring(0, appDomainDir.Length - 1);

				tempArgFile = Path.Combine(Path.GetTempPath(), "ndoarg.txt");
				
				if (string.Compare(appDomainDir, locationDir, true) == 0)
				{
					if (args.Length < 1)
						throw new Exception("usage: NDOEnhancer <file_name>\n");
					string arg = args[0];
					arg = Path.GetFullPath(arg);

					StreamWriter sw = new StreamWriter(tempArgFile);
					sw.Write(arg);
					sw.Close();
					result = new EnhancerTest(arg).DomainLaunch();
				}
				else
				{
					StreamReader sr = new StreamReader(tempArgFile);
					string newarg = sr.ReadToEnd();
					sr.Close();
					File.Delete(tempArgFile);
					new EnhancerTest(newarg).InternalStart();
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
