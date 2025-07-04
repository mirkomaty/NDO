﻿//
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
using System.Diagnostics;
using System.Collections;
using NUnit.Framework;
using NDO;
using DateTest;
using NDO.Query;
using NDO.ProviderFactory;

namespace NdoUnitTests
{
	[TestFixture]
	public class DateTest : NDOTest
	{
		
		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			NDOQuery<DateTestClass> q = new NDOQuery<DateTestClass>(pm, null);
			IList l = q.Execute();
			pm.Delete(l);
			pm.Save();
			pm.Close();
		}

		[Test]
		public void TestCreated()
		{
			var pm = PmFactory.NewPersistenceManager();

			// dt = new DateTime(2004, 10, 12, 13, 30, 31, 123);
			DateTestClass dtc = new DateTestClass();
			InnerDate id = new InnerDate();
			id.SetInnerDate();
			pm.MakePersistent(id);
			pm.MakePersistent(dtc);
			pm.Save();
			dtc.InnerDate = id;
			dtc.Name = "Test";			
			pm.Save();
			pm.UnloadCache();

			NDOQuery<DateTestClass> q = new NDOQuery<DateTestClass>(pm, null);
			dtc = (DateTestClass) q.ExecuteSingle(true);
			Assert.That(2002 ==  dtc.InnerDate.Dt.Year, "DateTime konnte nicht richtig gelesen werden");
		}

		[Test]
		public void TestPersistent()
		{
			var pm = PmFactory.NewPersistenceManager();

			InnerDate id = new InnerDate();
			pm.MakePersistent(id);
			pm.Save();
			pm.UnloadCache();
			id.SetInnerDate();
			ObjectHelper.MarkDirty(id);
			Assert.That(NDOObjectState.PersistentDirty ==  id.NDOObjectState, "Status falsch");
			pm.Save();
		}

        [Test]
        public void ProviderTest()
        {
			var pm = PmFactory.NewPersistenceManager();

			Trace.WriteLine("Provider: " + pm.NDOMapping.GetProvider(typeof(Reisekosten.Personal.Mitarbeiter)).GetType().Assembly.FullName);
            Trace.WriteLine("Available Providers:");
            foreach (string s in NDOProviderFactory.Instance.ProviderNames)
                Trace.WriteLine("  " + s);
        }

	}
}
