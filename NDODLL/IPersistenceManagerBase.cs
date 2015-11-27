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
