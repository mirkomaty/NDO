using NDO.ProviderFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NDOEnhancer
{
	class ProviderPathFinder : IProviderPathFinder
	{
        ProjectDescription projectDescription;

        public ProviderPathFinder(ProjectDescription projectDescription)
		{
            this.projectDescription = projectDescription;
        }

        public IEnumerable<string> GetPaths()
		{
            List<string>paths = new List<string>();

            var nugetFolder = new NugetProps(this.projectDescription).DefaultNugetPackageFolder;
            if (nugetFolder == null)
                return paths;

            // The NDO package itself is no provider. ProjectAssets handles the
            // directory separators and the lower case package folders of NuGet.
            var assetPaths = new ProjectAssets(this.projectDescription).GetPackageDirectories(@"ndo\..+", "ndo.dll")
                .Select(p=>Path.GetDirectoryName(Path.Combine(nugetFolder, p)));

			foreach (var assetPath in assetPaths)
			{
                paths.Add( assetPath );
            }

            if (projectDescription.ConfigurationOptions.VerboseMode)
            {
                Console.WriteLine( "Provider probing paths: " );
                foreach (var path in paths)
                {
                    Console.WriteLine( "  " + path );
                }
                if (paths.Count == 0)
                    Console.WriteLine( "  None found" );
            }

            return paths;
		}


	}
}
