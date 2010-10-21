//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Text;
using EnvDTE;
using VSLangProj;

namespace NDOEnhancer
{
    internal class ProjectIterator
    {
        Hashtable dictionary = new Hashtable();
        string solutionDir;

//#if !NDO11
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

        void FindProjects(Project project)
        {
            string kindUpper = project.Kind.ToUpper();
            if (kindUpper == VSLangProj.PrjKind.prjKindCSharpProject.ToUpper()
                || kindUpper == VSLangProj.PrjKind.prjKindVBProject.ToUpper()
                || kindUpper == VSLangProj2.PrjKind2.prjKindVJSharpProject.ToUpper()
                || kindUpper == "{E24C65DC-7377-472B-9ABA-BC803B73C61A}")
            dictionary.Add(ProjectName(project).ToLower(CultureInfo.InvariantCulture), project);
            FindProjects(project.ProjectItems);
        }



        string ProjectName(Project project)
        {
#if NDO11
            return project.Name;
#else
            string fileName = project.FileName;
            if (fileName.EndsWith("\\"))
                fileName = fileName.Substring(0, fileName.Length - 1);
            if (project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")
                return fileName;
            fileName = ExtendedPath.GetRelativePath(this.solutionDir, fileName);
            return fileName;
            //int p = fileName.LastIndexOf(Path.DirectorySeparatorChar);
            //if (p > -1)
            //    fileName = fileName.Substring(p + 1);
            //return fileName;
#endif
        }
//#endif

        public ProjectIterator(Solution solution)
        {
            this.solutionDir = Path.GetDirectoryName(solution.FileName);
            FindProjects(solution.Projects);
        }

        public Project this[string name]
        {
            get
            {
                return dictionary[name.ToLower()] as Project;
            }
        }

        public Project[] Projects
        {
            get
            {
                Project[] result = new Project[dictionary.Count];
                int i = 0;
                foreach (DictionaryEntry de in this.dictionary)
                    result[i++] = (Project)de.Value;
                return result;
            }
        }
    }
}
