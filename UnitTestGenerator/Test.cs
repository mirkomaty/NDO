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

namespace TestGenerator
{
	/// <summary>
	/// Summary for Test.
	/// </summary>
	public class Test
	{
		RelInfo ri;
		public Test(RelInfo ri)
		{
			this.ri = ri;
		}

		PersistentClass ownClass = null;
        public PersistentClass OwnClass
		{
			get { Dissolve(); return ownClass; }
		}
        PersistentClass ownDerivedClass;
        public PersistentClass OwnDerivedClass
		{
			get { Dissolve(); return ownDerivedClass; }
		}
        PersistentClass otherClass;
        public PersistentClass OtherClass
		{
			get { Dissolve(); return otherClass; }
		}
        PersistentClass otherDerivedClass;
        public PersistentClass OtherDerivedClass
		{
			get { Dissolve(); return otherDerivedClass; }
		}

		public Test GetReverse()
		{
			Test test = new Test(this.ri);
			test.DissolveReverse();
			return test;
		}
		
		void Dissolve()
		{
			if (this.ownClass == null)
			{
				string ownClassName;
				string otherClassName;

				// Left class
				ownClassName = ri.ToString() + "Left";
				if (ri.OwnPoly)
					ownClassName += "Base";

				otherClassName = ri.ToString() + "Right";
				if (ri.OtherPoly)
					otherClassName += "Base";

				ownClass = new PersistentClass(ri.OwnPoly && ri.IsAbstract, ownClassName, null);
				if (ri.UseGuid)
					ownClass.Attributes.Add("NDOOidType(typeof(Guid))");

				if (ri.OwnPoly)
				{
					string ownDerivedName = ri.ToString() + "LeftDerived";
					ownDerivedClass = new PersistentClass(false, ownDerivedName, ownClass);


					if (ri.UseGuid)
						ownDerivedClass.Attributes.Add("NDOOidType(typeof(Guid))");
				}

			
				// Right class
				otherClass = new PersistentClass(ri.OtherPoly && ri.IsAbstract, otherClassName, null);
				if (ri.UseGuid)
					otherClass.Attributes.Add("NDOOidType(typeof(Guid))");
				if (ri.OtherPoly)
				{
					string otherDerivedName = ri.ToString() + "RightDerived";
					otherDerivedClass = new PersistentClass(false, otherDerivedName, otherClass);
					if (ri.UseGuid)
						otherDerivedClass.Attributes.Add("NDOOidType(typeof(Guid))");
				}
			}
		}

		void DissolveReverse()
		{
			string ownClassName;
			string otherClassName;

			// Left class
			ownClassName = ri.ToString() + "Left";
			if (ri.OwnPoly)
				ownClassName += "Base";

			otherClassName = ri.ToString() + "Right";
			if (ri.OtherPoly)
				otherClassName += "Base";

			otherClass = new PersistentClass(ri.OwnPoly && ri.IsAbstract, ownClassName, null);

			if (ri.OwnPoly)
			{
				string ownDerivedName = ri.ToString() + "LeftDerived";
				otherDerivedClass = new PersistentClass(false, ownDerivedName, ownClass);
			}

			
			// Right class
			ownClass = new PersistentClass(ri.OtherPoly && ri.IsAbstract, otherClassName, null);
			if (ri.OtherPoly)
			{
				string otherDerivedName = ri.ToString() + "RightDerived";
				ownDerivedClass = new PersistentClass(false, otherDerivedName, otherClass);
			}
		}


	}
}
