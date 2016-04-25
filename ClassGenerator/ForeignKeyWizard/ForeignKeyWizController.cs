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
