//
// Copyright (C) 2002-2009 Mirko Matytschak 
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
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Resources;
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif
using Extensibility;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif

namespace NDOAddIn
{
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	//[GuidAttribute("D861E693-1993-4C4E-B9A7-5657D7F4F33A"), ProgId("NDOAddIn.Connect")]
	public class Connect : Extensibility.IDTExtensibility2, IDTCommandTarget
	{
		private _DTE					applicationObject;
		private BuildEvents				buildEvents;
		private BuildEventHandler		buildEventHandler;
		private DocumentEventHandler	documentEventHandler;
#if NET11
		private static ArrayList allCommands;
#else
		private ArrayList allCommands;
#endif

		/// <summary>
		///		Leerer Konstruktor ist nötig für dynamisches Anlegen
		/// </summary>
		public Connect()
		{
            if (allCommands == null)
            {
                allCommands = new ArrayList();
                // Die Reihenfolge ist wichtig, wegen der Positionen im Menü
                allCommands.Add(new Configure());
                allCommands.Add(new MergeConflictUseYourCode());
                allCommands.Add(new MergeConflictUseCGCode());
                allCommands.Add(new AddAccessor());
                allCommands.Add(new AddPersistentClass());
                allCommands.Add(new AddRelation());
                allCommands.Add(new OpenMappingTool());
#if PRO
                allCommands.Add(new OpenClassGenerator());
#endif
            }
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInstance, ref System.Array custom)
		{
            applicationObject = (_DTE)application;
            ApplicationObject.VisualStudioApplication = applicationObject;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

#if needsCmdBarsFile
        int i = 0;
        try
        {
            i = 1;
            StreamWriter sw = new StreamWriter("c:\\cmdbars.txt");
            object o = applicationObject.CommandBars;
            if (o == null)
                throw new Exception("CommandBars is null");
            foreach (CommandBar cb in (CommandBars) applicationObject.CommandBars)
            {
                i = 2;
                if (cb != null)
                {
                    i = 3;
                    if (cb.Name != null)
                    {
                        i = 4;
                        sw.WriteLine(cb.Name);
                    }
                }
            }
            i = 5;
            sw.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(i.ToString() + " " + ex.Message);
        }
#endif
            NDOCommandBar.CreateInstance(applicationObject);

            System.Array arr = null;

            foreach (IDTExtensibility2 cmd in allCommands)
                cmd.OnConnection(application, connectMode, addInInstance, ref custom);

            if (connectMode == Extensibility.ext_ConnectMode.ext_cm_AfterStartup)
                OnStartupComplete(ref arr);  // Makes sure the commands and command bars exist

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
		}

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.IndexOf("NDOEnhancer.dll") > -1)
                return this.GetType().Assembly;
            //args.Name	"NDOEnhancerLight.resources, Version=2.0.0.0, Culture=en-US, PublicKeyToken=null"	string
            //args.Name	"C:\\\\Program Files\\\\NDO 2.0 Enterprise Edition\\\\en\\\\NDOEnhancerLight.resources.dll"	string

            if (args.Name.StartsWith("NDOEnhancerLight.resources,") || args.Name.IndexOf("NDOEnhancerLight.resources.dll") > -1)
            {
                Assembly ass = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "en\\NDOEnhancerLight.resources.dll"));
                return ass;
            }
            return null;
        }

	
			
		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
            if (disconnectMode == ext_DisconnectMode.ext_dm_HostShutdown)
            {
                foreach (IDTExtensibility2 cmd in allCommands)
                    cmd.OnDisconnection(disconnectMode, ref custom);
            }
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
			foreach (IDTExtensibility2 cmd in allCommands)
				cmd.OnAddInsUpdate(ref custom);
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
            try
            {
                NDOCommandBar.CreateInstance(applicationObject);  // Makes sure, the command bar exists

                Events events = applicationObject.Events;

                if (null == buildEventHandler)
                {
                    this.buildEventHandler = new BuildEventHandler(applicationObject);

                    buildEvents = events.BuildEvents;

                    buildEvents.OnBuildBegin			+= buildEventHandler.onBuildBegin;
                    buildEvents.OnBuildDone				+= buildEventHandler.onBuildDone;
                    buildEvents.OnBuildProjConfigBegin	+= buildEventHandler.onBuildProjConfigBegin;
                    buildEvents.OnBuildProjConfigDone	+= buildEventHandler.onBuildProjConfigDone;					
                }

				if (null == documentEventHandler)
				{
					this.documentEventHandler = new DocumentEventHandler( applicationObject );
					events.DocumentEvents.DocumentSaved += documentEventHandler.OnDocumentSaved;
				}
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "NDO Enhancer Startup");
            }

			foreach (IDTExtensibility2 cmd in allCommands)
				cmd.OnStartupComplete(ref custom);
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
			foreach (IDTExtensibility2 cmd in allCommands)
				cmd.OnBeginShutdown(ref custom);
		}
		
		/// <summary>
		///      Implements the QueryStatus method of the IDTCommandTarget interface.
		///      This is called when the command's availability is updated
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to determine state for.
		/// </param>
		/// <param term='neededText'>
		///		Text that is needed for the command.
		/// </param>
		/// <param term='status'>
		///		The state of the command in the user interface.
		/// </param>
		/// <param term='commandText'>
		///		Text requested by the neededText parameter.
		/// </param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText)
		{
				foreach (IDTCommandTarget cmd in allCommands)
					cmd.QueryStatus(commandName, neededText, ref status, ref commandText);
		}

		/// <summary>
		///      Implements the Exec method of the IDTCommandTarget interface.
		///      This is called when the command is invoked.
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to execute.
		/// </param>
		/// <param term='executeOption'>
		///		Describes how the command should be run.
		/// </param>
		/// <param term='varIn'>
		///		Parameters passed from the caller to the command handler.
		/// </param>
		/// <param term='varOut'>
		///		Parameters passed from the command handler to the caller.
		/// </param>
		/// <param term='handled'>
		///		Informs the caller if the command was handled or not.
		/// </param>
		/// <seealso class='Exec' />
		public void Exec( string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled )
		{
			handled = false;
			if ( executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault )
			{
				foreach (IDTCommandTarget cmd in allCommands)
					cmd.Exec(commandName, executeOption, ref varIn, ref varOut, ref handled);
			}
		}
		
	}
}