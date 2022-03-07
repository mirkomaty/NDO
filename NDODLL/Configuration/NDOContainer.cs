using NDO.Provider;
using NDO.Query;
using NDO.SqlPersistenceHandling;
using System;
using System.Diagnostics;
using System.Linq;

namespace NDO.Configuration
{
	/// <summary>
	/// Application-wide IoC container
	/// </summary>
	public class NDOContainer : ServiceContainer, IDisposable, INDOContainer
	{
		static NDOContainer instance;
		static object lockObject = new object();
		Scope rootScope;

		/// <summary>
		/// Gets the root instance of the container.
		/// </summary>
		public static NDOContainer Instance
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
							// IPersistenceHandlers are cached individually for each type. So it's safe to create a new object with every resolve.
							instance.Register<IPersistenceHandler, SqlPersistenceHandler>( new TransientLifetime() );
							instance.Register<Mappings, RelationContextGenerator>( ( factory, mappings ) => new RelationContextGenerator( mappings ) );
							instance.Register<IQueryGenerator, SqlQueryGenerator>( new TransientLifetime() );
							instance.Register<IPersistenceHandlerManager, NDOPersistenceHandlerManager>();
							instance.Register<IProviderPathFinder, NDOProviderPathFinder>();
							instance.Register<IPersistenceHandlerPool, NDOPersistenceHandlerPool>();
							instance.Register<INDOTransactionScope, NDOTransactionScope>();
							instance.RegisterInstance<INDOContainer>( instance );
						}
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Creates an NDOContainer object
		/// </summary>
		public NDOContainer() : base( new ContainerOptions
			{
				EnablePropertyInjection = false,
				LogFactory = t => msg => Debug.WriteLine( $"{msg.Level}: {msg.Message}" )
			}
		)
		{
			SetDefaultLifetime<PerScopeLifetime>();
			rootScope = BeginScope();
		}

		void IDisposable.Dispose()
		{
			rootScope.Dispose();
			base.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tFrom"></param>
		/// <param name="tTo"></param>
		/// <param name="lifetime"></param>
		[Obsolete( "Use Register()" )]
		public void RegisterType( Type tFrom, Type tTo, ILifetime lifetime = null )
		{
			base.Register( tFrom, tTo, lifetime );
		}


		/// <summary>
		/// Obsolete
		/// </summary>
		/// <typeparam name="TFrom"></typeparam>
		/// <typeparam name="TTo"></typeparam>
		/// <param name="lifetimeManager"></param>
		[Obsolete( "Use Register()" )]
		public void RegisterType<TFrom, TTo>( ILifetime lifetimeManager = null )
		{
			RegisterType( typeof( TFrom ), typeof( TTo ), lifetimeManager );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="lifetimeManager"></param>
		[Obsolete( "Use Register()" )]
		public void RegisterType<T>( ILifetime lifetimeManager = null )
		{
			RegisterType<T, T>( lifetimeManager );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tFrom"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[Obsolete( "Use GetInstance" )]
		public object Resolve( Type tFrom, string name = null )
		{
			if (name != null)
				return base.GetInstance( tFrom, name );
			else
				return base.GetInstance( tFrom );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		[Obsolete( "Use GetInstance" )]
		public T Resolve<T>( string name = null )
		{
			if (name != null)
				return (T) base.GetInstance( typeof( T ), name );
			else
				return (T) base.GetInstance( typeof( T ) );
		}

		/// <summary>
		/// Registers an instance of a given type
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		public void RegisterInstance( object instance, string name = null )
		{
			if (name != null)
				base.RegisterInstance( instance.GetType(), instance, name );
			else
				base.RegisterInstance( instance.GetType(), instance );
		}

		/// <summary>
		/// Tries to resolve an instance, and if it is not registered, register it and get a new instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName"></param>
		/// <param name="factory"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		public T ResolveOrRegisterInstance<T>( string serviceName = null, Func<T> factory = null, ILifetime lifetime = null )
		{
			if (lifetime == null)
				lifetime = new PerScopeLifetime();

			T result;
			if (serviceName == null)
				result = (T) TryGetInstance( typeof( T ) );
			else
				result = (T) TryGetInstance( typeof( T ), serviceName );

			if (result != null)
				return result;
			if (factory == null)
			{
				if (serviceName == null)
					Register<T, T>( lifetime );
				else
					Register<T, T>( serviceName, lifetime );
			}
			else
			{
				if (serviceName == null)
					Register( ( sf ) => factory(), lifetime );
				else
					Register( ( sf ) => factory(), serviceName, lifetime );
			}

			if (serviceName == null)
				return (T) GetInstance( typeof( T ) );
			else
				return (T) GetInstance( typeof( T ), serviceName );
		}

		/// <summary>
		/// Resolves an object of a type. If it isn't registered, register it.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="serviceName"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		public object ResolveOrRegisterType( Type t, string serviceName = null, ILifetime lifetime = null )
		{
			if (lifetime == null)
				lifetime = new PerScopeLifetime();

			object result;
			if (serviceName == null)
				result = TryGetInstance( t );
			else
				result = TryGetInstance( t, serviceName );

			if (result != null)
				return result;
			if (serviceName == null)
				Register( t, lifetime );
			else
				Register( t, t, serviceName, lifetime );

			if (serviceName == null)
				return GetInstance( t );
			else
				return GetInstance( t, serviceName );
		}

		/// <summary>
		/// Resolves an object of a type. If it isn't registered, register it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		public T ResolveOrRegisterType<T>( string serviceName = null, ILifetime lifetime = null )
		{
			return (T) ResolveOrRegisterType( typeof( T ), serviceName, lifetime );
		}

	}
}
