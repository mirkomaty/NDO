using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using NDO.Xml;

namespace VisualStudioProject.DomainClasses
{
	public class Configuration
	{
		string value;
		string condition;

		public Configuration( XmlNode xmlNode )
		{
            this.condition = (string) XmlHelper.GetAttribute(xmlNode, "Condition", typeof(string));
            this.value = xmlNode.InnerText;
		}

		public void Save( XmlNode parentNode, string elementName )
		{
            XmlDocument parentDocument = parentNode as XmlDocument;
            if (parentDocument == null)
            {
                parentDocument = parentNode.OwnerDocument;
            }
            XmlElement myElement = parentDocument.CreateElement(elementName);
            parentNode.AppendChild(myElement);
			myElement.InnerText = this.value;
			myElement.SetAttribute("Condition", this.condition);
		}

		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
		public string Condition
		{
			get { return this.condition; }
			set { this.condition = value; }
		}
	}
}
