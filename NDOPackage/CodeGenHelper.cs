//
// Copyright (C) 2002-2015 Mirko Matytschak 
// (www.netdataobjects.de)
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

using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace NETDataObjects.NDOVSPackage
{

	internal class CodeGenHelper
	{

		public static bool IsCsProject(Project project)
		{
			return project.Kind == ProjectKind.prjKindCSharpProject;
		}

		public static bool IsVbProject(Project project)
		{
			return project.Kind == ProjectKind.prjKindVBProject;
		}


		public static ProjectItem FindProjectItemByName(Project prj, string strItemNameWithFileExtension)
		{
			if (prj.ProjectItems.Count < 1)
				throw new Exception(string.Format("Project Item {0} not found. No project items in project {1}", strItemNameWithFileExtension, prj.Name));
			if (strItemNameWithFileExtension == null)
				return prj.ProjectItems.Item(1);
			foreach (ProjectItem pri in prj.ProjectItems)
				if (pri.Name == strItemNameWithFileExtension)
					return pri;
			return null;
		}

		public static TextDocument ActivateAndGetTextDocument(Project prj, string strProjectItem)
		{
			ProjectItem pri = FindProjectItemByName(prj, strProjectItem);
			if (pri == null)
				return null;

			// we need to ensure that the item is open since we would not
			// be able to get a text document otherwise
			if (!pri.get_IsOpen(Constants.vsViewKindCode))
				pri.Open(Constants.vsViewKindCode);
			Document doc = pri.Document;
			doc.Activate();
			TextDocument txdoc = doc.DTE.ActiveDocument.Object("TextDocument") as TextDocument;
			return txdoc;
		}

	}
}