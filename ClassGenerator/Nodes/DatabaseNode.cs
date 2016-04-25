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
using System.Collections.Generic;
using System.ComponentModel;
using NDO;
using NDOInterfaces;
using System.Data;
using System.Windows.Forms;
using ClassGenerator.AssemblyWizard;
using System.Diagnostics;

namespace ClassGenerator
{
	[Serializable]
#if DEBUG	
	public class DatabaseNode : NDOTreeNode
#else
	internal class DatabaseNode : NDOTreeNode
#endif
	{
		public ArrayList TableNodes
		{
			get 
			{
				ArrayList al = new ArrayList();
				foreach(NDOTreeNode trn in this.Nodes)
				{
					TableNode tn = trn as TableNode;
					if (tn == null)
						continue;
					al.Add(tn);
				}
				return al;
			}
		}

		public ArrayList UnmappedTableNodes
		{
			get 
			{
				ArrayList al = new ArrayList();
				foreach(NDOTreeNode trn in this.Nodes)
				{
					TableNode tn = trn as TableNode;
					if (tn == null)
						continue;
					if (tn.Table.MappingType == TableMappingType.NotMapped)
						al.Add(tn);
				}
				return al;
			}
		}

		public ArrayList Classes
		{
			get 
			{
				ArrayList al = new ArrayList();
				foreach(NDOTreeNode trn in this.Nodes)
				{
					TableNode tn = trn as TableNode;
					if (tn == null)
						continue;
					if (tn.Table.MappingType == TableMappingType.MappedAsClass)
						al.Add(tn);
				}
				return al;
			}
		}

		public DatabaseNode(AssemblyWizModel model) : base("Database", null)
		{
			this.myObject = new Database(model);			
			Init();
		}

		public DatabaseNode(Database db) : base("Database", null)
		{
			this.myObject = db;
			Init();
		}

		void Init()
		{
#if DontUseDataSets
			if ( !this.Database.IsXmlSchema )
			{
				IProvider provider = NDOProviderFactory.Instance[Database.ConnectionType];
				IDbConnection conn = null;
				try
				{
					conn = provider.NewConnection( Database.ConnectionString );
				}
				catch
				{
					conn = null;//MessageBox.Show("Can't open connection '" + Database.ConnectionString + '\'');
				}
				if ( conn == null ) // can be null out of several reasons
				{
					MessageBox.Show( "Can't open connection '" + Database.ConnectionString + '\'' );
					return;
				}
				string[] tnames;
				try
				{
					conn.Open();
				}
				catch ( Exception ex )
				{
					MessageBox.Show( "Can't open connection '" + Database.ConnectionString + "'.\n" + ex.Message );
					return;
				}
				try
				{
					if ( Database.OwnerName != null && Database.OwnerName != string.Empty )
						tnames = provider.GetTableNames( conn, Database.OwnerName );
					else
						tnames = provider.GetTableNames( conn );
					foreach ( string s in tnames )
					{
						this.Nodes.Add( new TableNode( s, this, Database.OwnerName, conn, provider ) );
					}
				}
				catch ( Exception ex )
				{
					MessageBox.Show( "Error while collecting database schema information: " + ex.Message );
				}
				if ( conn.State != ConnectionState.Closed )
					conn.Close();
			}
			else
			{
#endif

                List<TableNode> l = new List<TableNode>();
				foreach ( DataTable dt in this.Database.DataSet.Tables )
				{
					l.Add( new TableNode( dt, this, string.Empty, null, null) );
				}
                l.Sort(delegate(TableNode tn1, TableNode tn2)
                {
                    return tn1.Name.CompareTo(tn2.Name);
                });
                foreach (TableNode tn in l)
                    this.Nodes.Add(tn);
				// Store the parent nodes, so that we can navigate through the schema hierarchy.
				// We need this to generate the correct XPath, if we skip an Xml Element
				//foreach ( DataRelation dr in this.Database.DataSet.Relations )
				//{
				//    TableNode parentNode = FindTableNode(dr.ParentTable.TableName, true);
				//    TableNode childNode = FindTableNode(dr.ChildTable.TableName, true);
				//    childNode.UserData.Add( "parentnode", parentNode );
				//}
//			}
			CalculateIndex();
		}

		public TableNode this[string name]
		{
			get 
			{ 
				foreach(TableNode tn in this.TableNodes)
					if (tn.Text == name)
						return tn;
				return null;
			}
		}


		public void MapXmlSchema()
		{
			foreach(TableNode tn in this.TableNodes)
				tn.MapClass(null, EventArgs.Empty);

			DataSet ds = this.Database.DataSet;
			foreach ( DataRelation dr in ds.Relations )
			{
                string xpath = (string)dr.ExtendedProperties["xpath"];
				ApplicationController.Instance.MakeForeignKey(dr);
			}
		}

		public TableNode FindTableNode(string tableName, bool throwIfNotFound)
		{
			foreach(NDOTreeNode trn in this.Nodes)
			{
				TableNode tn = trn as TableNode;
				if (tn == null)
					continue;
				if (tn.ToString() == tableName)
					return tn;
			}
			if (throwIfNotFound)
				throw new Exception("FindTableNode: Can't find table '" + tableName + "'.");
			return null;
		}

		public Database Database
		{
			get { return this.myObject as Database; }
		}

		protected override void CalculateIndex()
		{			
			SetImageIndex(12);
		}
		/// <summary>
		/// Used by serialization only
		/// </summary>
		public DatabaseNode()
		{
		}

		public override void FromXml(System.Xml.XmlElement element)
		{
			base.FromXml (element);
			//TODO: Db-Refresh
		}


	}
}
