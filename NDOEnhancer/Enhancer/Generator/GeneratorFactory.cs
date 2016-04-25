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
using System.Collections;
using System.Collections.Specialized;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für GeneratorFactory.
	/// </summary>
	internal class GeneratorFactory
	{
		public static GeneratorFactory Instance = new GeneratorFactory();
		ListDictionary generators = new ListDictionary();

		private GeneratorFactory()
		{
			//TODO: Diese Logik muss überprüft werden
#if STANDALONE
			foreach (Type t in this.GetType().Assembly.GetTypes())
			{
				if (t.IsClass && !t.IsAbstract && t.Name != "StandardSqlGenerator" && (typeof(ISqlGenerator)).IsAssignableFrom(t))
				{
					try
					{
						ISqlGenerator gen = (ISqlGenerator) Activator.CreateInstance(t);
						gen.Provider = NDOProviderFactory.Instance[gen.ProviderName];
						generators.Add(gen.ProviderName, gen);
					}
					catch {}
				}
			}
			foreach(string name in NDOProviderFactory.Instance.ProviderNames)
			{
				foo (this[name]);
			}
#else
            foreach (string name in NDOProviderFactory.Instance.ProviderNames)
            {
                generators.Add(name, null);
            }
#endif
		}

#if STANDALONE
		// use dummy ISqlGenerator in a function call to avoid 
		// optimization
		protected virtual void foo(ISqlGenerator bar)
		{
		}
#endif

		public ISqlGenerator this[string providerName]
		{
			get 
			{ 
				ISqlGenerator result = null;
				if (!generators.Contains(providerName))
				{
					result = NDOProviderFactory.Instance.Generators[providerName] as ISqlGenerator;
					if (result != null)
					{
						IProvider p = NDOProviderFactory.Instance[result.ProviderName];
						if (p == null)
							p = NDOProviderFactory.Instance["Oracle"];  // Standard quoting
						result.Provider = p;
						generators.Add(result.ProviderName, result);
					}
				}
				else
					result = (ISqlGenerator) generators[providerName];
#if STANDALONE
				if (result == null)
				{
					IProvider p = NDOProviderFactory.Instance[providerName];
					if (p == null)
						p = NDOProviderFactory.Instance["Oracle"];  // Standard quoting
					result = new StandardSqlGenerator();
					((StandardSqlGenerator)result).Provider = p;
					((StandardSqlGenerator)result).SetProviderName(providerName);
				}
#endif
				return result;
			}
		}

		public string[] ProviderNames
		{
			get 
			{
				string[] result = new string[generators.Count];
				int i = 0;
				foreach(DictionaryEntry de in generators)
				{
					result[i++] = (string) de.Key;
				}
				return result;
			}
		}

	}
}
