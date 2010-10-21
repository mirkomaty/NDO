//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
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
    /// registry key HKEY_LOCAL_MACHINE\SOFTWARE\HoT - House of Tools Development GmbH\NDO\x.y where 
    /// x.y is the NDO version, i.e. 2.0.
    /// </remarks>
    public class NDOProviderFactory
    {
        private static NDOProviderFactory factory = new NDOProviderFactory();
        private ListDictionary providers = null; // Marks the providers as not loaded
        private ListDictionary generators = new ListDictionary();
        private bool skipPrivatePath;


        public ListDictionary Generators
        {
            get 
            {
                LoadProviders();
                return generators; 
            }
        }

        private void LoadProviders()
        {
            if (this.providers != null)
                return;
            this.providers = new ListDictionary();
            this["SqlServer"] = new NDOSqlProvider();
            this["Access"] = new NDOAccessProvider();
            this["Oracle"] = new NDOOracleProvider();
            SqlServerGenerator sqlGen = new SqlServerGenerator();
            sqlGen.Provider = this["SqlServer"];
            this.generators.Add("SqlServer", sqlGen);
            AccessGenerator accGen = new AccessGenerator();
            accGen.Provider = this["Access"];
            this.generators.Add("Access", accGen);
            OracleGenerator oraGen = new OracleGenerator();
            oraGen.Provider = this["Oracle"];
            this.generators.Add("Oracle", oraGen);
            SearchProviderPlugIns();
        }

        private NDOProviderFactory()
        {
        }

        private void AddProviderPlugIns(string path)
        {
            if (path == null || !Directory.Exists(path))
                return;
            string[] dlls = new string[] { };
            dlls = Directory.GetFiles(path, "*.dll");
            foreach (string dll in dlls)
            {
                try
                {
                    string dlllower = Path.GetFileName(dll.ToLower());
                    if (dlllower == "ndoenhancer.dll")
                        continue;
                    if (dlllower == "ndo.dll")
                        continue;
                    if (dlllower == "ndointerfaces.dll")
                        continue;
                    if (dlllower == "microsoft.visualstudio.vsip.helper.dll")
                        continue;
                    bool success = false;
                    Assembly ass = null;
                    if (PeTool.IsAssembly(dll))
                    {
                        try
                        {
                            ass = Assembly.LoadFrom(dll);
                            success = true;
                        }
                        catch
                        {
                            // Wrong assembly format, ignore it
                        }
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
                            if (this[provider.Name] == null)
                                this[provider.Name] = provider;
                        }
                    }
                    foreach (Type t in types)
                    {
                        if (t.IsClass && !t.IsAbstract && typeof(ISqlGenerator).IsAssignableFrom(t))
                        {
                            generator = Activator.CreateInstance(t) as ISqlGenerator;
                            if (!generators.Contains(generator.ProviderName))
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
                string path = NDOAddInPath.Instance;
                AddProviderPlugIns(path);
                if (!skipPrivatePath)
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                    AddProviderPlugIns(path);
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
                string[] result = new string[this.providers.Count];
                int i = 0;
                foreach (DictionaryEntry de in this.providers)
                {
                    result[i++] = (string)de.Key;
                }
                return result;
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
                return (IProvider)providers[name];
            }
            set 
            {
                LoadProviders();
                providers[name] = value; 
            }
        }

    }

}
