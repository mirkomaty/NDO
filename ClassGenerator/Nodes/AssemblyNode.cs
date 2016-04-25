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
using ClassGenerator.AssemblyWizard;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für AssemblyNode.
	/// </summary>
	[Serializable]
#if DEBUG	
	public class AssemblyNode : NDOTreeNode
#else
	internal class AssemblyNode : NDOTreeNode
#endif
	{
		public Assembly Assembly
		{
			get { return myObject as Assembly; }
		}
		public AssemblyNode(AssemblyWizModel model) : base("Assembly", null)
		{
			this.myObject = new Assembly(model.ProjectName, model.DefaultNamespace, model.ProjectDirectory, model.UseClassField, model.MapStringsAsGuids, model.TargetLanguage);
			CalculateIndex();
		}

		public AssemblyNode(Assembly ass) : base ("Assembly", null)
		{
			this.myObject = ass;
			CalculateIndex();
		}

		public void Refresh()
		{
			this.Nodes.Clear();
			foreach(NDOTreeNode trn in ApplicationController.Instance.DatabaseNode.Nodes)
			{
				TableNode tn = trn as TableNode;
				if (tn == null)
					continue;
				if (tn.Table.MappingType == TableMappingType.MappedAsClass || tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass)
				this.Nodes.Add(new ClassNode(tn.Table.ClassName, tn.Table.Name, this));
			}
		}

		protected override void CalculateIndex()
		{
			SetImageIndex(18);
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public AssemblyNode()
		{
		}


#if SoapSerialisation
		protected AssemblyNode(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
#endif
	}
}
