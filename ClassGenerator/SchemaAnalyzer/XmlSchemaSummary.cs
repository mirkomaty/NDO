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
using System.Xml;
using System.Xml.Schema;

namespace ClassGenerator.SchemaAnalyzer
{
    class XmlSchemaSummary
    {
        string summary;
        public string Text
        {
            get { return summary; }
        }

        public XmlSchemaSummary(XmlSchemaObject obj)
        {
            XmlSchemaAnnotated xsa = obj as XmlSchemaAnnotated;
            if (xsa == null)
                return;
            XmlSchemaAnnotation ann = xsa.Annotation;
            if (ann == null)
                return;

            foreach (XmlSchemaObject xso in ann.Items)
            {
                XmlSchemaDocumentation doc = xso as XmlSchemaDocumentation;
                if (doc == null)
                    continue;
                this.summary = string.Empty;
                bool first = true;
                foreach (XmlNode node in doc.Markup)
                {
                    string text = node.InnerText.Replace("\r\n", " ");
                    text = text.Replace("\n", " ");
                    text = text.Replace("\t", " ");
                    text = text.Trim();
                    while (text.IndexOf("  ") > 0)
                        text = text.Replace("  ", " ");
                    if (!first)
                        this.summary += ' ';
                    this.summary += text;
                    first = false;
                }
            }

        }
    }
}
