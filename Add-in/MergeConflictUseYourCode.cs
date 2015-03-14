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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif
using System.Text.RegularExpressions;

namespace NDOAddIn
{
    /// <summary>
    /// Zusammenfassung für MergeConflictUseYourCode.
    /// </summary>
    internal class MergeConflictUseYourCode : MergeConflictBase
    {
        public MergeConflictUseYourCode()
        {
            this.CommandBarButtonText = "Use Your Code";
            this.CommandBarButtonToolTip = "Resolves a merge conflict using your code.";
        }


        public override void DoIt()
        {
            EditPoint ep;
            int myCodeOffset;
            int cgCodeOffset;
            int endConflictOffset;
            if (!GetConflictPositions(out ep, out myCodeOffset, out cgCodeOffset, out endConflictOffset))
                return; 

            ep.MoveToAbsoluteOffset(endConflictOffset);
            ep.LineDown(1);
            EditPoint ep2 = ep.CreateEditPoint();
            ep.MoveToAbsoluteOffset(cgCodeOffset);
            ep.Delete(ep2);

            ep.MoveToAbsoluteOffset(myCodeOffset);
            ep.LineDown(1);
            ep2 = ep.CreateEditPoint();
            ep.MoveToAbsoluteOffset(myCodeOffset);
            ep.Delete(ep2);
        }


        #region IDTExtensibility2 Member

        public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInstance, ref Array custom)
        {
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;
            Debug.WriteLine("MergeConflictUseYourCode.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("MergeConflictUseYourCode.OnConnection: command already exists");
                return;
                //----------------
            }

            Debug.WriteLine("MergeConflictUseYourCode.OnConnection: creating command");


            try
            {

                // MergeConflictUseYourCode-Kommando
                Command command = this.AddNamedCommand(108);

                CommandBar commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Code Window"];
                CommandBarButton cbb = (CommandBarButton)command.AddControl(commandBar, 1);
                // Use default for cbb.Style (context menu)

            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.ToString(), "NDO Use Your Code");
#else
				MessageBox.Show(e.Message, "NDO Use Your Code");
#endif
            }
        }



        #endregion
    }
}



