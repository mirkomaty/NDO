using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NDOEnhancer
{
	class NugetProps
	{
		static ProjectDescription projectDescription;
		static NugetProps()
		{
			projectDescription = (ProjectDescription) AppDomain.CurrentDomain.GetData( "ProjectDescription" );
		}

		public static string DefaultNugetPackageFolder
		{
			get
			{
				var objPath = Path.Combine(projectDescription.ProjPath, "obj");
				var fnwe = Path.GetFileNameWithoutExtension(projectDescription.FileName);
				var propFileName = Path.Combine(objPath, $"{fnwe}.csproj.nuget.g.props");
				if (!File.Exists( propFileName ))
				{
					Console.WriteLine( $"Can't find file {propFileName}" );
					return null;
				}

				XElement propsElement = XElement.Load(propFileName);
				var ns = propsElement.GetDefaultNamespace();
				var nuGetPackageFolders = propsElement.Elements(ns + "PropertyGroup").Elements(ns + "NuGetPackageFolders").FirstOrDefault();

				if (nuGetPackageFolders == null)
				{
					Console.WriteLine( $"Can't find element 'PropertyGroup/NuGetPackageFolders' in file {propFileName}" );
					return null;
				}

				var path = nuGetPackageFolders.Value.Split(';').FirstOrDefault(p=>p.IndexOf(".nuget") > -1);

				return path;
			}
		}
	}
}
