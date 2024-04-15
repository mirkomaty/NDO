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
using System.Linq;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Project = EnvDTE.Project;
using System.Diagnostics;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using NDO.UISupport;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace NDOVsPackage
{
	/// <summary>
	/// Summary description for ConfigurationDialog.
	/// </summary>
	internal class ConfigurationDialog : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private Project project;
		private System.Windows.Forms.ToolTip toolTip1;
		private TabControl tabControl;
		private TabPage tabPageGeneral;
		private TextBox txtTargetMappingFileName;
		private Label label1;
		private CheckBox chkVerboseMode;
		private TextBox txtSchemaVersion;
		private Label label5;
		private Button btnNewDatabase;
		private Button btnSaveAs;
		private Button btnPresetApp;
		private TextBox txtDbOwner;
		private TextBox txtConnectionString;
		private Label label3;
		private GroupBox groupBox1;
		private CheckBox chkDropExistingElements;
		private CheckBox chkGenerateFkConstraints;
		private CheckBox chkIncludeTypecodes;
		private RadioButton radioDefaultEncoding;
		private RadioButton radioUtf8Encoding;
		private Label label4;
		private ComboBox cbSqlDialect;
		private CheckBox chkGenerateSQLScript;
		private CheckBox chkUseTimeStamps;
		private CheckBox chkChangeEvents;
		private Label label2;
		private Button btnConnString;
		private Button btnOK;
		private Button btnPresetLibrary;
		private Button btnCancel;
		private CheckBox chkMappingNew;
		private CheckBox chkActivateEnhancer;
		private CheckBox chkActivateAddIn;
		private TabPage tabPageAssemblies;
		private Label label6;
		private CheckedListBox chlbAssemblies;
		private ProjectDescription projectDescription;
		private Button btnInstallProvider;
		private NDOReference[] references;

		public ConfigurationDialog(Project project, ProjectDescription projectDescription)
		{
			try
			{
				this.project = project;
				this.projectDescription = projectDescription;
				this.projectDescription.BuildReferences();
				this.projectDescription.FixDllState();
				InitializeComponent();
				var lbAssemblies = (ListBox)this.chlbAssemblies;
				this.references = projectDescription.References.Values.ToArray();
				lbAssemblies.DataSource = this.references;
				lbAssemblies.DisplayMember = nameof( NDOReference.Name );
				lbAssemblies.ValueMember = nameof( NDOReference.CheckState );
				int i = 0;
				foreach (var item in this.references)
				{
					this.chlbAssemblies.SetItemCheckState( i++, item.CheckState );
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
			}
		}

		void CheckEnabledStateForLoadProviderButton()
		{
			string providerName = this.cbSqlDialect.Text;

			if (string.IsNullOrEmpty( providerName ))
			{
				this.btnInstallProvider.Enabled = false;
				return;
			}

			if (IsProviderInstalled(providerName))
			{
				this.btnInstallProvider.Enabled = false;
				return;
			}

			this.btnInstallProvider.Enabled = true;
		}

		bool IsProviderInstalled(string providerName)
		{
			IComponentModel componentModel;
			var installerService = GetInstallerService( out componentModel );

			string packageName = $"ndo.{providerName.ToLower()}";
			return installerService.IsPackageInstalled( this.project, packageName );
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkVerboseMode = new System.Windows.Forms.CheckBox();
			this.btnNewDatabase = new System.Windows.Forms.Button();
			this.btnSaveAs = new System.Windows.Forms.Button();
			this.btnPresetApp = new System.Windows.Forms.Button();
			this.txtDbOwner = new System.Windows.Forms.TextBox();
			this.txtConnectionString = new System.Windows.Forms.TextBox();
			this.chkDropExistingElements = new System.Windows.Forms.CheckBox();
			this.chkGenerateFkConstraints = new System.Windows.Forms.CheckBox();
			this.chkIncludeTypecodes = new System.Windows.Forms.CheckBox();
			this.radioDefaultEncoding = new System.Windows.Forms.RadioButton();
			this.radioUtf8Encoding = new System.Windows.Forms.RadioButton();
			this.cbSqlDialect = new System.Windows.Forms.ComboBox();
			this.chkGenerateSQLScript = new System.Windows.Forms.CheckBox();
			this.chkUseTimeStamps = new System.Windows.Forms.CheckBox();
			this.chkChangeEvents = new System.Windows.Forms.CheckBox();
			this.btnConnString = new System.Windows.Forms.Button();
			this.btnPresetLibrary = new System.Windows.Forms.Button();
			this.chkMappingNew = new System.Windows.Forms.CheckBox();
			this.chkActivateEnhancer = new System.Windows.Forms.CheckBox();
			this.chkActivateAddIn = new System.Windows.Forms.CheckBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.txtTargetMappingFileName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtSchemaVersion = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnInstallProvider = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageAssemblies = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.chlbAssemblies = new System.Windows.Forms.CheckedListBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageAssemblies.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkVerboseMode
			// 
			this.chkVerboseMode.Location = new System.Drawing.Point(21, 72);
			this.chkVerboseMode.Name = "chkVerboseMode";
			this.chkVerboseMode.Size = new System.Drawing.Size(238, 21);
			this.chkVerboseMode.TabIndex = 51;
			this.chkVerboseMode.Text = "Add-in Verbose Mode";
			this.toolTip1.SetToolTip(this.chkVerboseMode, "Causes the Add-in and the Enhancer to show more information for debugging purpose" +
        "s.");
			// 
			// btnNewDatabase
			// 
			this.btnNewDatabase.Location = new System.Drawing.Point(583, 313);
			this.btnNewDatabase.Name = "btnNewDatabase";
			this.btnNewDatabase.Size = new System.Drawing.Size(42, 21);
			this.btnNewDatabase.TabIndex = 48;
			this.btnNewDatabase.Text = "New";
			this.toolTip1.SetToolTip(this.btnNewDatabase, "Create new database");
			this.btnNewDatabase.Click += new System.EventHandler(this.btnNewDatabase_Click);
			// 
			// btnSaveAs
			// 
			this.btnSaveAs.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSaveAs.Location = new System.Drawing.Point(249, 385);
			this.btnSaveAs.Name = "btnSaveAs";
			this.btnSaveAs.Size = new System.Drawing.Size(100, 39);
			this.btnSaveAs.TabIndex = 47;
			this.btnSaveAs.Text = "Save as...";
			this.toolTip1.SetToolTip(this.btnSaveAs, "Save the options to be used in unattended builds with the stand-alone enhancer.");
			this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
			// 
			// btnPresetApp
			// 
			this.btnPresetApp.Location = new System.Drawing.Point(135, 385);
			this.btnPresetApp.Name = "btnPresetApp";
			this.btnPresetApp.Size = new System.Drawing.Size(100, 39);
			this.btnPresetApp.TabIndex = 46;
			this.btnPresetApp.Text = "Preset for Application";
			this.toolTip1.SetToolTip(this.btnPresetApp, "Selects all settings used for applications which don\'t contain but reference pers" +
        "istent types.");
			this.btnPresetApp.Click += new System.EventHandler(this.btnPresetApp_Click);
			// 
			// txtDbOwner
			// 
			this.txtDbOwner.Location = new System.Drawing.Point(145, 126);
			this.txtDbOwner.Name = "txtDbOwner";
			this.txtDbOwner.Size = new System.Drawing.Size(160, 20);
			this.txtDbOwner.TabIndex = 45;
			this.toolTip1.SetToolTip(this.txtDbOwner, "Enter an owner name, if you like your tables to be written like owner.tablename.");
			// 
			// txtConnectionString
			// 
			this.txtConnectionString.Location = new System.Drawing.Point(18, 313);
			this.txtConnectionString.Name = "txtConnectionString";
			this.txtConnectionString.Size = new System.Drawing.Size(514, 20);
			this.txtConnectionString.TabIndex = 39;
			this.toolTip1.SetToolTip(this.txtConnectionString, "This string will be copied into the mapping file, if there doesn\'t exist a valid " +
        "connection string. Otherwise it will be ignored.");
			// 
			// chkDropExistingElements
			// 
			this.chkDropExistingElements.Location = new System.Drawing.Point(13, 202);
			this.chkDropExistingElements.Name = "chkDropExistingElements";
			this.chkDropExistingElements.Size = new System.Drawing.Size(235, 21);
			this.chkDropExistingElements.TabIndex = 24;
			this.chkDropExistingElements.Text = "Insert Drop Statements in the Script";
			this.toolTip1.SetToolTip(this.chkDropExistingElements, "If checked, NDO generates instructions to remove existing tables and constraints." +
        "");
			// 
			// chkGenerateFkConstraints
			// 
			this.chkGenerateFkConstraints.Location = new System.Drawing.Point(13, 152);
			this.chkGenerateFkConstraints.Name = "chkGenerateFkConstraints";
			this.chkGenerateFkConstraints.Size = new System.Drawing.Size(235, 21);
			this.chkGenerateFkConstraints.TabIndex = 23;
			this.chkGenerateFkConstraints.Text = "Generate Foreign Key Constraints";
			this.toolTip1.SetToolTip(this.chkGenerateFkConstraints, "If checked, NDO generates foreign key constraints for the relations in the databa" +
        "se.");
			// 
			// chkIncludeTypecodes
			// 
			this.chkIncludeTypecodes.Location = new System.Drawing.Point(13, 177);
			this.chkIncludeTypecodes.Name = "chkIncludeTypecodes";
			this.chkIncludeTypecodes.Size = new System.Drawing.Size(235, 21);
			this.chkIncludeTypecodes.TabIndex = 22;
			this.chkIncludeTypecodes.Text = "Include Typecodes in the Script";
			this.toolTip1.SetToolTip(this.chkIncludeTypecodes, "If checked, NDO generates instructions to build an additional table with the type" +
        " code information.");
			// 
			// radioDefaultEncoding
			// 
			this.radioDefaultEncoding.Location = new System.Drawing.Point(13, 121);
			this.radioDefaultEncoding.Name = "radioDefaultEncoding";
			this.radioDefaultEncoding.Size = new System.Drawing.Size(140, 20);
			this.radioDefaultEncoding.TabIndex = 21;
			this.radioDefaultEncoding.Text = "Default Encoding";
			this.toolTip1.SetToolTip(this.radioDefaultEncoding, "Check this option, if the script files should use windows encoding.");
			// 
			// radioUtf8Encoding
			// 
			this.radioUtf8Encoding.Checked = true;
			this.radioUtf8Encoding.Location = new System.Drawing.Point(13, 99);
			this.radioUtf8Encoding.Name = "radioUtf8Encoding";
			this.radioUtf8Encoding.Size = new System.Drawing.Size(140, 21);
			this.radioUtf8Encoding.TabIndex = 20;
			this.radioUtf8Encoding.TabStop = true;
			this.radioUtf8Encoding.Text = "UTF-8 Encoding";
			this.toolTip1.SetToolTip(this.radioUtf8Encoding, "Check this option, if the script files should be UTF-8 encoded.");
			// 
			// cbSqlDialect
			// 
			this.cbSqlDialect.Location = new System.Drawing.Point(13, 70);
			this.cbSqlDialect.Name = "cbSqlDialect";
			this.cbSqlDialect.Size = new System.Drawing.Size(220, 21);
			this.cbSqlDialect.TabIndex = 18;
			this.toolTip1.SetToolTip(this.cbSqlDialect, "Choose an available NDO provider.");
			this.cbSqlDialect.SelectedIndexChanged += new System.EventHandler(this.cbSqlDialect_SelectedIndexChanged);
			// 
			// chkGenerateSQLScript
			// 
			this.chkGenerateSQLScript.Location = new System.Drawing.Point(13, 21);
			this.chkGenerateSQLScript.Name = "chkGenerateSQLScript";
			this.chkGenerateSQLScript.Size = new System.Drawing.Size(187, 21);
			this.chkGenerateSQLScript.TabIndex = 13;
			this.chkGenerateSQLScript.Text = "Generate SQL Script";
			this.toolTip1.SetToolTip(this.chkGenerateSQLScript, "If checked, NDO will create a script with DDL code, which can be used to construc" +
        "t a database structure.");
			this.chkGenerateSQLScript.CheckedChanged += new System.EventHandler(this.chkGenerateSQLScript_CheckedChanged);
			// 
			// chkUseTimeStamps
			// 
			this.chkUseTimeStamps.Location = new System.Drawing.Point(21, 153);
			this.chkUseTimeStamps.Name = "chkUseTimeStamps";
			this.chkUseTimeStamps.Size = new System.Drawing.Size(302, 20);
			this.chkUseTimeStamps.TabIndex = 42;
			this.chkUseTimeStamps.Text = "Generate Time Stamp Columns for each class";
			this.toolTip1.SetToolTip(this.chkUseTimeStamps, "Check this option, if all tables should be protected by collistion detection.");
			// 
			// chkChangeEvents
			// 
			this.chkChangeEvents.Location = new System.Drawing.Point(21, 179);
			this.chkChangeEvents.Name = "chkChangeEvents";
			this.chkChangeEvents.Size = new System.Drawing.Size(302, 21);
			this.chkChangeEvents.TabIndex = 41;
			this.chkChangeEvents.Text = "Generate change events with Add Accessor";
			this.toolTip1.SetToolTip(this.chkChangeEvents, "Check this option, if you intend to bind the UI directly to the accessor properti" +
        "es of your persistent classes.");
			// 
			// btnConnString
			// 
			this.btnConnString.Location = new System.Drawing.Point(538, 313);
			this.btnConnString.Name = "btnConnString";
			this.btnConnString.Size = new System.Drawing.Size(42, 21);
			this.btnConnString.TabIndex = 38;
			this.btnConnString.Text = "...";
			this.toolTip1.SetToolTip(this.btnConnString, "Enter existing database connection");
			this.btnConnString.Click += new System.EventHandler(this.btnConnString_Click);
			// 
			// btnPresetLibrary
			// 
			this.btnPresetLibrary.Location = new System.Drawing.Point(21, 385);
			this.btnPresetLibrary.Name = "btnPresetLibrary";
			this.btnPresetLibrary.Size = new System.Drawing.Size(100, 39);
			this.btnPresetLibrary.TabIndex = 36;
			this.btnPresetLibrary.Text = "Preset for Library";
			this.toolTip1.SetToolTip(this.btnPresetLibrary, "Selects all settings necessary for projects containing persistent types.");
			this.btnPresetLibrary.Click += new System.EventHandler(this.btnPresetLibrary_Click);
			// 
			// chkMappingNew
			// 
			this.chkMappingNew.Location = new System.Drawing.Point(21, 99);
			this.chkMappingNew.Name = "chkMappingNew";
			this.chkMappingNew.Size = new System.Drawing.Size(272, 21);
			this.chkMappingNew.TabIndex = 34;
			this.chkMappingNew.Text = "Always Generate a new mapping File";
			this.toolTip1.SetToolTip(this.chkMappingNew, "Choose this options, if NDO should discard and rebuild all mapping information.");
			// 
			// chkActivateEnhancer
			// 
			this.chkActivateEnhancer.Location = new System.Drawing.Point(21, 46);
			this.chkActivateEnhancer.Name = "chkActivateEnhancer";
			this.chkActivateEnhancer.Size = new System.Drawing.Size(188, 20);
			this.chkActivateEnhancer.TabIndex = 33;
			this.chkActivateEnhancer.Text = "Activate enhancer";
			this.toolTip1.SetToolTip(this.chkActivateEnhancer, "Choose this option, if your project contains persistent types.");
			// 
			// chkActivateAddIn
			// 
			this.chkActivateAddIn.Location = new System.Drawing.Point(21, 19);
			this.chkActivateAddIn.Name = "chkActivateAddIn";
			this.chkActivateAddIn.Size = new System.Drawing.Size(188, 21);
			this.chkActivateAddIn.TabIndex = 32;
			this.chkActivateAddIn.Text = "Activate NDO AddIn";
			this.toolTip1.SetToolTip(this.chkActivateAddIn, "Choose this options, if your project contains or references persistent types.");
			this.chkActivateAddIn.Click += new System.EventHandler(this.chkActivateAddIn_CheckedChanged);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageGeneral);
			this.tabControl.Controls.Add(this.tabPageAssemblies);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(661, 374);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Controls.Add(this.txtTargetMappingFileName);
			this.tabPageGeneral.Controls.Add(this.label1);
			this.tabPageGeneral.Controls.Add(this.chkVerboseMode);
			this.tabPageGeneral.Controls.Add(this.txtSchemaVersion);
			this.tabPageGeneral.Controls.Add(this.label5);
			this.tabPageGeneral.Controls.Add(this.btnNewDatabase);
			this.tabPageGeneral.Controls.Add(this.txtDbOwner);
			this.tabPageGeneral.Controls.Add(this.txtConnectionString);
			this.tabPageGeneral.Controls.Add(this.label3);
			this.tabPageGeneral.Controls.Add(this.groupBox1);
			this.tabPageGeneral.Controls.Add(this.chkUseTimeStamps);
			this.tabPageGeneral.Controls.Add(this.chkChangeEvents);
			this.tabPageGeneral.Controls.Add(this.label2);
			this.tabPageGeneral.Controls.Add(this.btnConnString);
			this.tabPageGeneral.Controls.Add(this.chkMappingNew);
			this.tabPageGeneral.Controls.Add(this.chkActivateEnhancer);
			this.tabPageGeneral.Controls.Add(this.chkActivateAddIn);
			this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageGeneral.Size = new System.Drawing.Size(653, 348);
			this.tabPageGeneral.TabIndex = 0;
			this.tabPageGeneral.Text = "General";
			this.tabPageGeneral.UseVisualStyleBackColor = true;
			// 
			// txtTargetMappingFileName
			// 
			this.txtTargetMappingFileName.Location = new System.Drawing.Point(145, 245);
			this.txtTargetMappingFileName.Name = "txtTargetMappingFileName";
			this.txtTargetMappingFileName.Size = new System.Drawing.Size(160, 20);
			this.txtTargetMappingFileName.TabIndex = 53;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 246);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 21);
			this.label1.TabIndex = 52;
			this.label1.Text = "Mapping File Name";
			// 
			// txtSchemaVersion
			// 
			this.txtSchemaVersion.Location = new System.Drawing.Point(145, 219);
			this.txtSchemaVersion.Name = "txtSchemaVersion";
			this.txtSchemaVersion.Size = new System.Drawing.Size(160, 20);
			this.txtSchemaVersion.TabIndex = 50;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(18, 220);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(121, 21);
			this.label5.TabIndex = 49;
			this.label5.Text = "Schema Version";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 128);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(127, 21);
			this.label3.TabIndex = 44;
			this.label3.Text = "Owner / Schema Name";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnInstallProvider);
			this.groupBox1.Controls.Add(this.chkDropExistingElements);
			this.groupBox1.Controls.Add(this.chkGenerateFkConstraints);
			this.groupBox1.Controls.Add(this.chkIncludeTypecodes);
			this.groupBox1.Controls.Add(this.radioDefaultEncoding);
			this.groupBox1.Controls.Add(this.radioUtf8Encoding);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.cbSqlDialect);
			this.groupBox1.Controls.Add(this.chkGenerateSQLScript);
			this.groupBox1.Location = new System.Drawing.Point(343, 21);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(265, 244);
			this.groupBox1.TabIndex = 43;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " SQL ";
			// 
			// btnInstallProvider
			// 
			this.btnInstallProvider.Location = new System.Drawing.Point(142, 102);
			this.btnInstallProvider.Name = "btnInstallProvider";
			this.btnInstallProvider.Size = new System.Drawing.Size(91, 22);
			this.btnInstallProvider.TabIndex = 25;
			this.btnInstallProvider.Text = "Install Provider";
			this.btnInstallProvider.UseVisualStyleBackColor = true;
			this.btnInstallProvider.Click += new System.EventHandler(this.btnInstallProvider_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(13, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(200, 17);
			this.label4.TabIndex = 19;
			this.label4.Text = "SQL Dialect";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(18, 292);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(360, 21);
			this.label2.TabIndex = 40;
			this.label2.Text = "Default Connection String";
			// 
			// tabPageAssemblies
			// 
			this.tabPageAssemblies.Controls.Add(this.label6);
			this.tabPageAssemblies.Controls.Add(this.chlbAssemblies);
			this.tabPageAssemblies.Location = new System.Drawing.Point(4, 22);
			this.tabPageAssemblies.Name = "tabPageAssemblies";
			this.tabPageAssemblies.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAssemblies.Size = new System.Drawing.Size(653, 348);
			this.tabPageAssemblies.TabIndex = 1;
			this.tabPageAssemblies.Text = "Assemblies";
			this.tabPageAssemblies.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(10, 12);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(328, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Choose Assemblies which should be analyzed by the NDOEnhancer";
			// 
			// chlbAssemblies
			// 
			this.chlbAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chlbAssemblies.FormattingEnabled = true;
			this.chlbAssemblies.Location = new System.Drawing.Point(10, 39);
			this.chlbAssemblies.Name = "chlbAssemblies";
			this.chlbAssemblies.Size = new System.Drawing.Size(631, 289);
			this.chlbAssemblies.TabIndex = 0;
			this.chlbAssemblies.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlbAssemblies_ItemCheck);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(363, 385);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 39);
			this.btnOK.TabIndex = 37;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(477, 385);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 39);
			this.btnCancel.TabIndex = 35;
			this.btnCancel.Text = "Cancel";
			// 
			// ConfigurationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(661, 439);
			this.Controls.Add(this.btnSaveAs);
			this.Controls.Add(this.btnPresetApp);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnPresetLibrary);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConfigurationDialog";
			this.Text = "NDO Configuration";
			this.Load += new System.EventHandler(this.ConfigurationDialog_Load);
			this.tabControl.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageGeneral.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.tabPageAssemblies.ResumeLayout(false);
			this.tabPageAssemblies.PerformLayout();
			this.ResumeLayout(false);

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

                foreach (string s in NdoUIProviderFactory.Instance.Keys)
                {
                    this.cbSqlDialect.Items.Add(s);
                    if (options.SQLScriptLanguage == s)
                        currentDialectIndex = i;
                    i++;
                }
                if (currentDialectIndex > -1)
                    cbSqlDialect.SelectedIndex = currentDialectIndex;

                // Must be initialized after changing the cbSqlDialect index
                txtConnectionString.Text = options.DefaultConnection;
				txtTargetMappingFileName.Text = options.TargetMappingFileName;
                txtDbOwner.Text = options.DatabaseOwner;
                chkActivateAddIn_CheckedChanged(null, EventArgs.Empty);
                chkGenerateSQLScript_CheckedChanged(null, EventArgs.Empty);
                if (options.Utf8Encoding)
                    radioUtf8Encoding.Checked = true;
                else
                    radioDefaultEncoding.Checked = true;
                if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
                    this.chkActivateEnhancer.Enabled = false;

				CheckEnabledStateForLoadProviderButton();
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

                string connType = options.SQLScriptLanguage;
                string connName = options.DefaultConnection;
				WriteBack(options);
                if (options.SQLScriptLanguage != connType || options.DefaultConnection != connName)
                {
                    string mappingFileName = this.project.MappingFilePath();
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
                                if (mapping.Connections.Count() == 1)
                                {
                                    conn = (Connection)mapping.Connections.FirstOrDefault();
                                }
                                else
                                {
                                    conn = mapping.NewConnection(String.Empty, String.Empty);
                                    MessageBox.Show("Added a connection with the ID " + conn.ID, "NDO Configuration");
                                }
                                conn.Type = options.SQLScriptLanguage;
                                conn.Name = options.DefaultConnection;
                                mapping.Save();
                            }
                        }
                    }
                }

				if ( options.EnableAddIn )
				{
					if ( !options.UseMsBuild )
					{
						options.UseMsBuild = true;
					}
					GeneratePackageReference();
				}

				ThreadHelper.JoinableTaskFactory.Run( async () => await options.SaveAsync( this.projectDescription ) );
				
			}
			catch (Exception ex)
			{
#if DEBUG
				MessageBox.Show( "The following error occured while saving your options: " + ex.ToString(), "NDO Add-in" );
#else
				MessageBox.Show("The following error occured while saving your options: " + ex.Message, "NDO Add-in");
#endif
			}
		}


		void GeneratePackageReference()
		{
			try
			{
				IComponentModel componentModel;
				var installerService = GetInstallerService( out componentModel );

				if (!installerService.IsPackageInstalled( this.project, "ndo.dll" ))
				{
					var installer = componentModel.GetService<IVsPackageInstaller>();
					installer.InstallPackage( null, this.project, "ndo.dll", (string)null, false );
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show( "Error while installing the ndo.dll package: " + ex.ToString() );
			}
		}

		private static IVsPackageInstallerServices GetInstallerService( out IComponentModel componentModel )
		{
			componentModel = (IComponentModel)Package.GetGlobalService( typeof( SComponentModel ) );
			var installerService = componentModel.GetService<IVsPackageInstallerServices>();
			return installerService;
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
                var provider = NdoUIProviderFactory.Instance[connType];
                if (provider == null)
                {
                    MessageBox.Show("Can't find a NDO UI provider for the sql dialect " + connType);
                    return;
                }

                string temp = this.txtConnectionString.Text;
                NdoDialogResult r = provider.ShowConnectionDialog(ref temp);
                if (r == NdoDialogResult.Cancel)
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
			options.TargetMappingFileName = txtTargetMappingFileName.Text;
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
			CheckEnabledStateForLoadProviderButton();
		}

		private void btnNewDatabase_Click(object sender, System.EventArgs e)
		{
			string connType = cbSqlDialect.Text;
			if (connType == string.Empty)
				connType = "SqlServer";
			IDbUISupport provider = NdoUIProviderFactory.Instance[connType];
			if (provider == null)
			{
				MessageBox.Show("Can't find a NDO UI provider for the sql dialect " + connType);
				return;
			}
			object necessaryData = null;
			try
			{
                if (provider.ShowCreateDbDialog(ref necessaryData) == NdoDialogResult.Cancel)
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

		private void chlbAssemblies_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			this.references[e.Index].CheckState = e.NewValue;
		}

		private void btnInstallProvider_Click( object sender, EventArgs e )
		{
			try
			{
				string providerName = this.cbSqlDialect.Text;

				if (string.IsNullOrEmpty( providerName ))
				{
					MessageBox.Show( "Please choose a provider" );
					return;
				}

				IComponentModel componentModel;
				var installerService = GetInstallerService( out componentModel );

				string packageName = $"ndo.{providerName.ToLower()}";
				if (!installerService.IsPackageInstalled( this.project, packageName ))
				{
					var installer = componentModel.GetService<IVsPackageInstaller>();
					installer.InstallPackage( null, this.project, packageName, (string)null, false );
				}

				this.btnInstallProvider.Enabled = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show( "Error while installing the ndo.dll package: " + ex.ToString() );
			}
		}
	}
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
