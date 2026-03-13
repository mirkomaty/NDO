//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
using System.Data.Common;

namespace NDOInterfaces
{
	/// <summary>
	/// This interface is used to manage installation and handling of row update callbacks. 
	/// </summary>
	public interface IRowUpdateListener
	{
		/// <summary>
		/// Gets the data adapter which is responsible for insert and update commands.
		/// This is needed in case of RowUpdate-Callbacks to retrieve autonumbered ids from
		/// data sources which don't support batch insert commands.
		/// </summary>
		DbDataAdapter DataAdapter { get; }

		
		/// <summary>
		/// Callback function for RowUpdate events - see RegisterRowUpdateHandler.
		/// This is needed to retrieve autonumbered IDs from
		/// data sources not supporting batch insert commands.
		/// </summary>
		/// <param name="row">The row which has been updated.</param>
		void OnRowUpdate(DataRow row);

	}
}
