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
using NDO;
using System.Collections;

namespace AbstractBaseTestClasses
{

	[NDOPersistent]
	public abstract class ABBase
	{
		string myString;
		public string MyString
		{
			get { return myString; }
			set { myString = value; }
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
	public class AbstractRelLeft
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
		public void AddABBase(ABBase a)
		{
			theBases.Add(a);
		}
		public void RemoveABBase(ABBase a)
		{
			if (theBases.Contains(a))
				theBases.Remove(a);
		}
		
	}
}

