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


using SD = System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using EnvDTE;
using Project = Community.VisualStudio.Toolkit.Project;
using Solution = Community.VisualStudio.Toolkit.Solution;

namespace NDOVsPackage
{

	/// <summary>
	/// ProjectDescription.
	/// </summary>
	[SD.CodeAnalysis.SuppressMessage( "Usage", "VSTHRD010:Invoke single-threaded types on Main thread", Justification = "This code always runs on the UI thread" )]
	internal class ProjectDescription
	{
		Solution solution = null;
		Project project = null;
		Dictionary<string, NDOReference> references = null;
		bool debug;
		string binFile;
		string objPath;
		string projPath;
		string assemblyName;
		ConfigurationOptions options;
        string keyFile = string.Empty;
		string platformTarget;
		string targetFramework;
		string version = "4.0";

#if DEBUG
        MessageAdapter messageAdapter;
        public MessageAdapter MessageAdapter
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
			return new ConfigurationOptions(project);
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

			var vattr = ((XmlElement)node).Attributes["version"];
			if (vattr != null)
				this.version = vattr.Value;

			binFile = AbsolutePath((string) XmlHelper.GetNode(node, pns + "BinPath"));
			objPath = AbsolutePath((string) XmlHelper.GetNode(node, pns + "ObjPath"));
            keyFile = (string)XmlHelper.GetNode(node, pns + "KeyFile", string.Empty);

            if (keyFile != string.Empty)
                keyFile = AbsolutePath(keyFile);
            
			assemblyName = (string)XmlHelper.GetNode(node, pns + "AssemblyName");
			debug = (bool) XmlHelper.GetNode(node, pns + "Debug", false);
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

