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

namespace NDO.Logging
{
	/// <summary>
	/// Interface for logging sinks.
	/// </summary>
	public interface ILogAdapter
	{
		/// <summary>
		/// This function is called by the framework, if an exception occured while
		/// accessing the database.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <remarks>Note, that the error message can consist of numerous lines.</remarks>
		void Error(string message);

		/// <summary>
		/// This function is called by the framework, to dump Sql statements and 
		/// transaction start/stop statements.
		/// </summary>
		/// <param name="message">Dump information.</param>
		/// <remarks>Note, that dump information can consist of numerous lines.</remarks>
		void Info(string message);

		/// <summary>
		/// This function is called by the framework, if something doesn't work as expected, 
		/// but no exception was thrown.
		/// </summary>
		/// <param name="message">The warning message.</param>
		/// <remarks>Note, that the warning message can consist of numerous lines.</remarks>
		void Warn(string message);

		/// <summary>
		/// This function is called by the framework, to provide some information for debugging purposes.
		/// </summary>
		/// <param name="message">Dump information.</param>
		/// <remarks>Note, that dump information can consist of numerous lines.</remarks>
		void Debug(string message);


		/// <summary>
		/// If the log sink supports clearing all entries, this function clears all entries.
		/// Else the call to Clear is ignored.
		/// </summary>
		void Clear();
	}
}
