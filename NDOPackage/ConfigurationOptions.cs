//
// Copyright (c) 2002-2024 Mirko Matytschak 
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


using Microsoft.VisualStudio.Threading;
using System;
using System.IO;
using System.Xml;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace NDOVsPackage
{
    /// <summary>
    /// Attention!!!!! This code is essentially the same as in ConfigurationOptions.cs 
    /// in the project NDOEnhancer.
    /// So, if you change one of them, be aware to change the second.
    /// </summary>
    internal class ConfigurationOptions
    {
        string fileName = null;

        //void Anlegen() { }

		// This is called to check the options in the Add-in
        public ConfigurationOptions(Project project) : this( GetNdoProjFileName( project.FullPath ) )
        {
        }

        public ConfigurationOptions(EnvDTE.Project project) : this (GetNdoProjFileName(project.FullName))
		{
		}

        private ConfigurationOptions( string fileName )
        {
            this.fileName = fileName; 
            this.TargetMappingFileName = "NDOMapping.xml"; // Set the default name. Can be overridden by the configuration.
            this.Utf8Encoding = true;
            if (File.Exists( this.fileName ))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( fileName );
                Init( doc );
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
            ThreadHelper.JoinableTaskFactory.Run( async () => await SaveAsync( pd ) );
			this.fileName = oldFileName;
		}

        public async Task SaveAsync(ProjectDescription projectDescription)
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
				MakeNode("TargetMappingFileName", this.TargetMappingFileName, optionsNode);
                MakeNode("GenerateChangeEvents", this.GenerateChangeEvents, optionsNode);
                MakeNode("UseTimeStamps", this.UseTimeStamps, optionsNode);
                MakeNode("Utf8Encoding", this.Utf8Encoding, optionsNode);
                MakeNode("SQLScriptLanguage", this.SQLScriptLanguage, optionsNode);
				MakeNode("SchemaVersion", this.SchemaVersion, optionsNode);
                MakeNode("IncludeTypecodes", this.IncludeTypecodes, optionsNode);
                MakeNode("DatabaseOwner", this.DatabaseOwner, optionsNode);
				MakeNode("GenerateConstraints", this.GenerateConstraints, optionsNode);
				MakeNode("DropExistingElements", this.DropExistingElements, optionsNode);

                await projectDescription.ToXmlAsync(docNode);
                doc.Save(fileName);
            }
            else
                throw new Exception("ConfigurationOptions.Save: file name is null");
        }

        public static string GetNdoProjFileName(string fullPath)
        {
            string result;
            if (Directory.Exists(fullPath)) // Web Projects have a directory as name
            {
                string s = fullPath;
                if (s.EndsWith("\\"))
                    s = s.Substring(0, s.Length - 1);
                int p = s.LastIndexOf(Path.DirectorySeparatorChar);
                if (p > -1)
                    s = s.Substring(p + 1);
                s += ".ndoproj";
                result = Path.Combine(fullPath, s);
            }
            else
                result = Path.ChangeExtension(fullPath, ".ndoproj");
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
			this.TargetMappingFileName = (string) XmlHelper.GetNode( node, pns + "TargetMappingFileName", "NDOMapping.xml" );
			this.EnableEnhancer = (bool) XmlHelper.GetNode( node, pns + "EnableEnhancer", true );
            this.IncludeTypecodes = (bool)XmlHelper.GetNode(node, pns + "IncludeTypecodes", false);
            this.VerboseMode = (bool)XmlHelper.GetNode(node, pns + "VerboseMode", false);
            this.GenerateChangeEvents = (bool)XmlHelper.GetNode(node, pns + "GenerateChangeEvents", false);
            this.Utf8Encoding = (bool)XmlHelper.GetNode(node, pns + "Utf8Encoding", true);
			this.DropExistingElements = (bool)XmlHelper.GetNode(node, pns + "DropExistingElements", true);
			this.GenerateConstraints = (bool)XmlHelper.GetNode(node, pns + "GenerateConstraints", false);
		}


		public bool EnableAddIn { get; set; }
		public bool IncludeTypecodes { get; set; }
		public bool UseTimeStamps { get; set; }
		public bool GenerateChangeEvents { get; set; }
		public bool EnableEnhancer { get; set; }
		public bool VerboseMode { get; set; }
		public bool Utf8Encoding { get; set; }
		public bool GenerateSQL { get; set; }
		public bool GenerateConstraints { get; set; }
		public bool DropExistingElements { get; set; }
		public bool NewMapping { get; set; }
		public string DefaultConnection { get; set; }
		public string TargetMappingFileName { get; set; }
		public string SQLScriptLanguage { get; set; }
		public string SchemaVersion { get; set; }
		public string DatabaseOwner { get; set; }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
