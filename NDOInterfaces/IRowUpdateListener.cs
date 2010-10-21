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
