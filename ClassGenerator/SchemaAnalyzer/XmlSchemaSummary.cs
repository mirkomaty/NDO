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
