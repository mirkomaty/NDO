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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ClassGenerator.SchemaAnalyzer
{

	// See Feature Request 1904596
	class ElementWrapper
	{
		XmlSchema schema;
        string summary;
        List<string> warnings = new List<string>();

		XmlSchemaElement element;
		List<XmlSchemaElement> complexSubElements = new List<XmlSchemaElement>();
        List<XmlSchemaElement> simpleSubElements = new List<XmlSchemaElement>();
        List<XmlSchemaAttribute> attributes = new List<XmlSchemaAttribute>();
        Hashtable addedElements = new Hashtable();
        
        public List<XmlSchemaElement> ComplexSubElements
		{
			get { return complexSubElements; }
		}
        public List<XmlSchemaElement> SimpleSubElements
		{
			get { return simpleSubElements; }
		}
		public List<XmlSchemaAttribute> Attributes
		{
			get { return attributes; }
		}
        public List<string> Warnings
        {
            get { return warnings; }
        }
        public string Summary
        {
            get { return summary; }
        }



        // The TypeName and ElementName might differ,
        // if an element with a given name is of a complex type
        // with another name.
        public string ElementName
        {
            get
            {
                string typeName = this.element.Name;
                if (typeName == null)
                    typeName = this.element.QualifiedName.Name; ;
                return typeName;
            }
        }


        public XmlQualifiedName QualifiedName
        {
            get
            {
                return this.element.QualifiedName;
            }
        }

        // The TypeName and ElementName might differ,
        // if an element with a given name is of a complex type
        // with another name.
        // If the complex type doesn't have a name, we can assume
        // to have an element or element ref.
        public string TypeName
		{
			get 
			{ 
				string typeName = this.ComplexType.Name;
				if (string.IsNullOrEmpty(typeName))
					typeName = this.element.Name;
                if (typeName == null)
                    typeName = this.element.QualifiedName.Name; ;
				return typeName;
			}
		}

		XmlSchema GetSchema()
		{
			XmlSchemaObject o = this.element.Parent;
			while(!(o is XmlSchema))
				o = o.Parent;
			return (XmlSchema) o;
		}

		public XmlSchemaComplexType ComplexType
		{
			get
			{
				if ( !this.element.SchemaTypeName.IsEmpty )
					return this.schema.SchemaTypes[this.element.SchemaTypeName] as XmlSchemaComplexType;
                if (this.element.ElementSchemaType != null)
                    return this.element.ElementSchemaType as XmlSchemaComplexType;
				return this.element.SchemaType as XmlSchemaComplexType;
			}
		}

		public bool IsComplexType
		{
			get { return this.ComplexType != null; }
		}

		public ElementWrapper( XmlSchemaElement element ) : this(element, true)
		{
		}

        public XmlSchemaSimpleContentExtension SimpleContentExtension
        {
            get
            {
                XmlSchemaComplexType ct = this.ComplexType;
                if (ct == null)
                    return null;
                XmlSchemaSimpleContent sc = ct.ContentModel as XmlSchemaSimpleContent;
                if (sc == null)
                    return null;
                XmlSchemaSimpleContentExtension ex = sc.Content as XmlSchemaSimpleContentExtension;
                return ex;
            }
        }

        public XmlSchemaSimpleContentRestriction SimpleContentRestriction
        {
            get
            {
                XmlSchemaComplexType ct = this.ComplexType;
                if (ct == null)
                    return null;
                XmlSchemaSimpleContent sc = ct.ContentModel as XmlSchemaSimpleContent;
                if (sc == null)
                    return null;
                XmlSchemaSimpleContentRestriction cr = sc.Content as XmlSchemaSimpleContentRestriction;
                return cr;
            }
        }


        void GetAnnotations()
        {
            XmlSchemaSummary xss = new XmlSchemaSummary(this.element);
            if (!string.IsNullOrEmpty(xss.Text))
            {
                this.summary = xss.Text;
                return;
            }
            xss = new XmlSchemaSummary(this.ComplexType);
            this.summary = xss.Text;
        }


        void CheckForSimpleRestriction()
        {
            XmlSchemaComplexType ct = this.ComplexType;
            XmlSchemaSimpleContent sc = ct.ContentModel as XmlSchemaSimpleContent;
            if (sc != null)
                this.simpleSubElements.Add(this.element);
        }


		public ElementWrapper( XmlSchemaElement element, bool collectSubElements )
		{
			this.element = element;
			this.schema = GetSchema();
            XmlSchemaComplexType ct = this.ComplexType;
			if ( collectSubElements && ct != null )
			{
                GetAnnotations();
                CollectSubElements();
                CheckForSimpleRestriction();
                CollectAttributes(ct.Attributes);
                XmlSchemaSimpleContentExtension scex = this.SimpleContentExtension;
                if (scex != null)
                    CollectAttributes(scex.Attributes);
                XmlSchemaSimpleContentRestriction scr = this.SimpleContentRestriction;
                if (scr != null)
                    CollectAttributes(scr.Attributes);
            }
		}


        void CollectAttributes(XmlSchemaObjectCollection xsos)
        {
            foreach (XmlSchemaObject xso in xsos)
            {
                XmlSchemaAttribute attr = xso as XmlSchemaAttribute;
                if (attr != null)
                {
                    this.attributes.Add(attr);
                    continue;
                }
                XmlSchemaAttributeGroupRef gref = xso as XmlSchemaAttributeGroupRef;
                CollectAttributes(((XmlSchemaAttributeGroup)this.schema.AttributeGroups[gref.RefName]).Attributes);
            }
        }


        void CollectSubElements()
        {
            XmlSchemaParticle sp = this.ComplexType.ContentTypeParticle;
            if (sp != null)
                CollectSubElements(sp);
        }

        void Add(List<XmlSchemaElement> l, XmlSchemaElement el)
        {
            string name = el.Name;
            if (name == null)
                name = el.QualifiedName.Name;
            if (name == null)
                throw new Exception("Added element has no name");
            if (this.addedElements.Contains(name))
            {
                this.warnings.Add("Element: '" + this.ElementName + "': Subelement '" + name + "' is defined twice.");
                return;
            }
            addedElements.Add(name, null);
            l.Add(el);
        }

        void CollectSubElements(XmlSchemaParticle particle)
		{
            // choice | sequence
            XmlSchemaGroupBase sgb = particle as XmlSchemaGroupBase;
            if (sgb != null)
            {
                foreach (XmlSchemaObject xso in sgb.Items)
                {
                    XmlSchemaElement xse = xso as XmlSchemaElement;
                    if (xse != null)
                    {
                        // If it's a ref, xse.QualifiedName.Name is the name of the element                    
                        ElementWrapper elw = new ElementWrapper(xse, false);
                        if (elw.IsComplexType)
                            Add(this.complexSubElements, xse);
                        else
                            Add(this.simpleSubElements, xse);

                        continue;
                    }
                    XmlSchemaParticle subParticle = xso as XmlSchemaParticle;
                    if (subParticle != null)
                    {
                        CollectSubElements(subParticle);
                        continue;
                    }
                }
            }
            // group
            XmlSchemaGroupRef groupRef = particle as XmlSchemaGroupRef;
            if (groupRef != null)
            {
                CollectSubElements(groupRef.Particle);
            }

		}

	}
}
