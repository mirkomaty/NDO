using NDO.Provider;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NDOEnhancer
{
	class ProviderPathFinder : IProviderPathFinder
	{
        ProjectDescription projectDescription;

        public ProviderPathFinder()
		{
            this.projectDescription = (ProjectDescription) AppDomain.CurrentDomain.GetData( "ProjectDescription" );
        }

        public IEnumerable<string> GetPaths()
		{
            List<string>paths = new List<string>();
            if (!this.projectDescription.IsSdkStyle)
            {
                paths.Add( AppDomain.CurrentDomain.BaseDirectory );
                paths.Add( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) );
            }
            else
            {
                var nugetFolder = NugetProps.DefaultNugetPackageFolder;
                var assetPaths = ProjectAssets.GetPackageDirectories(@"ndo\..+")
                    .Where(p=>p.IndexOf("/ndo.dll/", StringComparison.OrdinalIgnoreCase) == -1).
                    Select(p=>Path.GetDirectoryName(Path.Combine(nugetFolder, p)));
				foreach (var assetPath in assetPaths)
				{
                    paths.Add( assetPath );
                }
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
