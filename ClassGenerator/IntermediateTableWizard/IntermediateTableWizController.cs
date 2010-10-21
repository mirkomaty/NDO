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
using WizardBase;

namespace ClassGenerator.IntermediateTableWizard
{
	/// <summary>
	/// Zusammenfassung für IntermediateWizController.
	/// </summary>
#if DEBUG
	public class IntermediateTableWizController : GenericController
#else
	internal class IntermediateTableWizController : GenericController
#endif
	{
		public IntermediateTableWizController(string viewTypeName, System.Reflection.Assembly assy, string title) : base (viewTypeName, assy, title)
		{
		}
		protected override void SwitchToState(int newState, out ViewBase newView, out WizardState wizardState)
		{
			int oldState = this.State;
			IntermediateTableWizardModel model = (IntermediateTableWizardModel) this.Model;
			RelationDirection dir = model.IntermediateTableNode.IntermediateTable[0].RelationDirection;

			if (dir == RelationDirection.DirectedFromMe)
				base.LastState = 2;
			else
				base.LastState = 3;

			// State 2 is shared, state 3 is a dummy.
			// State 2: Index = 0
			// State 3: Index = 1
			if (newState == 2) // means: State == 1
			{
				if (dir == RelationDirection.DirectedToMe)
				{
					// Skip State 2, Index 0
					model.Index = 1;
					base.LastState = 2;
				}
				else
				{
					model.Index = 0;
				}
			}

			if (newState == 1 && State == 2 && model.Index == 1 && dir != RelationDirection.DirectedToMe)
			{
				newState = 2;
				model.Index = 0;
			}

			if (newState == 3)
			{
// dir == RelationDirection.DirectedFromMe shouldn't occur				
				newState = 2;
				model.Index = 1;
				base.LastState = 2;
			}
			base.SwitchToState (newState, out newView, out wizardState);
		}
	}
}
