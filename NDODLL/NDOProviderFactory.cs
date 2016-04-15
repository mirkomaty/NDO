//
// Copyright (C) 2002-2013 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.de.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Cli;
using NDOInterfaces;

namespace NDO
{
    /// <summary>
    /// This singleton class is a factory for NDO providers. 
    /// </summary>
    /// <remarks>
    /// Providers implement the interface IProvider and optional the interface ISqlGenerator.
    /// All managed dlls in the NDO provider directory  and in the base directory of the running AppDomain
    /// will be scanned for implementations of the IProvider interface.
    /// Sql Server, Oracle and Access providers are built into the NDO.Dll. 
    /// All other providers reside in additional dlls. NDO finds the NDO provider directory using the
	/// file %ALLUSERSPROFILE%\Microsoft\MSEnvShared\Addins\NDOxy.AddIn where 
    /// xy is the NDO version x.y, i.e. 2.1 -> NDO21.AddIn. See also the documentation in the UserSetup directory
	/// of your NDO installation.
    /// </remarks>
    public class NDOProviderFactory
    {
		private static readonly object lockObject = new object();
        private static NDOProviderFactory factory = new NDOProviderFactory();
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
                    IProvider provider = new NDOSqlProvider();
                    if (!this.providers.ContainsKey("SqlServer"))
                        this.providers.Add("SqlServer", provider);
					SqlServerGenerator sqlGen = new SqlServerGenerator();
					sqlGen.Provider = provider;
					this.generators.Add( "SqlServer", sqlGen );
                    provider = new NDOAccessProvider();
                    if (!this.providers.ContainsKey("Access"))
                        this.providers.Add("Access", provider);
                    AccessGenerator accGen = new AccessGenerator();
					accGen.Provider = provider;
					this.generators.Add( "Access", accGen );
					SearchProviderPlugIns();
				}
			}
        }
		public IDictionary<string, ISqlGenerator> Generators
        {
            get 
            {
                LoadProviders();
                return generators; 
            }
        }


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
                    AddProviderPlugIns(path);
					string binPath = Path.Combine( path, "bin" );
					if ( Directory.Exists( binPath ) )
						AddProviderPlugIns( binPath );
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
