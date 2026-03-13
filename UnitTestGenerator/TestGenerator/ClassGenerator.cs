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
using System.Text;
using System.IO;
using CodeGenerator;
using System.Collections.Generic;

namespace TestGenerator
{
	/// <summary>
	/// Summary for ClassGenerator.
	/// </summary>
	public class ClassGenerator
	{
		IEnumerable<RelInfo> relInfos;
		string fileName;
		StreamWriter sw;
		int count;
		readonly string nameSpace = "RelationTestClasses";

		public ClassGenerator( IEnumerable<RelInfo> relInfos )
		{
			this.relInfos = relInfos;
			fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\PersistentClasses\PersistentClasses.cs");
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
			Test test = new Test(ri, this.nameSpace);

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

" );
			sw.WriteLine("using System;");
			sw.WriteLine("using System.Collections.Generic;");
			sw.WriteLine("using NDO;\n");
			sw.WriteLine( "using NDO.Mapping.Attributes;\n" );
			sw.WriteLine( $"namespace  {this.nameSpace}" );
			sw.WriteLine("{\n");
			foreach(RelInfo ri in relInfos)
				GenerateClassGroup(ri);
			Console.WriteLine("Number of Classes: " + count);
			sw.WriteLine("}");
			sw.Close();
		}
	}
}
