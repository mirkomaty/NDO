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
