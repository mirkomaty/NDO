//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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

namespace TestGenerator
{
	/// <summary>
	/// Zusammenfassung f�r Test.
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

                if (ri.OwnIsGeneric)
                {
                    ownClass.GenericParameters.Add("T");
                    ownClass.AddVarAndProperty("T", "genericField");
                }

				if (ri.UseGuid)
					ownClass.Attributes.Add("NDOOidType(typeof(Guid))");

				if (ri.OwnPoly)
				{
					string ownDerivedName = ri.ToString() + "LeftDerived";
					ownDerivedClass = new PersistentClass(false, ownDerivedName, ownClass);

                    if (ri.OwnIsGeneric)
                        ownDerivedClass.GenericParameters.Add("T");

					if (ri.UseGuid)
						ownDerivedClass.Attributes.Add("NDOOidType(typeof(Guid))");
				}

			
				// Right class
				otherClass = new PersistentClass(ri.OtherPoly && ri.IsAbstract, otherClassName, null);
				if (ri.UseGuid)
					otherClass.Attributes.Add("NDOOidType(typeof(Guid))");
                if (ri.OtherIsGeneric)
                    otherClass.GenericParameters.Add("T");

				if (ri.OtherPoly)
				{
					string otherDerivedName = ri.ToString() + "RightDerived";
					otherDerivedClass = new PersistentClass(false, otherDerivedName, otherClass);
					if (ri.UseGuid)
						otherDerivedClass.Attributes.Add("NDOOidType(typeof(Guid))");
                    if (ri.OtherIsGeneric)
                        otherDerivedClass.GenericParameters.Add("T");
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
