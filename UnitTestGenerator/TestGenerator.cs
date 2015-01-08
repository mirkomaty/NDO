//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Collections;
using System.IO;
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Summary for TestGenerator.
	/// </summary>
	public class TestGenerator
	{
		ArrayList relInfos;
		string fileName;
		StreamWriter sw;
		TestFixture fixture;
		Test test;
		const string nameSpace = "RelationUnitTests";

		public TestGenerator(ArrayList relInfos)
		{
			this.relInfos = relInfos;
			fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\UnitTests\UnitTests.cs");			
		}


		void CreateTests(RelInfo ri)
		{
			CreateTestSaveReload(ri);
			CreateTestSaveReloadNull(ri);
			CreateTestSaveReloadRemove(ri);
			CreateTestChangeKeyHolderLeft(ri);
			CreateTestChangeKeyHolderRight(ri);
			CreateTestChangeKeyHolderLeftNoTouch(ri);
			CreateTestChangeKeyHolderRightNoTouch(ri);
			CreateTestUpdateOrder(ri);
			CreateTestRelationHash(ri);
			GenerateCreateObjects(ri);
			GenerateQueryOwn(ri);
			GenerateQueryOther(ri);
		}


		string AssertEquals(string text, object o2, object o3)
		{
			return "Assert.AreEqual(" + o2 + ", " + o3 + ", \"" + text + "\");";
		}
		string AssertNotNull(string text, object o)
		{
			return "Assert.NotNull(" + o + ", \"" + text + "\");";
		}
		string AssertNull(string text, object o)
		{
			return "Assert.Null(" + o + ", \"" + text + "\");";
		}

		string Assert(string text, object o)
		{
			return "Assert.That(" + o + ", \"" + text + "\");";
		}

		string QualifiedClassName(string className)
		{
			return nameSpace + "." + className;
		}

		void CreateTestRelationHash(RelInfo ri)
		{
			if (!ri.IsBi)
				return;
			Function func = fixture.NewFunction("void", "TestRelationHash");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";
			func.Statements.Add("Class clbaseLeft = pm.NDOMapping.FindClass(typeof(" + test.OwnClass.Name + "));");
			func.Statements.Add("Relation relbaseLeft = clbaseLeft.FindRelation(\"relField\");");
			func.Statements.Add("Class clbaseRight = pm.NDOMapping.FindClass(typeof(" + test.OtherClass.Name + "));");
			func.Statements.Add("Relation relbaseRight = clbaseRight.FindRelation(\"relField\");");
			func.Statements.Add( Assert( "Relation should be equal #1", "relbaseRight.Equals(relbaseLeft)" ) );
			func.Statements.Add( Assert( "Relation should be equal #2", "relbaseLeft.Equals(relbaseRight)" ) );
			if (ri.OwnPoly)
			{
				func.Statements.Add("Class clderLeft = pm.NDOMapping.FindClass(typeof("+ test.OwnDerivedClass.Name + "));");
				func.Statements.Add("Relation relderLeft = clderLeft.FindRelation(\"relField\");");
				func.Statements.Add( Assert( "Relation should be equal #3", "relderLeft.Equals(relbaseRight)" ) );
				func.Statements.Add( Assert( "Relation should be equal #4", "relbaseRight.Equals(relderLeft)" ) );
			}
			if (ri.OtherPoly)
			{
				func.Statements.Add("Class clderRight = pm.NDOMapping.FindClass(typeof(" + test.OtherDerivedClass.Name + "));");
				func.Statements.Add("Relation relderRight = clderRight.FindRelation(\"relField\");");
				func.Statements.Add( Assert( "Relation should be equal #5", "relbaseLeft.Equals(relderRight)" ) );
				func.Statements.Add( Assert( "Relation should be equal #6", "relderRight.Equals(relbaseLeft)" ) );
				if (ri.OwnPoly)
				{
					func.Statements.Add( Assert( "Relation should be equal #7", "relderLeft.Equals(relderRight)" ) );
					func.Statements.Add( Assert( "Relation should be equal #8", "relderRight.Equals(relderLeft)" ) );
				}
			}
		}

		bool IsForbiddenCase(RelInfo ri)
		{
			return ri.IsComposite && !ri.UseGuid && !ri.HasTable && !ri.IsList && ri.OtherPoly;
		}

		void CreateTestSaveReload(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "TestSaveReload");
			func.Attributes.Add("Test");
			if (IsForbiddenCase(ri))
				func.Attributes.Add("ExpectedException(typeof(NDOException))");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));
			if (ri.IsList)
				func.Statements.Add(AssertEquals("Count wrong", 1, "ownVar.RelField.Count"));
			else
				func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));
		}

		void CreateTestSaveReloadNull(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "TestSaveReloadNull");
			func.Attributes.Add("Test");
			if (IsForbiddenCase(ri))
				func.Attributes.Add("ExpectedException(typeof(NDOException))");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));

			if (ri.IsList)
				func.Statements.Add(AssertEquals("Count wrong", 1, "ownVar.RelField.Count"));
			else
				func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));

			if (ri.IsList)
				func.Statements.Add("ownVar.RelField = new ArrayList();");
			else
				func.Statements.Add("ownVar.RelField = null;");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOwn();");

			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));

			if (ri.IsList)
				func.Statements.Add(AssertEquals("Count wrong", 0, "ownVar.RelField.Count"));
			else
				func.Statements.Add(AssertNull("There should be no object", "ownVar.RelField"));
		}

		void CreateTestSaveReloadRemove(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "TestSaveReloadRemove");
			func.AccessModifier = "public";
			func.Attributes.Add("Test");
			if (!ri.IsList)
				return;
			if (IsForbiddenCase(ri))
				func.Attributes.Add("ExpectedException(typeof(NDOException))");

			func.Statements.Add("CreateObjects();");
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));

			if (ri.IsList)
				func.Statements.Add(AssertEquals("Count wrong", 1, "ownVar.RelField.Count"));
			else
				func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));

			func.Statements.Add("ownVar.RemoveRelatedObject();");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOwn();");

			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));

			func.Statements.Add(AssertEquals("Count wrong", 0, "ownVar.RelField.Count"));
		}

		void CreateTestChangeKeyHolderLeft(RelInfo ri)
		{
			if (IsForbiddenCase(ri))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.IsList)
				return;

			Function func = fixture.NewFunction("void", "TestChangeKeyHolderLeft");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));
			func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));
			// touch the related object
			func.Statements.Add("int x = ownVar.RelField.Dummy;");
			// change our object
			func.Statements.Add("ownVar.Dummy = 4711;");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));
			func.Statements.Add(AssertNotNull("Wrong value", "ownVar.Dummy == 4711"));
			func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));

		}

		void CreateTestChangeKeyHolderRight(RelInfo ri)
		{
			if (IsForbiddenCase(ri))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.ForeignIsList || !ri.IsBi)
				return;

			Function func = fixture.NewFunction("void", "TestChangeKeyHolderRight");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add("QueryOther();");
			func.Statements.Add(AssertNotNull("No Query Result", "otherVar"));
			func.Statements.Add(AssertNotNull("No related object", "otherVar.RelField"));
			// touch the related object
			func.Statements.Add("int x = otherVar.RelField.Dummy;");
			// change our object
			func.Statements.Add("otherVar.Dummy = 4711;");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOther();");
			func.Statements.Add(AssertNotNull("No Query Result", "otherVar"));
			func.Statements.Add(AssertNotNull("Wrong value", "otherVar.Dummy == 4711"));
			func.Statements.Add(AssertNotNull("No related object", "otherVar.RelField"));
		}

		void CreateTestChangeKeyHolderLeftNoTouch(RelInfo ri)
		{
			if (IsForbiddenCase(ri))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.IsList)
				return;

			Function func = fixture.NewFunction("void", "TestChangeKeyHolderLeftNoTouch");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));
			func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));
			// change our object
			func.Statements.Add("ownVar.Dummy = 4711;");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOwn();");
			func.Statements.Add(AssertNotNull("No Query Result", "ownVar"));
			func.Statements.Add(AssertNotNull("Wrong value", "ownVar.Dummy == 4711"));
			func.Statements.Add(AssertNotNull("No related object", "ownVar.RelField"));

		}

		void CreateTestChangeKeyHolderRightNoTouch(RelInfo ri)
		{
			if (IsForbiddenCase(ri))
				return; // These would throw exceptions

			// Check Keyholders only
			if (ri.ForeignIsList || !ri.IsBi)
				return;

			Function func = fixture.NewFunction("void", "TestChangeKeyHolderRightNoTouch");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";

			func.Statements.Add("CreateObjects();");
			// 1:1 or n:1 - we check only the left side
			func.Statements.Add("QueryOther();");
			func.Statements.Add(AssertNotNull("No Query Result", "otherVar"));
			func.Statements.Add(AssertNotNull("No related object", "otherVar.RelField"));
			// change our object
			func.Statements.Add("otherVar.Dummy = 4711;");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("QueryOther();");
			func.Statements.Add(AssertNotNull("No Query Result", "otherVar"));
			func.Statements.Add(AssertNotNull("Wrong value", "otherVar.Dummy == 4711"));
			func.Statements.Add(AssertNotNull("No related object", "otherVar.RelField"));
		}

		void CreateTestUpdateOrder(RelInfo ri)
		{
			if (ri.HasTable || ri.UseGuid || IsForbiddenCase(ri))
				return;
			if (!ri.IsList && ri.IsBi && !ri.ForeignIsList)
				return;

			Function func = fixture.NewFunction("void", "TestUpdateOrder");
			func.Attributes.Add("Test");
			func.AccessModifier = "public";

			func.Statements.Add("NDO.Mapping.NDOMapping mapping = pm.NDOMapping;");
			func.Statements.Add("MethodInfo mi = mapping.GetType().GetMethod(\"GetUpdateOrder\");");
			string br = null;
			if (!ri.IsList)
				br = ">";
			else
				br = "<";
			if ((!ri.OwnPoly && !ri.OtherPoly) || !ri.IsAbstract)
			{
				func.Statements.Add(Assert("Wrong order #1", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherClass.Name + ")}))"));
			}
			if (ri.OwnPoly && !ri.OtherPoly)
			{
				func.Statements.Add(Assert("Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnDerivedClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherClass.Name + ")}))"));
			}
			if (!ri.OwnPoly && ri.OtherPoly)
			{
				func.Statements.Add(Assert("Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherDerivedClass.Name + ")}))"));
			}
			if (ri.OwnPoly && ri.OtherPoly)
			{
				func.Statements.Add(Assert("Wrong order #2", @"((int)mi.Invoke(mapping, new object[]{typeof(" + test.OwnDerivedClass.Name + @")})) 
				" + br + " ((int)mi.Invoke(mapping, new object[]{typeof(" + test.OtherDerivedClass.Name + ")}))"));
			}
			func.Statements.Add("Debug.WriteLine(\"" + test.OwnClass.Name + "\");");

		}

		void GenerateTearDown(RelInfo ri)
		{
			Function func = fixture.TearDown;
			func.Statements.Add("pm.UnloadCache();");
			func.Statements.Add("IList l;");
			func.Statements.Add("l = pm.NewQuery(typeof(" + test.OwnClass.Name + ")).Execute();");
			func.Statements.Add("pm.Delete(l);");
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
			if (!ri.IsComposite)
			{
				func.Statements.Add("l = pm.NewQuery(typeof(" + test.OtherClass.Name + ")).Execute();");
				func.Statements.Add("pm.Delete(l);");
				func.Statements.Add("pm.Save();");
				func.Statements.Add("pm.UnloadCache();");
			}
			func.Statements.Add("decimal count;");
			func.Statements.Add("count = (decimal) pm.NewQuery(typeof(" + test.OwnClass.Name + ")).ExecuteAggregate(\"dummy\", Query.AggregateType.Count);");
			func.Statements.Add("Assert.AreEqual(0, count, \"Count wrong #1\");");
			func.Statements.Add("count = (decimal) pm.NewQuery(typeof(" + test.OtherClass.Name + ")).ExecuteAggregate(\"dummy\", Query.AggregateType.Count);");
			func.Statements.Add("Assert.AreEqual(0, count, \"Count wrong #2\");");
		}

		void GenerateTestGroup(RelInfo ri)
		{
			fixture = new TestFixture("Test" + ri.ToString());
			test = new Test(ri);
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


			fixture.Statements.Add(test.OwnClass.Name + " ownVar;");
			fixture.Statements.Add(test.OtherClass.Name + " otherVar;");  // always use the base class type
			fixture.Statements.Add("PersistenceManager pm;");

			fixture.SetUp.Statements.Add("pm = PmFactory.NewPersistenceManager();");
			fixture.SetUp.Statements.Add("ownVar = new " + ownClass.Name + "();");
			fixture.SetUp.Statements.Add("otherVar = new " + otherClass.Name + "();");

			GenerateTearDown(ri);

			CreateTests(ri);

			sw.WriteLine(fixture.ToString());
		}

		void GeneratePmFactory()
		{
			Class cl = new Class("PmFactory");
			string path = AppDomain.CurrentDomain.BaseDirectory;
			path = Path.Combine(path, @"..\..\UnitTests\bin\Debug\NDOMapping.xml");
			path = Path.GetFullPath(path);
			cl.Statements.Add("static PersistenceManager pm;");
			Function func = cl.NewFunction("PersistenceManager", "NewPersistenceManager");
			func.IsStatic = true;
			func.AccessModifier = "public";

			func.Statements.Add("if (pm == null)");
			func.Statements.Add("{");
			func.Statements.Add("\tpm = new PersistenceManager(@\"" + path + "\");");
			path = Path.GetFullPath(Path.Combine(path, @"..\..\.."));
			func.Statements.Add("\tpm.LogPath = @\"" + Path.GetDirectoryName(path) + "\";");
			func.Statements.Add("}");
			func.Statements.Add("else");
			func.Statements.Add("{");
			func.Statements.Add("\tpm.UnloadCache();");
			func.Statements.Add("}");
			func.Statements.Add("return pm;");
			sw.WriteLine(cl.ToString());
		}

		void GenerateCreateObjects(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "CreateObjects");
			func.Statements.Add("pm.MakePersistent(ownVar);");
			string secondMakePersistent = "pm.MakePersistent(otherVar);";
			string assignRelation = "ownVar.AssignRelation(otherVar);";
			if (ri.IsComposite)
			{
				if (!ri.IsList && (ri.OtherPoly || ri.OwnPoly) && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add("pm.Save();");
				func.Statements.Add(assignRelation);
			}
			else
			{
				if (!ri.IsList && ri.OtherPoly && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add("pm.Save();");
				func.Statements.Add(secondMakePersistent);
				if (!ri.IsList && ri.OtherPoly && !ri.HasTable && !ri.UseGuid)
					func.Statements.Add("pm.Save();");
				if (ri.IsBi && !ri.ForeignIsList && ri.OwnPoly && !ri.UseGuid)
					func.Statements.Add("pm.Save();");
				func.Statements.Add(assignRelation);
			}
			func.Statements.Add("pm.Save();");
			func.Statements.Add("pm.UnloadCache();");
	}

		void GenerateQueryOwn(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "QueryOwn");
			func.Statements.Add("Query q = pm.NewQuery(typeof(" + test.OwnClass.Name + "));");
			func.Statements.Add("ownVar = (" + test.OwnClass.Name + ") q.ExecuteSingle();");
		}

		void GenerateQueryOther(RelInfo ri)
		{
			Function func = fixture.NewFunction("void", "QueryOther");
			func.Statements.Add("Query q = pm.NewQuery(typeof(" + test.OtherClass.Name + "));");
			func.Statements.Add("otherVar = (" + test.OtherClass.Name + ") q.ExecuteSingle();");
		}


		public void Generate()
		{
			sw = new StreamWriter(fileName);
			sw.WriteLine("using System;");
			sw.WriteLine("using System.Reflection;");
			sw.WriteLine("using System.Diagnostics;");
			sw.WriteLine("using System.Collections;");
			sw.WriteLine("using NDO;");
			sw.WriteLine("using NDO.Mapping;");
			sw.WriteLine("using NUnit.Framework;");
			sw.WriteLine("using RelationTestClasses;\n");
			sw.WriteLine("namespace " + nameSpace);
			sw.WriteLine("{\n");
			GeneratePmFactory();
			foreach(RelInfo ri in relInfos)
				GenerateTestGroup(ri);
			sw.WriteLine("\n}");
			sw.Close();
		}

	}
}
