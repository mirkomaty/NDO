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
using System.IO;
using NDO;

namespace NDO.Logging
{
	/// <summary>
	/// The LogFileAdapter class allows NDO logging to a text file.
	/// </summary>
    /// <remarks>
    /// Use the LogAdapter property of the PersistenceManager and 
    /// set the VerboseMode property = true to enable logging.
    /// </remarks>
	public class LogFileAdapter : ILogAdapter
	{
		string fileName;

		/// <summary>
		/// Gets the file name, the log information is written to.
		/// </summary>
		public string FileName
		{
			get { return fileName; }
		}

		/// <summary>
		/// Constructs a log adapter.
		/// </summary>
		/// <param name="fileName">Log information is written to this file.</param>
		/// <remarks>
		/// If the file doesn't exist, it will be created.
		/// If the path to the log file doesn't exist, an exception will be thrown.
		/// </remarks>
		public LogFileAdapter(string fileName)
		{
			this.fileName = fileName;
		}
		#region ILogAdapter Member

		/// <summary>
		/// This function is called by the framework, if an exception occured while
		/// accessing the database.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <remarks>Note, that the error message can consist of numerous lines.</remarks>
		public void Error(string message)
		{
			Output(message);
		}

		/// <summary>
		/// This function is called by the framework, to dump Sql statements and 
		/// transaction start/stop statements.
		/// </summary>
		/// <param name="message">Dump information.</param>
		/// <remarks>Note, that dump information can consist of numerous lines.</remarks>
		public void Info(string message)
		{
			Output(message);
		}

		/// <summary>
		/// This function is called by the framework, to dump Sql statements and 
		/// transaction start/stop statements.
		/// </summary>
		/// <param name="message">Dump information.</param>
		/// <remarks>Note, that dump information can consist of numerous lines.</remarks>
		public void Debug(string message)
		{
			Output(message);
		}

		/// <summary>
		/// This function is called by the framework, if something doesn't work as expected, 
		/// but no exception was thrown.
		/// </summary>
		/// <param name="message">The warning message.</param>
		/// <remarks>Note, that the warning message can consist of numerous lines.</remarks>
		public void Warn(string message)
		{
			Output(message);
		}

		/// <summary>
		/// Deletes all entries in the log file.
		/// </summary>
		public void Clear()
		{
			StreamWriter sw = new StreamWriter(fileName, false);
			sw.Close();
		}

		#endregion

		private void Output(string s)
		{
			using(StreamWriter sw = new StreamWriter(fileName, true))
			{
				sw.WriteLine("--------- " + DateTime.Now.ToString() + " ---------");
				sw.WriteLine(s);
				sw.Close();
			}			
		}
	}
}
