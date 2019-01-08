//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Text;

namespace NDOInterfaces
{
	/// <summary>
	/// Zusammenfassung für AbstractSQLGenerator.
	/// </summary>
	public abstract class AbstractSQLGenerator : ISqlGenerator
	{
		/// <summary>
		/// The NDO provider used to quote names.
		/// </summary>
		private IProvider provider;

		/// <summary>
		/// This property is used by the enhancer to get and set the ndo provider, 
		/// the generator belongs to. The provider is used to quote table and column names.
		/// </summary>
		public virtual IProvider Provider
		{
			get { return provider; }
			set { provider = value; }
		}

		/// <summary>
		/// Error messages go there.
		/// </summary>
		protected IMessageAdapter messages;

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public AbstractSQLGenerator()
		{
		}

		#region ISqlGenerator Member

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		public abstract string ProviderName { get; }

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="messages"></param>
		public virtual void SetMessageAdapter(IMessageAdapter messages)
		{
			this.messages = messages;
		}
		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";			
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public virtual string ConnectToDatabase(string connectionString)
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string BeginnTable(string tableName)
		{			
			return "CREATE TABLE " + tableName + "(";
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string EndTable(string tableName)
		{
			return ");";
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		public virtual NDOInterfaces.PrimaryConstraintPlacement PrimaryConstraintPlacement
		{
			get { return PrimaryConstraintPlacement.InTable; }
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public abstract bool LengthAllowed(Type t);

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public abstract bool LengthAllowed(string dbType);

		/// <summary>
		/// Converts a System.Type to default DbType value.
		/// </summary>
		/// <param name="t">The type to convert.</param>
		/// <returns>A string representation of the DbType.</returns>
		/// <remarks>
		/// Concrete providers should override this function to provide DbType names, 
		/// which can be used in the DDL code of the underlying database.
		/// </remarks>
		public abstract string DbTypeFromType(Type t);

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="dataType"></param>
		/// <param name="columnType"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public virtual string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="dataType"></param>
		/// <param name="columnType"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public virtual string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		public virtual bool HasSpecialAutoIncrementColumnFormat
		{
			get { return false; }
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="primaryKeyColumns"></param>
		/// <param name="constraintName"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string CreatePrimaryKeyConstraint(System.Data.DataColumn[] primaryKeyColumns, string constraintName, string tableName)
		{
			StringBuilder sb = new StringBuilder("CONSTRAINT ");
			sb.Append(constraintName);
			sb.Append(" PRIMARY KEY ");
			GenerateColumnList(primaryKeyColumns, sb);
			return sb.ToString();
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="sourceColumns"></param>
		/// <param name="relatedColumns"></param>
		/// <param name="constraintName"></param>
		/// <param name="relatedTableName"></param>
		/// <returns></returns>
		public virtual string CreateForeignKeyConstraint(DataColumn[] sourceColumns, DataColumn[] relatedColumns, string constraintName, string relatedTableName)
		{
			StringBuilder sb = new StringBuilder("CONSTRAINT ");
			sb.Append(constraintName);
			sb.Append(" FOREIGN KEY ");
			GenerateColumnList(sourceColumns, sb);
			sb.Append(" REFERENCES ");
			sb.Append(relatedTableName);
			GenerateColumnList(relatedColumns, sb);
			return sb.ToString();
		}


		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="allowNull"></param>
		/// <returns></returns>
		public virtual string NullExpression(bool allowNull)
		{
			if (allowNull)
				return "NULL";
			else
				return "NOT NULL";
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="sb"></param>
		void GenerateColumnList(DataColumn[] columns, StringBuilder sb)
		{
			sb.Append("(");
			int lastIndex = columns.Length - 1;
			for (int i = 0; i < columns.Length; i++)
			{
				DataColumn dc = columns[i];
				sb.Append(provider.GetQuotedName(dc.ColumnName));
				if (i < lastIndex)
					sb.Append(",");
			}
			sb.Append(")");
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// </summary>
		/// <param name="indexName"></param>
		/// <param name="tableName"></param>
		/// <param name="indexColums"></param>
		/// <returns></returns>
		public virtual string CreateIndex(string indexName, string tableName, DataColumn[] indexColums)
		{
			StringBuilder sb = new StringBuilder("CREATE INDEX ");
			sb.Append(indexName);
			sb.Append(" ON ");
			sb.Append(tableName);
			GenerateColumnList(indexColums, sb);
			return sb.ToString();
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// Override this function, if dropping a column needs another syntax than 
		/// 'ADD column_definition'.
		/// </summary>
		/// <returns></returns>
		public virtual string AddColumn()
		{
			return "ADD";
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// Override this function, if dropping a column needs another syntax than 
		/// 'DROP COLUMN ColName'
		/// </summary>
		/// <param name="colName"></param>
		/// <returns></returns>
		public virtual string RemoveColumn(string colName)
		{
			return "DROP COLUMN " + colName;
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// Override this function, if renaming a column needs another syntax than 
		/// a sequence of ADD COLUMN, UPDATE and DROP COLUMN.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="oldName"></param>
		/// <param name="newName"></param>
		/// <param name="typeName"></param>
		/// <returns>This default implementaton returns an empty string (see remarks).</returns>
		/// <remarks>
		/// If RenameColumn returns an empty string, NDO tries to synthesize the rename function 
		/// with a sequence of ADD COLUMN, UPDATE and DROP COLUMN.
		/// </remarks>
		public virtual string RenameColumn(string tableName, string oldName, string newName, string typeName)
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="ISqlGenerator">ISqlGenerator interface</see>.
		/// Override this function, if altering a column type needs another syntax than 
		/// 'ALTER column_definition'
		/// </summary>
		/// <returns></returns>
		public virtual string AlterColumnType()
		{
			return "ALTER";
		}

		#endregion
	}
}
