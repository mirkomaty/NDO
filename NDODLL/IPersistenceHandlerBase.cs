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
