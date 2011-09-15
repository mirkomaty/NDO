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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO;
using NDOInterfaces;

namespace NDO.SqliteProvider
{
	public class SqliteGenerator : AbstractSQLGenerator
	{

		public SqliteGenerator()
		{			
		}

		public override string ProviderName
		{
			get { return "Sqlite"; }
		}

		public override string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";
		}


		public override bool LengthAllowed(Type t)
		{
			return true;
		}

		public override bool LengthAllowed(string dbType)
		{
			return true;
		}

		public override string DbTypeFromType(Type t)
		{
			return NDO.SqliteProvider.Provider.GetInternalDbType( t ).ToString();
#if MaskedOut
            if (t == typeof(bool))
                return("Int2");
            else if (t == typeof(Byte))
				return ( "Int2" );
			else if ( t == typeof( Byte[] ) )
                return "Blob";
            else if (t == typeof(DateTime))
				return "Datetime";
			else if (t == typeof(decimal))
				return "Decimal";
			else if (t == typeof(double))
				return "Real";
			else if (t.IsEnum)
				return "Integer";
			else if (t == typeof(float))
				return "Real";
			else if (t == typeof(Guid))
				return "Text";
			else if (t == typeof(Int16) || t == typeof(UInt16))
				return "Int2";
			else if (t == typeof(Int32) || t == typeof(UInt32))
				return "Integer";
			else if ( t == typeof( Int64 ) || t == typeof( UInt64 ) )
				return "Numeric";
			else if ( t == typeof( string ) )
				return "Text";
			throw new Exception("Can't resolve type " + t.FullName + " as storable.");
#endif
		}

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " Integer PRIMARY KEY AUTOINCREMENT";
		}

		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}

        public override PrimaryConstraintPlacement PrimaryConstraintPlacement
        {
            get { return PrimaryConstraintPlacement.InColumn; }
        }

        public override string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
        {
			string coldef = columnName + " " + columnType;
            if (dataType == typeof(string))
                coldef += '(' + width + ')'; 
            return coldef + " PRIMARY KEY";
        }

	}

}
