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
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;

namespace ClassGenerator.SchemaAnalyzer
{
	// See Feature Request 1904596
	class XsdAnalyzer
	{
		XmlSchema schema;
		DataSet generatedDataSet; 
		List<string> validationMessages;
		List<RelationEntry> relations;
        NamespaceWrapper namespaceWrapper;

		class MyXmlResolver : XmlUrlResolver
		{
			string path;

			public MyXmlResolver( string filename )
			{
				this.path = Path.GetDirectoryName(filename);
			}

			public override Uri ResolveUri( Uri baseUri, string relativeUri )
			{
				if (String.IsNullOrEmpty(baseUri.ToString()))
				return new Uri(Path.Combine(this.path, relativeUri));
				return new Uri(Path.Combine(Path.GetDirectoryName(baseUri.ToString()), relativeUri));
			}

		}

		public List<string> ValidationMessages
		{
			get { return validationMessages; }
		}

		public DataSet GeneratedDataSet
		{
			get { return generatedDataSet; }
		}


		/// <summary>
		/// Tries to add an id column "IDnameBase", which is either a primary key or a foreign key.
		/// If a column of the same name 
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="nameBase"></param>
		/// <returns></returns>
		string GetIdColumnName( DataTable dt, string nameBase )
		{
			string colName = "ID" + nameBase;
			int i = 1;
			while(dt.Columns.Contains(colName))
			{
				colName = "ID" + nameBase + i;
				i++;
			}
			return colName;
		}

		public DataSet Analyze(string fileName)
		{
			this.generatedDataSet = new DataSet();
			this.validationMessages = new List<string>();
			this.relations = new List<RelationEntry>();

			// Determine Encoding
			StreamReader sr = new StreamReader(fileName);
			string xmlDecl = sr.ReadLine();
			sr.Close();

			Regex regex = new Regex(@"encoding=""UTF-(\d+)""");
			Match match = regex.Match(xmlDecl);
			Encoding encoding = Encoding.Default;
			if ( match.Success )
			{
				if (match.Groups[1].Value == "8")
					encoding = Encoding.UTF8;
				if (match.Groups[1].Value == "16")
					encoding = Encoding.Unicode;
			}

			sr = new StreamReader(fileName, encoding);
			string xsdString = sr.ReadToEnd();
			StringReader strreader = new StringReader(xsdString);
			this.schema = XmlSchema.Read( strreader, new ValidationEventHandler ( OnValidation ) );
			sr.Close();

			MyXmlResolver resolver = new MyXmlResolver(fileName);
            this.namespaceWrapper = new NamespaceWrapper(this.schema.Namespaces.ToArray(), this.schema.TargetNamespace);
            generatedDataSet.ExtendedProperties.Add("namespacewrapper", this.namespaceWrapper);
            
			this.schema.Compile(OnValidation, resolver);

            foreach (DictionaryEntry de in this.schema.Groups)
            {
                XmlSchemaObject xso = (XmlSchemaObject) de.Value;
                XmlSchemaGroup xsg = xso as XmlSchemaGroup;
                if (xsg != null)
                {
                    AnalyzeGroup(xsg.Particle); 
                }
            }


			// Analyze the top-level elements
			foreach ( DictionaryEntry de in this.schema.Elements )
			{
				XmlSchemaElement se = (XmlSchemaElement) de.Value;
				AnalyzeElement (se, null);
			}
            

			foreach ( DataTable dt in generatedDataSet.Tables )
			{
				string pkColumnName = GetIdColumnName(dt, dt.TableName);
				DataColumn pkColumn  = dt.Columns.Add(pkColumnName, typeof(int));
				pkColumn.AutoIncrement = true;
				pkColumn.AutoIncrementSeed = 1;
				pkColumn.AutoIncrementStep = 1;
				dt.PrimaryKey = new DataColumn[]{pkColumn};
			}

			foreach ( RelationEntry re in this.relations )
			{
				DataTable childTable = generatedDataSet.Tables[re.ChildTable];
				if (childTable == null)  // Relations to skipped elements are listed, but won't used
					continue;
				string relName = string.Empty;
                //if ( re.ChildTable.CompareTo( re.ParentTable ) < 0 )
                //    relName = "fkc" + re.ChildTable + re.ParentTable;
                //else
                //    relName = "fkc" + re.ParentTable + re.ChildTable;
				if ( !this.generatedDataSet.Relations.Contains( relName ) )
				{
					DataTable parentTable = generatedDataSet.Tables[re.ParentTable];
					string fkColumnName = GetIdColumnName( childTable, parentTable.TableName );
					DataColumn fkColumn = childTable.Columns.Add( fkColumnName, typeof( int ) );
					DataRelation dataRelation = new DataRelation( relName, parentTable.PrimaryKey[0], fkColumn );
					dataRelation.ExtendedProperties.Add( "xpath", re.XPath );
					this.generatedDataSet.Relations.Add( dataRelation );
				}
			}

			return this.generatedDataSet;
		}

