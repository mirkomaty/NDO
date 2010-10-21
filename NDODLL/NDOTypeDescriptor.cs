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
using System.Reflection;
using System.ComponentModel;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für NDOTypeDescriptor.
	/// </summary>
	internal class NDOTypeDescriptor
	{
		private static Hashtable collections = new Hashtable();

		private static bool IsNdoName(string name)
		{
			foreach(string s in ndoNames)
				if (name == s)
					return true;
			return false;
		}

		static string[] ndoNames = 
		{
			"NDOObjectState", 
			"NDOTimeStamp",
			"NDOHandler",
			"NDOStateManager",
			"NDOLoadState"
		};


		public static PropertyDescriptorCollection GetProperties(Type memberType)
		{
			if (memberType == null)
				return PropertyDescriptorCollection.Empty;
			PropertyDescriptorCollection pdc;
			PropertyDescriptorCollection result;

			lock(collections)
			{
				if ((pdc = (PropertyDescriptorCollection) collections[memberType]) != null)
					return (pdc);
	 
				PropertyInfo[] allProps = memberType.GetProperties();
				int l = allProps.Length;
				for (int i = 0; i < allProps.Length; i++)
				{
					PropertyInfo pi = allProps[i];
					if (IsNdoName(pi.Name))
					{
						allProps[i] = null;
						l--;
					}
				}

				PropertyDescriptor[] descriptors = new PropertyDescriptor[l];
				
				int j = 0;
				foreach(PropertyInfo pinfo in allProps)
				{
					if (pinfo != null)
					{
						descriptors[j++] = new NDOPropertyDescriptor(pinfo.Name, memberType, pinfo.PropertyType);
					}		
				}								 
				result = new PropertyDescriptorCollection(descriptors);
				collections.Add(memberType, result);
			}
			return result;			
		}
	}
}
