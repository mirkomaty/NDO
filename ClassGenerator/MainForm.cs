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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ClassGenerator.AssemblyWizard;
using ClassGenerator.Serialisation;
using WizardBase;
using NDO;
using NDOInterfaces;
using System.Xml;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuGenerateAssembly;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TreeView allObjects;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.ComponentModel.IContainer components;


		AssemblyWizModel model;
		private System.Windows.Forms.MenuItem menuNew;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.MenuItem menuSaveAs;
		private System.Windows.Forms.MenuItem menuLoad;
		string defaultDir;
		string fileName = string.Empty;

		public MainForm(string[] args)
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();
	
			ApplicationController.WizardControllerFactory = new WizardControllerFactory(this.GetType().Assembly, string.Empty);
			if (args.Length > 0)
				this.defaultDir = args[0];
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuNew = new System.Windows.Forms.MenuItem();
            this.menuGenerateAssembly = new System.Windows.Forms.MenuItem();
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuSaveAs = new System.Windows.Forms.MenuItem();
            this.menuLoad = new System.Windows.Forms.MenuItem();
            this.allObjects = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuNew,
            this.menuGenerateAssembly,
            this.menuSave,
            this.menuSaveAs,
            this.menuLoad});
            this.menuItem1.Text = "&File";
            // 
            // menuNew
            // 
            this.menuNew.Index = 0;
            this.menuNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuNew.Text = "&New Assembly...";
            this.menuNew.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuGenerateAssembly
            // 
            this.menuGenerateAssembly.Index = 1;
            this.menuGenerateAssembly.Text = "&Generate Assembly";
            this.menuGenerateAssembly.Click += new System.EventHandler(this.menuGenerateAssembly_Click);
            // 
            // menuSave
            // 
            this.menuSave.Index = 2;
            this.menuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Index = 3;
            this.menuSaveAs.Text = "Save &as...";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // menuLoad
            // 
            this.menuLoad.Index = 4;
            this.menuLoad.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.menuLoad.Text = "&Load...";
            this.menuLoad.Click += new System.EventHandler(this.menuLoad_Click);
            // 
            // allObjects
            // 
            this.allObjects.Dock = System.Windows.Forms.DockStyle.Left;
            this.allObjects.ImageIndex = 0;
            this.allObjects.ImageList = this.imageList1;
            this.allObjects.Location = new System.Drawing.Point(0, 0);
            this.allObjects.Name = "allObjects";
            this.allObjects.SelectedImageIndex = 0;
            this.allObjects.Size = new System.Drawing.Size(313, 464);
            this.allObjects.TabIndex = 0;
            this.allObjects.DoubleClick += new System.EventHandler(this.allObjects_DoubleClick);
            this.allObjects.MouseUp += new System.Windows.Forms.MouseEventHandler(this.allObjects_MouseUp);
            this.allObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.allObjects_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(313, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 464);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(316, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(516, 464);
            this.propertyGrid.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(832, 464);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.allObjects);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "NDO Class Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		public static void Run(string[] args) 
		{
			Application.Run(new MainForm(args));
		}

#if DEBUG
		private void FakeXmlAssembly()
		{
			allObjects.Nodes.Clear();
			model = new AssemblyWizModel();
			model.ConnectionType = "Access";
			model.ConnectionString = "Replace this string with your Connection String";
			model.ProjectDirectory = @"C:\MyXmlReverseClasses\NDO";
			model.ProjectName = "NDO.Mapping.Reverse";
			model.DefaultNamespace = "NDO.Mapping.Reverse";
			model.IsXmlSchema = true;
			model.XmlSchemaFile = @"C:\Projekte\NDO\NDODLL\NDOMapping.xsd";
			model.TargetLanguage = TargetLanguage.VB;
			FillNodes();
		}

		private void FakeOracleAssembly()
		{
			allObjects.Nodes.Clear();
			model = new AssemblyWizModel();
			model.ConnectionType = "Oracle";
			model.ConnectionString = "Password=manager;User ID=SYSTEM;Data Source=maty;Persist Security Info=True";
			model.ProjectDirectory = @"c:\MyNw\Scott";
			model.ProjectName = "Scott";
			model.DefaultNamespace = "Scott.Reverse";
			model.MapStringsAsGuids = false;
			model.UseClassField = true;
			model.OwnerName = "SCOTT";
			FillNodes();
			DatabaseNode dbn = (DatabaseNode) allObjects.Nodes[0];
			TableNode tn = (TableNode) dbn.FindNode("EMP");
			tn.MapClass(null, EventArgs.Empty);

			//			allObjects.Nodes[0].ForeColor = System.Drawing.Color.LightGray;
		}

		private void FakeMySqlAssembly()
		{
			allObjects.Nodes.Clear();
			model = new AssemblyWizModel();
			model.ConnectionType = "MySql";
			model.ConnectionString = "Database=NDOShop;Data Source=localhost;User Id=root;";
			model.ProjectDirectory = @"c:\MyNw\MySql";
			model.ProjectName = "NDOShop";
			model.DefaultNamespace = "NDOShop";
			model.MapStringsAsGuids = false;
			model.UseClassField = true;
			FillNodes();
			DatabaseNode dbn = (DatabaseNode) allObjects.Nodes[0];

			//			allObjects.Nodes[0].ForeColor = System.Drawing.Color.LightGray;
		}

