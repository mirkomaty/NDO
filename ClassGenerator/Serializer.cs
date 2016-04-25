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
using System.Reflection;
using System.Xml;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace ClassGenerator.Serialisation
{
#if SoapSerializer
	[Serializable]
	internal class ValuePair
	{
		NDOTreeNode treeNode;
		ArrayList list;

		public ValuePair(NDOTreeNode treeNode, ArrayList list)
		{
			this.treeNode = treeNode;
			this.list = list;
		}

		public ArrayList List
		{
			get { return list; }
		}

		public NDOTreeNode TreeNode
		{
			get { return treeNode; }
		}
	}
#endif
	/// <summary>
	/// Zusammenfassung für Serializer.
	/// </summary>
	internal class NodeListSerializer
	{
		string fileName;
		IList mainNodeCollection;
		Hashtable allNodes;

		// We assume, that there is always only one NodeListSerializer at work...
		static ArrayList fixupList;

		public NodeListSerializer(string fileName, IList nodeCollection)
		{
			this.fileName = fileName;
			this.mainNodeCollection = nodeCollection;
		}

		public void Serialize()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement el = doc.CreateElement("ClassGeneratorData");
			doc.AppendChild(el);
			Serialize(this.mainNodeCollection, el);
			doc.Save(this.fileName);
#if Soap
			ArrayList al = new ArrayList();
			Serialize(nodeCollection, al);
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(fileName, 
				FileMode.Create, 
				FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, al);
			stream.Close();		
#endif
		}

		void Serialize(IList nodeCollection, XmlElement parent)//, ArrayList parent)
		{
			foreach(NDOTreeNode trn in nodeCollection)
			{
#if Soap
				ArrayList al = new ArrayList();
				parent.Add(new ValuePair(trn, al));
#endif
				XmlElement el = parent.OwnerDocument.CreateElement("NDOTreeNode");
				parent.AppendChild(el);
				trn.ToXml(el);
				Serialize(trn.Nodes, el);
			}
		}
#if Soap
		void Deserialize(IList valuePairs, IList nodeCollection)
		{

			foreach(ValuePair vp in valuePairs)
			{
				NDOTreeNode trn = vp.TreeNode;
				nodeCollection.Add(trn);
				Deserialize(vp.List, trn.Nodes);
			}
		}
#endif
		public static void AddFixup(int id, FieldInfo fi, object o)
		{
			System.Diagnostics.Debug.Assert(fi != null);
			fixupList.Add(new FixupEntry(id, fi, o));
		}


		void Deserialize(XmlElement parentElement, IList nodeCollection)
		{
			foreach(XmlElement element in parentElement.SelectNodes("NDOTreeNode"))
			{
				Type t = Type.GetType(element.Attributes["Type"].Value);
				NDOTreeNode trn = (NDOTreeNode)Activator.CreateInstance(t);
				nodeCollection.Add(trn);
				int id = int.Parse(element.Attributes["FixupId"].Value);
				this.allNodes.Add(id, trn);
				trn.FromXml(element);
				Deserialize(element, trn.Nodes);
			}
		}

		void CheckParent(IList nodeColl)
		{
			System.Diagnostics.Debug.Indent();
			foreach(NDOTreeNode tn in nodeColl)
			{
				bool b = tn.Parent != null;
				//System.Diagnostics.Debug.WriteLine(tn.Text + " " + b.ToString());
				CheckParent(tn.Nodes);
			}
			System.Diagnostics.Debug.Unindent();
		}
		public void Deserialize(IList nodeCollection)
		{
			// We assume, that there is always only one NodeListSerializer at work...			
			fixupList = new ArrayList();
			allNodes = new Hashtable();

			XmlDocument doc = new XmlDocument();
			doc.Load(this.fileName);
			XmlElement parent = (XmlElement)doc.SelectSingleNode("ClassGeneratorData");
			Deserialize(parent, nodeCollection);
			foreach(FixupEntry entry in fixupList)
			{
				NDOTreeNode trn = (NDOTreeNode) this.allNodes[entry.Id];
				entry.FieldInfo.SetValue(entry.Object, trn);
			}

			CheckParent(nodeCollection);
			fixupList = null;
			allNodes = null;

#if Soap
//			IFormatter formatter = new BinaryFormatter();
//			Stream stream = new FileStream(fileName, 
//				FileMode.Open, 
//				FileAccess.Read, 
//				FileShare.Read);
//			ArrayList valuePairs = (ArrayList) formatter.Deserialize(stream);
//			stream.Close();
//			Deserialize(valuePairs, nodeCollection);
#endif
		}
	}

}
