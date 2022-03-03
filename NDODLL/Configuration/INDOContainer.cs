using NDO.LightInject;
using System;

namespace NDO.Configuration
{
	public interface INDOContainer
	{
		void RegisterInstance( object instance, string name = null );
		void RegisterType( Type tFrom, Type tTo, ILifetime lifetime = null );
		void RegisterType<T>( ILifetime lifetimeManager = null );
		void RegisterType<TFrom, TTo>( ILifetime lifetimeManager = null );
		object Resolve( Type tFrom, string name = null );
		T Resolve<T>( string name = null );
		T ResolveOrRegisterInstance<T>( string serviceName = "", Func<T> factory = null, ILifetime lifetime = null );
	}
}