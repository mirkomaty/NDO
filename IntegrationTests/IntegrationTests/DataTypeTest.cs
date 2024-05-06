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
using System.Reflection;
using System.Collections;
using DataTypeTestClasses;
using NDO;
using NDO.Mapping;
using NUnit.Framework;
using NDO.Query;
using System.Linq;

namespace NdoUnitTests
{
	[TestFixture] 
	public class DataTypeTest
	{
		[SetUp]
		public void Setup() 
		{
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();

			IList l = pm.GetClassExtent(typeof(DataContainer), true);
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}
			l = pm.GetClassExtent( typeof( DataContainerDerived ), true );
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}
			l = pm.GetClassExtent( typeof( VtAndEtContainer ), true );
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}
			l = pm.GetClassExtent( typeof( VtAndEtContainerDerived ), true );
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}
			l = pm.GetClassExtent( typeof( NullableDataContainer ), true );
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}
			l = pm.GetClassExtent( typeof( NullableDataContainerDerived ), true );
			if (l.Count > 0)
			{
				pm.Delete( l );
				pm.Save();
			}

			pm.Close();
		}

		[Test]
		public void TestDataContainer()
		{
			DataContainer dc = new DataContainer();

			dc.Init();

			var pm = PmFactory.NewPersistenceManager();

			pm.MakePersistent(dc);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(DataContainer));
			Assert.AreEqual(1, l.Count, "Ein Objekt sollte in der Liste sein");
			dc = (DataContainer) l[0];
			AssertDataContainer(dc);
		}

        [Test]
        public void TestNullableDataContainer()
        {
            NullableDataContainer ndc = new NullableDataContainer();
            ndc.Init();

			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(ndc);
            pm.Save();
            pm.UnloadCache();
            ndc = new NDOQuery<NullableDataContainer>(pm, null, false).ExecuteSingle(true);
            AssertNullableDataContainer(ndc);
        }


        [Test]
        public void TestNullableDataContainerDerived()
        {
            NullableDataContainerDerived ndc = new NullableDataContainerDerived();
            ndc.Init();

			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(ndc);
            pm.Save();
            pm.UnloadCache();
            ndc = new NDOQuery<NullableDataContainerDerived>(pm, null, false).ExecuteSingle(true);
            AssertNullableDataContainer(ndc);
        }

		private bool FieldWithNameExists(Class cl, string name)
		{
			foreach(Field field in cl.Fields)
				if (field.Name.IndexOf(name) > -1)
					return true;
			return false;
		}

		//public void TestDataContainerEvent()
		//{
		//	Type t = typeof(DataContainer.QueryHelper);

		//	Class cl = pm.NDOMapping.FindClass(typeof(DataContainer));
		//	Assert.Null(cl.FindField("TestEvent"), "Events should not be mapped");

		//	Assert.NotNull(t.GetMember("int16Var", BindingFlags.Instance), "Variables should appear in qh");
		//	PropertyInfo pi = t.GetProperty("TestEvent");
		//	Assert.Null(pi, "Events should not appear in qh");

		//	//----
		//	t = typeof(DataContainerDerived.QueryHelper);

		//	cl = pm.NDOMapping.FindClass(typeof(DataContainerDerived));
		//	Assert.That(!FieldWithNameExists(cl, "TestEvent"), "Events should not be mapped");

		//	Assert.NotNull(t.GetMember("int16Var", BindingFlags.Instance), "Variables should appear in qh");
		//	pi = t.GetProperty("TestEvent");
		//	Assert.Null(pi, "Events should not appear in qh");

		//	//----
		//	t = typeof(VtAndEtContainer.QueryHelper);

		//	cl = pm.NDOMapping.FindClass(typeof(VtAndEtContainer));
		//	Assert.That(!FieldWithNameExists(cl, "TestEvent"), "Events should not be mapped");

		//	Assert.NotNull(t.GetMember("int16Var", BindingFlags.Instance), "Variables should appear in qh");
		//	pi = t.GetProperty("TestEvent");
		//	Assert.Null(pi, "Events should not appear in qh");

		//	//----
		//	t = typeof(VtAndEtContainerDerived.QueryHelper);

		//	cl = pm.NDOMapping.FindClass(typeof(VtAndEtContainerDerived));
		//	Assert.That(!FieldWithNameExists(cl, "TestEvent"), "Events should not be mapped");

		//	Assert.NotNull(t.GetMember("int16Var", BindingFlags.Instance), "Variables should appear in qh");
		//	pi = t.GetProperty("TestEvent");
		//	Assert.Null(pi, "Events should not appear in qh");

		//}

		[Test]
		public void TestDataContainerDerived()
		{			
			DataContainerDerived dcd = new DataContainerDerived();
			dcd.Init();
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(dcd);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(DataContainerDerived));
			Assert.AreEqual(1, l.Count, "Ein Objekt sollte in der Liste sein");
			dcd = (DataContainerDerived) l[0];
			AssertDataContainer(dcd);
		}

		bool CheckDouble( double d1, double d2 )
		{
			double eps = Math.Abs(d1 * 1E-6);
			return (d1 - eps) <= d2 && d2 <= (d1 + eps);
		}

		bool CheckFloat( float d1, float d2 )
		{
			double eps = Math.Abs( d1 * 1E-6 );
			return (d1 - eps) <= d2 && d2 <= (d1 + eps);
		}

		void AssertDataContainer(DataContainer dc)
		{
			Assert.AreEqual(true, dc.BoolVar, "BoolVar falsc h");
			Assert.AreEqual(127, dc.ByteVar, "ByteVar falsch");
			Assert.AreEqual("Test", dc.StringVar, "StringVar falsch");
			Assert.AreEqual(DateTime.Now.Date, dc.DateTimeVar.Date, "DateTimeVar falsch");
			Assert.AreEqual(new Guid("12341234-1234-1234-1234-123412341234"), dc.GuidVar, "GuidVar falsch");
			Assert.AreEqual(1231.12m, dc.DecVar, "DecVar falsch");
			Assert.AreEqual(int.MaxValue, dc.Int32Var, "Int32Var falsch");
			Assert.AreEqual((uint) int.MaxValue, dc.Uint32Var, "UInt32Var falsch");
#if SQLITE
			Assert.That(CheckDouble(1E28, dc.DoubleVar), "DoubleVar falsch");
			Assert.That(CheckFloat(1E14F, dc.FloatVar), "FloatVar falsch");
#else
			Assert.AreEqual(1E28, dc.DoubleVar, "DoubleVar falsch");
			Assert.AreEqual(1E14F, dc.FloatVar, "FloatVar falsch");
#endif
			Assert.AreEqual(0x1ffffffff, dc.Int64Var, "Int64Var falsch");
			Assert.AreEqual(short.MaxValue, dc.Int16Var, "Int16Var falsch");
			Assert.AreEqual((ushort) short.MaxValue, dc.Uint16Var, "UInt16Var falsch");
			Assert.AreEqual(0x1ffffffff, dc.Uint64Var, "UInt64Var falsch");
			Assert.AreEqual(EnumType.drei, dc.EnumVar, "EnumType falsch");
			Assert.AreEqual(Guid.Empty, dc.EmptyGuidVar, "Empty Guid falsch");
			Assert.AreEqual(DateTime.MinValue, dc.EmptyDateTimeVar, "Empty DateTime falsch");
			Assert.Null(dc.NullString, "Empty String falsch");
		}


        void AssertNullableDataContainer(NullableDataContainer dc)
        {
            Assert.AreEqual(true, dc.BoolVar, "BoolVar falsch");
            Assert.AreEqual(127, dc.ByteVar, "ByteVar falsch");
            Assert.AreEqual(DateTime.Now.Date, dc.DateTimeVar.Value.Date, "DateTimeVar falsch");
            Assert.AreEqual(new Guid("12341234-1234-1234-1234-123412341234"), dc.GuidVar, "GuidVar falsch");
            Assert.AreEqual(1231.12m, dc.DecVar, "DecVar falsch");
            Assert.AreEqual(int.MaxValue, dc.Int32Var, "Int32Var falsch");
            Assert.AreEqual((uint)int.MaxValue, dc.Uint32Var, "UInt32Var falsch");
#if SQLITE
            Assert.That(CheckDouble(1E28, dc.DoubleVar.Value), "DoubleVar falsch");
            Assert.That(CheckFloat(1E14F, dc.FloatVar.Value), "FloatVar falsch");
#else
            Assert.AreEqual(1E28, dc.DoubleVar.Value, "DoubleVar falsch");
            Assert.AreEqual(1E14F, dc.FloatVar.Value, "FloatVar falsch");
#endif
			Assert.AreEqual(0x1ffffffff, dc.Int64Var, "Int64Var falsch");
            Assert.AreEqual(short.MaxValue, dc.Int16Var, "Int16Var falsch");
            Assert.AreEqual((ushort)short.MaxValue, dc.Uint16Var, "UInt16Var falsch");
            Assert.AreEqual(0x1ffffffff, dc.Uint64Var, "UInt64Var falsch");
            Assert.AreEqual(EnumType.drei, dc.EnumVar, "EnumType falsch");

#if !ACCESS  // Access initializes bool vars always with False
            Assert.That(!dc.BoolEmptyVar.HasValue, "BoolEmptyVar falsch");
#endif
            Assert.That(!dc.ByteEmptyVar.HasValue, "ByteVar falsch");
            Assert.That(!dc.DateTimeEmptyVar.HasValue, "DateTimeVar falsch");
            Assert.That(!dc.GuidEmptyVar.HasValue, "GuidVar falsch");
            Assert.That(!dc.DecEmptyVar.HasValue, "DecVar falsch");
            Assert.That(!dc.Int32EmptyVar.HasValue, "Int32Var falsch");
            Assert.That(!dc.Uint32EmptyVar.HasValue, "UInt32Var falsch");
            Assert.That(!dc.DoubleEmptyVar.HasValue, "DoubleVar falsch");
            Assert.That(!dc.FloatEmptyVar.HasValue, "FloatVar falsch");
            Assert.That(!dc.Int64EmptyVar.HasValue, "Int64Var falsch");
            Assert.That(!dc.Int16EmptyVar.HasValue, "Int16Var falsch");
            Assert.That(!dc.Uint16EmptyVar.HasValue, "UInt16Var falsch");
            Assert.That(!dc.Uint64EmptyVar.HasValue, "UInt64Var falsch");
            Assert.That(!dc.EnumEmptyVar.HasValue, "EnumType falsch");

        }

		[Test]
		public void QueryWithEmptyGuidParameterSearchesForNull()
		{
			var pm = PmFactory.NewPersistenceManager();

			DataContainer dc = new DataContainer();
			dc.Init();
			dc.GuidVar = Guid.Empty;
			pm.MakePersistent( dc );
			pm.Save();
			pm.UnloadCache();

			var q = new NDOQuery<DataContainer>(pm, "guidVar = {0}");
			q.Parameters.Add( Guid.Empty );
			var list = q.Execute();
			Assert.AreEqual( Guid.Empty, list.First().GuidVar );
		}

		[Test]
		public void QueryWithDateTimeMinValueParameterSearchesForNull()
		{
			var pm = PmFactory.NewPersistenceManager();

			DataContainer dc = new DataContainer();

			dc.Init();
			dc.DateTimeVar = DateTime.MinValue;
			pm.MakePersistent( dc );
			pm.Save();
			pm.UnloadCache();

			var q = new NDOQuery<DataContainer>(pm, "dateTimeVar = {0}");
			q.Parameters.Add( DateTime.MinValue );
			var list = q.Execute();
			Assert.AreEqual( DateTime.MinValue, list.First().DateTimeVar );
		}


		[Test]
		public void ParametersTest()
		{
			var pm = PmFactory.NewPersistenceManager();

			DataContainer dc = new DataContainer();
			dc.Init();
			pm.MakePersistent(dc);
			pm.Save();
			IQuery q = new NDOQuery<DataContainer>(pm, "boolVar" + " = " + "{0}");
#if !ORACLE
			q.Parameters.Add(true);
#else
			q.Parameters.Add(1);
#endif
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.
			
			q = new NDOQuery<DataContainer>(pm, "byteVar" + " = " + "{0}");
			q.Parameters.Add((byte)127);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "dateTimeVar" + " <= " + "{0}");
			q.Parameters.Add(DateTime.Now);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "guidVar" + " = " + "{0}");
			q.Parameters.Add(new Guid("12341234-1234-1234-1234-123412341234"));
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "decVar" + " = " + "{0}");
			q.Parameters.Add(1231.12m);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "doubleVar" + " = " + "{0}");
			q.Parameters.Add(1E28);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "enumVar" + " = " + "{0}");
			q.Parameters.Add(EnumType.drei);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

