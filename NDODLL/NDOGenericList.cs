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


#if dontNeedThisListAnymore
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace NDO
{
	/// <summary>
	/// This class derives from ArrayList to support
	/// Windows Forms Data Binding. Windows Forms can 
	/// obtain property information for the members of the list
	/// even if the list is empty.
	/// </summary>
	[Serializable]
	public class NDOGenericList<T> : List<T>, ITypedList, INDOTypedList
	{
		Type memberType = typeof(T);

		//Type oidKeyType = null;
		//string oidHeaderName = null;

		///// <summary>
		///// Gets a read only wrapper object for the list.
		///// </summary>
		///// <param name="list">Object which implements IList</param>
		///// <returns>IList or ArrayList implementation of the wrapper.</returns>
		//public static IList ReadOnly(IList list)
		//{
		//    return ArrayList.ReadOnly(list);
		//}

		/// <summary>
		/// Gets a read only wrapper object for the list.
		/// </summary>
		/// <param name="list">Object which implements IList</param>
		/// <returns>IList or ArrayList implementation of the wrapper.</returns>
		public static IList<T> ReadOnly(IList<T> list)
		{
			return new NDOReadOnlyGenericList<T>(list);
		}


		/// <summary>
		/// Create a list for a certain type
		/// </summary>
		public NDOGenericList()
		{
		}


		/// <summary>
		/// Create a list for a certain type
		/// </summary>
		/// <param name="count">Initial size of the list.</param>
		public NDOGenericList(int count)
			: base(count)
		{
		}


		/// <summary>
		/// Creates a NDOGenericList and initializes it with the elements given in a collection.
		/// </summary>
		/// <param name="coll">The list will be initialized with elements in coll.</param>
		/// <remarks>
		/// This constructor bridges between the old non-generic containers 
		/// and the generic containers, introduced with .NET 2.0.
		/// </remarks>
		public NDOGenericList(IList coll)
		{
			T[] arr = new T[coll.Count];
			for (int i = 0; i < coll.Count; i++)
				arr[i] = (T)coll[i];
			base.AddRange(arr);
		}


		/// <summary>
		/// Generates a new list with the same elements.
		/// </summary>
		/// <returns>The new NDOGenericList.</returns>
		public object Clone()
		{
			NDOGenericList<T> l = new NDOGenericList<T>();
			l.AddRange(this);
			return l;
		}

		#region Implementation of ITypedList
		/// <summary>
		/// See documemtation for ITypedList
		/// </summary>
		/// <param name="listAccessors">Parameter will be provided by Windows Forms binding infrastructure</param>
		/// <returns>Information about all listable properties</returns>
		public System.ComponentModel.PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length > 0)
			{
				Type t = this.memberType;
				for (int i = 0; i < listAccessors.Length; i++)
				{
					PropertyDescriptor pd = listAccessors[i];
#if DEBUG
					//					System.Diagnostics.Debug.WriteLine("*** " + t.FullName + ": " + pd.Name);
#endif
					t = (Type)PropertyTypeHash.Instance[t, pd.Name];
				}
#if DEBUG
				//				System.Diagnostics.Debug.WriteLine("*** New Collection for " + t.FullName);
#endif
				// if t is null an empty list will be generated
				return NDOTypeDescriptor.GetProperties(t);
			}
			return NDOTypeDescriptor.GetProperties(memberType);
		}

		public static Type GetElementType(System.Collections.IList list, Type parentType, string propertyName)
		{
			INDOTypedList al = null;
			object element = null;
			al = CheckForArrayList(list);
			if (al == null)
			{
				if (list.Count > 0)
				{
					element = list[0];
				}
			}
			if (al == null && element == null)
			{
				PropertyInfo pi = parentType.GetProperty(propertyName);
				if (pi != null)
				{
					object parentObject = null;
					try
					{
						parentObject = Activator.CreateInstance(parentType);
					}
					catch { }
					if (parentObject != null)
					{
						list = pi.GetValue(parentObject, null) as IList;
						al = CheckForArrayList(list);
					}
				}
			}
			if (al != null)
			{
				return al.MemberType;
			}
			else if (element != null)
			{
				return element.GetType();
			}
			return null;
		}

		private static INDOTypedList CheckForArrayList(object l)
		{
			IList list = l as IList;
			if (list == null)
				return null;
			if (list.GetType().FullName == "System.Collections.ArrayList+ReadOnlyArrayList"
				|| list.GetType().FullName == "System.Collections.ArrayList+ReadOnlyList")
			{
				FieldInfo fi = list.GetType().GetField("_list", BindingFlags.NonPublic | BindingFlags.Instance);
				if (fi != null)
				{
					list = (IList)fi.GetValue(list);
				}
			}
			return list as INDOTypedList;
		}


		/// <summary>
		/// See documentation for ITypedList
		/// </summary>
		/// <param name="listAccessors">Parameter will be provided by Windows Forms binding infrastructure</param>
		/// <returns>Name of the list for use as Mapping Name in Table Mappings</returns>
		public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length > 0)
			{
				PropertyDescriptor p = listAccessors[listAccessors.Length - 1];
				return p.DisplayName;
			}
			else
				return memberType.Name;

		}

		#endregion

		#region INDOTypedList Member

		public Type MemberType
		{
			get { return this.memberType; }
		}

		#endregion

	}
}
#endif

