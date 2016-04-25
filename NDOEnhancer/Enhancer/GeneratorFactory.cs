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

		Hashtable ht = new Hashtable(5);

		private GeneratorFactory()
		{	
			foreach (Type t in this.GetType().Assembly.GetTypes())
			{
				if (t.IsClass && !t.IsAbstract && (typeof(ISqlGenerator)).IsAssignableFrom(t))
				{
					try
					{
						ISqlGenerator gen = (ISqlGenerator) Activator.CreateInstance(t);
						if (NDOProviderFactory.Instance[gen.ProviderName] != null)
						{
							ht.Add(gen.ProviderName, gen);
						}
					}
					catch {}
				}
			}
		}

		public ISqlGenerator this[string providerName]
		{
			get { return (ISqlGenerator) ht[providerName]; }
		}

		public string[] ProviderNames
		{
			get 
			{
				string[] result = new string[ht.Count];
				int i = 0;
				foreach(DictionaryEntry de in ht)
				{
					result[i++] = (string) de.Key;
				}
				return result;
			}
		}

	}
}
