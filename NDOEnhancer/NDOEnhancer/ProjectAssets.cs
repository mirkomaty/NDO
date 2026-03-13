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

		public string GetPackageDir( string packageName )
		{
			var json = new JSON();
			JSON.Object root;
			var objPath = Path.Combine(this.projectDescription.ProjPath, "obj");
			var assetsPath = Path.Combine( objPath, "project.assets.json" );
			if (assetsPath == null)
			{
				Console.WriteLine( $"Can't find assets file: {assetsPath}" );
				return null;
			}
			using (TextReader tr = new StreamReader( assetsPath ))
				root = json.Parse( tr ).Value.AsObject();

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
			var relPath = packageKey + "/" + compile;
			if (Path.DirectorySeparatorChar != '/')
				relPath = relPath.Replace( '/', Path.DirectorySeparatorChar );
			//Regex regex = new Regex(@"netstandard\d\.\d");
			//relPath = regex.Replace( relPath, "net452" );
			return relPath;
		}

		public IEnumerable<string> GetPackageDirectories( string pattern )
		{
			var json = new JSON();
			JSON.Object root;
			var objPath = Path.Combine(projectDescription.ProjPath, "obj");
			var assetsPath = Path.Combine( objPath, "project.assets.json" );
			if (assetsPath == null)
			{
				Console.WriteLine( $"Can't find assets file: {assetsPath}" );
				return Enumerable.Empty<string>();
			}
			using (TextReader tr = new StreamReader( assetsPath ))
				root = json.Parse( tr ).Value.AsObject();

			var targets = root["targets"].AsObject();
			var standardKey = targets.FirstOrDefault( o => o.StartsWith( ".NETStandard,Version=" ) || o.StartsWith( ".NETCoreApp,Version=" ) || o.StartsWith( "net" ) );
			if (standardKey == null)
			{
				Console.WriteLine("Can't find any .NETStandard or .NETCoreApp target in the project.assets.json file");
				return Enumerable.Empty<string>();
			}
			var standard = targets[standardKey].AsObject();
			Regex regex = new Regex(pattern + '/');
			var packageKeys = standard.Where(o=>regex.Match(o).Success);
			var paths = packageKeys.Select(packageKey=>
			{
				var package = standard[packageKey].AsObject();
				var compile = package["compile"].AsObject().First();
				var relPath = packageKey + "/" + compile;
				if (Path.DirectorySeparatorChar != '/')
					relPath = relPath.Replace( '/', Path.DirectorySeparatorChar );
				//Regex regx = new Regex(@"netstandard\d\.\d");
				//relPath = regx.Replace( relPath, "net452" );
				return relPath;
			} );
			return paths;
		}
	}
}
