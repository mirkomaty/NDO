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
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for MessageAdapter.
	/// </summary>
	internal class MessageAdapter : IMessageAdapter
	{
		int indent = 0;
		bool success = true;
		private OutputWindowPane		outputPane;
		private TaskList				taskList;

#if NET20
		ErrorList errorList;
#endif

		public MessageAdapter( )
		{
			_DTE applicationObject = ApplicationObject.VisualStudioApplication;
#if NET20
			this.errorList = ((EnvDTE80.DTE2)applicationObject).ToolWindows.ErrorList;
#endif

			OutputWindow outputWindow = applicationObject.Windows.Item( Constants.vsWindowKindOutput ).Object as OutputWindow;			
			this.taskList = applicationObject.Windows.Item( Constants.vsWindowKindTaskList ).Object as TaskList;
			
			if ( null != outputWindow )
			{
				try
				{
					this.outputPane = outputWindow.OutputWindowPanes.Item("{1BD8A850-02D1-11D1-BEE7-00A0C913D1F8}");
					//outputPane = outputWindow.OutputWindowPanes.Item( "Build" );
				}
				catch ( System.Exception )
				{
				}
			}
		}



		public void Indent() { indent += 2; }
		public void Unindent()
		{
			if (indent >= 2)
				indent -= 2;
		}

//		public void DeleteTasks()
//		{
//			foreach (TaskItem ti in taskList.TaskItems)
//				if (ti.Description.StartsWith("NDO Enhancer"))
//					ti.Delete();
//		}

		public bool Success
		{
			get 
			{
				return success;
			}
			set
			{
				success = value;
			}
		}

//		public void AddTask()
//		{
//			success = false;
//			//TaskItem taskItem = new TaskItem();
//			if (taskList == null)
//				return;
//			foreach(TaskItem ti in taskList.TaskItems)
//				if(ti.Category.StartsWith("NDO Enhancer Error"))
//					return;
//			taskList.TaskItems.Add("BuildCompile", "", "NDO Enhancer Error - see Output Window", vsTaskPriority.vsTaskPriorityHigh, vsTaskIcon.vsTaskIconCompile, false, "", 0, true, true);
//		}

		public void ActivateErrorList()
		{
#if NET11
			taskList.Parent.Activate();
#else
			errorList.Parent.Activate();
#endif
		}

		public void ShowError(string errorMsg)
		{
			outputPane.OutputTaskItemString(errorMsg + '\n', vsTaskPriority.vsTaskPriorityHigh, 
				"BuildCompile", vsTaskIcon.vsTaskIconCompile, "NDO Enhancer", 0, errorMsg, true);
			this.success = false;
		}

/*
		public void DumpTasks()
		{
			taskList.TaskItems.Add("BuildCompile", "", "NDO Enhancer Error - see Output Window", vsTaskPriority.vsTaskPriorityHigh, vsTaskIcon.vsTaskIconCompile, false, "(file not specified)", 1, true, true);
			this.WriteLine("---------");
			foreach(TaskItem ti in taskList.TaskItems)
				this.WriteLine("|" + ti.Category + "|" + ti.SubCategory + "|" + ti.Displayed + "|" + ti.Priority + "|" + ti.Description + "|" + ti.Line.ToString() + "|");
			this.WriteLine("---------");
		}
*/

		public void Write( String text )
		{
			string indentStr = string.Empty;
			for (int i = 0; i < indent; i++)
				indentStr += ' ';
			
			if ( null != outputPane )
				outputPane.OutputString( indentStr + text );
		}

		
		public void
		WriteInsertedLine( String text )
		{
			Indent();
			WriteLine(text);
			Unindent();
		}
		

		public void
		WriteLine( String text )
		{
			string indentStr = string.Empty;
			for (int i = 0; i < indent; i++)
				indentStr += ' ';
			
			if ( null != outputPane )
				outputPane.OutputString( indentStr + text + '\n');
		}


	}
}
