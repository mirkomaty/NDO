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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	internal class MySqlGenerator : AbstractSQLGenerator
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

		public override string DbTypeFromType(Type t)
		{
			if (t == typeof(bool) || t == typeof(Byte))
				return "Tinyint";
			else if (t == typeof(DateTime))
				return "Datetime";
			else if (t == typeof(decimal))
				return "Decimal";
			else if (t == typeof(double))
				return "Double";
			else if (t.IsEnum)
				return "Int";
			else if (t == typeof(float))
				return "Float";
			else if (t == typeof(Guid))
				return "Varchar";
			else if (t == typeof(Int16) || t == typeof(UInt16))
				return "Smallint";
			else if (t == typeof(Int32) || t == typeof(UInt32))
				return "Int";
			else if (t == typeof(Int64) || t == typeof(UInt64))
				return "BigInt";
			else if (t == typeof(string))
				return "Varchar";
			throw new Exception("Can't resolve type " + t.FullName + " as storable.");
/*
 * Der Provider wurde bereits auf MySql.Data.dll umgeschrieben,
 * als der Generator geschrieben wurde. Man konnte nicht mehr
 * die richtigen ByteFX-Typen zuordnen.
			object dbt = provider.GetDbType(t);
			Type dbtType = dbt.GetType();
			string fullName = dbtType.FullName;
			string nameSpace = fullName.Substring(0, fullName.LastIndexOf("."));
			object par = dbtType.Assembly.CreateInstance(nameSpace + "." + "MySqlParameter", false, BindingFlags.CreateInstance, null, new object[]{"@test", dbt}, null, null);
			FieldInfo fi = par.GetType().GetField("dbType", BindingFlags.NonPublic | BindingFlags.Instance);
			string parType = fi.GetValue(par).ToString();	

			return parType;
			*/
		}

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " Int AUTO_INCREMENT";
		}

		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}

		public string CreateConstraint(DataColumn[] primaryKeyColumns, string constraintName, string tableName)
		{
			StringBuilder sb = new StringBuilder("CONSTRAINT ");
			sb.Append(constraintName);
			sb.Append(" PRIMARY KEY (");
			int lastIndex = primaryKeyColumns.Length - 1;
			for (int i = 0; i < primaryKeyColumns.Length; i++)
			{
				DataColumn dc = primaryKeyColumns[i];
				sb.Append(Provider.GetQuotedName(dc.ColumnName));
				if (i < lastIndex)
					sb.Append(",");
			}
			sb.Append(")");
			return sb.ToString();
		}

	}

}
#endif