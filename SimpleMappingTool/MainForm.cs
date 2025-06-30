//
// Copyright (c) 2002-2025 Mirko Matytschak 
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


using NDO.Mapping;
using System.ComponentModel;
using System.Text.RegularExpressions;
using WinForms.FontSize;

namespace SimpleMappingTool
{
    /// <summary>
    /// Zusammenfassung für MainForm.
    /// </summary>
    internal class MainForm : Form
    {
        private IContainer components;
        private MenuStrip mainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem menuOpen;
        private ToolStripMenuItem menuSave;
        private ToolStripMenuItem menuSaveAs;
        private bool saveOnClose = false;
        private Splitter splitter1;
        private TreeView allObjects;
        private ImageList imageList1;
        private ComponentResourceManager resources;
		private Panel panel1;
		private PropertyGrid propertyGrid1;
		NDOMapping? mapping = null;

#pragma warning disable 8618
        public MainForm( string[] args )
        {
            try
            {
				this.resources = new ComponentResourceManager( typeof( MainForm ) );
				InitializeComponent();
				// Calculate the new font size after InitializeComponent
				var newFontSize = FontCalculator.Calculate(Screen.FromControl(this), Font.Size);
                if (newFontSize > Font.Size)
                    Font = new Font( Font.FontFamily, newFontSize, FontStyle.Regular, GraphicsUnit.Point, 0 );
				this.imageList1!.ImageStream = (ImageListStreamer) resources.GetObject( "imageList1.ImageStream" )!;
				imageList1.Images.SetKeyName( 0, "" );
				imageList1.Images.SetKeyName( 1, "" );
				imageList1.Images.SetKeyName( 2, "" );
				imageList1.Images.SetKeyName( 3, "" );
				imageList1.Images.SetKeyName( 4, "" );
				imageList1.Images.SetKeyName( 5, "" );
				imageList1.Images.SetKeyName( 6, "" );
				imageList1.Images.SetKeyName( 7, "" );
				imageList1.Images.SetKeyName( 8, "" );
				imageList1.Images.SetKeyName( 9, "" );
				imageList1.Images.SetKeyName( 10, "" );
				imageList1.Images.SetKeyName( 11, "" );
				imageList1.Images.SetKeyName( 12, "" );
				imageList1.Images.SetKeyName( 13, "" );
				Icon = (Icon) resources.GetObject( "$this.Icon" )!;
				ScanArgs( args );
                Application.Idle += new EventHandler( OnIdle );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.ToString(), "Mapping tool error" );
            }
        }
#pragma warning restore 8618

