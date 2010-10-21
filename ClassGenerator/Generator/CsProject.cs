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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using ClassGenerator;
using VisualStudioProject.DomainClasses;


namespace Generator
{
    internal class VsProject
    {
		//XmlDocument doc;
        //XmlElement filesElement;
		string fileName;
		string projName;
		string sourcePath;
		string defaultNamespace;
        string dialect;
		Project project;
		ItemGroup xmlItemGroup = null;		// xml files in the document
		ItemGroup compileItemGroup = null;	// .cs or .vb files
		bool hasConflicts = false;
		public bool HasConflicts
		{
			get { return hasConflicts; }
		}

		public string DefaultNamespace
		{
			get { return defaultNamespace; }
		}

		public string SourcePath
		{
			get { return sourcePath; }
		}


		public VsProject(TargetLanguage targetLanguage, string fileName, string projName, string sourcePath, string defaultNamespace, string dialect)
		{
			this.fileName = fileName;
			this.projName = projName;
			this.sourcePath = sourcePath;
			this.defaultNamespace = defaultNamespace;

			XmlDocument doc;
			doc = new XmlDocument();
			if (targetLanguage == TargetLanguage.VB)
				doc.LoadXml(VsProject.vbtemplate);
			else
				doc.LoadXml(VsProject.cstemplate);
            this.dialect = dialect;

			string existingGuid = null;
            string toolsVersion = null;
			existingGuid = AnalyzeExistingProjectFile( fileName, out toolsVersion );

			this.project = new Project(doc.DocumentElement);
			PropertyGroup mainPropertyGroup = this.project.MainPropertyGroup;
			if (existingGuid != null)
				mainPropertyGroup["ProjectGuid"] = existingGuid;
			else
				mainPropertyGroup["ProjectGuid"] = "{" + Guid.NewGuid().ToString() + "}";

			mainPropertyGroup["AssemblyName"] = projName;
			mainPropertyGroup["RootNamespace"] = defaultNamespace;

			foreach ( ItemGroup ig in project.ItemGroups )
			{
				foreach (Reference reference in ig.References)
				{
					if ( reference.Include == "NDO" )  // This is a marker
					{
						reference.Include = typeof( NDO.PersistenceManager ).Assembly.FullName;
						reference.HintPath = NDO.NDOApplicationPath.Instance + @"\NDO.dll";
						break;
					}
				}
			}

			this.compileItemGroup = project.NewItemGroup();			

#if UseXml
			XmlNode pnode = doc.SelectSingleNode("/Project");
            ((XmlElement)pnode).SetAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
            pnode = doc.SelectSingleNode("/Project/PropertyGroup/ProjectGuid");
			if (existingGuid != null)
				pnode.InnerText = existingGuid;
			else
				pnode.InnerText = "{" + Guid.NewGuid().ToString() + "}";
            pnode = doc.SelectSingleNode("/Project/PropertyGroup/AssemblyName");
            pnode.InnerText = projName;
            pnode = doc.SelectSingleNode("/Project/PropertyGroup/RootNamespace");
            pnode.InnerText = defaultNamespace;
            pnode = doc.SelectSingleNode("/Project/ItemGroup/Reference[@Include='NDO']");
            ((XmlElement)pnode).SetAttribute("Include", typeof(NDO.PersistenceManager).Assembly.FullName);
            pnode = pnode.SelectSingleNode("HintPath");
            pnode.InnerText = NDO.NDOApplicationPath.Instance + @"\NDO.dll";

            pnode = doc.SelectSingleNode("/Project/ItemGroup/Compile[@Include='###Marker###']");
            this.filesElement = (XmlElement)pnode.ParentNode;
            filesElement.InnerXml = string.Empty;
#endif
		}

