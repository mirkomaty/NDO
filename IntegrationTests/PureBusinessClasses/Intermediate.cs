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
using NDO;

namespace Zirkulär
{
	[NDOPersistent]
	public class Intermediate : IPersistentObject
	{

		[NDORelation(typeof(TopicIndirect), RelationInfo.Composite, "Subtopics")]
		IList topics;

		public IList TopicIndirects
		{
			get { return ArrayList.ReadOnly(topics); }
			set { topics = value; }
		}

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public TopicIndirect NewTopicIndirect()
		{
			TopicIndirect t = new TopicIndirect();
			topics.Add(t);
			return t;
		}
		public void RemoveTopicIndirect(TopicIndirect t)
		{
			if (topics.Contains(t))
				topics.Remove(t);
		}

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}

		public Intermediate()
		{
		}
		
	}
}
