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
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Data;
using System.Collections;
using NDO.Mapping;


namespace NDO
{
	/// <summary>
	/// Only available in the NDO Enterprise Edition.
	/// </summary>
	/// <remarks>
	/// This class is obsolete. Use the ObjectContainer classes instead.
	/// </remarks>
	[Obsolete]
	public class Serializer
	{
		IPersistenceCapable root;
		PersistenceManager pm;
		Mappings mapping;
		SerializationFlags serFlags;
		Hashtable objects;

		public Serializer(object root, SerializationFlags serFlags)
		{
			this.serFlags = serFlags;

			this.root = root as IPersistenceCapable;
			if (this.root == null)
				throw new ArgumentException("Parameter must implement IPersistenceCapable derivate and must not be null.", "root");
			if (this.root.NDOObjectState == NDOObjectState.Transient)
				throw  new ArgumentException("Parameter must be persistent", "root");
			if (this.root.NDOStateManager == null)
				throw new NDO.InternalException(1, "Container");
			pm = this.root.NDOStateManager.PersistenceManager as PersistenceManager;
			if (pm == null)
				throw new NDO.InternalException(2, "Container");
			mapping = pm.NDOMapping as Mappings;
			if (mapping == null)
				throw new NDO.InternalException(3, "Container");
			if (pm.DataSet == null)
				throw new NDO.InternalException(4, "Container");
		}	


		public string Serialize()
		{
			objects = new Hashtable();
			new SerializationIterator(this.serFlags).Iterate(this.root);
			SoapFormatter formatter = new SoapFormatter();
			MemoryStream ms = new MemoryStream();
			formatter.Serialize(ms, root);
			return Encoding.UTF8.GetString(ms.GetBuffer());
		}

	}
}
