using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public interface INDOContainer : IDisposable
	{
		/// <summary>
		/// A child container shares the parent's configuration, but can be configured with different settings or lifetime.
		/// </summary>
		/// <returns></returns>
		INDOContainer CreateChildContainer();

		/// <summary>
		/// Registers a type mapping with the container.
		/// </summary>
		/// <param name="fFrom"></param>
		/// <param name="tTo"></param>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="injectionMembers">This parameter is currently unused</param>
		void RegisterType( Type fFrom, Type tTo, ILifetimeManager lifetimeManager = null, params object[] injectionMembers );

		/// <summary>
		/// Registers a type mapping with the container.
		/// </summary>
		/// <typeparam name="TFrom"></typeparam>
		/// <typeparam name="TTo"></typeparam>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="injectionMembers">This parameter is currently unused</param>
		void RegisterType<TFrom, TTo>( ILifetimeManager lifetimeManager = null, params object[] injectionMembers);

		/// <summary>
		/// Registers a type mapping with the container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <remarks>The key type T is identical to the result type.</remarks>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="injectionMembers">This parameter is currently unused</param>
		void RegisterType<T>( ILifetimeManager lifetimeManager = null, params object[] injectionMembers );

		/// <summary>
		/// Resolves an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="overrides"></param>
		T Resolve<T>( string name = null, params ParameterOverride[] overrides );

		/// <summary>
		/// Resolves an object
		/// </summary>
		/// <param name="tFrom"></param>
		/// <param name="name"></param>
		/// <param name="overrides"></param>
		/// <returns>The resolved object</returns>
		object Resolve( Type tFrom, string name = null, params ParameterOverride[] overrides );

		/// <summary>
		/// Gets all registrations of a container
		/// </summary>
		IDictionary<Type, IResolver> Registrations { get; }

		/// <summary>
		/// Resolves an object of a given type and registers a new instance, if the object is not present.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="name"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		object ResolveOrRegisterInstance(Type t, string name, Func<string, object> factory );

		/// <summary>
		/// Resolves an object of a given type and registers a new instance, if the object is not present.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		T ResolveOrRegisterInstance<T>( string name, Func<string, T> factory );

		/// <summary>
		/// Resolves an object of a given type and adds a new registration, if the registration is not present.
		/// </summary>
		/// <param name="tFrom">The registration type</param>
		/// <param name="tTo">The result type</param>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="name">An optional name</param>
		/// <param name="overrides">Optional constructor parameter overrides</param>
		/// <returns></returns>
		object ResolveOrRegisterType( Type tFrom, Type tTo, ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides );

		/// <summary>
		/// Resolves an object of a given type and adds a new registration, if the registration is not present.
		/// </summary>
		/// <typeparam name="TFrom">The registration type</typeparam>
		/// <typeparam name="TTo">The result type</typeparam>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="name">An optional name</param>
		/// <param name="overrides">Optional constructor parameter overrides</param>
		/// <returns></returns>
		TFrom ResolveOrRegisterType<TFrom,TTo>( ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides );

		/// <summary>
		/// Resolves an object of a given type and adds a new registration, if the registration is not present.
		/// </summary>
		/// <typeparam name="TFrom">The registration and result type</typeparam>
		/// <param name="lifetimeManager">A lifetime manager controls the lifetime of the generated objects. Default is the TransientLifetimeManager.</param>
		/// <param name="name">An optional name</param>
		/// <param name="overrides">Optional constructor parameter overrides</param>
		/// <returns></returns>
		TFrom ResolveOrRegisterType<TFrom>( ILifetimeManager lifetimeManager = null, string name = null, params ParameterOverride[] overrides );

		/// <summary>
		/// Resolves an object of a given type and adds a new registration, if the registration is not present.
		/// </summary>
		/// <typeparam name="TFrom">The registration and result type</typeparam>
		/// <param name="name">An registration name</param>
		/// <param name="overrides">Optional constructor parameter overrides</param>
		/// <returns></returns>
		TFrom ResolveOrRegisterType<TFrom>( string name, params ParameterOverride[] overrides );

		/// <summary>
		/// Registers an object under it's own type
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		void RegisterInstance( object instance, string name = null );

		/// <summary>
		/// Registers an object as a certain type
		/// </summary>
		/// <param name="t"></param>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		void RegisterInstance( Type t, object instance, string name = null );

		/// <summary>
		/// Registers an object as a certain type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		void RegisterInstance<T>( T instance, string name = null );

	}
}
