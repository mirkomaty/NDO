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
	public class TestGenerator
	{
		List<RelInfo> relInfos;
		string fileName;
		StreamWriter sw;
		TestFixture fixture;
		Test test;
		readonly string nameSpace = "RelationUnitTests";

		public TestGenerator( List<RelInfo> relInfos )
		{
			this.relInfos = relInfos;
			fileName = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\UnitTests\UnitTests.cs" );
		}


		void CreateTests( RelInfo ri )
		{
			CreateTestSaveReload( ri );
			CreateTestSaveReloadNull( ri );
			CreateTestSaveReloadRemove( ri );
			CreateTestChangeKeyHolderLeft( ri );
			CreateTestChangeKeyHolderRight( ri );
			CreateTestChangeKeyHolderLeftNoTouch( ri );
			CreateTestChangeKeyHolderRightNoTouch( ri );
			CreateTestUpdateOrder( ri );
			CreateTestRelationHash( ri );
			GenerateCreateObjects( ri );
			GenerateQueryOwn( ri );
			GenerateQueryOther( ri );
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

		void CreateTestRelationHash( RelInfo ri )
		{
			if (!ri.IsBi)
				return;
			Function func = fixture.NewFunction( "void", "TestRelationHash" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";
			func.Statements.Add( "Class clbaseLeft = pm.NDOMapping.FindClass(typeof(" + test.OwnClass.Name + "));" );
			func.Statements.Add( "Relation relbaseLeft = clbaseLeft.FindRelation(\"relField\");" );
			func.Statements.Add( "Class clbaseRight = pm.NDOMapping.FindClass(typeof(" + test.OtherClass.Name + "));" );
			func.Statements.Add( "Relation relbaseRight = clbaseRight.FindRelation(\"relField\");" );
			func.Statements.Add( Assert( "Relation should be equal #1", "relbaseRight.Equals(relbaseLeft)" ) );
			func.Statements.Add( Assert( "Relation should be equal #2", "relbaseLeft.Equals(relbaseRight)" ) );
			if (ri.OwnPoly)
			{
				func.Statements.Add( "Class clderLeft = pm.NDOMapping.FindClass(typeof(" + test.OwnDerivedClass.Name + "));" );
				func.Statements.Add( "Relation relderLeft = clderLeft.FindRelation(\"relField\");" );
				func.Statements.Add( Assert( "Relation should be equal #3", "relderLeft.Equals(relbaseRight)" ) );
				func.Statements.Add( Assert( "Relation should be equal #4", "relbaseRight.Equals(relderLeft)" ) );
			}
			if (ri.OtherPoly)
			{
				func.Statements.Add( "Class clderRight = pm.NDOMapping.FindClass(typeof(" + test.OtherDerivedClass.Name + "));" );
				func.Statements.Add( "Relation relderRight = clderRight.FindRelation(\"relField\");" );
				func.Statements.Add( Assert( "Relation should be equal #5", "relbaseLeft.Equals(relderRight)" ) );
				func.Statements.Add( Assert( "Relation should be equal #6", "relderRight.Equals(relbaseLeft)" ) );
				if (ri.OwnPoly)
				{
					func.Statements.Add( Assert( "Relation should be equal #7", "relderLeft.Equals(relderRight)" ) );
					func.Statements.Add( Assert( "Relation should be equal #8", "relderRight.Equals(relderLeft)" ) );
				}
			}
		}

		bool IsForbiddenCase( RelInfo ri )
		{
			return ri.IsComposite && !ri.UseGuid && !ri.HasTable && !ri.IsList && ri.OtherPoly;
		}

		void CreateTestSaveReload( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "TestSaveReload" );
			func.Attributes.Add( "Test" );
			bool forbidden = IsForbiddenCase( ri );
			func.AccessModifier = "public";

			if (forbidden)
			{
				func.Statements.Add( "bool thrown = false;" );
				func.Statements.Add( "try" );
				func.Statements.Add( "{" );
			}
			func.Statements.Add( "CreateObjects();" );
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );
			if (ri.IsList)
				func.Statements.Add( AssertEquals( "Count wrong", 1, "ownVar.RelField.Count" ) );
			else
				func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );
			if (forbidden)
			{
				func.Statements.Add( "}" );
				func.Statements.Add( "catch (NDOException)" );
				func.Statements.Add( "{" );
				func.Statements.Add( "thrown = true;" );
				func.Statements.Add( "}" );
				func.Statements.Add( AssertEquals( "NDOException should have been thrown", true, "thrown" ) );
			}
		}

		void CreateTestSaveReloadNull( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "TestSaveReloadNull" );
			func.Attributes.Add( "Test" );
			bool forbidden = IsForbiddenCase( ri );
			func.AccessModifier = "public";

			if (forbidden)
			{
				func.Statements.Add( "bool thrown = false;" );
				func.Statements.Add( "try" );
				func.Statements.Add( "{" );
			}
			func.Statements.Add( "CreateObjects();" );
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );

			if (ri.IsList)
				func.Statements.Add( AssertEquals( "Count wrong", 1, "ownVar.RelField.Count" ) );
			else
				func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );

			if (ri.IsList)
				func.Statements.Add( "ownVar.RelField = new List<" + this.test.OtherClass.Name + ">();" );
			else
				func.Statements.Add( "ownVar.RelField = null;" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOwn();" );

			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );

			if (ri.IsList)
				func.Statements.Add( AssertEquals( "Count wrong", 0, "ownVar.RelField.Count" ) );
			else
				func.Statements.Add( AssertNull( "There should be no object", "ownVar.RelField" ) );

			if (forbidden)
			{
				func.Statements.Add( "}" );
				func.Statements.Add( "catch (NDOException)" );
				func.Statements.Add( "{" );
				func.Statements.Add( "thrown = true;" );
				func.Statements.Add( "}" );
				func.Statements.Add( AssertEquals( "NDOException should have been thrown", true, "thrown" ) );
			}
		}

		void CreateTestSaveReloadRemove( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "TestSaveReloadRemove" );
			func.AccessModifier = "public";
			func.Attributes.Add( "Test" );
			if (!ri.IsList)
				return;

			bool forbidden = IsForbiddenCase( ri );
			if (forbidden)
			{
				func.Statements.Add( "bool thrown = false;" );
				func.Statements.Add( "try" );
				func.Statements.Add( "{" );
			}
			func.Statements.Add( "CreateObjects();" );
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );

			if (ri.IsList)
				func.Statements.Add( AssertEquals( "Count wrong", 1, "ownVar.RelField.Count" ) );
			else
				func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );

			func.Statements.Add( "ownVar.RemoveRelatedObject();" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOwn();" );

			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );

			func.Statements.Add( AssertEquals( "Count wrong", 0, "ownVar.RelField.Count" ) );
			if (forbidden)
			{
				func.Statements.Add( "}" );
				func.Statements.Add( "catch (NDOException)" );
				func.Statements.Add( "{" );
				func.Statements.Add( "thrown = true;" );
				func.Statements.Add( "}" );
				func.Statements.Add( AssertEquals( "NDOException should have been thrown", true, "thrown" ) );
			}
		}

		void CreateTestChangeKeyHolderLeft( RelInfo ri )
		{
			if (IsForbiddenCase( ri ))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.IsList)
				return;

			Function func = fixture.NewFunction( "void", "TestChangeKeyHolderLeft" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";

			func.Statements.Add( "CreateObjects();" );
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );
			func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );
			// touch the related object
			func.Statements.Add( "int x = ownVar.RelField.Dummy;" );
			// change our object
			func.Statements.Add( "ownVar.Dummy = 4711;" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );
			func.Statements.Add( AssertNotNull( "Wrong value", "ownVar.Dummy == 4711" ) );
			func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );

		}

		void CreateTestChangeKeyHolderRight( RelInfo ri )
		{
			if (IsForbiddenCase( ri ))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.ForeignIsList || !ri.IsBi)
				return;

			Function func = fixture.NewFunction( "void", "TestChangeKeyHolderRight" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";

			func.Statements.Add( "CreateObjects();" );
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add( "QueryOther();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "otherVar" ) );
			func.Statements.Add( AssertNotNull( "No related object", "otherVar.RelField" ) );
			// touch the related object
			func.Statements.Add( "int x = otherVar.RelField.Dummy;" );
			// change our object
			func.Statements.Add( "otherVar.Dummy = 4711;" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOther();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "otherVar" ) );
			func.Statements.Add( AssertNotNull( "Wrong value", "otherVar.Dummy == 4711" ) );
			func.Statements.Add( AssertNotNull( "No related object", "otherVar.RelField" ) );
		}

		void CreateTestChangeKeyHolderLeftNoTouch( RelInfo ri )
		{
			if (IsForbiddenCase( ri ))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.IsList)
				return;

			Function func = fixture.NewFunction( "void", "TestChangeKeyHolderLeftNoTouch" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";

			func.Statements.Add( "CreateObjects();" );
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );
			func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );
			// change our object
			func.Statements.Add( "ownVar.Dummy = 4711;" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOwn();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "ownVar" ) );
			func.Statements.Add( AssertNotNull( "Wrong value", "ownVar.Dummy == 4711" ) );
			func.Statements.Add( AssertNotNull( "No related object", "ownVar.RelField" ) );

		}

		void CreateTestChangeKeyHolderRightNoTouch( RelInfo ri )
		{
			if (IsForbiddenCase( ri ))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.ForeignIsList || !ri.IsBi)
				return;

			Function func = fixture.NewFunction( "void", "TestChangeKeyHolderRightNoTouch" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";

			func.Statements.Add( "CreateObjects();" );
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add( "QueryOther();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "otherVar" ) );
			func.Statements.Add( AssertNotNull( "No related object", "otherVar.RelField" ) );
			// change our object
			func.Statements.Add( "otherVar.Dummy = 4711;" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "QueryOther();" );
			func.Statements.Add( AssertNotNull( "No Query Result", "otherVar" ) );
			func.Statements.Add( AssertNotNull( "Wrong value", "otherVar.Dummy == 4711" ) );
			func.Statements.Add( AssertNotNull( "No related object", "otherVar.RelField" ) );
		}

		void CreateTestUpdateOrder( RelInfo ri )
		{
			if (ri.HasTable || ri.UseGuid || IsForbiddenCase( ri ))
				return;
			if (!ri.IsList && ri.IsBi && !ri.ForeignIsList)
				return;

			Function func = fixture.NewFunction( "void", "TestUpdateOrder" );
			func.Attributes.Add( "Test" );
			func.AccessModifier = "public";

			func.Statements.Add( "NDO.Mapping.NDOMapping mapping = pm.NDOMapping;" );
			func.Statements.Add( "MethodInfo mi = mapping.GetType().GetMethod(\"GetUpdateOrder\");" );
			string br = null;
			if (!ri.IsList)
				br = ">";
			else
				br = "<";
			if ((!ri.OwnPoly && !ri.OtherPoly) || !ri.IsAbstract)
			{
				func.Statements.Add( Assert( "Wrong order #1", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherClass.Name + ")}))" ) );
			}
			if (ri.OwnPoly && !ri.OtherPoly)
			{
				func.Statements.Add( Assert( "Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnDerivedClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherClass.Name + ")}))" ) );
			}
			if (!ri.OwnPoly && ri.OtherPoly)
			{
				func.Statements.Add( Assert( "Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherDerivedClass.Name + ")}))" ) );
			}
			if (ri.OwnPoly && ri.OtherPoly)
			{
				func.Statements.Add( Assert( "Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnDerivedClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherDerivedClass.Name + ")}))" ) );
			}
			func.Statements.Add( "Debug.WriteLine(\"" + test.OwnClass.Name + "\");" );

		}

		void GenerateTearDown( RelInfo ri )
		{
			Function func = fixture.TearDown;
			func.Statements.Add( "try" );
			func.Statements.Add( "{" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "var l = pm.Objects<" + test.OwnClass.Name + ">().ResultTable;" );
			func.Statements.Add( "pm.Delete(l);" );
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
			if (!ri.IsComposite)
			{
				func.Statements.Add( "var m = pm.Objects<" + test.OtherClass.Name + ">().ResultTable;" );
				func.Statements.Add( "pm.Delete(m);" );
				func.Statements.Add( "pm.Save();" );
				func.Statements.Add( "pm.UnloadCache();" );
			}
			func.Statements.Add( "decimal count;" );
			func.Statements.Add( "count = (decimal) new " + NDOQuery( test.OwnClass.Name ) + ".ExecuteAggregate(\"dummy\", AggregateType.Count);" );
			func.Statements.Add( "Assert.That(0, Is.EqualTo(count), \"Count wrong #1\");" );
			func.Statements.Add( "count = (decimal) new " + NDOQuery( test.OtherClass.Name ) + ".ExecuteAggregate(\"dummy\", AggregateType.Count);" );
			func.Statements.Add( "Assert.That(0, Is.EqualTo(count), \"Count wrong #2\");" );
			func.Statements.Add( "}" );
			func.Statements.Add( "catch (Exception)" );
			func.Statements.Add( "{" );
			func.Statements.Add( "var handler = pm.GetSqlPassThroughHandler( pm.NDOMapping.Connections.First() );" );
			
			func.Statements.Add( "handler.Execute($\"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).TableName}\");" );
			func.Statements.Add( "handler.Execute($\"DELETE FROM {pm.NDOMapping.FindClass( otherVar.GetType() ).TableName}\");" );
			if (ri.HasTable)
			{
				func.Statements.Add( "handler.Execute( $\"DELETE FROM {pm.NDOMapping.FindClass( ownVar.GetType() ).Relations.First().MappingTable.TableName}\" );" );
			}
			func.Statements.Add( "}" );

		}

		void GenerateTestGroup( RelInfo ri )
		{
			fixture = new TestFixture( this.nameSpace, "Test" + ri.ToString() );
			test = new Test( ri, "RelationTestClasses" );
			Class ownClass = null;
			Class otherClass = null;
			if (ri.OwnPoly)
			{
				ownClass = test.OwnDerivedClass;
			}
			else
			{
				ownClass = test.OwnClass;
			}
			if (ri.OtherPoly)
			{
				otherClass = test.OtherDerivedClass;
			}
			else
			{
				otherClass = test.OtherClass;
			}


			fixture.Statements.Add( test.OwnClass.Name + " ownVar;" );
			fixture.Statements.Add( test.OtherClass.Name + " otherVar;" );  // always use the base class type
			fixture.Statements.Add( "PersistenceManager pm;" );

			fixture.SetUp.Statements.Add( "pm = PmFactory.NewPersistenceManager();" );
			fixture.SetUp.Statements.Add( "ownVar = new " + ownClass.Name + "();" );
			fixture.SetUp.Statements.Add( "otherVar = new " + otherClass.Name + "();" );

			GenerateTearDown( ri );

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
			func.Statements.Add( "}" );
			func.Statements.Add( "else" );
			func.Statements.Add( "{" );
			func.Statements.Add( "pm.UnloadCache();" );
			func.Statements.Add( "}" );
			func.Statements.Add( "return pm;" );
			sw.WriteLine( cl.ToString() );
		}

		void GenerateCreateObjects( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "CreateObjects" );
			func.Statements.Add( "pm.MakePersistent(ownVar);" );
			string secondMakePersistent = "pm.MakePersistent(otherVar);";
			string assignRelation = "ownVar.AssignRelation(otherVar);";
			if (ri.IsComposite)
			{
				if (!ri.IsList && (ri.OtherPoly || ri.OwnPoly) && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add( "pm.Save();" );
				func.Statements.Add( assignRelation );
			}
			else
			{
				if (!ri.IsList && ri.OtherPoly && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add( "pm.Save();" );
				func.Statements.Add( secondMakePersistent );
				if (!ri.IsList && ri.OtherPoly && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add( "pm.Save();" );
				if (ri.IsBi && !ri.ForeignIsList && ri.OwnPoly && !ri.UseGuid)
					func.Statements.Add( "pm.Save();" );
				func.Statements.Add( assignRelation );
			}
			func.Statements.Add( "pm.Save();" );
			func.Statements.Add( "pm.UnloadCache();" );
		}

		string NDOQuery( string className, string condition = null )
		{
			StringBuilder sb = new StringBuilder( "NDOQuery<" );
			sb.Append( className );
			sb.Append( ">(pm" );
			if (condition != null)
			{
				sb.Append( "," );
				sb.Append( condition );
			}
			sb.Append( ")" );
			return sb.ToString();
		}

		void GenerateQueryOwn( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "QueryOwn" );
			func.Statements.Add( "var q = new " + NDOQuery(test.OwnClass.Name) + ';' );
			func.Statements.Add( "ownVar = q.ExecuteSingle();" );
		}

		void GenerateQueryOther( RelInfo ri )
		{
			Function func = fixture.NewFunction( "void", "QueryOther" );
			func.Statements.Add( "var q = new " + NDOQuery( test.OtherClass.Name ) + ';' );
			func.Statements.Add( "otherVar = q.ExecuteSingle();" );
		}


		public void Generate()
		{
			sw = new StreamWriter( fileName );
			sw.WriteLine( @"//
// Copyright (c) 2002-2016 Mirko Matytschak 
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
			sw.WriteLine( "using System.Reflection;" );
			sw.WriteLine( "using System.Diagnostics;" );
			sw.WriteLine( "using System.Collections;" );
			sw.WriteLine( "using System.Collections.Generic;" );
			sw.WriteLine( "using NDO;" );
			sw.WriteLine( "using NDO.Mapping;" );
			sw.WriteLine( "using NDO.Query;" );
			sw.WriteLine( "using NUnit.Framework;" );
			sw.WriteLine( "using RelationTestClasses;" );
			sw.WriteLine( "using NdoUnitTests;\n" );
			sw.WriteLine( "namespace " + nameSpace );
			sw.WriteLine( "{\n" );
			GeneratePmFactory();
			foreach (RelInfo ri in relInfos)
				GenerateTestGroup( ri );
			sw.WriteLine( "\n}" );
			sw.Close();
		}

	}
}
