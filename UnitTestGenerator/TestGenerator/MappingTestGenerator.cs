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
using System.Collections;
using System.IO;
using CodeGenerator;
using System.Text;
using System.Collections.Generic;

namespace TestGenerator
{
	/// <summary>
	/// Summary for TestGenerator.
	/// </summary>
	public class MappingTestGenerator
	{
		List<RelInfo> relInfos;
		string fileName;
		StreamWriter sw;
		TestFixture fixture;
		Test test;
		readonly string nameSpace = "MappingUnitTests";

		public MappingTestGenerator( List<RelInfo> relInfos )
		{
			this.relInfos = relInfos;
			fileName = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\UnitTests\MappingUnitTests.cs" );
		}


		void CreateTests( RelInfo ri )
		{
			CreateTestHasMappingTable( ri );
			CreateTestHasTypeColumn( ri );
			CreateTestHasTypeCode( ri );
		}


		string AssertEquals( string text, object o2, object o3 )
		{
			return $"Assert.That({o2.ToString().ToLower()}, Is.EqualTo({o3}), \"{text}\");";
		}
		string AssertNotNull( string text, object o )
		{
			return "Assert.That(" + o + ", Is.Not.Null, \"" + text + "\");";
		}
		string AssertNull( string text, object o )
		{
			return "Assert.That(" + o + ", Is.Null, \"" + text + "\");";
		}

		string Assert( string text, object o )
		{
			return "Assert.That(" + o + ", \"" + text + "\");";
		}

		string QualifiedClassName( string className )
		{
			return nameSpace + "." + className;
		}

		void CreateTestHasTypeCode( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "HasTypeCode" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";
			if (!(ri.IsAbstract && ri.OwnPoly)) // abstract classes don't need a type code
				func.Statements.Add( Assert("Class should have a Type Code #1", "this.ownClass.TypeCode != 0" ) );
			if (!(ri.IsAbstract && ri.OtherPoly))
				func.Statements.Add( Assert( "Class should have a Type Code #2", "this.otherClass.TypeCode != 0" ) );
			if (ri.OwnPoly)
				func.Statements.Add( Assert( "Class should have a Type Code #3", "this.ownDerivedClass.TypeCode != 0" ) );
			if (ri.OtherPoly)
				func.Statements.Add( Assert( "Class should have a Type Code #4", "this.otherDerivedClass.TypeCode != 0" ) );
		}

		void CreateTestHasMappingTable( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "HasMappingTable" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";
			if (ri.HasTable)
			{
				func.Statements.Add( AssertNotNull( "Relation should have a MappingTable #1", "ownClass.Relations.First().MappingTable" ) );
				if (ri.OwnPoly)
					func.Statements.Add( AssertNotNull( "Relation should have a MappingTable #2", "ownDerivedClass.Relations.First().MappingTable" ) );
				if (ri.IsBi)
				{
					func.Statements.Add( AssertNotNull( "Relation should have a MappingTable #3", "otherClass.Relations.First().MappingTable" ) );
					if (ri.OtherPoly)
						func.Statements.Add( AssertNotNull( "Relation should have a MappingTable #4", "otherDerivedClass.Relations.First().MappingTable" ) );
				}
			}
			else
			{
				func.Statements.Add( AssertNull( "Relation shouldn't have a MappingTable #5", "ownClass.Relations.First().MappingTable" ) );
				if (ri.OwnPoly)
					func.Statements.Add( AssertNull( "Relation shouldn't have a MappingTable #6", "ownDerivedClass.Relations.First().MappingTable" ) );
				if (ri.IsBi)
				{
					func.Statements.Add( AssertNull( "Relation shouldn't have a MappingTable #7", "otherClass.Relations.First().MappingTable" ) );
					if (ri.OtherPoly)
						func.Statements.Add( AssertNull( "Relation shouldn't have a MappingTable #8", "otherDerivedClass.Relations.First().MappingTable" ) );
				}
			}
		}

