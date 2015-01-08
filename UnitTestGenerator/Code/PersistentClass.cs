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
using System.Text;
using System.Collections;
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Zusammenfassung f�r PersistentClass.
	/// </summary>
	public class PersistentClass : Class
	{
		public PersistentClass(bool isAbstract, string name, Class baseClass) : base (isAbstract, name, baseClass)
		{			
			this.Attributes.Add("NDOPersistent");
		}


        public Relation NewRelation(RelInfo ri, string relatedTypeName)
        {
            if (ri.OtherIsGeneric)
                relatedTypeName += "<int>";
            Relation r;
            this.relations.Add((r = new Relation(this, ri, relatedTypeName)));
            if (ri.IsList)
            {
                this.Properties.Add(new Property("IList", Relation.StandardFieldName, true, true));
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
