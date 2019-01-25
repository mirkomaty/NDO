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

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung för PersistenceManagerFactory.
	/// </summary>
	public class PmFactory
	{
		public static PersistenceManager NewPersistenceManager()
		{
			string path = Path.Combine( Path.GetDirectoryName( typeof( PmFactory ).Assembly.Location ), "NDOMapping.xml" );
            //string path = @"C:\Projekte\NDO\TestEnhancerVersion4\NDOUnitTests\bin\Debug\NDOMapping.xml";
            PersistenceManager pm = new PersistenceManager(path);
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