#if !MYSQL
			q = new NDOQuery<DataContainer>(pm, "floatVar" + " = " + "{0}");
			q.Parameters.Add(1E14F);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.
#endif
			q = new NDOQuery<DataContainer>(pm, "int16Var" + " = " + "{0}");
			q.Parameters.Add(short.MaxValue);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "int32Var" + " = " + "{0}");
			q.Parameters.Add(int.MaxValue);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "int64Var" + " = " + "{0}");
			q.Parameters.Add(0x1ffffffff);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "stringVar" + " = " + "{0}");
			q.Parameters.Add("Test");
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "uint16Var" + " = " + "{0}");
			q.Parameters.Add((ushort) short.MaxValue);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "uint32Var" + " = " + "{0}");
			q.Parameters.Add((uint) int.MaxValue);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			q = new NDOQuery<DataContainer>(pm, "uint64Var" + " = " + "{0}");
			q.Parameters.Add(0x1ffffffff);
			dc = (DataContainer) q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.

			dc.StringVar = "";
			pm.Save();

#if ORACLE
			q = new NDOQuery<DataContainer>(pm, "stringVar" + Query.Op.IsNull);
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.
#else
			q = new NDOQuery<DataContainer>(pm, "stringVar" + " = " + "{0}");
			q.Parameters.Add("");
			q.ExecuteSingle(true);  // If something goes wrong, an Exception will be thrown.
