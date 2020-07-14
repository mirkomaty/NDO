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
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using NDO;
using NDO.Mapping;
using NDO.Query;
using NUnit.Framework;
using RelationTestClasses;

namespace RelationUnitTests
{

public class PmFactory
{
	static PersistenceManager pm;
	public static PersistenceManager NewPersistenceManager()
	{
		if (pm == null)
		{
			pm = new PersistenceManager(@"C:\Projekte\NDO\UnitTestGenerator\UnitTests\bin\Debug\NDOMapping.xml");
			pm.LogPath = @"C:\Projekte\NDO\UnitTestGenerator";
		}
		else
		{
			pm.UnloadCache();
		}
		return pm;
	}
}


[TestFixture]
public class TestAgrDir1NoTblAuto
{
	AgrDir1NoTblAutoLeft ownVar;
	AgrDir1NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1NoTblAutoLeft();
		otherVar = new AgrDir1NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1NoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1NoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1NoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("AgrDir1NoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1TblAuto
{
	AgrDir1TblAutoLeft ownVar;
	AgrDir1TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1TblAutoLeft();
		otherVar = new AgrDir1TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1TblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11NoTblAuto
{
	AgrBi11NoTblAutoLeft ownVar;
	AgrBi11NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11NoTblAutoLeft();
		otherVar = new AgrBi11NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11NoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11NoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11NoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11TblAuto
{
	AgrBi11TblAutoLeft ownVar;
	AgrBi11TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11TblAutoLeft();
		otherVar = new AgrBi11TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11TblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11TblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11TblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnNoTblAuto
{
	AgrDirnNoTblAutoLeft ownVar;
	AgrDirnNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnNoTblAutoLeft();
		otherVar = new AgrDirnNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnNoTblAutoLeft)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnNoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("AgrDirnNoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnTblAuto
{
	AgrDirnTblAutoLeft ownVar;
	AgrDirnTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnTblAutoLeft();
		otherVar = new AgrDirnTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1NoTblAuto
{
	AgrBin1NoTblAutoLeft ownVar;
	AgrBin1NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1NoTblAutoLeft();
		otherVar = new AgrBin1NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1NoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1NoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1NoTblAutoLeft)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1NoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("AgrBin1NoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1NoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1NoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1TblAuto
{
	AgrBin1TblAutoLeft ownVar;
	AgrBin1TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1TblAutoLeft();
		otherVar = new AgrBin1TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1TblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1TblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1TblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1TblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nNoTblAuto
{
	AgrBi1nNoTblAutoLeft ownVar;
	AgrBi1nNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nNoTblAutoLeft();
		otherVar = new AgrBi1nNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nNoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("AgrBi1nNoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nTblAuto
{
	AgrBi1nTblAutoLeft ownVar;
	AgrBi1nTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nTblAutoLeft();
		otherVar = new AgrBi1nTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnTblAuto
{
	AgrBinnTblAutoLeft ownVar;
	AgrBinnTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnTblAutoLeft();
		otherVar = new AgrBinnTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1NoTblAuto
{
	CmpDir1NoTblAutoLeft ownVar;
	CmpDir1NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1NoTblAutoLeft();
		otherVar = new CmpDir1NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1NoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1NoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("CmpDir1NoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1TblAuto
{
	CmpDir1TblAutoLeft ownVar;
	CmpDir1TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1TblAutoLeft();
		otherVar = new CmpDir1TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11NoTblAuto
{
	CmpBi11NoTblAutoLeft ownVar;
	CmpBi11NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11NoTblAutoLeft();
		otherVar = new CmpBi11NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11NoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11NoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11TblAuto
{
	CmpBi11TblAutoLeft ownVar;
	CmpBi11TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11TblAutoLeft();
		otherVar = new CmpBi11TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11TblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11TblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnNoTblAuto
{
	CmpDirnNoTblAutoLeft ownVar;
	CmpDirnNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnNoTblAutoLeft();
		otherVar = new CmpDirnNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnNoTblAutoLeft)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnNoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("CmpDirnNoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnTblAuto
{
	CmpDirnTblAutoLeft ownVar;
	CmpDirnTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnTblAutoLeft();
		otherVar = new CmpDirnTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1NoTblAuto
{
	CmpBin1NoTblAutoLeft ownVar;
	CmpBin1NoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1NoTblAutoLeft();
		otherVar = new CmpBin1NoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1NoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1NoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1NoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1NoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1NoTblAutoLeft)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1NoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("CmpBin1NoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1NoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1NoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1NoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1NoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1TblAuto
{
	CmpBin1TblAutoLeft ownVar;
	CmpBin1TblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1TblAutoLeft();
		otherVar = new CmpBin1TblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1TblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1TblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1TblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1TblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1TblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1TblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1TblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1TblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nNoTblAuto
{
	CmpBi1nNoTblAutoLeft ownVar;
	CmpBi1nNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nNoTblAutoLeft();
		otherVar = new CmpBi1nNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpBi1nNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(CmpBi1nNoTblAutoRight)})), "Wrong order #1");
		Debug.WriteLine("CmpBi1nNoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nTblAuto
{
	CmpBi1nTblAutoLeft ownVar;
	CmpBi1nTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nTblAutoLeft();
		otherVar = new CmpBi1nTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnTblAuto
{
	CmpBinnTblAutoLeft ownVar;
	CmpBinnTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnTblAutoLeft();
		otherVar = new CmpBinnTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconNoTblAuto
{
	AgrDir1OwnpconNoTblAutoLeftBase ownVar;
	AgrDir1OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconNoTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconNoTblAutoLeftBase)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OwnpconNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconTblAuto
{
	AgrDir1OwnpconTblAutoLeftBase ownVar;
	AgrDir1OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconNoTblAuto
{
	AgrBi11OwnpconNoTblAutoLeftBase ownVar;
	AgrBi11OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconNoTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconTblAuto
{
	AgrBi11OwnpconTblAutoLeftBase ownVar;
	AgrBi11OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconNoTblAuto
{
	AgrDirnOwnpconNoTblAutoLeftBase ownVar;
	AgrDirnOwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconNoTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpconNoTblAutoLeftBase)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpconNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrDirnOwnpconNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconTblAuto
{
	AgrDirnOwnpconTblAutoLeftBase ownVar;
	AgrDirnOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconNoTblAuto
{
	AgrBin1OwnpconNoTblAutoLeftBase ownVar;
	AgrBin1OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconNoTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpconNoTblAutoLeftBase)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpconNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrBin1OwnpconNoTblAutoLeftBase");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconTblAuto
{
	AgrBin1OwnpconTblAutoLeftBase ownVar;
	AgrBin1OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpconTblAuto
{
	AgrBi1nOwnpconTblAutoLeftBase ownVar;
	AgrBi1nOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpconTblAutoLeftDerived();
		otherVar = new AgrBi1nOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpconTblAuto
{
	AgrBinnOwnpconTblAutoLeftBase ownVar;
	AgrBinnOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpconTblAutoLeftDerived();
		otherVar = new AgrBinnOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpconTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconNoTblAuto
{
	CmpDir1OwnpconNoTblAutoLeftBase ownVar;
	CmpDir1OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconNoTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpconNoTblAutoLeftBase)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpconNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpDir1OwnpconNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconTblAuto
{
	CmpDir1OwnpconTblAutoLeftBase ownVar;
	CmpDir1OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconNoTblAuto
{
	CmpBi11OwnpconNoTblAutoLeftBase ownVar;
	CmpBi11OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconNoTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconTblAuto
{
	CmpBi11OwnpconTblAutoLeftBase ownVar;
	CmpBi11OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconNoTblAuto
{
	CmpDirnOwnpconNoTblAutoLeftBase ownVar;
	CmpDirnOwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconNoTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpconNoTblAutoLeftBase)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpconNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpDirnOwnpconNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconTblAuto
{
	CmpDirnOwnpconTblAutoLeftBase ownVar;
	CmpDirnOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconNoTblAuto
{
	CmpBin1OwnpconNoTblAutoLeftBase ownVar;
	CmpBin1OwnpconNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconNoTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpconNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpconNoTblAutoLeftBase)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpconNoTblAutoRight)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpconNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpconNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpBin1OwnpconNoTblAutoLeftBase");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconTblAuto
{
	CmpBin1OwnpconTblAutoLeftBase ownVar;
	CmpBin1OwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpconTblAuto
{
	CmpBi1nOwnpconTblAutoLeftBase ownVar;
	CmpBi1nOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpconTblAutoLeftDerived();
		otherVar = new CmpBi1nOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpconTblAuto
{
	CmpBinnOwnpconTblAutoLeftBase ownVar;
	CmpBinnOwnpconTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpconTblAutoLeftDerived();
		otherVar = new CmpBinnOwnpconTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpconTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpconTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpconTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpconNoTblAuto
{
	AgrDir1OthpconNoTblAutoLeft ownVar;
	AgrDir1OthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpconNoTblAutoLeft();
		otherVar = new AgrDir1OthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpconNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpconNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpconNoTblAutoRightBase)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpconNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpconNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OthpconNoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpconTblAuto
{
	AgrDir1OthpconTblAutoLeft ownVar;
	AgrDir1OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpconTblAutoLeft();
		otherVar = new AgrDir1OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpconNoTblAuto
{
	AgrBi11OthpconNoTblAutoLeft ownVar;
	AgrBi11OthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpconNoTblAutoLeft();
		otherVar = new AgrBi11OthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpconNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpconTblAuto
{
	AgrBi11OthpconTblAutoLeft ownVar;
	AgrBi11OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpconTblAutoLeft();
		otherVar = new AgrBi11OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOthpconTblAuto
{
	AgrDirnOthpconTblAutoLeft ownVar;
	AgrDirnOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOthpconTblAutoLeft();
		otherVar = new AgrDirnOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OthpconTblAuto
{
	AgrBin1OthpconTblAutoLeft ownVar;
	AgrBin1OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OthpconTblAutoLeft();
		otherVar = new AgrBin1OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpconNoTblAuto
{
	AgrBi1nOthpconNoTblAutoLeft ownVar;
	AgrBi1nOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpconNoTblAutoLeft();
		otherVar = new AgrBi1nOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpconNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpconNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpconNoTblAutoRightBase)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpconNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpconNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrBi1nOthpconNoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpconTblAuto
{
	AgrBi1nOthpconTblAutoLeft ownVar;
	AgrBi1nOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpconTblAutoLeft();
		otherVar = new AgrBi1nOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOthpconTblAuto
{
	AgrBinnOthpconTblAutoLeft ownVar;
	AgrBinnOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOthpconTblAutoLeft();
		otherVar = new AgrBinnOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpconNoTblAuto
{
	CmpDir1OthpconNoTblAutoLeft ownVar;
	CmpDir1OthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpconNoTblAutoLeft();
		otherVar = new CmpDir1OthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpconTblAuto
{
	CmpDir1OthpconTblAutoLeft ownVar;
	CmpDir1OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpconTblAutoLeft();
		otherVar = new CmpDir1OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpconNoTblAuto
{
	CmpBi11OthpconNoTblAutoLeft ownVar;
	CmpBi11OthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpconNoTblAutoLeft();
		otherVar = new CmpBi11OthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpconTblAuto
{
	CmpBi11OthpconTblAutoLeft ownVar;
	CmpBi11OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpconTblAutoLeft();
		otherVar = new CmpBi11OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOthpconTblAuto
{
	CmpDirnOthpconTblAutoLeft ownVar;
	CmpDirnOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOthpconTblAutoLeft();
		otherVar = new CmpDirnOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OthpconTblAuto
{
	CmpBin1OthpconTblAutoLeft ownVar;
	CmpBin1OthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OthpconTblAutoLeft();
		otherVar = new CmpBin1OthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpconNoTblAuto
{
	CmpBi1nOthpconNoTblAutoLeft ownVar;
	CmpBi1nOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpconNoTblAutoLeft();
		otherVar = new CmpBi1nOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpconNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpconNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpconNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpconTblAuto
{
	CmpBi1nOthpconTblAutoLeft ownVar;
	CmpBi1nOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpconTblAutoLeft();
		otherVar = new CmpBi1nOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOthpconTblAuto
{
	CmpBinnOthpconTblAutoLeft ownVar;
	CmpBinnOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOthpconTblAutoLeft();
		otherVar = new CmpBinnOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOthpconTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOthpconTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOthpconTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconOthpconNoTblAuto
{
	AgrDir1OwnpconOthpconNoTblAutoLeftBase ownVar;
	AgrDir1OwnpconOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconOthpconNoTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpconOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconOthpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconOthpconNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconOthpconNoTblAutoLeftBase)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconOthpconNoTblAutoRightBase)})), "Wrong order #1");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconOthpconNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpconOthpconNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OwnpconOthpconNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconOthpconTblAuto
{
	AgrDir1OwnpconOthpconTblAutoLeftBase ownVar;
	AgrDir1OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconOthpconNoTblAuto
{
	AgrBi11OwnpconOthpconNoTblAutoLeftBase ownVar;
	AgrBi11OwnpconOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconOthpconNoTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpconOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconOthpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconOthpconNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconOthpconTblAuto
{
	AgrBi11OwnpconOthpconTblAutoLeftBase ownVar;
	AgrBi11OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconOthpconTblAuto
{
	AgrDirnOwnpconOthpconTblAutoLeftBase ownVar;
	AgrDirnOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconOthpconTblAuto
{
	AgrBin1OwnpconOthpconTblAutoLeftBase ownVar;
	AgrBin1OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpconOthpconTblAuto
{
	AgrBi1nOwnpconOthpconTblAutoLeftBase ownVar;
	AgrBi1nOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrBi1nOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpconOthpconTblAuto
{
	AgrBinnOwnpconOthpconTblAutoLeftBase ownVar;
	AgrBinnOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpconOthpconTblAutoLeftDerived();
		otherVar = new AgrBinnOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpconOthpconTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconOthpconNoTblAuto
{
	CmpDir1OwnpconOthpconNoTblAutoLeftBase ownVar;
	CmpDir1OwnpconOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconOthpconNoTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpconOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconOthpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconOthpconTblAuto
{
	CmpDir1OwnpconOthpconTblAutoLeftBase ownVar;
	CmpDir1OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconOthpconNoTblAuto
{
	CmpBi11OwnpconOthpconNoTblAutoLeftBase ownVar;
	CmpBi11OwnpconOthpconNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconOthpconNoTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpconOthpconNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconOthpconNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconOthpconTblAuto
{
	CmpBi11OwnpconOthpconTblAutoLeftBase ownVar;
	CmpBi11OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconOthpconTblAuto
{
	CmpDirnOwnpconOthpconTblAutoLeftBase ownVar;
	CmpDirnOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconOthpconTblAuto
{
	CmpBin1OwnpconOthpconTblAutoLeftBase ownVar;
	CmpBin1OwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpconOthpconTblAuto
{
	CmpBi1nOwnpconOthpconTblAutoLeftBase ownVar;
	CmpBi1nOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpBi1nOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpconOthpconTblAuto
{
	CmpBinnOwnpconOthpconTblAutoLeftBase ownVar;
	CmpBinnOwnpconOthpconTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpconOthpconTblAutoLeftDerived();
		otherVar = new CmpBinnOwnpconOthpconTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpconOthpconTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpconOthpconTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpconOthpconTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpconOthpconTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpconOthpconTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpconOthpconTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1NoTblGuid
{
	AgrDir1NoTblGuidLeft ownVar;
	AgrDir1NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1NoTblGuidLeft();
		otherVar = new AgrDir1NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1NoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1TblGuid
{
	AgrDir1TblGuidLeft ownVar;
	AgrDir1TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1TblGuidLeft();
		otherVar = new AgrDir1TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1TblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11NoTblGuid
{
	AgrBi11NoTblGuidLeft ownVar;
	AgrBi11NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11NoTblGuidLeft();
		otherVar = new AgrBi11NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11NoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11NoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11NoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11TblGuid
{
	AgrBi11TblGuidLeft ownVar;
	AgrBi11TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11TblGuidLeft();
		otherVar = new AgrBi11TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11TblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11TblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11TblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnNoTblGuid
{
	AgrDirnNoTblGuidLeft ownVar;
	AgrDirnNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnNoTblGuidLeft();
		otherVar = new AgrDirnNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnTblGuid
{
	AgrDirnTblGuidLeft ownVar;
	AgrDirnTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnTblGuidLeft();
		otherVar = new AgrDirnTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1NoTblGuid
{
	AgrBin1NoTblGuidLeft ownVar;
	AgrBin1NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1NoTblGuidLeft();
		otherVar = new AgrBin1NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1NoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1NoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1NoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1NoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1TblGuid
{
	AgrBin1TblGuidLeft ownVar;
	AgrBin1TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1TblGuidLeft();
		otherVar = new AgrBin1TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1TblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1TblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1TblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1TblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nNoTblGuid
{
	AgrBi1nNoTblGuidLeft ownVar;
	AgrBi1nNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nNoTblGuidLeft();
		otherVar = new AgrBi1nNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nTblGuid
{
	AgrBi1nTblGuidLeft ownVar;
	AgrBi1nTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nTblGuidLeft();
		otherVar = new AgrBi1nTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnTblGuid
{
	AgrBinnTblGuidLeft ownVar;
	AgrBinnTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnTblGuidLeft();
		otherVar = new AgrBinnTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1NoTblGuid
{
	CmpDir1NoTblGuidLeft ownVar;
	CmpDir1NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1NoTblGuidLeft();
		otherVar = new CmpDir1NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1TblGuid
{
	CmpDir1TblGuidLeft ownVar;
	CmpDir1TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1TblGuidLeft();
		otherVar = new CmpDir1TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11NoTblGuid
{
	CmpBi11NoTblGuidLeft ownVar;
	CmpBi11NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11NoTblGuidLeft();
		otherVar = new CmpBi11NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11NoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11NoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11TblGuid
{
	CmpBi11TblGuidLeft ownVar;
	CmpBi11TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11TblGuidLeft();
		otherVar = new CmpBi11TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11TblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11TblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnNoTblGuid
{
	CmpDirnNoTblGuidLeft ownVar;
	CmpDirnNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnNoTblGuidLeft();
		otherVar = new CmpDirnNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnTblGuid
{
	CmpDirnTblGuidLeft ownVar;
	CmpDirnTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnTblGuidLeft();
		otherVar = new CmpDirnTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1NoTblGuid
{
	CmpBin1NoTblGuidLeft ownVar;
	CmpBin1NoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1NoTblGuidLeft();
		otherVar = new CmpBin1NoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1NoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1NoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1NoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1NoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1NoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1NoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1NoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1NoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1TblGuid
{
	CmpBin1TblGuidLeft ownVar;
	CmpBin1TblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1TblGuidLeft();
		otherVar = new CmpBin1TblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1TblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1TblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1TblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1TblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1TblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1TblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1TblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1TblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nNoTblGuid
{
	CmpBi1nNoTblGuidLeft ownVar;
	CmpBi1nNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nNoTblGuidLeft();
		otherVar = new CmpBi1nNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nTblGuid
{
	CmpBi1nTblGuidLeft ownVar;
	CmpBi1nTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nTblGuidLeft();
		otherVar = new CmpBi1nTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnTblGuid
{
	CmpBinnTblGuidLeft ownVar;
	CmpBinnTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnTblGuidLeft();
		otherVar = new CmpBinnTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconNoTblGuid
{
	AgrDir1OwnpconNoTblGuidLeftBase ownVar;
	AgrDir1OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconNoTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconTblGuid
{
	AgrDir1OwnpconTblGuidLeftBase ownVar;
	AgrDir1OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconNoTblGuid
{
	AgrBi11OwnpconNoTblGuidLeftBase ownVar;
	AgrBi11OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconNoTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconTblGuid
{
	AgrBi11OwnpconTblGuidLeftBase ownVar;
	AgrBi11OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconNoTblGuid
{
	AgrDirnOwnpconNoTblGuidLeftBase ownVar;
	AgrDirnOwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconNoTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconTblGuid
{
	AgrDirnOwnpconTblGuidLeftBase ownVar;
	AgrDirnOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconNoTblGuid
{
	AgrBin1OwnpconNoTblGuidLeftBase ownVar;
	AgrBin1OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconNoTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconTblGuid
{
	AgrBin1OwnpconTblGuidLeftBase ownVar;
	AgrBin1OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpconTblGuid
{
	AgrBi1nOwnpconTblGuidLeftBase ownVar;
	AgrBi1nOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpconTblGuidLeftDerived();
		otherVar = new AgrBi1nOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpconTblGuid
{
	AgrBinnOwnpconTblGuidLeftBase ownVar;
	AgrBinnOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpconTblGuidLeftDerived();
		otherVar = new AgrBinnOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpconTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconNoTblGuid
{
	CmpDir1OwnpconNoTblGuidLeftBase ownVar;
	CmpDir1OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconNoTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconTblGuid
{
	CmpDir1OwnpconTblGuidLeftBase ownVar;
	CmpDir1OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconNoTblGuid
{
	CmpBi11OwnpconNoTblGuidLeftBase ownVar;
	CmpBi11OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconNoTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconTblGuid
{
	CmpBi11OwnpconTblGuidLeftBase ownVar;
	CmpBi11OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconNoTblGuid
{
	CmpDirnOwnpconNoTblGuidLeftBase ownVar;
	CmpDirnOwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconNoTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconTblGuid
{
	CmpDirnOwnpconTblGuidLeftBase ownVar;
	CmpDirnOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconNoTblGuid
{
	CmpBin1OwnpconNoTblGuidLeftBase ownVar;
	CmpBin1OwnpconNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconNoTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpconNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconTblGuid
{
	CmpBin1OwnpconTblGuidLeftBase ownVar;
	CmpBin1OwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpconTblGuid
{
	CmpBi1nOwnpconTblGuidLeftBase ownVar;
	CmpBi1nOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpconTblGuidLeftDerived();
		otherVar = new CmpBi1nOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpconTblGuid
{
	CmpBinnOwnpconTblGuidLeftBase ownVar;
	CmpBinnOwnpconTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpconTblGuidLeftDerived();
		otherVar = new CmpBinnOwnpconTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpconTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpconTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpconTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpconNoTblGuid
{
	AgrDir1OthpconNoTblGuidLeft ownVar;
	AgrDir1OthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpconNoTblGuidLeft();
		otherVar = new AgrDir1OthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpconNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpconTblGuid
{
	AgrDir1OthpconTblGuidLeft ownVar;
	AgrDir1OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpconTblGuidLeft();
		otherVar = new AgrDir1OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpconNoTblGuid
{
	AgrBi11OthpconNoTblGuidLeft ownVar;
	AgrBi11OthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpconNoTblGuidLeft();
		otherVar = new AgrBi11OthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpconNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpconTblGuid
{
	AgrBi11OthpconTblGuidLeft ownVar;
	AgrBi11OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpconTblGuidLeft();
		otherVar = new AgrBi11OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOthpconTblGuid
{
	AgrDirnOthpconTblGuidLeft ownVar;
	AgrDirnOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOthpconTblGuidLeft();
		otherVar = new AgrDirnOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OthpconTblGuid
{
	AgrBin1OthpconTblGuidLeft ownVar;
	AgrBin1OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OthpconTblGuidLeft();
		otherVar = new AgrBin1OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpconNoTblGuid
{
	AgrBi1nOthpconNoTblGuidLeft ownVar;
	AgrBi1nOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpconNoTblGuidLeft();
		otherVar = new AgrBi1nOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpconNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpconTblGuid
{
	AgrBi1nOthpconTblGuidLeft ownVar;
	AgrBi1nOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpconTblGuidLeft();
		otherVar = new AgrBi1nOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOthpconTblGuid
{
	AgrBinnOthpconTblGuidLeft ownVar;
	AgrBinnOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOthpconTblGuidLeft();
		otherVar = new AgrBinnOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpconNoTblGuid
{
	CmpDir1OthpconNoTblGuidLeft ownVar;
	CmpDir1OthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpconNoTblGuidLeft();
		otherVar = new CmpDir1OthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpconTblGuid
{
	CmpDir1OthpconTblGuidLeft ownVar;
	CmpDir1OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpconTblGuidLeft();
		otherVar = new CmpDir1OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpconNoTblGuid
{
	CmpBi11OthpconNoTblGuidLeft ownVar;
	CmpBi11OthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpconNoTblGuidLeft();
		otherVar = new CmpBi11OthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpconTblGuid
{
	CmpBi11OthpconTblGuidLeft ownVar;
	CmpBi11OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpconTblGuidLeft();
		otherVar = new CmpBi11OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOthpconTblGuid
{
	CmpDirnOthpconTblGuidLeft ownVar;
	CmpDirnOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOthpconTblGuidLeft();
		otherVar = new CmpDirnOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OthpconTblGuid
{
	CmpBin1OthpconTblGuidLeft ownVar;
	CmpBin1OthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OthpconTblGuidLeft();
		otherVar = new CmpBin1OthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpconNoTblGuid
{
	CmpBi1nOthpconNoTblGuidLeft ownVar;
	CmpBi1nOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpconNoTblGuidLeft();
		otherVar = new CmpBi1nOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpconNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpconNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpconNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpconTblGuid
{
	CmpBi1nOthpconTblGuidLeft ownVar;
	CmpBi1nOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpconTblGuidLeft();
		otherVar = new CmpBi1nOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOthpconTblGuid
{
	CmpBinnOthpconTblGuidLeft ownVar;
	CmpBinnOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOthpconTblGuidLeft();
		otherVar = new CmpBinnOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOthpconTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOthpconTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOthpconTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconOthpconNoTblGuid
{
	AgrDir1OwnpconOthpconNoTblGuidLeftBase ownVar;
	AgrDir1OwnpconOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconOthpconNoTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpconOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconOthpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconOthpconNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpconOthpconTblGuid
{
	AgrDir1OwnpconOthpconTblGuidLeftBase ownVar;
	AgrDir1OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconOthpconNoTblGuid
{
	AgrBi11OwnpconOthpconNoTblGuidLeftBase ownVar;
	AgrBi11OwnpconOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconOthpconNoTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpconOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconOthpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconOthpconNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpconOthpconTblGuid
{
	AgrBi11OwnpconOthpconTblGuidLeftBase ownVar;
	AgrBi11OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpconOthpconTblGuid
{
	AgrDirnOwnpconOthpconTblGuidLeftBase ownVar;
	AgrDirnOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpconOthpconTblGuid
{
	AgrBin1OwnpconOthpconTblGuidLeftBase ownVar;
	AgrBin1OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpconOthpconTblGuid
{
	AgrBi1nOwnpconOthpconTblGuidLeftBase ownVar;
	AgrBi1nOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrBi1nOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpconOthpconTblGuid
{
	AgrBinnOwnpconOthpconTblGuidLeftBase ownVar;
	AgrBinnOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpconOthpconTblGuidLeftDerived();
		otherVar = new AgrBinnOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpconOthpconTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconOthpconNoTblGuid
{
	CmpDir1OwnpconOthpconNoTblGuidLeftBase ownVar;
	CmpDir1OwnpconOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconOthpconNoTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpconOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconOthpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpconOthpconTblGuid
{
	CmpDir1OwnpconOthpconTblGuidLeftBase ownVar;
	CmpDir1OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconOthpconNoTblGuid
{
	CmpBi11OwnpconOthpconNoTblGuidLeftBase ownVar;
	CmpBi11OwnpconOthpconNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconOthpconNoTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpconOthpconNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconOthpconNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpconOthpconTblGuid
{
	CmpBi11OwnpconOthpconTblGuidLeftBase ownVar;
	CmpBi11OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpconOthpconTblGuid
{
	CmpDirnOwnpconOthpconTblGuidLeftBase ownVar;
	CmpDirnOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpconOthpconTblGuid
{
	CmpBin1OwnpconOthpconTblGuidLeftBase ownVar;
	CmpBin1OwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpconOthpconTblGuid
{
	CmpBi1nOwnpconOthpconTblGuidLeftBase ownVar;
	CmpBi1nOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpBi1nOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpconOthpconTblGuid
{
	CmpBinnOwnpconOthpconTblGuidLeftBase ownVar;
	CmpBinnOwnpconOthpconTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpconOthpconTblGuidLeftDerived();
		otherVar = new CmpBinnOwnpconOthpconTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpconOthpconTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpconOthpconTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpconOthpconTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpconOthpconTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpconOthpconTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpconOthpconTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpconOthpconTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsNoTblAuto
{
	AgrDir1OwnpabsNoTblAutoLeftBase ownVar;
	AgrDir1OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsNoTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpabsNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OwnpabsNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsTblAuto
{
	AgrDir1OwnpabsTblAutoLeftBase ownVar;
	AgrDir1OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsNoTblAuto
{
	AgrBi11OwnpabsNoTblAutoLeftBase ownVar;
	AgrBi11OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsNoTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsTblAuto
{
	AgrBi11OwnpabsTblAutoLeftBase ownVar;
	AgrBi11OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsNoTblAuto
{
	AgrDirnOwnpabsNoTblAutoLeftBase ownVar;
	AgrDirnOwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsNoTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpabsNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrDirnOwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrDirnOwnpabsNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsTblAuto
{
	AgrDirnOwnpabsTblAutoLeftBase ownVar;
	AgrDirnOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsNoTblAuto
{
	AgrBin1OwnpabsNoTblAutoLeftBase ownVar;
	AgrBin1OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsNoTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsNoTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpabsNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(AgrBin1OwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("AgrBin1OwnpabsNoTblAutoLeftBase");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsTblAuto
{
	AgrBin1OwnpabsTblAutoLeftBase ownVar;
	AgrBin1OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpabsTblAuto
{
	AgrBi1nOwnpabsTblAutoLeftBase ownVar;
	AgrBi1nOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpabsTblAutoLeftDerived();
		otherVar = new AgrBi1nOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpabsTblAuto
{
	AgrBinnOwnpabsTblAutoLeftBase ownVar;
	AgrBinnOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpabsTblAutoLeftDerived();
		otherVar = new AgrBinnOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpabsTblAutoRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsNoTblAuto
{
	CmpDir1OwnpabsNoTblAutoLeftBase ownVar;
	CmpDir1OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsNoTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpabsNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(CmpDir1OwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpDir1OwnpabsNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsTblAuto
{
	CmpDir1OwnpabsTblAutoLeftBase ownVar;
	CmpDir1OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsNoTblAuto
{
	CmpBi11OwnpabsNoTblAutoLeftBase ownVar;
	CmpBi11OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsNoTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsTblAuto
{
	CmpBi11OwnpabsTblAutoLeftBase ownVar;
	CmpBi11OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsNoTblAuto
{
	CmpDirnOwnpabsNoTblAutoLeftBase ownVar;
	CmpDirnOwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsNoTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpabsNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpDirnOwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpDirnOwnpabsNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsTblAuto
{
	CmpDirnOwnpabsTblAutoLeftBase ownVar;
	CmpDirnOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsNoTblAuto
{
	CmpBin1OwnpabsNoTblAutoLeftBase ownVar;
	CmpBin1OwnpabsNoTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsNoTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpabsNoTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsNoTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsNoTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpabsNoTblAutoLeftDerived)})) 
				< ((int)mi.Invoke(mapping, new object[]{typeof(CmpBin1OwnpabsNoTblAutoRight)})), "Wrong order #2");
		Debug.WriteLine("CmpBin1OwnpabsNoTblAutoLeftBase");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsNoTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsTblAuto
{
	CmpBin1OwnpabsTblAutoLeftBase ownVar;
	CmpBin1OwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpabsTblAuto
{
	CmpBi1nOwnpabsTblAutoLeftBase ownVar;
	CmpBi1nOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpabsTblAutoLeftDerived();
		otherVar = new CmpBi1nOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpabsTblAuto
{
	CmpBinnOwnpabsTblAutoLeftBase ownVar;
	CmpBinnOwnpabsTblAutoRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpabsTblAutoLeftDerived();
		otherVar = new CmpBinnOwnpabsTblAutoRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpabsTblAutoRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpabsTblAutoRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblAutoRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpabsTblAutoRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpabsNoTblAuto
{
	AgrDir1OthpabsNoTblAutoLeft ownVar;
	AgrDir1OthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpabsNoTblAutoLeft();
		otherVar = new AgrDir1OthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpabsNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpabsNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OthpabsNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OthpabsNoTblAutoLeft");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpabsTblAuto
{
	AgrDir1OthpabsTblAutoLeft ownVar;
	AgrDir1OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpabsTblAutoLeft();
		otherVar = new AgrDir1OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpabsNoTblAuto
{
	AgrBi11OthpabsNoTblAutoLeft ownVar;
	AgrBi11OthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpabsNoTblAutoLeft();
		otherVar = new AgrBi11OthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpabsNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpabsTblAuto
{
	AgrBi11OthpabsTblAutoLeft ownVar;
	AgrBi11OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpabsTblAutoLeft();
		otherVar = new AgrBi11OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOthpabsTblAuto
{
	AgrDirnOthpabsTblAutoLeft ownVar;
	AgrDirnOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOthpabsTblAutoLeft();
		otherVar = new AgrDirnOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OthpabsTblAuto
{
	AgrBin1OthpabsTblAutoLeft ownVar;
	AgrBin1OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OthpabsTblAutoLeft();
		otherVar = new AgrBin1OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpabsNoTblAuto
{
	AgrBi1nOthpabsNoTblAutoLeft ownVar;
	AgrBi1nOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpabsNoTblAutoLeft();
		otherVar = new AgrBi1nOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpabsNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpabsNoTblAutoLeft)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrBi1nOthpabsNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrBi1nOthpabsNoTblAutoLeft");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpabsTblAuto
{
	AgrBi1nOthpabsTblAutoLeft ownVar;
	AgrBi1nOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpabsTblAutoLeft();
		otherVar = new AgrBi1nOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOthpabsTblAuto
{
	AgrBinnOthpabsTblAutoLeft ownVar;
	AgrBinnOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOthpabsTblAutoLeft();
		otherVar = new AgrBinnOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpabsNoTblAuto
{
	CmpDir1OthpabsNoTblAutoLeft ownVar;
	CmpDir1OthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpabsNoTblAutoLeft();
		otherVar = new CmpDir1OthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpabsTblAuto
{
	CmpDir1OthpabsTblAutoLeft ownVar;
	CmpDir1OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpabsTblAutoLeft();
		otherVar = new CmpDir1OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpabsNoTblAuto
{
	CmpBi11OthpabsNoTblAutoLeft ownVar;
	CmpBi11OthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpabsNoTblAutoLeft();
		otherVar = new CmpBi11OthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpabsTblAuto
{
	CmpBi11OthpabsTblAutoLeft ownVar;
	CmpBi11OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpabsTblAutoLeft();
		otherVar = new CmpBi11OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOthpabsTblAuto
{
	CmpDirnOthpabsTblAutoLeft ownVar;
	CmpDirnOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOthpabsTblAutoLeft();
		otherVar = new CmpDirnOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OthpabsTblAuto
{
	CmpBin1OthpabsTblAutoLeft ownVar;
	CmpBin1OthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OthpabsTblAutoLeft();
		otherVar = new CmpBin1OthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpabsNoTblAuto
{
	CmpBi1nOthpabsNoTblAutoLeft ownVar;
	CmpBi1nOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpabsNoTblAutoLeft();
		otherVar = new CmpBi1nOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpabsNoTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpabsNoTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpabsNoTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpabsTblAuto
{
	CmpBi1nOthpabsTblAutoLeft ownVar;
	CmpBi1nOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpabsTblAutoLeft();
		otherVar = new CmpBi1nOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOthpabsTblAuto
{
	CmpBinnOthpabsTblAutoLeft ownVar;
	CmpBinnOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOthpabsTblAutoLeft();
		otherVar = new CmpBinnOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOthpabsTblAutoLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOthpabsTblAutoLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblAutoLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOthpabsTblAutoLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsOthpabsNoTblAuto
{
	AgrDir1OwnpabsOthpabsNoTblAutoLeftBase ownVar;
	AgrDir1OwnpabsOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsOthpabsNoTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpabsOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsOthpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsOthpabsNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestUpdateOrder()
	{
		NDO.Mapping.NDOMapping mapping = pm.NDOMapping;
		MethodInfo mi = mapping.GetType().GetMethod("GetUpdateOrder");
		Assert.That(((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpabsOthpabsNoTblAutoLeftDerived)})) 
				> ((int)mi.Invoke(mapping, new object[]{typeof(AgrDir1OwnpabsOthpabsNoTblAutoRightDerived)})), "Wrong order #2");
		Debug.WriteLine("AgrDir1OwnpabsOthpabsNoTblAutoLeftBase");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsOthpabsTblAuto
{
	AgrDir1OwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrDir1OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrDir1OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsOthpabsNoTblAuto
{
	AgrBi11OwnpabsOthpabsNoTblAutoLeftBase ownVar;
	AgrBi11OwnpabsOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsOthpabsNoTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpabsOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsOthpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsOthpabsNoTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		pm.MakePersistent(otherVar);
		pm.Save();
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsOthpabsTblAuto
{
	AgrBi11OwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrBi11OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrBi11OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsOthpabsTblAuto
{
	AgrDirnOwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrDirnOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrDirnOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsOthpabsTblAuto
{
	AgrBin1OwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrBin1OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrBin1OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpabsOthpabsTblAuto
{
	AgrBi1nOwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrBi1nOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrBi1nOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpabsOthpabsTblAuto
{
	AgrBinnOwnpabsOthpabsTblAutoLeftBase ownVar;
	AgrBinnOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new AgrBinnOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpabsOthpabsTblAutoRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsOthpabsNoTblAuto
{
	CmpDir1OwnpabsOthpabsNoTblAutoLeftBase ownVar;
	CmpDir1OwnpabsOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsOthpabsNoTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpabsOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsOthpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsOthpabsTblAuto
{
	CmpDir1OwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpDir1OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpDir1OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsOthpabsNoTblAuto
{
	CmpBi11OwnpabsOthpabsNoTblAutoLeftBase ownVar;
	CmpBi11OwnpabsOthpabsNoTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsOthpabsNoTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpabsOthpabsNoTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsOthpabsNoTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsNoTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsNoTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		bool thrown = false;
		try
		{
			CreateObjects();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.NotNull(ownVar.RelField, "No related object");
			ownVar.RelField = null;
			pm.Save();
			pm.UnloadCache();
			QueryOwn();
			Assert.NotNull(ownVar, "No Query Result");
			Assert.Null(ownVar.RelField, "There should be no object");
		}
		catch (NDOException)
		{
			thrown = true;
		}
		Assert.AreEqual(true, thrown, "NDOException should have been thrown");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.Save();
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsNoTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsNoTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsOthpabsTblAuto
{
	CmpBi11OwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpBi11OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpBi11OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsOthpabsTblAuto
{
	CmpDirnOwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpDirnOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpDirnOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsOthpabsTblAuto
{
	CmpBin1OwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpBin1OwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpBin1OwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpabsOthpabsTblAuto
{
	CmpBi1nOwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpBi1nOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpBi1nOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpabsOthpabsTblAuto
{
	CmpBinnOwnpabsOthpabsTblAutoLeftBase ownVar;
	CmpBinnOwnpabsOthpabsTblAutoRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpabsOthpabsTblAutoLeftDerived();
		otherVar = new CmpBinnOwnpabsOthpabsTblAutoRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpabsOthpabsTblAutoLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpabsOthpabsTblAutoLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpabsOthpabsTblAutoRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpabsOthpabsTblAutoRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblAutoRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblAutoRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpabsOthpabsTblAutoLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpabsOthpabsTblAutoRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsNoTblGuid
{
	AgrDir1OwnpabsNoTblGuidLeftBase ownVar;
	AgrDir1OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsNoTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsTblGuid
{
	AgrDir1OwnpabsTblGuidLeftBase ownVar;
	AgrDir1OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsNoTblGuid
{
	AgrBi11OwnpabsNoTblGuidLeftBase ownVar;
	AgrBi11OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsNoTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsTblGuid
{
	AgrBi11OwnpabsTblGuidLeftBase ownVar;
	AgrBi11OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsNoTblGuid
{
	AgrDirnOwnpabsNoTblGuidLeftBase ownVar;
	AgrDirnOwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsNoTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsTblGuid
{
	AgrDirnOwnpabsTblGuidLeftBase ownVar;
	AgrDirnOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsNoTblGuid
{
	AgrBin1OwnpabsNoTblGuidLeftBase ownVar;
	AgrBin1OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsNoTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsNoTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsTblGuid
{
	AgrBin1OwnpabsTblGuidLeftBase ownVar;
	AgrBin1OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpabsTblGuid
{
	AgrBi1nOwnpabsTblGuidLeftBase ownVar;
	AgrBi1nOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpabsTblGuidLeftDerived();
		otherVar = new AgrBi1nOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpabsTblGuid
{
	AgrBinnOwnpabsTblGuidLeftBase ownVar;
	AgrBinnOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpabsTblGuidLeftDerived();
		otherVar = new AgrBinnOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpabsTblGuidRight>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsNoTblGuid
{
	CmpDir1OwnpabsNoTblGuidLeftBase ownVar;
	CmpDir1OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsNoTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsTblGuid
{
	CmpDir1OwnpabsTblGuidLeftBase ownVar;
	CmpDir1OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsNoTblGuid
{
	CmpBi11OwnpabsNoTblGuidLeftBase ownVar;
	CmpBi11OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsNoTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsTblGuid
{
	CmpBi11OwnpabsTblGuidLeftBase ownVar;
	CmpBi11OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsNoTblGuid
{
	CmpDirnOwnpabsNoTblGuidLeftBase ownVar;
	CmpDirnOwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsNoTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsTblGuid
{
	CmpDirnOwnpabsTblGuidLeftBase ownVar;
	CmpDirnOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsNoTblGuid
{
	CmpBin1OwnpabsNoTblGuidLeftBase ownVar;
	CmpBin1OwnpabsNoTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsNoTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpabsNoTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsNoTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsNoTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsNoTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsTblGuid
{
	CmpBin1OwnpabsTblGuidLeftBase ownVar;
	CmpBin1OwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpabsTblGuid
{
	CmpBi1nOwnpabsTblGuidLeftBase ownVar;
	CmpBi1nOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpabsTblGuidLeftDerived();
		otherVar = new CmpBi1nOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpabsTblGuid
{
	CmpBinnOwnpabsTblGuidLeftBase ownVar;
	CmpBinnOwnpabsTblGuidRight otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpabsTblGuidLeftDerived();
		otherVar = new CmpBinnOwnpabsTblGuidRight();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpabsTblGuidRight>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpabsTblGuidRight>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblGuidRight));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpabsTblGuidRight>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpabsNoTblGuid
{
	AgrDir1OthpabsNoTblGuidLeft ownVar;
	AgrDir1OthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpabsNoTblGuidLeft();
		otherVar = new AgrDir1OthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpabsNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OthpabsTblGuid
{
	AgrDir1OthpabsTblGuidLeft ownVar;
	AgrDir1OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OthpabsTblGuidLeft();
		otherVar = new AgrDir1OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpabsNoTblGuid
{
	AgrBi11OthpabsNoTblGuidLeft ownVar;
	AgrBi11OthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpabsNoTblGuidLeft();
		otherVar = new AgrBi11OthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpabsNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OthpabsTblGuid
{
	AgrBi11OthpabsTblGuidLeft ownVar;
	AgrBi11OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OthpabsTblGuidLeft();
		otherVar = new AgrBi11OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOthpabsTblGuid
{
	AgrDirnOthpabsTblGuidLeft ownVar;
	AgrDirnOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOthpabsTblGuidLeft();
		otherVar = new AgrDirnOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OthpabsTblGuid
{
	AgrBin1OthpabsTblGuidLeft ownVar;
	AgrBin1OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OthpabsTblGuidLeft();
		otherVar = new AgrBin1OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpabsNoTblGuid
{
	AgrBi1nOthpabsNoTblGuidLeft ownVar;
	AgrBi1nOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpabsNoTblGuidLeft();
		otherVar = new AgrBi1nOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpabsNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOthpabsTblGuid
{
	AgrBi1nOthpabsTblGuidLeft ownVar;
	AgrBi1nOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOthpabsTblGuidLeft();
		otherVar = new AgrBi1nOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOthpabsTblGuid
{
	AgrBinnOthpabsTblGuidLeft ownVar;
	AgrBinnOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOthpabsTblGuidLeft();
		otherVar = new AgrBinnOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpabsNoTblGuid
{
	CmpDir1OthpabsNoTblGuidLeft ownVar;
	CmpDir1OthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpabsNoTblGuidLeft();
		otherVar = new CmpDir1OthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OthpabsTblGuid
{
	CmpDir1OthpabsTblGuidLeft ownVar;
	CmpDir1OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OthpabsTblGuidLeft();
		otherVar = new CmpDir1OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpabsNoTblGuid
{
	CmpBi11OthpabsNoTblGuidLeft ownVar;
	CmpBi11OthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpabsNoTblGuidLeft();
		otherVar = new CmpBi11OthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OthpabsTblGuid
{
	CmpBi11OthpabsTblGuidLeft ownVar;
	CmpBi11OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OthpabsTblGuidLeft();
		otherVar = new CmpBi11OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOthpabsTblGuid
{
	CmpDirnOthpabsTblGuidLeft ownVar;
	CmpDirnOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOthpabsTblGuidLeft();
		otherVar = new CmpDirnOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OthpabsTblGuid
{
	CmpBin1OthpabsTblGuidLeft ownVar;
	CmpBin1OthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OthpabsTblGuidLeft();
		otherVar = new CmpBin1OthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpabsNoTblGuid
{
	CmpBi1nOthpabsNoTblGuidLeft ownVar;
	CmpBi1nOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpabsNoTblGuidLeft();
		otherVar = new CmpBi1nOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpabsNoTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpabsNoTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpabsNoTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOthpabsTblGuid
{
	CmpBi1nOthpabsTblGuidLeft ownVar;
	CmpBi1nOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOthpabsTblGuidLeft();
		otherVar = new CmpBi1nOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOthpabsTblGuid
{
	CmpBinnOthpabsTblGuidLeft ownVar;
	CmpBinnOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOthpabsTblGuidLeft();
		otherVar = new CmpBinnOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOthpabsTblGuidLeft>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOthpabsTblGuidLeft>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblGuidLeft));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOthpabsTblGuidLeft>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsOthpabsNoTblGuid
{
	AgrDir1OwnpabsOthpabsNoTblGuidLeftBase ownVar;
	AgrDir1OwnpabsOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsOthpabsNoTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpabsOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsOthpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsOthpabsNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDir1OwnpabsOthpabsTblGuid
{
	AgrDir1OwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrDir1OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDir1OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrDir1OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDir1OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDir1OwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDir1OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDir1OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsOthpabsNoTblGuid
{
	AgrBi11OwnpabsOthpabsNoTblGuidLeftBase ownVar;
	AgrBi11OwnpabsOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsOthpabsNoTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpabsOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsOthpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsOthpabsNoTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi11OwnpabsOthpabsTblGuid
{
	AgrBi11OwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrBi11OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi11OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrBi11OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi11OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi11OwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi11OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi11OwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi11OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrDirnOwnpabsOthpabsTblGuid
{
	AgrDirnOwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrDirnOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrDirnOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrDirnOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrDirnOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrDirnOwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrDirnOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrDirnOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrDirnOwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrDirnOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrDirnOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBin1OwnpabsOthpabsTblGuid
{
	AgrBin1OwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrBin1OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBin1OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrBin1OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBin1OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBin1OwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBin1OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBin1OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBin1OwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBin1OwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBin1OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBin1OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBi1nOwnpabsOthpabsTblGuid
{
	AgrBi1nOwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrBi1nOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBi1nOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrBi1nOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBi1nOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBi1nOwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBi1nOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBi1nOwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBi1nOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestAgrBinnOwnpabsOthpabsTblGuid
{
	AgrBinnOwnpabsOthpabsTblGuidLeftBase ownVar;
	AgrBinnOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new AgrBinnOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new AgrBinnOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<AgrBinnOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			var m = pm.Objects<AgrBinnOwnpabsOthpabsTblGuidRightBase>().ResultTable;
			pm.Delete(m);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<AgrBinnOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<AgrBinnOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<AgrBinnOwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(AgrBinnOwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		pm.MakePersistent(otherVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<AgrBinnOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<AgrBinnOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsOthpabsNoTblGuid
{
	CmpDir1OwnpabsOthpabsNoTblGuidLeftBase ownVar;
	CmpDir1OwnpabsOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsOthpabsNoTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpabsOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsOthpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDir1OwnpabsOthpabsTblGuid
{
	CmpDir1OwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpDir1OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDir1OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpDir1OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDir1OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDir1OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDir1OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsOthpabsNoTblGuid
{
	CmpBi11OwnpabsOthpabsNoTblGuidLeftBase ownVar;
	CmpBi11OwnpabsOthpabsNoTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsOthpabsNoTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpabsOthpabsNoTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsOthpabsNoTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsNoTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsNoTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsNoTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsNoTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsNoTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi11OwnpabsOthpabsTblGuid
{
	CmpBi11OwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpBi11OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi11OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpBi11OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi11OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi11OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi11OwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi11OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpDirnOwnpabsOthpabsTblGuid
{
	CmpDirnOwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpDirnOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpDirnOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpDirnOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpDirnOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpDirnOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpDirnOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpDirnOwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpDirnOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpDirnOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBin1OwnpabsOthpabsTblGuid
{
	CmpBin1OwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpBin1OwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBin1OwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpBin1OwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBin1OwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBin1OwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBin1OwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBin1OwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestChangeKeyHolderRight()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		int x = otherVar.RelField.Dummy;
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderRightNoTouch()
	{
		CreateObjects();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.RelField, "No related object");
		otherVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOther();
		Assert.NotNull(otherVar, "No Query Result");
		Assert.NotNull(otherVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(otherVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBin1OwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBin1OwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBin1OwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBi1nOwnpabsOthpabsTblGuid
{
	CmpBi1nOwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpBi1nOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBi1nOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpBi1nOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBi1nOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBi1nOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.RelField = null;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.Null(ownVar.RelField, "There should be no object");
	}
	[Test]
	public void TestChangeKeyHolderLeft()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		int x = ownVar.RelField.Dummy;
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestChangeKeyHolderLeftNoTouch()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
		ownVar.Dummy = 4711;
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.Dummy == 4711, "Wrong value");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBi1nOwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBi1nOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}


[TestFixture]
public class TestCmpBinnOwnpabsOthpabsTblGuid
{
	CmpBinnOwnpabsOthpabsTblGuidLeftBase ownVar;
	CmpBinnOwnpabsOthpabsTblGuidRightBase otherVar;
	PersistenceManager pm;
	[SetUp]
	public void Setup()
	{
		pm = PmFactory.NewPersistenceManager();
		ownVar = new CmpBinnOwnpabsOthpabsTblGuidLeftDerived();
		otherVar = new CmpBinnOwnpabsOthpabsTblGuidRightDerived();
	}
	[TearDown]
	public void TearDown()
	{
		try
		{
			pm.UnloadCache();
			var l = pm.Objects<CmpBinnOwnpabsOthpabsTblGuidLeftBase>().ResultTable;
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			decimal count;
			count = (decimal) new NDOQuery<CmpBinnOwnpabsOthpabsTblGuidLeftBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #1");
			count = (decimal) new NDOQuery<CmpBinnOwnpabsOthpabsTblGuidRightBase>(pm).ExecuteAggregate("dummy", AggregateType.Count);
			Assert.AreEqual(0, count, "Count wrong #2");
		}
		catch (Exception)
		{
			var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}");
			handler.Execute($"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}");
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}" );
		}
	}
	[Test]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadNull()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RelField = new List<CmpBinnOwnpabsOthpabsTblGuidRightBase>();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestSaveReloadRemove()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(1, ownVar.RelField.Count, "Count wrong");
		ownVar.RemoveRelatedObject();
		pm.Save();
		pm.UnloadCache();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.AreEqual(0, ownVar.RelField.Count, "Count wrong");
	}
	[Test]
	public void TestRelationHash()
	{
		Class clbaseLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftBase));
		Relation relbaseLeft = clbaseLeft.FindRelation("relField");
		Class clbaseRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblGuidRightBase));
		Relation relbaseRight = clbaseRight.FindRelation("relField");
		Assert.That(relbaseRight.Equals(relbaseLeft), "Relation should be equal #1");
		Assert.That(relbaseLeft.Equals(relbaseRight), "Relation should be equal #2");
		Class clderLeft = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftDerived));
		Relation relderLeft = clderLeft.FindRelation("relField");
		Assert.That(relderLeft.Equals(relbaseRight), "Relation should be equal #3");
		Assert.That(relbaseRight.Equals(relderLeft), "Relation should be equal #4");
		Class clderRight = pm.NDOMapping.FindClass(typeof(CmpBinnOwnpabsOthpabsTblGuidRightDerived));
		Relation relderRight = clderRight.FindRelation("relField");
		Assert.That(relbaseLeft.Equals(relderRight), "Relation should be equal #5");
		Assert.That(relderRight.Equals(relbaseLeft), "Relation should be equal #6");
		Assert.That(relderLeft.Equals(relderRight), "Relation should be equal #7");
		Assert.That(relderRight.Equals(relderLeft), "Relation should be equal #8");
	}
	void CreateObjects()
	{
		pm.MakePersistent(ownVar);
		ownVar.AssignRelation(otherVar);
		pm.Save();
		pm.UnloadCache();
	}
	void QueryOwn()
	{
		var q = new NDOQuery<CmpBinnOwnpabsOthpabsTblGuidLeftBase>(pm);
		ownVar = q.ExecuteSingle();
	}
	void QueryOther()
	{
		var q = new NDOQuery<CmpBinnOwnpabsOthpabsTblGuidRightBase>(pm);
		otherVar = q.ExecuteSingle();
	}
}



}
