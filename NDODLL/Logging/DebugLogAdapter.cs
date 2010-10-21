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

namespace NDO.Logging
{
	/// <summary>
	/// Logs to System.Diagnostics.Trace.
	/// </summary>
	public class DebugLogAdapter : ILogAdapter
	{
		/// <summary>
		/// Constructs a log adapter.
		/// </summary>
		public DebugLogAdapter()
		{
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
			Trace.WriteLine("Error-log: " + message);
		}

		/// <summary>
		/// This function is called by the framework, to dump Sql statements and 
		/// transaction start/stop statements.
		/// </summary>
		/// <param name="message">Dump information.</param>
		/// <remarks>Note, that dump information could consist of numerous lines.</remarks>
		public void Info(string message)
		{
			Trace.WriteLine(message);
		}

		/// <summary>
		/// This function is called by the framework, if something doesn't work as expected, 
		/// but no exception was thrown.
		/// </summary>
		/// <param name="message">The warning message.</param>
		/// <remarks>Note, that the warning message can consist of numerous lines.</remarks>
		public void Warn(string message)
		{
			Trace.WriteLine("Warning-log: " + message);
		}

		/// <summary>
		/// Calls to this function are ignored.
		/// </summary>
		public void Clear()
		{
			// nix
		}

		#endregion
	}
}
