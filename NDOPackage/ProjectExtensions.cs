using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell.Interop;

namespace NDOVsPackage
{
    internal static class ProjectExtensions
    {
        public static EnvDTE.Project DteProject( this Project project )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var projects = ApplicationObject.VisualStudioApplication.Solution.Projects;
            EnvDTE.Project dteProj = null;
            foreach (EnvDTE.Project proj in projects)
            {
                if (proj.Name == project.Name)
                {
                    dteProj = proj;
                    break;
                }
            }

            return dteProj;
        }

        public static IVsHierarchy GetVsHierarchy( this EnvDTE.Project project )
        {
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsHierarchy projectHierarchy = null;

            ( (IVsSolution) Package.GetGlobalService( typeof( IVsSolution ) ) ).GetProjectOfUniqueName( project.UniqueName, out projectHierarchy );
            return projectHierarchy;
        }


        public static IVsHierarchy GetVsHierarchy( this Project project )
        {
            IVsHierarchy projectHierarchy;
            project.GetItemInfo( out projectHierarchy, out _, out _ );

            return projectHierarchy;
        }
    }
}
