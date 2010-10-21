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
	/// This class supports data binding
	/// </summary>
	internal class NDOPropertyDescriptor : PropertyDescriptor
	{
		bool readOnly = false;
		bool isOid = false;
		Type componentType;
		Type propertyType;
		PropertyInfo prop;

		public NDOPropertyDescriptor (string name, Type componentType, Type propertyType)
			: base (name, null)
		{
			this.isOid = (name == "NDOObjectId");
			this.componentType = componentType;
			this.propertyType = propertyType;
		}


		public override object GetValue (object component)
		{
			IPersistenceCapable pc = component as IPersistenceCapable;
//			System.Diagnostics.Debug.Write("Get Value: ");
//			if (pc != null)
//				System.Diagnostics.Debug.WriteLine(pc.NDOObjectId + "/" + this.Name);
//			else
//				System.Diagnostics.Debug.WriteLine(component.GetType().Name + "/" + this.Name);
			
			if (!componentType.IsAssignableFrom(component.GetType()))
			{
				return null;
			}

			if (!isOid)
			{
				if (prop == null)
					prop = componentType.GetProperty (Name);
				object o = prop.GetValue (component, null);
				if (o is IList)
				{
//					System.Diagnostics.Debug.WriteLine("--- GetValue for " + componentType.FullName + " / " + Name);
					PropertyTypeHash.Instance[componentType, Name] = NDOArrayList.GetElementType((IList)o, componentType, Name);
				}
				return o;
			}
			else
			{
				if (pc == null)
					return "";
				NDO.ObjectId oid = pc.NDOObjectId;
				if (oid == null)
					return "";
                MultiKey mk = (MultiKey)oid.Id;
                string s = string.Empty;
                for (int i = 0; i < mk.Keydata.Length; i++)
                {
                    object value = mk.Keydata[i];
                    s += value;
                    if (i < mk.Keydata.Length - 1)
                        s += ", ";
                }
				return s;
			}
		}

		public override void SetValue(object component,	object value) {
			
			if (isOid)
				return;  // can't change oid values with data binding

			if (prop == null)
				prop = componentType.GetProperty (Name);

			prop.SetValue (component, value, null);
		}

		public override void ResetValue(object component) {

			return;
		}

		public override bool CanResetValue(object component) {

			return false;
		}

		public override bool ShouldSerializeValue(object component) {

			return false;
		}

		public override Type ComponentType
		{
			get {
				return componentType;
			}

		}

		public override bool IsReadOnly
		{
			get {
				return isOid || readOnly;				
			}
		}

		public override Type PropertyType
		{
			get {
				return propertyType;
			}
		}
	}
}

