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
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Resources;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using VSLangProj;
//using VSLangProj80;
using NDO.Mapping;
using NDOInterfaces;


namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for ConfigurationDialog.
	/// </summary>
	internal class ConfigurationDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.CheckBox chkActivateAddIn;
        private System.Windows.Forms.CheckBox chkActivateEnhancer;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnPresetLibrary;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.CheckBox chkMappingNew;
        private System.Windows.Forms.Button btnConnString;
		private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkChangeEvents;
		private System.Windows.Forms.CheckBox chkUseTimeStamps;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkGenerateSQLScript;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtDbOwner;
		private System.Windows.Forms.Button btnPresetApp;
		private System.Windows.Forms.Button btnSaveAs;

		private Project project;
		private System.Windows.Forms.ComboBox cbSqlDialect;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnNewDatabase;
		private System.Windows.Forms.RadioButton radioUtf8Encoding;
		private System.Windows.Forms.RadioButton radioDefaultEncoding;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtSchemaVersion;
        private CheckBox chkVerboseMode;
        private CheckBox chkIncludeTypecodes;
		private CheckBox chkDropExistingElements;
		private CheckBox chkGenerateFkConstraints;
		private ProjectDescription projectDescription;

		public ConfigurationDialog(Project project, ProjectDescription projectDescription)
		{
			this.project = project;
			this.projectDescription = projectDescription;
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ConfigurationDialog ) );
			this.chkActivateAddIn = new System.Windows.Forms.CheckBox();
			this.chkActivateEnhancer = new System.Windows.Forms.CheckBox();
			this.chkMappingNew = new System.Windows.Forms.CheckBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnPresetLibrary = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnConnString = new System.Windows.Forms.Button();
			this.txtConnectionString = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkChangeEvents = new System.Windows.Forms.CheckBox();
			this.chkUseTimeStamps = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkIncludeTypecodes = new System.Windows.Forms.CheckBox();
			this.radioDefaultEncoding = new System.Windows.Forms.RadioButton();
			this.radioUtf8Encoding = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.cbSqlDialect = new System.Windows.Forms.ComboBox();
			this.chkGenerateSQLScript = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtDbOwner = new System.Windows.Forms.TextBox();
			this.btnPresetApp = new System.Windows.Forms.Button();
			this.btnSaveAs = new System.Windows.Forms.Button();
			this.btnNewDatabase = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
			this.chkVerboseMode = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtSchemaVersion = new System.Windows.Forms.TextBox();
			this.chkGenerateFkConstraints = new System.Windows.Forms.CheckBox();
			this.chkDropExistingElements = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkActivateAddIn
			// 
			this.chkActivateAddIn.Location = new System.Drawing.Point( 20, 10 );
			this.chkActivateAddIn.Name = "chkActivateAddIn";
			this.chkActivateAddIn.Size = new System.Drawing.Size( 188, 21 );
			this.chkActivateAddIn.TabIndex = 0;
			this.chkActivateAddIn.Text = "Activate NDO AddIn";
			this.toolTip1.SetToolTip( this.chkActivateAddIn, "Choose this options, if your project contains or references persistent types." );
			this.chkActivateAddIn.Click += new System.EventHandler( this.chkActivateAddIn_CheckedChanged );
			// 
			// chkActivateEnhancer
			// 
			this.chkActivateEnhancer.Location = new System.Drawing.Point( 20, 37 );
			this.chkActivateEnhancer.Name = "chkActivateEnhancer";
			this.chkActivateEnhancer.Size = new System.Drawing.Size( 188, 20 );
			this.chkActivateEnhancer.TabIndex = 1;
			this.chkActivateEnhancer.Text = "Activate enhancer";
			this.toolTip1.SetToolTip( this.chkActivateEnhancer, "Choose this option, if your project contains persistent types." );
			this.chkActivateEnhancer.CheckedChanged += new System.EventHandler( this.chkActivateEnhancer_CheckedChanged );
			// 
			// chkMappingNew
			// 
			this.chkMappingNew.Location = new System.Drawing.Point( 20, 90 );
			this.chkMappingNew.Name = "chkMappingNew";
			this.chkMappingNew.Size = new System.Drawing.Size( 272, 21 );
			this.chkMappingNew.TabIndex = 4;
			this.chkMappingNew.Text = "Always Generate a new mapping File";
			this.toolTip1.SetToolTip( this.chkMappingNew, "Choose this options, if NDO should discard and rebuild all mapping information." );
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point( 473, 310 );
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size( 100, 39 );
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "Cancel";
			// 
			// btnPresetLibrary
			// 
			this.btnPresetLibrary.Location = new System.Drawing.Point( 17, 310 );
			this.btnPresetLibrary.Name = "btnPresetLibrary";
			this.btnPresetLibrary.Size = new System.Drawing.Size( 100, 39 );
			this.btnPresetLibrary.TabIndex = 9;
			this.btnPresetLibrary.Text = "Preset for Library";
			this.toolTip1.SetToolTip( this.btnPresetLibrary, "Selects all settings necessary for projects containing persistent types." );
			this.btnPresetLibrary.Click += new System.EventHandler( this.btnPresetLibrary_Click );
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point( 359, 310 );
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size( 100, 39 );
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
			// 
			// btnConnString
			// 
			this.btnConnString.Location = new System.Drawing.Point( 537, 276 );
			this.btnConnString.Name = "btnConnString";
			this.btnConnString.Size = new System.Drawing.Size( 42, 21 );
			this.btnConnString.TabIndex = 11;
			this.btnConnString.Text = "...";
			this.toolTip1.SetToolTip( this.btnConnString, "Enter existing database connection" );
			this.btnConnString.Click += new System.EventHandler( this.btnConnString_Click );
			// 
			// txtConnectionString
			// 
			this.txtConnectionString.Location = new System.Drawing.Point( 17, 276 );
			this.txtConnectionString.Name = "txtConnectionString";
			this.txtConnectionString.Size = new System.Drawing.Size( 514, 20 );
			this.txtConnectionString.TabIndex = 16;
			this.toolTip1.SetToolTip( this.txtConnectionString, "This string will be copied into the mapping file, if there doesn\'t exist a valid " +
        "connection string. Otherwise it will be ignored." );
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point( 17, 255 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 360, 21 );
			this.label2.TabIndex = 17;
			this.label2.Text = "Default Connection String";
			// 
			// chkChangeEvents
			// 
			this.chkChangeEvents.Location = new System.Drawing.Point( 20, 170 );
			this.chkChangeEvents.Name = "chkChangeEvents";
			this.chkChangeEvents.Size = new System.Drawing.Size( 302, 21 );
			this.chkChangeEvents.TabIndex = 19;
			this.chkChangeEvents.Text = "Generate change events with Add Accessor";
			this.toolTip1.SetToolTip( this.chkChangeEvents, "Check this option, if you intend to bind the UI directly to the accessor properti" +
        "es of your persistent classes." );
			// 
			// chkUseTimeStamps
			// 
			this.chkUseTimeStamps.Location = new System.Drawing.Point( 20, 144 );
			this.chkUseTimeStamps.Name = "chkUseTimeStamps";
			this.chkUseTimeStamps.Size = new System.Drawing.Size( 302, 20 );
			this.chkUseTimeStamps.TabIndex = 20;
			this.chkUseTimeStamps.Text = "Generate Time Stamp Columns for each class";
			this.toolTip1.SetToolTip( this.chkUseTimeStamps, "Check this option, if all tables should be protected by collistion detection." );
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add( this.chkDropExistingElements );
			this.groupBox1.Controls.Add( this.chkGenerateFkConstraints );
			this.groupBox1.Controls.Add( this.chkIncludeTypecodes );
			this.groupBox1.Controls.Add( this.radioDefaultEncoding );
			this.groupBox1.Controls.Add( this.radioUtf8Encoding );
			this.groupBox1.Controls.Add( this.label4 );
			this.groupBox1.Controls.Add( this.cbSqlDialect );
			this.groupBox1.Controls.Add( this.chkGenerateSQLScript );
			this.groupBox1.Location = new System.Drawing.Point( 342, 12 );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size( 265, 230 );
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " SQL ";
			// 
			// chkIncludeTypecodes
			// 
			this.chkIncludeTypecodes.Location = new System.Drawing.Point( 13, 173 );
			this.chkIncludeTypecodes.Name = "chkIncludeTypecodes";
			this.chkIncludeTypecodes.Size = new System.Drawing.Size( 235, 21 );
			this.chkIncludeTypecodes.TabIndex = 22;
			this.chkIncludeTypecodes.Text = "Include Typecodes in the Script";
			this.toolTip1.SetToolTip( this.chkIncludeTypecodes, "If checked, NDO generates instructions to build an additional table with the type" +
        " code information." );
			// 
			// radioDefaultEncoding
			// 
			this.radioDefaultEncoding.Location = new System.Drawing.Point( 13, 120 );
			this.radioDefaultEncoding.Name = "radioDefaultEncoding";
			this.radioDefaultEncoding.Size = new System.Drawing.Size( 140, 20 );
			this.radioDefaultEncoding.TabIndex = 21;
			this.radioDefaultEncoding.Text = "Default Encoding";
			this.toolTip1.SetToolTip( this.radioDefaultEncoding, "Check this option, if the script files should use windows encoding." );
			// 
			// radioUtf8Encoding
			// 
			this.radioUtf8Encoding.Checked = true;
			this.radioUtf8Encoding.Location = new System.Drawing.Point( 13, 99 );
			this.radioUtf8Encoding.Name = "radioUtf8Encoding";
			this.radioUtf8Encoding.Size = new System.Drawing.Size( 140, 21 );
			this.radioUtf8Encoding.TabIndex = 20;
			this.radioUtf8Encoding.TabStop = true;
			this.radioUtf8Encoding.Text = "UTF-8 Encoding";
			this.toolTip1.SetToolTip( this.radioUtf8Encoding, "Check this option, if the script files should be UTF-8 encoded." );
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point( 13, 50 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 200, 17 );
			this.label4.TabIndex = 19;
			this.label4.Text = "SQL Dialect";
			// 
			// cbSqlDialect
			// 
			this.cbSqlDialect.Location = new System.Drawing.Point( 13, 70 );
			this.cbSqlDialect.Name = "cbSqlDialect";
			this.cbSqlDialect.Size = new System.Drawing.Size( 220, 21 );
			this.cbSqlDialect.TabIndex = 18;
			this.toolTip1.SetToolTip( this.cbSqlDialect, "Choose an available NDO provider." );
			this.cbSqlDialect.SelectedIndexChanged += new System.EventHandler( this.cbSqlDialect_SelectedIndexChanged );
			// 
			// chkGenerateSQLScript
			// 
			this.chkGenerateSQLScript.Location = new System.Drawing.Point( 13, 21 );
			this.chkGenerateSQLScript.Name = "chkGenerateSQLScript";
			this.chkGenerateSQLScript.Size = new System.Drawing.Size( 187, 21 );
			this.chkGenerateSQLScript.TabIndex = 13;
			this.chkGenerateSQLScript.Text = "Generate SQL Script";
			this.toolTip1.SetToolTip( this.chkGenerateSQLScript, "If checked, NDO will create a script with DDL code, which can be used to construc" +
        "t a database structure." );
			this.chkGenerateSQLScript.CheckedChanged += new System.EventHandler( this.chkGenerateSQLScript_CheckedChanged );
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point( 17, 119 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 127, 21 );
			this.label3.TabIndex = 22;
			this.label3.Text = "Owner / Schema Name";
			// 
			// txtDbOwner
			// 
			this.txtDbOwner.Location = new System.Drawing.Point( 144, 117 );
			this.txtDbOwner.Name = "txtDbOwner";
			this.txtDbOwner.Size = new System.Drawing.Size( 160, 20 );
			this.txtDbOwner.TabIndex = 23;
			this.toolTip1.SetToolTip( this.txtDbOwner, "Enter an owner name, if you like your tables to be written like owner.tablename. " +
        "Mandatory for Oracle databases (enter SCOTT ;-) )." );
			// 
			// btnPresetApp
			// 
			this.btnPresetApp.Location = new System.Drawing.Point( 131, 310 );
			this.btnPresetApp.Name = "btnPresetApp";
			this.btnPresetApp.Size = new System.Drawing.Size( 100, 39 );
			this.btnPresetApp.TabIndex = 24;
			this.btnPresetApp.Text = "Preset for Application";
			this.toolTip1.SetToolTip( this.btnPresetApp, "Selects all settings used for applications which don\'t contain but reference pers" +
        "istent types." );
			this.btnPresetApp.Click += new System.EventHandler( this.btnPresetApp_Click );
			// 
			// btnSaveAs
			// 
			this.btnSaveAs.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSaveAs.Location = new System.Drawing.Point( 245, 310 );
			this.btnSaveAs.Name = "btnSaveAs";
			this.btnSaveAs.Size = new System.Drawing.Size( 100, 39 );
			this.btnSaveAs.TabIndex = 25;
			this.btnSaveAs.Text = "Save as...";
			this.toolTip1.SetToolTip( this.btnSaveAs, "Save the options to be used in unattended builds with the stand-alone enhancer." );
			this.btnSaveAs.Click += new System.EventHandler( this.btnSaveAs_Click );
			// 
			// btnNewDatabase
			// 
			this.btnNewDatabase.Location = new System.Drawing.Point( 582, 276 );
			this.btnNewDatabase.Name = "btnNewDatabase";
			this.btnNewDatabase.Size = new System.Drawing.Size( 42, 21 );
			this.btnNewDatabase.TabIndex = 26;
			this.btnNewDatabase.Text = "New";
			this.toolTip1.SetToolTip( this.btnNewDatabase, "Create new database" );
			this.btnNewDatabase.Click += new System.EventHandler( this.btnNewDatabase_Click );
			// 
			// chkVerboseMode
			// 
			this.chkVerboseMode.Location = new System.Drawing.Point( 20, 63 );
			this.chkVerboseMode.Name = "chkVerboseMode";
			this.chkVerboseMode.Size = new System.Drawing.Size( 238, 21 );
			this.chkVerboseMode.TabIndex = 29;
			this.chkVerboseMode.Text = "Add-in Verbose Mode";
			this.toolTip1.SetToolTip( this.chkVerboseMode, "Causes the Add-in and the Enhancer to show more information for debugging purpose" +
        "s." );
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point( 17, 211 );
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size( 121, 21 );
			this.label5.TabIndex = 27;
			this.label5.Text = "Schema Version";
			// 
			// txtSchemaVersion
			// 
			this.txtSchemaVersion.Location = new System.Drawing.Point( 144, 210 );
			this.txtSchemaVersion.Name = "txtSchemaVersion";
			this.txtSchemaVersion.Size = new System.Drawing.Size( 160, 20 );
			this.txtSchemaVersion.TabIndex = 28;
			// 
			// chkGenerateFkConstraints
			// 
			this.chkGenerateFkConstraints.Location = new System.Drawing.Point( 13, 151 );
			this.chkGenerateFkConstraints.Name = "chkGenerateFkConstraints";
			this.chkGenerateFkConstraints.Size = new System.Drawing.Size( 235, 21 );
			this.chkGenerateFkConstraints.TabIndex = 23;
			this.chkGenerateFkConstraints.Text = "Generate Foreign Key Constraints";
			this.toolTip1.SetToolTip( this.chkGenerateFkConstraints, "If checked, NDO generates foreign key constraints for the relations in the databa" +
        "se." );
			// 
			// chkDropExistingElements
			// 
			this.chkDropExistingElements.Location = new System.Drawing.Point( 13, 195 );
			this.chkDropExistingElements.Name = "chkDropExistingElements";
			this.chkDropExistingElements.Size = new System.Drawing.Size( 235, 21 );
			this.chkDropExistingElements.TabIndex = 24;
			this.chkDropExistingElements.Text = "Insert Drop Statements in the Script";
			this.toolTip1.SetToolTip( this.chkDropExistingElements, "If checked, NDO generates instructions to remove existing tables and constraints." +
        "" );
			// 
			// ConfigurationDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size( 682, 394 );
			this.Controls.Add( this.chkVerboseMode );
			this.Controls.Add( this.txtSchemaVersion );
			this.Controls.Add( this.label5 );
			this.Controls.Add( this.btnNewDatabase );
			this.Controls.Add( this.btnSaveAs );
			this.Controls.Add( this.btnPresetApp );
			this.Controls.Add( this.txtDbOwner );
			this.Controls.Add( this.txtConnectionString );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.groupBox1 );
			this.Controls.Add( this.chkUseTimeStamps );
			this.Controls.Add( this.chkChangeEvents );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.btnConnString );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.btnPresetLibrary );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.chkMappingNew );
			this.Controls.Add( this.chkActivateEnhancer );
			this.Controls.Add( this.chkActivateAddIn );
			this.Icon = ((System.Drawing.Icon) (resources.GetObject( "$this.Icon" )));
			this.Name = "ConfigurationDialog";
			this.Text = "NDO Configuration";
			this.Load += new System.EventHandler( this.ConfigurationDialog_Load );
			this.groupBox1.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}
		#endregion

		private void chkActivateAddIn_CheckedChanged(object sender, System.EventArgs e)
		{
            EnableAddin(this.chkActivateAddIn.Checked);
		}


		void EnableAddin(bool enabled)
		{
            if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
                chkActivateEnhancer.Enabled = false;
            else
			    chkActivateEnhancer.Enabled = enabled;
            chkActivateEnhancer_CheckedChanged(null, EventArgs.Empty);
		}



		private void ConfigurationDialog_Load(object sender, System.EventArgs e)
		{
            try
            {
                ConfigurationOptions options = new ConfigurationOptions(project);
                this.Text = "NDO Configuration - " + project.Name;
                chkIncludeTypecodes.Checked = options.IncludeTypecodes;
                chkDropExistingElements.Checked = options.DropExistingElements;
                chkGenerateFkConstraints.Checked = options.GenerateConstraints;
                chkVerboseMode.Checked = options.VerboseMode;
                chkChangeEvents.Checked = options.GenerateChangeEvents;
                chkActivateAddIn.Checked = options.EnableAddIn;
                chkActivateEnhancer.Checked = options.EnableEnhancer;
                chkMappingNew.Checked = options.NewMapping;
                if (chkActivateAddIn.Checked == false)
                    EnableAddin(false);
                chkGenerateSQLScript.Checked = options.GenerateSQL;
                txtSchemaVersion.Text = options.SchemaVersion;
                if (string.IsNullOrEmpty(txtSchemaVersion.Text))
                    txtSchemaVersion.Text = "1.0";
                this.chkUseTimeStamps.Checked = options.UseTimeStamps;
                int i = 0;
                this.cbSqlDialect.Items.Clear();
                int currentDialectIndex = -1;
                NDO.NDOProviderFactory.Instance.SkipPrivatePath = true;
                foreach (string s in NDO.NDOProviderFactory.Instance.ProviderNames) //GeneratorFactory.Instance.ProviderNames)
                {
                    if (NDO.NDOProviderFactory.Instance.Generators[s] != null)
                    {
                        this.cbSqlDialect.Items.Add(s);
                        if (options.SQLScriptLanguage == s)
                            currentDialectIndex = i;
                        i++;
                    }
                }
                if (currentDialectIndex > -1)
                    cbSqlDialect.SelectedIndex = currentDialectIndex;

                // Must be initialized after changing the cbSqlDialect index
                txtConnectionString.Text = options.DefaultConnection;
                txtDbOwner.Text = options.DatabaseOwner;
                chkActivateAddIn_CheckedChanged(null, EventArgs.Empty);
                chkActivateEnhancer_CheckedChanged(null, EventArgs.Empty);
                chkGenerateSQLScript_CheckedChanged(null, EventArgs.Empty);
                if (options.Utf8Encoding)
                    radioUtf8Encoding.Checked = true;
                else
                    radioDefaultEncoding.Checked = true;
                if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
                    this.chkActivateEnhancer.Enabled = false;
            }
            catch (Exception ex)
            {
#if !DEBUG
                MessageBox.Show(ex.Message, "NDO Configuration");
#else
                MessageBox.Show(ex.ToString(), "NDO Configuration");
#endif
            }
		}

		private void btnPresetLibrary_Click(object sender, System.EventArgs e)
		{
//			ConfigurationOptions options = new ConfigurationOptions( project );
			try
			{
				EnableCheckBoxes();

                this.chkVerboseMode.Checked = false;
                this.chkActivateAddIn.Checked = true;
				this.chkActivateEnhancer.Checked = true;
				this.chkMappingNew.Checked = false;
				this.chkChangeEvents.Checked = false;
                this.chkIncludeTypecodes.Checked = false;
				this.chkGenerateFkConstraints.Checked = false;
				this.chkDropExistingElements.Checked = false;
				this.chkUseTimeStamps.Checked = false;	
				this.txtDbOwner.Text = "";
				this.chkGenerateSQLScript.Checked = false;
				this.txtSchemaVersion.Text = "";
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}


		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				ConfigurationOptions options = new ConfigurationOptions( project );
		
				// Add the NDO dll to the project references
				if (chkActivateAddIn.Checked && !options.EnableAddIn)
				{
					string ndoPath = typeof(NDO.PersistenceManager).Assembly.Location;
					bool found = false;
					VSProject vsProject = project.Object as VSProject;
					if (vsProject != null)
					{
						foreach (VSLangProj.Reference r in vsProject.References)
						{
							if (r.Name.ToUpper() == "NDO" || string.Compare(r.Path, ndoPath, true) == 0)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							VSLangProj.References refs = ((VSProject)project.Object).References;
							refs.Add(ndoPath.Replace("\\ndo", "\\NDO"));
						}
					}
				}
                string connType = options.SQLScriptLanguage;
                string connName = options.DefaultConnection;
				WriteBack(options);
                if (options.SQLScriptLanguage != connType || options.DefaultConnection != connName)
                {
                    string mappingFileName = this.projectDescription.CheckedMappingFileName;
                    if (mappingFileName != null)
                    {
                        NDOMapping mapping = new NDOMapping(mappingFileName);
                        bool connectionExists = false;
                        foreach (Connection conn in mapping.Connections)
                        {
                            if (conn.Type == options.SQLScriptLanguage && conn.Name == options.DefaultConnection)
                            {
                                connectionExists = true;
                                break;
                            }
                        }
                        if (!connectionExists)
                        {
                            if (MessageBox.Show("The database connection settings have been changed. Should NDO change the connection settings in the mapping file " + Path.GetFileName(mappingFileName) + " too?", "NDO Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Connection conn = null;
                                if (mapping.Connections.Count == 1)
                                {
                                    conn = (Connection)mapping.Connections[0];
                                }
                                else
                                {
                                    conn = mapping.NewConnection(null, null);
                                    MessageBox.Show("Added a connection with the ID " + conn.ID, "NDO Configuration");
                                }
                                conn.Type = options.SQLScriptLanguage;
                                conn.Name = options.DefaultConnection;
                                mapping.Save();
                            }
                        }
                    }
                }

				options.Save(this.projectDescription);
			}
			catch (Exception ex)
			{
				MessageBox.Show("The following error occured while saving your options: " + ex.Message, "NDO Add-in");
			}
		}

		void ShowError(Exception ex)
		{
			MessageBox.Show("The following error occured: " + ex.Message, "NDO Add-in");
		}

		private void btnConnString_Click(object sender, System.EventArgs e)
		{
			try
			{
				string connType = cbSqlDialect.Text;
				if (connType == string.Empty)
					connType = "SqlServer";
				IProvider provider = NDO.NDOProviderFactory.Instance[connType];
				if (provider == null)
				{
					MessageBox.Show("Can't find a NDO provider for the sql dialect " + connType);
					return;
				}
				string temp = this.txtConnectionString.Text;
				DialogResult r = provider.ShowConnectionDialog(ref temp);
				if (r == DialogResult.Cancel)
					return;
				this.txtConnectionString.Text = temp;
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}


		private void chkGenerateSQLScript_CheckedChanged(object sender, System.EventArgs e)
		{
			bool genSql = this.chkGenerateSQLScript.Checked;

			this.cbSqlDialect.Enabled = genSql;
            this.chkIncludeTypecodes.Enabled = genSql;
			this.chkDropExistingElements.Enabled = genSql;
			this.chkGenerateFkConstraints.Enabled = genSql;
            this.radioUtf8Encoding.Enabled = genSql;
            this.radioDefaultEncoding.Enabled = genSql;
		}

		private void EnableCheckBoxes()
		{
			foreach(Control c in this.Controls)
			{
				CheckBox cb = c as CheckBox;
				if (c == null)
					continue;
				c.Enabled = true;
			}
            if (this.project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
                this.chkActivateEnhancer.Enabled = false;
		}

		private void btnPresetApp_Click(object sender, System.EventArgs e)
		{
            try
            {
                EnableCheckBoxes();
                this.chkVerboseMode.Checked = false;
                this.chkActivateAddIn.Checked = true;
                this.chkActivateEnhancer.Checked = false;
                this.chkMappingNew.Checked = false;
                this.chkGenerateSQLScript.Checked = true;
                this.chkChangeEvents.Checked = false;
                this.chkIncludeTypecodes.Checked = false;
				this.chkDropExistingElements.Checked = false;
				this.chkGenerateFkConstraints.Checked = false;
                this.txtConnectionString.Text = "";
                this.chkUseTimeStamps.Checked = false;
                this.txtDbOwner.Text = "";
                this.txtSchemaVersion.Text = "1.0";
                if (this.txtConnectionString.Text == string.Empty)
                    this.btnConnString_Click(null, null);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
		}

//		private void MakeNode(string name, object value, XmlNode parentNode, XmlDocument doc)
//		{
//			XmlElement el = doc.CreateElement(name);
//			parentNode.AppendChild(el);
//			if (value != null)
//				el.InnerText = value.ToString();
//		}

		void WriteBack(ConfigurationOptions options)
		{
			options.EnableAddIn = chkActivateAddIn.Checked;
			options.EnableEnhancer = chkActivateEnhancer.Checked;
            options.IncludeTypecodes = chkIncludeTypecodes.Checked;
			options.GenerateConstraints = chkGenerateFkConstraints.Checked;
			options.DropExistingElements = chkDropExistingElements.Checked;
            options.VerboseMode = chkVerboseMode.Checked;
            options.NewMapping = chkMappingNew.Checked;
			options.GenerateSQL = chkGenerateSQLScript.Checked;
			options.DefaultConnection = txtConnectionString.Text;
			options.GenerateChangeEvents = chkChangeEvents.Checked;
			options.UseTimeStamps = chkUseTimeStamps.Checked;
			options.SQLScriptLanguage = this.cbSqlDialect.Text;
			options.DatabaseOwner = this.txtDbOwner.Text;
			options.Utf8Encoding = this.radioUtf8Encoding.Checked;
			options.SchemaVersion = this.txtSchemaVersion.Text;
		}

		private void btnSaveAs_Click(object sender, System.EventArgs e)
		{
			try
			{
				string projDir = Path.GetDirectoryName(project.FullName);
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.CheckFileExists = false;
				sfd.DefaultExt = "xml";
				sfd.Filter = "NDO Configuration Files (*.ndoproj)|*.ndoproj";
				sfd.FileName = "EnhancerParameters.ndoproj";
				sfd.InitialDirectory = projDir;
				if (sfd.ShowDialog(this) != DialogResult.OK)
					return;

				ConfigurationOptions options = new ConfigurationOptions(project);				
				WriteBack(options);
				options.SaveAs(sfd.FileName, this.projectDescription);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}		
		}

		private void cbSqlDialect_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtConnectionString.Text = string.Empty;
		}

		private void btnNewDatabase_Click(object sender, System.EventArgs e)
		{
			string connType = cbSqlDialect.Text;
			if (connType == string.Empty)
				connType = "SqlServer";
			IProvider provider = NDO.NDOProviderFactory.Instance[connType];
			if (provider == null)
			{
				MessageBox.Show("Can't find a NDO provider for the sql dialect " + connType);
				return;
			}
			object necessaryData = null;
			try
			{
				if (provider.ShowCreateDbDialog(ref necessaryData) == DialogResult.Cancel)
					return;
				this.txtConnectionString.Text = provider.CreateDatabase(necessaryData);
			}
			catch (Exception ex)
			{
#if !DEBUG
				MessageBox.Show("Can't construct the database: " + ex.Message);
#else
				MessageBox.Show("Can't construct the database: " + ex.ToString());
#endif
			}
		}

        private void chkActivateEnhancer_CheckedChanged(object sender, EventArgs e)
        {
            
        }

	}
}
