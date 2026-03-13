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
