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
