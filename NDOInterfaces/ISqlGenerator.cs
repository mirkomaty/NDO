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

namespace NDOInterfaces
{
	/// <summary>
	/// Determines, where the primary key constraint is located in the DDL code.
	/// </summary>
	public enum PrimaryConstraintPlacement
	{
		/// <summary>
		/// PK constraint is located at the end of a column definition.
		/// </summary>
		InColumn,
		/// <summary>
		/// PK constraint is located like a row in the table definition. 
		/// This is standard and the case for most databases.
		/// </summary>
		InTable,
		/// <summary>
		/// PK constraint is located after the table definition. This can be used
		/// to create the constraint with ALTER TABLE statements.
		/// </summary>
		AfterTable
	}

	/// <summary>
	/// This interface is used to write DDL language providers.
	/// </summary>
	public interface ISqlGenerator
	{
		/// <summary>
		/// Gets the name of the NDO provider, which should be used to generate the table and column names.
		/// </summary>
		string ProviderName
		{
			get ;
		}

		/// <summary>
		/// This is used by the enhancer to get and set the ndo provider, the generator belongs to.
		/// </summary>
		IProvider Provider
		{
			get; set;
		}

		/// <summary>
		/// Sets the message adapter object, where warnings and error messages are written to.
		/// </summary>
		/// <param name="messages">The message adapter object.</param>
		void SetMessageAdapter(IMessageAdapter messages);

		/// <summary>
		/// Statements to delete a table before it is newly created.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>A DDL string</returns>
		string DropTable(string tableName);

		/// <summary>
		/// Any statements necessary, before a table description begins. 
		/// </summary>
		/// <param name="connectionString">Connection string, corresponding to the class, 
		/// for which a table is to be constructed.</param>
		/// <returns>A DDL string.</returns>
		string ConnectToDatabase(string connectionString);

		/// <summary>
		/// Code to start a table description.
		/// </summary>
		/// <param name="tableName">Qualified name of the table.</param>
		/// <returns>A DDL string.</returns>
		/// <remarks>After this code the column descriptions are placed.</remarks>
		string BeginnTable(string tableName);

		/// <summary>
		/// Code to end a table description.
		/// </summary>
		/// <param name="tableName">Qualified name of the table.</param>
		/// <returns>A DDL string.</returns>
		string EndTable(string tableName);

		/// <summary>
		/// Determines, where the primary key constraint must be placed. 
		/// </summary>
		PrimaryConstraintPlacement PrimaryConstraintPlacement
		{
			get ;
		}

		/// <summary>
		/// Determines, if a field length may be placed in the DDL.
		/// </summary>
		/// <param name="t">Sytem.Type for which the column is to be prepared.</param>
		/// <returns>True, if a field length can be placed in the DDL.</returns>
		bool LengthAllowed(Type t);

		/// <summary>
		/// Determines, if a field length may be placed in the DDL.
		/// </summary>
		/// <param name="dbType">A database specific column type name.</param>
		/// <returns>True, if a field length can be placed in the DDL.</returns>
		bool LengthAllowed(string dbType);


		/// <summary>
		/// Maps a certain System.Type to a database specific column type
		/// </summary>
		/// <param name="t">The System.Type to convert.</param>
		/// <returns>A type string</returns>
		string DbTypeFromType(Type t);

		/// <summary>
		/// DDL for auto increment columns.
		/// </summary>
		/// <returns>A DDL string</returns>
		string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width);

