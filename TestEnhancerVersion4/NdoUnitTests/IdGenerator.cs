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
using System.Data.OracleClient;
using NDO;
using NDO.Mapping;
using PureBusinessClasses;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung f�r IdGenerator.
	/// </summary>
	public class IdGenerator
	{
		static string schemaName = "SCOTT";
		static string sequenceName = "NDOSequence";
		static int increment = 10;
		static int currentId = 0;
		static int upperLimit;
		static string connectionString;

		/// <summary>
		/// Sets the Connection String which is used to fetch the sequence data
		/// </summary>
		public static string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}


		public static void OnIdGenerationEvent(Type t, ObjectId oid)
		{
			if (t.FullName.StartsWith ("NDOObjectIdTestClasses."))
				return;  // Use the other handler
            OidColumn oidColumn = (OidColumn)PmFactory.NewPersistenceManager().NDOMapping.FindClass(t).Oid.OidColumns[0];
			if (oidColumn.SystemType == typeof(int))
				oid.Id[0] = NextId();
			else if (oidColumn.SystemType == typeof(Guid))
				oid.Id[0] = Guid.NewGuid();
		}

		/// <summary>
		/// Get the next id from the database
		/// </summary>
		/// <returns>the id as int value</returns>
		public static int NextId()
		{
			if (currentId == 0)
				Requery();
			if (currentId >= upperLimit)
				Requery();
			return currentId++;
		}

		private static void Requery()
		{
#if persistentIds
#if ORACLE
			string seq = "\"" + schemaName + "\".\"" + sequenceName + "\"";
			string sql = "select " + seq + ".Nextval from dual";
			OracleConnection conn = new OracleConnection(connectionString);
			OracleCommand cmd = new OracleCommand(sql, conn);
			conn.Open();
			OracleDataReader reader = cmd.ExecuteReader();
			if (!reader.Read())
				throw new Exception("Can't read oracle sequence " + seq);
			currentId = reader.GetInt32(0);			
			conn.Close();
			upperLimit = currentId + increment;
#endif
#if FIREBIRD
			//SET GENERATOR "NDOGenerator" TO 0;
			PersistenceManager pm = PmFactory.NewPersistenceManager();
			Query q = pm.NewQuery(typeof(SequenceValue), "SELECT GEN_ID(\"NDOGenerator\", " + increment.ToString() + ") AS \"Value\" FROM RDB$DATABASE;", false, Query.Language.Sql); 
			SequenceValue sv = (SequenceValue) q.ExecuteSingle();
			currentId = sv.Value;
			upperLimit = sv.Value + increment;
#endif
#else
            currentId = 1;
            upperLimit = 0x7fffffff;
#endif
		}

		
		/// <summary>
		/// Execute this function once to create the ndo sequence
		/// </summary>
		public static void CreateSequence()
		{

			string sql = @"CREATE SEQUENCE """ + schemaName + @""".""" + sequenceName + @""" 
INCREMENT BY " + increment.ToString() + @" START WITH 
1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE 
CACHE 20 NOORDER";
			OracleConnection conn = new OracleConnection(connectionString);
			OracleCommand cmd = new OracleCommand(sql, conn);
			conn.Open();
			cmd.ExecuteNonQuery();
			conn.Close();
		}

	}
}
