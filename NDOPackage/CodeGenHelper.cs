﻿//
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
			return project.Kind == ProjectKind.prjKindCSharpProject || project.Kind == ProjectKind.prjKindNewCSharpProject;  // prjKindNewCSharpProject can be removed after VS 17.8
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
