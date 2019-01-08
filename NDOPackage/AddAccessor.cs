//
// Copyright (c) 2002-2019 Mirko Matytschak 
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


using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETDataObjects.NDOVSPackage
{
	class AddAccessor : AbstractCommand
	{

		public AddAccessor( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

		protected override void DoIt( object sender, EventArgs e )
		{			
			try
			{
				Document document;
				TextDocument textDoc;

				document = this.VisualStudioApplication.ActiveDocument;
				if (document == null)
					return;

				textDoc = (TextDocument)document.Object( "TextDocument" );
				if (textDoc == null)
					return;

				string fileName = document.FullName.ToLower();
				if (fileName.EndsWith( ".cs" ))
					new AddAccessorCs( textDoc, document ).DoIt();
				else if (fileName.EndsWith( ".vb" ))
					new AddAccessorVb( textDoc, document ).DoIt();
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
				MessageBox.Show( ex.Message, "Configure" );
			}
		}

		protected override void OnBeforeQueryStatus( object sender, EventArgs e )
		{
			OleMenuCommand item = sender as OleMenuCommand;
			bool enabled = false;
			Document document = this.VisualStudioApplication.ActiveDocument;
			if (document != null)
			{

				TextDocument textDoc = (TextDocument)document.Object( "TextDocument" );
				if (textDoc != null)
				{
					string fileName = document.FullName.ToLower();
					if (fileName.EndsWith( ".cs" ))
						enabled = true;
					else if (fileName.EndsWith( ".vb" ))
						enabled = true;
				}
			}
			item.Enabled = enabled;
		}

		public static implicit operator OleMenuCommand( AddAccessor abstractCommand )
		{
			return abstractCommand.command;
		}
	}
}
