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

using NDO;
using NDO.Mapping;
using NDO.ShortId;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NDO.JsonFormatter
{
	static class PersistenceCapableExtensions
	{
		/// <summary>
		/// Reads an object from a JToken
		/// </summary>
		/// <param name="objectState"></param>
		/// <remarks>
		/// The method doesn't change the NDOObjectState.
		/// </remarks>		
		public static void FromJToken( this IPersistenceCapable pc, JToken objectState )
		{
			Type myType = pc.GetType();
			FieldMap fm = new FieldMap( myType );
			Action setDirty = ()=>{ };
			if (pc != null && pc.NDOObjectState == NDOObjectState.Persistent)
			{
				if (pc.NDOStateManager == null)  // Tests ohne Persistenz
					setDirty = () => { pc.NDOObjectState = NDOObjectState.PersistentDirty; };
				else
					setDirty = () => { pc.NDOStateManager.MarkDirty( pc ); };
			}

			foreach (var dictEntry in fm.PersistentFields)
			{
				if (!( dictEntry.Value is FieldInfo ))
					continue;   // This are properties of embedded value types or classes. We should extend the code here, to support embedded members.

				FieldInfo fi = (FieldInfo)dictEntry.Value;
				object oldObj = fi.GetValue(pc);

				JToken obj = objectState[fi.Name];

				if (obj != null)
					fi.SetValue( pc, obj.ToObject( fi.FieldType ) );
			}

			var ndoStateToken = objectState["NDOObjectState"];
			if (ndoStateToken != null)
				pc.NDOObjectState = ndoStateToken.ToObject<NDOObjectState>();
		}


		public static IDictionary<string, object> ToDictionary(this IPersistenceCapable pc, IPersistenceManager pm )
		{
			if (pc == null)
				return new Dictionary<string, object>();

			if (pm == null)
			{
				var sm = pc.NDOStateManager;
				if (sm == null)
					throw new Exception( "ToDictionary: pm == null und kann nicht aus dem Objekt ermittelt werden." );
				pm = sm.PersistenceManager;
			}

			Type myType = pc.GetType();
			FieldMap fm = new FieldMap( myType );
			Dictionary<string, object> result = new Dictionary<string, object>();
			foreach (var dictEntry in fm.PersistentFields)
			{
				MemberInfo mi = (MemberInfo)dictEntry.Value;
				if (!( mi is FieldInfo ))
					continue;  // This are properties of embedded value types or classes. We should extend the code here, to support embedded members.
				FieldInfo fi = (FieldInfo)dictEntry.Value;

				object obj = fi.GetValue( pc );
				result.Add( dictEntry.Key, obj );
			}

			result.Add( "_oid", pc.ShortId() );
			result.Add( "NDOObjectState", pc.NDOObjectState );
			return result;
		}
	}
}
