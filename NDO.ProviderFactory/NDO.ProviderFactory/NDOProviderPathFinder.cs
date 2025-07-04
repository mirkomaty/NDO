﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NDO.ProviderFactory
{
	class NDOProviderPathFinder : IProviderPathFinder
	{
		public NDOProviderPathFinder() 
		{
		}

		readonly string[] netVersions = { "net8.0", "net6.0", "netstandard2.0", "netstandard2.1" };

		public IEnumerable<string> GetPaths()
		{
			List<string>paths = new List<string>();
			var path = AppDomain.CurrentDomain.BaseDirectory;
			paths.Add( path );
			
			string binPath = Path.Combine( path, "bin" );
			if (Directory.Exists( binPath ))
				paths.Add(binPath);

			// This is a hack to determine any loaded provider packages.
			var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

			if (runtimeDir != null && runtimeDir.IndexOf( "Microsoft.NETCore.App" ) > -1)
			{
				path = typeof( NDOProviderFactory ).Assembly.Location;
				var pattern = $".nuget{Path.DirectorySeparatorChar}packages";
				int p;
				if (( p = path.IndexOf( pattern ) ) > -1)
				{
					p += pattern.Length;
					path = path.Substring( 0, p );
					var majorVersion = GetType().Assembly.GetName().Version.Major.ToString();

					foreach (var subPath in Directory.GetDirectories( path, "ndo.*" ))
					{
						if (Path.GetFileName( subPath ).Equals( "ndo.dll", StringComparison.OrdinalIgnoreCase ))
							continue;
						if (Path.GetFileName( subPath ).Equals( "ndo.jsonformatter", StringComparison.OrdinalIgnoreCase ) )
							continue;
						//TODO We should sort the versions here and take the last one of the same major version
						var versionDir = Directory.GetDirectories( subPath ).FirstOrDefault( s => Path.GetFileName( s ).StartsWith( majorVersion) );
						if (versionDir == null)
							continue;

						foreach (var v in netVersions)
						{
							var libDir = Path.Combine( versionDir, "lib", v );
							if (!Directory.Exists( libDir ))
								continue;
							paths.Add( libDir );
						}
					}
				}
			}

			return paths;
		}
	}
}
