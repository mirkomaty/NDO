//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using SD = System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using VSLangProj;
using EnvDTE;
using VsWebSite;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace NETDataObjects.NDOVSPackage
{

	/// <summary>
	/// 
	/// </summary>
	internal class NDOReference
	{
		public string Name;
		public string Path;
		public bool CheckThisDLL;

		public NDOReference( string name, string path, bool checkThisDLL )
		{
			this.Name = name;
			this.Path = path;
			this.CheckThisDLL = checkThisDLL;
		}

		public override bool Equals( object obj )
		{
			NDOReference ndoRef = obj as NDOReference;
			if ( obj == null )
				return false;

			return ndoRef.Name == this.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
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
		string platformTarget;
		string targetFramework;

#if DEBUG
        IMessageAdapter messageAdapter;
        public IMessageAdapter MessageAdapter
        {
            get { return messageAdapter; }
            set { messageAdapter = value; }
        }
#endif

		public string TargetFramework
		{
			get { return this.targetFramework; }
		}

		public string PlatformTarget
		{
			get { return this.platformTarget; }
		}

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

		public ConfigurationOptions NewConfigurationOptions()
		{
#if !STANDALONE
			return new ConfigurationOptions(project);
#else
			return new ConfigurationOptions();
#endif
		}

		public ProjectDescription()
		{
		}

		public ProjectDescription(string fileName)
		{
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
			Load(doc);
			options = new ConfigurationOptions(doc);
		}

		string AbsolutePath(string path)
		{
			if (Path.IsPathRooted(path))
				return path;
			else
				return Path.Combine(projPath, path);
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
			foreach ( XmlNode rnode in refList )
			{
				string assName = (string) XmlHelper.GetAttribute(rnode, "AssemblyName");
				string assPath = AbsolutePath((string) XmlHelper.GetAttribute(rnode, "AssemblyPath"));
				bool processDLL = true;
				XmlAttribute attr = rnode.Attributes["CheckThisDLL"];
				if ( attr != null && string.Compare( attr.Value, "True", true ) != 0 )
					processDLL = false;

				AddReference( assName, assPath, processDLL );
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
			foreach ( string key in References.Keys )
			{
				NDOReference ndoreference = References[key];
				if ( ndoreference.Path == binFile )
					continue;
				XmlElement refNode = (XmlElement) MakeNode( "Reference", "", refsNode, doc );
				refNode.SetAttribute( "AssemblyName", ndoreference.Name );
				refNode.SetAttribute( "AssemblyPath", ExtendedPath.GetRelativePath( reference, ndoreference.Path ) );
				if ( !ndoreference.CheckThisDLL )
				{
					refNode.SetAttribute( "CheckThisDLL", "False" );
				}
			}
		}

		string GetBuildProperty(IVsBuildPropertyStorage propertyStorage, string key, string configuration = "")
		{
			string result = null;
			if (propertyStorage != null)
				propertyStorage.GetPropertyValue( key, configuration, (uint)_PersistStorageType.PST_PROJECT_FILE, out result );
			return result;
		}

		public ProjectDescription(Solution solution, Project project)
		{
			this.solution = solution;
			this.project = project;

			Configuration conf = project.ConfigurationManager.ActiveConfiguration;
			IVsBuildPropertyStorage propertyStorage = GetPropertyStorage( project );

			try
			{
				this.platformTarget = (string)conf.Properties.Item( "PlatformTarget" ).Value;
			}
			catch { }
			try
			{
				this.targetFramework = (string)project.Properties.Item( "TargetFrameworkMoniker" ).Value;
			}
			catch { }

			// Web Projects don't have an assembly name.
			// Since NDO computes some information from the 
			// binFile, we produce a dummy binFile name.
			if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
			{
				this.isWebProject = true;
				binFile = project.Name;
				projPath = binFile;
				if (binFile.EndsWith( "\\" ))
					binFile = binFile.Substring( 0, binFile.Length - 1 );
				int p = binFile.LastIndexOf( "\\" );
				string fileName = binFile;
				if (p > -1)
				{
					fileName = binFile.Substring( p + 1 );
				}
				objPath = Path.Combine( binFile, "bin" );
				// Dummy file name
				binFile = Path.Combine( objPath, fileName + ".dll" );
			}
			else
			{
				string outputPath = (string)conf.Properties.Item( "OutputPath" ).Value;
				string fullPath = project.Properties.Item( "FullPath" ).Value as string;
				string outputFileName = GetBuildProperty( propertyStorage, "TargetFileName" );
				//foreach (Property item in project.Properties)
				//{
				//	SD.Debug.WriteLine( $"{item.Name} = {item.Value}" );
				//}
				if (String.IsNullOrEmpty( outputFileName ))
				{
					int outputType = (int)project.Properties.Item( "OutputType" ).Value;
					// .NET Core Executables are dlls.
					if (this.targetFramework.StartsWith( ".NETCoreApp" ))
						outputType = 2;
					outputFileName = (string)project.Properties.Item( "AssemblyName" ).Value + (outputType == 2 ? ".dll" : ".exe");
				}
				string configuration = GetBuildProperty( propertyStorage, "Configuration" );
				debug = configuration == "Debug";
				objPath = GetBuildProperty( propertyStorage, "IntermediateOutputPath" );
				binFile = Path.Combine( fullPath, outputPath );
				binFile = Path.Combine( binFile, outputFileName );
				projPath = Path.GetDirectoryName( project.FileName ) + "\\";
				string sign = GetBuildProperty( propertyStorage, "SignAssembly" );
				if (!String.IsNullOrEmpty( sign ) && String.Compare( sign, "true", true ) == 0)
					keyFile = GetBuildProperty( propertyStorage, "AssemblyOriginatorKeyFile" );
				else
					keyFile = null;
			}
		}

		private static IVsBuildPropertyStorage GetPropertyStorage( Project project )
		{
			IVsHierarchy projectHierarchy = null;

			//if (NDOPackage.SolutionService.GetProjectOfUniqueName( project.UniqueName, out projectHierarchy ) != 0)  // 0 == S_OK
			//{
			//	projectHierarchy = null;
			//}

			((IVsSolution)Package.GetGlobalService( typeof( IVsSolution ) )).GetProjectOfUniqueName( project.UniqueName, out projectHierarchy );

			IVsBuildPropertyStorage propertyStorage = null;

			if (projectHierarchy != null)
			{
				propertyStorage = (IVsBuildPropertyStorage)projectHierarchy;
			}

			return propertyStorage;
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


		private void BuildReferences()
		{
			if (references != null)
				return;
			references = new Dictionary<string,NDOReference>();
			Dictionary<string,string> allProjects = new Dictionary<string, string>();
			foreach(Project p in new ProjectIterator(solution).Projects)
			{
				if (p.Name == project.Name) continue;
				ConfigurationManager cman = p.ConfigurationManager;
				if (cman == null)
					continue;
				Configuration		 conf = cman.ActiveConfiguration;
                if (conf.Properties == null)
                    continue;
                try // Skip the project, if a property is not present
                {
                    string outputPath = (string)conf.Properties.Item("OutputPath").Value;
                    string fullPath = (string)p.Properties.Item("FullPath").Value;
					string outputFileName = GetBuildProperty( GetPropertyStorage(p), "TargetFileName" );
					//messages.Output(fullPath + outputPath + outputFileName);
					if (!allProjects.ContainsKey(p.Name))
                        allProjects.Add(p.Name, fullPath + outputPath + outputFileName);
                }
                catch { }
			}
            if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
            {

                VSWebSite ws = project.Object as VSWebSite;
                if (ws != null)
                {
                    foreach (AssemblyReference ar in ws.References)
                    {
                        Project referencedProject = ar.ReferencedProject;
                        if (referencedProject != null)
                        {
                            string rname = referencedProject.Name;
                            if (allProjects.ContainsKey(rname))
                                AddReference(ar.Name, (string)allProjects[rname], true);
                            else
                            {
                                // Referenzen, die auf keine gültige DLL verweisen...
                                if (ar.FullPath != "") 
                                    AddReference(rname, ar.FullPath, true);
                            }
                        }
                        else
                        {
                            if (ar.FullPath != "")
                                AddReference(ar.Name, ar.FullPath, true);
                        }
                    }
                }

            }

            else if (project.Object is VSProject)
            {
                foreach (VSLangProj.Reference r in ((VSProject)project.Object).References)
                {
                    string rname = "";
                    if (r.SourceProject != null)
                        rname = r.SourceProject.Name;
                    else
                        rname = r.Name;
                    if (rname == project.Name) continue;

                    if (allProjects.ContainsKey(rname))
                        AddReference(r.Name, (string)allProjects[rname], true);
                    else
                    {
                        // Referenzen, die auf keine gültige DLL verweisen...
                        if (!String.IsNullOrEmpty(r.Path) && NDOAssemblyChecker.IsEnhanced(r.Path))
                            AddReference(rname, r.Path, false);
                    }
                }
            }
//			AddReference(project.Name, this.binFile);

		}


        ProjectItems GetItemCollection(string fileName)
        {
            string relPath = ExtendedPath.GetRelativePath(this.projPath, fileName);
            ProjectItems result = project.ProjectItems;
            if (relPath.IndexOf(":\\") > -1)
                return result;
            string[] splittedName = relPath.Split(new char[] { '\\' });
            for (int i = 0; i < splittedName.Length - 1; i++)
            {
                string name = splittedName[i];
                ProjectItem pi = result.Item(name);
                if (pi != null)
                    result = pi.ProjectItems;
                else
                    break;
            }
            return result;
        }


		public void AddFileToProject(string fileName)
		{
            //TODO: Make the search hierarchical
			if (project == null)
				return;
			if (!File.Exists(fileName))
				return;
			bool found = false;

            ProjectItems itemCollection = GetItemCollection(fileName);

			foreach (ProjectItem pi in itemCollection)
			{
				if (string.Compare(Path.GetFileName(pi.Name), Path.GetFileName(fileName), true) == 0)
				{
					found = true;
					break;
				}
			}
			if (!found)
            {
#if DEBUG
                messageAdapter.WriteLine("  Adding file to project: " + fileName);
#endif
				this.project.ProjectItems.AddFromFile(fileName);
            }
		}


		public Dictionary<string, NDOReference> References
		{
			get
			{
#if !STANDALONE
					BuildReferences();
#else
					if (references == null)
						references = new Dictionary<string,NDOReference>();
#endif
					return references;
			}
		}

	}
}
