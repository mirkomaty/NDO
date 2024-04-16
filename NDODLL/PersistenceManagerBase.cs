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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using NDO.Logging;
using NDO.Mapping;
using NDO.Configuration;
using NDO.SqlPersistenceHandling;
using System.Reflection;

namespace NDO
{
	/// <summary>
	/// Provides base functionality for PersistenceManager classes
	/// </summary>
	public class PersistenceManagerBase : IPersistenceManagerBase
	{
		internal Cache cache = new Cache();
		/// <summary>
		/// The DataSet used as template for DataRows
		/// </summary>
		protected DataSet ds = null;
		/// <summary>
		/// The StateManager instance which will be used for all objects
		/// </summary>
		protected IStateManager sm;
		internal Mappings mappings;  // protected will make the compiler complaining
		private string logPath;
		private ILogAdapter logAdapter;
		private Type persistenceHandlerType = null;
		private INDOContainer configContainer;
		private IPersistenceHandlerManager persistenceHandlerManager;
		bool isClosing = false;

		/// <summary>
		/// Register a listener to this event, if you have to provide user generated ids. 
		/// This event is usefull for databases, which doesn't provide auto-incremented ids, like the Oracle Db. 
		/// The event will be fired if a new id is needed.
		/// </summary>
		public event IdGenerationHandler IdGenerationEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public PersistenceManagerBase() 
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists( Path.Combine( baseDir, "Web.config" ) ))
                baseDir = Path.Combine( baseDir, "bin" );
			var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name;
			List<string> paths = new List<string>();
			if (entryAssemblyName != null)
			paths.Add( Path.Combine( baseDir, $"{entryAssemblyName}.ndo.mapping" ) );
			paths.Add( Path.Combine( baseDir, "NDOMapping.xml" ) );
			
