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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using NDO.Mapping;
using System.Reflection;
using NDO.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDOInterfaces;

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
		private IServiceProvider scopedServiceProvider;
		private IServiceScope scope;
		/// <summary>
		/// Gets or sets the logger for the PersistenceManager instance
		/// </summary>
		protected ILogger Logger { get; set; }
		private IPersistenceHandlerManager persistenceHandlerManager;
		bool isClosing = false;
		private INDOProviderFactory providerFactory;
		private INDOProviderFactory ProviderFactory
		{
			get
			{
				if (this.providerFactory == null)
				{
					var serviceProvider = NDOApplication.ServiceProvider;

					if (serviceProvider != null && serviceProvider.GetService<INDOProviderFactory>() != null)
					{
						this.providerFactory = serviceProvider.GetService<INDOProviderFactory>();
					}
					else
					{
						this.providerFactory = NDOProviderFactory.Instance;
					}
				}

				return this.providerFactory;
			} 
		}


		/// <summary>
		/// If set to true, the resultset of each query is stored, so that results of an identical query 
		/// can be read from the cache.
		/// </summary>
		/// <remarks>
		/// NDO computes an SHA256 hash for each query including the query parameters. The hash is the key to the query cache.
		/// Do not use the QueryCache in applications, which work with one PersistenceManager 
		/// over a long time with multiple transactions.
		/// </remarks>
		public bool UseQueryCache { get; set; }

		private Dictionary<string, object> queryCache = new Dictionary<string, object>();

        /// <summary>
		/// This is the query cache in which resultsets can be stored.
		/// </summary>
		/// <remarks>Usually you won't have to access the query cache.</remarks>
		public Dictionary<string, object> QueryCache => this.queryCache; 

		/// <summary>
		/// Register a listener to this event, if you have to provide user generated ids. 
		/// This event is usefull for databases, which doesn't provide auto-incremented ids, like the Oracle Db. 
		/// The event will be fired if a new id is needed.
		/// </summary>
		public event IdGenerationHandler IdGenerationEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public PersistenceManagerBase( IServiceProvider scopedServiceProvider ) 
		{
			this.scopedServiceProvider = scopedServiceProvider;
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
        /// <param name="scopedServiceProvider">An IServiceProvider instance, which represents a scope (e.g. a request in an AspNet application)</param>
        /// <param name="mappingFile"></param>
        public PersistenceManagerBase(string mappingFile, IServiceProvider scopedServiceProvider)
		{
			this.scopedServiceProvider = scopedServiceProvider;
			Init(mappingFile);
		}

        /// <summary>
        /// Constructs a PersistenceManagerBase object using the mapping object.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="scopedServiceProvider">An IServiceProvider instance, which represents a scope (e.g. a request in an AspNet application)</param>
        public PersistenceManagerBase(NDOMapping mapping, IServiceProvider scopedServiceProvider)
		{
            this.scopedServiceProvider = scopedServiceProvider;
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

			Init( new Mappings( mappingPath, ProviderFactory ) );
		}

		/// <summary>
		/// Initializes the persistence manager
		/// </summary>
		/// <remarks>
		/// Note: This is the method, which will be called from all different ways to instantiate a PersistenceManagerBase.
		/// </remarks>
		/// <param name="mappings"></param>
		internal virtual void Init( Mappings mappings )
		{
			if (NDOApplication.ServiceProvider == null)
				throw new Exception( "ServiceProvider is not initialized. Please setup a host environment. See BuilderExtensions.AddNdo and .UseNdo." );

			Logger = NDOApplication.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger( GetType() );
			this.mappings = mappings;

			var scopedMappingsAccessor = ServiceProvider.GetRequiredService<IMappingsAccessor>();
			scopedMappingsAccessor.Mappings = mappings;

			this.ds = new NDODataSet( this.mappings );  // Each PersistenceManager instance must have it's own DataSet.
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
        /// Gets the IServiceProvider instance of the current scope.
        /// If no ServiceProvider was passed in the constructor, a new scope is created.
        /// </summary>
        internal IServiceProvider ServiceProvider
		{
			get
			{
				if (this.scopedServiceProvider == null)
				{
					this.scope = NDOApplication.ServiceProvider.CreateScope();
					this.scopedServiceProvider = scope.ServiceProvider;
				}

				return this.scopedServiceProvider;
			}
		}

		/// <summary>
		/// Gets or sets an implementation of the PersistenceHandlerManager.
		/// </summary>
		public IPersistenceHandlerManager PersistenceHandlerManager
		{
			get
			{
				return this.persistenceHandlerManager = ServiceProvider.GetRequiredService<IPersistenceHandlerManager>();
			}
			set { this.persistenceHandlerManager = value; }
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
			this.scope.Dispose();
			this.queryCache.Clear();
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
