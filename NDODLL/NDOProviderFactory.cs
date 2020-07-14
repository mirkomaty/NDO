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
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Cli;
using NDOInterfaces;
using System.Runtime.Versioning;
using System.Linq;

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
        private bool skipPrivatePath;

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
            dlls = Directory.GetFiles(path, "*Provider.dll");
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
                        if (t.IsClass && !t.IsAbstract && typeof(IProvider).IsAssignableFrom(t))
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
            try
            {
				//string path = NDOAddInPath.Instance;
				//AddProviderPlugIns(path);
				if (!skipPrivatePath)
				{
					string path = AppDomain.CurrentDomain.BaseDirectory;
					AddProviderPlugIns( path );
					string binPath = Path.Combine( path, "bin" );
					if (string.Compare( path, binPath, true ) != 0 && Directory.Exists( binPath ))
						AddProviderPlugIns( binPath );

					// This is a hack to determine any loaded provider packages.
					var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

					if (runtimeDir != null && runtimeDir.IndexOf( "Microsoft.NETCore.App" ) > -1)
					{
						path = typeof( PersistenceManager ).Assembly.Location;
						var pattern = $".nuget{Path.DirectorySeparatorChar}packages";
						int p;
						if ((p = path.IndexOf( pattern )) > -1)
						{
							p += pattern.Length;
							path = path.Substring( 0, p );
							var majorVersion = GetType().Assembly.GetName().Version.Major.ToString();
							var standardVersion = "netstandard2.0";

							foreach (var subPath in Directory.GetDirectories( path, "ndo.*" ))
							{
								if (Path.GetFileName( subPath ) == "ndo.dll")
									continue;
								var versionDir = Directory.GetDirectories( subPath ).FirstOrDefault( s => Path.GetFileName( s ).StartsWith( majorVersion) );
								if (versionDir == null)
									continue;
								var libDir = Path.Combine( versionDir, "lib", standardVersion );
								if (!Directory.Exists( libDir ))
									continue;
								AddProviderPlugIns( libDir );
							}
						}
					}
				}
            }
            catch
            {
                // Simply don't add the Plugins
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
        /// If set to true, NDO doesn't search for NDO providers in the base directory of the app domain. 
        /// If you use a built-in provider like SqlServer, Access of Oracle, this option can speed up the 
        /// PersistenceManager creation.
        /// </summary>
        public bool SkipPrivatePath
        {
            get { return skipPrivatePath; }
            set { skipPrivatePath = value; }
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
