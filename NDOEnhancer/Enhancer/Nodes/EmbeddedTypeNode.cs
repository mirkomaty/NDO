//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Reflection;
using NDO;


namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für EmbeddedTypeNode.
	/// </summary>
	internal class EmbeddedTypeNode : FieldNode
	{
		Type declaringType;
		public Type DeclaringType
		{
			get { return declaringType; }
		}


		public EmbeddedTypeNode(FieldInfo parentField) : base(parentField)
		{
			FieldInfo[] fis = this.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			if (parentField.DeclaringType != parentField.ReflectedType)
				this.declaringType = parentField.DeclaringType;				
			else
				this.declaringType = null;
			foreach (FieldInfo fi in fis)
			{
				string fname = fi.Name;
				if (fname.StartsWith("_ndo"))
					continue;
				if (fi.FieldType.IsSubclassOf(typeof(System.Delegate)))
					continue;

				object[] attributes = fi.GetCustomAttributes(false);
				bool cont = false;
				foreach (System.Attribute attr in attributes)
				{
					if (attr is NDOTransientAttribute)
					{
						cont = true;
						break;
					}
				}
				if (cont) continue;
				if (!new ReflectedType(fi.FieldType).IsStorable)
					return;  // Resultate verwerfen
				this.Fields.Add(new FieldNode(fi));
			}
		}




	}
}
