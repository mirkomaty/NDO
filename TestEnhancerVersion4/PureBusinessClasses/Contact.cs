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
using Reisekosten.Personal;
using NDO;

namespace PureBusinessClasses
{
	[NDOPersistent]
	public abstract class Contact
	{
		string firstName;
		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}
		string lastName;
		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		[NDORelation(typeof(Adresse))]
		IList addresses = new ArrayList();
		public IList Addresses
		{
			get { return ArrayList.ReadOnly(addresses); }
			set { addresses = value; }
		}
		public void AddAdresse(Adresse a)
		{
			addresses.Add(a);
		}
		public void RemoveAdresse(Adresse a)
		{
			if (addresses.Contains(a))
				addresses.Remove(a);
		}
		
		public Contact()
		{
		}
	}
}
