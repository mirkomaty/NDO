#if nix // We should use the abstract NDO classes in order to compute the sequence values
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
using System.Data.OracleClient;
using NDO;
using NDO.Mapping;
using PureBusinessClasses;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung för IdGenerator.
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
			string seq = /* "\"" + schemaName + "\".*/  \"" + sequenceName + "\"";
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
			IQuery q = new NDOQuery<SequenceValue), "SELECT GEN_ID(\"NDOGenerator\", " + increment.ToString() + ") AS \"Value\" FROM RDB$DATABASE;", false, QueryLanguage.Sql); 
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
#endif