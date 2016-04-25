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
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using NDO;
using NDOInterfaces;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für TableNode.
	/// </summary>
	[Serializable]
#if DEBUG
	public class TableNode : NDOTreeNode
#else
	internal class TableNode : NDOTreeNode
#endif
	{
		public void MapIntermediateTable(Object sender, EventArgs e)
		{
			ApplicationController.Instance.MapIntermediateTable(this);
		}
		public void MapIntermediateClass(Object sender, EventArgs e)
		{
				if (this.Table.MappingType != TableMappingType.NotMapped)
						return;

			MapClass(false);
			
			if (ApplicationController.Instance.MapIntermediateClass(this))
			{
				this.Table.MappingType = TableMappingType.MappedAsIntermediateClass;
				this.OnIsMappedChanged(null, EventArgs.Empty);
			}
			else
				UnmapClass(null, EventArgs.Empty);
		}

		public void UnmapIntermediateClass(Object sender, EventArgs e)
		{
			ApplicationController.Instance.UnmapIntermediateClass(this);
			this.Table.MappingType = TableMappingType.NotMapped;
			UnmapClass(sender, e);
		}

		private void MapClass(bool checkForPrimaryKey)
		{
			this.Table.Namespace = ApplicationController.Instance.AssemblyNode.Assembly.RootNamespace;

			this.Table.MappingType = TableMappingType.MappedAsClass;
			this.OnIsMappedChanged(null, EventArgs.Empty);
			this.Table.PrimaryKey = string.Empty;
			foreach(NDOTreeNode tn in this.Nodes)
			{
				ColumnNode cn = tn as ColumnNode;
				if (cn != null)
				{
                    if (!cn.IsPrimary)
                        cn.IsMapped = true;
                    else
                    {
                        this.Table.PrimaryKey = cn.Name;
                        if (cn.Column.Type == "System.String")
                        {
                            Assembly ass = ApplicationController.Instance.AssemblyNode.Assembly;
                            if (ass.MapStringsAsGuids)
                            {
                                this.Table.NdoOidType = "Guid";
                            }
                            else
                            {
                                if (ass.UseClassField)
                                    cn.IsMapped = true;
                                else
                                    this.Table.NdoOidType = "string";
                            }
                        }
                    }
				}
			}

			if (checkForPrimaryKey && this.Table.PrimaryKey == string.Empty)
			{
				ClassGenerator.MapClassWizard.MapClassWizard wiz = new ClassGenerator.MapClassWizard.MapClassWizard(this);
				DialogResult dr = wiz.ShowDialog();
				if (dr == DialogResult.OK && wiz.Result != null)
				{
					if (wiz.Result.IsMapped)
						wiz.Result.ChangeMapping(null, EventArgs.Empty);
					wiz.Result.ChangePrimary(null, EventArgs.Empty);
				}
			}			
			ApplicationController.Instance.MappingChanged();
		}

		public void MapClass(Object sender, EventArgs e)
		{
			if (this.Table.MappingType != TableMappingType.NotMapped)
					return;
			MapClass(true);
		}

        void AddPrimaryKeyColumn(Object sender, EventArgs e)
        {
            ApplicationController.Instance.AddPrimaryKeyColum(this);
        }

		public void UnmapClass(Object sender, EventArgs e)
		{
			if (this.Table.MappingType == TableMappingType.NotMapped)
				return;
			this.Table.MappingType = TableMappingType.NotMapped;
			this.OnIsMappedChanged(null, EventArgs.Empty);
			ArrayList nodesToDelete = new ArrayList();
			foreach(NDOTreeNode tn in this.Nodes)
			{
				ColumnNode cn;
				RelationNode rn;
				if ((cn = tn as ColumnNode) != null)
					cn.IsMapped = false;
				else if ((rn = tn as RelationNode) != null)
					nodesToDelete.Add(rn);
			}
			foreach(RelationNode rn2 in nodesToDelete)
				ApplicationController.Instance.DeleteRelation(rn2);
			ApplicationController.Instance.MappingChanged();
		}

		public void Skip( Object sender, EventArgs e )
		{
			this.Table.Skipped = true;
		}

		public void Unskip( Object sender, EventArgs e )
		{
			this.Table.Skipped = false;
		}


		public override System.Windows.Forms.ContextMenu GetContextMenu()
		{
			ContextMenu menu = new ContextMenu();

			if (!(((TreeNode)this).Parent is DatabaseNode))  // This is the case, if the table is mapped as intermediate table.
				return menu;

			if (this.Table.MappingType != TableMappingType.NotMapped)
			{
				menu.MenuItems.Add(new MenuItem("Unmap Table", new EventHandler(this.UnmapClass)));
			}
			else
			{
                menu.MenuItems.Add(new MenuItem("Add Primary Key Column", new EventHandler(this.AddPrimaryKeyColumn)));
				menu.MenuItems.Add(new MenuItem("Map Table to Class", new EventHandler(this.MapClass)));
				if (this.Nodes.Count >= 2 && ((DatabaseNode)this.Parent).Classes.Count >= 2)
				{
					menu.MenuItems.Add(new MenuItem("Define Table as Intermediate Table", new EventHandler(this.MapIntermediateTable)));
					menu.MenuItems.Add(new MenuItem("Map Table as Intermediate Class", new EventHandler(this.MapIntermediateClass)));
				}
			}

			if ( ((DatabaseNode) this.Parent).Database.IsXmlSchema )
			{
				if (this.Table.Skipped)
					menu.MenuItems.Add(new MenuItem("Don't skip this Xml Node", new EventHandler(this.Unskip)));
				else
					menu.MenuItems.Add(new MenuItem("Skip this Xml Node", new EventHandler(this.Skip)));
			}
			return menu;
		}

		private string GeneratePkName( DataTable dt )
		{
			int i = 0;
			string columnName = dt.TableName + "_Id";
			while(dt.Columns.Contains(columnName))
			{
				i++;
				columnName = dt.TableName + "_Id" + i;
			}
			return columnName;
		}

		public TableNode(DataTable dt, DatabaseNode parent, string ownerName, IDbConnection conn, IProvider provider) : base(dt.TableName, parent)
		{
			string tableName = dt.TableName;

			SetImageIndex(15); // unmapped
            this.myObject = new Table(tableName, string.Empty, (string)dt.ExtendedProperties["summary"]);
			this.Table.OnIsMappedChanged += new EventHandler(OnIsMappedChanged);

#if DontUseDataSets
			DataTable dt = null;

			if ( !parent.Database.IsXmlSchema )
			{
				string sql;

				if ( ownerName != null && ownerName.Trim() != "" )
				{
					sql = "SELECT * FROM " + provider.GetQuotedName( ownerName ) + "." + provider.GetQuotedName( tableName );
				}
				else
				{
					sql = "SELECT * FROM " + provider.GetQuotedName( tableName );
				}

				DataSet ds = new DataSet();
				IDbCommand cmd = provider.NewSqlCommand( conn );
				cmd.CommandText = sql;
				IDataAdapter da = provider.NewDataAdapter( cmd, null, null, null );

				da.FillSchema( ds, SchemaType.Source );
				dt = ds.Tables[0];
			}
			else
			{
				dt = parent.Database.DataSet.Tables[tableName];
#endif
			if ( parent.Database.IsXmlSchema && dt.PrimaryKey.Length  == 0 )
			{
				string pkColumnName = GeneratePkName( dt );
				DataColumn pkColumn = dt.Columns.Add( pkColumnName, typeof( int ) );
				dt.PrimaryKey = new DataColumn[] { pkColumn };
			}
			
			foreach ( DataColumn column in dt.Columns )
			{
				bool isPrimary = false;
				bool isAutoIncremented = false;
				foreach ( DataColumn pkColumn in dt.PrimaryKey )
				{
					if ( pkColumn == column )
					{
						isPrimary = true;
						isAutoIncremented = pkColumn.AutoIncrement;
					}
				}
				ColumnNode cn = new ColumnNode( column.ColumnName, isPrimary, isAutoIncremented, column.DataType.ToString(), (string)column.ExtendedProperties["summary"], this );
				if(parent.Database.IsXmlSchema && column.ExtendedProperties.Contains("schemaType"))
					cn.UserData.Add("schemaType", "Element");

				cn.IsPrimaryChanged += new EventHandler( OnIsPrimaryChanged );
				this.Nodes.Add( cn );
			}
		}


		private void OnIsMappedChanged(object sender, EventArgs e)
		{
			if (this.Table.MappingType == TableMappingType.MappedAsClass)
				SetImageIndex(14);
			else if (this.Table.MappingType == TableMappingType.MappedAsIntermediateClass)
				SetImageIndex(19);
			else
				SetImageIndex(15); // Unmapped. IntermediateTable shouldn't occur
		}		


		[Browsable(false)]
		public bool HasPrimary
		{
			get { return this.Table.PrimaryKey != string.Empty; }
		}

		public Table Table
		{
			get { return this.myObject as Table; }
		}

		private void OnIsPrimaryChanged(object sender, EventArgs e)
		{
			ColumnNode cn = sender as ColumnNode;
			if (cn == null)
				return;
			if (cn.IsPrimary)
			{
				this.Table.PrimaryKey = cn.Text;
			}
			if (cn.Name == this.Table.PrimaryKey)
			{
				if (!cn.IsPrimary)
				{
					this.Table.PrimaryKey = string.Empty;
				}
			}
		}

		public IList ColumnNodes
		{
			get 
			{ 
				IList result = new ArrayList();
				foreach(NDOTreeNode tn in Nodes)
				{
					if (tn is ColumnNode)
						result.Add(tn);
				}
				return result;
			}
		}

		public IList RelationNodes
		{
			get 
			{ 
				IList result = new ArrayList();
				foreach(NDOTreeNode tn in Nodes)
				{
					if (tn is RelationNode)
						result.Add(tn);
				}
				return result;
			}
		}

		public RelationNode FindRelationNode(string text, string relatedTableName)
		{
			foreach(NDOTreeNode tn in Nodes)
			{
				RelationNode rn = tn as RelationNode;
				if (rn == null)
					continue;
				if (rn.Text == text && rn.RelatedTableNode.Text == relatedTableName)
					return rn;
			}
			return null;
		}

		public override string ToString()
		{
			return this.Table.Name;
		}

		string[] dualKeyRelations = new string[2];
		public string[] DualKeyRelations
		{
			get { return dualKeyRelations; }
		}

		protected override void CalculateIndex()
		{
			OnIsMappedChanged(null, EventArgs.Empty);
		}

		public override void ToXml(System.Xml.XmlElement element)
		{
			base.ToXml (element);
			for (int i = 0; i < 2; i++)
			{
				element.SetAttribute("DualKeyRelations" + i, this.dualKeyRelations[i]);
			}
		}

		public override void FromXml(System.Xml.XmlElement element)
		{
			base.FromXml (element);
			for (int i = 0; i < 2; i++)
			{
				this.dualKeyRelations[i] = element.Attributes["DualKeyRelations" + i].Value; 
			}
			//TODO: Database refresh
		}

		public ColumnNode this[string name]
		{
			get 
			{ 
				foreach(ColumnNode cn in this.ColumnNodes)
					if (cn.Text == name)
						return cn;
				return null;
			}
		}


		/// <summary>
		/// Used for serialization only
		/// </summary>
		public TableNode()
		{
		}


#if SoapSerialisation
		protected TableNode(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
#endif

	}
}
