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