#endif

		}


#if nix
		[Test]
		public void TestQueryHelpers()
		{			
			Assert.AreEqual("propValType.ByteVar", "propValType.ByteVar", "ByteVar falsch #1");
			Assert.AreEqual("propValType.BoolVar",  "propValType.BoolVar", "BoolVar falsch #1");
			Assert.AreEqual("propValType.DecVar", "propValType.DecVar", "DecVar falsch #1");
			Assert.AreEqual("propValType.DateTimeVar", "propValType.DateTimeVar", "DateTimeVar falsch #1");
			Assert.AreEqual("propValType.DoubleVar", "propValType.DoubleVar", "DoubleVar falsch #1");
			Assert.AreEqual("propValType.FloatVar", "propValType.FloatVar", "FloatVar falsch #1");
			Assert.AreEqual("propValType.GuidVar", "propValType.GuidVar", "GuidVar falsch #1");
			Assert.AreEqual("propValType.Int16Var", "propValType.Int16Var", "Int16Var falsch #1");
			Assert.AreEqual("propValType.Int32Var", "propValType.Int32Var", "Int32Var falsch #1");
			Assert.AreEqual("propValType.Int64Var", "propValType.Int64Var", "Int64Var falsch #1");
			Assert.AreEqual("propValType.StringVar", "propValType.StringVar", "StringVar falsch #1");
			Assert.AreEqual("propValType.Uint16Var", "propValType.Uint16Var", "Uint16Var falsch #1");
			Assert.AreEqual("propValType.Uint32Var", "propValType.Uint32Var", "Uint32Var falsch #1");
			Assert.AreEqual("propValType.Uint64Var", "propValType.Uint64Var", "Uint64Var falsch #1");
			Assert.AreEqual("propValType.EnumVar", "propValType.EnumVar", "EnumVar falsch #1");

			Assert.AreEqual("pubValType.ByteVar", "pubValType.ByteVar", "ByteVar falsch #2");
			Assert.AreEqual("pubValType.BoolVar",  "pubValType.BoolVar", "BoolVar falsch #2");
			Assert.AreEqual("pubValType.DecVar", "pubValType.DecVar", "DecVar falsch #2");
			Assert.AreEqual("pubValType.DateTimeVar", "pubValType.DateTimeVar", "DateTimeVar falsch #2");
			Assert.AreEqual("pubValType.DoubleVar", "pubValType.DoubleVar", "DoubleVar falsch #2");
			Assert.AreEqual("pubValType.FloatVar", "pubValType.FloatVar", "FloatVar falsch #2");
			Assert.AreEqual("pubValType.GuidVar", "pubValType.GuidVar", "GuidVar falsch #2");
			Assert.AreEqual("pubValType.Int16Var", "pubValType.Int16Var", "Int16Var falsch #2");
			Assert.AreEqual("pubValType.Int32Var", "pubValType.Int32Var", "Int32Var falsch #2");
			Assert.AreEqual("pubValType.Int64Var", "pubValType.Int64Var", "Int64Var falsch #2");
			Assert.AreEqual("pubValType.StringVar", "pubValType.StringVar", "StringVar falsch #2");
			Assert.AreEqual("pubValType.Uint16Var", "pubValType.Uint16Var", "Uint16Var falsch #2");
			Assert.AreEqual("pubValType.Uint32Var", "pubValType.Uint32Var", "Uint32Var falsch #2");
			Assert.AreEqual("pubValType.Uint64Var", "pubValType.Uint64Var", "Uint64Var falsch #2");
			Assert.AreEqual("pubValType.EnumVar", "pubValType.EnumVar", "EnumVar falsch #2");

			Assert.AreEqual("embeddedType.byteVar", "embeddedType.byteVar", "ByteVar falsch #3");
			Assert.AreEqual("embeddedType.boolVar",  "embeddedType.boolVar", "BoolVar falsch #3");
			Assert.AreEqual("embeddedType.decVar", "embeddedType.decVar", "DecVar falsch #3");
			Assert.AreEqual("embeddedType.dateTimeVar", "embeddedType.dateTimeVar", "DateTimeVar falsch #3");
			Assert.AreEqual("embeddedType.doubleVar", "embeddedType.doubleVar", "DoubleVar falsch #3");
			Assert.AreEqual("embeddedType.floatVar", "embeddedType.floatVar", "FloatVar falsch #3");
			Assert.AreEqual("embeddedType.guidVar", "embeddedType.guidVar", "GuidVar falsch #3");
			Assert.AreEqual("embeddedType.int16Var", "embeddedType.int16Var", "Int16Var falsch #3");
			Assert.AreEqual("embeddedType.int32Var", "embeddedType.int32Var", "Int32Var falsch #3");
			Assert.AreEqual("embeddedType.int64Var", "embeddedType.int64Var", "Int64Var falsch #3");
			Assert.AreEqual("embeddedType.stringVar", "embeddedType.stringVar", "StringVar falsch #3");
			Assert.AreEqual("embeddedType.uint16Var", "embeddedType.uint16Var", "Uint16Var falsch #3");
			Assert.AreEqual("embeddedType.uint32Var", "embeddedType.uint32Var", "Uint32Var falsch #3");
			Assert.AreEqual("embeddedType.uint64Var", "embeddedType.uint64Var", "Uint64Var falsch #3");
			Assert.AreEqual("embeddedType.enumVar", "embeddedType.enumVar", "Uint64Var falsch #3");

			VtAndEtContainerDerived.QueryHelper "" = new VtAndEtContainerDerived.QueryHelper();
			
			Assert.AreEqual("propValType.ByteVar", ".propValType.ByteVar", "ByteVar falsch #1");
			Assert.AreEqual("propValType.BoolVar",  ".propValType.BoolVar", "BoolVar falsch #1");
			Assert.AreEqual("propValType.DecVar", ".propValType.DecVar", "DecVar falsch #1");
			Assert.AreEqual("propValType.DateTimeVar", ".propValType.DateTimeVar", "DateTimeVar falsch #1");
			Assert.AreEqual("propValType.DoubleVar", ".propValType.DoubleVar", "DoubleVar falsch #1");
			Assert.AreEqual("propValType.FloatVar", ".propValType.FloatVar", "FloatVar falsch #1");
			Assert.AreEqual("propValType.GuidVar", ".propValType.GuidVar", "GuidVar falsch #1");
			Assert.AreEqual("propValType.Int16Var", ".propValType.Int16Var", "Int16Var falsch #1");
			Assert.AreEqual("propValType.Int32Var", ".propValType.Int32Var", "Int32Var falsch #1");
			Assert.AreEqual("propValType.Int64Var", ".propValType.Int64Var", "Int64Var falsch #1");
			Assert.AreEqual("propValType.StringVar", ".propValType.StringVar", "StringVar falsch #1");
			Assert.AreEqual("propValType.Uint16Var", ".propValType.Uint16Var", "Uint16Var falsch #1");
			Assert.AreEqual("propValType.Uint32Var", ".propValType.Uint32Var", "Uint32Var falsch #1");
			Assert.AreEqual("propValType.Uint64Var", ".propValType.Uint64Var", "Uint64Var falsch #1");
			Assert.AreEqual("propValType.EnumVar", ".propValType.EnumVar", "EnumVar falsch #1");

			Assert.AreEqual("pubValType.ByteVar", ".pubValType.ByteVar", "ByteVar falsch #2");
			Assert.AreEqual("pubValType.BoolVar",  ".pubValType.BoolVar", "BoolVar falsch #2");
			Assert.AreEqual("pubValType.DecVar", ".pubValType.DecVar", "DecVar falsch #2");
			Assert.AreEqual("pubValType.DateTimeVar", ".pubValType.DateTimeVar", "DateTimeVar falsch #2");
			Assert.AreEqual("pubValType.DoubleVar", ".pubValType.DoubleVar", "DoubleVar falsch #2");
			Assert.AreEqual("pubValType.FloatVar", ".pubValType.FloatVar", "FloatVar falsch #2");
			Assert.AreEqual("pubValType.GuidVar", ".pubValType.GuidVar", "GuidVar falsch #2");
			Assert.AreEqual("pubValType.Int16Var", ".pubValType.Int16Var", "Int16Var falsch #2");
			Assert.AreEqual("pubValType.Int32Var", ".pubValType.Int32Var", "Int32Var falsch #2");
			Assert.AreEqual("pubValType.Int64Var", ".pubValType.Int64Var", "Int64Var falsch #2");
			Assert.AreEqual("pubValType.StringVar", ".pubValType.StringVar", "StringVar falsch #2");
			Assert.AreEqual("pubValType.Uint16Var", ".pubValType.Uint16Var", "Uint16Var falsch #2");
			Assert.AreEqual("pubValType.Uint32Var", ".pubValType.Uint32Var", "Uint32Var falsch #2");
			Assert.AreEqual("pubValType.Uint64Var", ".pubValType.Uint64Var", "Uint64Var falsch #2");
			Assert.AreEqual("pubValType.EnumVar", ".pubValType.EnumVar", "EnumVar falsch #2");

			Assert.AreEqual("embeddedType.byteVar", ".embeddedType.byteVar", "ByteVar falsch #3");
			Assert.AreEqual("embeddedType.boolVar",  ".embeddedType.boolVar", "BoolVar falsch #3");
			Assert.AreEqual("embeddedType.decVar", ".embeddedType.decVar", "DecVar falsch #3");
			Assert.AreEqual("embeddedType.dateTimeVar", ".embeddedType.dateTimeVar", "DateTimeVar falsch #3");
			Assert.AreEqual("embeddedType.doubleVar", ".embeddedType.doubleVar", "DoubleVar falsch #3");
			Assert.AreEqual("embeddedType.floatVar", ".embeddedType.floatVar", "FloatVar falsch #3");
			Assert.AreEqual("embeddedType.guidVar", ".embeddedType.guidVar", "GuidVar falsch #3");
			Assert.AreEqual("embeddedType.int16Var", ".embeddedType.int16Var", "Int16Var falsch #3");
			Assert.AreEqual("embeddedType.int32Var", ".embeddedType.int32Var", "Int32Var falsch #3");
			Assert.AreEqual("embeddedType.int64Var", ".embeddedType.int64Var", "Int64Var falsch #3");
			Assert.AreEqual("embeddedType.stringVar", ".embeddedType.stringVar", "StringVar falsch #3");
			Assert.AreEqual("embeddedType.uint16Var", ".embeddedType.uint16Var", "Uint16Var falsch #3");
			Assert.AreEqual("embeddedType.uint32Var", ".embeddedType.uint32Var", "Uint32Var falsch #3");
			Assert.AreEqual("embeddedType.uint64Var", ".embeddedType.uint64Var", "Uint64Var falsch #3");
			Assert.AreEqual("embeddedType.enumVar", ".embeddedType.enumVar", "Uint64Var falsch #3");

		}
