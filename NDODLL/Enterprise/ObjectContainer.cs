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
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using NDO.Mapping;

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
        private readonly Predicate<Relation> relationSelector;
        private readonly Action<IPersistenceCapable> actionOnRelatedObject;

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
        /// Constructor which is used mainly by the PersistenceManager.
        /// </summary>
        /// <param name="relationSelector">Determines whether a certain relation should be included in the container. Null means that no relations should be searched.</param>
        /// <param name="actionOnRelatedObject">Action to be executed on every object included in the container. Use this action to mark objects as NDOTransient.</param>
        /// <remarks>
        /// You probably won't need any constructor for ObjectContainer. Normally the 
        /// PersistenceManager.GetObjectContainer functions are used to construct ObjectContainers.
        /// </remarks>
        public ObjectContainer( Predicate<Relation> relationSelector = null, Action<IPersistenceCapable> actionOnRelatedObject = null ) : base()
		{
            this.relationSelector = relationSelector;
            this.actionOnRelatedObject = actionOnRelatedObject;
        }

        /// <summary>
        /// Serializes the objects collection to a string.
        /// </summary>
        /// <param name="formatter">The formatter used for serializing</param>
        /// <returns>The serialized string.</returns>
        public override string Serialize(IFormatter formatter)
		{
			Iterate();
			return base.Serialize(formatter);
		}

		private void Iterate()
		{
            if (this.iterator == null)
                this.iterator = new SerializationIterator( this.relationSelector, this.actionOnRelatedObject );

			foreach ( IPersistenceCapable pc in this.RootObjects )
				this.iterator.Iterate( pc );
		}
 
        /// <summary>
        /// Serializes the objects collection to a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        /// <param name="stream">The stream to serialize to</param>
        /// <param name="formatter">The formatter used for serializing</param>
        public override void Serialize( Stream stream, IFormatter formatter )
		{
			Iterate();
			base.Serialize( stream, formatter );
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
