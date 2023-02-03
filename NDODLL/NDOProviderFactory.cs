//
// Copyright (c) 2002-2016 Mirko Matytschak 
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
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using NDOInterfaces;
using System.Linq;
using NDO.Provider;
using NDO.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NDO
{
    /// <summary>
    /// This singleton class is a factory for NDO providers and generators. 
    /// </summary>
    /// <remarks>
    /// Providers implement the interface IProvider. Generators implement ISqlGenerator.
    /// All managed dllsin the base directory of the running AppDomain
    /// will be scanned for implementations of the IProvider interface.
    /// </remarks>
    public class NDOProviderFactory
    {
		private static readonly object lockObject = new object();
        private static readonly NDOProviderFactory factory = new NDOProviderFactory();
        private Dictionary<string,IProvider> providers = null; // Marks the providers as not loaded
		private Dictionary<string,ISqlGenerator> generators = new Dictionary<string,ISqlGenerator>();

        /// <summary>
        /// Gets or Sets the object which helps finding directories with NDOProviders.
        /// </summary>
        public static IProviderPathFinder ProviderPathFinder { get; set; }

        static NDOProviderFactory()
        {
            try
            {
                if (NDOApplication.ServiceProvider != null)
                    ProviderPathFinder = NDOApplication.ServiceProvider.GetService<IProviderPathFinder>();
                else
                    ProviderPathFinder = new NDOProviderPathFinder();
			}
			catch // We try to continue here, even if we don't have the DependencyInjection available.
            {
				ProviderPathFinder = new NDOProviderPathFinder();
			}
		}

		/// <summary>
		/// Private constructor makes sure, that only one object can be instantiated.
		/// </summary>
		private NDOProviderFactory()
        {
        }

		/// <summary>
		/// Actual initialization of the factory.
		/// This allows lazy instantiation.
		/// </summary>
        private void LoadProviders()
        {
			lock (lockObject)
			{				
				if (this.providers == null)  // Multithreading DoubleCheck
				{
					this.providers = new Dictionary<string, IProvider>();
					SearchProviderPlugIns();
				}
			}
        }
		
        /// <summary>
        /// Gets all Sql DDL generators
        /// </summary>
        public IDictionary<string, ISqlGenerator> Generators
        {
            get 
            {
                LoadProviders();
                return generators; 
            }
        }

        /// <summary>
        /// Adds provider dlls by path.
        /// </summary>
        /// <param name="path"></param>
        /// <remarks>In most cases the providers are detected automatically.</remarks>
        public void AddProviderPlugIns(string path)
        {
            if (path == null || !Directory.Exists(path))
                return;
            string[] dlls = new string[] { };
            dlls = Directory.GetFiles(path, "NDO.*.dll");
            foreach (string dll in dlls)
            {
                try
                {
                    bool success = false;
                    Assembly ass = null;
                    try
                    {
                        ass = Assembly.LoadFrom(dll);
                        success = true;
                    }
                    catch
                    {
                        // Wrong assembly format, ignore it
                    }
                    if (!success)
                        continue;

                    Type[] types = new Type[0];  // In case we get an error while loading the types
                    try
                    {
                        types = ass.GetExportedTypes();
                    }
                    catch { }
                    IProvider provider;
                    ISqlGenerator generator;
                    foreach (Type t in types)
                    {
                        if (t.IsClass && !t.IsAbstract && t.GetInterface("IProvider") != null)
                        {
                            provider = (IProvider)Activator.CreateInstance(t);
							if (!this.providers.ContainsKey( provider.Name ))
								this.providers.Add( provider.Name, provider );
                        }
                    }
                    foreach (Type t in types)
                    {
                        if (t.IsClass && !t.IsAbstract && typeof(ISqlGenerator).IsAssignableFrom(t))
                        {
                            generator = Activator.CreateInstance(t) as ISqlGenerator;
                            if (!generators.ContainsKey(generator.ProviderName))
                            {
                                this.generators.Add(generator.ProviderName, generator);
                                generator.Provider = this[generator.ProviderName];
                            }
                        }
                    }
                }
                catch //(Exception ex)
                {
                    // string s = ex.Message;
                    // Simply don't add the Plugin
                }
            }
        }

        private void SearchProviderPlugIns()
        {
            var serviceProvider = NDOApplication.ServiceProvider;

			try
            {
                if (ProviderPathFinder != null)
                {
                    foreach (var path in ProviderPathFinder.GetPaths())
                    {
                        AddProviderPlugIns( path );
                    }
                }
            }
            catch (Exception ex)
            {
                // Simply don't add any Plugins
                if (serviceProvider != null) // If NDOProviderFactory is used from the Enhancer, no ServiceProvider exists.
                {
                    var logger = serviceProvider.GetService<ILogger<NDOProviderFactory>>();
                    if (logger != null)  // Could be null during tests
                        logger.LogError( ex, nameof( SearchProviderPlugIns ) );
                }
            }
        }

        /// <summary>
        /// Gets the names of all installed NDO providers.
        /// </summary>
        public string[] ProviderNames
        {
            get
            {
                LoadProviders();
                lock(lockObject)
                {
                    string[] result = new string[this.providers.Count];
                    int i = 0;
                    foreach (string key in this.providers.Keys)
                    {
                        result[i++] = key;
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Get the global provider factory.
        /// </summary>
        public static NDOProviderFactory Instance
        {
            get
            {
                return factory;
            }
        }

        /// <summary>
        /// Tries to get a provider with the given name. If the provider doesn't exist, the first available provider is used.
        /// </summary>
        /// <remarks>
        /// This method ist used by the enhancer. Use NDOProviderFactory.Instance[providerName] in your application code.
        /// In most cases it is sufficient to use the first available provider, because only one provider is used.
        /// </remarks>
        /// <param name="name"></param>
        /// <returns></returns>
        public IProvider GetProviderOrDefault(string name)
        {
            LoadProviders();
            if (providers.ContainsKey( name ))
                return (IProvider) providers[name];
            var provider = providers.Values.FirstOrDefault();
            if (provider == null)
                throw new Exception( "There is no provider available. Please install at least one NDOProvider." );
            return provider;
        }


        /// <summary>
        /// Get or add a data provider.
        /// </summary>
        public IProvider this[string name]
        {
            get
            {
                LoadProviders();
				if (!providers.ContainsKey(name))
				{
					throw new Exception( String.Format( "There is no provider defined with name '{0}'. Check your mapping file.", name ) );
				}
                return (IProvider)providers[name];
            }
            set 
            {
				LoadProviders();
                lock(lockObject)
                {
                    if (providers.ContainsKey(name))
                        providers.Remove(name);
                    providers[name] = value;
                    foreach (Type t in value.GetType().Assembly.GetExportedTypes())
                    {
                        if (t.IsClass && !t.IsAbstract && typeof(ISqlGenerator).IsAssignableFrom(t))
                        {
                            var generator = Activator.CreateInstance(t) as ISqlGenerator;
                            if (!generators.ContainsKey(generator.ProviderName))
                            {
                                this.generators.Add(generator.ProviderName, generator);
                                generator.Provider = value;
                            }
                        }
                    }
                }
            }
        }

    }

}
