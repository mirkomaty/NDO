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
using System.Linq;
using System.IO;
using NDO;
using NDO.Mapping;
using Reisekosten.Personal;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung f�r PersistenceManagerFactory.
	/// </summary>
	public class PmFactory
	{
		public static PersistenceManager NewPersistenceManager()
		{
            string path = @"D:\Projekte\NDO22\TestEnhancerVersion4\NDOUnitTests\bin\Debug\NDOMapping.xml";
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
