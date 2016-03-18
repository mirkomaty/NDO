using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

namespace NDO.BuildTask
{
	public class NDOEnhancer : ToolTask
	{
		[Required]
		public string NdoProjectFile {get; set;}

		[Required]
		public string NdoPlatformTarget {get; set;}		

		protected override string ToolName
		{
			get
			{
				Console.WriteLine( "NDO BuildTask: " + NdoPlatformTarget );
				if (NdoPlatformTarget == "x86")
				{
					return "EnhancerX86Stub.exe";
				}
				return "NDOEnhancer.exe";
			}
		}

		void CreateNDOProjFile(string fileName)
		{
			string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
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
    <UseMsBuild>True</UseMsBuild>
    <DropExistingElements>False</DropExistingElements>
  </Options>
  <ProjectDescription>
    <BinPath>bin\Debug\ConsoleApplication3.exe</BinPath>
    <ObjPath>obj\Debug\</ObjPath>
    <AssemblyName />
    <Debug>True</Debug>
    <IsWebProject>False</IsWebProject>
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

		protected override int ExecuteTool( string pathToTool, string responseFileCommands, string ndoprojFileName )
		{
			ndoprojFileName = ndoprojFileName.Trim();  // Important. The parameter comes with a leading blank
			if (!File.Exists( ndoprojFileName ))
				CreateNDOProjFile( ndoprojFileName );

			ConsoleProcess cp = new ConsoleProcess( false );

			int result = cp.Execute( "\"" + pathToTool + "\"",
				"\"" + ndoprojFileName + "\"" );

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
					//						messages.WriteLine("[" + i + "]:" + startindex + ',' + endindex);
					// The substring always ends with a '\n', which should be removed.
					string outString = stderr.Substring( startindex, endindex - startindex );
					if (outString.EndsWith( "\r\n" ))
						outString = outString.Substring( 0, outString.Length - 2 );
					//						messages.ShowError("|" + out String + "|");
					base.Log.LogError( outString );
				}
			}

			return result;
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
			//if ( null == this.ndoPath )
			//{
			//	RegistryKey registryKey = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\NDO" );
			//	if ( registryKey == null )
			//		registryKey = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\Wow6432Node\NDO" );
			//	if ( registryKey == null )
			//		throw new Exception( @"NDO seems not to be installed -- can't run the enhancer. Registry Key HKLM\SOFTWARE\NDO is missing." );
			//	this.ndoPath = (string) registryKey.GetValue( "InstallDir" );
			//}
			//Trace.WriteLine( "Enhancer: " + Path.Combine( this.ndoPath, this.ToolName ) );
			return Path.Combine( Path.GetDirectoryName( GetType().Assembly.Location ), this.ToolName );
		}
	}
}
