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


using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETDataObjects.NDOVSPackage
{
	class AddRelation : AbstractCommand
	{

		public AddRelation( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

		protected override void OnBeforeQueryStatus( object sender, EventArgs e )
		{
			OleMenuCommand item = (OleMenuCommand)sender;

			bool enabled = false;
			Document document = base.dte.ActiveDocument;
			if (document != null)
			{
				TextDocument textDoc = (TextDocument)document.Object( "TextDocument" );
				if (textDoc != null)
				{
					string ext = Path.GetExtension( document.FullName ).ToLower();
					if (ext == ".cs" || ext == ".vb")
						enabled = true;
				}
			}

			item.Enabled = (enabled);
		}

		protected override void DoIt( object sender, EventArgs e )
		{
			OleMenuCommand item = sender as OleMenuCommand;
			Document document;

			document = base.dte.ActiveDocument;
			if (document == null)
				return;

			TextDocument textDoc = (TextDocument)document.Object( "TextDocument" );
			if (textDoc == null)
				return;

			StringBuilder sbResult = new System.Text.StringBuilder( 500 );

			RelationDialog dlg = new RelationDialog();
			DialogResult result = dlg.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

			string openBracket;
			string closeBracket;
			string typeOf;

			textDoc.Selection.StartOfLine( vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, false );


			string listLeft = null;
			string listRight = null;
			string startGeneric = null;
			string endGeneric = null;
			string spc = null;

			bool isVC;
			if (isVC = (Path.GetExtension( document.FullName ).ToLower() == ".cs"))
			{
				openBracket = "[";
				closeBracket = "]";
				typeOf = "typeof";
				spc = "\t\t";
				startGeneric = "<";
				endGeneric = ">";
			}
			else
			{
				openBracket = "<";
				closeBracket = "> _";
				typeOf = "GetType";
				spc = "\t";
				startGeneric = "(Of ";
				endGeneric = ")";
			}


			if (dlg.List)
			{
				if (!dlg.UseGenerics)
				{
					switch (dlg.ListType)
					{
						case ListType.IList:
							listLeft = "IList";
							listRight = "ArrayList";
							break;
						default:
							listLeft = listRight = "ArrayList";
							break;
					}
				}
				else
				{
					switch (dlg.ListType)
					{
						case ListType.IList:
							listLeft = "IList" + startGeneric + dlg.Type + endGeneric;
							listRight = "List" + startGeneric + dlg.Type + endGeneric;
							break;
						default:
							listLeft = listRight = "List" + startGeneric + dlg.Type + endGeneric;
							break;
					}
				}
			}


			try
			{
				sbResult.Append( spc + openBracket + "NDORelation" );
				if ((dlg.List && !dlg.UseGenerics) || dlg.Composite || dlg.RelationName.Trim() != string.Empty)
				{
					bool needsComma = false;
					sbResult.Append( "(" );
					if (dlg.List && !dlg.UseGenerics)
					{
						sbResult.Append( typeOf + '(' + dlg.Type + ')' );
						needsComma = true;
					}
					if (dlg.Composite)
					{
						if (needsComma)
							sbResult.Append( ", " );
						sbResult.Append( "RelationInfo.Composite" );
						needsComma = true;
					}
					if (dlg.RelationName.Trim() != string.Empty)
					{
						if (needsComma)
							sbResult.Append( ", " );
						sbResult.Append( "\"" + dlg.RelationName + "\"" );
					}
					sbResult.Append( ")" );

				}
				sbResult.Append( closeBracket + '\n' );

				if (isVC)
				{
					if (dlg.List)
						sbResult.Append( spc + listLeft + " " );
					else
						sbResult.Append( spc + dlg.Type + " " );
					sbResult.Append( dlg.FieldName );
					if (dlg.List)
					{
						sbResult.Append( " = new " + listRight );
						sbResult.Append( "()" );
					}

					sbResult.Append( ";\n" );
				}
				else
				{
					sbResult.Append( spc + "Dim " );
					sbResult.Append( dlg.FieldName );
					sbResult.Append( " As " );
					if (dlg.List)
						sbResult.Append( listLeft );
					else
						sbResult.Append( dlg.Type );
					if (dlg.List)
					{
						sbResult.Append( " = new " + listRight );
						sbResult.Append( "()" );
					}
					sbResult.Append( "\n" );
				}

				textDoc.Selection.Insert( sbResult.ToString(), (int)vsInsertFlags.vsInsertFlagsInsertAtStart );
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.Message, "Add Relation Add-in" );
			}
		}

		public static implicit operator OleMenuCommand( AddRelation addRelation )
		{
			return addRelation.command;
		}
	}
}
