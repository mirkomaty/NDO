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

namespace ForeignObjectManipulationClasses
{
	/// <summary>
	/// Summary for ChildList
	/// </summary>
	[NDOPersistent]
	public class ChildList
	{
		public ChildList()
		{
		}
	}

	[NDOPersistent]
	public class ChildElement
	{
		public ChildElement()
		{
		}
	}


	public class ChildEmbedded
	{
		DateTime childField = DateTime.Now;
		public ChildEmbedded()
		{
		}
	}

	[NDOPersistent]
	public class Parent
	{
		string name;

		System.Drawing.Point position = new System.Drawing.Point(0,0);

		public System.Drawing.Point Position
		{
			get { return position; }
			set { position = value; }
		}

		ChildEmbedded childEmbedded = new ChildEmbedded();

		[NDORelation(typeof(ChildList))]
		IList listChilds = new ArrayList();

		[NDORelation]
		ChildElement elementChild;

		public void ManipulateString(Parent p, string name)
		{
			p.name = name;
		}
		public void ManipulatePoint(Parent p, System.Drawing.Point pos)
		{
			p.position = pos;
		}
		public void ManipulatePoint(Parent p)
		{
			p.position = position;
		}
		public void ManipulateOwnPoint(Parent p)
		{
			position = p.position;
		}

		public void ManipulateEmbedded(Parent p, ChildEmbedded ce)
		{
			p.childEmbedded = ce;
		}
		public void ManipulateListElement(Parent p, ChildList cl)
		{
			p.listChilds.Add(cl);
		}
		public void ManipulateList(Parent p, IList l)
		{
			p.listChilds = l;
		}
		public void ManipulateElement(Parent p, ChildElement ec)
		{
			p.elementChild = ec;
		}
/*
		public void ManipulateValueTypeMember(Parent p)
		{
			p.position.X = 5;
		}

		public void ManipulateOwnValueTypeMember(Parent p)
		{
			position.X = p.position.X + 5;
		}
*/

		public Parent()
		{
		}

		#region Accessors
		public ChildEmbedded ChildEmbedded
		{
			get { return childEmbedded; }
			set { childEmbedded = value; }
		}

		public IList ListChilds
		{
			get { return ArrayList.ReadOnly(listChilds); }
			set { listChilds = value; }
		}
		public void AddChildList(ChildList c)
		{
			listChilds.Add(c);
		}
		public void RemoveChildList(ChildList c)
		{
			if (listChilds.Contains(c))
				listChilds.Remove(c);
		}

		public ChildElement ElementChild
		{
			get { return elementChild; }
			set { elementChild = value; }
		}

		#endregion
	}
}
