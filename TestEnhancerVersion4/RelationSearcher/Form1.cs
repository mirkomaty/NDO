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
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using NDO;
using NDO.Mapping;

namespace RelationSearcher
{
	/// <summary>
	/// Zusammenfassung fï¿½r Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.CheckBox cbComposite;
		private System.Windows.Forms.CheckBox cbWithTable;
		private System.Windows.Forms.CheckBox cbList;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.CheckBox cbBidirectional;
		private System.Windows.Forms.ListBox lbResult;
		private System.Windows.Forms.CheckBox cbHasSubclasses;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Erforderlich fï¿½r die Windows Form-Designerunterstï¿½tzung
			//
			InitializeComponent();

			//
			// TODO: Fï¿½gen Sie den Konstruktorcode nach dem Aufruf von InitializeComponent hinzu
			//
		}

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode fï¿½r die Designerunterstï¿½tzung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geï¿½ndert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.cbComposite = new System.Windows.Forms.CheckBox();
			this.cbWithTable = new System.Windows.Forms.CheckBox();
			this.cbList = new System.Windows.Forms.CheckBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.cbBidirectional = new System.Windows.Forms.CheckBox();
			this.lbResult = new System.Windows.Forms.ListBox();
			this.cbHasSubclasses = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// cbComposite
			// 
			this.cbComposite.Location = new System.Drawing.Point(24, 24);
			this.cbComposite.Name = "cbComposite";
			this.cbComposite.Size = new System.Drawing.Size(224, 24);
			this.cbComposite.TabIndex = 0;
			this.cbComposite.Text = "Composite";
			// 
			// cbWithTable
			// 
			this.cbWithTable.Location = new System.Drawing.Point(24, 56);
			this.cbWithTable.Name = "cbWithTable";
			this.cbWithTable.Size = new System.Drawing.Size(224, 24);
			this.cbWithTable.TabIndex = 1;
			this.cbWithTable.Text = "With Table";
			// 
			// cbList
			// 
			this.cbList.Location = new System.Drawing.Point(24, 88);
			this.cbList.Name = "cbList";
			this.cbList.Size = new System.Drawing.Size(224, 24);
			this.cbList.TabIndex = 2;
			this.cbList.Text = "List";
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(24, 168);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(120, 32);
			this.btnSearch.TabIndex = 3;
			this.btnSearch.Text = "Search";
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// cbBidirectional
			// 
			this.cbBidirectional.Location = new System.Drawing.Point(24, 120);
			this.cbBidirectional.Name = "cbBidirectional";
			this.cbBidirectional.Size = new System.Drawing.Size(224, 24);
			this.cbBidirectional.TabIndex = 5;
			this.cbBidirectional.Text = "Bidirectional";
			// 
			// lbResult
			// 
			this.lbResult.ItemHeight = 16;
			this.lbResult.Location = new System.Drawing.Point(24, 216);
			this.lbResult.Name = "lbResult";
			this.lbResult.Size = new System.Drawing.Size(712, 132);
			this.lbResult.TabIndex = 6;
			// 
			// cbHasSubclasses
			// 
			this.cbHasSubclasses.Location = new System.Drawing.Point(304, 16);
			this.cbHasSubclasses.Name = "cbHasSubclasses";
			this.cbHasSubclasses.Size = new System.Drawing.Size(184, 32);
			this.cbHasSubclasses.TabIndex = 7;
			this.cbHasSubclasses.Text = "Has Subclasses";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(760, 368);
			this.Controls.Add(this.cbHasSubclasses);
			this.Controls.Add(this.lbResult);
			this.Controls.Add(this.cbBidirectional);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.cbList);
			this.Controls.Add(this.cbWithTable);
			this.Controls.Add(this.cbComposite);
			this.Name = "Form1";
			this.Text = "Relation Searcher";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Der Haupteinstiegspunkt fï¿½r die Anwendung.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			this.lbResult.Items.Clear();
			PersistenceManager pm = new PersistenceManager();			
			foreach(Class cl in pm.NDOMapping.Classes)
			{
				Type t = cl.SystemType;
				foreach(Relation r in cl.Relations)
				{
					if (r.FieldName == "belege")
						System.Diagnostics.Debug.WriteLine("kjkj");
					string s = "";
					if (((cbComposite.Checked && r.Composition) || (!cbComposite.Checked && !r.Composition))
						&& ((cbBidirectional.Checked && r.Bidirectional) || !cbBidirectional.Checked)
						&& ((cbList.Checked && (r.Multiplicity == RelationMultiplicity.List)) || ((!cbList.Checked) && (r.Multiplicity != RelationMultiplicity.List)))
						&& ((cbWithTable.Checked && (r.MappingTable != null)) || (!cbWithTable.Checked && (r.MappingTable == null)))
						&& ((cbHasSubclasses.Checked && r.ReferencedSubClasses.Count > 1) || (!cbHasSubclasses.Checked && !(r.ReferencedSubClasses.Count > 1))))
					{
						s = cl.FullName + "->" + r.ReferencedTypeName;
						if (r.RelationName != string.Empty)
							s += "(Role: " + r.RelationName + ")";
						this.lbResult.Items.Add(s);
					}
				}
			}
		}
	}
}
