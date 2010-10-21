//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif
using System.Text.RegularExpressions;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für AddRelation.
	/// </summary>
	internal class AddRelation : AbstractCommand
	{
		public AddRelation()
		{
			this.CommandBarButtonText = "Add Relation";
            this.CommandBarButtonToolTip = "Adds a relation";
//            this.MyCommandName = "NDOEnhancer.Connect.AddRelation";
		}

		TextDocument textDoc;
		System.Text.StringBuilder sbResult;

		private void Write(string text)
		{
			sbResult.Append(text);
		}

		private void WriteDoc()
		{
			textDoc.Selection.Insert(sbResult.ToString(), (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
		}


		public void DoIt()
		{
			Document document;

			document = this.VisualStudioApplication.ActiveDocument;
			if (document == null) 
				return;
			
			textDoc = (TextDocument) document.Object("TextDocument");
			if (textDoc == null) 
				return;

			this.sbResult = new System.Text.StringBuilder(500);

			RelationDialog dlg = new RelationDialog();
			DialogResult result = dlg.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

			string openBracket;
			string closeBracket;
			string typeOf;

			textDoc.Selection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, false);


			string listLeft = null;
			string listRight = null;
			string startGeneric = null;
			string endGeneric = null;
			string spc = null;

			bool isVC;
			if (isVC = (Path.GetExtension(document.FullName).ToLower() == ".cs"))
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
						case ListType.ArrayList:
							listLeft = listRight = "ArrayList";
							break;
						case ListType.IList:
							listLeft = "IList";
							listRight = "ArrayList";
							break;
						default:
							listLeft = listRight = "NDOArrayList";
							break;
					}
				}
				else
				{
					switch (dlg.ListType)
					{
						case ListType.ArrayList:
							listLeft = listRight = "List" + startGeneric + dlg.Type + endGeneric;
							break;
						case ListType.IList:
							listLeft = "IList" + startGeneric + dlg.Type + endGeneric;
							listRight = "List" + startGeneric + dlg.Type + endGeneric;
							break;
						default:
							listLeft = listRight = "NDOGenericList" + startGeneric + dlg.Type + endGeneric;
							break;
					}
				}
			}


			try 
			{
				Write(spc + openBracket + "NDORelation");
				if ((dlg.List && !dlg.UseGenerics) || dlg.Composite || dlg.RelationName.Trim() != string.Empty)
				{
					bool needsComma = false;
					Write("(");
					if (dlg.List && !dlg.UseGenerics)
					{
						Write(typeOf + '(' + dlg.Type + ')');
						needsComma = true;
					}
					if (dlg.Composite)
					{
						if (needsComma)
							Write(", ");
						Write("RelationInfo.Composite");
						needsComma = true;
					}
					if (dlg.RelationName.Trim() != string.Empty)
					{
						if (needsComma)
							Write(", ");
						Write("\"" + dlg.RelationName + "\"");						
					}
					Write(")");
					
				}
				Write(closeBracket + '\n');

				if (isVC)
				{
					if (dlg.List)
						Write(spc + listLeft + " ");
					else 
						Write(spc + dlg.Type + " ");
					Write(dlg.FieldName);
					if (dlg.List)
					{
						Write(" = new " + listRight);
						if (dlg.ListType == ListType.NDOArrayList && !dlg.UseGenerics)
							Write("(typeof(" + dlg.Type + "))");
						else
							Write("()");
					}
					
					Write(";\n");
				}
				else
				{
					Write (spc + "Dim ");
					Write(dlg.FieldName);
					Write (" As ");
					if (dlg.List)
						Write(listLeft);
					else 
						Write(dlg.Type);
					if (dlg.List)
					{
						Write(" = new " + listRight);
						if (dlg.ListType == ListType.NDOArrayList && !dlg.UseGenerics)
							Write("(GetType(" + dlg.Type + "))");
						else
							Write("()");
					}
					Write("\n");
				}

				WriteDoc();
			}				
			catch (Exception e) 
			{
				MessageBox.Show(e.Message, "Add Relation Add-in");
			}
		}
	
		#region IDTCommandTarget Member

		public override void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
		{
			if ( commandName == MyCommandName )
			{
				DoIt();
				handled = true;
			}
		}

		public override void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if ( commandName == MyCommandName )
			{
				bool enabled = false;
				Document document = this.VisualStudioApplication.ActiveDocument;
				if (document != null) 
				{
			
					TextDocument textDoc = (TextDocument) document.Object("TextDocument");
					if (textDoc != null) 
					{
						string ext = Path.GetExtension(document.FullName).ToLower();
						if (ext == ".cs" || ext == ".vb")
							enabled = true;
					}						
				}

				if (enabled)
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
				else
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported;

			}
		}

		#endregion


		#region IDTExtensibility2 Member

		public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;

            Debug.WriteLine("AddRelation.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("AddRelation.OnConnection: command already exists");
                return;
                //----------------
            }

            Debug.WriteLine("AddRelation.OnConnection: creating command");

			try
			{

				// AddRelation-Kommando
				Command command = this.AddNamedCommand( 105 );

                CommandBar commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Code Window"];
				CommandBarButton cbb = (CommandBarButton) command.AddControl(commandBar, 1);
				// use default for cbb.Style (context menu)

				commandBar = NDOCommandBar.Instance.CommandBar;
				cbb = (CommandBarButton) command.AddControl( commandBar, 2 );
				cbb.Style = MsoButtonStyle.msoButtonIcon;
			}
			catch (Exception e)
			{
#if DEBUG
				MessageBox.Show(e.ToString(), "NDO Add Relation");
#else
				MessageBox.Show(e.Message, "NDO Add Relation");
#endif
            }
		}


		#endregion
	}
}
	


