﻿//
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
using System.Linq;
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class Country
    {

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [NDORelation(RelationInfo.Default)]
        List<Travel> travels = new List<Travel>();
        public IEnumerable<Travel> Travels
        {
        	get { return this.travels; }
        	set { this.travels = value.ToList(); }
        }
        public void AddTravel(Travel t)
        {
        	this.travels.Add(t);
        }
        public void RemoveTravel(Travel t)
        {
        	if (this.travels.Contains(t))
        		this.travels.Remove(t);
        }

        public Country()
        {
        }
    }
}
