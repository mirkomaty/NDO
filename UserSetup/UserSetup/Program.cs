//
// Copyright (c) 2002-2016 Mirko Matytschak 
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
