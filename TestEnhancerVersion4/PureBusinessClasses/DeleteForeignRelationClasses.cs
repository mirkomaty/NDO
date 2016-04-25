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

namespace DeleteForeignRelation
{
	[NDOPersistent]
	public class DfrContact 
	{
		private string name;
		[NDORelation(typeof(DfrAddressDescriptor))]
		private IList addresses = new ArrayList();

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public IList Addresses 
		{
			get { return ArrayList.ReadOnly(addresses); }
		}
	}

	[NDOPersistent]
	public class DfrAddress 
	{
		private string ort;
		[NDORelation]
		private DfrContact originContact = null;
		[NDORelation(typeof(DfrAddressDescriptor), RelationInfo.Composite)]
		IList descriptorList = new ArrayList();

		public string Ort 
		{
			get { return ort; }
			set { ort = value; }
		}

		public DfrContact OriginContact 
		{
			get { return originContact; }
			set { originContact = value; }
		}

		public IList DescriptorList 
		{
			get { return ArrayList.ReadOnly(descriptorList); }
		}

		public DfrAddressDescriptor NewAddressDescriptor(DfrContact p_Contact) 
		{
			DfrAddressDescriptor oDescriptor = new DfrAddressDescriptor();
			descriptorList.Add(oDescriptor);
			oDescriptor.DfrContact = p_Contact;         
			return oDescriptor;
		}
      
		public void RemoveAddressDescriptor(DfrAddressDescriptor a) 
		{
			if (descriptorList.Contains(a))
				descriptorList.Remove(a);
		}
	}


	[NDOPersistent]
	public class DfrAddressDescriptor 
	{
		private bool isAdopted = false;
		//private AddressType addressType = AddressType.Private;
		[NDORelation]
		private DfrContact contact = null;
		[NDORelation]
		private DfrAddress address = null;

		public bool IsAdopted 
		{
			get { return isAdopted; }
			set { isAdopted = value; }
		}


		public DfrContact DfrContact 
		{
			get { return contact; }
			set { contact = value; }
		}

		public DfrAddress Address 
		{
			get { return address; }
		}
	}


}
