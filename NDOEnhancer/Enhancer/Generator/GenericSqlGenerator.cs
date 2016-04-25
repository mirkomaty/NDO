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
using System.Data;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für GenericSqlGenerator.
	/// </summary>
	internal class GenericSqlGenerator : GenericSqlGeneratorBase
	{

		public GenericSqlGenerator(ISqlGenerator concreteGenerator, MessageAdapter messages, NDO.Mapping.NDOMapping mappings) : base(concreteGenerator, messages, mappings)
		{
		}

		public void Generate(DataSet dsSchema, DataSet dsOld, System.IO.StreamWriter stream, TypeManager typeManager, bool generateConstraints)
		{
			IProvider provider = NDOProviderFactory.Instance[concreteGenerator.ProviderName];

			if ( dsOld != null )
			{
				foreach ( DataTable dt in dsOld.Tables )
				{
					if ( generateConstraints )
					{
						for ( int i = 0; i < dt.ParentRelations.Count; i++ )
						{
							DataRelation dr = dt.ParentRelations[i];
							stream.Write("ALTER TABLE ");
							stream.Write(QualifiedTableName.Get(dt.TableName, provider));
							stream.Write(" DROP CONSTRAINT ");
							stream.Write(provider.GetQuotedName(dr.RelationName));
							stream.WriteLine(";");
						}
					}
				}
				foreach ( DataTable dt in dsOld.Tables )
				{
					stream.WriteLine( concreteGenerator.DropTable( QualifiedTableName.Get(dt.TableName, provider) ) );
				}
				if (dsOld.Tables.Count > 0)
					stream.WriteLine();
			}


			foreach (DataTable dt in dsSchema.Tables)
				stream.Write(CreateTable(dt));


			if ( generateConstraints )
			{
				foreach ( DataTable dt in dsSchema.Tables )
				{
					for ( int i = 0; i < dt.ParentRelations.Count; i++ )
					{
						DataRelation dr = dt.ParentRelations[i];
						stream.Write("ALTER TABLE ");
						stream.Write(QualifiedTableName.Get(dt.TableName, provider));
						stream.Write(" ADD ");
						stream.WriteLine(concreteGenerator.CreateForeignKeyConstraint( dr.ChildColumns, dr.ParentColumns, 
							provider.GetQuotedName( dr.RelationName ), QualifiedTableName.Get(dr.ParentTable.TableName, provider) ) + ";" );
					}
				}
			}

            if (typeManager != null && typeManager.Entries.Length > 0)
            {
                DataTable typeTable = new DataTable("NDOTypeCodes");
                DataColumn pkColumn = typeTable.Columns.Add("TypeCode", typeof (int));
                typeTable.Columns.Add("TypeName", typeof(string));
                typeTable.Columns.Add("AssemblyName", typeof(string));
                typeTable.Columns.Add("TableName", typeof(string));
                typeTable.PrimaryKey = new DataColumn[] { pkColumn };
                stream.Write(CreateTable(typeTable));

                Class[] descriptors = typeManager.Entries;
                int l = descriptors.Length;
		        for (int i = 0; i < l; i++)
                {
                    Class cl = descriptors[i];
                    stream.Write("INSERT INTO NDOTypeCodes (TypeCode, TypeName, AssemblyName, TableName) VALUES (");
                    stream.Write(cl.TypeCode);
                    stream.Write(",");
                    stream.Write(provider.GetSqlLiteral(cl.FullName));
                    stream.Write(",");
                    stream.Write(provider.GetSqlLiteral(cl.AssemblyName));
                    stream.Write(",");
                    stream.Write(provider.GetSqlLiteral(cl.TableName));
                    stream.WriteLine(");");
                }

            }
#if noindex
			foreach (DataTable dt in dsSchema.Tables)
			for (int i = 0; i < dt.ParentRelations.Count; i++)
			{
				DataRelation dr = dt.ParentRelations[i];
				string index = concreteGenerator.CreateIndex(provider.GetQuotedName("Ix" + dr.RelationName), QualifiedTableName.Get(dt.TableName, provider), dr.ChildColumns);
				if (index != string.Empty)
				{
					stream.Write(index);
					stream.WriteLine(";");
				}
			}
			foreach(DataTable dt in dsSchema.Tables)
			{
				if (GetClassForTable(dt.TableName, mappings) == null) // mappingTable
				{
					int i = 1;
					string tname = dt.TableName;
					tname = tname.Substring(tname.IndexOf('.') + 1);
					if (tname.StartsWith("rel"))
						tname = tname.Substring(3);
					tname = "Ix" + tname;
					foreach(DataColumn dc in dt.Columns)
					{
						string index = concreteGenerator.CreateIndex(provider.GetQuotedName(tname + i.ToString()), QualifiedTableName.Get(dt.TableName, provider), new DataColumn[] {dc});
						if (index != string.Empty)
						{
							stream.Write(index);
							stream.WriteLine(";");
							i++;
						}
					}
				}
			}
#endif
		}

	}
}
