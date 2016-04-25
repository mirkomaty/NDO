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
using System.Diagnostics;
using WizardBase;

namespace ClassGenerator.AssemblyWizard
{
	internal class AssemblyWizController : IWizardController
	{
		private enum State
		{
			ConnectionType,
			ConnectionString,
			Namespace,
			ProjectDir
		}

		private State state;
		private IModel model;
		private DialogResult result;
		private WizardCompleteHandler completeHandler;
		private WizardFrame frame;

		public AssemblyWizController()
		{
		}
		#region IWizardController Member

		public DialogResult Run(IModel model)
		{
			this.model = model;
			frame = new WizardFrame(this);
			frame.ShowDialog();
			return result;
		}

		public void OnInit(out ViewBase newView, out WizardState wizardState)
		{
			SwitchToState(State.ConnectionType, out newView, out wizardState);
		}

		public void Next(out ViewBase newView, out WizardState wizardState)
		{
			switch (state)
			{
				case State.ConnectionType:
					SwitchToState(State.ConnectionString, out newView, out wizardState);
					break;
				case State.ConnectionString:
					SwitchToState(State.Namespace, out newView, out wizardState);
					break;
				case State.Namespace:
					SwitchToState(State.ProjectDir, out newView, out wizardState);
					break;
				default:
					throw new Exception("Wrong state");
					break;
			}
		}

		void SwitchToState(State newState, out ViewBase newView, out WizardState wizardState)
		{
			this.state = newState;
			switch (this.state)
			{
				case State.ConnectionType:
					newView = new AssemblyWiz1(model);
					wizardState = WizardState.First;
					break;
				case State.ConnectionString:
					newView = new AssemblyWiz2(model);
					wizardState = WizardState.Middle;
					break;
				case State.Namespace:
					newView = new AssemblyWiz3(model);
					wizardState = WizardState.Middle;
					break;
				case State.ProjectDir:
					newView = new AssemblyWiz2(model);
					wizardState = WizardState.Last;
					break;
				default:
					throw new Exception("Wrong state");
					break;
			}
		}

		public void Back(out ViewBase newView, out WizardState wizardState)
		{
			switch (state)
			{
				case State.ProjectDir:
					SwitchToState(State.ConnectionString, out newView, out wizardState);
					break;
				default:
					throw new Exception("Wrong state");
					break;
			}			
		}

		public void Cancel()
		{
			frame.Close();
			result = DialogResult.Cancel;
		}

		public void Finish()
		{
			frame.Close();
			result = DialogResult.OK;
		}

		#endregion


	}
}
