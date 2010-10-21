
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
