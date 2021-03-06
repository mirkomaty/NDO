﻿//
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
using NDO;

namespace NDOObjectIdTestClasses
{
	/// <summary>
	/// Summary for NDOoidAndHandler
	/// </summary>
	[NDOPersistent]
	public class NDOoidAndHandler : IPersistentObject
	{
		[NDOObjectId]
		Guid myId;
		public Guid MyId
		{
			get { return myId; }
			set { myId = value; }
		}

		string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public NDOoidAndHandler()
		{
		}

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}

	[NDOPersistent, NDOOidType(typeof(int))]
	public class ObjectOwner : IPersistentObject
	{
		[NDORelation(RelationInfo.Composite)]
		NDOoidAndHandler element;

		public NDOoidAndHandler Element
		{
			get { return element; }
			set { element = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}

	[NDOPersistent, NDOOidType(typeof(int))]
	public class HintOwner : IPersistentObject
	{
		[NDORelation(RelationInfo.Composite)]
		ClassWithHint element;

		public ClassWithHint Element
		{
			get { return element; }
			set { element = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}


	[NDOPersistent, NDOOidType(typeof(Guid))]
	public class ClassWithHint : IPersistentObject
	{
		string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
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
	public abstract class AbstractBaseGuid : IPersistentObject
	{
		[NDOObjectId]
		Guid guid;
		public Guid Guid
		{
			get { return guid; }
			set { guid = value; }
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
	public class DerivedGuid : AbstractBaseGuid
	{
		string myfield;
		public string Myfield
		{
			get { return myfield; }
			set { myfield = value; }
		}

	}
}