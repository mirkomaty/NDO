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
using WizardBase;

namespace ClassGenerator.IntermediateTableWizard
{
	/// <summary>
	/// Zusammenfassung für IntermediateTableWizardModel.
	/// </summary>
#if DEBUG
	public class IntermediateTableWizardModel : IModel
#else
	internal class IntermediateTableWizardModel : IModel
#endif
	{
		IntermediateTableNode intTableNode;
		IList tableNodes;

		// This is used for the state management.
		// Two views of the view controller share one user control.
		int index;
		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		public IList TableNodes
		{
			get { return tableNodes; }
		}

		public IntermediateTableNode IntermediateTableNode
		{
			get { return intTableNode; }
		}
		
		public IntermediateTableWizardModel(IntermediateTableNode intTableNode, IList tableNodes)
		{
			this.intTableNode = intTableNode;
			this.tableNodes = tableNodes;
		}

		public TableNode FindTable(string name)
		{
			foreach(TableNode tn in this.tableNodes)
				if (tn.Text == name)
					return tn;
			return null;
		}
	}
}
