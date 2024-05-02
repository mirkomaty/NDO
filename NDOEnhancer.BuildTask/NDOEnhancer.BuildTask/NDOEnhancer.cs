
// Copyright (c) 2002-2024 Mirko Matytschak 
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


using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using NDOEnhancer;

namespace NDO.BuildTask
{
	public class NDOEnhancer : ToolTask
	{
		[Required]
		public string NdoProjectFile {get; set;}

		[Required]
		public string NdoPlatformTarget {get; set;}

		[Required]
		public string TargetFramework { get; set; }

		ProjectDescription projectDescription;
		bool verboseMode = false;

        protected override string ToolName
		{
			get
			{
				Console.WriteLine( "NDO BuildTask: " + NdoPlatformTarget );
				if (NdoPlatformTarget == "x86")
				{
					throw new Exception( $"Platform x86 is not supported. Use NDO <= 4." );
				}
				return "NDOEnhancer.exe";
			}
		}

		void CreateNDOProjFile(string fileName)
		{
			var binFile = Path.ChangeExtension( Path.GetFileName( NdoProjectFile ), ".dll" );
			string xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Enhancer>
  <Options>
    <EnableAddIn>True</EnableAddIn>
    <EnableEnhancer>True</EnableEnhancer>
    <VerboseMode>False</VerboseMode>
    <NewMapping>False</NewMapping>
    <GenerateSQL>False</GenerateSQL>
    <DefaultConnection>
    </DefaultConnection>
    <TargetMappingFileName>NDOMapping.xml</TargetMappingFileName>
    <GenerateChangeEvents>False</GenerateChangeEvents>
    <UseTimeStamps>False</UseTimeStamps>
    <Utf8Encoding>True</Utf8Encoding>
    <SQLScriptLanguage>
    </SQLScriptLanguage>
    <SchemaVersion>
    </SchemaVersion>
    <IncludeTypecodes>False</IncludeTypecodes>
    <DatabaseOwner>
    </DatabaseOwner>
    <GenerateConstraints>False</GenerateConstraints>
    <DropExistingElements>False</DropExistingElements>
  </Options>
  <ProjectDescription>
    <BinPath>bin\Debug\{TargetFramework}\{binFile}</BinPath>
    <ObjPath>obj\Debug\{TargetFramework}</ObjPath>
    <AssemblyName />
    <Debug>True</Debug>
    <KeyFile>
    </KeyFile>
    <References>
    </References>
  </ProjectDescription>
</Enhancer>";			
			using (StreamWriter sw = new StreamWriter( fileName, false, Encoding.UTF8 ))
			{
				sw.WriteLine( xml );
				sw.Close();
			}
		}

        void CopyFile( string source, string dest )
        {            
            base.Log.LogMessageFromText( $"BuildTask: Copying: {source} => {dest}", MessageImportance.High );
            File.Copy( source, dest, true );
        }

        void LoadProjectDescription( string ndoprojFileName )
        {
            this.projectDescription = new ProjectDescription(ndoprojFileName, TargetFramework);
			this.verboseMode = this.projectDescription.ConfigurationOptions.VerboseMode;
        }


        void CopyBinFile()
		{
            string tempDir = Path.Combine(projectDescription.ObjPath, "ndotemp");
            string enhObjFile = Path.Combine(tempDir, Path.GetFileName(projectDescription.BinFile));

			CopyFile( enhObjFile, projectDescription.BinFile );
        }

        protected override int ExecuteTool( string pathToTool, string responseFileCommands, string ndoprojFileName )
		{
            try
			{
				ndoprojFileName = NdoProjectFile.Trim();
				if (!File.Exists( ndoprojFileName ))
					CreateNDOProjFile( ndoprojFileName );

				LoadProjectDescription( ndoprojFileName );
				ConsoleProcess cp = new ConsoleProcess( false );

				if (this.verboseMode)
				{
					base.Log.LogMessageFromText( $"Build Task: Executing: \"{pathToTool}\"", MessageImportance.High );
                    base.Log.LogMessageFromText( $"Build Task: Parameters: \"{ndoprojFileName}\" {TargetFramework}", MessageImportance.High );
                }

				int result = cp.Execute( $"{pathToTool}", $"\"{ndoprojFileName}\" {TargetFramework}" );

				if (cp.Stdout != String.Empty)
					base.Log.LogMessageFromText( cp.Stdout, MessageImportance.High );

				string stderr = cp.Stderr;
				if (stderr != string.Empty)
				{
					Regex regex = new Regex( @"Error:" );
					MatchCollection mc = regex.Matches( stderr );
					int lastmatch = mc.Count - 1;
					for (int i = 0; i < mc.Count; i++)
					{
						int endindex;
						if (i == lastmatch)
							endindex = stderr.Length;
						else
							endindex = mc[i + 1].Index;
						int startindex = mc[i].Index;
						string outString = stderr.Substring( startindex, endindex - startindex );
						if (outString.EndsWith( "\r\n" ))
							outString = outString.Substring( 0, outString.Length - 2 );
						base.Log.LogError( outString );
					}
				}

                if (result == 0)
					CopyBinFile();

				return result;
			}
			catch (Exception ex)
			{
                base.Log.LogError( $"Build Task: {ex.ToString()}", MessageImportance.High );
				return -1;
            }
		}

		protected override MessageImportance StandardOutputLoggingImportance
		{
			get
			{
				return MessageImportance.Normal;
			}
		}

		protected override string GenerateCommandLineCommands()
		{
			return NdoProjectFile;
		}

		protected override string GenerateFullPathToTool()
		{
			return Path.Combine( Path.GetDirectoryName( GetType().Assembly.Location ), ToolName );
		}
	}
}
