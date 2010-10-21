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

                NDOTypeDescriptor[] descriptors = typeManager.Entries;
                int l = descriptors.Length;
		        for (int i = 0; i < l; i++)
                {
                    NDOTypeDescriptor td = descriptors[i];
                    stream.Write("INSERT INTO NDOTypeCodes (TypeCode, TypeName, AssemblyName, TableName) VALUES (");
                    stream.Write(td.TypeId);
                    stream.Write(",");
                    stream.Write(provider.GetSqlLiteral(td.TypeName));
                    stream.Write(",");
                    stream.Write(provider.GetSqlLiteral(td.AssemblyName));
                    stream.Write(",");
                    Class cl = mappings.FindClass(td.TypeName);
                    if (cl == null)
                        throw new Exception("Can't find class mapping for type " + td.TypeName);
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
