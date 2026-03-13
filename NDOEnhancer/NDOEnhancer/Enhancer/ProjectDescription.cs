
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
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace NDOEnhancer
{
	internal class NDOReference
	{
		public string Name;
		public string Path;
		public bool CheckThisDLL;

		public NDOReference(string name, string path, bool checkThisDLL)
		{
			this.Name = name;
			this.Path = path;
			this.CheckThisDLL = checkThisDLL;
		}

	}

	/// <summary>
	/// Zusammenfassung für ProjectDescription.
	/// </summary>
	internal class ProjectDescription
	{
		Dictionary<string, NDOReference> references = null;
		bool debug;
		string binFile;
		string objPath;
		string projPath;
		string assemblyName;
		ConfigurationOptions options;
        string keyFile = string.Empty;
        public string FileName { get; set; }

        public string KeyFile
        {
            get { return keyFile; }
            set { keyFile = value; }
        }

		public ConfigurationOptions ConfigurationOptions
		{
			get { return options; }
		}

		public string AssemblyName
		{
			get { return assemblyName; }
			set { assemblyName = value; }
		}

		public ProjectDescription(string fileName, string targetFramework)
		{
			this.FileName = fileName;
			this.projPath = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(fileName);
			}
			catch(XmlException ex)
			{
				throw new Exception("Parameter file '" + fileName + "' is not a valid Xml file. Line: " + ex.LineNumber.ToString() + " Position: " + ex.LinePosition.ToString());
			}
			XmlHelper.AddNamespace(doc);
			Load(doc, targetFramework);
			options = new ConfigurationOptions(this, doc);
		}

		string AbsolutePath(string path)
		{
			if (Path.IsPathRooted(path))
				return Path.GetFullPath(path);
			else
				return Path.GetFullPath(Path.Combine(projPath, path));
		}

		void Load(XmlDocument doc, string targetFramework)
		{
			string pns = XmlHelper.Pns(doc);

			XmlNode node = doc.SelectSingleNode("//" + pns + "Enhancer/" + pns + "ProjectDescription", XmlHelper.Nsmgr);
			if (node == null)
				throw new Exception("Parameters must have at least one //Enhancer/ProjectDescription entry.");
			
			this.binFile = AbsolutePath((string) XmlHelper.GetNode(node, pns + "BinPath"))
                .Replace( "$(TargetFramework)", targetFramework );
			this.objPath = AbsolutePath((string) XmlHelper.GetNode(node, pns + "ObjPath"))
                .Replace( "$(TargetFramework)", targetFramework );

            this.keyFile = (string)XmlHelper.GetNode(node, pns + "KeyFile", string.Empty);
            if (this.keyFile != string.Empty)
                this.keyFile = AbsolutePath(keyFile);

            this.assemblyName = (string)XmlHelper.GetNode(node, pns + "AssemblyName");
			this.debug = (bool) XmlHelper.GetNode(node, pns + "Debug", false);
            XmlNodeList refList = doc.SelectNodes("//" + pns + "Enhancer/" + pns + "ProjectDescription/" + pns + "References/" + pns + "Reference", XmlHelper.Nsmgr);
			this.references = new Dictionary<string, NDOReference>();
			foreach(XmlNode rnode in refList)
			{
				string assName = (string) XmlHelper.GetAttribute(rnode, "AssemblyName");
				string assPath = AbsolutePath((string) XmlHelper.GetAttribute(rnode, "AssemblyPath"));
				bool processDLL = true;
				XmlAttribute attr = rnode.Attributes["CheckThisDLL"];
				if ( attr != null && string.Compare( attr.Value, "True", true ) != 0 )
					processDLL = false;

				AddReference(assName, assPath, processDLL);
			}
		}

		public string ObjPath
		{
			get { return objPath; }
			set { objPath = value; }
		}

		public string BinFile
		{
			get { return binFile; }
			set { binFile = value; }
		}

		public bool Debug
		{
			get { return debug; }
			set { debug = value; }
		}

		public string ProjPath
		{
			get { return projPath; }
			set { projPath = value; }
		}

		private void AddReference( string name, string path, bool checkThisDLL )
		{
			if ( path.IndexOf( @"Microsoft.NET\Framework" ) > -1 )
				return;
			if ( name.ToUpper() == "NDO" )
				return;
			if ( name.ToUpper() == "NDOInterfaces" )
				return;

			if ( !references.ContainsKey( name ) )
				references.Add( name, new NDOReference( name, path, checkThisDLL ) );
		}

        public string DefaultMappingFileName
        {
            get
            {
                return Path.Combine(this.projPath, "NDOMapping.xml");
            }
        }

        public bool MappingFileExists
        {
            get
            {
                return CheckedMappingFileName != null;
            }
        }

        /// <summary>
        /// Returns the mapping file, if the file exists, or null otherwise
        /// </summary>
        public string CheckedMappingFileName
        {
            get
            {
                if (File.Exists(DefaultMappingFileName))
                    return DefaultMappingFileName;
                if (assemblyName == null)
                    return null;
                string mappingFile = Path.Combine(projPath, this.assemblyName + "ndo.xml");
                if (File.Exists(mappingFile))
                    return mappingFile;
                return null;
            }
        }

		public Dictionary<string, NDOReference> References
		{
			get
			{
				if (references == null)
					references = new Dictionary<string,NDOReference>();
				return references;
			}
		}

	}
}
