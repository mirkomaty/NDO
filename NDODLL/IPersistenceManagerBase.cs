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
using NDO.Logging;
using System.Data;

namespace NDO
{
	/// <summary>
	/// Methods, which will be shared by all NDO implementations of PersistenceManager
	/// </summary>
	public interface IPersistenceManagerBase
	{
		/// <summary>
		/// Register a listener to this event, if you have to provide user generated ids. 
		/// This event is usefull for databases, which doesn't provide auto-incremented ids, like the Oracle Db. 
		/// The event will be fired if a new id is needed.
		/// </summary>
		event IdGenerationHandler IdGenerationEvent;

		/// <summary>
		/// Gets or sets the LogAdapter, where the logs are written to.
		/// </summary>
		ILogAdapter LogAdapter { get; set; }

		/// <summary>
		/// If set, the peristence manager writes a log of all SQL statements issued to the databases. 
		/// By default a LogFileAdapter to the file SqlIOLog.txt will be used. The log medium can be
		/// changed using the <see cref="NDO.PersistenceManagerBase.LogAdapter">LogAdapter property</see>.
		/// </summary>
		bool VerboseMode { set; get; }


		/// <summary>
		/// Gets or sets the directory, where NDO writes the sql log file to.
		/// </summary>
		string LogPath { get; set; }

		/// <summary>
		/// Deletes all logging entries, if the logger supports this function.
		/// </summary>
		void ClearLogfile();


		/// <summary>
		/// Gets or sets the type which is used to construct persistence handlers.
		/// </summary>
		Type PersistenceHandlerType { get; set; }

		/// <summary>
		/// Gets the Mapping structure of the application as stored in NDOMapping.xml. 
		/// Use it only if you know exactly, what you're doing!
		/// Do not change anything in the mapping structure because it will cause the
		/// NDO Framework to fail.
		/// </summary>
		NDO.Mapping.NDOMapping NDOMapping { get; }

	}
}