		public async Task ToXmlAsync(XmlNode parent)
		{
			XmlDocument doc = (XmlDocument) parent.ParentNode;
			XmlNode node = doc.CreateElement("ProjectDescription");
			this.version = await NDOPackage.Instance.GetNdoVersionAsync( this.project );
            Version.TryParse( this.version, out var ndoprojVersion );

            if (ndoprojVersion.Major >= 5)
				( (XmlElement) node ).SetAttribute( "version", this.version );

			parent.AppendChild(node);
			string reference = this.projPath;
			if (reference.EndsWith("\\"))
				reference = reference.Substring(0, reference.Length - 1);

			MakeNode("BinPath", ExtendedPath.GetRelativePath(reference, binFile), node, doc);
			MakeNode("ObjPath", ExtendedPath.GetRelativePath(reference, objPath), node, doc);
			MakeNode("AssemblyName", assemblyName, node, doc);
			MakeNode("Debug", debug, node, doc);
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

		public ProjectDescription( Project project )
		{
			this.solution = (Solution) project.Parent;
			this.project = project;

			ThreadHelper.ThrowIfNotOnUIThread();
			var dteProj = project.DteProject();
			EnvDTE.Configuration conf = dteProj.ConfigurationManager.ActiveConfiguration;
			//var props = new Dictionary<string,object>();
			//foreach (Property item in conf.Properties)
			//{
			//	object value = "-";
			//	try
			//	{
			//		value = item.Value;
			//		props.Add( item.Name, value );
			//	}
			//	catch(Exception ex)
			//	{ }
			//}

			//foreach (Property item in dteProj.Properties)
			//{
			//	object value = "-";
			//	try
			//	{
			//		value = item.Value;
			//		props.Add( item.Name, value );
			//	}
			//	catch (Exception ex)
			//	{ }
			//}

			//foreach (var item in props)
			//{
			//	SD.Debug.WriteLine( $"{item.Key} = {item.Value}" );
			//}

			// Get the MSBuild property storage
			ThreadHelper.JoinableTaskFactory.Run( async () => this.version = await NDOPackage.Instance.GetNdoVersionAsync( this.project ) );
			Version.TryParse( this.version, out var ndoprojVersion );
			var friendlyTargetFramework = (string)dteProj.Properties.Item("FriendlyTargetFramework").Value;

			IVsBuildPropertyStorage propertyStorage = GetPropertyStorage( project );

			try
			{
				this.platformTarget = (string) conf.Properties.Item( "PlatformTarget" ).Value;
			}
			catch { }
			try
			{
				this.targetFramework = (string) dteProj.Properties.Item( "TargetFrameworkMoniker" ).Value;
			}
			catch { }

			string outputPath = (string) conf.Properties.Item( "OutputPath" ).Value;
			string fullPath = dteProj.Properties.Item( "FullPath" ).Value as string;
			string outputFileName = GetBuildProperty( propertyStorage, "TargetFileName" );

			if (String.IsNullOrEmpty( outputFileName ))
			{
				int outputType = (int) dteProj.Properties.Item( "OutputType" ).Value;
				// .NET Core Executables are dlls.
				if (this.targetFramework.StartsWith( ".NETCoreApp" ))
					outputType = 2;
				outputFileName = (string) dteProj.Properties.Item( "AssemblyName" ).Value + ( outputType == 2 ? ".dll" : ".exe" );
			}

			if (project.GetVsHierarchy().IsCapabilityMatch( "CPS" ))
			{
				// new .csproj format
				this.objPath = GetBuildProperty( propertyStorage, "IntermediateOutputPath" );
				string configuration = GetBuildProperty( propertyStorage, "Configuration" );
				debug = configuration == "Debug";
			}
			else
			{
				// old .csproj format
				string debugInfo = (string) conf.Properties.Item( "DebugInfo" ).Value;
				debug = debugInfo == "full";
				this.objPath = (string) conf.Properties.Item( "IntermediatePath" ).Value;
			}
			this.binFile = Path.Combine( fullPath, outputPath );
			this.binFile = Path.Combine( binFile, outputFileName );
			this.projPath = Path.GetDirectoryName( dteProj.FileName ) + "\\";

			if (ndoprojVersion.Major >= 5)
			{
				this.binFile = this.binFile.Replace( friendlyTargetFramework, "$(TargetFramework)" );
				this.objPath = this.objPath.Replace(friendlyTargetFramework, "$(TargetFramework)" );
			}

			string sign = GetBuildProperty( propertyStorage, "SignAssembly" );
			if (!String.IsNullOrEmpty( sign ) && String.Compare( sign, "true", true ) == 0)
				keyFile = GetBuildProperty( propertyStorage, "AssemblyOriginatorKeyFile" );
			else
				keyFile = null;

		}


		private static IVsBuildPropertyStorage GetPropertyStorage( Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsHierarchy projectHierarchy = project.GetVsHierarchy();

			return GetPropertyStorage( projectHierarchy );
		}

		private static IVsBuildPropertyStorage GetPropertyStorage( EnvDTE.Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsHierarchy projectHierarchy = project.GetVsHierarchy();

			return GetPropertyStorage( projectHierarchy );
		}

		private static IVsBuildPropertyStorage GetPropertyStorage( IVsHierarchy projectHierarchy )
		{
			IVsBuildPropertyStorage propertyStorage = null;

			if (projectHierarchy != null)
			{
				propertyStorage = (IVsBuildPropertyStorage) projectHierarchy;
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


		public void BuildReferences()
		{
			if (this.references != null)
				return;

			this.references = new Dictionary<string,NDOReference>();
			var allProjects = new Dictionary<string, string>();
			var solution = ApplicationObject.VisualStudioApplication.Solution;
			
			foreach(var p in new ProjectIterator(solution).Projects)
			{
				if (p.Name == project.Name) continue;
				EnvDTE.ConfigurationManager cman = p.ConfigurationManager;
				if (cman == null)
					continue;
				var		 conf = cman.ActiveConfiguration;
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
			

            foreach ( var r in this.project.References )
            {
                string rname = "";
                //if (r.SourceProject != null)
                //    rname = r.SourceProject.Name;
                //else
                rname = r.Name;
                if (rname == project.Name) continue;

				if (allProjects.ContainsKey( rname ))
				{
					AddReference( r.Name, (string) allProjects[rname], false );
				}
				else
				{
					var vsRef = r.VsReference;
					var path = vsRef.FullPath;
					// Referenzen, die auf keine gültige DLL verweisen...
					if (!String.IsNullOrEmpty( path ) && NDOAssemblyChecker.IsEnhanced( path ))
						AddReference( rname, path, false );
				}
			}
//			AddReference(project.Name, this.binFile);

		}

        ProjectItems GetItemCollection(string fileName)
        {
            string relPath = ExtendedPath.GetRelativePath(this.projPath, fileName);
			var dteProj = project.DteProject();
            ProjectItems result = dteProj.ProjectItems;
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

		public void FixDllState()
		{
			// The package works with a transient version of the ProjectDescription, which will be saved 
			// after a successful Build. But we need the CheckThisDLL state from the saved version for the UI.

			var fileName = ConfigurationOptions.GetNdoProjFileName( project.FullPath );

			if (!String.IsNullOrEmpty( fileName ) && File.Exists( fileName ))
			{
				ProjectDescription storedDescription = new ProjectDescription( fileName );
				var storedReferences = storedDescription.references.Values.ToArray();
				foreach (var reference in this.references.Values)
				{
					var storedReference = storedReferences.FirstOrDefault( r => r.Name == reference.Name );
					if (storedReference != null)
					{
						reference.CheckThisDLL = storedReference.CheckThisDLL;
					}
				}
			}
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
				this.project.DteProject().ProjectItems.AddFromFile(fileName);
            }
		}


		public Dictionary<string, NDOReference> References
		{
			get
			{
				BuildReferences();
				return references;
			}
		}

	}
}
