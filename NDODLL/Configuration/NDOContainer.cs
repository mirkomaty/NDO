using NDO.Provider;
using NDO.Query;
using NDO.SqlPersistenceHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
		public void RegisterType( Type tFrom, Type tTo, string name = null, params object[] injectionMembers )
		{
			Func<Type, IResolver> valueFactory = _ => new TypeMappingResolver(this, tFrom, tTo, name);

			lock (lockObject)
			{
				this.values.AddOrUpdate( tFrom, valueFactory, ( k, o ) => valueFactory( k ) );
			}
		}

		///<inheritdoc/>
		public void RegisterType<TFrom, TTo>( string name = null, params object[] injectionMembers )
		{
			RegisterType( typeof( TFrom ), typeof( TTo ), name, injectionMembers );
		}

		///<inheritdoc/>
		public void RegisterType<T>( string name = null, params object[] injectionMembers )
		{
			RegisterType<T, T>( name, injectionMembers );
		}

		void CollectRegistrations(IDictionary<Type,IResolver> result)
		{
			foreach (var item in values)
			{
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
							instance.RegisterType<IPersistenceHandlerPool, NDOPersistenceHandlerPool>();
						}
					}
                }

                return instance;
            }
        }

		///<inheritdoc/>
		public object Resolve( Type tFrom, string name = null, params ParameterOverride[] overrides )
		{
			lock (lockObject)
			{
				if (values.TryGetValue( tFrom, out IResolver resolver ))
					return resolver.Resolve( name, overrides );
				if (parent != null)
					return parent.Resolve( tFrom, name, overrides );
			}
			return null;
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
			Func<Type, IResolver> valueFactory = _ => new InstanceResolver(name, instance);

			lock (lockObject)
			{
				this.values.AddOrUpdate( t, valueFactory, ( k, old ) => valueFactory( k ) );
			}
		}

		///<inheritdoc/>
		public void RegisterInstance<T>( T instance, string name = null )
		{
			RegisterInstance( typeof( T ), instance, name );
		}

		///<inheritdoc/>
		public object ResolveOrRegisterInstance( Type t, string name, Func<string, object> factory )
		{
			lock (lockObject)
			{
				var result = Resolve(t, name);
				if (result == null)
					RegisterInstance( t, factory( name ) );
				return Resolve( t, name );
			}
		}

		///<inheritdoc/>
		public T ResolveOrRegisterInstance<T>( string name, Func<string, T> factory )
		{
			return (T) ResolveOrRegisterInstance( typeof( T ), name, s => factory( s ) );
		}

		///<inheritdoc/>
		public object ResolveOrRegisterType( Type tFrom, Type tTo, string name = null, params ParameterOverride[] overrides )
		{
			lock (lockObject)
			{
				var result = Resolve(tFrom, name, overrides);
				if (result == null)
				{
					RegisterType( tFrom, tTo, name );
				}

				return Resolve( tFrom, name, overrides );
			}
		}

		///<inheritdoc/>
		public TFrom ResolveOrRegisterType<TFrom, TTo>( string name = null, params ParameterOverride[] overrides )
		{
			return (TFrom) ResolveOrRegisterType( typeof( TFrom ), typeof( TTo ), name, overrides );
		}

		///<inheritdoc/>
		public TFrom ResolveOrRegisterType<TFrom>( string name = null, params ParameterOverride[] overrides )
		{
			return (TFrom) ResolveOrRegisterType( typeof( TFrom ), typeof( TFrom ), name, overrides );
		}
	}
}
