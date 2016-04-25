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


namespace ClassGenerator.AddPrimaryKeyWizard
{
    partial class AddPrimaryKeyWiz
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPrimaryKeyWiz));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioGuid = new System.Windows.Forms.RadioButton();
            this.radioString = new System.Windows.Forms.RadioButton();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.radioInteger = new System.Windows.Forms.RadioButton();
            this.txtColumnName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioGuid);
            this.groupBox1.Controls.Add(this.radioString);
            this.groupBox1.Controls.Add(this.radioAuto);
            this.groupBox1.Controls.Add(this.radioInteger);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Oid Type";
            // 
            // radioGuid
            // 
            this.radioGuid.AutoSize = true;
            this.radioGuid.Location = new System.Drawing.Point(15, 105);
            this.radioGuid.Name = "radioGuid";
            this.radioGuid.Size = new System.Drawing.Size(59, 21);
            this.radioGuid.TabIndex = 4;
            this.radioGuid.TabStop = true;
            this.radioGuid.Text = "Guid";
            this.radioGuid.UseVisualStyleBackColor = true;
            // 
            // radioString
            // 
            this.radioString.AutoSize = true;
            this.radioString.Location = new System.Drawing.Point(15, 78);
            this.radioString.Name = "radioString";
            this.radioString.Size = new System.Drawing.Size(66, 21);
            this.radioString.TabIndex = 3;
            this.radioString.TabStop = true;
            this.radioString.Text = "String";
            this.radioString.UseVisualStyleBackColor = true;
            // 
            // radioAuto
            // 
            this.radioAuto.AutoSize = true;
            this.radioAuto.Location = new System.Drawing.Point(15, 51);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.Size = new System.Drawing.Size(122, 21);
            this.radioAuto.TabIndex = 2;
            this.radioAuto.TabStop = true;
            this.radioAuto.Text = "Autonumbered";
            this.radioAuto.UseVisualStyleBackColor = true;
            // 
            // radioInteger
            // 
            this.radioInteger.AutoSize = true;
            this.radioInteger.Location = new System.Drawing.Point(15, 24);
            this.radioInteger.Name = "radioInteger";
            this.radioInteger.Size = new System.Drawing.Size(73, 21);
            this.radioInteger.TabIndex = 1;
            this.radioInteger.TabStop = true;
            this.radioInteger.Text = "Integer";
            this.radioInteger.UseVisualStyleBackColor = true;
            // 
            // txtColumnName
            // 
            this.txtColumnName.Location = new System.Drawing.Point(234, 56);
            this.txtColumnName.Name = "txtColumnName";
            this.txtColumnName.Size = new System.Drawing.Size(188, 22);
            this.txtColumnName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(231, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Column Name";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(339, 168);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(230, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddPrimaryKeyWiz
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(446, 217);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtColumnName);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddPrimaryKeyWiz";
            this.Text = "Add Primary Key";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioGuid;
        private System.Windows.Forms.RadioButton radioString;
        private System.Windows.Forms.RadioButton radioAuto;
        private System.Windows.Forms.RadioButton radioInteger;
        private System.Windows.Forms.TextBox txtColumnName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}