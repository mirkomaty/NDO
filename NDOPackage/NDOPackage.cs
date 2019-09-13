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


using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using EnvDTE;
using System.Threading;
using ST=System.Threading.Tasks;

namespace NETDataObjects.NDOVSPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true )]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", NDOPackage.Version, IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidNDOPackagePkgString)]
	[ProvideAutoLoad( VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad )]
	[ProvideLoadKey("Standard", NDOPackage.Version, ".NET Data Objects (NDO)", "Mirko Matytschak", 104) ]
    public sealed class NDOPackage : AsyncPackage
    {
		public const string Version = "4.0.0";
		private BuildEventHandler		buildEventHandler;
		public static IVsSolution SolutionService { get; private set; }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NDOPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
		protected override async ST.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			Debug.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString() ) );

			await base.InitializeAsync( cancellationToken, progress );

			// Let the remainder of the InitializeAsync method run on the main thread,
			// because it uses a lot of UI and Com stuff.

			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();			

			_DTE dte = (_DTE)this.GetService( typeof( _DTE ) );
			var serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte;
			SolutionService = (IVsSolution)GetService(typeof( IVsSolution ) );

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService( typeof( IMenuCommandService ) ) as OleMenuCommandService;
			if (null != mcs)
			{
				OleMenuCommand menuItem;
				// Create the command for the menu items.
				CommandID menuCommandID = new CommandID( GuidList.guidNDOPackageCmdSet, (int)PkgCmdIDList.cmdidNDOConfiguration );
				var cmd = mcs.FindCommand( menuCommandID );
				if (cmd == null)
				{
					menuItem = new Configure( dte, menuCommandID );
					mcs.AddCommand( menuItem );
				}

				menuCommandID = new CommandID( GuidList.guidNDOPackageCmdSet, (int)PkgCmdIDList.cmdidAddRelation );
				menuItem = new AddRelation( dte, menuCommandID );
				mcs.AddCommand( menuItem );

				menuCommandID = new CommandID( GuidList.guidNDOPackageCmdSet, (int)PkgCmdIDList.cmdidAddAccessor );
				menuItem = new AddAccessor( dte, menuCommandID );
				mcs.AddCommand( menuItem );

				menuCommandID = new CommandID( GuidList.guidNDOPackageCmdSet, (int)PkgCmdIDList.cmdidAddClass );
				menuItem = new AddPersistentClass( dte, menuCommandID );
				mcs.AddCommand( menuItem );

				menuCommandID = new CommandID( GuidList.guidNDOPackageCmdSet, (int)PkgCmdIDList.cmdidOpenMappingTool );
				menuItem = new OpenMappingTool( dte, menuCommandID );
				mcs.AddCommand( menuItem );
			}

			this.buildEventHandler = new BuildEventHandler( dte );

			//await ST.Task.CompletedTask;// Run( () => { } );
		}
		#endregion

	}
}
