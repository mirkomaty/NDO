
namespace VisualStudioProject.DomainClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using NDO.Xml;
    using NDO;
    
    
    [NDOPersistent()]
    public class UserProperties
    {                
        [NDORelation()]
        private Project project;

		private Dictionary<string, string> properties = new Dictionary<string,string>();

		public Dictionary<string, string> Properties
		{
			get { return properties; }
			set { properties = value; }
		}
        
        public UserProperties()
        {
        }
        
        public UserProperties(XmlNode xmlNode)
        {
			foreach(XmlAttribute attr in xmlNode.Attributes)
				this.properties.Add(attr.Name, attr.Value);
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
            this.Save(parentNode, "UserProperties");
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
			foreach(KeyValuePair<string, string> kvp in this.properties)
				myElement.SetAttribute(kvp.Key, kvp.Value);
        }
    }
}
