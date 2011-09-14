using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UserSetup
{
	class Program
	{
		static readonly string xml = @"<?xml version=""1.0"" encoding=""UTF-16"" standalone=""no""?>
<Extensibility xmlns=""http://schemas.microsoft.com/AutomationExtensibility"">
	<Addin>
		<Assembly>###</Assembly>
	</Addin>
</Extensibility>";

		[STAThread]
		static int Main( string[] args )
		{
			string ndoPath = null;
			if ( args.Length > 0 )
			{
				ndoPath = args[0];
			}
			else
			{
				FolderBrowserDialog fbd = new FolderBrowserDialog();
				fbd.Description = "Select the NDO installation directory";
				fbd.SelectedPath = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles );
				DialogResult result = fbd.ShowDialog();
				if ( result == DialogResult.OK )
				{
					ndoPath = fbd.SelectedPath;
				}
				else
				{
					return -2;
				}
			}
			if ( !Directory.Exists( ndoPath ) )
			{
				MessageBox.Show( "NDO installation directory not found at '" + ndoPath +"'");
				return -1;
			}
			ndoPath = Path.Combine( ndoPath, "NDOEnhancer.dll" );
			string path = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), @"Microsoft\MSEnvShared\AddIns\NDO21.AddIn" );
			StreamWriter sw = new StreamWriter( path, false, Encoding.Unicode );
			sw.Write( xml.Replace( "###", ndoPath ) );
			sw.Close();

			return 0;
		}
	}
}
