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

            return paths;
		}


	}
}
