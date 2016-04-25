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
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using ClassGenerator.Serialisation;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für NDOTreeNode.
	/// </summary>
	[Serializable]
#if DEBUG
	public class NDOTreeNode : TreeNode, IXmlStorable
#else
	internal class NDOTreeNode : TreeNode
#endif
	{
		protected object myObject;
		NDOTreeNode tnparent;

		public NDOTreeNode(string s, NDOTreeNode parent) : base (s)
		{
			this.tnparent = parent;
		}


		[Browsable(false)]
		public object Object
		{
			get { return myObject; }
			set { myObject = value; }
		}

		public new NDOTreeNode Parent
		{
			get { return tnparent; }
		}

		protected void SetImageIndex(int ix)
		{
			this.ImageIndex = ix;
			this.SelectedImageIndex = ix;
		}

		public virtual ContextMenu GetContextMenu()
		{
			return new ContextMenu();
		}

		public NDOTreeNode FindNode(string name)
		{
			foreach(NDOTreeNode node in this.Nodes)
			{
				if (node.Text == name)
					return node;
			}
			return null;
		}

		public NDOTreeNode FindNode(string name, Type t)
		{
			foreach(NDOTreeNode node in this.Nodes)
			{
				if (node.Text == name && node.GetType() == t)
					return node;
			}
			return null;
		}

		public NDOTreeNode FindNode(string name, Type t, NDOTreeNode parent)
		{
			foreach(NDOTreeNode node in this.Nodes)
			{
				if (node.tnparent == parent && node.Text == name && node.GetType() == t)
					return node;
			}
			return null;
		}


		Dictionary<string, object> userData = new Dictionary<string,object>();
		public Dictionary<string, object> UserData
		{
			get { return userData; }
		}

		public virtual new string Name
		{
			set { throw new NotImplementedException(); }
			get { return base.Text; }
		}


		public override string ToString()
		{
			return base.Text;
		}

		protected virtual void CalculateIndex()
		{
			throw new Exception("Internal Error: NDOTreeNode.CalculateIndex for type " 
				+ GetType().Name + " called.");
		}		

		public virtual void ToXml(XmlElement element)
		{
			element.SetAttribute("Type", this.GetType().FullName);
			element.SetAttribute("Text", this.Text);
			element.SetAttribute("FixupId", this.GetHashCode().ToString());
			if (tnparent != null)
				element.SetAttribute("Parent", this.tnparent.GetHashCode().ToString());
			IXmlStorable storable = myObject as IXmlStorable;
			if (myObject != null && storable == null)
			{
				MessageBox.Show("ToXml: Type " + myObject.GetType().FullName + " is not an IXmlStorable.", "Class Generator", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				XmlElement objectElement = element.OwnerDocument.CreateElement("Entity");
				element.AppendChild(objectElement);
				objectElement.SetAttribute("Type", myObject.GetType().FullName);
				storable.ToXml(objectElement);
			}
		}

		public virtual void FromXml(XmlElement element)
		{
			this.Text = element.Attributes["Text"].Value;
			if (element.Attributes["Parent"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["Parent"].Value),
					typeof(NDOTreeNode).GetField("tnparent", BindingFlags.NonPublic | BindingFlags.Instance),
					this);
			}
			XmlElement objectElement = (XmlElement)element.SelectSingleNode("Entity");
			Type t = Type.GetType(objectElement.Attributes["Type"].Value);
			this.myObject = Activator.CreateInstance(t);
			((IXmlStorable)myObject).FromXml(objectElement);
			CalculateIndex();
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public NDOTreeNode()
		{
		}


#if SoapSerialisation
		#region Serialization Members

		IList GetSerializableFields()
		{
			ArrayList fieldInfos = new ArrayList(this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
			fieldInfos.AddRange(this.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
			ArrayList result = new ArrayList(fieldInfos.Count);
			foreach(FieldInfo fi in fieldInfos)
			{
				bool serialize = (fi.Attributes & FieldAttributes.NotSerialized) == 0;				
				if (serialize && !(typeof(System.Delegate).IsAssignableFrom(fi.FieldType)))
					result.Add(fi);
			}
			return result;
		}

		public void GetObjectData(SerializationInfo serInfo, StreamingContext context)
		{
			IList fieldInfos = GetSerializableFields();
			serInfo.AddValue("Text", this.Text);

			foreach(FieldInfo fi in fieldInfos)
			{
				serInfo.AddValue(fi.Name, fi.GetValue(this));
			}
		}

		protected NDOTreeNode(SerializationInfo info, StreamingContext context)
		{
			try
			{
				IList fieldInfos = GetSerializableFields();
				this.Text = (string) info.GetValue("Text", typeof(string));
				foreach(FieldInfo fi in fieldInfos)
				{
					try
					{
						object o = info.GetValue(fi.Name, typeof(object));
						fi.SetValue(this, info.GetValue(fi.Name, typeof(object)));
					}
					catch (Exception ex)
					{
						//System.Diagnostics.Debug.WriteLine("Deser: " + ex.Message + " " + this.GetType().Name + "." + fi.Name);
					}
				}				
			}
			catch (Exception ex)
			{
				//System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
			CalculateIndex();
		}

		#endregion
#endif
	}
}
