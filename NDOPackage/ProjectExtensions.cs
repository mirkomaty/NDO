//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
            ThreadHelper.ThrowIfNotOnUIThread();
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
