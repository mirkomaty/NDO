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
using System.Text;
using System.IO;
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Summary for ClassGenerator.
	/// </summary>
	public class ClassGenerator
	{
		ArrayList relInfos;
		string fileName;
		StreamWriter sw;
		int count;

		public ClassGenerator(ArrayList relInfos)
		{
			this.relInfos = relInfos;
			fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\PersistentClasses\PersistentClasses.cs");
		}

		void AddRelationAccessor(Class ownBaseClass, Class otherBaseClass, RelInfo ri)
		{
			Function func = ownBaseClass.NewFunction("void", "AssignRelation", new string[]{otherBaseClass.Name}, new string[]{"relObj"});
			func.AccessModifier = "public";
			if (ri.IsList)
				func.Statements.Add("relField.Add(relObj);");
			else
				func.Statements.Add("relField = relObj;");
		}

		void AddRelationRemover(Class ownBaseClass, RelInfo ri)
		{
			if (!ri.IsList)
				return;
			Function func = ownBaseClass.NewFunction("void", "RemoveRelatedObject");
			func.AccessModifier = "public";
			func.Statements.Add("relField.RemoveAt(0);");
		}


		void GenerateClassGroup(RelInfo ri)
		{
			Test test = new Test(ri);

			PersistentClass ownBaseClass = test.OwnClass;
			PersistentClass otherBaseClass = test.OtherClass;
			ownBaseClass.AddVarAndProperty("int", "dummy");
			PersistentClass ownDerivedClass;
			ownBaseClass.NewRelation(ri, otherBaseClass.Name);
			AddRelationAccessor(ownBaseClass, otherBaseClass, ri);
			AddRelationRemover(ownBaseClass, ri);

			sw.WriteLine(ownBaseClass.ToString());
			if (ri.OwnPoly)
			{
				ownDerivedClass = test.OwnDerivedClass;
				sw.WriteLine(ownDerivedClass.ToString());
			}

			
			// Right class
			otherBaseClass.AddVarAndProperty("int", "dummy");
			if (ri.IsBi)
				otherBaseClass.NewForeignRelation(ri, ownBaseClass.Name);
			sw.WriteLine(otherBaseClass.ToString());
			if (ri.OtherPoly)
			{
				Class otherDerivedClass = test.OtherDerivedClass;
				sw.WriteLine(otherDerivedClass.ToString());
			}
			count += 2;
			if (ri.OwnPoly)
				count++;
			if (ri.OtherPoly)
				count++;
		}


		public void Generate()
		{
			sw = new StreamWriter(fileName, false, Encoding.UTF8);
			sw.WriteLine("using System;");
			sw.WriteLine("using System.Collections;");
			sw.WriteLine("using NDO;\n");
			sw.WriteLine("namespace RelationTestClasses");
			sw.WriteLine("{\n");
			foreach(RelInfo ri in relInfos)
				GenerateClassGroup(ri);
			Console.WriteLine("Anzahl Klassen: " + count);
			sw.WriteLine("}");
			sw.Close();
		}
	}
}
