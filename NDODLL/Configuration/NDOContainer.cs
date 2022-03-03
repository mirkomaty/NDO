using NDO.LightInject;
using NDO.Provider;
using NDO.Query;
using NDO.SqlPersistenceHandling;
using System;
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
		/// Creates an NDOContainer object
		/// </summary>
		public NDOContainer()
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
							instance.Register<IPersistenceHandler, SqlPersistenceHandler>();
							instance.Register<RelationContextGenerator>();
							instance.Register<IQueryGenerator, SqlQueryGenerator>();
							instance.Register<IPersistenceHandlerManager, NDOPersistenceHandlerManager>();
							instance.Register<IProviderPathFinder, NDOProviderPathFinder>();
							instance.Register<IPersistenceHandlerPool, NDOPersistenceHandlerPool>();
						}
					}
				}

				return instance;
			}
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
			return base.GetInstance( tFrom, name );
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
			return (T) base.GetInstance( typeof( T ), name );
		}

		/// <summary>
		/// Registers an instance of a given type
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		public void RegisterInstance( object instance, string name = null )
		{
			base.RegisterInstance( instance.GetType(), instance, name );
		}

		/// <summary>
		/// Tries to resolve an instance, and if it is not registered, register it and get a new instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName"></param>
		/// <param name="factory"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		public T ResolveOrRegisterInstance<T>( string serviceName = "", Func<T> factory = null, ILifetime lifetime = null )
		{
			if (lifetime == null)
				lifetime = new PerScopeLifetime();

			T result = (T) TryGetInstance( typeof( T ), serviceName );
			if (result != null)
				return result;
			if (factory == null)
				Register<T, T>( serviceName, lifetime );
			else
				Register( ( sf ) => factory(), serviceName, lifetime );
			return (T) GetInstance( typeof( T ), serviceName );
		}
	}
}
