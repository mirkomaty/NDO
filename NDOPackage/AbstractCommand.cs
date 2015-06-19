using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Extensibility;
using EnvDTE80;

namespace NDOEnhancer
{
    internal abstract class AbstractCommand : IDTExtensibility2, IDTCommandTarget
    {
        private _DTE visualStudioApplication;
        private string commandBarButtonText;
        private string commandBarButtonToolTip;
        private AddIn addInInstance;

        public AddIn AddInInstance
        {
            get { return addInInstance; }
            set { addInInstance = value; }
        }

        public string MyCommandName
        {
            get 
            { 
                return "NDOEnhancer.Connect." + this.commandBarButtonText.Replace(" ", string.Empty); 
            }
        }

        public _DTE VisualStudioApplication
        {
            get { return this.visualStudioApplication; }
            set { this.visualStudioApplication = value; }
        }
        public string CommandBarButtonText
        {
            get { return this.commandBarButtonText; }
            set { this.commandBarButtonText = value; }
        }
        public string CommandBarButtonToolTip
        {
            get { return this.commandBarButtonToolTip; }
            set { this.commandBarButtonToolTip = value; }
        }

        protected virtual bool CommandExists
        {
            get
            {
                foreach (Command cmd in this.visualStudioApplication.Commands)
                {
                    if (null != cmd.Name && cmd.Name.Equals(this.MyCommandName))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual Command AddNamedCommand( int buttonNr )
        {
            object[] contextGUIDS = new object[] { };
            Commands2 commands = (Commands2)this.visualStudioApplication.Commands;
            //string version = this.visualStudioApplication.Version;
            int flags = (int)vsCommandStatus.vsCommandStatusSupported |
                (int)vsCommandStatus.vsCommandStatusEnabled;
            Command result = commands.AddNamedCommand2(this.addInInstance,
                    this.commandBarButtonText.Replace(" ", string.Empty), this.commandBarButtonText, this.commandBarButtonToolTip,
                    false, buttonNr, ref contextGUIDS, flags, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton );
            return result;
        }

        #region IDTExtensibility2 Members

        public virtual void OnAddInsUpdate(ref Array custom)
        {
        }

        public virtual void OnBeginShutdown(ref Array custom)
        {
        }

        public virtual void OnStartupComplete(ref Array custom)
        {
        }

        public abstract void OnConnection(object Application, Extensibility.ext_ConnectMode ConnectMode, object AddInInst, ref Array custom);

        public virtual void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
#if nix
			Commands commands = applicationObject.Commands;

			foreach(Command cmd in commands)
			{
				if ( null != cmd.Name && cmd.Name.Equals( myCommandName ) )
				{
					try
					{
						cmd.Delete();
					}
					catch {}
					break;
				}
			}
#endif
        }

        #endregion

        #region IDTCommandTarget Members

        public abstract void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled);

        public virtual void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (commandName == MyCommandName)
            {
                Array projects = visualStudioApplication.ActiveSolutionProjects as Array;
                if (1 == projects.Length)
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                else
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported;
            }
        }

        #endregion
    }
}
