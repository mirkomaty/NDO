//
// Copyright (c) 2002-2019 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using dte=EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell.ServiceBroker;
using Microsoft.VisualStudio;
using NuGet.VisualStudio.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NDOVsPackage
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.NDOPackageString)]
    public sealed class NDOPackage : ToolkitPackage
    {
        BuildEventHandler buildEventHandler;
        public static NDOPackage Instance { get; set; }
		static Regex regex = new Regex(@"(\d)\.(\d)", RegexOptions.Compiled);
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = (dte._DTE) await this.GetServiceAsync(typeof(dte._DTE));
            ApplicationObject.VisualStudioApplication = dte;
            this.buildEventHandler = new BuildEventHandler();
            Instance = this;
        }


		public async Task<NuGetInstalledPackage> GetNdoPackageAsync( Project project )
		{
			var brokeredServiceContainer = await NDOPackage.Instance.GetServiceAsync<SVsBrokeredServiceContainer, IBrokeredServiceContainer>();
			if (brokeredServiceContainer != null)
			{
				var serviceBroker = brokeredServiceContainer.GetFullAccessServiceBroker();
				using (var disposable = await serviceBroker.GetProxyAsync<INuGetProjectService>( NuGetServices.NuGetProjectServiceV1 ) as IDisposable)
				{
					var nugetService = disposable as INuGetProjectService;
					var hier = project.GetVsHierarchy();
					hier.GetGuidProperty( VSConstants.VSITEMID_ROOT,
												(int) __VSHPROPID.VSHPROPID_ProjectIDGuid,
												out Guid projGuid );
					var packagesResult = await nugetService.GetInstalledPackagesAsync( projGuid, CancellationToken.None );
					return packagesResult.Packages.FirstOrDefault( p => String.Compare( "ndo.dll", p.Id, true ) == 0 );
				}
			}

			return null;
		}

		public async Task<string> GetNdoVersionAsync( Project project )
		{
			var ndoPackage = await GetNdoPackageAsync(project);
			if (ndoPackage != null)
			{
				var match = regex.Match(ndoPackage.Version);
				var major = int.Parse(match.Groups[1].Value);
				if (major >= 5)
					return "5.0";
			}

			return "4.0";
		}

	}
}