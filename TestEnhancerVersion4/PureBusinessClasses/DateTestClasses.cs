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
using NDO;

namespace DateTest
{
	[NDOPersistent]
	public class InnerDate
	{
		DateTime dt = DateTime.Now;
		DateTime dt2 = DateTime.Now;
		
		public DateTime Dt
		{
			get { return dt; }
			set { dt = value; }
		}

		public void SetInnerDate()
		{
			dt = new DateTime(2002, 10, 12, 13, 30, 31, 123);
		}

		public void SetInnerDate2()
		{
			dt = DateTime.Now;
		}

		public void SetInnerDate3(DateTime dt)
		{
			this.dt = dt;
		}

		public void SetInnerDate4()
		{
			dt2 = dt;
		}

		public InnerDate()
		{
		}
	}

	[NDOPersistent]
	public class DateTestClass
	{
		string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[NDORelation]
		InnerDate innerDate;

		public InnerDate InnerDate
		{
			get { return innerDate; }
			set { innerDate = value; }
		}

		public DateTestClass()
		{
		}

		public DateTestClass(bool MakeInnerDate)
		{
			innerDate = new InnerDate();
			innerDate.SetInnerDate();
		}
	}
}
