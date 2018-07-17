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
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Data;
using NDO.Logging;
using NDO.Mapping;
using System.Web;
using Unity;
using NDO.Configuration;

namespace NDO
{
	/// <summary>
	/// Provides base functionality for PersistenceManager classes
	/// </summary>
	public class PersistenceManagerBase : IPersistenceManagerBase
	{
		internal Cache cache = new Cache();
		protected DataSet ds = null;
		protected IStateManager sm;
		internal Mappings mappings;  // protected will make the compiler complaining
		protected string mappingPath = null;
		private string logPath;
		private ILogAdapter logAdapter;
		private Type persistenceHandlerType = null;


		/// <summary>
		/// Register a listener to this event, if you have to provide user generated ids. 
		/// This event is usefull for databases, which doesn't provide auto-incremented ids, like the Oracle Db. 
		/// The event will be fired if a new id is needed.
		/// </summary>
		public event IdGenerationHandler IdGenerationEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public PersistenceManagerBase(UnityContainer container = null) 
		{
			if (container == null)
				container = NDOContainer.Instance;
			Assembly ass = Assembly.GetEntryAssembly();

			if (ass == null)
			{
				StackTrace st = new StackTrace();
				Assembly ndoAss = this.GetType().Assembly;
				for(int i = 0; i < st.FrameCount; i++)
				{
					MethodBase mb = st.GetFrame(i).GetMethod();
					MethodInfo mi = mb as MethodInfo;
					Assembly frameAss = null;
					if (mi != null)
					{
						frameAss = mi.ReflectedType.Assembly;
					}
					else
					{
						ConstructorInfo ci = mb as ConstructorInfo;
						if (ci != null)
						{
							frameAss = ci.ReflectedType.Assembly;
						}
					}
					if (frameAss != null && frameAss != ndoAss)
					{
						ass = frameAss;
						break;
					}
				}
			}
			if (ass == null)
				throw new NDOException(49, "Can't determine the path to the mapping file - please provide a mapping file path as argument to the PersistenceManager ctor.");

			string baseDir;
			if ( HttpContext.Current != null /*ass.Location.ToLower().IndexOf("temporary asp.net files") > -1*/)
			{
				/*
				string localPath = HttpContext.Current.Request.Url.LocalPath;
				int p; 
				if ((p = localPath.ToLower().IndexOf(".asmx")) > -1)
					localPath = localPath.Substring(0, p + 5);
				 */
				baseDir = HttpContext.Current.Request.MapPath("/");
				//baseDir = Path.GetDirectoryName(localPath);
				if (Directory.Exists(Path.Combine(baseDir, "bin")))
					baseDir = Path.Combine( baseDir, "bin" );
			}
			else
			{
				baseDir = Path.GetDirectoryName(ass.Location);
			}
			string mappingFileName = Path.GetFileNameWithoutExtension(ass.Location) + ".ndo.xml";
			string mp = null;

			mp = Path.Combine(baseDir, mappingFileName);
			if (!File.Exists(mp))
				mp = Path.Combine(baseDir, "NDOMapping.xml");
			Init(Path.Combine(baseDir, mp));
		}

		public PersistenceManagerBase(string mappingFile)
		{
			Init(mappingFile);
		}

		protected virtual void Init(string mappingFileName)
		{
			string dir = Path.GetDirectoryName(mappingFileName);
			this.mappingPath = mappingFileName;
			if (!File.Exists(mappingPath))
				throw new NDOException(45, String.Format("Mapping File {0} doesn't exist.", mappingFileName));
			mappings = new Mappings(mappingPath, this.persistenceHandlerType);
			ds = new NDODataSet(mappings);
			mappings.DataSet = ds;
			string logPath = AppDomain.CurrentDomain.BaseDirectory;
			if (logPath == null)
				logPath = dir;
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
		public Type PersistenceHandlerType
		{
			get { return persistenceHandlerType; }
			set 
			{ 
				if (value != null && value.GetInterface("IPersistenceHandler") == null)
					throw new NDOException(46, "Invalid PersistenceHandlerType: " + value.FullName);
                this.persistenceHandlerType = value;
                this.mappings.DefaultHandlerType = value;
            }
		}

		/// <summary>
		/// Available only in the NDO Professional Edition or above. 
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
		/// Available only in the NDO Professional Edition or above. 
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

		internal DataSet DataSet
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

		public bool LoggingPossible
		{
			get { return (VerboseMode && this.logAdapter != null); }
		}

		protected IPersistenceCapable CheckPc(object o)
		{
			IPersistenceCapable pc = o as IPersistenceCapable;
			if (pc == null && !(o == null))
				throw new NDOException(31, "Parameter should implement IPersistenceCapable. Check, if the type " + o.GetType().FullName + "," + o.GetType().Assembly.FullName + " is enhanced.");
			return pc;
		}






	}
}
