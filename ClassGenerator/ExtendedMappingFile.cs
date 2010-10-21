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
using System.Xml;
using System.Collections;
using NDO.Mapping;
using Generator;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für ExtendedMappingFile.
	/// </summary>
	public class ExtendedMappingFile
	{
		string fileName;
		IList nodeCollection;
		DatabaseNode databaseNode;
		NDOMapping mapping;

		public DatabaseNode DatabaseNode
		{
			get { return databaseNode; }
		}
		AssemblyNode assemblyNode;
		public AssemblyNode AssemblyNode
		{
			get { return assemblyNode; }
		}

		public ExtendedMappingFile(string fileName, IList nodeCollection)
		{
			this.fileName = fileName;
			this.nodeCollection = nodeCollection;
		}
		public ExtendedMappingFile()
		{
		}

		public void Save()
		{
			foreach(NDOTreeNode trn in nodeCollection)
			{
				DatabaseNode dn = trn as DatabaseNode;
				if (dn != null)
					this.databaseNode = dn;
				AssemblyNode an = trn as AssemblyNode;
				if (an != null)
					this.assemblyNode = an;
			}
			new MappingGenerator(fileName, databaseNode, assemblyNode).GenerateCode();
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			XmlNode refNode = doc.SelectSingleNode("NDOMapping");
			XmlNode comment = doc.CreateComment("NDO Class Generator file - this is not a valid mapping file!");
			doc.InsertBefore(comment, refNode);
			assemblyNode.Assembly.ToXml(doc);
			XmlElement node = (XmlElement) doc.SelectSingleNode("NDOMapping/AssemblyElement");
			node.SetAttribute("DbOwner", this.databaseNode.Database.OwnerName);
			doc.Save(fileName);
		}

		void CheckFields(TableNode tn, NDO.Mapping.Class cl)
		{
		}

		void CheckRelations(TableNode tn, NDO.Mapping.Class cl)
		{
		}

		string GetClassNamespace(string classFullName)
		{
			int pos = classFullName.LastIndexOf('.');
			if (pos > -1)
				return classFullName.Substring(0, pos);
			else
				return "";
		}

		string GetClassShortName(string classFullName)
		{
			int pos = classFullName.LastIndexOf('.');
			if (pos > -1)
				return classFullName.Substring(pos + 1);
			else
				return classFullName;
		}


		void MapIntermediateClass(TableNode tn, NDO.Mapping.Class cl)
		{
			if (tn.Table.PrimaryKey == "")
				tn.Table.PrimaryKey = cl.Oid.ColumnName;
			//TODO: Intermediate class berücksichtigen
			tn.MapClass(null, EventArgs.Empty);
		}

		void MapClass(TableNode tn, NDO.Mapping.Class cl)
		{
			tn.MapIntermediateClass(null, EventArgs.Empty);
		}

		public void CheckMappings()
		{
			Hashtable classMappings = new Hashtable(this.mapping.Classes.Count);
			foreach(NDO.Mapping.Class cl in this.mapping.Classes)
				classMappings.Add(cl.TableName, cl);
			foreach(TableNode tn in databaseNode.Nodes)
			{
				NDO.Mapping.Class cl = classMappings[tn.Table.Name] as NDO.Mapping.Class;
				if (cl != null)
				{
					tn.Table.Namespace = GetClassNamespace(cl.FullName);
					tn.Table.ClassName = GetClassShortName(cl.FullName);
					if (cl.Oid.IsDual)
						MapIntermediateClass(tn, cl);
					else
						MapClass(tn, cl);
					CheckFields(tn, cl);
					CheckRelations(tn, cl);
				}
			}
		}


		public void Load(string fileName)
		{
			this.fileName = fileName;
			this.nodeCollection = new ArrayList();
				
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			Assembly assembly = new Assembly(doc);
			this.assemblyNode = new AssemblyNode(assembly);
			this.mapping = new NDO.Mapping.NDOMapping(fileName);
			NDO.Mapping.Connection conn = (NDO.Mapping.Connection)mapping.Connections[0];
			XmlElement node = (XmlElement) doc.SelectSingleNode("NDOMapping/AssemblyElement");
			Database db = new Database(conn.Name, conn.Type, node.Attributes["DbOwner"].Value);
			this.databaseNode = new DatabaseNode(db);  // Loads all db related information
		}
	}
}
