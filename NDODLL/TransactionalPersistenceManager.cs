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


#if nix
using System;
using System.Collections;
using System.Reflection;
using NDO;
using System.EnterpriseServices;

namespace NDO
{
	/// <summary>
	/// TransactionalPersistenceManager has the same functionality as the 
	/// PersistenceManager but adds Transaction enlisting via .NET Enterprise Services
	/// </summary>
	[Transaction(TransactionOption.Supported)]
	public class TransactionalPersistenceManager : ServicedComponent, IPersistenceManager
	{
		PersistenceManager pm;
		INDOTx committer = null;

		internal INDOTx Committer
		{
//			get { return committer; }
			set { committer = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public TransactionalPersistenceManager()
		{
			pm = new PersistenceManager();
		}


		#region Implementation of IPersistenceManager

		/// <summary>
		/// Gets or sets the type which is used to construct persistence handlers
		/// </summary>
		public Type PersistenceHandlerType
		{
			get { return pm.PersistenceHandlerType; }
			set { pm.PersistenceHandlerType = value; }
		}

		/// <summary>
		/// Registers a listener which will be notified, if a new connection is opened.
		/// </summary>
		/// <param name="listener">Delegate of a listener function</param>
		/// <remarks>The listener is called the first time a certain connection is used. A call to Save() resets the connection list so that the listener is called again.</remarks>
		public void RegisterConnectionListener(OpenConnectionListener listener)
		{
			pm.RegisterConnectionListener(listener);
		}

		/// <summary>
		/// See IPersistenceManager:MakePersistent
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		public void MakePersistent(NDO.IPersistenceCapable pc)
		{
			pm.MakePersistent(pc);
		}

		/// <summary>
		/// See IPersistenceManager:MakePersistent
		/// </summary>
		/// <param name="list">List of persistent objects</param>
		public void MakePersistent(System.Collections.IList list)
		{
			pm.MakePersistent(list);
		}

		/// <summary>
		/// See IPersistenceManager:MakeTransient
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		public void MakeTransient(NDO.IPersistenceCapable pc)
		{
			pm.MakeTransient(pc);
		}

		/// <summary>
		/// See IPersistenceManager:MakeTransient
		/// </summary>
		/// <param name="list">List of persistent objects</param>
		public void MakeTransient(System.Collections.IList list)
		{
			pm.MakeTransient(list);
		}


		/// <summary>
		/// See IPersistenceManager:Delete
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		public void Delete(NDO.IPersistenceCapable pc)
		{
			pm.Delete(pc);
		}

		/// <summary>
		/// See IPersistenceManager:Delete
		/// </summary>
		/// <param name="list">List of persistent objects</param>
		public void Delete(System.Collections.IList list)
		{
			pm.Delete(list);
		}

		/// <summary>
		/// See IPersistenceManager:MakeHollow
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		public void MakeHollow(NDO.IPersistenceCapable pc)
		{
			pm.MakeHollow(pc);		
		}

		/// <summary>
		/// See IPersistenceManager:MakeHollow
		/// </summary>
		/// <param name="list">List of persistent objects</param>
		public void MakeHollow(System.Collections.IList list)
		{
			pm.MakeHollow(list);		
		
		}

		/// <summary>
		/// See IPersistenceManager:MakeAllHollow
		/// </summary>
		public void MakeAllHollow()
		{
			pm.MakeAllHollow();		
		}

		/// <summary>
		/// See IPersistenceManager:FindObjects
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NDO.IPersistenceCapable FindObject(NDO.ObjectId id)
		{
			return pm.FindObject(id);
		}


		/// <summary>
		/// See IPersistenceManager:GetClassExtent
		/// </summary>
		/// <param name="t">DataType</param>
		/// <returns>List of persistent objects</returns>
		public System.Collections.IList GetClassExtent(System.Type t)
		{
			return pm.GetClassExtent(t);
		}

		/// <summary>
		/// See IPersistenceManager:GetClassExtent
		/// </summary>
		/// <param name="t">DataType</param>
		/// <param name="hollow">Retrieve only object ids</param>
		/// <returns>List of persistent objects</returns>
		public System.Collections.IList GetClassExtent(System.Type t, bool hollow)
		{
			return pm.GetClassExtent(t, hollow);
		}

/*
		/// <summary>
		/// Function is obsolete - See IPersistenceManager:NewQuery
		/// </summary>
		/// <param name="t">Result data type</param>
		/// <param name="expression">Filter expression</param>
		/// <param name="hollow">Retrieve only object ids</param>
		/// <returns>List of persistent objects</returns>
		public System.Collections.IList Query(System.Type t, string expression, bool hollow)
		{
			return pm.Query(t, expression, hollow);
		}

		/// <summary>
		/// Function is obsolete - See IPersistenceManager:NewQuery
		/// </summary>
		/// <param name="t">Result data type</param>
		/// <param name="expression">Filter expression</param>
		/// <returns>List of persistent objects</returns>
		public System.Collections.IList Query(System.Type t, string expression)
		{
			return pm.Query(t, expression);;
		}
*/
		/// <summary>
		/// See IPersistenceManager:RefreshAll
		/// </summary>
		public void RefreshAll()
		{
			pm.RefreshAll();
		}

		/// <summary>
		/// See IPersistenceManager:Refresh
		/// </summary>
		/// <param name="list">List of persistent objects</param>
		public void Refresh(System.Collections.IList list)
		{
			pm.Refresh(list);
		}

		/// <summary>
		/// See IPersistenceManager:Refresh
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		public void Refresh(NDO.IPersistenceCapable pc)
		{
			pm.Refresh(pc);
		}

		/// <summary>
		/// See IPersistenceManager:Save
		/// </summary>
		public void Save()
		{
			pm.Save();
			if (committer == null)
				throw new InternalException(227, "TransactionalPersistenceManager: Committer is null - can't commit the transaction.");
			ContextUtil.MyTransactionVote = TransactionVote.Commit;
			committer.Commit();
		}

		/// <summary>
		/// See IPersistenceManager:Abort
		/// </summary>
		public void Abort()
		{
			ContextUtil.SetAbort();
			pm.Abort();
		}

		/// <summary>
		/// See IPersistenceManager:Close
		/// </summary>
		[AutoComplete]
		public void Close()
		{
			pm.Close();
		}

		/// <summary>
		/// See IPersistenceManager:CleanupCache
		/// </summary>
		public void CleanupCache()
		{
			pm.CleanupCache();
		}

		/// <summary>
		/// See IPersistenceManager:UnloadCache
		/// </summary>
		public void UnloadCache()
		{
			pm.UnloadCache();
		}

		/// <summary>
		/// See IPersistenceManager:HollowMode
		/// </summary>
		public bool HollowMode
		{
			get
			{
				return pm.HollowMode;
			}
			set
			{
				pm.HollowMode = value;
			}
		}

		/// <summary>
		/// See IPersistenceManager:NewQuery
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">Filter expression</param>
		/// <returns>A query object</returns>
		public NDO.Query NewQuery(System.Type t, string expression)
		{
			return pm.NewQuery(t, expression);
		}

		/// <summary>
		/// See IPersistenceManager:NewQuery
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">Filter expression</param>
		/// <param name="hollow"></param>
		/// <returns>A query object</returns>
		public NDO.Query NewQuery(System.Type t, string expression, bool hollow)
		{
			return pm.NewQuery(t, expression, hollow);
		}

		/// <summary>
		/// See IPersistenceManager:NewQuery
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">Filter expression</param>
		/// <param name="hollow"></param>
		/// <param name="queryLanguage"></param>
		/// <returns>A query object</returns>
		public virtual NDO.Query NewQuery(System.Type t, string expression, bool hollow, NDO.Query.Language queryLanguage)
		{
			return pm.NewQuery(t, expression, hollow, queryLanguage);
		}

		public virtual bool VerboseMode
		{
			get {return pm.VerboseMode;}
			set {pm.VerboseMode = value;}
		}


		/// <summary>
		/// Gets the Mapping structure of the application as stored in NDOMapping.xml. 
		/// Use it only if you know exactly, what you're doing!
		/// Do not change anything in the mapping structure because it can cause the
		/// NDO Framework to fail.
		/// </summary>
		public NDO.Mapping.NDOMapping NDOMapping
		{ 
			get { return pm.NDOMapping; } 
		}

		#endregion
		

		/// <summary>
		/// <see cref="PersistenceManager.TransactionMode"/>
		/// </summary>
		public TransactionMode TransactionMode
		{
			get { return TransactionMode.None; }
			set { }
		}
	
		/// <summary>
		/// <see cref="PersistenceManager.IsolationLevel"/>
		/// </summary>
		public System.Data.IsolationLevel IsolationLevel
		{
			get { return System.Data.IsolationLevel.Unspecified; }
			set { }
		}

		/// <summary>
		/// <see cref="PersistenceManager.LogPath"/>
		/// </summary>
		public string LogPath
		{
			get { return pm.LogPath; }
			set { pm.LogPath = value; }
		}
	
		/// <summary>
		/// <see cref="PersistenceManager.CollisionEvent"/>
		/// </summary>
		public event CollisionHandler CollisionEvent
		{
			add { pm.CollisionEvent += value; }
			remove { pm.CollisionEvent -= value; }
		}
		/// <summary>
		/// <see cref="PersistenceManager.IdGenerationEvent"/>
		/// </summary>
		public event IdGenerationHandler IdGenerationEvent
		{
			add { pm.IdGenerationEvent += value; }
			remove { pm.IdGenerationEvent -= value; }
		}

		/// <summary>
		/// <see cref="PersistenceManager.OnSavingEvent"/>
		/// </summary>
		public event OnSavingHandler OnSavingEvent
		{
			add { pm.OnSavingEvent += value; }
			remove { pm.OnSavingEvent -= value; }
		}

		public void ClearLogfile()
		{
			pm.ClearLogfile();
		}
		
		public string DumpCache()
		{
			return pm.DumpCache();
		}

		public bool HasChanges
		{
			get { return pm.HasChanges; }
		}

		public bool HasOwnerCreatedIds
		{
			get { return pm.HasOwnerCreatedIds; }
		}

		public void LoadData(IPersistenceCapable pc)
		{
			pm.LoadData(pc);
		}

		public void LoadRelation(IPersistenceCapable pc, string fieldName, bool hollow)
		{
			pm.LoadRelation(pc, fieldName, hollow);
		}
		
//		public IList LoadRelation(IPersistenceCapable pc, Mapping.Relation r, bool hollow)
//		{
//			return pm.LoadRelation(pc, r, hollow);
//		}

		public ObjectId NewObjectId(ObjectId oidToClone, Type t)
		{
			return pm.NewObjectId(oidToClone, t);
		}

		public ObjectId NewObjectId(object keyData, Type t)
		{
			return pm.NewObjectId(keyData, t);
		}

		
		public void Restore(IPersistenceCapable pc)
		{
			pm.Restore(pc);
		}

	}
}
#endif