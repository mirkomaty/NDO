﻿//
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
    public class PropertyGroup
    {
        
		Dictionary<string, string> properties = new Dictionary<string,string>();

		string condition;
		public string Condition
		{
			get { return condition; }
			set { condition = value; }
		}
        
        [NDORelation()]
        private Project project;

		private Platform platform;

		private Configuration configuration;
        
        public PropertyGroup()
        {
        }
        
        public PropertyGroup(XmlNode xmlNode)
        {
            this.condition = ((string)(XmlHelper.GetAttribute(xmlNode, "Condition", typeof(string))));

			foreach ( XmlNode child in xmlNode.ChildNodes )
			{
				if (child.Name != "Platform" && child.Name != "Configuration")
					this.properties.Add(child.Name, (string)XmlHelper.GetElement(xmlNode, child.Name, typeof(string)));
			}

			XmlNode pfNode = xmlNode.SelectSingleNode("Platform");
			if (pfNode != null)
				this.platform = new Platform(pfNode);
			XmlNode confNode = xmlNode.SelectSingleNode("Configuration");
			if (confNode != null)
				this.configuration = new Configuration(confNode);
        }

		public void SetProperty( string name, string value )
		{
			if (this.properties[name] != null)
				this.properties.Remove(name);
			this.properties.Add(name, value);
		}

		public string this[string name]
		{
			get
			{
				return properties[name];
			}
			set
			{
				SetProperty(name, value);
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


		public virtual Platform Platform
		{
			get { return platform; }
			set { platform = value; }
		}
		public virtual Configuration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		}

		public Dictionary<string, string> Properties
		{
			get { return properties; }
			set { properties = value; }
		}


        
        // This method is the default save method to be used to store document root objects.
        public virtual void Save(XmlNode parentNode)
        {
            this.Save(parentNode, "PropertyGroup");
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

			XmlHelper.SetAttribute(myElement, "Condition", this.condition, typeof(string));

			foreach(KeyValuePair<string, string> kvp in this.properties)
				XmlHelper.SetElement(myElement, kvp.Key, kvp.Value, typeof(string));

			if (this.platform != null)
				this.platform.Save(myElement, "Platform");
			if (this.configuration != null)
				this.configuration.Save(myElement, "Configuration");
        }
    }
}
