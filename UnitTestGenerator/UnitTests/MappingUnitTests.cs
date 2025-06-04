//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using NDO;
using NDO.Mapping;
using NUnit.Framework;
using NdoUnitTests;

namespace MappingUnitTests
{

public class PmFactory
{
	static PersistenceManager pm;
	public static PersistenceManager NewPersistenceManager()
	{
		if (pm == null)
		{
			pm = new PersistenceManager(@"C:\Projekte\NDO5\UnitTestGenerator\UnitTests\bin\Debug\NDOMapping.xml");
		}
		else
		{
			pm.UnloadCache();
		}
		return pm;
	}
}


[TestFixture]
public class MappingTestAgrDir1NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDir1TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi11NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi11TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDirnNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnNoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDirnTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBin1NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBin1TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi1nNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nNoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi1nTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBinnTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDir1NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDir1TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi11NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi11TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDirnNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnNoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDirnTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBin1NoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1NoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1NoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBin1TblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1TblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1TblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi1nNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nNoTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi1nTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBinnTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnTblAutoRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconOthpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconOthpconNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpconOthpconTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDir1TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi11NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi11TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDirnNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnNoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDirnTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBin1NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBin1TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi1nNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nNoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBi1nTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrBinnTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDir1NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDir1TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi11NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi11TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDirnNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnNoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpDirnTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBin1NoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1NoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1NoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBin1TblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1TblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1TblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi1nNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nNoTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBi1nTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestCmpBinnTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnTblGuidRight" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconOthpconNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpconOthpconTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpconOthpconTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblAutoRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblAutoLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblAutoLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblAutoRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsOthpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsOthpabsNoTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpabsOthpabsTblAuto : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblAutoLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblAutoRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblAutoLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblAutoRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsNoTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblGuidRight" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsTblGuidLeftDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.otherClass.TypeCode != 0, "Class should have a Type Code #2");
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsNoTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsNoTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblGuidLeft" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblGuidRightBase" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #1");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Null, "Relation shouldn't have a TypeColumn #2");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownClass.TypeCode != 0, "Class should have a Type Code #1");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDir1OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDir1OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi11OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi11OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrDirnOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrDirnOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBin1OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBin1OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBi1nOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBi1nOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestAgrBinnOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.AgrBinnOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDir1OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDir1OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsOthpabsNoTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsNoTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsNoTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsNoTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsNoTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #5");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #6");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Null, "Relation shouldn't have a MappingTable #8");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #9");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #10");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi11OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi11OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpDirnOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpDirnOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBin1OwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBin1OwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBi1nOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBi1nOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}


[TestFixture]
public class MappingTestCmpBinnOwnpabsOthpabsTblGuid : NDOTest
{
	PersistenceManager pm;
	NDOMapping mapping;
	Class ownClass;
	Class otherClass;
	Class ownDerivedClass;
	Class otherDerivedClass;
	[SetUp]
	public void Setup()
	{
		this.pm = PmFactory.NewPersistenceManager();
		this.mapping = pm.NDOMapping;
		this.ownClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblGuidLeftBase" );
		this.otherClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblGuidRightBase" );
		this.ownDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblGuidLeftDerived" );
		this.otherDerivedClass = this.mapping.FindClass( "RelationTestClasses.CmpBinnOwnpabsOthpabsTblGuidRightDerived" );
	}
	[Test]
	public void HasMappingTable()
	{
		Assert.That(ownClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #1");
		Assert.That(ownDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #2");
		Assert.That(otherClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #3");
		Assert.That(otherDerivedClass.Relations.First().MappingTable, Is.Not.Null, "Relation should have a MappingTable #4");
	}
	[Test]
	public void HasTypeColumn()
	{
		Assert.That(ownClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #1");
		Assert.That(ownDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #2");
		Assert.That(ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #3");
		Assert.That(ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #4");
		Assert.That(otherClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #5");
		Assert.That(otherDerivedClass.Relations.First().ForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #6");
		Assert.That(otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #7");
		Assert.That(otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName, Is.Not.Null, "Relation should have a TypeColumn #8");
	}
	[Test]
	public void HasTypeCode()
	{
		Assert.That(this.ownDerivedClass.TypeCode != 0, "Class should have a Type Code #3");
		Assert.That(this.otherDerivedClass.TypeCode != 0, "Class should have a Type Code #4");
	}
}



}
