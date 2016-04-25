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


#if ENT
using System;
using NDO;

namespace NDO
{
	/// <summary>
	/// NDO uses objects of this type to record, 
	/// if objects are added or removed to/from relation fields or containers.
	/// </summary>
	/// <remarks>
	/// Only available in the NDO Enterprise Edition.
	/// </remarks>
	[Serializable]
	public class RelationChangeRecord
	{
		/// <summary>
		/// Constructs a RelationChangeRecord object.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="child">Child object.</param>
		/// <param name="relationName">Field name of the relation.</param>
		/// <param name="isAdded">True, if the child object was added, false, if it was deleted.</param>
		public RelationChangeRecord(IPersistenceCapable parent, IPersistenceCapable child, string relationName, bool isAdded)
		{
			this.parent = parent;
			this.child = child;
			this.relationName = relationName;
			this.isAdded = isAdded;
		}

		IPersistenceCapable parent;

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public IPersistenceCapable Parent
		{
			get { return parent; }
			set { parent = value; }
		}
		IPersistenceCapable child;
		/// <summary>
		/// Gets or sets the child.
		/// </summary>
		/// <value>The child.</value>
		public IPersistenceCapable Child
		{
			get { return child; }
			set { child = value; }
		}
		string relationName;
		/// <summary>
		/// Gets or sets the field name of the relation.
		/// </summary>
		/// <value>The field name of the relation.</value>
		public string RelationName
		{
			get { return relationName; }
			set { relationName = value; }
		}

		bool isAdded;
		/// <summary>
		/// Gets or sets a value indicating whether the child object is added or removed.
		/// </summary>
		/// <value><c>true</c> if the child object is added; <c>false</c> if the child object is removed.</value>
		public bool IsAdded
		{
			get { return isAdded; }
			set { isAdded = value; }
		}
	}
}



#endif