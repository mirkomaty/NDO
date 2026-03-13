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
using NDOInterfaces;

namespace NDO.ProviderFactory
{
    /// <summary>
    /// Startup extensions to integrate NDO into the Microsoft .NET application architecture
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// This method is called by NDO.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostEnvironment"></param>
        /// <param name="config"></param>
        public static void AddNdoProviderFactory( this IServiceCollection services, IHostEnvironment hostEnvironment, IConfiguration config )
        {
            NDOApplication.Configuration = config;
            NDOApplication.HostEnvironment = hostEnvironment;
            services.AddSingleton<IProviderPathFinder, NDOProviderPathFinder>();
            services.AddSingleton<INDOProviderFactory>( NDOProviderFactory.Instance );
        }

        /// <summary>
        /// This method is called by NDO.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IServiceProvider UseNdoProviderFactory( this IServiceProvider serviceProvider )
        {
            NDOApplication.ServiceProvider = serviceProvider;
            return serviceProvider;
        }
    }
}
