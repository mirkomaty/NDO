using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NDOEnhancer
{
	class NugetProps
	{
		private readonly string assemblyPath;
		string projPath;
		public NugetProps(string assemblyPath)
		{
			this.projPath = Path.GetFullPath(Path.Combine(assemblyPath, @"..\..\..\.."));
			this.assemblyPath = assemblyPath;
		}

		public string DefaultNugetPackageFolder
		{
			get
			{
				var objPath = Path.Combine(this.projPath, "obj");
				var fnwe = Path.GetFileNameWithoutExtension(assemblyPath);
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
