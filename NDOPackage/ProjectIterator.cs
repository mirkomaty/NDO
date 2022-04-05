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
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Project = EnvDTE.Project;
using System.Linq;

namespace NDOVsPackage
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Usage", "VSTHRD010:Invoke single-threaded types on Main thread", Justification = "<Ausstehend>" )]
    internal class ProjectIterator
    {
        Dictionary<string, Project> dictionary = new Dictionary<string, Project>();
        string solutionDir;


        public ProjectIterator( EnvDTE.Solution solution )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.solutionDir = Path.GetDirectoryName( solution.FileName );
            FindProjects( solution.Projects );
        }

        void FindProjects(ProjectItems projectItems)
        {
            if (projectItems == null)
                return;
            foreach (ProjectItem pi in projectItems)
            {
                if (pi.SubProject != null)
                    FindProjects(pi.SubProject);
                else
                    FindProjects(pi.ProjectItems);
            }
        }

        void FindProjects(Projects projects)
        {
            foreach (Project pro in projects)
            {
                FindProjects(pro);
            }
        }

		void FindProjects( Project project )
        {
            //string kindUpper = project.Kind.ToUpper();

            if (CodeGenHelper.IsCsOrVbProject( project ))
                dictionary.Add( ProjectName( project ).ToLower( CultureInfo.InvariantCulture ), project );

            FindProjects( project.ProjectItems );
        }



        string ProjectName(EnvDTE.Project project)
        {
            string fileName = project.FileName.TrimEnd( Path.DirectorySeparatorChar );

            fileName = ExtendedPath.GetRelativePath(this.solutionDir, fileName);
            return fileName;
        }


        public EnvDTE.Project this[string name]
        {
            get
            {
                return dictionary[name.ToLower()] as EnvDTE.Project;
            }
        }

        public Project[] Projects
        {
            get
            {
                return this.dictionary.Values.ToArray();
            }
        }
    }
}
