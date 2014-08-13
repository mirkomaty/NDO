//
// Copyright (C) 2002-2014 HoT - Mirko Matytschak
// (www.netdataobjects.de)
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
using System.ComponentModel;
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
	public class NDOArrayList 
	{
		public static Type GetElementType(IList list, Type parentType, string propertyName)
		{
			Type t = list.GetType();
			if (t.IsGenericType)
			{
				return t.GetGenericArguments()[0];
			}
			if (list.Count > 0)
			{
				object element = list[0];
			}
			return null;
		}
	}
}
