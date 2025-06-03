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
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Summary for PersistentClass.
	/// </summary>
	public class TestFixture : Class
	{
		Function setUp;
		public Function SetUp
		{
			get { return setUp; }
		}
		Function tearDown;
		public Function TearDown
		{
			get { return tearDown; }
		}

		public TestFixture(string nameSpace, string name) : base (false, nameSpace, name, null)
		{			
			setUp = new Function("void", "Setup");
			setUp.Attributes.Add("SetUp");
			setUp.AccessModifier = "public";
			this.Functions.Add(setUp);

			tearDown = new Function("void", "TearDown");
			tearDown.Attributes.Add("TearDown");
			tearDown.AccessModifier = "public";
			this.Functions.Add(tearDown);
		}

		protected override string GenerateHeader(bool isAbstract, string name, Class baseClass)
		{
			var cls = new Class(this.nameSpace, "NDOTest");
			return "[TestFixture]\n" + base.GenerateHeader (isAbstract, name, cls);
		}

	}
}
