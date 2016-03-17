using NDO;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NETDataObjects.NDOVSPackage
{
    class ProviderLoader
    {

        public static void AddProviderDlls()
        {
            AddProviderPlugIns(Path.GetDirectoryName(typeof(ProviderLoader).Assembly.Location));
        }

        private static void AddProviderPlugIns(string path)
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
                    if (dlllower.IndexOf("provider") == -1)
                        continue;
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
                    foreach (Type t in types)
                    {
                        if (t.IsClass && !t.IsAbstract && typeof(IProvider).IsAssignableFrom(t))
                        {
                            provider = (IProvider)Activator.CreateInstance(t);
                            if (!NDOProviderFactory.Instance.ProviderNames.Contains(provider.Name))
                                NDOProviderFactory.Instance[provider.Name] = provider;
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

    }
}
