using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NDOEnhancer
{
	class NugetProps
	{
		ProjectDescription projectDescription;
		public NugetProps( ProjectDescription projectDescription )
		{
			this.projectDescription = projectDescription;
		}

		public string DefaultNugetPackageFolder
		{
			get
			{
				var objPath = Path.Combine(this.projectDescription.ProjPath, "obj");
				var projName = Path.GetFileNameWithoutExtension(projectDescription.FileName);
				var propFileName = Path.Combine(objPath, $"{projName}.csproj.nuget.g.props");
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