        Type GetClrType(XmlSchemaElement xse)
        {
            if (!xse.SchemaTypeName.IsEmpty)
                return GetClrType(xse.SchemaTypeName);
            XmlSchemaSimpleType xst = xse.SchemaType as XmlSchemaSimpleType;
            if (xst == null)
                return null;  // i.e. abstract type with no elements
            XmlSchemaSimpleTypeRestriction tr = xst.Content as XmlSchemaSimpleTypeRestriction;
            return (GetClrType(tr.BaseTypeName));
        }


        public XmlQualifiedName GetBaseTypeName(XmlSchemaComplexType ct)
        {
            XmlSchemaSimpleContent sc = ct.ContentModel as XmlSchemaSimpleContent;
            if (sc == null)
                return null;
            XmlSchemaSimpleContentExtension ex = sc.Content as XmlSchemaSimpleContentExtension;
            if (ex != null)
                return ex.BaseTypeName;
            XmlSchemaSimpleContentRestriction cr = sc.Content as XmlSchemaSimpleContentRestriction;
            if (cr != null)
                return cr.BaseTypeName;
            return null;
        }
        


		Type GetClrType( XmlQualifiedName qname )
		{
            while (this.schema.SchemaTypes[qname] != null)
            {
                XmlSchemaSimpleType xst = this.schema.SchemaTypes[qname] as XmlSchemaSimpleType;
                if (xst != null)
                {
                    XmlSchemaSimpleTypeRestriction str = xst.Content as XmlSchemaSimpleTypeRestriction;
                    if (str != null)
                    {
                        qname = str.BaseTypeName;
                        continue;
                    }
                    XmlSchemaSimpleTypeUnion stu = xst.Content as XmlSchemaSimpleTypeUnion;
                    if (stu != null)
                    {
                        if (stu.MemberTypes.Length == 0)
                            return null;            // can't determine the type
                        qname = stu.MemberTypes[0]; // Needs just the first, since all types should map to 
                        // the same primitive type
                    }
                    else
                        return null;  // cant't determine the type
                }
                // In some cases the element has a type by itself but
                // has additional attributes
                XmlSchemaComplexType ct = this.schema.SchemaTypes[qname] as XmlSchemaComplexType;
                if (ct != null)
                {
                    qname = GetBaseTypeName(ct);
                    if (qname == null)
                        return null;   // xs:any or such things... That means: no own type
                }
            }

			switch ( qname.Name )
			{
			case "boolean":
				return typeof( bool );
			case "byte":
				return typeof( byte );
			case "date":
			case "time":
			case "dateTime":
				return typeof( DateTime );
			case "decimal":
				return typeof( decimal );
			case "double":
				return typeof( double );
			case "duration":
				return typeof( TimeSpan );
			case "long":
				return typeof( long );
			case "int":   // limited to 32 bits
			case "negativeInteger":
			case "nonNegativeInteger":
			case "nonPositiveInteger":
			case "positiveInteger":
			case "integer":  // unlimited number without fractional part
				return typeof( int );
			case "short":
				return typeof( short );
			case "unsignedByte":
				return typeof( byte );
			case "unsignedInt":
				return typeof( uint );
			case "unsignedLong":
				return typeof( ulong );
			case "unsignedShort":
				return typeof( ushort );
			default:
				return typeof( string );
			}
		}


        void AnalyzeGroup(XmlSchemaGroupBase xsg)
        {
            foreach (XmlSchemaObject xso in xsg.Items)
            {
                XmlSchemaElement el = xso as XmlSchemaElement;
                if (el != null)
                {
                    AnalyzeElement(el, null);
                    continue;
                }
                XmlSchemaGroupBase innerGroup = xso as XmlSchemaGroupBase;
                if (innerGroup != null)
                {
                    AnalyzeGroup(innerGroup);
                    continue;
                }
            }
            // groupref don't have to be analyzed. groupref references another group which should be treated by its own.
        }
        


