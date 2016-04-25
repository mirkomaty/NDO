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


namespace WizardBase
{
	/// <summary>
	/// Zusammenfassung für ViewBase.
	/// </summary>
#if DEBUG
	public class ViewBase : System.Windows.Forms.UserControl
#else
	internal class ViewBase : System.Windows.Forms.UserControl
#endif
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer skipTimer;
		IModel model;
		public ViewBase()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();

			// TODO: Initialisierungen nach dem Aufruf von InitializeComponent hinzufügen

		}

		public ViewBase(IModel model)
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();

			this.model = model;

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

		public WizardFrame Frame
		{
			get 
			{
				if (this.Parent == null)
					return null;
				return ((WizardFrame)this.Parent.Parent); 
			} 
		}

		public virtual void OnLeaveView()
		{
		}

		#region Vom Komponenten-Designer generierter Code
		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.skipTimer = new System.Windows.Forms.Timer(this.components);
			// 
			// skipTimer
			// 
			this.skipTimer.Interval = 1;
			this.skipTimer.Tick += new System.EventHandler(this.skipTimer_Tick);
			// 
			// ViewBase
			// 
			this.Name = "ViewBase";

		}
		#endregion

		private void skipTimer_Tick(object sender, System.EventArgs e)
		{
			skipTimer.Enabled = false;
			Frame.SkipToNextView();
		}

		protected void SkipToNextView()
		{
			this.skipTimer.Enabled = true;
		}
	}
}
