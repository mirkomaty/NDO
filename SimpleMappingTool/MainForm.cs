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
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NDO.Mapping;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für MainForm.
	/// </summary>
	internal class MainForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuOpen;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.MenuItem menuSaveAs;

		private bool saveOnClose = false;

        public MainForm(string[] args)
        {
            try
            {
                InitializeComponent();
                ScanArgs(args);
                Application.Idle += new EventHandler(OnIdle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Mapping tool error");
            }
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
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.allObjects = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuOpen = new System.Windows.Forms.MenuItem();
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuSaveAs = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // allObjects
            // 
            this.allObjects.Dock = System.Windows.Forms.DockStyle.Left;
            this.allObjects.ImageIndex = 0;
            this.allObjects.ImageList = this.imageList1;
            this.allObjects.Location = new System.Drawing.Point(0, 0);
            this.allObjects.Name = "allObjects";
            this.allObjects.SelectedImageIndex = 0;
            this.allObjects.Size = new System.Drawing.Size(387, 432);
            this.allObjects.TabIndex = 0;
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
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(387, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 432);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Location = new System.Drawing.Point(389, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(395, 432);
            this.propertyGrid1.TabIndex = 2;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuOpen,
            this.menuSave,
            this.menuSaveAs});
            this.menuItem1.Text = "&File";
            // 
            // menuOpen
            // 
            this.menuOpen.Index = 0;
            this.menuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuOpen.Text = "&Open...";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Index = 1;
            this.menuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Index = 2;
            this.menuSaveAs.Text = "Save &as...";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(784, 432);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.allObjects);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "NDO Mapping Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			Application.Run(new MainForm(args));
		}
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TreeView allObjects;

		private System.Windows.Forms.ImageList imageList1;
		NDOMapping mapping = null;			



		private void LoadMapping(string fileName)
		{
			allObjects.Nodes.Clear();
			try
			{
				mapping = new NDOMapping(fileName);
                PrepareRelations();
            }
			catch (Exception ex)
			{
				mapping = null;
				this.Text = string.Empty;
#if DEBUG
				MessageBox.Show(ex.ToString(), "Error");
#else
				MessageBox.Show(ex.Message, "Error");
#endif
                return;
			}
			this.Text = fileName;
			FillNodes();
		}


		private void FillNodes()
		{
            NDOMappingNode mn = new NDOMappingNode(this.mapping);
            allObjects.Nodes.Add(mn);
		}

		private void ScanArgs(string[] args)
		{
            Regex regexm = new Regex(@"-m:(.*)");
            foreach (string arg in args)
            {
                Match matchm = regexm.Match(arg);
                if (matchm.Success)
                {
                    LoadMapping(matchm.Groups[1].Value);
                    saveOnClose = true;
                }
            }
		}


		private void MainForm_Load(object sender, System.EventArgs e)
		{
			//ScanArgs(new string[] {@"-m:C:\Projekte\Persistenz\v4\TestEnhancerVersion4\PureBusinessClasses\NDOMapping.xml"});
		}

		private void allObjects_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			NDOTreeNode tn = e.Node as NDOTreeNode;
            if (tn != null)
                this.propertyGrid1.SelectedObject = tn.Object;
            else
                this.propertyGrid1.SelectedObject = null;
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (saveOnClose && mapping != null)
				mapping.Save();
			else if (mapping != null && mapping.HasChanges)
			{
				if (MessageBox.Show("Save changes?", "NDO Mapping Tool", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					mapping.Save();
				}				
			}
		}

		private void menuOpen_Click(object sender, System.EventArgs e)
		{
			if (mapping != null && mapping.HasChanges)
			{
				if (MessageBox.Show("Save changes?", "Mapping Tool", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					mapping.Save();
					mapping = null;
				}
			}

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.DefaultExt = "xml";
			ofd.Multiselect = false;
			ofd.Filter = "Mapping Files (*.xml)|*.xml";
			ofd.FileName = "NDOMapping.xml";
			if (ofd.ShowDialog() == DialogResult.Cancel)
				return;
			LoadMapping(ofd.FileName);
		}


        bool PrepareRelations()
        {
            bool result = false;
            foreach (Class cl in mapping.Classes)
            {
                foreach (Relation r in cl.Relations)
                {
                    // This computes the foreign relation, which is
                    // needed to avoid a null pointer exception
                    // while binding to the property grid.
                    result = result || r.Bidirectional;
                }
            }
            return result;  // Nobody reads this value, but we make shure, that the code won't get removed by the optimizer.
        }


		private void menuSave_Click(object sender, System.EventArgs e)
		{
			mapping.Save();
		}

		private void menuSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			if (mapping != null && mapping.FileName != string.Empty)
				sfd.InitialDirectory = Path.GetDirectoryName(mapping.FileName);
			sfd.CheckFileExists = false;
			sfd.DefaultExt = "xml";
			sfd.Filter = "Mapping Files (*.xml)|*.xml";
			sfd.FileName = "NDOMapping.xml";
			if (sfd.ShowDialog(this) != DialogResult.OK)
				return;
            mapping.SaveAs(sfd.FileName);
            this.Text = sfd.FileName;
		}

		private void OnIdle(object sender, EventArgs e)
		{
			menuSave.Enabled = mapping != null && mapping.HasChanges;
			menuOpen.Enabled = !saveOnClose;
		}

#if DEBUG
		private void MainForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			if ((int) e.KeyChar == 27)
				MessageBox.Show("Escape");
		}
#endif

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
					this.propertyGrid1.SelectedObject = tn.Object;
				}
			}
		}

	}
}