        void AnalyzeElement(XmlSchemaElement element, XmlSchemaElement parentElement)
        {
            ElementWrapper elementWrapper = new ElementWrapper(element);
            this.validationMessages.AddRange(elementWrapper.Warnings);
            // Don't process simple types
            if (!elementWrapper.IsComplexType)
                return;
            string elementTypeName = elementWrapper.TypeName;

            bool skip = elementWrapper.IsComplexType && elementWrapper.SimpleSubElements.Count == 0 && elementWrapper.Attributes.Count == 0 && elementWrapper.ComplexSubElements.Count < 2;
            bool isAlreadyGenerated = false;

            if (!skip)
            {
                DataTable dt;
                if (generatedDataSet.Tables.Contains(elementTypeName))
                {
                    dt = generatedDataSet.Tables[elementTypeName];
                    isAlreadyGenerated = true;
                }
                else
                {
                    dt = new DataTable(elementTypeName);
                    if (!string.IsNullOrEmpty(elementWrapper.Summary))
                        dt.ExtendedProperties.Add("summary", elementWrapper.Summary);
                    generatedDataSet.Tables.Add(dt);
                }
                if (!isAlreadyGenerated)
                {
                    foreach (XmlSchemaAttribute attrObj in elementWrapper.Attributes)
                    {
                        Type t = GetClrType(attrObj.SchemaTypeName);
                        DataColumn dc = dt.Columns.Add(attrObj.Name, t);
                        XmlSchemaSummary sum = new XmlSchemaSummary(attrObj);
                        if (!string.IsNullOrEmpty(sum.Text))
                            dc.ExtendedProperties.Add("summary", sum.Text);
                        //						dc.ExtendedProperties.Add( "schemaType", "Attribute" );
                    }
                    foreach (XmlSchemaElement subElement in elementWrapper.SimpleSubElements)
                    {
                        Type t = GetClrType(subElement);
                        if (/* subElement == element && */t == null)  //TODO: Check all cases with subElement != element
                            continue;  // element seemed to have an own type, but it's xs:any or something undetermined
                        string columnName = null;
                        if (subElement == element)
                            columnName = "Value";
                        else
                            columnName = subElement.Name;
                        DataColumn dc = dt.Columns.Add(columnName, t);
                        dc.ExtendedProperties.Add("schemaType", "Element");
                    }
                }
            }
            if (isAlreadyGenerated)
                return;
            //----------------------

            foreach (XmlSchemaElement subEl in elementWrapper.ComplexSubElements)
            {
                ElementWrapper subElementWrapper = new ElementWrapper(subEl, false);
                // Now add the relation
                if (!(skip && parentElement == null)) // otherwise we omit the root element
                {
                    RelationEntry relEntry = new RelationEntry();
                    string ownName = this.namespaceWrapper.GetXPath(subElementWrapper.QualifiedName);
                    if (!skip)
                    {
                        relEntry.ParentTable = elementTypeName;
                        relEntry.XPath = ownName;
                    }
                    else
                    {
                        ElementWrapper parentElementWrapper = new ElementWrapper(parentElement, false);
                        relEntry.ParentTable = parentElementWrapper.TypeName;
                        string parentName = this.namespaceWrapper.GetXPath(elementWrapper.QualifiedName);
                        relEntry.XPath = parentName + "/" + ownName;
                    }
                    relEntry.ChildTable = subElementWrapper.TypeName;

                    this.relations.Add(relEntry);
                }

                if (subElementWrapper.TypeName == elementTypeName)
                    continue;  // don't walk into endless recursion

                AnalyzeElement(subEl, skip ? parentElement : element);
            }
        }


		void OnValidation(object o, ValidationEventArgs args)
		{
            string name = null;
            XmlSchemaElement el = args.Exception.SourceSchemaObject as XmlSchemaElement;
            if (el != null)
            {
                name = "Element '" + el.QualifiedName.Name + "': ";
            }
            else
            {
                name = "Line " + args.Exception.LineNumber + ", Col " + args.Exception.LinePosition + ": ";
            }
			this.validationMessages.Add(name + args.Message);
		}
	}
}
