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


using System;
using System.IO;
using System.Xml;

namespace NDOEnhancer
{
    /// <summary>
    /// Attention!!!!! This code is essentially the same as in ConfigurationOptions.cs 
    /// in the Add-in project.
    /// So, if you change one of them, be aware to change the second.
    /// </summary>
    internal class ConfigurationOptions
	{
		ProjectDescription projectDescription;

		public ConfigurationOptions( ProjectDescription projectDescription, XmlDocument doc )
		{
			this.projectDescription = projectDescription;
			string pns = XmlHelper.Pns(doc);
			XmlNode node = doc.SelectSingleNode("//" + pns + "Enhancer/" + pns + "Options", XmlHelper.Nsmgr);
			if (node == null)
				throw new Exception("Parameters must have at least one //Enhancer/Options entry.");

			this.NewMapping = (bool) XmlHelper.GetNode(node, pns + "NewMapping", false);
			this.GenerateSQL = (bool) XmlHelper.GetNode(node, pns + "GenerateSQL", true);
			this.SQLScriptLanguage = (string) XmlHelper.GetNode(node, pns + "SQLScriptLanguage", "SqlServer");
			this.SchemaVersion = (string)XmlHelper.GetNode(node, pns + "SchemaVersion", "");
			this.UseTimeStamps = (bool) XmlHelper.GetNode(node, pns + "UseTimeStamps", false);
			this.DatabaseOwner = (string) XmlHelper.GetNode(node, pns + "DatabaseOwner", string.Empty);
			this.DefaultConnection = (string) XmlHelper.GetNode(node, pns + "DefaultConnection", string.Empty);
			this.targetMappingFileName = (string) XmlHelper.GetNode( node, pns + "TargetMappingFileName", "NDOMapping.xml" );
			this.EnableAddIn = (bool) XmlHelper.GetNode( node, pns + "EnableAddIn", true );
			this.EnableEnhancer = (bool) XmlHelper.GetNode(node, pns + "EnableEnhancer", true);
            this.IncludeTypecodes = (bool)XmlHelper.GetNode(node, pns + "IncludeTypecodes", false);
            this.GenerateChangeEvents = (bool)XmlHelper.GetNode(node, pns + "GenerateChangeEvents", false);
			this.Utf8Encoding = (bool) XmlHelper.GetNode(node, pns + "Utf8Encoding", true);
            this.VerboseMode = (bool)XmlHelper.GetNode(node, pns + "VerboseMode", false);
			this.GenerateConstraints = (bool)XmlHelper.GetNode(node, pns + "GenerateConstraints", false);
			this.DropExistingElements = (bool)XmlHelper.GetNode(node, pns + "DropExistingElements", true);
		}

		public bool EnableAddIn { get; set; }
		public bool IncludeTypecodes { get; set; }
		public bool VerboseMode { get; set; }
		public bool UseTimeStamps { get; set; }
		public bool GenerateChangeEvents { get; set; }
		public bool EnableEnhancer { get; set; }
		public bool Utf8Encoding { get; set; }
		public bool GenerateSQL { get; set; }
		public bool GenerateConstraints { get; set; }
		public bool DropExistingElements { get; set; }
		public bool NewMapping { get; set; }

		string targetMappingFileName;
		public string TargetMappingFileName 
		{
			get 
			{
				if (string.IsNullOrEmpty( this.targetMappingFileName ))
				{
					var fileName = this.projectDescription.FileName;
					return Path.GetFileNameWithoutExtension(fileName) + ".ndo.mapping";
				}
				return this.targetMappingFileName;
			} 
		}

		public string DefaultConnection { get; set; }
		public string SchemaVersion { get; set; }
		public string SQLScriptLanguage { get; set; }
		public string DatabaseOwner { get; set; }
	}
}
