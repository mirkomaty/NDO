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
using STDBusinessClasses;
using NUnit.Framework;
using NDO;
using NDO.Mapping;

namespace TestSTD
{
	/// <summary>
	/// Zusammenfassung fï¿½r TestAbort.
	/// </summary>
	[TestFixture]
	public class TestAbort
	{
		PersistenceManager pm = new PersistenceManager();

		[SetUp]
		public void SetUp()
		{
			pm = new PersistenceManager();
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestClassCount()
		{
			string strclass = "Class";
			NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
			int anz = 10 - mapping.Classes.Count;
			for (int i = 0; i < anz; i++)
			{
				mapping.AddStandardClass(strclass + i.ToString(), "MyAss");
			}
			Assert.AreEqual("Class count wrong", 10, mapping.Classes.Count);
			System.Diagnostics.Debug.WriteLine("10 Classes in list");
			mapping.AddStandardClass("Class11", "ShitAssembly");
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestConnectionCount()
		{
			string strclass = "Class";
			NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
			int anz = 1 - mapping.Connections.Count;			
			for (int i = 0; i < anz; i++)
			{
				mapping.Connections.Add(mapping.NewConnection("nix", "garnix"));
			}
			Assert.AreEqual("Connection count wrong", 1, mapping.Connections.Count);
			System.Diagnostics.Debug.WriteLine("1 Connection in list");
			mapping.Connections.Add(mapping.NewConnection("zweimalnix", "UndSchonï¿½berhauptNix"));
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestObjectCount()
		{
			Mitarbeiter m;
			for(int i = 0; i < 500; i++)
			{
				m = new Mitarbeiter();
				pm.MakePersistent(m);
			}
			System.Diagnostics.Debug.WriteLine("500 Objects");
			m = new Mitarbeiter();
			pm.MakePersistent(m); // Should give an exception
		}


		[Test, ExpectedException(typeof(NotImplementedException))]
		public void Abort()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = "Mirko";
			pm.MakePersistent(m);
			pm.Abort();
			System.Diagnostics.Debug.WriteLine(m.NDOObjectState);
		}
	
	}
}
