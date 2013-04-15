using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;

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
