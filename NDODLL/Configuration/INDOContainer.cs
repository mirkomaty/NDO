using System;

namespace NDO.Configuration
{
	/// <summary>
	/// Interface for the NDOContainer
	/// </summary>
	/// <remarks>This interface and all declared methods is obsolete. Use the methods provided by IServiceContainer and IServiceFactory</remarks>
	public interface INDOContainer : IServiceContainer, IServiceFactory
	{
		/// <summary>
		/// Registers an object under it's own type
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		void RegisterInstance( object instance, string name = null );

		/// <summary>
		/// Registers a Type
		/// </summary>
		/// <param name="tFrom"></param>
		/// <param name="tTo"></param>
		/// <param name="lifetime"></param>
		[Obsolete( "Use GetInstance" )]
		void RegisterType( Type tFrom, Type tTo, ILifetime lifetime = null );
		/// <summary>
		/// Registers a Type with generic parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="lifetimeManager"></param>

		[Obsolete( "Use GetInstance" )]
		void RegisterType<T>( ILifetime lifetimeManager = null );
		/// <summary>
		/// Registers a type for a target type
		/// </summary>
		/// <typeparam name="TFrom"></typeparam>
		/// <typeparam name="TTo"></typeparam>
		/// <param name="lifetimeManager"></param>
		[Obsolete( "Use GetInstance" )]
		void RegisterType<TFrom, TTo>( ILifetime lifetimeManager = null );

		/// <summary>
		/// Resolves an object of a certain type
		/// </summary>
		/// <param name="tFrom"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[Obsolete( "Use GetInstance" )]
		object Resolve( Type tFrom, string name = null );

		/// <summary>
		/// Resolves an object of a certain type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		[Obsolete("Use GetInstance")]
		T Resolve<T>( string name = null );

		/// <summary>
		/// Resolves an object of a given type and registers a new instance, if the object is not present.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName"></param>
		/// <param name="factory"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		T ResolveOrRegisterInstance<T>( string serviceName = null, Func<T> factory = null, ILifetime lifetime = null );

		/// <summary>
		/// Resolves an object of a type. If it isn't registered, register it.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="serviceName"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		object ResolveOrRegisterType( Type t, string serviceName = null, ILifetime lifetime = null );

		/// <summary>
		/// Resolves an object of a type. If it isn't registered, register it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceName"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		T ResolveOrRegisterType<T>( string serviceName = null, ILifetime lifetime = null );

		/// <summary>
		/// Resolves a type and registers it using a factory method, if the type is not yet registered
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory"></param>
		/// <param name="serviceName"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		T ResolveOrRegisterType<T>( Func<IServiceFactory, T> factory, string serviceName = null, ILifetime lifetime = null );

		/// <summary>
		/// Creates a child container for PersistenceManager instance scope
		/// </summary>
		/// <returns></returns>
		INDOContainer CreateChildContainer();
	}
}
