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
using System.Runtime.Serialization;
using System.Collections;

namespace NDO
{
	
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ChangeSetContainer : ObjectContainerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeSetContainer"/> class.
		/// </summary>
		/// <remarks>
		/// Only available in the NDO Enterprise Edition.
		/// You probably won't need any constructor for ChangeSetContainer. Normally the
		/// PersistenceManager.GetChangeSet function is used to construct ChangeSetContainers.
		/// </remarks>
		public ChangeSetContainer()
		{
			for (int i = 0; i < 4; i++)
				RootObjects.Add(null);
		}

		/// <summary>
		/// Gets or sets the added objects.
		/// </summary>
		/// <value>The added objects.</value>
		public IList AddedObjects
		{
			get { return (IList) this.RootObjects[0]; }
			set { this.RootObjects[0] = value; }
		}

		/// <summary>
		/// Gets or sets the deleted objects.
		/// </summary>
		/// <value>The deleted objects.</value>
		public IList DeletedObjects
		{
			get { return (IList) this.RootObjects[1]; }
			set { this.RootObjects[1] = value; }
		}

		/// <summary>
		/// Gets or sets the changed objects.
		/// </summary>
		/// <value>The changed objects.</value>
		public IList ChangedObjects
		{
			get { return (IList) this.RootObjects[2]; }
			set { this.RootObjects[2] = value; }
		}

		/// <summary>
		/// Gets or sets the relation changes.
		/// </summary>
		/// <value>A list of <see cref="RelationChangeRecord"/> objects.</value>
		public IList RelationChanges
		{
			get { return (IList) this.RootObjects[3]; }
			set { this.RootObjects[3] = value; }
		}

		/// <summary>
		/// This constructor is used by the Binary/Soap serializers.
		/// </summary>
		/// <param name="info">A SerializationInfo object.</param>
		/// <param name="context">A StreamingContext object.</param>
		public ChangeSetContainer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}


	}
}
