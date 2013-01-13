using System;
using System.IO;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE;

namespace NDOInstallHelper
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult NDOInstall( Session session )
		{
			new ActionPerformer( session.CustomActionData["INSTALLLOCATION"] ).Install();

			return ActionResult.Success;
		}
		[CustomAction]
		public static ActionResult NDOUninstall( Session session )
		{
			new ActionPerformer( session.CustomActionData["INSTALLLOCATION"] ).Uninstall();

			return ActionResult.Success;
		}
	}

	class ActionPerformer
	{
		string installPath;
		string logPath;
		string[] addInFileNames = { "NDO21-VS2010.AddIn", "NDO21-VS2012.AddIn" };

		public ActionPerformer(string installPath)
		{
			this.installPath = installPath;
			this.logPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "NDO" );
			if ( !Directory.Exists( logPath ) )
			{
				new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) ).CreateSubdirectory( "NDO" );
			}			
		}

		public void Install()
		{
			try
			{

				string enhPath = Path.Combine( this.installPath, "NDOEnhancer.dll" );
				string addInPath = Path.Combine( this.installPath, "NDO21.AddIn" );
				string addInText = null;
				using ( StreamReader sr = new StreamReader( addInPath, System.Text.Encoding.Unicode ) )
				{
					addInText = sr.ReadToEnd();
					sr.Close();
				}
				string addInTargetDir = GetAddInTargetPath( true );
				for ( int i = 0; i < this.addInFileNames.Length; i++ )
				{
					string addInFileName = this.addInFileNames[i];
					try
					{
						addInText = addInText.Replace( @"YourAddInPath", enhPath );

						if ( i == 1 )
							addInText = addInText.Replace( "<Version>10.0", "<Version>11.0" );

						string addInTargetPath = Path.Combine( addInTargetDir, addInFileName );
						StreamWriter sw = new StreamWriter( addInTargetPath, false, System.Text.Encoding.Unicode );
						sw.Write( addInText );
						sw.Close();
					}
					catch ( Exception ex )
					{
						Log( ex.Message, true );
					}
				}
			}
			catch ( Exception ex )
			{
				Log( ex.Message, true );				
			}
		}

		string GetAddInTargetPath( bool createIfNotExists )
		{
			string root = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );

			string subPath = null;
			string returnPath = null;
			try
			{
				subPath = @"Microsoft\MSEnvShared\Addins";

				returnPath = Path.Combine( root, subPath );

				if ( createIfNotExists )
				{
					Log( "Create AddInTargetPath = " + returnPath, false );
					DirectoryInfo di = new DirectoryInfo( root );
					di.CreateSubdirectory( subPath );
				}
			}
			catch(Exception ex) 
			{
				Log( ex.ToString(), true );
			}
			return returnPath;
		}

		void Log( string msg, bool isError )
		{
			if (isError)
				msg += @"
Please install or uninstall the NDO .addin files manually from the folder %ALLUSERSPROFILE%\Microsoft\MSEnvShared\addins";

			File.AppendAllText(Path.Combine( this.logPath, "InstLog.txt" ), msg );
		}

		public void Uninstall()
		{
			try
			{
				foreach ( string addInFileName in this.addInFileNames )
				{
					string addInTargetPath = Path.Combine( GetAddInTargetPath( false ), addInFileName );
					if ( File.Exists( addInTargetPath ) )
						File.Delete( addInTargetPath );
				}
			}
			catch { }

			try
			{
				Type t = Type.GetTypeFromProgID( "VisualStudio.DTE" );
				EnvDTE.DTE dte = (DTE) Activator.CreateInstance( t );
				CommandBars cbs = (CommandBars) dte.CommandBars;
				Commands cmds = dte.Commands;
				foreach ( CommandBar cb in cbs )
				{
					if ( cb.Name.StartsWith( ".NET Data Objects" ) )
						cmds.RemoveCommandBar( cb );
				}
			}
			catch { }
		}
	}
}
