using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyJSON;

namespace NDOEnhancer
{
	class ProjectAssets
	{
		private readonly ProjectDescription projectDescription;
		public ProjectAssets( ProjectDescription projectDescription )
		{
			this.projectDescription = projectDescription;
		}

		JSON.Object LoadAssets()
		{
			var json = new JSON();
			var objPath = Path.Combine(this.projectDescription.ProjPath, "obj");
			var assetsPath = Path.Combine( objPath, "project.assets.json" );
			if (!File.Exists( assetsPath ))
			{
				Console.WriteLine( $"Can't find assets file: {assetsPath}" );
				return null;
			}
			using (TextReader tr = new StreamReader( assetsPath ))
				return json.Parse( tr ).Value.AsObject();
		}

		/// <summary>
		/// Computes the path of a compile asset relative to the NuGet package folder.
		/// </summary>
		/// <remarks>
		/// The keys in the targets section (e.g. "NDO.Mapping/6.0.0") contain the original casing
		/// of the package id, but NuGet stores the packages in lower case folders
		/// (e.g. "ndo.mapping/6.0.0"). On case sensitive file systems (Linux) we have to use
		/// the folder name provided in the libraries section.
		/// </remarks>
		static string GetRelativeCompilePath( JSON.Object root, string packageKey, string compile )
		{
			string packagePath = null;
			JSON.Object libraries = null;
			JSON.Object library = null;
			if (root.Get( "libraries", ref libraries ) && libraries.Get( packageKey, ref library ))
				library.Get( "path", ref packagePath );
			if (string.IsNullOrEmpty( packagePath ))
				packagePath = packageKey.ToLowerInvariant();

			var relPath = packagePath.TrimEnd( '/' ) + "/" + compile;
			if (Path.DirectorySeparatorChar != '/')
				relPath = relPath.Replace( '/', Path.DirectorySeparatorChar );
			return relPath;
		}

		public string GetPackageDir( string packageName )
		{
			var root = LoadAssets();
			if (root == null)
				return null;

			var targets = root["targets"].AsObject();
			var standardKey = targets.FirstOrDefault(o => o.StartsWith( ".NETStandard,Version=" ) || o.StartsWith( ".NETCoreApp,Version=" ));
			if (standardKey == null)
				standardKey = targets.First();
			var standard = targets[standardKey].AsObject();
			var packageKey = standard.FirstOrDefault(o=>o.StartsWith(packageName + '/', StringComparison.OrdinalIgnoreCase));
			if (packageKey == null)
				return null;
			var package = standard[packageKey].AsObject();
			var compile = package["compile"].AsObject().First();
			return GetRelativeCompilePath( root, packageKey, compile );
		}

		/// <summary>
		/// Gets the relative paths of the compile assets of all packages with ids matching the pattern.
		/// </summary>
		/// <param name="pattern">Regular expression for the package id. Package ids are case insensitive.</param>
		/// <param name="excludedPackageIds">Package ids which should not be part of the result.</param>
		public IEnumerable<string> GetPackageDirectories( string pattern, params string[] excludedPackageIds )
		{
			var root = LoadAssets();
			if (root == null)
				return Enumerable.Empty<string>();

			var targets = root["targets"].AsObject();
			var standardKey = targets.FirstOrDefault( o => o.StartsWith( ".NETStandard,Version=" ) || o.StartsWith( ".NETCoreApp,Version=" ) || o.StartsWith( "net" ) );
			if (standardKey == null)
			{
				Console.WriteLine("Can't find any .NETStandard or .NETCoreApp target in the project.assets.json file");
				return Enumerable.Empty<string>();
			}
			var standard = targets[standardKey].AsObject();
			Regex regex = new Regex( "^" + pattern + '/', RegexOptions.IgnoreCase );
			var packageKeys = standard.Where(o=>regex.Match(o).Success)
				.Where(o=>!excludedPackageIds.Any(id=>o.StartsWith(id + '/', StringComparison.OrdinalIgnoreCase)));
			var paths = packageKeys.Select(packageKey=>
			{
				var package = standard[packageKey].AsObject();
				var compile = package["compile"].AsObject().First();
				return GetRelativeCompilePath( root, packageKey, compile );
			} ).ToList();
			return paths;
		}
	}
}
