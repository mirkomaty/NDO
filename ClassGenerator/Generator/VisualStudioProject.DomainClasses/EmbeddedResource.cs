
namespace VisualStudioProject.DomainClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using NDO.Xml;
    using NDO;
    
    
    [NDOPersistent()]
    public class EmbeddedResource
    {
        
        private string include;
        
        private string dependentUpon;
        
        [NDORelation()]
        private ItemGroup itemGroup;
        
        public EmbeddedResource()
        {
        }
        
        public EmbeddedResource(XmlNode xmlNode)
        {
            this.include = ((string)(XmlHelper.GetAttribute(xmlNode, "Include", typeof(string))));
            this.dependentUpon = ((string)(XmlHelper.GetElement(xmlNode, "DependentUpon", typeof(string))));
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
        
        public virtual string DependentUpon
        {
            get
            {
                return this.dependentUpon;
            }
            set
            {
                this.dependentUpon = value;
            }
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
            this.Save(parentNode, "EmbeddedResource");
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
            XmlHelper.SetElement(myElement, "DependentUpon", this.dependentUpon, typeof(string));
        }
    }
}