#endif

        [Test]
        public void TestPrimitiveTypeMethodCall()
        {
            PrimitiveTypeMethodCaller mc1 = new PrimitiveTypeMethodCaller();
            PrimitiveTypeMethodCaller mc2 = new PrimitiveTypeMethodCaller();
            Assert.AreEqual(0, mc1.StringTest(mc2), "Result must be 0 #1");
            Assert.AreEqual(0, mc1.DoubleTest(mc2), "Result must be 0 #2");
            Assert.AreEqual(0, mc1.DateTimeTest(mc2), "Result must be 0 #3");
            Assert.AreEqual(0, mc1.BoolTest(mc2), "Result must be 0 #4");
            Assert.AreEqual(0, mc1.IntTest(mc2), "Result must be 0 #5");
        }

		[Test]
		public void TestValueTypeAndEmbeddedType()
		{
			var pm = PmFactory.NewPersistenceManager();

			Class cl = pm.NDOMapping.FindClass("DataTypeTestClasses.VtAndEtContainer");
			Field f = cl.FindField("embeddedType.doubleVar");
			f.Column.Size = 12;
			f.Column.Precision = 7;
			VtAndEtContainer cont = new VtAndEtContainer();
			cont.Init();
			pm.MakePersistent(cont);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(VtAndEtContainer));
			Assert.AreEqual(1, l.Count, "Anzahl Container stimmt nicht");
			cont = (VtAndEtContainer) l[0];
			AssertVtAndEtContainer(cont);		
		}

		[Test]
		public void TestValueTypeAndEmbeddedTypeDerived()
		{
			var pm = PmFactory.NewPersistenceManager();

			VtAndEtContainerDerived cont = new VtAndEtContainerDerived();
			cont.Init();
			pm.MakePersistent(cont);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(VtAndEtContainerDerived));
			Assert.AreEqual(1, l.Count, "Anzahl Container stimmt nicht");
			cont = (VtAndEtContainerDerived) l[0];
			AssertVtAndEtContainer(cont);		
		}


		void AssertVtAndEtContainer(VtAndEtContainer cont)
		{
			Assert.AreEqual(0x55, cont.PropValType.ByteVar, "ByteVar falsch #1");
			Assert.AreEqual(true,  cont.PropValType.BoolVar, "BoolVar falsch #1");
			Assert.AreEqual(12.34m, cont.PropValType.DecVar, "DecVar falsch #1");
#if !FIREBIRD && !POSTGRE
			Assert.AreEqual(new DateTime(2002, 12, 1, 1, 0, 20), cont.PropValType.DateTimeVar, "DateTimeVar falsch #1");
#else
			Assert.AreEqual(new DateTime(2002, 12, 1, 0, 0, 0), cont.PropValType.DateTimeVar, "DateTimeVar falsch #1");
#endif
#if SQLITE
			Assert.That( CheckDouble( 12345.123456, cont.PropValType.DoubleVar ), "DoubleVar falsch #1" );
			Assert.That( CheckFloat( 12345.1f, cont.PropValType.FloatVar ), "FloatVar falsch #1" );
#else
			Assert.AreEqual(12345.123456, cont.PropValType.DoubleVar, "DoubleVar falsch #1");
			Assert.AreEqual(12345.1f, cont.PropValType.FloatVar, "FloatVar falsch #1");
#endif
			Assert.AreEqual(new Guid("12341234-1234-1234-1234-123412341234"), cont.PropValType.GuidVar, "GuidVar falsch #1");
			Assert.AreEqual(0x1234, cont.PropValType.Int16Var, "Int16Var falsch #1");
			Assert.AreEqual(0x12341234, cont.PropValType.Int32Var, "Int32Var falsch #1");
			Assert.AreEqual(0x143214321, cont.PropValType.Int64Var, "Int64Var falsch #1");
			Assert.AreEqual("Teststring", cont.PropValType.StringVar, "StringVar falsch #1");
			Assert.AreEqual(0xabc, cont.PropValType.Uint16Var, "Uint16Var falsch #1");
			Assert.AreEqual(0x12341234, cont.PropValType.Uint32Var, "Uint32Var falsch #1");
			Assert.AreEqual(0x143214321, cont.PropValType.Uint64Var, "Uint64Var falsch #1");
			Assert.AreEqual(EnumType.zwei, cont.PropValType.EnumVar, "EnumVar falsch #1");
			Assert.AreEqual(Guid.Empty, cont.PropValType.EmptyGuidVar, "EmptyGuidVar falsch #1");
			Assert.AreEqual(DateTime.MinValue, cont.PropValType.EmptyDateTimeVar, "EmptyDateTimeVar falsch #1");
			Assert.Null(cont.PropValType.NullString, "Empty String falsch #1");

			Assert.AreEqual(0x55, cont.PubValType.ByteVar, "ByteVar falsch #2");
			Assert.AreEqual(true,  cont.PubValType.BoolVar, "BoolVar falsch #2");
			Assert.AreEqual(12.34m, cont.PubValType.DecVar, "DecVar falsch #2");
#if !FIREBIRD && !POSTGRE
			Assert.AreEqual(new DateTime(2002, 12, 1, 1, 0, 20), cont.PubValType.DateTimeVar, "DateTimeVar falsch #2");
#else
			Assert.AreEqual(new DateTime(2002, 12, 1, 0, 0, 0), cont.PubValType.DateTimeVar, "DateTimeVar falsch #2");
#endif
#if SQLITE
			Assert.That( CheckDouble( 12345.123456, cont.PropValType.DoubleVar ), "DoubleVar falsch #1" );
			Assert.That( CheckFloat( 12345.1f, cont.PropValType.FloatVar ), "FloatVar falsch #1" );
#else
			Assert.AreEqual(12345.123456, cont.PubValType.DoubleVar, "DoubleVar falsch #2");
			Assert.AreEqual(12345.1f, cont.PubValType.FloatVar, "FloatVar falsch #2");
#endif
			Assert.AreEqual(new Guid("12341234-1234-1234-1234-123412341234"), cont.PubValType.GuidVar, "GuidVar falsch #2");
			Assert.AreEqual(0x1234, cont.PubValType.Int16Var, "Int16Var falsch #2");
			Assert.AreEqual(0x12341234, cont.PubValType.Int32Var, "Int32Var falsch #2");
			Assert.AreEqual(0x143214321, cont.PubValType.Int64Var, "Int64Var falsch #2");
			Assert.AreEqual("Teststring", cont.PubValType.StringVar, "StringVar falsch #2");
			Assert.AreEqual(0xabc, cont.PubValType.Uint16Var, "Uint16Var falsch #2");
			Assert.AreEqual(0x12341234, cont.PubValType.Uint32Var, "Uint32Var falsch #2");
			Assert.AreEqual(0x143214321, cont.PubValType.Uint64Var, "Uint64Var falsch #2");
			Assert.AreEqual(EnumType.zwei, cont.PubValType.EnumVar, "EnumVar falsch #2");
			Assert.AreEqual(Guid.Empty, cont.PubValType.EmptyGuidVar, "EmptyGuidVar falsch #2");
			Assert.AreEqual(DateTime.MinValue, cont.PubValType.EmptyDateTimeVar, "EmptyDateTimeVar falsch #2");
			Assert.Null(cont.PubValType.NullString, "Empty String falsch #1");

			Assert.AreEqual(0x55, cont.EmbeddedType.ByteVar, "ByteVar falsch #3");
			Assert.AreEqual(true,  cont.EmbeddedType.BoolVar, "BoolVar falsch #3");
			Assert.AreEqual(12.34m, cont.EmbeddedType.DecVar, "DecVar falsch #3");
#if !FIREBIRD && !POSTGRE
			Assert.AreEqual(new DateTime(2002, 12, 1, 1, 0, 20), cont.EmbeddedType.DateTimeVar, "DateTimeVar falsch #2");
#else
			Assert.AreEqual(new DateTime(2002, 12, 1, 0, 0, 0), cont.EmbeddedType.DateTimeVar, "DateTimeVar falsch #2");
#endif
#if SQLITE
			Assert.That( CheckDouble( 12345.123456, cont.PropValType.DoubleVar ), "DoubleVar falsch #1" );
			Assert.That( CheckFloat( 12345.1f, cont.PropValType.FloatVar ), "FloatVar falsch #1" );
#else
			Assert.AreEqual(12345.123456, cont.EmbeddedType.DoubleVar, "DoubleVar falsch #3");
			Assert.AreEqual(12345.1f, cont.EmbeddedType.FloatVar, "FloatVar falsch #3");
#endif
			Assert.AreEqual(new Guid("12341234-1234-1234-1234-123412341234"), cont.EmbeddedType.GuidVar, "GuidVar falsch #3");
			Assert.AreEqual(0x1234, cont.EmbeddedType.Int16Var, "Int16Var falsch #3");
			Assert.AreEqual(0x12341234, cont.EmbeddedType.Int32Var, "Int32Var falsch #3");
			Assert.AreEqual(0x143214321, cont.EmbeddedType.Int64Var, "Int64Var falsch #3");
			Assert.AreEqual("Teststring", cont.EmbeddedType.StringVar, "StringVar falsch #3");
			Assert.AreEqual(0xabc, cont.EmbeddedType.Uint16Var, "Uint16Var falsch #3");
			Assert.AreEqual(0x12341234, cont.EmbeddedType.Uint32Var, "Uint32Var falsch #3");
			Assert.AreEqual(0x143214321, cont.EmbeddedType.Uint64Var, "Uint64Var falsch #3");
			Assert.AreEqual(EnumType.drei, cont.EmbeddedType.EnumVar, "EnumVar falsch #3");
			Assert.AreEqual(Guid.Empty, cont.EmbeddedType.EmptyGuidVar, "EmptyGuidVar falsch #3");
			Assert.AreEqual(DateTime.MinValue, cont.EmbeddedType.EmptyDateTimeVar, "EmptyDateTimeVar falsch #3");
			Assert.Null(cont.EmbeddedType.NullString, "Empty String falsch #1");
		}
	}
}
