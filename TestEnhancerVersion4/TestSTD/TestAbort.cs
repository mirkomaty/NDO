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
using STDBusinessClasses;
using NUnit.Framework;
using NDO;
using NDO.Mapping;

namespace TestSTD
{
	/// <summary>
	/// Zusammenfassung f�r TestAbort.
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
			mapping.Connections.Add(mapping.NewConnection("zweimalnix", "UndSchon�berhauptNix"));
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
