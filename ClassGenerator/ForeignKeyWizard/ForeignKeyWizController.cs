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

namespace ClassGenerator.ForeignKeyWizard
{
	/// <summary>
	/// Zusammenfassung für ForeignKeyWizController.
	/// </summary>
	internal class ForeignKeyWizController : GenericController
	{
		public ForeignKeyWizController(string viewTypeName, System.Reflection.Assembly assy, string title) : base (viewTypeName, assy, title)
		{
		}

		protected override void SwitchToState(int newState, out ViewBase newView, out WizardState wizardState)
		{
			int oldState = this.State;
			ForeignKeyWizModel model = (ForeignKeyWizModel) this.Model;
			FkRelation fkRelation = (FkRelation) model.RelationNode.Relation;
			if (fkRelation.RelationDirection == RelationDirection.DirectedToMe && newState == 1)
			{
				// skip state 1
				if (State == 0)
					newState = 2;
				if (State == 2)
					newState = 0;
			}
			if (fkRelation.RelationDirection == RelationDirection.DirectedFromMe && newState == 2)
			{
				// skip state 2
				if (State == 1)
					newState = 3;
				if (State == 3)
					newState = 1;
			}
			base.SwitchToState (newState, out newView, out wizardState);
		}

	}
}
