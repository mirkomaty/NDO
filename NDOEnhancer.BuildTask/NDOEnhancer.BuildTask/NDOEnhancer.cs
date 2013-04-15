using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NDO.BuildTask
{
	public class NDOEnhancer : ToolTask
	{
		[Required]
		public string NdoProjectFile {get; set;}

		[Required]
		public string NdoPlatformTarget {get; set;}

		private string ndoPath;

		protected override string ToolName
		{
			get
			{
				if ( NdoPlatformTarget == "x86" )
					return "EnhancerX86Stub.exe";
				return "NDOEnhancer.exe";
			}
		}

		protected override int ExecuteTool( string pathToTool, string responseFileCommands, string commandLineCommands )
		{
			ConsoleProcess cp = new ConsoleProcess( false );

			int result = cp.Execute( "\"" + pathToTool + "\"",
				"\"" + commandLineCommands.Trim() + "\"" );

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
			if ( null == this.ndoPath )
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\NDO" );
				if ( registryKey == null )
					registryKey = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\Wow6432Node\NDO" );
				if ( registryKey == null )
					throw new Exception( @"NDO seems not to be installed -- can't run the enhancer. Registry Key HKLM\SOFTWARE\NDO is missing." );
				this.ndoPath = (string) registryKey.GetValue( "InstallDir" );
			}
			Trace.WriteLine( "Enhancer: " + Path.Combine( this.ndoPath, this.ToolName ) );
			return Path.Combine( this.ndoPath, this.ToolName );
		}
	}
}
