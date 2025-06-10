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

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NDO.ProviderFactory;
using NDO.Query;
using NDO.SqlPersistenceHandling;

namespace NDO.Application
{
	/// <summary>
	/// Startup extensions to integrate NDO into the Microsoft .NET application architecture
	/// </summary>
	public static class BuilderExtensions
	{
		/// <summary>
		/// Call this method in your Startup class, so that NDO has a chance to register required services.
		/// </summary>
		/// <remarks>
		/// <code>
		/// public void ConfigureServices(IServiceCollection services)
		/// {
		///		...
		///		services.AddNdo(_env,_config);
		///		...
		/// }
		/// </code>
		/// </remarks>
		/// <param name="services"></param>
		/// <param name="hostEnvironment"></param>
		/// <param name="config"></param>
		public static void AddNdo( this IServiceCollection services, IHostEnvironment hostEnvironment, IConfiguration config )
		{
			services.AddNdoProviderFactory( hostEnvironment, config );
			NDOApplication.Configuration = config;
			NDOApplication.HostEnvironment = hostEnvironment;
			services.AddTransient<IPersistenceHandler, SqlPersistenceHandler>();
			services.AddTransient<RelationContextGenerator>();
			services.AddTransient<IQueryGenerator, SqlQueryGenerator>();
			services.AddScoped<IPersistenceHandlerManager, NDOPersistenceHandlerManager>();
			services.AddSingleton<IPersistenceHandlerPool, NDOPersistenceHandlerPool>();
			services.AddScoped<IMappingsAccessor, MappingsAccessor>();
			services.AddTransient<INDOTransactionScope, NDOTransactionScope>();
			services.AddScoped<IPersistenceManagerAccessor, PersistenceManagerAccessor>();
		}

		/// <summary>
		/// Call this method in your Startup class to give NDO the chance to use the IServiceProvider interface
		/// </summary>
		/// <remarks>
		/// In the Startup class of a web project write
		/// <code>
		/// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		/// {
		///		...
		///		app.ApplicationServices.UseNdo();
		///		...
		/// }
		/// </code>
		/// If you have an IHostBuilder interface use this code:
		/// <code>
		/// var builder = Host.CreateDefaultBuilder();
		/// ...
		/// builder.ConfigureServices( services =>
		/// {
		///		services.UseNdo();
		/// } );
		/// </code>
		/// With CreateApplicationBuilder use this code
		/// <code>
		/// var builder = Host.CreateApplicationBuilder();
		/// ...
		/// var services = builder.Services;
		/// services.UseNdo();
		/// </code>
		/// </remarks>
		/// <param name="serviceProvider"></param>
		/// <returns>The IServiceProvider instance, passed as parameter.</returns>
		public static IServiceProvider UseNdo( this IServiceProvider serviceProvider )
		{
			// Initializes the internal class NDOApplication of the ProviderFactory
			serviceProvider.UseNdoProviderFactory();
			// Initializes the internal class NDOApplication of NDO.dll.
			NDOApplication.ServiceProvider = serviceProvider;
			return serviceProvider;
		}
	}
}