        /// <summary>
        /// Die verwendeten Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if (disposing)
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
			components = new Container();
			allObjects = new TreeView();
			imageList1 = new ImageList( components );
			splitter1 = new Splitter();
			mainMenu = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			menuOpen = new ToolStripMenuItem();
			menuSave = new ToolStripMenuItem();
			menuSaveAs = new ToolStripMenuItem();
			panel1 = new Panel();
			propertyGrid1 = new PropertyGrid();
			mainMenu.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// allObjects
			// 
			allObjects.Dock = DockStyle.Left;
			allObjects.ImageIndex = 0;
			allObjects.ImageList = imageList1;
			allObjects.Location = new Point( 0, 24 );
			allObjects.Name = "allObjects";
			allObjects.SelectedImageIndex = 0;
			allObjects.Size = new Size( 464, 408 );
			allObjects.TabIndex = 0;
			allObjects.AfterSelect +=  allObjects_AfterSelect ;
			allObjects.MouseUp +=  allObjects_MouseUp ;
			// 
			// imageList1
			// 
			imageList1.ColorDepth = ColorDepth.Depth8Bit;
			imageList1.ImageSize = new Size( 16, 16 );
			imageList1.TransparentColor = Color.Transparent;
			// 
			// splitter1
			// 
			splitter1.Location = new Point( 464, 24 );
			splitter1.Name = "splitter1";
			splitter1.Size = new Size( 3, 408 );
			splitter1.TabIndex = 1;
			splitter1.TabStop = false;
			// 
			// mainMenu
			// 
			mainMenu.Items.AddRange( new ToolStripItem[] { fileToolStripMenuItem } );
			mainMenu.Location = new Point( 0, 0 );
			mainMenu.Name = "mainMenu";
			mainMenu.Size = new Size( 784, 24 );
			mainMenu.TabIndex = 0;
			mainMenu.Text = "mainMenu";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange( new ToolStripItem[] { menuOpen, menuSave, menuSaveAs } );
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size( 37, 20 );
			fileToolStripMenuItem.Text = "&File";
			// 
			// menuOpen
			// 
			menuOpen.Name = "menuOpen";
			menuOpen.Size = new Size( 121, 22 );
			menuOpen.Text = "&Open";
			menuOpen.Click +=  menuOpen_Click ;
			// 
			// menuSave
			// 
			menuSave.Name = "menuSave";
			menuSave.Size = new Size( 121, 22 );
			menuSave.Text = "&Save";
			menuSave.Click +=  menuSave_Click ;
			// 
			// menuSaveAs
			// 
			menuSaveAs.Name = "menuSaveAs";
			menuSaveAs.Size = new Size( 121, 22 );
			menuSaveAs.Text = "Save &as...";
			menuSaveAs.Click +=  menuSaveAs_Click ;
			// 
			// panel1
			// 
			panel1.Controls.Add( propertyGrid1 );
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point( 467, 24 );
			panel1.Name = "panel1";
			panel1.Padding = new Padding( 0, 0, 3, 3 );
			panel1.Size = new Size( 317, 408 );
			panel1.TabIndex = 2;
			// 
			// propertyGrid1
			// 
			propertyGrid1.Dock = DockStyle.Fill;
			propertyGrid1.LineColor = SystemColors.ScrollBar;
			propertyGrid1.Location = new Point( 0, 0 );
			propertyGrid1.Name = "propertyGrid1";
			propertyGrid1.Size = new Size( 314, 405 );
			propertyGrid1.TabIndex = 3;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF( 7F, 15F );
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size( 784, 432 );
			Controls.Add( panel1 );
			Controls.Add( splitter1 );
			Controls.Add( allObjects );
			Controls.Add( mainMenu );
			MainMenuStrip = mainMenu;
			Name = "MainForm";
			Text = "NDO Mapping Tool";
			Closing +=  MainForm_Closing ;
			Load +=  MainForm_Load ;
			mainMenu.ResumeLayout( false );
			mainMenu.PerformLayout();
			panel1.ResumeLayout( false );
			ResumeLayout( false );
			PerformLayout();
		}
		#endregion

		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
        static void Main( string[] args )
        {
            Application.Run( new MainForm( args ) );
        }

        private void LoadMapping( string fileName )
        {
            allObjects.Nodes.Clear();
            try
            {
                mapping = new NDOMapping( fileName, null );
                PrepareRelations();
            }
            catch (Exception ex)
            {
                mapping = null;
                this.Text = string.Empty;
#if DEBUG
                MessageBox.Show( ex.ToString(), "Error" );
#else
				MessageBox.Show(ex.Message, "Error");
#endif
                return;
            }
            this.Text = fileName;
            if (mapping != null)
                FillNodes();
        }


        private void FillNodes()
        {
            NDOMappingNode mn = new NDOMappingNode(this.mapping!);
            allObjects.Nodes.Add( mn );
        }

        private void ScanArgs( string[] args )
        {
            Regex regexm = new Regex(@"-m:(.*)");
            foreach (string arg in args)
            {
                Match matchm = regexm.Match(arg);
                if (matchm.Success)
                {
                    LoadMapping( matchm.Groups[1].Value );
                    saveOnClose = true;
                }
            }
        }


        private void MainForm_Load( object? sender, System.EventArgs e )
        {
        }

        private void allObjects_AfterSelect( object? sender, TreeViewEventArgs e )
        {
            var tn = e.Node as NDOTreeNode;
            if (tn != null)
                this.propertyGrid1.SelectedObject = tn.Object;
            else
                this.propertyGrid1.SelectedObject = null;
        }

        private void MainForm_Closing( object? sender, System.ComponentModel.CancelEventArgs e )
        {
            if (saveOnClose && mapping != null)
                mapping.Save();
            else if (mapping != null && mapping.HasChanges)
            {
                if (MessageBox.Show( "Save changes?", "NDO Mapping Tool", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes)
                {
                    mapping.Save();
                }
            }
        }

        private void menuOpen_Click( object? sender, System.EventArgs e )
        {
            if (mapping != null && mapping.HasChanges)
            {
                if (MessageBox.Show( "Save changes?", "Mapping Tool", MessageBoxButtons.YesNo ) == DialogResult.Yes)
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
            LoadMapping( ofd.FileName );
        }


        bool PrepareRelations()
        {
            bool result = false;
            if (mapping != null)
            {
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
            }

            return result;  // Nobody reads this value, but we make shure, that the code won't get removed by the optimizer.
        }


        private void menuSave_Click( object? sender, System.EventArgs e )
        {
            if (mapping != null)
                mapping.Save();
        }

        private void menuSaveAs_Click( object? sender, System.EventArgs e )
        {
            if (mapping != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (mapping.FileName != string.Empty)
                    sfd.InitialDirectory = Path.GetDirectoryName( mapping.FileName );
                sfd.CheckFileExists = false;
                sfd.DefaultExt = "xml";
                sfd.Filter = "Mapping Files (*.xml)|*.xml";
                sfd.FileName = "NDOMapping.xml";
                if (sfd.ShowDialog( this ) != DialogResult.OK)
                    return;
                mapping.SaveAs( sfd.FileName );
                this.Text = sfd.FileName;
            }
        }

        private void OnIdle( object? sender, EventArgs e )
        {
            menuSave.Enabled = mapping != null && mapping.HasChanges;
            menuOpen.Enabled = !saveOnClose;
        }

#if DEBUG
        private void MainForm_KeyPress( object? sender, KeyPressEventArgs e )
        {

            if ((int) e.KeyChar == 27)
                MessageBox.Show( "Escape" );
        }
#endif

        private void allObjects_MouseUp( object? sender, MouseEventArgs e )
        {

            if (e.Button == MouseButtons.Right)
            {
                var tn = this.allObjects.GetNodeAt(e.X, e.Y) as NDOTreeNode;
                if (tn != null)
                {
                    allObjects.SelectedNode = tn;
                    var menu = tn.GetContextMenu();
                    if (menu.Items.Count > 0)
                        menu.Show(allObjects, new Point(e.X, e.Y));
                    this.propertyGrid1.SelectedObject = tn.Object;
                }
            }
        }
    }
}
