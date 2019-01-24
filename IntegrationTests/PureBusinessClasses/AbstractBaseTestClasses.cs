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
using System.Collections;
using System.Data;

namespace AbstractBaseTestClasses
{

	[NDOPersistent]
	public abstract class ABBase : IPersistentObject
	{
		string myString;
		public string MyString
		{
			get { return myString; }
			set { myString = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}

	[NDOPersistent]
	public class ABA : ABBase
	{
		public ABA()
		{
		}
	}

	[NDOPersistent]
	public class ABB : ABBase
	{
		[NDORelation(typeof(ABA))]
		IList myA = new ArrayList();
		public IList MyA
		{
			get { return ArrayList.ReadOnly(myA); }
			set { myA = value; }
		}
		public void AddA(ABA a)
		{
			myA.Add(a);
		}
		public void RemoveA(ABA a)
		{
			if (myA.Contains(a))
				myA.Remove(a);
		}
		
	}

	[NDOPersistent]
	public class AbstractRelLeft : IPersistentObject
	{
		string myString = "AbstractRelLeft";
		public string MyString
		{
			get { return myString; }
			set { myString = value; }
		}

		[NDORelation(typeof(ABBase))]
		IList theBases = new ArrayList();

		public IList TheBases
		{
			get { return ArrayList.ReadOnly(theBases); }
			set { theBases = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void AddABBase(ABBase a)
		{
			theBases.Add(a);
		}
		public void RemoveABBase(ABBase a)
		{
			if (theBases.Contains(a))
				theBases.Remove(a);
		}

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}
}

