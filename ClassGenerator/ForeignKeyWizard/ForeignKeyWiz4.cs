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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using WizardBase;


namespace ClassGenerator.ForeignKeyWizard
{
	/// <summary>
	/// Zusammenfassung für ForeignKeyWiz4.
	/// </summary>
	internal class ForeignKeyWiz4 : WizardBase.ViewBase
	{
		private System.Windows.Forms.TextBox txtRelationName;
		private System.Windows.Forms.Label label2;
		ForeignKeyWizModel model;

		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ForeignKeyWiz4()
		{
			InitializeComponent();
		}

		public ForeignKeyWiz4(IModel model)
		{
			InitializeComponent();
			this.model = (ForeignKeyWizModel) model;
		}

		/// <summary> 
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Komponenten-Designer generierter Code
		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtRelationName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtRelationName
			// 
			this.txtRelationName.Location = new System.Drawing.Point(136, 88);
			this.txtRelationName.Name = "txtRelationName";
			this.txtRelationName.Size = new System.Drawing.Size(152, 22);
			this.txtRelationName.TabIndex = 9;
			this.txtRelationName.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 24);
			this.label2.TabIndex = 8;
			this.label2.Text = "Relation Name:";
			// 
			// ForeignKeyWiz4
			// 
			this.Controls.Add(this.txtRelationName);
			this.Controls.Add(this.label2);
			this.Name = "ForeignKeyWiz4";
			this.Size = new System.Drawing.Size(528, 200);
			this.Load += new System.EventHandler(this.ForeignKeyWiz4_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void ForeignKeyWiz4_Load(object sender, System.EventArgs e)
		{
			FkRelation relation = (FkRelation) model.RelationNode.Relation;
			this.txtRelationName.DataBindings.Add("Text", relation, "RelationName");
			Frame.Description = "The relation name is used to distinguish between different relations to the same target type. If there aren't more than one relation to the same target type, leave this field empty.";
		}
	}
}
