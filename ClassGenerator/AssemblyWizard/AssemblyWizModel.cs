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
using WizardBase;
using System.Xml;
using NDO.Xml;
using System.Collections;

namespace ClassGenerator.AssemblyWizard
{
	/// <summary>
	/// Zusammenfassung für AssemblyWizModel.
	/// </summary>
#if DEBUG
	public class AssemblyWizModel : IModel
#else
	internal class AssemblyWizModel : IModel
#endif
	{
		string connectionType = string.Empty;
		string connectionString = string.Empty;
		string projectDirectory = string.Empty;
		string defaultNamespace = string.Empty;
		string ownerName = string.Empty;
		TargetLanguage targetLanguage = TargetLanguage.CSharp;
		string projectName = string.Empty;
		bool isXmlSchema;
		bool mapStringsAsGuids;
		string xmlSchemaFile;
		public string XmlSchemaFile
		{
			get { return xmlSchemaFile; }
			set { xmlSchemaFile = value; }
		}

		public bool IsXmlSchema
		{
			get { return isXmlSchema; }
			set { isXmlSchema = value; }
		}
		public bool MapStringsAsGuids
		{
			get { return mapStringsAsGuids; }
			set { mapStringsAsGuids = value; }
		}
		bool useClassField = true; // Opposite of Use OidTypes
		public bool UseClassField
		{
			get 
			{ 
				if (mapStringsAsGuids) return false;
				return useClassField; 
			}
			set { useClassField = value; }
		}

		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}

		public string OwnerName
		{
			get { return ownerName; }
			set { ownerName = value; }
		}

		public string ConnectionType
		{
			get { return connectionType; }
			set { connectionType = value; }
		}
		public string DefaultNamespace
		{
			get { return defaultNamespace; }
			set { defaultNamespace = value; }
		}

		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}
		public string ProjectDirectory
		{
			get { return projectDirectory; }
			set { projectDirectory = value; }
		}
		public TargetLanguage TargetLanguage
		{
			get { return targetLanguage; }
			set { targetLanguage = value; }
		}

        public void Save(XmlNode parentElement)
        {
            XmlDocument doc = parentElement.OwnerDocument;
            XmlElement myElement = doc.CreateElement("AssemblyWizModel");
            parentElement.AppendChild(myElement);
            XmlHelper.SetAttribute(myElement, "connectionString", this.connectionString, typeof(string));
            XmlHelper.SetAttribute(myElement, "connectionType", this.connectionType, typeof(string));
            XmlHelper.SetAttribute(myElement, "defaultNamespace", this.defaultNamespace, typeof(string));
            XmlHelper.SetAttribute(myElement, "isXmlSchema", this.isXmlSchema, typeof(bool));
            XmlHelper.SetAttribute(myElement, "mapStringsAsGuids", this.mapStringsAsGuids, typeof(bool));
            XmlHelper.SetAttribute(myElement, "ownerName", this.ownerName, typeof(string));
            XmlHelper.SetAttribute(myElement, "projectDirectory", this.projectDirectory, typeof(string));
            XmlHelper.SetAttribute(myElement, "projectName", this.projectName, typeof(string));
            XmlHelper.SetAttribute(myElement, "targetLanguage", this.targetLanguage, typeof(TargetLanguage));
            XmlHelper.SetAttribute(myElement, "connectionString", this.useClassField, typeof(bool));
            XmlHelper.SetAttribute(myElement, "xmlSchemaFile", this.xmlSchemaFile, typeof(string));
        }

        public AssemblyWizModel(XmlNode parentNode)
        {
            XmlNode myNode = parentNode.SelectSingleNode("AssemblyWizModel");
            this.connectionString = (string)XmlHelper.GetAttribute(myNode, "connectionString", typeof(string));
            this.connectionType = (string)XmlHelper.GetAttribute(myNode, "connectionType", typeof(string));
            this.defaultNamespace = (string)XmlHelper.GetAttribute(myNode, "defaultNamespace", typeof(string));
            this.isXmlSchema = (bool)XmlHelper.GetAttribute(myNode, "isXmlSchema", typeof(bool));
            this.mapStringsAsGuids = (bool)XmlHelper.GetAttribute(myNode, "mapStringsAsGuids", typeof(bool));
            this.ownerName = (string)XmlHelper.GetAttribute(myNode, "ownerName", typeof(string));
            this.projectDirectory = (string)XmlHelper.GetAttribute(myNode, "projectDirectory", typeof(string));
            this.projectName = (string)XmlHelper.GetAttribute(myNode, "projectName", typeof(string));
            this.targetLanguage = (TargetLanguage)XmlHelper.GetAttribute(myNode, "targetLanguage", typeof(TargetLanguage));
            this.useClassField = (bool)XmlHelper.GetAttribute(myNode, "useClassField", typeof(bool));
            this.xmlSchemaFile = (string)XmlHelper.GetAttribute(myNode, "xmlSchemaFile", typeof(string));
        }


		public AssemblyWizModel()
		{
		}
	}
}
