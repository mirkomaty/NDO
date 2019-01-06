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
using System.Reflection;
using System.Data;
using System.Collections;
using System.IO;
using NDO.Logging;

namespace NDO
{
	/// <summary>
	/// Use this class, if you need to parameterize PersistenceManager instances in always the same way.
	/// </summary>
	public sealed class PersistenceManagerFactory
	{
		private static string mappingFileName;
		private static bool hollowMode = false;
		private static IdGenerationHandler idGenerationHandler = null;
		private static TransactionMode transactionMode = TransactionMode.None;
		private static bool verboseMode = false;		
		private static Type persistenceHandlerType;
		private static IsolationLevel isolationLevel = IsolationLevel.ReadCommitted;
		private static CollisionHandler collisionHandler;
		private static string logPath;
		private static OpenConnectionListener openConnectionListener;
		internal static Assembly CallingAssembly;
		private static OnSavingHandler onSavingHandler;
		private static ILogAdapter logAdapter;

		/// <summary>
		/// <see cref="NDO.PersistenceManagerBase.LogAdapter"/>
		/// </summary>
		public static ILogAdapter LogAdapter
		{
			get { return logAdapter; }
			set { logAdapter = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManager.OnSavingEvent"/>
		/// </summary>
		public static OnSavingHandler OnSavingHandler
		{
			get { return onSavingHandler; }
			set { onSavingHandler = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManager.RegisterConnectionListener"/>
		/// </summary>
		public static OpenConnectionListener OpenConnectionListener
		{
			get { return openConnectionListener; }
			set { openConnectionListener = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManagerBase.LogPath"/>
		/// </summary>
		public static string LogPath
		{
			get { return logPath; }
			set { logPath = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManager.CollisionEvent"/>
		/// </summary>
		public static CollisionHandler CollisionHandler
		{
			get { return collisionHandler; }
			set { collisionHandler = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManagerBase.PersistenceHandlerType"/>
		/// </summary>
		public static Type PersistenceHandlerType
		{
			get { return persistenceHandlerType; }
			set { persistenceHandlerType = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManagerBase.VerboseMode"/>
		/// </summary>
		public static bool VerboseMode
		{
			get { return verboseMode; }
			set { verboseMode = value; }
		}


		/// <summary>
		/// <see cref="NDO.PersistenceManager.TransactionMode"/>
		/// </summary>
		public static TransactionMode TransactionMode
		{
			get { return transactionMode; }
			set { transactionMode = value; }
		}


		
		/// <summary>
		/// <see cref="NDO.PersistenceManager.IsolationLevel"/>
		/// </summary>
		public static IsolationLevel IsolationLevel
		{
			get { return isolationLevel; }
			set { isolationLevel = value; }
		}
		/// <summary>
		/// <see cref="PersistenceManagerBase.IdGenerationEvent"/>
		/// </summary>
		public static IdGenerationHandler IdGenerationHandler
		{
			get { return idGenerationHandler; }
			set { idGenerationHandler = value; }
		}

		/// <summary>
		/// <see cref="NDO.PersistenceManager.HollowMode"/>
		/// </summary>
		public static bool HollowMode
		{
			get { return hollowMode; }
			set { hollowMode = value; }
		}

		/// <summary>
		/// Uses the PersistenceManager constructor with the string parameter.
		/// </summary>
		public static string MappingFileName
		{
			get { return mappingFileName; }
			set 
			{ 
				if (!File.Exists(value))
					throw new NDOException(45, "Mapping File " + value + " doesn't exist.");
				mappingFileName = value; 
			}
		}

		/// <summary>
		/// Creates an instance of the PersistenceManager class, which will be parameterized using the
		/// static properties of the PersistenceManagerFactory class. 
		/// <seealso cref="NDO.PersistenceManager"/>
		/// </summary>
		/// <returns>The created PersistenceManager object.</returns>
		public static PersistenceManager NewPersistenceManager()
		{
			CallingAssembly = Assembly.GetCallingAssembly();
			PersistenceManager pm;
			if (mappingFileName == null || mappingFileName.Trim() == string.Empty)
				pm = new PersistenceManager();
			else
				pm = new PersistenceManager(mappingFileName);
			pm.HollowMode = hollowMode;
			if (idGenerationHandler != null)
				pm.IdGenerationEvent += idGenerationHandler;
			if (collisionHandler != null)
				pm.CollisionEvent += collisionHandler;
			pm.IsolationLevel = isolationLevel;
			pm.TransactionMode = transactionMode;
			pm.VerboseMode = verboseMode;
#pragma warning disable 618
			if (persistenceHandlerType != null)
				pm.PersistenceHandlerType = persistenceHandlerType;
#pragma warning restore 618
			pm.LogPath = logPath;
			if (openConnectionListener != null)
				pm.RegisterConnectionListener(openConnectionListener);
			if (onSavingHandler != null)
				pm.OnSavingEvent += onSavingHandler;
			if (logAdapter != null)
				pm.LogAdapter = logAdapter;
			return pm;
		}


		// don't instanciate
		private PersistenceManagerFactory()
		{
		}

//		public static PersistenceManagerFactory()
//		{
//		}
	}
}
