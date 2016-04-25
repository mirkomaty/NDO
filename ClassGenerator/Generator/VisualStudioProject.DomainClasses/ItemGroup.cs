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
    public class ItemGroup
    {
        
        [NDORelation()]
        private Project project;
        
        [NDORelation(typeof(Compile), RelationInfo.Composite)]
        private List<Compile> compiles = new List<Compile>();
        
        [NDORelation(typeof(EmbeddedResource), RelationInfo.Composite)]
        private List<EmbeddedResource> embeddedResources = new List<EmbeddedResource>();
        
        [NDORelation(typeof(Content), RelationInfo.Composite)]
        private List<Content> contents = new List<Content>();
        
        [NDORelation(typeof(Reference), RelationInfo.Composite)]
        private List<Reference> references = new List<Reference>();

        [NDORelation(typeof(Content), RelationInfo.Composite)]
        private List<Content> imports = new List<Content>();

        public ItemGroup()
        {
        }
        
        public ItemGroup(XmlNode xmlNode)
        {
            int i;
            XmlNodeList compilesNodeList = xmlNode.SelectNodes("Compile");
            for (i = 0; (i < compilesNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = compilesNodeList[i];
                this.compiles.Add(new Compile(subNode));
            }
            XmlNodeList embeddedResourcesNodeList = xmlNode.SelectNodes("EmbeddedResource");
            for (i = 0; (i < embeddedResourcesNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = embeddedResourcesNodeList[i];
                this.embeddedResources.Add(new EmbeddedResource(subNode));
            }
            XmlNodeList contentsNodeList = xmlNode.SelectNodes("Content");
            for (i = 0; (i < contentsNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = contentsNodeList[i];
                this.contents.Add(new Content(subNode));
            }
            XmlNodeList referencesNodeList = xmlNode.SelectNodes("Reference");
            for (i = 0; (i < referencesNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = referencesNodeList[i];
                this.references.Add(new Reference(subNode));
            }
			XmlNodeList importsNodeList = xmlNode.SelectNodes("Import");
            for (i = 0; (i < importsNodeList.Count); i = (i + 1))
            {
                XmlNode subNode = importsNodeList[i];
                this.imports.Add(new Content(subNode));
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
        
        public virtual List<Compile> Compiles
        {
            get
            {
                return new NDOReadOnlyGenericList<Compile>(this.compiles);
            }
            set
            {
                this.compiles = value;
            }
        }
        
        public virtual List<EmbeddedResource> EmbeddedResources
        {
            get
            {
                return new NDOReadOnlyGenericList<EmbeddedResource>(this.embeddedResources);
            }
            set
            {
                this.embeddedResources = value;
            }
        }
        
        public virtual List<Content> Contents
        {
            get
            {
                return new NDOReadOnlyGenericList<Content>(this.contents);
            }
            set
            {
                this.contents = value;
            }
        }

		public virtual List<Content> Imports
        {
            get
            {
                return new NDOReadOnlyGenericList<Content>(this.imports);
            }
            set
            {
                this.imports = value;
            }
        }

        
        public virtual List<Reference> References
        {
            get
            {
                return new NDOReadOnlyGenericList<Reference>(this.references);
            }
            set
            {
                this.references = value;
            }
        }
        
        // This method is the default save method to be used to store document root objects.
        public virtual void Save(XmlNode parentNode)
        {
            this.Save(parentNode, "ItemGroup");
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
            int i;
            for (i = 0; (i < this.compiles.Count); i = (i + 1))
            {
                Compile relObject = this.compiles[i];
                relObject.Save(myElement, "Compile");
            }
            for (i = 0; (i < this.embeddedResources.Count); i = (i + 1))
            {
                EmbeddedResource relObject = this.embeddedResources[i];
                relObject.Save(myElement, "EmbeddedResource");
            }
            for (i = 0; (i < this.contents.Count); i = (i + 1))
            {
                Content relObject = this.contents[i];
                relObject.Save(myElement, "Content");
            }
            for (i = 0; (i < this.references.Count); i = (i + 1))
            {
                Reference relObject = this.references[i];
                relObject.Save(myElement, "Reference");
            }

			foreach(Content relObject in this.imports)
				relObject.Save(myElement, "Import");
        }
        
        public virtual Compile NewCompile()
        {
            Compile c = new Compile();
            this.compiles.Add(c);
            return c;
        }
        
        public virtual void RemoveCompile(Compile c)
        {
            if (this.compiles.Contains(c))
            {
                this.compiles.Remove(c);
            }
        }
        
        public virtual EmbeddedResource NewEmbeddedResource()
        {
            EmbeddedResource e = new EmbeddedResource();
            this.embeddedResources.Add(e);
            return e;
        }
        
        public virtual void RemoveEmbeddedResource(EmbeddedResource e)
        {
            if (this.embeddedResources.Contains(e))
            {
                this.embeddedResources.Remove(e);
            }
        }
        
        public virtual Content NewContent()
        {
            Content c = new Content();
            this.contents.Add(c);
            return c;
        }
        
        public virtual void RemoveContent(Content c)
        {
            if (this.contents.Contains(c))
            {
                this.contents.Remove(c);
            }
        }
        
        public virtual Reference NewReference()
        {
            Reference r = new Reference();
            this.references.Add(r);
            return r;
        }
        
        public virtual void RemoveReference(Reference r)
        {
            if (this.references.Contains(r))
            {
                this.references.Remove(r);
            }
        }
    }
}
