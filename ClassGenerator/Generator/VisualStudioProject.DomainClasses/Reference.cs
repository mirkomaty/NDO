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
    public class Reference
    {
        
        private string include;
        
        private string name;
        
        private string hintPath;
        
        private string assemblyFolderKey;

		private string @private;

		private string specificVersion;
        
        [NDORelation()]
        private ItemGroup itemGroup;
        
        public Reference()
        {
        }
        
        public Reference(XmlNode xmlNode)
        {
            this.include = ((string)(XmlHelper.GetAttribute(xmlNode, "Include", typeof(string))));
            this.name = ((string)(XmlHelper.GetElement(xmlNode, "Name", typeof(string))));
            this.hintPath = ((string)(XmlHelper.GetElement(xmlNode, "HintPath", typeof(string))));
            this.assemblyFolderKey = ((string)(XmlHelper.GetElement(xmlNode, "AssemblyFolderKey", typeof(string))));
            this.@private = ((string)(XmlHelper.GetElement(xmlNode, "Private", typeof(string))));
            this.specificVersion = ((string)(XmlHelper.GetElement(xmlNode, "SpecificVersion", typeof(string))));
        }
        
        public virtual string Include
        {
            get
            {
                return this.include;
            }
            set
            {
                this.include = value;
            }
        }
        
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        
        public virtual string HintPath
        {
            get
            {
                return this.hintPath;
            }
            set
            {
                this.hintPath = value;
            }
        }
        
        public virtual string AssemblyFolderKey
        {
            get
            {
                return this.assemblyFolderKey;
            }
            set
            {
                this.assemblyFolderKey = value;
            }
        }

		public string Private
		{
			get { return @private; }
			set { @private = value; }
		}

		public string SpecificVersion
		{
			get { return specificVersion; }
			set { specificVersion = value; }
		}

        
        public virtual ItemGroup ItemGroup
        {
            get
            {
                return this.itemGroup;
            }
            set
            {
                this.itemGroup = value;
            }
        }
        
        // This method is the default save method to be used to store document root objects.
        public virtual void Save(XmlNode parentNode)
        {
            this.Save(parentNode, "Reference");
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
            XmlHelper.SetAttribute(myElement, "Include", this.include, typeof(string));
            XmlHelper.SetElement(myElement, "Name", this.name, typeof(string));
            XmlHelper.SetElement(myElement, "HintPath", this.hintPath, typeof(string));
            XmlHelper.SetElement(myElement, "AssemblyFolderKey", this.assemblyFolderKey, typeof(string));
            XmlHelper.SetElement(myElement, "SpecificVersion", this.specificVersion, typeof(string));
            XmlHelper.SetElement(myElement, "Private", this.@private, typeof(string));
        }
    }
}
