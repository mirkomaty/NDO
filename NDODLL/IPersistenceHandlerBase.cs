//
// Copyright (C) 2002-2014 Mirko Matytschak 
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Data;
using NDO.Logging;

namespace NDO
{
	/// <summary>
	/// This interface is used to manage all transaction related operations of a PersistenceHandler.
	/// </summary>
	public interface IPersistenceHandlerBase
	{
		/// <summary>
		/// Gets or sets a value which determines, if database operations will be logged in a logging file.
		/// </summary>
		bool VerboseMode { get; set; }

		/// <summary>
		/// Called by the NDO Framework. Gets or sets the connection used in all commands. Allows the PersistenceManager to bring in an own open connection for use with transactions.
		/// </summary>
		IDbConnection Connection
		{
			set; get;
		}

		/// <summary>
		/// Called by the NDO Framework. Gets or sets the transaction attribute for each command.
		/// </summary>
		IDbTransaction Transaction
		{
			set; get;
		}

		/// <summary>
		/// In verbose mode, the logs will be written into this file.
		/// </summary>
		ILogAdapter LogAdapter
		{
			set; get;
		}
	}
}
