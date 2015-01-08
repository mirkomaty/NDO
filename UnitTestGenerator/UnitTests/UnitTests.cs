using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using NDO;
using NDO.Mapping;
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1NoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1NoTblAutoLeft));
		ownVar = (AgrDir1NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1NoTblAutoRight));
		otherVar = (AgrDir1NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1TblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1TblAutoLeft));
		ownVar = (AgrDir1TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1TblAutoRight));
		otherVar = (AgrDir1TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11NoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11NoTblAutoLeft));
		ownVar = (AgrBi11NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11NoTblAutoRight));
		otherVar = (AgrBi11NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11TblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11TblAutoLeft));
		ownVar = (AgrBi11TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11TblAutoRight));
		otherVar = (AgrBi11TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnNoTblAutoLeft));
		ownVar = (AgrDirnNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnNoTblAutoRight));
		otherVar = (AgrDirnNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnTblAutoLeft));
		ownVar = (AgrDirnTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnTblAutoRight));
		otherVar = (AgrDirnTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1NoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1NoTblAutoLeft));
		ownVar = (AgrBin1NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1NoTblAutoRight));
		otherVar = (AgrBin1NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1TblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1TblAutoLeft));
		ownVar = (AgrBin1TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1TblAutoRight));
		otherVar = (AgrBin1TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nNoTblAutoLeft));
		ownVar = (AgrBi1nNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nNoTblAutoRight));
		otherVar = (AgrBi1nNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nTblAutoLeft));
		ownVar = (AgrBi1nTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nTblAutoRight));
		otherVar = (AgrBi1nTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnTblAutoLeft));
		ownVar = (AgrBinnTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnTblAutoRight));
		otherVar = (AgrBinnTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1NoTblAutoLeft));
		ownVar = (CmpDir1NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1NoTblAutoRight));
		otherVar = (CmpDir1NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1TblAutoLeft));
		ownVar = (CmpDir1TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1TblAutoRight));
		otherVar = (CmpDir1TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11NoTblAutoLeft));
		ownVar = (CmpBi11NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11NoTblAutoRight));
		otherVar = (CmpBi11NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11TblAutoLeft));
		ownVar = (CmpBi11TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11TblAutoRight));
		otherVar = (CmpBi11TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnNoTblAutoLeft));
		ownVar = (CmpDirnNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnNoTblAutoRight));
		otherVar = (CmpDirnNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnTblAutoLeft));
		ownVar = (CmpDirnTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnTblAutoRight));
		otherVar = (CmpDirnTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1NoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1NoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1NoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1NoTblAutoLeft));
		ownVar = (CmpBin1NoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1NoTblAutoRight));
		otherVar = (CmpBin1NoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1TblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1TblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1TblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1TblAutoLeft));
		ownVar = (CmpBin1TblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1TblAutoRight));
		otherVar = (CmpBin1TblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nNoTblAutoLeft));
		ownVar = (CmpBi1nNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nNoTblAutoRight));
		otherVar = (CmpBi1nNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nTblAutoLeft));
		ownVar = (CmpBi1nTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nTblAutoRight));
		otherVar = (CmpBi1nTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnTblAutoLeft));
		ownVar = (CmpBinnTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnTblAutoRight));
		otherVar = (CmpBinnTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoLeftBase));
		ownVar = (AgrDir1OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconNoTblAutoRight));
		otherVar = (AgrDir1OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconTblAutoLeftBase));
		ownVar = (AgrDir1OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconTblAutoRight));
		otherVar = (AgrDir1OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoLeftBase));
		ownVar = (AgrBi11OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconNoTblAutoRight));
		otherVar = (AgrBi11OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconTblAutoLeftBase));
		ownVar = (AgrBi11OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconTblAutoRight));
		otherVar = (AgrBi11OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoLeftBase));
		ownVar = (AgrDirnOwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconNoTblAutoRight));
		otherVar = (AgrDirnOwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconTblAutoLeftBase));
		ownVar = (AgrDirnOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconTblAutoRight));
		otherVar = (AgrDirnOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoLeftBase));
		ownVar = (AgrBin1OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconNoTblAutoRight));
		otherVar = (AgrBin1OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconTblAutoLeftBase));
		ownVar = (AgrBin1OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconTblAutoRight));
		otherVar = (AgrBin1OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoLeftBase));
		ownVar = (AgrBi1nOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconTblAutoRight));
		otherVar = (AgrBi1nOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpconTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconTblAutoLeftBase));
		ownVar = (AgrBinnOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconTblAutoRight));
		otherVar = (AgrBinnOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconNoTblAutoLeftBase));
		ownVar = (CmpDir1OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconNoTblAutoRight));
		otherVar = (CmpDir1OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconTblAutoLeftBase));
		ownVar = (CmpDir1OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconTblAutoRight));
		otherVar = (CmpDir1OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconNoTblAutoLeftBase));
		ownVar = (CmpBi11OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconNoTblAutoRight));
		otherVar = (CmpBi11OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconTblAutoLeftBase));
		ownVar = (CmpBi11OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconTblAutoRight));
		otherVar = (CmpBi11OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconNoTblAutoLeftBase));
		ownVar = (CmpDirnOwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconNoTblAutoRight));
		otherVar = (CmpDirnOwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconTblAutoLeftBase));
		ownVar = (CmpDirnOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconTblAutoRight));
		otherVar = (CmpDirnOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconNoTblAutoLeftBase));
		ownVar = (CmpBin1OwnpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconNoTblAutoRight));
		otherVar = (CmpBin1OwnpconNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconTblAutoLeftBase));
		ownVar = (CmpBin1OwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconTblAutoRight));
		otherVar = (CmpBin1OwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconTblAutoLeftBase));
		ownVar = (CmpBi1nOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconTblAutoRight));
		otherVar = (CmpBi1nOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconTblAutoLeftBase));
		ownVar = (CmpBinnOwnpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconTblAutoRight));
		otherVar = (CmpBinnOwnpconTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoLeft));
		ownVar = (AgrDir1OthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpconNoTblAutoRightBase));
		otherVar = (AgrDir1OthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpconTblAutoLeft));
		ownVar = (AgrDir1OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpconTblAutoRightBase));
		otherVar = (AgrDir1OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoLeft));
		ownVar = (AgrBi11OthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpconNoTblAutoRightBase));
		otherVar = (AgrBi11OthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpconTblAutoLeft));
		ownVar = (AgrBi11OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpconTblAutoRightBase));
		otherVar = (AgrBi11OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOthpconTblAutoLeft));
		ownVar = (AgrDirnOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOthpconTblAutoRightBase));
		otherVar = (AgrDirnOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OthpconTblAutoLeft));
		ownVar = (AgrBin1OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OthpconTblAutoRightBase));
		otherVar = (AgrBin1OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoLeft));
		ownVar = (AgrBi1nOthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconNoTblAutoRightBase));
		otherVar = (AgrBi1nOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconTblAutoLeft));
		ownVar = (AgrBi1nOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconTblAutoRightBase));
		otherVar = (AgrBi1nOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOthpconTblAutoLeft));
		ownVar = (AgrBinnOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOthpconTblAutoRightBase));
		otherVar = (AgrBinnOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpconNoTblAutoLeft));
		ownVar = (CmpDir1OthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpconNoTblAutoRightBase));
		otherVar = (CmpDir1OthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpconTblAutoLeft));
		ownVar = (CmpDir1OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpconTblAutoRightBase));
		otherVar = (CmpDir1OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpconNoTblAutoLeft));
		ownVar = (CmpBi11OthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpconNoTblAutoRightBase));
		otherVar = (CmpBi11OthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpconTblAutoLeft));
		ownVar = (CmpBi11OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpconTblAutoRightBase));
		otherVar = (CmpBi11OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOthpconTblAutoLeft));
		ownVar = (CmpDirnOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOthpconTblAutoRightBase));
		otherVar = (CmpDirnOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OthpconTblAutoLeft));
		ownVar = (CmpBin1OthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OthpconTblAutoRightBase));
		otherVar = (CmpBin1OthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpconNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconNoTblAutoLeft));
		ownVar = (CmpBi1nOthpconNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconNoTblAutoRightBase));
		otherVar = (CmpBi1nOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconTblAutoLeft));
		ownVar = (CmpBi1nOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconTblAutoRightBase));
		otherVar = (CmpBi1nOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOthpconTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpconTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOthpconTblAutoLeft));
		ownVar = (CmpBinnOthpconTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOthpconTblAutoRightBase));
		otherVar = (CmpBinnOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoLeftBase));
		ownVar = (AgrDir1OwnpconOthpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblAutoRightBase));
		otherVar = (AgrDir1OwnpconOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrDir1OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblAutoRightBase));
		otherVar = (AgrDir1OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoLeftBase));
		ownVar = (AgrBi11OwnpconOthpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblAutoRightBase));
		otherVar = (AgrBi11OwnpconOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrBi11OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblAutoRightBase));
		otherVar = (AgrBi11OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrDirnOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblAutoRightBase));
		otherVar = (AgrDirnOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrBin1OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblAutoRightBase));
		otherVar = (AgrBin1OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrBi1nOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblAutoRightBase));
		otherVar = (AgrBi1nOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoLeftBase));
		ownVar = (AgrBinnOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblAutoRightBase));
		otherVar = (AgrBinnOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblAutoLeftBase));
		ownVar = (CmpDir1OwnpconOthpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblAutoRightBase));
		otherVar = (CmpDir1OwnpconOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpDir1OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblAutoRightBase));
		otherVar = (CmpDir1OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblAutoLeftBase));
		ownVar = (CmpBi11OwnpconOthpconNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblAutoRightBase));
		otherVar = (CmpBi11OwnpconOthpconNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpBi11OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblAutoRightBase));
		otherVar = (CmpBi11OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpDirnOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblAutoRightBase));
		otherVar = (CmpDirnOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpBin1OwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblAutoRightBase));
		otherVar = (CmpBin1OwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpBi1nOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblAutoRightBase));
		otherVar = (CmpBi1nOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblAutoLeftBase));
		ownVar = (CmpBinnOwnpconOthpconTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblAutoRightBase));
		otherVar = (CmpBinnOwnpconOthpconTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1NoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1NoTblGuidLeft));
		ownVar = (AgrDir1NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1NoTblGuidRight));
		otherVar = (AgrDir1NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1TblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1TblGuidLeft));
		ownVar = (AgrDir1TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1TblGuidRight));
		otherVar = (AgrDir1TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11NoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11NoTblGuidLeft));
		ownVar = (AgrBi11NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11NoTblGuidRight));
		otherVar = (AgrBi11NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11TblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11TblGuidLeft));
		ownVar = (AgrBi11TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11TblGuidRight));
		otherVar = (AgrBi11TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnNoTblGuidLeft));
		ownVar = (AgrDirnNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnNoTblGuidRight));
		otherVar = (AgrDirnNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnTblGuidLeft));
		ownVar = (AgrDirnTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnTblGuidRight));
		otherVar = (AgrDirnTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1NoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1NoTblGuidLeft));
		ownVar = (AgrBin1NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1NoTblGuidRight));
		otherVar = (AgrBin1NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1TblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1TblGuidLeft));
		ownVar = (AgrBin1TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1TblGuidRight));
		otherVar = (AgrBin1TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nNoTblGuidLeft));
		ownVar = (AgrBi1nNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nNoTblGuidRight));
		otherVar = (AgrBi1nNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nTblGuidLeft));
		ownVar = (AgrBi1nTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nTblGuidRight));
		otherVar = (AgrBi1nTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnTblGuidLeft));
		ownVar = (AgrBinnTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnTblGuidRight));
		otherVar = (AgrBinnTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1NoTblGuidLeft));
		ownVar = (CmpDir1NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1NoTblGuidRight));
		otherVar = (CmpDir1NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1TblGuidLeft));
		ownVar = (CmpDir1TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1TblGuidRight));
		otherVar = (CmpDir1TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11NoTblGuidLeft));
		ownVar = (CmpBi11NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11NoTblGuidRight));
		otherVar = (CmpBi11NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11TblGuidLeft));
		ownVar = (CmpBi11TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11TblGuidRight));
		otherVar = (CmpBi11TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnNoTblGuidLeft));
		ownVar = (CmpDirnNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnNoTblGuidRight));
		otherVar = (CmpDirnNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnTblGuidLeft));
		ownVar = (CmpDirnTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnTblGuidRight));
		otherVar = (CmpDirnTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1NoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1NoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1NoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1NoTblGuidLeft));
		ownVar = (CmpBin1NoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1NoTblGuidRight));
		otherVar = (CmpBin1NoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1TblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1TblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1TblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1TblGuidLeft));
		ownVar = (CmpBin1TblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1TblGuidRight));
		otherVar = (CmpBin1TblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nNoTblGuidLeft));
		ownVar = (CmpBi1nNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nNoTblGuidRight));
		otherVar = (CmpBi1nNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nTblGuidLeft));
		ownVar = (CmpBi1nTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nTblGuidRight));
		otherVar = (CmpBi1nTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnTblGuidLeft));
		ownVar = (CmpBinnTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnTblGuidRight));
		otherVar = (CmpBinnTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidLeftBase));
		ownVar = (AgrDir1OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconNoTblGuidRight));
		otherVar = (AgrDir1OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconTblGuidLeftBase));
		ownVar = (AgrDir1OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconTblGuidRight));
		otherVar = (AgrDir1OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidLeftBase));
		ownVar = (AgrBi11OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconNoTblGuidRight));
		otherVar = (AgrBi11OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconTblGuidLeftBase));
		ownVar = (AgrBi11OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconTblGuidRight));
		otherVar = (AgrBi11OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidLeftBase));
		ownVar = (AgrDirnOwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconNoTblGuidRight));
		otherVar = (AgrDirnOwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconTblGuidLeftBase));
		ownVar = (AgrDirnOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconTblGuidRight));
		otherVar = (AgrDirnOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidLeftBase));
		ownVar = (AgrBin1OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconNoTblGuidRight));
		otherVar = (AgrBin1OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconTblGuidLeftBase));
		ownVar = (AgrBin1OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconTblGuidRight));
		otherVar = (AgrBin1OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidLeftBase));
		ownVar = (AgrBi1nOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconTblGuidRight));
		otherVar = (AgrBi1nOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpconTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconTblGuidLeftBase));
		ownVar = (AgrBinnOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconTblGuidRight));
		otherVar = (AgrBinnOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconNoTblGuidLeftBase));
		ownVar = (CmpDir1OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconNoTblGuidRight));
		otherVar = (CmpDir1OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconTblGuidLeftBase));
		ownVar = (CmpDir1OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconTblGuidRight));
		otherVar = (CmpDir1OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconNoTblGuidLeftBase));
		ownVar = (CmpBi11OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconNoTblGuidRight));
		otherVar = (CmpBi11OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconTblGuidLeftBase));
		ownVar = (CmpBi11OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconTblGuidRight));
		otherVar = (CmpBi11OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconNoTblGuidLeftBase));
		ownVar = (CmpDirnOwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconNoTblGuidRight));
		otherVar = (CmpDirnOwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconTblGuidLeftBase));
		ownVar = (CmpDirnOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconTblGuidRight));
		otherVar = (CmpDirnOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconNoTblGuidLeftBase));
		ownVar = (CmpBin1OwnpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconNoTblGuidRight));
		otherVar = (CmpBin1OwnpconNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconTblGuidLeftBase));
		ownVar = (CmpBin1OwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconTblGuidRight));
		otherVar = (CmpBin1OwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconTblGuidLeftBase));
		ownVar = (CmpBi1nOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconTblGuidRight));
		otherVar = (CmpBi1nOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconTblGuidLeftBase));
		ownVar = (CmpBinnOwnpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconTblGuidRight));
		otherVar = (CmpBinnOwnpconTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidLeft));
		ownVar = (AgrDir1OthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpconNoTblGuidRightBase));
		otherVar = (AgrDir1OthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpconTblGuidLeft));
		ownVar = (AgrDir1OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpconTblGuidRightBase));
		otherVar = (AgrDir1OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidLeft));
		ownVar = (AgrBi11OthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpconNoTblGuidRightBase));
		otherVar = (AgrBi11OthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpconTblGuidLeft));
		ownVar = (AgrBi11OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpconTblGuidRightBase));
		otherVar = (AgrBi11OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOthpconTblGuidLeft));
		ownVar = (AgrDirnOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOthpconTblGuidRightBase));
		otherVar = (AgrDirnOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OthpconTblGuidLeft));
		ownVar = (AgrBin1OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OthpconTblGuidRightBase));
		otherVar = (AgrBin1OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidLeft));
		ownVar = (AgrBi1nOthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconNoTblGuidRightBase));
		otherVar = (AgrBi1nOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconTblGuidLeft));
		ownVar = (AgrBi1nOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpconTblGuidRightBase));
		otherVar = (AgrBi1nOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOthpconTblGuidLeft));
		ownVar = (AgrBinnOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOthpconTblGuidRightBase));
		otherVar = (AgrBinnOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpconNoTblGuidLeft));
		ownVar = (CmpDir1OthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpconNoTblGuidRightBase));
		otherVar = (CmpDir1OthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpconTblGuidLeft));
		ownVar = (CmpDir1OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpconTblGuidRightBase));
		otherVar = (CmpDir1OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpconNoTblGuidLeft));
		ownVar = (CmpBi11OthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpconNoTblGuidRightBase));
		otherVar = (CmpBi11OthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpconTblGuidLeft));
		ownVar = (CmpBi11OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpconTblGuidRightBase));
		otherVar = (CmpBi11OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOthpconTblGuidLeft));
		ownVar = (CmpDirnOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOthpconTblGuidRightBase));
		otherVar = (CmpDirnOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OthpconTblGuidLeft));
		ownVar = (CmpBin1OthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OthpconTblGuidRightBase));
		otherVar = (CmpBin1OthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpconNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconNoTblGuidLeft));
		ownVar = (CmpBi1nOthpconNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconNoTblGuidRightBase));
		otherVar = (CmpBi1nOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconTblGuidLeft));
		ownVar = (CmpBi1nOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpconTblGuidRightBase));
		otherVar = (CmpBi1nOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOthpconTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpconTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOthpconTblGuidLeft));
		ownVar = (CmpBinnOthpconTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOthpconTblGuidRightBase));
		otherVar = (CmpBinnOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidLeftBase));
		ownVar = (AgrDir1OwnpconOthpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconNoTblGuidRightBase));
		otherVar = (AgrDir1OwnpconOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrDir1OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpconOthpconTblGuidRightBase));
		otherVar = (AgrDir1OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidLeftBase));
		ownVar = (AgrBi11OwnpconOthpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconNoTblGuidRightBase));
		otherVar = (AgrBi11OwnpconOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrBi11OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpconOthpconTblGuidRightBase));
		otherVar = (AgrBi11OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrDirnOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpconOthpconTblGuidRightBase));
		otherVar = (AgrDirnOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrBin1OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpconOthpconTblGuidRightBase));
		otherVar = (AgrBin1OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrBi1nOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpconOthpconTblGuidRightBase));
		otherVar = (AgrBi1nOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidLeftBase));
		ownVar = (AgrBinnOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpconOthpconTblGuidRightBase));
		otherVar = (AgrBinnOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblGuidLeftBase));
		ownVar = (CmpDir1OwnpconOthpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconNoTblGuidRightBase));
		otherVar = (CmpDir1OwnpconOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpDir1OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpconOthpconTblGuidRightBase));
		otherVar = (CmpDir1OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblGuidLeftBase));
		ownVar = (CmpBi11OwnpconOthpconNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconNoTblGuidRightBase));
		otherVar = (CmpBi11OwnpconOthpconNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpBi11OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpconOthpconTblGuidRightBase));
		otherVar = (CmpBi11OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpDirnOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpconOthpconTblGuidRightBase));
		otherVar = (CmpDirnOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpBin1OwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpconOthpconTblGuidRightBase));
		otherVar = (CmpBin1OwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpBi1nOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpconOthpconTblGuidRightBase));
		otherVar = (CmpBi1nOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblGuidLeftBase));
		ownVar = (CmpBinnOwnpconOthpconTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpconOthpconTblGuidRightBase));
		otherVar = (CmpBinnOwnpconOthpconTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoLeftBase));
		ownVar = (AgrDir1OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblAutoRight));
		otherVar = (AgrDir1OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoLeftBase));
		ownVar = (AgrDir1OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsTblAutoRight));
		otherVar = (AgrDir1OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoLeftBase));
		ownVar = (AgrBi11OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblAutoRight));
		otherVar = (AgrBi11OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoLeftBase));
		ownVar = (AgrBi11OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsTblAutoRight));
		otherVar = (AgrBi11OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoLeftBase));
		ownVar = (AgrDirnOwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblAutoRight));
		otherVar = (AgrDirnOwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoLeftBase));
		ownVar = (AgrDirnOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsTblAutoRight));
		otherVar = (AgrDirnOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoLeftBase));
		ownVar = (AgrBin1OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblAutoRight));
		otherVar = (AgrBin1OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoLeftBase));
		ownVar = (AgrBin1OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsTblAutoRight));
		otherVar = (AgrBin1OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoLeftBase));
		ownVar = (AgrBi1nOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsTblAutoRight));
		otherVar = (AgrBi1nOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoLeftBase));
		ownVar = (AgrBinnOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsTblAutoRight));
		otherVar = (AgrBinnOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblAutoLeftBase));
		ownVar = (CmpDir1OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblAutoRight));
		otherVar = (CmpDir1OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsTblAutoLeftBase));
		ownVar = (CmpDir1OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsTblAutoRight));
		otherVar = (CmpDir1OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblAutoLeftBase));
		ownVar = (CmpBi11OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblAutoRight));
		otherVar = (CmpBi11OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsTblAutoLeftBase));
		ownVar = (CmpBi11OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsTblAutoRight));
		otherVar = (CmpBi11OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblAutoLeftBase));
		ownVar = (CmpDirnOwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblAutoRight));
		otherVar = (CmpDirnOwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsTblAutoLeftBase));
		ownVar = (CmpDirnOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsTblAutoRight));
		otherVar = (CmpDirnOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsNoTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblAutoLeftBase));
		ownVar = (CmpBin1OwnpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblAutoRight));
		otherVar = (CmpBin1OwnpabsNoTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsTblAutoLeftBase));
		ownVar = (CmpBin1OwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsTblAutoRight));
		otherVar = (CmpBin1OwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsTblAutoLeftBase));
		ownVar = (CmpBi1nOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsTblAutoRight));
		otherVar = (CmpBi1nOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsTblAutoRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsTblAutoLeftBase));
		ownVar = (CmpBinnOwnpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsTblAutoRight));
		otherVar = (CmpBinnOwnpabsTblAutoRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoLeft));
		ownVar = (AgrDir1OthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsNoTblAutoRightBase));
		otherVar = (AgrDir1OthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsTblAutoLeft));
		ownVar = (AgrDir1OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsTblAutoRightBase));
		otherVar = (AgrDir1OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoLeft));
		ownVar = (AgrBi11OthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsNoTblAutoRightBase));
		otherVar = (AgrBi11OthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsTblAutoLeft));
		ownVar = (AgrBi11OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsTblAutoRightBase));
		otherVar = (AgrBi11OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOthpabsTblAutoLeft));
		ownVar = (AgrDirnOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOthpabsTblAutoRightBase));
		otherVar = (AgrDirnOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OthpabsTblAutoLeft));
		ownVar = (AgrBin1OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OthpabsTblAutoRightBase));
		otherVar = (AgrBin1OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoLeft));
		ownVar = (AgrBi1nOthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblAutoRightBase));
		otherVar = (AgrBi1nOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoLeft));
		ownVar = (AgrBi1nOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsTblAutoRightBase));
		otherVar = (AgrBi1nOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOthpabsTblAutoLeft));
		ownVar = (AgrBinnOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOthpabsTblAutoRightBase));
		otherVar = (AgrBinnOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsNoTblAutoLeft));
		ownVar = (CmpDir1OthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsNoTblAutoRightBase));
		otherVar = (CmpDir1OthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsTblAutoLeft));
		ownVar = (CmpDir1OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsTblAutoRightBase));
		otherVar = (CmpDir1OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsNoTblAutoLeft));
		ownVar = (CmpBi11OthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsNoTblAutoRightBase));
		otherVar = (CmpBi11OthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsTblAutoLeft));
		ownVar = (CmpBi11OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsTblAutoRightBase));
		otherVar = (CmpBi11OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOthpabsTblAutoLeft));
		ownVar = (CmpDirnOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOthpabsTblAutoRightBase));
		otherVar = (CmpDirnOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OthpabsTblAutoLeft));
		ownVar = (CmpBin1OthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OthpabsTblAutoRightBase));
		otherVar = (CmpBin1OthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsNoTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblAutoLeft));
		ownVar = (CmpBi1nOthpabsNoTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblAutoRightBase));
		otherVar = (CmpBi1nOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsTblAutoLeft));
		ownVar = (CmpBi1nOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsTblAutoRightBase));
		otherVar = (CmpBi1nOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOthpabsTblAutoLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpabsTblAutoLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOthpabsTblAutoLeft));
		ownVar = (CmpBinnOthpabsTblAutoLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOthpabsTblAutoRightBase));
		otherVar = (CmpBinnOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoLeftBase));
		ownVar = (AgrDir1OwnpabsOthpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblAutoRightBase));
		otherVar = (AgrDir1OwnpabsOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrDir1OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrDir1OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoLeftBase));
		ownVar = (AgrBi11OwnpabsOthpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblAutoRightBase));
		otherVar = (AgrBi11OwnpabsOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrBi11OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrBi11OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrDirnOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrDirnOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrBin1OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrBin1OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrBi1nOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrBi1nOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (AgrBinnOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblAutoRightBase));
		otherVar = (AgrBinnOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblAutoLeftBase));
		ownVar = (CmpDir1OwnpabsOthpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblAutoRightBase));
		otherVar = (CmpDir1OwnpabsOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpDir1OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpDir1OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
	}
	[Test, ExpectedException(typeof(NDOException))]
	public void TestSaveReload()
	{
		CreateObjects();
		QueryOwn();
		Assert.NotNull(ownVar, "No Query Result");
		Assert.NotNull(ownVar.RelField, "No related object");
	}
	[Test, ExpectedException(typeof(NDOException))]
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblAutoLeftBase));
		ownVar = (CmpBi11OwnpabsOthpabsNoTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblAutoRightBase));
		otherVar = (CmpBi11OwnpabsOthpabsNoTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpBi11OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpBi11OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpDirnOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpDirnOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpBin1OwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpBin1OwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpBi1nOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpBi1nOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblAutoRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblAutoLeftBase));
		ownVar = (CmpBinnOwnpabsOthpabsTblAutoLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblAutoRightBase));
		otherVar = (CmpBinnOwnpabsOthpabsTblAutoRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidLeftBase));
		ownVar = (AgrDir1OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsNoTblGuidRight));
		otherVar = (AgrDir1OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidLeftBase));
		ownVar = (AgrDir1OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsTblGuidRight));
		otherVar = (AgrDir1OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidLeftBase));
		ownVar = (AgrBi11OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsNoTblGuidRight));
		otherVar = (AgrBi11OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidLeftBase));
		ownVar = (AgrBi11OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsTblGuidRight));
		otherVar = (AgrBi11OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidLeftBase));
		ownVar = (AgrDirnOwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsNoTblGuidRight));
		otherVar = (AgrDirnOwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidLeftBase));
		ownVar = (AgrDirnOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsTblGuidRight));
		otherVar = (AgrDirnOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidLeftBase));
		ownVar = (AgrBin1OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsNoTblGuidRight));
		otherVar = (AgrBin1OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidLeftBase));
		ownVar = (AgrBin1OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsTblGuidRight));
		otherVar = (AgrBin1OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidLeftBase));
		ownVar = (AgrBi1nOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsTblGuidRight));
		otherVar = (AgrBi1nOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidRight)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidLeftBase));
		ownVar = (AgrBinnOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsTblGuidRight));
		otherVar = (AgrBinnOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblGuidLeftBase));
		ownVar = (CmpDir1OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsNoTblGuidRight));
		otherVar = (CmpDir1OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsTblGuidLeftBase));
		ownVar = (CmpDir1OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsTblGuidRight));
		otherVar = (CmpDir1OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblGuidLeftBase));
		ownVar = (CmpBi11OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsNoTblGuidRight));
		otherVar = (CmpBi11OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsTblGuidLeftBase));
		ownVar = (CmpBi11OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsTblGuidRight));
		otherVar = (CmpBi11OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblGuidLeftBase));
		ownVar = (CmpDirnOwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsNoTblGuidRight));
		otherVar = (CmpDirnOwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsTblGuidLeftBase));
		ownVar = (CmpDirnOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsTblGuidRight));
		otherVar = (CmpDirnOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsNoTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblGuidLeftBase));
		ownVar = (CmpBin1OwnpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsNoTblGuidRight));
		otherVar = (CmpBin1OwnpabsNoTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsTblGuidLeftBase));
		ownVar = (CmpBin1OwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsTblGuidRight));
		otherVar = (CmpBin1OwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsTblGuidLeftBase));
		ownVar = (CmpBi1nOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsTblGuidRight));
		otherVar = (CmpBi1nOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsTblGuidRight)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsTblGuidLeftBase));
		ownVar = (CmpBinnOwnpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsTblGuidRight));
		otherVar = (CmpBinnOwnpabsTblGuidRight) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidLeft));
		ownVar = (AgrDir1OthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsNoTblGuidRightBase));
		otherVar = (AgrDir1OthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsTblGuidLeft));
		ownVar = (AgrDir1OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OthpabsTblGuidRightBase));
		otherVar = (AgrDir1OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidLeft));
		ownVar = (AgrBi11OthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsNoTblGuidRightBase));
		otherVar = (AgrBi11OthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsTblGuidLeft));
		ownVar = (AgrBi11OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OthpabsTblGuidRightBase));
		otherVar = (AgrBi11OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOthpabsTblGuidLeft));
		ownVar = (AgrDirnOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOthpabsTblGuidRightBase));
		otherVar = (AgrDirnOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OthpabsTblGuidLeft));
		ownVar = (AgrBin1OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OthpabsTblGuidRightBase));
		otherVar = (AgrBin1OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidLeft));
		ownVar = (AgrBi1nOthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsNoTblGuidRightBase));
		otherVar = (AgrBi1nOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidLeft));
		ownVar = (AgrBi1nOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOthpabsTblGuidRightBase));
		otherVar = (AgrBi1nOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOthpabsTblGuidLeft));
		ownVar = (AgrBinnOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOthpabsTblGuidRightBase));
		otherVar = (AgrBinnOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsNoTblGuidLeft));
		ownVar = (CmpDir1OthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsNoTblGuidRightBase));
		otherVar = (CmpDir1OthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsTblGuidLeft));
		ownVar = (CmpDir1OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OthpabsTblGuidRightBase));
		otherVar = (CmpDir1OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsNoTblGuidLeft));
		ownVar = (CmpBi11OthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsNoTblGuidRightBase));
		otherVar = (CmpBi11OthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsTblGuidLeft));
		ownVar = (CmpBi11OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OthpabsTblGuidRightBase));
		otherVar = (CmpBi11OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOthpabsTblGuidLeft));
		ownVar = (CmpDirnOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOthpabsTblGuidRightBase));
		otherVar = (CmpDirnOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OthpabsTblGuidLeft));
		ownVar = (CmpBin1OthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OthpabsTblGuidRightBase));
		otherVar = (CmpBin1OthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsNoTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblGuidLeft));
		ownVar = (CmpBi1nOthpabsNoTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsNoTblGuidRightBase));
		otherVar = (CmpBi1nOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsTblGuidLeft));
		ownVar = (CmpBi1nOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOthpabsTblGuidRightBase));
		otherVar = (CmpBi1nOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOthpabsTblGuidLeft)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpabsTblGuidLeft)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOthpabsTblGuidLeft));
		ownVar = (CmpBinnOthpabsTblGuidLeft) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOthpabsTblGuidRightBase));
		otherVar = (CmpBinnOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidLeftBase));
		ownVar = (AgrDir1OwnpabsOthpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsNoTblGuidRightBase));
		otherVar = (AgrDir1OwnpabsOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrDir1OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDir1OwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrDir1OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidLeftBase));
		ownVar = (AgrBi11OwnpabsOthpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsNoTblGuidRightBase));
		otherVar = (AgrBi11OwnpabsOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrBi11OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi11OwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrBi11OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrDirnOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrDirnOwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrDirnOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrBin1OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBin1OwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrBin1OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrBi1nOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBi1nOwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrBi1nOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		l = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidRightBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (AgrBinnOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(AgrBinnOwnpabsOthpabsTblGuidRightBase));
		otherVar = (AgrBinnOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblGuidLeftBase));
		ownVar = (CmpDir1OwnpabsOthpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsNoTblGuidRightBase));
		otherVar = (CmpDir1OwnpabsOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpDir1OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDir1OwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpDir1OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblGuidLeftBase));
		ownVar = (CmpBi11OwnpabsOthpabsNoTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsNoTblGuidRightBase));
		otherVar = (CmpBi11OwnpabsOthpabsNoTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpBi11OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi11OwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpBi11OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpDirnOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpDirnOwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpDirnOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpBin1OwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBin1OwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpBin1OwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
	public void TestSaveReloadRemove()
	{
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
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpBi1nOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBi1nOwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpBi1nOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
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
		pm.UnloadCache();
		IList l;
		l = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftBase)).Execute();
		pm.Delete(l);
		pm.Save();
		pm.UnloadCache();
		decimal count;
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #1");
		count = (decimal) pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblGuidRightBase)).ExecuteAggregate("dummy", Query.AggregateType.Count);
		Assert.AreEqual(0, count, "Count wrong #2");
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
		ownVar.RelField = new ArrayList();
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
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblGuidLeftBase));
		ownVar = (CmpBinnOwnpabsOthpabsTblGuidLeftBase) q.ExecuteSingle();
	}
	void QueryOther()
	{
		Query q = pm.NewQuery(typeof(CmpBinnOwnpabsOthpabsTblGuidRightBase));
		otherVar = (CmpBinnOwnpabsOthpabsTblGuidRightBase) q.ExecuteSingle();
	}
}



}
