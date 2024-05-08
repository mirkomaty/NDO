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
using System.Data.Common;
using Microsoft.Extensions.Logging;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// This class supports reading and writing of relation mapping tables.
	/// </summary>
	internal class NDOMappingTableHandler : IMappingTableHandler
	{
		private Relation relation;
		private IDbCommand selectCommand;
		private IDbCommand insertCommand;
		//private IDbCommand updateCommand;
		private IDbCommand deleteCommand;
		private IDbConnection connection;
		private DbDataAdapter dataAdapter;
		private IProvider provider;
		private readonly ILogger logger;
		private readonly ILoggerFactory loggerFactory;

		public NDOMappingTableHandler(ILoggerFactory loggerFactory)
		{
			this.logger = loggerFactory.CreateLogger<NDOMappingTableHandler>();
			this.loggerFactory = loggerFactory;
		}

		private string GetParameter(IProvider provider, string name)
		{
			return provider.UseNamedParams ? provider.GetNamedParameter(name) : "?";
		}

		public void Initialize(NDOMapping mappings, Relation r)
		{
			Connection con = mappings.FindConnection(r.MappingTable.ConnectionId);
			this.provider = mappings.GetProvider(con);

			// The connection object will be initialized in the pm, to 
			// enable the callback for getting the real connection string.
			// CheckTransaction is the place, where this happens.
			this.connection = null;   

			selectCommand = provider.NewSqlCommand(connection);
			insertCommand = provider.NewSqlCommand(connection);
			deleteCommand = provider.NewSqlCommand(connection);
			dataAdapter = provider.NewDataAdapter(selectCommand, null, insertCommand, deleteCommand);
			this.relation = r;

			//
			// select
			//
			string sql = string.Format("SELECT * FROM {0} WHERE ", provider.GetQualifiedTableName(r.MappingTable.TableName));
			selectCommand.CommandText = sql;

			//
			// insert
			//
			sql = "INSERT INTO {0}({1}) VALUES ({2})";
			//{0} = TableName: Mitarbeiter			
			//{1} = FieldList: vorname, nachname
			//{2} = FieldList mit @:

			// IDOwn1, IDOwn2
            string columns = string.Empty;
            string namedParameters = string.Empty;

            new ForeignKeyIterator(r).Iterate
            (
                delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                {
                    columns += provider.GetQuotedName(fkColumn.Name);
                    namedParameters += GetParameter(provider, fkColumn.Name);
                    provider.AddParameter(insertCommand, provider.GetNamedParameter(fkColumn.Name), provider.GetDbType(fkColumn.SystemType), provider.GetDefaultLength(fkColumn.SystemType), fkColumn.Name);
                    columns += ", ";
                    namedParameters += ", ";
                }
            );


            // IDOther1, IDOther2
            new ForeignKeyIterator(r.MappingTable).Iterate
            (
                delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                {
                    columns += provider.GetQuotedName(fkColumn.Name);
                    namedParameters += GetParameter(provider, fkColumn.Name);
                    provider.AddParameter(insertCommand, provider.GetNamedParameter(fkColumn.Name), provider.GetDbType(fkColumn.SystemType), provider.GetDefaultLength(fkColumn.SystemType), fkColumn.Name);
                    if (!isLastElement)
                    {
                        columns += ", ";
                        namedParameters += ", ";
                    }
                }
            );


			// TCOwn
			if (r.ForeignKeyTypeColumnName != null) {
				columns += ", " + provider.GetQuotedName(r.ForeignKeyTypeColumnName);
				namedParameters += ", " + GetParameter(provider, r.ForeignKeyTypeColumnName);
                provider.AddParameter(insertCommand, provider.GetNamedParameter(r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), r.ForeignKeyTypeColumnName);
            }
			// TCOther
			if (r.MappingTable.ChildForeignKeyTypeColumnName != null) {
				columns += ", " + provider.GetQuotedName(r.MappingTable.ChildForeignKeyTypeColumnName);
				namedParameters += ", " + GetParameter(provider, r.MappingTable.ChildForeignKeyTypeColumnName);
                provider.AddParameter(insertCommand, provider.GetNamedParameter(r.MappingTable.ChildForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), r.MappingTable.ChildForeignKeyTypeColumnName);
            }
			insertCommand.CommandText = string.Format(sql, provider.GetQualifiedTableName(r.MappingTable.TableName), columns, namedParameters);

			Class relatedClass = mappings.FindClass(r.ReferencedTypeName);

			// 
			// delete
			// 
			sql = "DELETE FROM {0} WHERE ({1})";
			//{0} = Tabellenname: Mitarbeiter
			//{1} = Where-Bedingung: id = @Original_id
            string where = string.Empty;
            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    where += provider.GetQuotedName(fkColumn.Name) + " = " + GetParameter(provider, "Original_" + fkColumn.Name) + " AND ";
                    provider.AddParameter(deleteCommand, provider.GetNamedParameter("Original_" + fkColumn.Name), provider.GetDbType(fkColumn.SystemType), fkColumn.Size, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), fkColumn.Name, System.Data.DataRowVersion.Original, null);
                }
            );
            new ForeignKeyIterator(r.MappingTable).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    where += provider.GetQuotedName(fkColumn.Name) + " = " + GetParameter(provider, "Original_" + fkColumn.Name);
                    if (!isLastIndex)
                        where += " AND ";
                    provider.AddParameter(deleteCommand, provider.GetNamedParameter("Original_" + fkColumn.Name), provider.GetDbType(fkColumn.SystemType), fkColumn.Size, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), fkColumn.Name, System.Data.DataRowVersion.Original, null);
                }
            );

			if (r.ForeignKeyTypeColumnName != null) 
            {
				where += " AND " + provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = " + GetParameter(provider, "Original_" + r.ForeignKeyTypeColumnName);
                provider.AddParameter(deleteCommand, provider.GetNamedParameter("Original_" + r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), 4, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), r.ForeignKeyTypeColumnName, System.Data.DataRowVersion.Original, null);
            }
			if (r.MappingTable.ChildForeignKeyTypeColumnName != null) 
            {
				where += " AND " + provider.GetQuotedName(r.MappingTable.ChildForeignKeyTypeColumnName) + " = " + GetParameter(provider, "Original_" + r.MappingTable.ChildForeignKeyTypeColumnName);
                provider.AddParameter(deleteCommand, provider.GetNamedParameter("Original_" + r.MappingTable.ChildForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), 4, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), r.MappingTable.ChildForeignKeyTypeColumnName, System.Data.DataRowVersion.Original, null);
            }

            deleteCommand.CommandText = string.Format(sql, provider.GetQualifiedTableName(r.MappingTable.TableName), where);

		}

		private void Dump(DataRow[] rows)
		{
			new SqlDumper(this.loggerFactory, this.provider, insertCommand, selectCommand, null, deleteCommand).Dump(rows);
		}

		private DataTable GetTableTemplate(DataSet templateDataset, string name)
		{
			DataTable dt = templateDataset.Tables[name];
			if (dt == null)
				throw new NDOException(24, "Can't find mapping table '" + name + "' in the schema. Check your mapping file.");
			return dt;
		}

		
		public DataTable FindRelatedObjects(ObjectId oid, DataSet templateDataset) 
		{
			DataTable table = GetTableTemplate(templateDataset, relation.MappingTable.TableName).Clone();
            string sql = "SELECT * FROM " + provider.GetQualifiedTableName(relation.MappingTable.TableName) + " WHERE ";
            selectCommand.Parameters.Clear();

            int i = 0;
            new ForeignKeyIterator(relation).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastEntry)
            {                    
                string parName = "p" + i;
                sql += provider.GetQuotedName(fkColumn.Name) + " = " + provider.GetNamedParameter(parName);
                IDataParameter dataParameter = provider.AddParameter(selectCommand, provider.GetNamedParameter(parName), provider.GetDbType(fkColumn.SystemType), fkColumn.Size, parName);
				object o = oid.Id[i];
				if ( o is Guid && !provider.SupportsNativeGuidType )
					o = ((Guid) o).ToString();
                dataParameter.Value = o;
                dataParameter.Direction = ParameterDirection.Input;
                if (!isLastEntry)
                    sql += " AND ";
                i++;
            });
            
			if (relation.ForeignKeyTypeColumnName != null) 
			{
                //sql += " AND " + provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = " + oid.Id.TypeId;
                sql += " AND " + provider.GetQuotedName(relation.ForeignKeyTypeColumnName) + " = " + provider.GetNamedParameter("typeCode");
                IDataParameter dataParameter = provider.AddParameter(selectCommand, provider.GetNamedParameter("typeCode"), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), "typeCode");
                dataParameter.Value = oid.Id.TypeId;
                dataParameter.Direction = ParameterDirection.Input;                
			}

			selectCommand.CommandText = sql;
            try 
            {
				Dump(null);
				dataAdapter.Fill(table);
            }
            catch (System.Exception ex)
            {
                string text = "Exception in dataAdapter.Fill: " + ex.Message + "\n";
                text += "Sql-Anweisung: " + selectCommand.CommandText + "\n";
                throw new NDOException(25, text);
            }

			return table;		
		}

		public void Update(DataSet ds) 
		{
			DataTable dt = ds.Tables[relation.MappingTable.TableName];
			try 
			{
				DataRow[] rows = dt.Select(null, null, DataViewRowState.Added 
					| DataViewRowState.ModifiedCurrent 
					| DataViewRowState.Deleted);

				if (rows.Length > 0)
				{
					Dump(rows);
					dataAdapter.Update(dt);
				}
			}
			catch (System.Exception ex) 
			{
				throw new NDOException(26, "Exception in dataAdapter.Update: " + ex.Message + "\n");
			}
		}

		public void Dispose()
		{
		}

		public IDbConnection Connection
		{
			get { return this.selectCommand.Connection; }
			set
			{
				this.selectCommand.Connection = value;
				this.deleteCommand.Connection = value;
				this.insertCommand.Connection = value;
			}
		}
	
		public IDbTransaction Transaction
		{
			get { return this.selectCommand.Transaction; }
			set
			{
				this.selectCommand.Transaction = value;
				this.deleteCommand.Transaction = value;
				this.insertCommand.Transaction = value;
			}
		}

		public Relation Relation
		{
			get { return this.relation; }
		}
	}
}
