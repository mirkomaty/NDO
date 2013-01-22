//
// Copyright (C) 2002-2013 Mirko Matytschak 
// (www.netdataobjects.de)
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
using System.IO;
using System.Collections;
using System.Xml;
using EnvDTE;

namespace NDOEnhancer
{
    /// <summary>
    /// Attention!!!!! This code is essentially the same as in TestConfigurationOptions.cs 
    /// in the project Enhancer.exe.
    /// Because VSS makes Trouble with one file being in two VSS stores, we decided
    /// to keep two versions.
    /// So, if you change one of them, be aware to change the second.
    /// </summary>
    internal class ConfigurationOptions
    {
        private Hashtable m_globals = null;
        string fileName = null;

        //void Anlegen() { }

		// This is called to check the options in the Add-in
        public ConfigurationOptions(Project project)
        {
            m_globals = new Hashtable();
            this.fileName = GetNdoProjFileName(project);
            if (File.Exists(this.fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                Init(doc);
            }
        }

        private void MakeNode(string name, object value, XmlNode parentNode)
        {
            XmlElement el = parentNode.OwnerDocument.CreateElement(name);
            parentNode.AppendChild(el);
            if (value != null)
                el.InnerText = value.ToString();
        }

		public void SaveAs(string fileName, ProjectDescription pd)
		{
			string oldFileName = this.fileName;  // just in case...
			this.fileName = fileName;
			Save(pd);
			this.fileName = oldFileName;
		}

        public void Save(ProjectDescription projectDescription)
        {
            if (fileName != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement docNode = doc.CreateElement("Enhancer");
                doc.AppendChild(docNode);
                XmlElement optionsNode = doc.CreateElement("Options");
                docNode.AppendChild(optionsNode);

                MakeNode("EnableAddIn", this.EnableAddIn, optionsNode);
                MakeNode("EnableEnhancer", this.EnableEnhancer, optionsNode);
                MakeNode("VerboseMode", this.VerboseMode, optionsNode);
                MakeNode("NewMapping", this.NewMapping, optionsNode);
                MakeNode("GenerateSQL", this.GenerateSQL, optionsNode);
                MakeNode("DefaultConnection", this.DefaultConnection, optionsNode);
                MakeNode("GenerateChangeEvents", this.GenerateChangeEvents, optionsNode);
                MakeNode("UseTimeStamps", this.UseTimeStamps, optionsNode);
                MakeNode("Utf8Encoding", this.Utf8Encoding, optionsNode);
                MakeNode("SQLScriptLanguage", this.SQLScriptLanguage, optionsNode);
				MakeNode("SchemaVersion", this.SchemaVersion, optionsNode);
                MakeNode("IncludeTypecodes", this.IncludeTypecodes, optionsNode);
                MakeNode("DatabaseOwner", this.DatabaseOwner, optionsNode);
				MakeNode("GenerateConstraints", this.GenerateConstraints, optionsNode);
				MakeNode("UseMsBuild", this.UseMsBuild, optionsNode);
				MakeNode("DropExistingElements", this.DropExistingElements, optionsNode);

                projectDescription.ToXml(docNode);
                doc.Save(fileName);
            }
            else
                throw new Exception("ConfigurationOptions.Save: file name is null");
        }

        public string GetNdoProjFileName(Project project)
        {
            string result;
            if (Directory.Exists(project.FullName)) // Web Projects have a directory as name
            {
                string s = project.FullName;
                if (s.EndsWith("\\"))
                    s = s.Substring(0, s.Length - 1);
                int p = s.LastIndexOf(Path.DirectorySeparatorChar);
                if (p > -1)
                    s = s.Substring(p + 1);
                s += ".ndoproj";
                result = Path.Combine(project.FullName, s);
            }
            else
                result = Path.ChangeExtension(project.FullName, ".ndoproj");
            return result;
        }

        public string FileName
        {
            get { return fileName; }
        }


        public ConfigurationOptions(XmlDocument doc)
        {
            Init(doc);
        }

        private void Init(XmlDocument doc)
        {
            m_globals = new Hashtable();

            string pns = XmlHelper.Pns(doc);
            XmlNode node = doc.SelectSingleNode("//" + pns + "Enhancer/" + pns + "Options", XmlHelper.Nsmgr);
            if (node == null)
                throw new Exception("NDO Project file must have an //Enhancer/Options element.");

            this.NewMapping = (bool)XmlHelper.GetNode(node, pns + "NewMapping", false);
            this.GenerateSQL = (bool)XmlHelper.GetNode(node, pns + "GenerateSQL", true);
            this.SQLScriptLanguage = (string)XmlHelper.GetNode(node, pns + "SQLScriptLanguage", "SqlServer");
			this.SchemaVersion = (string)XmlHelper.GetNode(node, pns + "SchemaVersion", "");
            this.UseTimeStamps = (bool)XmlHelper.GetNode(node, pns + "UseTimeStamps", false);
            this.DatabaseOwner = (string)XmlHelper.GetNode(node, pns + "DatabaseOwner", string.Empty);
            this.DefaultConnection = (string)XmlHelper.GetNode(node, pns + "DefaultConnection", string.Empty);
            this.EnableAddIn = (bool)XmlHelper.GetNode(node, pns + "EnableAddIn", true);
            this.EnableEnhancer = (bool)XmlHelper.GetNode(node, pns + "EnableEnhancer", true);
            this.IncludeTypecodes = (bool)XmlHelper.GetNode(node, pns + "IncludeTypecodes", false);
            this.VerboseMode = (bool)XmlHelper.GetNode(node, pns + "VerboseMode", false);
            this.GenerateChangeEvents = (bool)XmlHelper.GetNode(node, pns + "GenerateChangeEvents", false);
            this.Utf8Encoding = (bool)XmlHelper.GetNode(node, pns + "Utf8Encoding", true);
			this.DropExistingElements = (bool)XmlHelper.GetNode(node, pns + "DropExistingElements", true);
			this.GenerateConstraints = (bool)XmlHelper.GetNode(node, pns + "GenerateConstraints", false);
			this.UseMsBuild = (bool) XmlHelper.GetNode(node, pns + "UseMsBuild", false);
		}


        private void SetBoolValue(string fieldName, bool val)
        {
            if (m_globals.Contains(fieldName))
                m_globals.Remove(fieldName);

            m_globals[fieldName] = val;
        }

        private bool GetBoolValue(string fieldName)
        {
            if (!m_globals.Contains(fieldName))
                return false;
            return (bool)m_globals[fieldName];
        }


        private void SetStringValue(string fieldName, string val)
        {
            if (null == m_globals) return;

            if (m_globals.Contains(fieldName))
                m_globals.Remove(fieldName);

            m_globals[fieldName] = val;
        }

        private String GetStringValue(string fieldName)
        {
            if (!m_globals.Contains(fieldName))
                return string.Empty;
            return (string)m_globals[fieldName];
        }


        public bool EnableAddIn
        {
            get
            {
                return GetBoolValue("NDOEnableAddIn");
            }
            set
            {
                SetBoolValue("NDOEnableAddIn", value);
            }
        }

        public bool IncludeTypecodes
        {
            get
            {
                return GetBoolValue("IncludeTypecodes");
            }
            set
            {
                SetBoolValue("IncludeTypecodes", value);
            }
        }

        
        public bool UseTimeStamps
        {
            get
            {
                return GetBoolValue("NDOUseTimeStamps");
            }
            set
            {
                SetBoolValue("NDOUseTimeStamps", value);
            }
        }

        public bool GenerateChangeEvents
        {
            get
            {
                return GetBoolValue("NDOGenerateChangeEvents");
            }
            set
            {
                SetBoolValue("NDOGenerateChangeEvents", value);
            }
        }

        public bool EnableEnhancer
        {
            get
            {
                return GetBoolValue("NDOEnableEnhancer");
            }
            set
            {
                SetBoolValue("NDOEnableEnhancer", value);
            }
        }

        public bool VerboseMode
        {
            get
            {
                return GetBoolValue("NDOVerboseMode");
            }
            set
            {
                SetBoolValue("NDOVerboseMode", value);
            }
        }


        public bool Utf8Encoding
        {
            get { return GetBoolValue("NDOUtf8Encoding"); }
            set { SetBoolValue("NDOUtf8Encoding", value); }
        }

        public bool GenerateSQL
        {
            get
            {
                return GetBoolValue("NDOGenerateSQL");
            }
            set
            {
                SetBoolValue("NDOGenerateSQL", value);
            }
        }


        public bool GenerateConstraints
        {
            get
            {
                return GetBoolValue("NDOGenerateConstraints");
            }
            set
            {
                SetBoolValue("NDOGenerateConstraints", value);
            }
        }

        public bool DropExistingElements
        {
            get
            {
                return GetBoolValue("NDODropExistingElements");
            }
            set
            {
                SetBoolValue("NDODropExistingElements", value);
            }
        }

		public bool UseMsBuild
		{
			get
			{
				return GetBoolValue( "UseMsBuild" );
			}
			set
			{
				SetBoolValue( "UseMsBuild", value );
			}
		}


        public bool NewMapping
        {
            get
            {
                return GetBoolValue("NDONewMapping");
            }
            set
            {
                SetBoolValue("NDONewMapping", value);
            }
        }

        public string DefaultConnection
        {
            get
            {
                return GetStringValue("NDODefaultConnection");
            }
            set
            {
                SetStringValue("NDODefaultConnection", value);
            }
        }

        public string SQLScriptLanguage
        {
            get
            {
                string s = GetStringValue("NDOSQLScriptLanguage");
                if (s == string.Empty)
                    return "SqlServer";
                return s;
            }
            set
            {
                SetStringValue("NDOSQLScriptLanguage", value);
            }
        }

		public string SchemaVersion
		{
			get
			{
				return GetStringValue("SchemaVersion");
			}
			set
			{
				SetStringValue("SchemaVersion", value);
			}
		}


        public string DatabaseOwner
        {
            get
            {
                return GetStringValue("DatabaseOwner");
            }
            set
            {
                SetStringValue("DatabaseOwner", value);
            }
        }

    }
}

