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
using System.Text;
using System.Collections;
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Summary for PersistentClass.
	/// </summary>
	public class PersistentClass : Class
	{
		public PersistentClass(bool isAbstract, string name, Class baseClass) : base (isAbstract, name, baseClass)
		{			
			this.Attributes.Add("NDOPersistent");
		}


        public Relation NewRelation(RelInfo ri, string relatedTypeName)
        {
            Relation r;
            this.relations.Add((r = new Relation(this, ri, relatedTypeName)));
            if (ri.IsList)
            {
                this.Properties.Add(new Property("List<" + relatedTypeName + ">", Relation.StandardFieldName, true, true));
            }
            else
            {
                this.Properties.Add(new Property(relatedTypeName, Relation.StandardFieldName, true, true));
            }
            return r;
        }

        public Relation NewForeignRelation(RelInfo ri, string relatedTypeName)
        {
            RelInfo ri2 = ri.GetReverse();
            return NewRelation(ri2, relatedTypeName);
        }

        ArrayList relations = new ArrayList();
        public ArrayList Relations
        {
            get { return relations; }
        }

        protected virtual void GenerateRelations(StringBuilder sb)
        {
            foreach (Relation r in relations)
                AddStringCollection(sb, r.Text);
        }

        protected override void GenerateFields(StringBuilder sb)
        {
            base.GenerateFields(sb);
            GenerateRelations(sb);
        }

	}
}
