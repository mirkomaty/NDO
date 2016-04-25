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


namespace VisualStudioProject.DomainClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using NDO.Xml;
    using NDO;
    
    
    [NDOPersistent()]
    public class Import
    {
        
        private string projectImportPath;
        
        [NDORelation()]
        private Project project;
        
        public Import()
        {
        }
        
        public Import(XmlNode xmlNode)
        {
            this.projectImportPath = ((string)(XmlHelper.GetAttribute(xmlNode, "Project", typeof(string))));
        }
        
        public virtual string ProjectImportPath
        {
            get
            {
                return this.projectImportPath;
            }
            set
            {
                this.projectImportPath = value;
            }
        }
        
        public virtual Project Project
        {
            get
            {
                return this.project;
            }
            set
            {
                this.project = value;
            }
        }
        
        // This method is the default save method to be used to store document root objects.
        public virtual void Save(XmlNode parentNode)
        {
            this.Save(parentNode, "Import");
        }
        
        // This method will be used for non-root objects.
        // Note that a complex type can be saved by different parent nodes using different element names.
        public virtual void Save(XmlNode parentNode, string elementName)
        {
            XmlDocument parentDocument = parentNode as XmlDocument;
            if (parentDocument == null)
            {
                parentDocument = parentNode.OwnerDocument;
            }
            XmlElement myElement = parentDocument.CreateElement(elementName);
            parentNode.AppendChild(myElement);
            XmlHelper.SetAttribute(myElement, "Project", this.projectImportPath, typeof(string));
        }
    }
}