			bool found = false;
			foreach (var path in paths)
			{
				if (File.Exists(path))
				{
					Init( path );
					found = true;
					break;
				}
			}
			if (!found)
                throw new NDOException( 49, $"Can't determine the path to the mapping file. Tried the following locations:\n{string.Join("\n", paths)}\nPlease provide a mapping file path as argument to the PersistenceManager ctor." );
		}

		/// <summary>
		/// Constructs a PersistenceManagerBase object using the path to a mapping file.
		/// </summary>
		/// <param name="mappingFile"></param>
		public PersistenceManagerBase(string mappingFile)
		{
			Init(mappingFile);
		}

		/// <summary>
		/// Constructs a PersistenceManagerBase object using the mapping object.
		/// </summary>
		/// <param name="mapping"></param>
		public PersistenceManagerBase(NDOMapping mapping)
		{
			var localMappings = mapping as Mappings;
			if (localMappings == null)
				throw new ArgumentException( "The mapping must be constructed by a PersistenceManager", nameof( mapping ) );

			Init( localMappings );
		}

		/// <summary>
		/// Initializes a PersistenceManager using the path to a mapping file
		/// </summary>
		/// <param name="mappingPath"></param>
		protected virtual void Init(string mappingPath)
		{
			if (!File.Exists(mappingPath))
				throw new NDOException(45, String.Format("Mapping File {0} doesn't exist.", mappingPath));
			Init( new Mappings( mappingPath ) );
		}

		/// <summary>
		/// Initializes the persistence manager
		/// </summary>
		/// <remarks>
		/// Note: This is the method, which will be called from all different ways to instantiate a PersistenceManagerBase.
		/// </remarks>
		/// <param name="mapping"></param>
		internal virtual void Init( Mappings mapping )
		{
			this.mappings = mapping;

			ConfigContainer.RegisterInstance( mappings );

			this.ds = new NDODataSet( mappings );  // Each PersistenceManager instance must have it's own DataSet.

			string logPath = AppDomain.CurrentDomain.BaseDirectory;

			if (logPath == null)
				logPath = Path.GetDirectoryName( mapping.FileName );

			this.LogPath = logPath;
		}

		/// <summary>
		/// Used by PersistenceManagers, to get an owner supplied id.
		/// </summary>
		/// <param name="t">Type of the object, the id is intended for.</param>
		/// <param name="oid">ObjectId object which will hold the id value.</param>
		protected void FireIdGenerationEvent(Type t, ObjectId oid)
		{
			if (IdGenerationEvent != null)
				IdGenerationEvent(t, oid);
		}


		Dictionary<string,Class> myClassesName;
        Dictionary<Type, Class> myClassesType;

		/// <summary>
		/// Initializes the class mappings
		/// </summary>
		protected void InitClasses()
		{
            int cnt = mappings.Classes.Count();
            myClassesName = new Dictionary<string, Class>(cnt);
            myClassesType = new Dictionary<Type, Class>(cnt);
			foreach(Class cl in mappings.Classes)
			{
				myClassesName.Add(cl.FullName, cl);
				myClassesType.Add(cl.SystemType, cl);
			}
		}
		

		internal Class GetClass(string name) 
		{
			if (!myClassesName.ContainsKey(name))
				throw new NDOException(17, "Can't find mapping information for class " + name);

			return myClassesName[name];
		}

		internal Class GetClass(IPersistenceCapable pc) 
		{
			return GetClass(pc.GetType());
		}

		internal Class GetClass(Type type) 
		{
			Type t = type;

			if (type.IsGenericType)
    			t = type.GetGenericTypeDefinition();

			if (! myClassesType.ContainsKey(t))
				throw new NDOException(17, "Can't find mapping information for class " + t.FullName);

			return myClassesType[t];

		}

		internal Field GetField(Class cl, string field)
		{
			Field f = cl.FindField(field);
			if (f == null)
				throw new NDOException(7, "Can't find mapping information for field " + cl.FullName + "." + field);
			return f;
		}

		/// <summary>
		/// Hilfsfunktion
		/// Liefert die Tabelle im DataSet ab, in der die DataRows des Datentyps liegen
		/// </summary>
		/// <param name="t">Data type</param>
		/// <returns></returns>
		protected DataTable GetTable(Type t) 
		{
			return GetTable(GetClass(t).TableName);
		}

		/// <summary>
		/// Hilfsfunktion
		/// Liefert die Tabelle im DataSet ab, in der die DataRow des Objekts liegt
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
		protected DataTable GetTable(IPersistenceCapable pc) 
		{
			return GetTable(GetClass(pc).TableName);
		}


		/// <summary>
		/// Retrieve a table with the given name
		/// </summary>
		/// <param name="name">Table name</param>
		/// <returns></returns>
		protected DataTable GetTable(string name) 
		{
			DataTable dt = ds.Tables[name];
			if (dt == null)
				throw new NDOException(39, "Can't find table '" + name + "' in the schema. Check your mapping file.");
			return dt;
		}

		/// <summary>
		/// Gets a DataRow for a given object; if necessary the row will be constructed
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <returns></returns>
		protected DataRow GetDataRow(IPersistenceCapable pc) 
		{
			DataRow row;
			if ((row = this.cache.GetDataRow(pc)) != null)
				return row;
			return null;
		}



		/// <summary>
		/// Indicates, if there is a listener registered for the IdGenerationEvent.
		/// </summary>
		public bool HasOwnerCreatedIds
		{
			get { return IdGenerationEvent != null; }
		}

		/// <summary>
		/// If set, the PersistenceManager writes a log of all SQL statements issued to the databases. 
		/// By default a LogFileAdapter to the file SqlIOLog.txt will be used. The log medium can be
		/// changed using the <see cref="NDO.PersistenceManagerBase.LogAdapter">LogAdapter property</see>.
		/// </summary>
		public virtual bool VerboseMode 
		{
			get {return mappings.VerboseMode;}
			set {mappings.VerboseMode = value;}
		}

		/// <summary>
		/// Gets or sets the type which is used to construct persistence handlers.
		/// </summary>
		[Obsolete("Use the ConfigContainer to register a handler type.")]
		public Type PersistenceHandlerType
		{
			get { return persistenceHandlerType; }
			set 
			{ 
				if (value != null && value.GetInterface("IPersistenceHandler") == null)
					throw new NDOException(46, "Invalid PersistenceHandlerType: " + value.FullName);
				ConfigContainer.RegisterType( typeof( IPersistenceHandler ), persistenceHandlerType );
            }
		}

		/// <summary>
		/// Gets or sets the container for the configuration of the system.
		/// </summary>
		public INDOContainer ConfigContainer
		{
			get
			{
				if (this.configContainer == null)
				{
					this.configContainer = NDOContainer.Instance.CreateChildContainer();
					this.configContainer.RegisterType<IQueryGenerator, SqlQueryGenerator>();

					// Currently the PersistenceManager instance is not used.
					// But we are able to pull it from the container.
					this.configContainer.RegisterInstance( typeof( PersistenceManager ), this );
				}

				return this.configContainer;
			}
			set { this.configContainer = value; }
		}


		/// <summary>
		/// Sets or gets the logging Adapter, log information is written to.
		/// </summary>
		/// <remarks>
		/// If LogPath is set, a LogFileAdapter object is created and attached to this property. 
		/// <seealso cref="LogPath"/><seealso cref="ILogAdapter"/>
		/// </remarks>
		public ILogAdapter LogAdapter
		{
			get 
			{ 
				return this.logAdapter; 
			}
			set 
			{ 
				this.logAdapter = value; 
				mappings.LogAdapter = this.logAdapter;
				LogFileAdapter lfa = this.logAdapter as LogFileAdapter;
				if (lfa != null)
				{
					this.logPath = Path.GetDirectoryName(lfa.FileName);
				}
			}
		}

		/// <summary>
		/// Gets or sets an implementation of the PersistenceHandlerManager.
		/// </summary>
		public IPersistenceHandlerManager PersistenceHandlerManager
		{
			get
			{
				// (this.persistenceHandlerManager == null)
				return this.persistenceHandlerManager = ConfigContainer.Resolve<IPersistenceHandlerManager>();

				//return this.persistenceHandlerManager;
			}
			set { this.persistenceHandlerManager = value; }
		}


		/// <summary>
		/// Gets or sets the directory, where NDO writes the sql log file to.
		/// </summary>
		/// <remarks>
		/// A file with the name NDO.Sql.log will be generated in the LogPath, if 
		/// verbose mode is set to true. Note, that a FileLogAdapter object is created, 
		/// if LogPath is set. If a LogAdapter is set, LogPath might 
		/// reflect an undefined state.<seealso cref="LogAdapter"/><seealso cref="ILogAdapter"/>
		/// </remarks>
		public string LogPath
		{
			get { return logPath; }
			set 
			{ 
				logPath = value;
				if (logPath == null)
					return;
				if (!Directory.Exists(value))
					throw new NDOException(47, "Log path doesn't exist: " + value);
				string fileName = Path.Combine(logPath, "NDO.Sql.log");
				// use the Property to invoke the additional logic
				this.LogAdapter = new LogFileAdapter(fileName);
			}
		}

		
		/// <summary>
		/// Gets the Mapping structure of the application as stored in NDOMapping.xml. 
		/// Use it only if you know exactly, what you're doing!
		/// Do not change anything in the mapping structure because it will cause the
		/// NDO Framework to fail.
		/// </summary>
		public NDOMapping NDOMapping
		{ 
			get { return this.mappings; }
		}

		/// <summary>
		/// Gets the DataSet behind the operations of the pm.
		/// </summary>
		/// <remarks>This property should't be used by user code. It exists only for test purposes.</remarks>
		public DataSet DataSet
		{
			get { return this.ds; }
		}

		internal NDO.Cache Cache
		{
			get { return this.cache; }
		}

		/// <summary>
		/// Clears any log file entries in the log file
		/// </summary>
		/// <remarks>
		/// Note, that not all LogAdapters support this function. In that case, the 
		/// call to ClearLogfile is ignored.
		/// </remarks>
		public void ClearLogfile()
		{
			if (this.logAdapter != null)
				this.LogAdapter.Clear();
		}

		/// <summary>
		/// Determines, if a log message will actually be issued.
		/// </summary>
		public bool LoggingPossible
		{
			get { return (VerboseMode && this.logAdapter != null); }
		}

		/// <summary>
		/// Checks whether an object is an IPersistenceCapable and converts the object into an IPersistenceCapable.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		/// <remarks>Throws an NDOException, if the object can't be converted.</remarks>
		protected internal IPersistenceCapable CheckPc(object o)
		{
			IPersistenceCapable pc = o as IPersistenceCapable;
			if (pc == null && !(o == null))
				throw new NDOException(31, "Parameter should implement IPersistenceCapable. Check, if the type " + o.GetType().FullName + "," + o.GetType().Assembly.FullName + " is enhanced.");
			return pc;
		}

		/// <summary>
		/// Closes the PersistenceManager and releases all resources.
		/// </summary>
		public virtual void Close()
		{
			if (isClosing)
				return;
			isClosing = true;
			this.ds.Dispose();
			this.ds = null;
			this.configContainer.Dispose();  // Leads to another Disposal of the PM. therefore we query for isClosing.
		}

		/// <summary>
		/// IDisposable implementation.
		/// </summary>
		/// <remarks>Note: The derived classes don't need to override the Dispose methods, since Close() is virtual. Just override Close() and call base.Close() in the overridden version.</remarks>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				Close();
		}

		/// <summary>
		/// Disposes any Resources which might be held by the PersistenceManager implementation.
		/// </summary>
		public virtual void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~PersistenceManagerBase()
		{
			Dispose( false );
		}
	}
}
