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
