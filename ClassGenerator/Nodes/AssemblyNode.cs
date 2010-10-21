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