		void CreateTestHasTypeColumn( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "HasTypeColumn" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";
			if (ri.HasTable)
			{
				if (ri.OwnPoly)
				{
					func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #1", "ownClass.Relations.First().ForeignKeyTypeColumnName" ) );
					func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #2", "ownDerivedClass.Relations.First().ForeignKeyTypeColumnName" ) );
				}
				if (ri.OtherPoly)
				{
					func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #3", "ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
					if (ri.OwnPoly)
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #4", "ownDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
				}
				if (ri.IsBi)
				{
					if (ri.OtherPoly)
					{
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #5", "otherClass.Relations.First().ForeignKeyTypeColumnName" ) );
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #6", "otherDerivedClass.Relations.First().ForeignKeyTypeColumnName" ) );						
					}
					if (ri.OwnPoly)
					{
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #7", "otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
						if (ri.OtherPoly)
							func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #8", "otherDerivedClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
					}
				}
				if (!ri.OwnPoly)
				{
					func.Statements.Add( AssertNull( "Relation shouldn't have a TypeColumn #1", "ownClass.Relations.First().ForeignKeyTypeColumnName" ) );
					if (ri.IsBi)
					{
						func.Statements.Add( AssertNull( "Relation shouldn't have a TypeColumn #2", "otherClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
					}
				}
				if (!ri.OtherPoly)
				{
					func.Statements.Add( AssertNull( "Relation shouldn't have a TypeColumn #3", "ownClass.Relations.First().MappingTable.ChildForeignKeyTypeColumnName" ) );
					if (ri.IsBi)
					{
						func.Statements.Add( AssertNull( "Relation shouldn't have a TypeColumn #4", "otherClass.Relations.First().ForeignKeyTypeColumnName" ) );
					}
				}
			}
			else  // No Mapping Table
			{
				if (!ri.OtherPoly)
					func.Statements.Add( AssertNull( "Relation shouldn't have a TypeColumn #1", "ownClass.Relations.First().ForeignKeyTypeColumnName" ) );

				// Polymorphic 1:n relations always have a mapping table,
				// so we check only the 1:1 relations.
				if (!ri.IsList)
				{
					if (ri.OtherPoly)
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #9", "ownClass.Relations.First().ForeignKeyTypeColumnName" ) );
					if (ri.IsBi && ri.OwnPoly)
						func.Statements.Add( AssertNotNull( "Relation should have a TypeColumn #10", "otherClass.Relations.First().ForeignKeyTypeColumnName" ) );
				}
			}
		}

		bool IsForbiddenCase( RelInfo ri )
		{
			return ri.IsComposite && !ri.UseGuid && !ri.HasTable && !ri.IsList && ri.OtherPoly;
		}


		public void Generate()
		{
			sw = new StreamWriter( fileName );
			sw.WriteLine( @"//
// Copyright (c) 2002-2019 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the ""Software""), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

");
			sw.WriteLine( "using System;" );
			sw.WriteLine( "using System.Linq;" );
			sw.WriteLine( "using System.Diagnostics;" );
			sw.WriteLine( "using System.Collections;" );
			sw.WriteLine( "using System.Collections.Generic;" );
			sw.WriteLine( "using NDO;" );
			sw.WriteLine( "using NDO.Mapping;" );
			sw.WriteLine( "using NUnit.Framework;" );
			sw.WriteLine( "using NdoUnitTests;\n" );

			sw.WriteLine( "namespace " + nameSpace );
			sw.WriteLine( "{\n" );
			GeneratePmFactory();
			foreach (RelInfo ri in relInfos)
			{
				if (IsForbiddenCase( ri ))
					continue;

				GenerateTestGroup( ri );
			}
			sw.WriteLine( "\n}" );
			sw.Close();
		}

		void GenerateTestGroup( RelInfo ri )
		{
			fixture = new TestFixture( this.nameSpace, "MappingTest" + ri.ToString() );
			test = new Test( ri, "RelationTestClasses" );
			fixture.AddField( "PersistenceManager", "pm" );
			fixture.AddField( "NDOMapping", "mapping" );
			fixture.AddField( "Class", "ownClass" );
			fixture.AddField( "Class", "otherClass" );
			if (ri.OwnPoly)
				fixture.AddField( "Class", "ownDerivedClass" );
			if (ri.OtherPoly)
				fixture.AddField( "Class", "otherDerivedClass" );

			fixture.SetUp.Statements.Add( "this.pm = PmFactory.NewPersistenceManager();" );
			fixture.SetUp.Statements.Add( "this.mapping = pm.NDOMapping;" );
			fixture.SetUp.Statements.Add( $"this.ownClass = this.mapping.FindClass( \"{test.OwnClass.FullName}\" );" );
			fixture.SetUp.Statements.Add( $"this.otherClass = this.mapping.FindClass( \"{test.OtherClass.FullName}\" );" );
			if (ri.OwnPoly)
			fixture.SetUp.Statements.Add( $"this.ownDerivedClass = this.mapping.FindClass( \"{test.OwnDerivedClass.FullName}\" );" );
			if (ri.OtherPoly)
			fixture.SetUp.Statements.Add( $"this.otherDerivedClass = this.mapping.FindClass( \"{test.OtherDerivedClass.FullName}\" );" );

			//GenerateTearDown( ri );

			CreateTests( ri );

			this.sw.WriteLine( fixture.ToString() );
		}

		void GeneratePmFactory()
		{
			Class cl = new Class( this.nameSpace, "PmFactory" );
			string path = AppDomain.CurrentDomain.BaseDirectory;
			path = Path.Combine( path, @"..\..\UnitTests\bin\Debug\NDOMapping.xml" );
			path = Path.GetFullPath( path );
			cl.Statements.Add( "static PersistenceManager pm;" );
			Function func = cl.NewFunction( "PersistenceManager", "NewPersistenceManager" );
			func.IsStatic = true;
			func.AccessModifier = "public";

			func.Statements.Add( "if (pm == null)" );
			func.Statements.Add( "{" );
			func.Statements.Add( "pm = new PersistenceManager(@\"" + path + "\");" );
			path = Path.GetFullPath( Path.Combine( path, @"..\..\.." ) );
			func.Statements.Add( "}" );
			func.Statements.Add( "else" );
			func.Statements.Add( "{" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "}" );
			func.Statements.Add( "return pm;" );
			sw.WriteLine( cl.ToString() );
		}

	}
}
