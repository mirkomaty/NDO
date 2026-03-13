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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO;
using NDOInterfaces;

namespace MySqlProvider
{
	public class MySqlGenerator : AbstractSQLGenerator
	{

		public MySqlGenerator()
		{			
		}

		public override string ProviderName
		{
			get { return "MySql"; }
		}

		public override string DropTable(string tableName)
		{
			return "DROP TABLE IF EXISTS " + tableName + ";";
		}

		public override string ConnectToDatabase(string connectionString)
		{
			Regex regex = new Regex(@"Database\s*=\s*([^;]+)");
			Match match = regex.Match(connectionString);
			if (match.Success)
			{
				return("USE " + match.Groups[1] + ";");
			}
			return string.Empty;
		}



		public override bool LengthAllowed(Type t)
		{
			return t == typeof(string) || t == typeof(decimal);
		}

		public override bool LengthAllowed(string dbType)
		{
			return (string.Compare(dbType, "varchar", true) == 0 || string.Compare(dbType, "decimal", true) == 0);
		}

		public override string DbTypeFromType(Type t, int size)
		{
			if (t == typeof( bool ) || t == typeof( Byte ))
				return "Tinyint";
			else if (t == typeof( DateTime ))
				return "Datetime";
			else if (t == typeof( decimal ))
				return "Decimal";
			else if (t == typeof( double ))
				return "Double";
			else if (t.IsEnum)
				return "Int";
			else if (t == typeof( float ))
				return "Float";
			else if (t == typeof( Guid ))
				return "Varchar";
			else if (t == typeof( Int16 ) || t == typeof( UInt16 ))
				return "Smallint";
			else if (t == typeof( Int32 ) || t == typeof( UInt32 ))
				return "Int";
			else if (t == typeof( Int64 ) || t == typeof( UInt64 ))
				return "BigInt";
			else if (t == typeof( string ) && size == -1)
				return "TEXT";
			else if (t == typeof( string ))
				return "Varchar";
			else if (t == typeof( byte[] ))
				return "MediumBlob";
            throw new Exception("Can't resolve type " + t.FullName + " as storable.");
		}

		public override PrimaryConstraintPlacement PrimaryConstraintPlacement => PrimaryConstraintPlacement.InColumn;

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width, bool isPrimary)
		{
			string primary = isPrimary ? " PRIMARY KEY" : "";
			return $"{columnName} Int AUTO_INCREMENT{primary}";
		}

		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}

		public override string RemoveColumn(string colName)
		{
			return "DROP " + colName;
		}

		//ALTER TABLE EMPLOYEE CHANGE old_col_name new_col_name char(20) (mysql)
		public override string RenameColumn(string tableName, string oldName, string newName, string typeName)
		{
			return "ALTER TABLE " + tableName + " CHANGE " + oldName + ' ' + newName + ' ' + typeName;
		}

		public override string AlterColumnType()
		{
			return "MODIFY";
		}
	}

}
