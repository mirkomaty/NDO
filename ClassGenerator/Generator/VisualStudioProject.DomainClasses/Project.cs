
namespace VisualStudioProject.DomainClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using NDO.Xml;
    using NDO;
    
    
    [NDOPersistent()]
    public class Project
    {
        
        private string defaultTargets;
        private string toolsVersion;
        
        [NDORelation(typeof(PropertyGroup), RelationInfo.Composite)]
        private List<PropertyGroup> propertyGroups = new List<PropertyGroup>();
        
        [NDORelation(typeof(ItemGroup), RelationInfo.Composite)]
        private List<ItemGroup> itemGroups = new List<ItemGroup>();
        
        [NDORelation(typeof(UserProperties), RelationInfo.Composite)]
        private List<UserProperties> userProperties = new List<UserProperties>();
        
        [NDORelation(typeof(Import), RelationInfo.Composite)]
        private List<Import> imports = new List<Import>();
        
        public Project()
        {
        }
        
        public Project(XmlNode xmlNode)
        {
            int i;
            this.defaultTargets = ((string)(XmlHelper.GetAttribute(xmlNode, "DefaultTargets", typeof(string))));
            this.toolsVersion = ((string)(XmlHelper.GetAttribute(xmlNode, "ToolsVersion", typeof(string))));
            XmlNodeList propertyGroupsNodeList = xmlNode.SelectNodes("PropertyGroup");
            for (i = 0; (i < propertyGroupsNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = propertyGroupsNodeList[i];
                this.propertyGroups.Add(new PropertyGroup(subNode));
            }
            XmlNodeList itemGroupsNodeList = xmlNode.SelectNodes("ItemGroup");
            for (i = 0; (i < itemGroupsNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = itemGroupsNodeList[i];
                this.itemGroups.Add(new ItemGroup(subNode));
            }
            XmlNodeList userPropertiesNodeList = xmlNode.SelectNodes("ProjectExtensions/VisualStudio/UserProperties");
            for (i = 0; (i < userPropertiesNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = userPropertiesNodeList[i];
                this.userProperties.Add(new UserProperties(subNode));
            }
            XmlNodeList importsNodeList = xmlNode.SelectNodes("Import");
            for (i = 0; (i < importsNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = importsNodeList[i];
                this.imports.Add(new Import(subNode));
            }
        }

		public virtual PropertyGroup MainPropertyGroup
		{
			get
			{
				foreach (PropertyGroup pg in this.propertyGroups)
				{
					if (pg["ProjectGuid"] != null)
						return pg;
				}
				return null;
			}
		}
        
        public virtual string DefaultTargets
        {
            get
            {
                return this.defaultTargets;
            }
            set
            {
                this.defaultTargets = value;
            }
        }

        public string ToolsVersion
        {
            get { return toolsVersion; }
            set { toolsVersion = value; }
        }

        
        public virtual List<PropertyGroup> PropertyGroups
        {
            get
            {
                return new NDOReadOnlyGenericList<PropertyGroup>(this.propertyGroups);
            }
            set
            {
                this.propertyGroups = value;
            }
        }
        
        public virtual List<ItemGroup> ItemGroups
        {
            get
            {
                return new NDOReadOnlyGenericList<ItemGroup>(this.itemGroups);
            }
            set
            {
                this.itemGroups = value;
            }
        }
        
        public virtual List<UserProperties> UserPropertieses
        {
            get
            {
                return new NDOReadOnlyGenericList<UserProperties>(this.userProperties);
            }
            set
            {
                this.userProperties = value;
            }
        }
        
        public virtual List<Import> Imports
        {
            get
            {
                return new NDOReadOnlyGenericList<Import>(this.imports);
            }
            set
            {
                this.imports = value;
            }
        }
        
        // This method is the default save method to be used to store document root objects.
        public virtual void Save(XmlNode parentNode)
        {
            this.Save(parentNode, "Project");
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
			if (myElement.Attributes["xmlns"] == null)
				myElement.SetAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
            int i;
            XmlHelper.SetAttribute(myElement, "DefaultTargets", this.defaultTargets, typeof(string));
            if (this.toolsVersion != null)
                XmlHelper.SetAttribute(myElement, "ToolsVersion", this.toolsVersion, typeof(string));
            for (i = 0; (i < this.propertyGroups.Count); i = (i + 1))
            {
                PropertyGroup relObject = this.propertyGroups[i];
                relObject.Save(myElement, "PropertyGroup");
            }
            for (i = 0; (i < this.itemGroups.Count); i = (i + 1))
            {
                ItemGroup relObject = this.itemGroups[i];
                relObject.Save(myElement, "ItemGroup");
            }

			if ( this.userProperties.Count > 0 )
			{
				XmlElement projectExtensionsElement = parentDocument.CreateElement( "ProjectExtensions" );
				myElement.AppendChild( projectExtensionsElement );
				XmlElement visualStudioElement = parentDocument.CreateElement( "VisualStudio" );
				projectExtensionsElement.AppendChild( visualStudioElement );
				for ( i = 0; (i < this.userProperties.Count); i = (i + 1) )
				{
					UserProperties relObject = this.userProperties[i];
					relObject.Save( visualStudioElement, "UserProperties" );
				}
			}
            for (i = 0; (i < this.imports.Count); i = (i + 1))
            {
                Import relObject = this.imports[i];
                relObject.Save(myElement, "Import");
            }
        }
        
        public virtual PropertyGroup NewPropertyGroup()
        {
            PropertyGroup p = new PropertyGroup();
            this.propertyGroups.Add(p);
            return p;
        }
        
        public virtual void RemovePropertyGroup(PropertyGroup p)
        {
            if (this.propertyGroups.Contains(p))
            {
                this.propertyGroups.Remove(p);
            }
        }
        
        public virtual ItemGroup NewItemGroup()
        {
            ItemGroup i = new ItemGroup();
            this.itemGroups.Add(i);
            return i;
        }
        
        public virtual void RemoveItemGroup(ItemGroup i)
        {
            if (this.itemGroups.Contains(i))
            {
                this.itemGroups.Remove(i);
            }
        }
        
        public virtual UserProperties NewUserProperties()
        {
            UserProperties u = new UserProperties();
            this.userProperties.Add(u);
            return u;
        }
        
        public virtual void RemoveUserProperties(UserProperties u)
        {
            if (this.userProperties.Contains(u))
            {
                this.userProperties.Remove(u);
            }
        }
        
        public virtual Import NewImport()
        {
            Import i = new Import();
            this.imports.Add(i);
            return i;
        }
        
        public virtual void RemoveImport(Import i)
        {
            if (this.imports.Contains(i))
            {
                this.imports.Remove(i);
            }
        }
    }
}
