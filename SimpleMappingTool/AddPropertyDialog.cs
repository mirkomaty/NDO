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
using NDO;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für AddPropertyDialog.
	/// </summary>
	public class AddPropertyDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.ComboBox cbTypes;
		private System.Windows.Forms.Label label4;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		static readonly string[] builtInTypes =
		{
			"System.Boolean", "System.Char", "System.SByte", "System.Byte", "System.Int16", "System.UInt16", "System.Int32", "System.UInt32", "System.Int64", "System.UInt64",
			"System.Single", "System.Double", "System.Decimal", "System.DateTime", "System.String"
		};


		public string PropName
		{
			get { return this.txtName.Text; }
			set { this.txtName.Text = value; }
		}

		public string Value
		{
			get { return this.txtValue.Text; }
			set { this.txtValue.Text = value; }
		}

		public string Type
		{
			get { return this.cbTypes.Text; }
			set { this.cbTypes.Text = value; }
		}

		public bool EditProperty
		{
			get { return !this.txtName.Enabled; }
			set { this.txtName.Enabled = !value; }
		}

		public AddPropertyDialog()
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();
			
			this.cbTypes.DataSource = builtInTypes;
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

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddPropertyDialog));
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbTypes = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(152, 24);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(264, 22);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "";
			// 
			// txtValue
			// 
			this.txtValue.Location = new System.Drawing.Point(152, 56);
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(264, 22);
			this.txtValue.TabIndex = 1;
			this.txtValue.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 24);
			this.label1.TabIndex = 2;
			this.label1.Text = "Name";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 24);
			this.label2.TabIndex = 3;
			this.label2.Text = "Value";
			// 
			// cbTypes
			// 
			this.cbTypes.CausesValidation = false;
			this.cbTypes.Location = new System.Drawing.Point(152, 88);
			this.cbTypes.Name = "cbTypes";
			this.cbTypes.Size = new System.Drawing.Size(264, 24);
			this.cbTypes.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 24);
			this.label3.TabIndex = 5;
			this.label3.Text = "Type";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(304, 160);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(112, 32);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(176, 160);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(112, 32);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(392, 40);
			this.label4.TabIndex = 8;
			this.label4.Text = "Use the NDOProperty class to convert the value and type strings into .NET objects" +
				" of the given type.";
			// 
			// AddPropertyDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(448, 206);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cbTypes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtValue);
			this.Controls.Add(this.txtName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AddPropertyDialog";
			this.Text = "Add Property";
			this.ResumeLayout(false);

		}
		#endregion

	}
}