		/// <summary>
		/// DDL for primary key columns, which are not autoincremented.
		/// </summary>
		/// <remarks>
		/// A DDL generator should implement this function, 
		/// if PrimaryConstraintPlacement has the value InColumn.
		/// </remarks>
		string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width);

		/// <summary>
		/// True, if the column DDL syntax of a primary key column is different
		/// to the DDL of an ordinary column.
		/// </summary>
		bool HasSpecialAutoIncrementColumnFormat
		{
			get ;
		}

		/// <summary>
		/// Creates the DDL for creating primary key constraints.
		/// </summary>
		/// <param name="primaryKeyColumns">All columns, building the primary key.</param>
		/// <param name="constraintName">Name of the constraint.</param>
		/// <param name="tableName">Qualified name of the table.</param>
		/// <returns>A DDL string.</returns>
		string CreatePrimaryKeyConstraint(DataColumn[] primaryKeyColumns, string constraintName, string tableName);

		/// <summary>
		/// Creates the DDL for creating foreign key constraints.
		/// </summary>
		/// <param name="sourceColumns">Colums, building the foreign Key in the current table.</param>
		/// <param name="relatedColumns">Primary key columns, to which the source columns relate.</param>
		/// <param name="constraintName">Name of the constraint.</param>
		/// <param name="relatedTableName">Name of the related table.</param>
		/// <returns>A DDL string.</returns>
		string CreateForeignKeyConstraint(DataColumn[] sourceColumns, DataColumn[] relatedColumns, string constraintName, string relatedTableName);


		/// <summary>
		/// Writes an expression, which determines, whether a column allows NULL values.
		/// </summary>
		/// <param name="allowNull">If true, the column should allow null values, otherwise not.</param>
		/// <returns>A DDL string.</returns>
		string NullExpression(bool allowNull);


		/// <summary>
		/// Creates an index in the database.
		/// </summary>
		/// <param name="indexName">Name of the index.</param>
		/// <param name="tableName">Name of the owning table.</param>
		/// <param name="indexColums">An array of DataColumn objects which denotes the columns to be indexed.</param>
		/// <returns></returns>
		string CreateIndex(string indexName, string tableName, DataColumn[] indexColums);

		/// <summary>
		/// Creates the part of a Alter Table statement, which adds a column. NDO will add the type
		/// and Null/Non Null expressions. 
		/// </summary>
		/// <returns>A DDL string.</returns>
		/// <remarks>
		/// Here is an Expample:
		/// <code>
		/// -- The next line will be generated by NDO
		/// ALTER Table myTable
		/// -- The next line should be generated by AddColumn
		/// ADD COLUMN
		/// -- The next line will be generated by NDO (it's exacly a column definition).
		/// myColumn INTEGER NOT NULL;
		/// </code>
		/// </remarks>
		string AddColumn();

		/// <summary>
		/// Creates the part of a Alter Table statement, which drops a column.
		/// </summary>
		/// <param name="colName">Name of the column to be dropped.</param>
		/// <returns>A DDL string.</returns>
		/// <remarks>
		/// Here is an Expample:
		/// <code>
		/// -- The next line will be generated by NDO
		/// ALTER Table myTable
		/// -- The next line should be generated by RemoveColumn
		/// DROP myColumn
		/// -- The next line will be generated by NDO
		/// ;
		/// </code>
		/// </remarks>
		string RemoveColumn(string colName);

		/// <summary>
		/// Creates the part of a Alter Table statement, which renames a column.		
		/// </summary>
		/// <param name="tableName">Name table to be altered.</param>
		/// <param name="oldName">Name of the column to be renamed.</param>
		/// <param name="newName">New name of the column.</param>
		/// <param name="typeName">An expression like 'CHAR(200) NOT NULL'.</param>
		/// <returns>A DDL string.</returns>
		/// <remarks>
		/// Here is an Expample:
		/// <code>
		/// -- All lines should be generated by RenameColumn
		/// ALTER Table myTable
		///   ALTER oldName TO newName
		/// </code>
		/// If RenameColumn returns an empty string, NDO tries to synthesize the rename function 
		/// with a sequence of ADD COLUMN, UPDATE and DROP COLUMN statements.
		/// </remarks>
		string RenameColumn(string tableName, string oldName, string newName, string typeName);

		/// <summary>
		/// Creates the part of a Alter Table statement, which changes a column type. NDO will add the type
		/// and Null/Non Null expressions. 
		/// </summary>
		/// <returns>A DDL string.</returns>
		/// <remarks>
		/// Here is an Expample:
		/// <code>
		/// -- The next line will be generated by NDO
		/// ALTER Table myTable
		/// -- The next line should be generated by AlterColumnType
		/// ALTER COLUMN 
		/// -- The next line will be generated by NDO (it's exacly a column definition)
		/// myColumn nvarchar(255) NOT NULL;
		/// </code>
		/// </remarks>
		string AlterColumnType();


	}
}
