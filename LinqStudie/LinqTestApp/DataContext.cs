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

namespace NDO.Linq
{
    public class DataContext
    {
        protected PersistenceManager pm;
        public DataContext()
        {
            this.pm = new PersistenceManager();
        }
        public DataContext(string s)
        {
            this.pm = new PersistenceManager(s);
        }

        public bool VerboseMode
        {
            get { return pm.VerboseMode; }
            set { pm.VerboseMode = value; }
        }

        public NDO.Logging.ILogAdapter LogAdapter
        {
            get { return pm.LogAdapter; }
            set { pm.LogAdapter = value; }
        }

        public Table<T> GetTable<T>() where T : LinqHelperBase
        {
            return new Table<T>(pm);
        }

        public void SubmitChanges()
        {
            pm.Save();
        }
    }
}
