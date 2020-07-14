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


using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETDataObjects.NDOVSPackage
{
	internal abstract class AbstractCommand
	{
		protected OleMenuCommand command;
		protected _DTE dte;

		public AbstractCommand(_DTE dte, CommandID commandId)			
		{
			command = new OleMenuCommand( DoIt, commandId );
			command.BeforeQueryStatus += OnBeforeQueryStatus;
			this.dte = dte;
		}

		protected virtual void OnBeforeQueryStatus( object sender, EventArgs e )
		{
			OleMenuCommand item = (OleMenuCommand)sender;
            Array projects = dte.ActiveSolutionProjects as Array;
            item.Enabled = (1 == projects.Length);
		}

        public _DTE VisualStudioApplication
        {
            get { return this.dte; }
        }


		protected abstract void DoIt( object sender, EventArgs e );
	}
}
