//
// Copyright (C) 2002-2014 Mirko Matytschak 
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
// there is a commercial licence available at www.netdataobjects.de.
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


#if ENT
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

#endif