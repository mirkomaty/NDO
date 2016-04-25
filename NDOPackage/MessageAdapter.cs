//
// Copyright (c) 2002-2016 Mirko Matytschak 
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
using EnvDTE;
using EnvDTE80;

namespace NETDataObjects.NDOVSPackage
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

		ErrorList errorList;

		public MessageAdapter( )
		{
			_DTE applicationObject = ApplicationObject.VisualStudioApplication;
			this.errorList = ((EnvDTE80.DTE2)applicationObject).ToolWindows.ErrorList;

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
