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


using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;

namespace NETDataObjects.NDOVSPackage
{
	/// <summary>
	/// Zusammenfassung für MergeConflictBase.
	/// </summary>
	internal abstract class MergeConflictBase : AbstractCommand
	{
		public MergeConflictBase( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

		protected bool GetConflictPositions( out EditPoint ep, out int myCodeOffset, out int cgCodeOffset, out int endConflictOffset )
		{
			Document document;
			TextDocument textDoc;

			// Values will be ignored, if we return mit false
			ep = null;
			myCodeOffset = 0;
			cgCodeOffset = 0;
			endConflictOffset = 0;

			document = this.VisualStudioApplication.ActiveDocument;
			if (document == null)
				return false;

			textDoc = (TextDocument)document.Object( "TextDocument" );
			if (textDoc == null)
				return false;

			ep = textDoc.Selection.ActivePoint.CreateEditPoint();
			textDoc.Selection.SelectLine();

			if (textDoc.Selection.Text.IndexOf( "!!!! ClassGenerator merge conflict !!!! Your code follows:" ) < 0)
				return false;  // Nothing to do

			ep.StartOfLine();
			myCodeOffset = ep.AbsoluteCharOffset;

			while (true)
			{
				if (ep.AtEndOfDocument)
					return false;
				ep.LineDown( 1 );
				if (ep.GetText( ep.LineLength ).IndexOf( "!!!! The ClassGenerator's code follows:" ) >= 0)
					break;
			}
			cgCodeOffset = ep.AbsoluteCharOffset;
			while (true)
			{
				if (ep.AtEndOfDocument)
					return false;
				ep.LineDown( 1 );
				if (ep.GetText( ep.LineLength ).IndexOf( "!!!! End of merge conflict" ) >= 0)
					break;
			}
			endConflictOffset = ep.AbsoluteCharOffset;
			return true;
		}

		public abstract void DoIt();

		protected override void DoIt( object sender, EventArgs e )
		{
			DoIt();
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
					if (enabled)
					{
						EditPoint ep = textDoc.Selection.ActivePoint.CreateEditPoint();
						ep.StartOfLine();
						string s = ep.GetText( ep.LineLength );
						if (s.IndexOf( "!!!! ClassGenerator merge conflict !!!! Your code follows:" ) < 0)
							enabled = false;
					}
				}
			}

			item.Enabled = enabled;
		}

		public static implicit operator OleMenuCommand( MergeConflictBase abstractCommand )
		{
			return abstractCommand.command;
		}
	}
}



