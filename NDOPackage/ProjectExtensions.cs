using System;
using System.IO;
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

        public static string DefaultMappingFileName( this Project project )
        {
            return Path.Combine( Path.GetDirectoryName(project.FullPath), "NDOMapping.xml" );
        }


        public static string MappingFilePath( this Project project )
		{
            string defaultFileName = project.DefaultMappingFileName();
            if (File.Exists( defaultFileName ))
                return defaultFileName;
            ThreadHelper.ThrowIfNotOnUIThread();
            var assemblyName = project.DteProject().Properties.Item( "AssemblyName" ).Value;
            if (assemblyName == null)
                return null;
            string mappingFile = Path.Combine( project.FullPath, assemblyName + "ndo.xml" );
            if (File.Exists( mappingFile ))
                return mappingFile;
            return null;
        }


        public static string DefaultMappingFileName( this EnvDTE.Project project )
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Path.Combine( Path.GetDirectoryName( project.FullName ), "NDOMapping.xml" );
        }


        public static string MappingFilePath( this EnvDTE.Project project )
        {
            string defaultFileName = project.DefaultMappingFileName();
            if (File.Exists( defaultFileName ))
                return defaultFileName;
            ThreadHelper.ThrowIfNotOnUIThread();
            var assemblyName = project.Properties.Item( "AssemblyName" ).Value;
            if (assemblyName == null)
                return null;
            string mappingFile = Path.Combine( project.FullName, assemblyName + "ndo.xml" );
            if (File.Exists( mappingFile ))
                return mappingFile;
            return null;
        }


        public static bool MappingFileExists(this Project project )
        {
            return project.MappingFilePath() != null;
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
