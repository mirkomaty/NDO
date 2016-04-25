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
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Drawing;

namespace WizardBase
{
	/// <summary>
	/// Zusammenfassung für GenericController.
	/// </summary>
#if DEBUG
	public class GenericController : IWizardController, IComparer
#else
	internal class GenericController : IWizardController, IComparer
#endif
	{
		ArrayList viewTypes = new ArrayList();
		int state;
		int lastState;
		IModel model;
		WizardFrame frame;
		DialogResult result;
		Size frameSize;
		string title;


		protected IModel Model
		{
			get { return model; }
		}
		protected int State
		{
			get { return state; }
		}


		public GenericController(string viewTypeName, Assembly assy, string title)
		{
			this.title = title;
			Type[] types = assy.GetTypes();
			foreach(Type t in types)
			{
				if (t.IsClass && t.IsSubclassOf(typeof(ViewBase)) & t.Name.StartsWith(viewTypeName))
					viewTypes.Add(t);
			}
			viewTypes.Sort(this);
			lastState = viewTypes.Count - 1;

			// Set the Frame size. Start with a reasonable view size.
			int maxWidth = 384;
			int maxHeight = 60;
			foreach ( Type t in this.viewTypes )
			{
				ViewBase vb = (ViewBase) Activator.CreateInstance(t);
				maxWidth = Math.Max(vb.Width, maxWidth);
				maxHeight = Math.Max(maxHeight, vb.Height);
				vb = null;  // GC
			}
			this.FrameSize = new Size(maxWidth + 40, maxHeight + 221);
		}

		public Size FrameSize
		{
			get { return frameSize; }
			set { frameSize = value; }
		}

		protected int LastState
		{
			get { return lastState; }
			set { lastState = value; }
		}


		#region IWizardController Member

		public virtual System.Windows.Forms.DialogResult Run(IModel model)
		{
			this.model = model;
			frame = new WizardFrame(this);
			if (frameSize.Height > 0 && frameSize.Width > 0)
				frame.Size = frameSize;
			int step = state + 1;
			frame.Text = title + " Step " + step.ToString();
			frame.ShowDialog();
			return result;
		}

		protected virtual void SwitchToState(int newState, out ViewBase newView, out WizardState wizardState)
		{
//			int oldState = this.state;
			this.state = newState;
//System.Diagnostics.Debug.WriteLine(oldState.ToString() + "->" + state + "/" + lastState);
			if (frame != null)
			{
				int step = state + 1;
				frame.Text = title + " Step " + step.ToString();
			}

			newView = (ViewBase) Activator.CreateInstance((Type) viewTypes[state], new object[]{model});
			if (state == 0)
			{
				wizardState = WizardState.First;
				if (lastState == 0)
				{
					wizardState &= (~WizardState.Next);
					wizardState |= (WizardState.Finish);
				}
			}
			else if (state == lastState)
			{
				wizardState = WizardState.Last;
				if (lastState == 0)
				{
					wizardState &= (~WizardState.Back);
				}
			}
			else
				wizardState = WizardState.Middle;
		
		}


		public void OnInit(out ViewBase newView, out WizardBase.WizardState wizardState)
		{
			SwitchToState(0, out newView, out wizardState);
		}

		public virtual void Next(out ViewBase newView, out WizardBase.WizardState wizardState)
		{
			if (state < lastState)
			{
				SwitchToState(state + 1, out newView, out wizardState);
			}
			else
				throw new Exception("GenericController.Next: Step over last state.");
		}

		public virtual void Back(out ViewBase newView, out WizardBase.WizardState wizardState)
		{
			if (state > 0)
			{
				SwitchToState(state - 1, out newView, out wizardState);
			}
			else
				throw new Exception("GenericController.Next: Step behind first state.");
		}

		public virtual void Cancel()
		{
			frame.Close();
			result = DialogResult.Cancel;
		}

		public virtual void Finish(DialogResult dr)
		{
			frame.Close();
			this.result = dr;
		}

		#endregion

		#region IComparer Member

		public int Compare(object x, object y)
		{
			if (x is Type && y is Type)
				return string.Compare(((Type)x).FullName, ((Type)y).FullName);
			throw new ArgumentException("WizardBase.GenericController: Should only compare types.");
		}

		#endregion
	}
}
