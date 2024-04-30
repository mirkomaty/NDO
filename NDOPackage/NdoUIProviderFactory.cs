//
// Copyright (c) 2002-2022 Mirko Matytschak 
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


using NDO.UISupport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NDOVsPackage
{
    internal class NdoUIProviderFactory
    {
        private Dictionary<string, IDbUISupport> uiSupportProviders;
        private static object lockObject = new object();
        private NdoUIProviderFactory()
        {
        }

        public static NdoUIProviderFactory Instance = new NdoUIProviderFactory();
        public IDbUISupport this[string name]
        {
            get
            {
                var px = NDOProviderFactory.Instance[""];  // Make sure to fetch the providers first

                if (this.uiSupportProviders == null)
                {
                    FetchProviders();
                }

                IDbUISupport result;
                uiSupportProviders.TryGetValue(name, out result);
                return result;
            }
        }

        private void FetchProviders()
        {
            lock (lockObject)
            {
                if (this.uiSupportProviders == null) // double check
                {
                    this.uiSupportProviders = new Dictionary<string, IDbUISupport>();
                    string path = Path.GetDirectoryName(GetType().Assembly.Location);

                    var ifc = typeof(IDbUISupport);

                    foreach (var fileName in Directory.GetFiles(path, "*UISupport.dll"))
                    {
                        if (fileName.EndsWith("NDO.UISupport.dll"))
                            continue;

                        try
                        {
                            var assembly = Assembly.LoadFrom(fileName);
                            foreach (var type in assembly.ExportedTypes.Where(t => ifc.IsAssignableFrom(t)))
                            {
                                var p = (IDbUISupport)Activator.CreateInstance(type);
                                this.uiSupportProviders.Add(p.Name, p);
                                var provider = NDOProviderFactory.Instance[p.Name];
                                if (provider == null)
                                    Debug.WriteLine($"No NDO provider for UI provider {p.Name}");
                                else
                                    p.Initialize(provider);
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

        public string[] Keys
        {
            get
            {
                if (this.uiSupportProviders == null)
                    FetchProviders();

                return (from s in this.uiSupportProviders.Keys select s).ToArray();
            }
        }
    }
}
