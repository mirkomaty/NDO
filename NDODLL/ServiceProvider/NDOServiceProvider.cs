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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDO.ProviderFactory;
using NDO.SqlPersistenceHandling;
using NDOInterfaces;
using System;
using System.Collections.Generic;

namespace NDO.Application
{
	/// <summary>
	/// Minimal IServiceProvider implementation, which is used, if the application
	/// doesn't set up a host and doesn't call AddNdo and UseNdo.
	/// </summary>
	/// <remarks>
	/// All registered services are transient, like the services registered in BuilderExtensions.AddNdo.
	/// The provider acts as its own scope factory, so that IServiceProvider.CreateScope() works.
	/// Logging services are provided by the NDOConsoleLogger.
	/// </remarks>
	internal class NDOServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope
	{
		private readonly Dictionary<Type, Func<IServiceProvider, object>> factories = new Dictionary<Type, Func<IServiceProvider, object>>();

		public NDOServiceProvider()
		{
			factories.Add( typeof( IPersistenceHandler ), sp => new SqlPersistenceHandler( sp ) );
			factories.Add( typeof( IQueryGenerator ), sp => new SqlQueryGenerator() );
			factories.Add( typeof( INDOTransactionScope ), sp => new NDOTransactionScope() );
			factories.Add( typeof( ILoggerFactory ), sp => NDOConsoleLoggerFactory.Instance );
			factories.Add( typeof( IServiceProvider ), sp => sp );
			factories.Add( typeof( IServiceScopeFactory ), sp => sp );
			factories.Add( typeof( IProviderPathFinder ), sp => new NDOProviderPathFinder() );
			factories.Add( typeof( INDOProviderFactory ), sp => NDOProviderFactory.Instance );
		}

		/// <inheritdoc/>
		public object GetService( Type serviceType )
		{
			if (serviceType == null)
				throw new ArgumentNullException( nameof( serviceType ) );

			if (factories.TryGetValue( serviceType, out var factory ))
				return factory( this );

			if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof( ILogger<> ))
				return Activator.CreateInstance( typeof( NDOConsoleLogger<> ).MakeGenericType( serviceType.GetGenericArguments() ) );

			return null;
		}

		/// <inheritdoc/>
		public IServiceScope CreateScope()
		{
			// There are no scoped services, so the provider itself can serve as scope.
			return this;
		}

		IServiceProvider IServiceScope.ServiceProvider => this;

		void IDisposable.Dispose()
		{
			// Nothing to dispose. The provider lives as long as the application.
		}
	}
}
