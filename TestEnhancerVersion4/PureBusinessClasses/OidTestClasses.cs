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
using System.Collections;
using NDO;

namespace NDOObjectIdTestClasses
{
	/// <summary>
	/// Summary for NDOoidAndHandler
	/// </summary>
	[NDOPersistent]
	public class NDOoidAndHandler
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
		public NDOoidAndHandler()
		{
		}
	}

	[NDOPersistent, NDOOidType(typeof(int))]
	public class ObjectOwner
	{
		[NDORelation(RelationInfo.Composite)]
		NDOoidAndHandler element;

		public NDOoidAndHandler Element
		{
			get { return element; }
			set { element = value; }
		}			
	}

	[NDOPersistent, NDOOidType(typeof(int))]
	public class HintOwner
	{
		[NDORelation(RelationInfo.Composite)]
		ClassWithHint element;

		public ClassWithHint Element
		{
			get { return element; }
			set { element = value; }
		}			
	}


	[NDOPersistent, NDOOidType(typeof(Guid))]
	public class ClassWithHint
	{
		string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
	}

	[NDOPersistent]
	public abstract class AbstractBaseGuid
	{
		[NDOObjectId]
		Guid guid;
		public Guid Guid
		{
			get { return guid; }
			set { guid = value; }
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