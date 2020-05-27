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
using System.Linq;
using System.IO;
using NDO;
using NDO.Mapping;
using Reisekosten.Personal;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Data.SqlClient;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung för PersistenceManagerFactory.
	/// </summary>
	public class PmFactory
	{
		static PmFactory()
		{
			//PersistenceManager pm = NewPersistenceManager();
			//pm.TransactionMode = TransactionMode.None;
			//var ndoConn = pm.NDOMapping.Connections.First();
			//if (ndoConn.Type == "SqlServer")
			//{
			//	using (SqlConnection conn = new SqlConnection( "Integrated Security=SSPI;Persist Security Info=False;Data Source=localhost" ))
			//	{
			//		numOfPoolConnectionsCounter = new PerformanceCounter();
			//		numOfPoolConnectionsCounter.CategoryName = ".NET-Datenanbieter für SqlServer";
			//		numOfPoolConnectionsCounter.CounterName = "NumberOfPooledConnections";
			//		var categories = PerformanceCounterCategory.GetCategories();
			//		var categoryNames = categories.Select(c=>c.CategoryName);
			//		PerformanceCounterCategory category = categories.FirstOrDefault(c => c.CategoryName == numOfPoolConnectionsCounter.CategoryName);
			//		var instanceName = category.GetInstanceNames().FirstOrDefault(n=>n.IndexOf(GetCurrentProcessId().ToString()) > -1);
			//		if (instanceName != null)
			//			numOfPoolConnectionsCounter.InstanceName = instanceName;
			//		numOfPoolConnectionsCounter.NextValue();  // Zähler starten.
			//	}
			//}
			//else
			//{
			//	numOfPoolConnectionsCounter = null;
			//}
		}


		[DllImport( "kernel32.dll", SetLastError = true )]
		static extern int GetCurrentProcessId();

		private static string GetAssemblyName()
		{
			string result = null;

			// First try GetEntryAssembly name, then AppDomain.FriendlyName.
			Assembly assembly = Assembly.GetEntryAssembly();

			if (null != assembly)
			{
				AssemblyName name = assembly.GetName();
				if (name != null)
				{
					result = name.Name; // MDAC 73469
				}
			}
			return result;
		}

		public static PersistenceManager NewPersistenceManager()
		{
			//string path = Path.Combine( Path.GetDirectoryName( typeof( PmFactory ).Assembly.Location ), "NDOMapping.xml" );
            string path = @"C:\Projekte\NDO\IntegrationTests\NDOUnitTests\bin\Debug\NDOMapping.xml";
            PersistenceManager pm = new PersistenceManager(path);
			pm.TransactionMode = TransactionMode.None;
            //PersistenceManager pm = new PersistenceManager();
//			pm.LogPath = Path.GetDirectoryName(path);
//			pm.RegisterConnectionListener(new OpenConnectionListener(ConnectionGenerator.OnConnection));
			
//			pm.TransactionMode = TransactionMode.Pessimistic;
			Connection conn = (Connection)pm.NDOMapping.Connections.First();
#if ORACLE || FIREBIRD || POSTGRE
			pm.IdGenerationEvent += new NDO.IdGenerationHandler(IdGenerator.OnIdGenerationEvent);
			IdGenerator.ConnectionString = ((Connection)pm.NDOMapping.Connections.First()).Name;
#endif
			pm.VerboseMode = false;
			return pm;
		}
	}
}
