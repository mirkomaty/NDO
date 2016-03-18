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
