using NDO.Provider;
using NDO.Query;
using NDO.SqlPersistenceHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NDO.Configuration
{
	/// <summary>
	/// Application-wide IoC container
	/// </summary>
    public class NDOContainer : INDOContainer
    {
        static NDOContainer instance;
		NDOContainer parent;
		ConcurrentDictionary<Type, IResolver> values = new ConcurrentDictionary<Type, IResolver>();
		// TODO: There is a little bit too much locking in the code. This could be optimized.
		// We leave it as it is at the moment because in this context locking hardly takes any extra time.
		static object lockObject = new object();
		List<INDOContainer> children = new List<INDOContainer>();

		/// <summary>
		/// Creates an NDOContainer object
		/// </summary>
        public NDOContainer()
		{
		}

        private NDOContainer(NDOContainer parent)
		{
			this.parent = parent;
		}

		/// <inheritdoc/>
		public INDOContainer CreateChildContainer()
		{
			var child = new NDOContainer( this );
			this.children.Add( child );
			return child;
		}

		void RemoveChildContainer(INDOContainer child)
		{
			this.children.Remove( child );
		}

		///<inheritdoc/>
		public void RegisterType( Type tFrom, Type tTo, ILifetimeManager lifetimeManager = null, params object[] injectionMembers )
		{
			if (lifetimeManager == null)
				lifetimeManager = new TransientLifetimeManager();

			Func<Type, IResolver> valueFactory = _ => lifetimeManager.CreateResolver(tFrom, tTo);

			lock (lockObject)
			{
				this.values.AddOrUpdate( tFrom, valueFactory, ( k, o ) => valueFactory( k ) );
			}
		}

		///<inheritdoc/>
		public void RegisterType<TFrom, TTo>( ILifetimeManager lifetimeManager = null, params object[] injectionMembers )
		{
			RegisterType( typeof( TFrom ), typeof( TTo ), lifetimeManager, injectionMembers );
		}

		///<inheritdoc/>
		public void RegisterType<T>( ILifetimeManager lifetimeManager = null, params object[] injectionMembers )
		{
			RegisterType<T, T>( lifetimeManager, injectionMembers );
		}

		void CollectRegistrations(IDictionary<Type,IResolver> result)
		{
			foreach (var item in values)
			{
				if (!result.ContainsKey(item.Key))
					result.Add( item.Key, item.Value );
			}

			if (parent != null)
				parent.CollectRegistrations( result );
		}

		/// <summary>
		/// Returns a dictionary of all current Registrations
		/// </summary>
		public IDictionary<Type, IResolver> Registrations 
		{ 
			get 
			{
				Dictionary<Type,IResolver> result = new Dictionary<Type, IResolver>();
				lock (lockObject)
				{
					CollectRegistrations( result );
				}
				return result;
			} 
		}

		///<inheritdoc/>
		public bool IsRegistered(Type t)
		{
			return Registrations.ContainsKey( t );
		}

		///<inheritdoc/>
		public bool IsRegistered<T>()
		{
			return Registrations.ContainsKey( typeof(T) );
		}

		/// <summary>
		/// Gets the root instance of the container.
		/// </summary>
		public static INDOContainer Instance
        {
            get
            {
                if (instance == null)
                {
					lock (lockObject)
					{
						if (instance == null) // Threading double check
						{
							instance = new NDOContainer();
							instance.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
							instance.RegisterType<RelationContextGenerator>();
							instance.RegisterType<IQueryGenerator, SqlQueryGenerator>();
							instance.RegisterType<IPersistenceHandlerManager, NDOPersistenceHandlerManager>();
							instance.RegisterType<IProviderPathFinder, NDOProviderPathFinder>();
							// ContainerControlled means in this case, that there is only one instance per application, 
							// but the registration can be overriden in child containers.
							instance.RegisterType<IPersistenceHandlerPool, NDOPersistenceHandlerPool>(new ContainerControlledLifetimeManager());
						}
					}
                }

                return instance;
            }
        }

		///<inheritdoc/>
		object InternalResolve( INDOContainer resolvingContainer, Type tFrom, string name, ParameterOverride[] overrides )
		{
			lock (lockObject)
			{
				var resolver = GetResolver(tFrom);
				if (resolver == null)
				{
					RegisterType( tFrom, tFrom, null );
					resolver = GetResolver( tFrom );
				}
				return resolver.Resolve( resolvingContainer, name, overrides );
			}
		}

		///<inheritdoc/>
		public object Resolve( Type tFrom, string name = null, params ParameterOverride[] overrides )
		{
			return InternalResolve( this, tFrom, name, overrides );
		}

		///<inheritdoc/>
		public T Resolve<T>( string name = null, params ParameterOverride[] overrides )
		{
			return (T) Resolve( typeof( T ), name, overrides );
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			lock (lockObject)
			{
				if (parent != null)
					parent.RemoveChildContainer( this );

				// This scenario is very unlikely
				// Child container should always be disposed by the child.
				foreach (var child in this.children)
				{
					child.Dispose();
				}

				this.children.Clear();

				// This is the actual task of the Dispose method
				foreach (var disposable in values.Values.Where( v => v is IDisposable ).Select( v => (IDisposable) v ))
				{
					disposable.Dispose();
				}

				this.values.Clear();
			}
		}

		///<inheritdoc/>
		public void RegisterInstance( object instance, string name = null )
		{
			RegisterInstance( instance.GetType(), instance, name );
		}

		///<inheritdoc/>
		public void RegisterInstance( Type t, object instance, string name = null )
		{
			lock (lockObject)
			{
				IResolver resolver;
				if (this.values.TryGetValue( t, out resolver ))
				{
					if (resolver is InstanceResolver instanceResolver)
					{
						instanceResolver.AddOrUpdate( name, instance );
						return;
					}
					else
					{
						this.values.TryRemove( t, out resolver );
					}
				}

				var instResolver = new InstanceResolver();
					this.values.TryAdd( t, instResolver );
				instResolver.AddOrUpdate( name, instance );
			}
		}

		///<inheritdoc/>
		public void RegisterInstance<T>( T instance, string name = null )
		{
			RegisterInstance( typeof( T ), instance, name );
		}

		IResolver GetResolver(Type t)
		{
			if (this.values.TryGetValue( t, out IResolver resolver ))
				return resolver;
			if (this.parent != null)
				return parent.GetResolver( t );
			return null;
		}

		///<inheritdoc/>
		public object ResolveOrRegisterInstance( Type t, string name, Func<string, object> factory )
		{
			lock (lockObject)
			{
				var resolver = (InstanceResolver)GetResolver(t);
				if (resolver == null)
				{
					resolver = new InstanceResolver();
					values.TryAdd( t, resolver );
				}

				var result = resolver.Resolve(this, name, null);
				if (result == null)
				{
					result = factory( name );
					resolver.AddOrUpdate( name, result );
				}
				return result;
			}
		}

		///<inheritdoc/>
		public T ResolveOrRegisterInstance<T>( string name, Func<string, T> factory )
		{
			return (T) ResolveOrRegisterInstance( typeof( T ), name, s => factory( s ) );
		}

		///<inheritdoc/>
		public object ResolveOrRegisterType( Type tFrom, Type tTo, ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides )
		{
			lock (lockObject)
			{
				var resolver = GetResolver(tFrom);
				if (resolver != null)
					return resolver.Resolve( this, name, overrides );
				
				RegisterType( tFrom, tTo, lifetimeManager );

				return Resolve( tFrom, name, overrides );
			}
		}

		///<inheritdoc/>
		public TFrom ResolveOrRegisterType<TFrom, TTo>( ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides )
		{
			return (TFrom) ResolveOrRegisterType( typeof( TFrom ), typeof( TTo ), lifetimeManager, name, overrides );
		}

		///<inheritdoc/>
		public TFrom ResolveOrRegisterType<TFrom>( ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides )
		{
			return (TFrom) ResolveOrRegisterType( typeof( TFrom ), typeof( TFrom ), lifetimeManager, name, overrides );
		}

		///<inheritdoc/>
		public TFrom ResolveOrRegisterType<TFrom>( string name, params ParameterOverride[] overrides )
		{
			return (TFrom) ResolveOrRegisterType( typeof( TFrom ), typeof( TFrom ), null, name, overrides );
		}
	}
}
