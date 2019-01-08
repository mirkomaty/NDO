using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NETDataObjects.NDOVSPackage
{
    class NDOProviderFactory
    {
        private Dictionary<string, IProvider> ndoProviders;
        private static object lockObject = new object();
        private NDOProviderFactory()
        {
        }

        public static NDOProviderFactory Instance = new NDOProviderFactory();
        public IProvider this[string name]
        {
            get
            {
                if (this.ndoProviders == null)
                {
                    lock (lockObject)
                    {
                        if (this.ndoProviders == null) // double check
                        {
                            this.ndoProviders = new Dictionary<string, IProvider>();
                            string path = Path.GetDirectoryName(GetType().Assembly.Location);

                            var ndoAssembly = Assembly.LoadFrom(Path.Combine(path, "NDO.dll"));
                            var sqlp = (IProvider)Activator.CreateInstance(ndoAssembly.GetType("NDO.NDOSqlServerProvider"));
                            this.ndoProviders.Add(sqlp.Name, sqlp);

                            var ifc = typeof(IProvider);

                            foreach (var fileName in Directory.GetFiles(path, "*Provider.dll"))
                            {
                                try
                                {
                                    var assembly = Assembly.LoadFrom(fileName);
                                    foreach (var type in assembly.ExportedTypes)
                                    {
                                        if (ifc.IsAssignableFrom(type))
                                        {
                                            var p = (IProvider)Activator.CreateInstance(type);
                                            this.ndoProviders.Add(p.Name, p);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(fileName + ": " + ex.ToString());
                                }
                            }
                        }
                    }
                }

                IProvider result = null;
                ndoProviders.TryGetValue(name, out result);
                return result;
            }
        }
    }
}
