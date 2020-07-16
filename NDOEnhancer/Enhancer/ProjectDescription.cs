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


using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
#if !STANDALONE
using VSLangProj;
using EnvDTE;
#if !NDO11
using VSLangProj80;
using VSLangProj2;
using VsWebSite;
#endif
#endif

#if DEBUG
using NDOInterfaces; // for IMessageAdapter
#endif

namespace NDOEnhancer
{
	/// <summary>
	/// 
	/// </summary>
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
#if !STANDALONE
		Solution solution = null;
		Project project = null;
#endif
		Dictionary<string, NDOReference> references = null;
		bool debug;
		string binFile;
		string objPath;
		string projPath;
		string assemblyName;
		ConfigurationOptions options;
        bool isWebProject;
        string keyFile = string.Empty;
		bool isSdkStyle = false;
#if DEBUG
        IMessageAdapter messageAdapter;
        public IMessageAdapter MessageAdapter
        {
            get { return messageAdapter; }
            set { messageAdapter = value; }
        }
#endif

        public string KeyFile
        {
            get { return keyFile; }
            set { keyFile = value; }
        }

        public bool IsWebProject
        {
            get { return isWebProject; }
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
		public bool IsSdkStyle
		{
			get { return this.isSdkStyle; }
		}

		public ProjectDescription()
		{
		}

		/// <summary>
		/// The project filename should come from the Build Task, but we don't want to change the Build Task at the moment.
		/// </summary>
		/// <param name="ndoProjPath"></param>
		internal void GuessProjectFile(string ndoProjPath)
		{
			var dir = Path.GetDirectoryName(ndoProjPath);
			var fnPattern = $"{Path.GetFileNameWithoutExtension(ndoProjPath)}.*proj";
			var files = Directory.GetFiles( dir, fnPattern );
			string file = null;
			if (files.Length == 0)
				return;
			if (files.Length > -1)
			{
				file = files.FirstOrDefault( fn => fn.EndsWith( ".csproj" ) );
				if (file == null)
					file = files[0];
			}
			else
				file = files[0];

			XElement projectElement = XElement.Load(file);
			this.isSdkStyle = projectElement.Attribute( "Sdk" ) != null;
		}

		public ProjectDescription(string fileName)
		{
			this.FileName = fileName;
			this.projPath = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
			GuessProjectFile( fileName );
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
			Load(doc);
			options = new ConfigurationOptions(this, doc);
		}

		string AbsolutePath(string path)
		{
			if (Path.IsPathRooted(path))
				return Path.GetFullPath(path);
			else
				return Path.GetFullPath(Path.Combine(projPath, path));
		}

		void Load(XmlDocument doc)
		{
			string pns = XmlHelper.Pns(doc);

			XmlNode node = doc.SelectSingleNode("//" + pns + "Enhancer/" + pns + "ProjectDescription", XmlHelper.Nsmgr);
			if (node == null)
				throw new Exception("Parameters must have at least one //Enhancer/ProjectDescription entry.");
			
			binFile = AbsolutePath((string) XmlHelper.GetNode(node, pns + "BinPath"));
			objPath = AbsolutePath((string) XmlHelper.GetNode(node, pns + "ObjPath"));
            keyFile = (string)XmlHelper.GetNode(node, pns + "KeyFile", string.Empty);
            if (keyFile != string.Empty)
                keyFile = AbsolutePath(keyFile);
            assemblyName = (string)XmlHelper.GetNode(node, pns + "AssemblyName");
			debug = (bool) XmlHelper.GetNode(node, pns + "Debug", false);
            isWebProject = (bool) XmlHelper.GetNode(node, pns + "IsWebProject", false);
            XmlNodeList refList = doc.SelectNodes("//" + pns + "Enhancer/" + pns + "ProjectDescription/" + pns + "References/" + pns + "Reference", XmlHelper.Nsmgr);
			references = new Dictionary<string, NDOReference>();
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


		private XmlNode MakeNode(string name, object value, XmlNode parentNode, XmlDocument doc)
		{
			XmlElement el = doc.CreateElement(name);
			parentNode.AppendChild(el);
			if (value != null)
				el.InnerText = value.ToString();
			return el;
		}


		public void ToXml(XmlNode parent)
		{
			XmlDocument doc = (XmlDocument) parent.ParentNode;
			XmlNode node = doc.CreateElement("ProjectDescription");
			parent.AppendChild(node);
			string reference = this.projPath;
			if (reference.EndsWith("\\"))
				reference = reference.Substring(0, reference.Length - 1);

			MakeNode("BinPath", ExtendedPath.GetRelativePath(reference, binFile), node, doc);
			MakeNode("ObjPath", ExtendedPath.GetRelativePath(reference, objPath), node, doc);
			MakeNode("AssemblyName", assemblyName, node, doc);
			MakeNode("Debug", debug, node, doc);
            MakeNode("IsWebProject", isWebProject, node, doc);
            MakeNode("KeyFile", keyFile, node, doc);
			XmlNode refsNode = MakeNode("References", string.Empty, node, doc);
			foreach(string key in References.Keys)
			{
				NDOReference ndoreference = References[key];
				if ( ndoreference.Path == binFile )
					continue;
				XmlElement refNode = (XmlElement) MakeNode("Reference", "", refsNode, doc);
				refNode.SetAttribute( "AssemblyName", ndoreference.Name );
				refNode.SetAttribute( "AssemblyPath", ExtendedPath.GetRelativePath( reference, ndoreference.Path ) );
				if ( !ndoreference.CheckThisDLL )
				{
					refNode.SetAttribute( "CheckThisDLL", "False" );
				}
			}
		}


		//		public string SolutionPath
		//		{
		//			get { return solutionPath; }
		//			set { solutionPath = value; }
		//		}

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

        //public bool IsMappingFile(string pathToCompare)
        //{
        //    string dir = Path.GetDirectoryName(this.binFile);
        //    return (string.Compare(Path.Combine(dir, this.assemblyName + "ndo.xml"), pathToCompare, true) == 0);
        //}

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

		public string FileName { get; set; }


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
