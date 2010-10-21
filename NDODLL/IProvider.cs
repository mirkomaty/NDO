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


#if nix
using System;
using System.Data;
using System.Data.Common;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// This interface creates objects for a specific ADO.NET data provider.
	/// Implement this interface if a special data provider is needed.
	/// NDO supplies implementations of IProvider for Microsoft SQL Server, Access and Oracle. 
	/// The provider should be registered with the NDOProviderFactory.
	/// </summary>
	public interface IProvider
	{
		/// <summary>
		/// Factory method for new connection objects.
		/// </summary>
		/// <param name="parameters">Connection string</param>
		/// <returns>An ADO.NET connection object</returns>
		IDbConnection NewConnection(string parameters);

		/// <summary>
		/// Factory method for new command objects. Commands will be assinged to the given connection.
		/// </summary>
		/// <param name="connection">The connection over which the command will be executed</param>
		/// <returns>An ADO.NET command object</returns>
		IDbCommand NewSqlCommand(IDbConnection connection);

		/// <summary>
		/// Factory method for new data adapter objects.
		/// </summary>
		/// <param name="select">Command object for the SELECT statement</param>
		/// <param name="update">Command object for the UPDATE statement</param>
		/// <param name="insert">Command object for the INSERT statement</param>
		/// <param name="delete">Command object for the DELETE statement</param>
		/// <returns>An ADO.NET data adapter</returns>
		DbDataAdapter NewDataAdapter(IDbCommand select, IDbCommand update, IDbCommand insert, IDbCommand delete);

		/// <summary>
		/// Factory method for new command builder objects.
		/// </summary>
		/// <param name="dataAdapter">The data adapter, which contains the SELECT command, from which all other commands will be constructed.</param>
		/// <returns>A newly constructed CommandBuilder object</returns>
		/// <remarks>This function is not used by the framework. It is only used by the mapping tool.</remarks>
		object NewCommandBuilder(DbDataAdapter dataAdapter);

		/// <summary>
		/// This is a generic wrapper for the AddParameter methods of specialized ADO.NET providers.
		/// Adds parameters to a given command object. The parameter types depend on the ADO.NET provider in use.
		/// </summary>
		/// <param name="command">Command object for which parameters are to be assigned.</param>
		/// <param name="parameterName">Parameter name</param>
		/// <param name="dbType">Parameter type code - value depends on the ADO.NET provider in use</param>
		/// <param name="size">Parameter size. This value will be interpreted by the database.</param>
		/// <param name="columnName">Name of the column, for which the command applies</param>
		void AddParameter(IDbCommand command, string parameterName, object dbType, int size, string columnName);
		/// <summary>
		/// This is a generic wrapper for the AddParameter methods of specialized ADO.NET providers.
		/// Adds parameters to a given command object. The parameter types depend on the ADO.NET provider in use.
		/// </summary>
		/// <param name="command">Command object for which parameters are to be assigned.</param>
		/// <param name="parameterName">Parameter name</param>
		/// <param name="dbType">Parameter type code - value depends on the ADO.NET provider in use</param>
		/// <param name="size">Parameter size. This value will be interpreted by the database.</param>
		/// <param name="dir">A value indicating whether the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.</param>
		/// <param name="isNullable">Indicates whether the parameter accepts null values.</param>
		/// <param name="precision">The maximum number of digits used to represent the Value property.</param>
		/// <param name="scale">The number of decimal places to which Value is resolved.</param>
		/// <param name="srcColumn">The name of the source column that is mapped to the DataSet and used for loading or returning the Value.</param>
		/// <param name="srcVersion">The DataRowVersion to use when loading Value.</param>
		/// <param name="value">The value of the parameter.</param>
		void AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value);

		/// <summary>
		/// Maps a given System.Type to a type code which represents the type in the database.
		/// </summary>
		/// <param name="t">A Type object.</param>
		/// <returns>A DbType value representing the given type.</returns>
		object GetDbType(Type t);

		/// <summary>
		/// Maps a type code name to the type code, which represents the type in the database. 
		/// Typically the type code names are the string representation of the enum name of the DbType value.
		/// If the DbType enum contains a member Rawbyte, the String representation would be "Rawbyte".
		/// GetDbType would return a MyDatabaseType.Rawbyte for the String "Rawbyte".
		/// </summary>
		/// <param name="dbTypeName">Type code name</param>
		/// <returns>The DbType code</returns>
		object GetDbType(string dbTypeName);

		/// <summary>
		/// Gets a usefull length for a given type, i.e. the value 4 for the type code name "Integer".
		/// </summary>
		/// <param name="dbTypeName">Type code name</param>
		/// <returns>The default lengt for a given type.</returns>
		int GetDefaultLength(string dbTypeName);
		/// <summary>
		/// Gets a usefull length for a given type, i.e. the value 4 for the type System.Int32.
		/// </summary>
		/// <param name="t">A Type object</param>
		/// <returns></returns>
		int GetDefaultLength(Type t);

		/// <summary>
		/// Maps a database specific type name to a System.Type object.
		/// </summary>
		/// <param name="dbTypeName">String representation of the database type.</param>
		/// <returns>A type object.</returns>
		Type GetSystemType(string dbTypeName);


		/// <summary>
		/// Gets the wildcard character, i.e. '%' for SqlServer or '*' for Microsoft Access.
		/// </summary>
		string Wildcard {get;}

		/// <summary>
		/// Indicates whether the last automatically generated ID can be retrieved. 
		/// Returns true if a database provides automatically incremented IDs and its syntax has an expression 
		/// which retrieves the last generated ID; otherwise false.
		/// </summary>
		bool SupportsLastInsertedId {get;}

		/// <summary>
		/// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
		/// inserted row, if the ID is automatically generated by the database.
		/// </summary>
		string GetLastInsertedId {get; }


		/// <summary>
		/// Indicates whether a database accepts named parameters in parameter lists. 
		/// If this value is false, NDO will use unnamed standard SQL parameter lists.
		/// </summary>
		bool UseNamedParams {get;}


		/// <summary>
		/// Convert a plain name to a parameter name, i.e. the string "id" will be converted
		/// to ":id" for oracle or "@id" for Sql Server.
		/// </summary>
		/// <param name="plainParameterName">The string to be converted</param>
		/// <returns>The converted name.</returns>
		string GetNamedParameter(string plainParameterName);

		/// <summary>
		/// Converts a plain name to a quoted name, if the database supports quoted names.
		/// Most databases use their own quoting characters to distinguish names and keywords. 
		/// Implementers of IProvider should make shure that the name to convert is not yet converted.
		/// </summary>
		/// <param name="plainName"></param>
		/// <returns></returns>
		string GetQuotedName(string plainName);


		/// <summary>
		/// This property is for future use and is always set to false.
		/// </summary>
		bool UseStoredProcedure {get;}

		/// <summary>
		/// Converts a value to a string representation which can be used in SQL commands 
		/// for a given database.
		/// </summary>
		/// <param name="o">The object to be converted</param>
		/// <returns></returns>
		string GetSqlLiteral(object o);


		/// <summary>
		/// Determines whether a database supports bulk command strings.
		/// </summary>
		bool SupportsBulkCommands {get;}
		

		/// <summary>
		/// Generate one big command string out of several partial commands to save roundtrips
		/// to the server.
		/// </summary>
		/// <param name="commands"></param>
		/// <returns></returns>
		string GenerateBulkCommand(string[] commands);

		/// <summary>
		/// Gets the maximum length of a bulk command if such commands are supported.
		/// </summary>
		int MaxBulkCommandLength {get;}


		/// <summary>
		/// Returns string representation of all possible database specific data types
		/// </summary>
		string[] TypeNames { get; }

		/// <summary>
		/// Returns the name of all tables of a given connection
		/// </summary>
		/// <param name="conn">The connection</param>
		/// <returns>The table names</returns>
		string[] GetTableNames (IDbConnection conn);

		/// <summary>
		/// Returns the name of all tables of a given connection
		/// </summary>
		/// <param name="conn">The connection</param>
		/// <param name="owner">Database owner name</param>
		/// <returns>The table names</returns>
		string[] GetTableNames (IDbConnection conn, string owner);


		/// <summary>
		/// Gets the name of the provider. This name will be used as index for the ProviderFactory
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Determines, if the provider accepts a batch of an insert command an a select
		/// command to retrieve autonumbered ids.
		/// </summary>
		bool SupportsInsertBatch { get; }


		/// <summary>
		/// This is needed, if a provider supports retrieving of the last autonumber id
		/// but doesn't support insert batches. In the RowUpdateHandler NDO can execute a separate statement
		/// to retrieve the id. The standard implementation in NDOAbstractProvider is empty.
		/// </summary>
		/// <param name="handler"></param>
		void RegisterRowUpdateHandler(IRowUpdateListener handler);

		/// <summary>
		/// True if the Database has a native Guid datatype.
		/// The guid type is represented as string type in the schema, with an additional attribute msdata:DataType.
		/// If the database doesn't support Guids the additional attribute has to be deleted before the first database operation.
		/// Thus the Guid will be stored and read as string from the database. 
		/// </summary>
		bool SupportsNativeGuidType { get; }

	}
}
#endif