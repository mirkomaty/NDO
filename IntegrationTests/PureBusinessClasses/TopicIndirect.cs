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

namespace Zirkulär
{

	[NDOPersistent]
	public class TopicIndirect : IPersistentObject
	{
		[NDORelation(typeof(Intermediate), RelationInfo.Composite)]
		IList intermediates = new ArrayList();

		[NDORelation(RelationInfo.Composite)]
		IntermediateSingle intermediateSingle;

		#region Accessors		
		public IList Intermediates
		{
			get { return ArrayList.ReadOnly(intermediates); }
			set { intermediates = value; }
		}
		public Intermediate NewIntermediate()
		{
			Intermediate i = new Intermediate();
			intermediates.Add(i);
			return i;
		}
		public void RemoveIntermediate(Intermediate i)
		{
			if (intermediates.Contains(i))
				intermediates.Remove(i);
		}

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}

		public IntermediateSingle IntermediateSingle
		{
			get { return intermediateSingle; }
			set { intermediateSingle = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		#endregion

		public TopicIndirect()
		{
		}
	}
	
}