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
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using ClassGenerator;

namespace Generator
{
	/// <summary>
	/// Zusammenfassung für MappingGenerator.
	/// </summary>
	internal class MappingGenerator
	{
		string fileName;
		DatabaseNode databaseNode;
		Assembly ass;
		Database database;
		public MappingGenerator(string fileName, DatabaseNode databaseNode, AssemblyNode assemblyNode)
		{
			this.fileName = fileName;
			this.databaseNode = databaseNode;
			this.ass = assemblyNode.Assembly;
			database = databaseNode.Database;
		}

		public void GenerateCode()
		{
			NDO.Mapping.NDOMapping mapping = NDO.Mapping.NDOMapping.Create(fileName + ".new");
			mapping.SchemaVersion = "1.0";
			NDO.Mapping.Connection conn = new NDO.Mapping.Connection(mapping);
			conn.Type = database.ConnectionType;
			conn.Name = database.ConnectionString;
			conn.ID = "C0";
			mapping.AddConnection(conn);
			foreach(NDOTreeNode treenode in databaseNode.Nodes)
			{
				TableNode tn = treenode as TableNode;
				if (tn != null && !tn.Table.Skipped)
				{
					if (tn.Table.MappingType == TableMappingType.MappedAsClass 
						|| tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass)
					{
						NDO.Mapping.Class cl = mapping.AddStandardClass(tn.Table.Namespace + "." + tn.Table.ClassName, ass.ProjectName, null);
						// Todo: Check, if not other options were choosed.
						if (tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass)
						{
                            //cl.Oid.ParentRelation = tn.DualKeyRelations[0];
                            //cl.Oid.ChildRelation = tn.DualKeyRelations[1];
						}
						if (((DatabaseNode)tn.Parent).Database.OwnerName != string.Empty)
							cl.TableName = ((DatabaseNode)tn.Parent).Database.OwnerName + "." + tn.Table.Name;
						else
							cl.TableName = tn.Table.Name;
						cl.ConnectionId = "C0";
						AddFields(tn, cl);
						AddRelations(tn, cl);
					}
				}
			}
			MergeableFile mergeableFile = new MergeableFile(fileName);
			Stream dummy = mergeableFile.Stream;  // moves the file to filename.old
			dummy.Close();
			try
			{
				mapping.Save();
				if ( File.Exists( mergeableFile.OldFileName ) )
				{
					Merge.CommentPrefix = "<!--";
					Merge.MergeFiles( mergeableFile.OldFileName, fileName + ".new", fileName );
					File.Delete(mergeableFile.OldFileName);
					File.Delete(fileName + ".new");
				}
				else
				{
					File.Copy( fileName + ".new", fileName, true );
					File.Delete( fileName + ".new" );
				}
			}
			catch ( Exception ex )
			{
				mergeableFile.Restore();
				throw ex;
			}
		}

		void AddFields(TableNode tn, NDO.Mapping.Class cl)
		{
			foreach(ColumnNode cn in tn.ColumnNodes)
			{
				if (cn.Column.IsPrimary)
				{
					NDO.Mapping.ClassOid oid = cl.NewOid();
                    NDO.Mapping.OidColumn oidColumn = oid.NewOidColumn();
					if (cn.Column.AutoIncremented)
						oidColumn.AutoIncremented = true;
                    oidColumn.Name = cn.Column.Name;
					if (cn.IsMapped)
                        oidColumn.FieldName = cn.Column.FieldName;
				}
			}

			foreach(ColumnNode cn in tn.ColumnNodes)
			{
				if (cn.IsMapped)
				{
					NDO.Mapping.Field field = cl.AddStandardField(cn.Column.FieldName, cn.Column.IsPrimary);
					field.Column.Name = cn.Column.Name;
				}
			}
		}

		void AddRelations(TableNode tn, NDO.Mapping.Class cl)
		{
			foreach(RelationNode rn in tn.RelationNodes)
			{
				Relation rel = rn.Relation;
				// This is for FkRelations, which must exist as entries in the
				// tree in order to delete the relation mapping.
				if (rel.RelationDirection == RelationDirection.DirectedToMe)
					return;
				if (rn.RelatedTableNode.Table.Skipped)
					continue;
				NDO.Mapping.Relation r = cl.AddStandardRelation(rel.FieldName, rn.RelatedTableNode.Table.Namespace + "." + rn.RelatedTableNode.Table.ClassName, rel.IsElement, string.Empty, false, false, null);
				r.RelationName = rel.RelationName;
				ForeignIntermediateTableRelation fitr = rel as ForeignIntermediateTableRelation;
                NDO.Mapping.ForeignKeyColumn fkColumn = (NDO.Mapping.ForeignKeyColumn) r.ForeignKeyColumns.First();
                if (fitr == null)
				{
					fkColumn.Name = rn.Text;
				}
				else
				{
                    fkColumn.Name = fitr.ForeignKeyColumnName;
					NDO.Mapping.MappingTable mtable = new NDO.Mapping.MappingTable(r);
					r.MappingTable = mtable;
                    NDO.Mapping.ForeignKeyColumn cfkColumn = mtable.NewForeignKeyColumn();
                    cfkColumn.Name = fitr.ChildForeignKeyColumnName;
					mtable.ConnectionId = cl.ConnectionId;
					if (((DatabaseNode)tn.Parent).Database.OwnerName != string.Empty)
						mtable.TableName = ((DatabaseNode)tn.Parent).Database.OwnerName + "." + fitr.TableName;
					else
						mtable.TableName = fitr.TableName;
				}
			}
		}
	}
}
