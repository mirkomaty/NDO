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

namespace PureBusinessClasses
{
    [NDOPersistent]
    public class Device
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

		[NDORelation(RelationInfo.Composite)]
		List<Device> subdevices = new List<Device>();
        public List<Device> Subdevices
        {
            get { return new NDOReadOnlyGenericList<Device>(subdevices); }
            set { subdevices = (List<Device>)value; }
        }
		public Device NewDevice(Type t)
		{
			Device d = (Device) Activator.CreateInstance(t);
			subdevices.Add(d);
			return d;
		}
		public void RemoveDevice(Device d)
		{
			if (subdevices.Contains(d))
				subdevices.Remove(d);
		}
    }

    [NDOPersistent]
    public class SnmpDevice : Device
    {
        int port;
        public int Port
        {
            get { return port; }
            set { port = value; }
        }
    }
}