#endif
		private void NewAssembly()
		{
//            MessageBox.Show("Note: This tool is in an unstable beta state. There are known issues with saving and restoring the .ndogen files.", "Class Generator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			allObjects.Nodes.Clear();
			model = new AssemblyWizModel();
			if (this.defaultDir != null)
				model.ProjectDirectory = this.defaultDir;
			IWizardController controller = new GenericController("AssemblyWiz", this.GetType().Assembly, "Generate Assembly Wizard");
			//controller.FrameSize = new Size(528, 380);
			DialogResult r = controller.Run(model);
			if (r == DialogResult.OK)
			{
				FillNodes();
			}
            if (r == DialogResult.Ignore)
            {
                this.menuLoad_Click(null, EventArgs.Empty);
            }
		}

		void FillNodes()
		{
			try
			{
				// Generation of the Database tree needs the AssemblyNode information, 
				// so we assign it first.
				ApplicationController.Instance.AssemblyNode = new AssemblyNode( model );
				ApplicationController.Instance.DatabaseNode = new DatabaseNode( model );
				// The database node appears first
				allObjects.Nodes.Add( ApplicationController.Instance.DatabaseNode );
				allObjects.Nodes.Add( ApplicationController.Instance.AssemblyNode );
				//if ( model.IsXmlSchema )
					ApplicationController.Instance.DatabaseNode.MapXmlSchema();
			}
			catch ( Exception ex )
			{
				MessageBox.Show(ex.ToString(), "Class Generator");
			}
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			//FakeXmlAssembly();
			NewAssembly();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.fileName = string.Empty;
			NewAssembly();
		}

		private void allObjects_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			NDOTreeNode tn = e.Node as NDOTreeNode;
			if (tn != null)
				this.propertyGrid.SelectedObject = tn.Object;
			else
				this.propertyGrid.SelectedObject = null;
		}

		private void allObjects_DoubleClick(object sender, System.EventArgs e)
		{
		}

		private void allObjects_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			
			if (e.Button == MouseButtons.Right)
			{
				NDOTreeNode tn = this.allObjects.GetNodeAt(e.X, e.Y) as NDOTreeNode;
				if (tn != null)
				{
					allObjects.SelectedNode = tn;
					ContextMenu menu = tn.GetContextMenu();
					if (menu.MenuItems.Count > 0)
						menu.Show(allObjects, new Point(e.X, e.Y));
					this.propertyGrid.SelectedObject = tn.Object;
				}
			}
		}

		private void menuGenerateAssembly_Click(object sender, System.EventArgs e)
		{
			ApplicationController.Instance.GenerateAssembly();
            MessageBox.Show("Assembly generated in folder\n" + ApplicationController.Instance.AssemblyNode.Assembly.TargetDir, "NDO Mapping Tool");
		}

		private void SaveFile()
		{
            XmlDocument doc = new XmlDocument();
            XmlNode myElement;
            doc.AppendChild(myElement = doc.CreateElement("ClassGeneratorData"));
            ApplicationController.Instance.ReCreateModel().Save(myElement);
            doc.Save(this.fileName);
//			new NodeListSerializer(fileName, allObjects.Nodes).Serialize();	
//			new ExtendedMappingFile(fileName, allObjects.Nodes).Save();
		}

		private void menuSave_Click(object sender, System.EventArgs e)
		{
			if (fileName == string.Empty)
			{
				menuSaveAs_Click(sender, e);
				return;
			}
			SaveFile();
		}

		private void menuLoad_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Class Generator Files (*.clgen)|*.clgen";
			ofd.DefaultExt = "clgen";
			DialogResult r = ofd.ShowDialog();

			if (r == DialogResult.OK)
			{
				this.fileName = ofd.FileName;
                XmlDocument doc = new XmlDocument();
                doc.Load(this.fileName);
                allObjects.Nodes.Clear();
                this.model = new AssemblyWizModel(doc.SelectSingleNode("ClassGeneratorData"));
                FillNodes();
#if MaskedOut
				this.fileName = ofd.FileName;
				NodeListSerializer nls = new NodeListSerializer(fileName, null);
				allObjects.Nodes.Clear();


				nls.Deserialize(allObjects.Nodes);
				foreach(NDOTreeNode trn in allObjects.Nodes)
				{
					DatabaseNode dn = trn as DatabaseNode;
					if (dn != null)
						ApplicationController.Instance.DatabaseNode = dn;
					AssemblyNode an = trn as AssemblyNode;
					if (an != null)
						ApplicationController.Instance.AssemblyNode = an;
				}
#endif
			}
	}

		private void menuSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.Filter = "Class Generator Files (*.clgen)|*.clgen";
			sfd.DefaultExt = ("clgen");
			DialogResult result = sfd.ShowDialog();
			if (result == DialogResult.OK)
			{
				this.fileName = sfd.FileName;
				SaveFile();
			}
			else
				return;
		}



	}
}