		private string AnalyzeExistingProjectFile( string fileName, out string toolsVersion )
		{
			string existingGuid = null;
            toolsVersion = null;
			if ( File.Exists( fileName ) )
			{
				XmlDocument existingDoc = new XmlDocument();
				StreamReader sr = new StreamReader( fileName, Encoding.UTF8 );
				string s = sr.ReadToEnd();
				sr.Close();

				// Remove namespaces
				Regex regex = new Regex( @"xmlns\s*=\s*"".+""" );
				s = regex.Replace( s, string.Empty );

				existingDoc.LoadXml( s );
				//XmlNode guidNode = existingDoc.SelectSingleNode("/Project/PropertyGroup/ProjectGuid");
				//if (guidNode != null)
				//    existingGuid = guidNode.InnerText;

				Project existingProject = new Project( existingDoc.DocumentElement );
                toolsVersion = existingProject.ToolsVersion;
				PropertyGroup mainPropertyGroup = existingProject.MainPropertyGroup;
				if ( mainPropertyGroup != null )
					existingGuid = mainPropertyGroup["ProjectGuid"];
			}
			return existingGuid;
		}

		public void Save()
		{
			XmlDocument doc = new XmlDocument();
			this.project.Save(doc);

			Merge.CommentPrefix = "<!--";
			MergeableFile mergeableFile = new MergeableFile(fileName);
			try
			{
				doc.Save( mergeableFile.Stream );
			}
			catch ( Exception ex )
			{
				mergeableFile.Restore();
				mergeableFile.Stream.Close();
				throw ex;
			}
			this.hasConflicts = mergeableFile.Write();
			mergeableFile.Stream.Close();
		}

		public void AddXmlFile(string name)
		{
#if UseXml
            XmlElement el = (XmlElement)doc.SelectSingleNode(@"//Project");
            XmlElement itemGroup = doc.CreateElement("ItemGroup");
            el.AppendChild(itemGroup);
            XmlElement contentElement = doc.CreateElement("Content");
            itemGroup.AppendChild(contentElement);
            contentElement.SetAttribute("Include", name);
#else
			if (this.xmlItemGroup == null)
				this.xmlItemGroup = this.project.NewItemGroup();
			Content content = this.xmlItemGroup.NewContent();
			content.Include = name;
#endif
		}

		public void AddCsFile(string name)
		{

#if UseXml
            XmlElement compile = doc.CreateElement("Compile");
            filesElement.AppendChild(compile);
            compile.SetAttribute("Include", name);
#else
			Compile compile = this.compileItemGroup.NewCompile();
			compile.Include = name;
#endif
		}



        //xmlns=""http://schemas.microsoft.com/developer/msbuild/2003""
        internal const string cstemplate = @"<Project DefaultTargets=""Build"" >
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProductVersion>8.0.50727</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{7BCB435C-5DB4-45BA-B6FD-5EAA9CC8D068}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>BusinessClasses</RootNamespace>
    <AssemblyName>BusinessClasses</AssemblyName>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""NDO"">
      <SpecificVersion>False</SpecificVersion>
      <Private>false</Private>
      <HintPath></HintPath>
    </Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Xml"" />
  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.CSharp.targets"" />
</Project>";


		internal const string vbtemplate =@"<Project DefaultTargets=""Build"" >
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProductVersion>8.0.50727</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{DFF06F70-DBD2-46A0-8F19-4FDB194B36F9}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>ClassLibrary1</RootNamespace>
    <AssemblyName>ClassLibrary1</AssemblyName>
    <MyType>Windows</MyType>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <DefineDebug>true</DefineDebug>
    <DefineTrace>true</DefineTrace>
    <OutputPath>bin\Debug\</OutputPath>
    <DocumentationFile>ClassLibrary1.xml</DocumentationFile>
    <NoWarn>42016,41999,42017,42018,42019,42032,42036,42020,42021,42022</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <DefineDebug>false</DefineDebug>
    <DefineTrace>true</DefineTrace>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DocumentationFile>ClassLibrary1.xml</DocumentationFile>
    <NoWarn>42016,41999,42017,42018,42019,42032,42036,42020,42021,42022</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Xml"" />
    <Reference Include=""NDO"">
      <SpecificVersion>False</SpecificVersion>
      <Private>false</Private>
      <HintPath></HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Import Include=""Microsoft.VisualBasic"" />
    <Import Include=""System"" />
    <Import Include=""System.Collections"" />
    <Import Include=""System.Collections.Generic"" />
    <Import Include=""System.Data"" />
    <Import Include=""System.Diagnostics"" />
    <Import Include=""NDO"" />
  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.VisualBasic.targets"" />
</Project>
	";
    }
}