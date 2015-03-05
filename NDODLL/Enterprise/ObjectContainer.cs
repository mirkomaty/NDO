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
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

namespace NDO
{
	/// <summary>
    /// This class can be used to serialize object graphs und transport 
    /// them as parameters of web service or remoting calls.
    /// An ObjectContainer can be merged to a PersistenceManager or 
    /// OfflinePersistenceManager.
	/// </summary>
	[Serializable]
	public class ObjectContainer : ObjectContainerBase
	{
		private ISerializationIterator iterator;

		/// <summary>
		/// The SerializationIterator iterates through the object tree and makes child objects visible.
		/// </summary>
		/// <remarks>
		/// Only available in the NDO Enterprise Edition.
		/// The NDOState.Hollow makes objects invisible in the receiving PersistenceManager.
		/// NDOState.Persistent makes the objects visible. All other States should be avoided and may lead 
		/// to undefined behavior.
		/// </remarks>
		[XmlIgnore]
		public ISerializationIterator SerialisationIterator
		{
			get { return iterator; }
			set { iterator = value; }
		}


		/// <summary>
		/// Standard Constructor, which is used mainly by the XmlSerializer.
		/// </summary>
		/// <remarks>
		/// You probably won't need any constructor for ObjectContainer. Normally the 
		/// PersistenceManager.GetObjectContainer functions are used to construct ObjectContainers.
		/// </remarks>
		public ObjectContainer() : base()
		{
			this.SerFlags = SerializationFlags.SerializeNone;
		}

		/// <summary>
		/// Constructor which is used mainly by the PersistenceManager.
		/// </summary>
		/// <param name="serFlags">Flags, which determine, how the Serialization is conducted.</param>
		/// <remarks>
		/// You probably won't need any constructor for ObjectContainer. Normally the 
		/// PersistenceManager.GetObjectContainer functions are used to construct ObjectContainers.
		/// </remarks>
		public ObjectContainer(SerializationFlags serFlags) : base()
		{
			this.SerFlags = serFlags;
		}



		/// <summary>
		/// Serializes the objects collection to a string.
		/// </summary>
		/// <returns>The serialized string.</returns>
		/// <remarks>
		/// If binaryFormat is true, the BinaryFormatter will be used and the results will be
		/// converted into a Base64 string. Otherwise the SoapFormatter will be used.
		/// </remarks>
		public override string Serialize()
		{
			Iterate();
			return base.Serialize();
		}

		private void Iterate()
		{
			if ( this.iterator == null )
				this.iterator = new SerializationIterator( this.SerFlags );
			foreach ( IPersistenceCapable pc in this.RootObjects )
				this.iterator.Iterate( pc );
		}

		/// <summary>
		/// Serializes the objects collection to a string.
		/// </summary>
		/// <returns>The serialized string.</returns>
		/// <remarks>
		/// If binaryFormat is true, the BinaryFormatter will be used and the results will be
		/// converted into a Base64 string. Otherwise the SoapFormatter will be used.
		/// </remarks>
		public override void Serialize( Stream stream )
		{
			Iterate();
			base.Serialize( stream );
		}


		/// <summary>
		/// This constructor is used by the Soap/Binary formatters and shouldn't be called by users.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ObjectContainer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

	}
}


#endif