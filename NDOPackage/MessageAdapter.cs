using System;
using EnvDTE;
using EnvDTE80;

namespace NETDataObjects.NDOVSPackage
{
    /// <summary>
    /// Use this interface to output messages, while generating DDL code.
    /// </summary>
    public interface IMessageAdapter
    {
        /// <summary>
        /// Increases the current indent level.
        /// </summary>
        void Indent();
        /// <summary>
        /// Decreases the current indent level.
        /// </summary>
        void Unindent();
        /// <summary>
        /// Writes the given text to the output device.
        /// </summary>
        /// <param name="text">A message to write.</param>
        /// <remarks>
        /// The text will be written to the Visual Studio Output pane, if the enhancer runs in visual studio.
        /// The stand-alone enhancer writes to the console.
        /// </remarks>
        void Write(String text);
        /// <summary>
        /// Writes the given text to the output device.
        /// </summary>
        /// <param name="text">A message to write.</param>
        /// <remarks>
        /// The text will be written to the Visual Studio Output pane, if the enhancer runs in visual studio.
        /// The stand-alone enhancer writes to the console.
        /// </remarks>
        void WriteLine(String text);
    }

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
