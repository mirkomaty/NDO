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

namespace NDOInterfaces
{
	/// <summary>
	/// Objects of this class are used to create a database using the default 
	/// implementation of IProvider.CreateDatabase in NDOAbstractProvider.
	/// <seealso cref="NDOAbstractProvider"/>
	/// </summary>
	public class NDOCreateDbParameter
	{
		string databaseName;
		string connection;


		/// <summary>
		/// Unquoted name of the database.
		/// </summary>
		/// <remarks>
		/// Please note, that NDO uses the provider to quote the database name before passing it to CREATE TABLE.
		/// </remarks>
		public string DatabaseName
		{
			get { return databaseName; }
			set { databaseName = value; }
		}

		/// <summary>
		/// Connection string to be used to create the database;
		/// </summary>
		public string Connection
		{
			get { return connection; }
			set { connection = value; }
		}


		/// <summary>
		/// Constructs a NDOCreateDbParameter object.
		/// </summary>
		/// <param name="databaseName">Initializes the datbase name.</param>
		/// <param name="connection">Initializes the connection string.</param>
		public NDOCreateDbParameter(string databaseName, string connection)
		{
			this.databaseName = databaseName;
			this.connection = connection;
		}

		/// <summary>
		/// Constructs a NDOCreateDbParameter object.
		/// </summary>
		public NDOCreateDbParameter(){}
	}
}
