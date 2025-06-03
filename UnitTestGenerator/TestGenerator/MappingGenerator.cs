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
using System.Diagnostics;
using System.Collections;
using System.IO;
using NDO.Mapping;
using System.Linq;
using NDO.ProviderFactory;

namespace TestGenerator
{
	/// <summary>
	/// Summary for ClassGenerator.
	/// </summary>
	public class MappingGenerator
	{
		ArrayList relInfos;
		string fileName;
		NDO.Mapping.NDOMapping mapping;

		public MappingGenerator(ArrayList relInfos)
		{
			this.relInfos = relInfos;
			fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\PersistentClasses\NDOMapping.xml");
		}

		void CheckRelation(Test test, RelInfo ri)
		{
			if (!ri.HasTable)
				return;
			Class clMapping = mapping.FindClass("RelationTestClasses." + test.OwnClass.Name);
			
			if (clMapping == null)
				return;
			NDO.Mapping.Relation rel = (NDO.Mapping.Relation) clMapping.Relations.FirstOrDefault();
			Debug.Assert(rel != null, "Relation mapping not found");
			if (rel == null)
				throw new Exception("Cant find relation 0 of" + test.OwnClass.Name);

			Class derivedMapping;
			NDO.Mapping.Relation derivedRel = null;

			if (ri.OwnPoly)
			{
				derivedMapping = mapping.FindClass("RelationTestClasses." + test.OwnDerivedClass.Name);
				if (derivedMapping == null)
					return;
				derivedRel = derivedMapping.Relations.FirstOrDefault();
				if (rel == null)
					throw new Exception("Cant find relation 0 of" + test.OwnDerivedClass.Name);
			}

			if (rel.MappingTable == null || ri.OwnPoly && derivedRel.MappingTable == null)
			{
				string tableName = null;
				if (test.OwnClass.Name.CompareTo(test.OtherClass.Name) < 0)
					tableName = "rel" + test.OwnClass.Name + test.OtherClass.Name;
				else
					tableName = "rel" + test.OtherClass.Name + test.OwnClass.Name;
                ForeignKeyColumn fkColumn = rel.NewForeignKeyColumn();
				fkColumn.Name = "ID" + test.OwnClass.Name;
				if (ri.OwnPoly)
					rel.ForeignKeyTypeColumnName = "TC" + test.OwnClass.Name;
				else
					rel.ForeignKeyTypeColumnName = null;
				rel.MappingTable = new MappingTable(rel);
				rel.MappingTable.TableName = tableName;
                fkColumn = rel.MappingTable.NewForeignKeyColumn();
				fkColumn.Name = "ID" + test.OtherClass.Name;
				if (ri.OtherPoly)
					rel.MappingTable.ChildForeignKeyTypeColumnName = "TC" + test.OtherClass.Name;
				else
					rel.MappingTable.ChildForeignKeyTypeColumnName = null;
				rel.MappingTable.ConnectionId = clMapping.ConnectionId;

				if (ri.OwnPoly)
				{
					ForeignKeyColumn dfkColumn = (ForeignKeyColumn) derivedRel.ForeignKeyColumns.FirstOrDefault();
					dfkColumn.Name = ((ForeignKeyColumn) rel.ForeignKeyColumns.FirstOrDefault()).Name;
					derivedRel.ForeignKeyTypeColumnName = rel.ForeignKeyTypeColumnName;
					derivedRel.MappingTable = rel.MappingTable;
				}
			}
		}


		public void Generate()
		{
			this.mapping = new NDOMapping(fileName, NDOProviderFactory.Instance);
			foreach(RelInfo ri in relInfos)
			{
				Test test = new Test(ri, "RelationTestClasses");
				CheckRelation(test, ri);
				if (ri.IsBi)
				{
					RelInfo reverseInfo = ri.GetReverse();
					Test reverseTest = test.GetReverse();
					CheckRelation(reverseTest, reverseInfo);
				}
			}
			this.mapping.Save();
		}
	}
}