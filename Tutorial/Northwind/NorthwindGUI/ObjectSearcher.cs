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
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NDO;

namespace NorthwindGUI
{
    class ObjectSearcher<T> where T : new()
    {
        PersistenceManager pm;
        public ObjectSearcher(PersistenceManager pm)
        {
            this.pm = pm;
        }
        public List<T> SearchObjects()
        {
            object searchTemplate = new T();
            ObjectPropertyForm opf = new ObjectPropertyForm(searchTemplate, true);
            ArrayList parameters = new ArrayList();

            string query = string.Empty;
            if (opf.ShowDialog() != DialogResult.OK)
                return null;

            foreach (FieldEntry fe in opf.SearchParams)
            {
                string name = fe.FieldInfo.Name;
                FieldInfo fi = fe.FieldInfo;
                query += "[" + name + "]";
                string op = " = ";
                if (fi.FieldType == typeof(string))
                    op = " LIKE ";
                query += op;
                query += "{" + parameters.Count + "}";
                parameters.Add(fe.Value);
            }
            if (query == string.Empty)
                query = null;
            NDOQuery<T> ndoQuery = new NDOQuery<T>(pm, query, false);
            if (parameters.Count > 0)
            {
                foreach (object par in parameters)
                    ndoQuery.Parameters.Add(par);
            }
            return ndoQuery.Execute();
        }

        static FieldEntry GetFieldEntry(string name, List<FieldEntry> fieldEntries)
        {
            foreach (FieldEntry fe in fieldEntries)
                if (fe.FieldInfo.Name == name)
                    return fe;
            return null;
        }

    }
}
