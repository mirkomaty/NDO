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
using System.Collections.Generic;
using System.Text;
using NDO;

namespace DoubleRelationClasses
{
    [NDOPersistent]
    public class DRAddress
    {
        private string street;
        [NDORelation(RelationInfo.Default, "RelAddresses")]
        private DRPerson person;

        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        public DRPerson Person
        {
            get { return person; }
        }

        public DRAddress()
        {
            this.street = string.Empty;
        }
    }


    [NDOPersistent]
    public class DRPerson
    {
        private string name;
        private string lastName;

        [NDORelation(RelationInfo.Default, "RelDefaultAddress")]
        private DRAddress defaultAddress;
        [NDORelation(RelationInfo.Composite, "RelAddresses")]
        private IList<DRAddress> addresses = new List<DRAddress>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public DRAddress DefaultAddress
        {
            get { return defaultAddress; }
            set { defaultAddress = value; }
        }

        public IList<DRAddress> Addresses
        {
            get { return new NDOReadOnlyGenericList<DRAddress>(addresses); }
        }

        public DRPerson()
        {
            this.name = string.Empty;
            this.lastName = string.Empty;
        }

        public DRAddress NewAddress()
        {
            DRAddress a = new DRAddress();
            addresses.Add(a);
            return a;
        }

        public void RemoveAddress(DRAddress a)
        {
            if (addresses.Contains(a))
                addresses.Remove(a);
        }
    }

}
