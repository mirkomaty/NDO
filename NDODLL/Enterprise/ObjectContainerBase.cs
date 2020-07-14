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

namespace NDO
{
	/// <summary>
	/// Base class for ObjectContainer and ChangeSetContainer.
	/// </summary>
	[Serializable]
	public class ObjectContainerBase : ISerializable
	{
		private ArrayList objects = new ArrayList();  // If transmitted via Soap/Binary Formatter, this list will be transfered
													  // List<object> can't be transferred via SoapFormatter, so we use ArrayList.

		/// <summary>
		/// Standard Constructor, which is used mainly by the XmlSerializer.
		/// </summary>
		/// <remarks>
		/// You probably won't need any constructor for ObjectContainer. Normally the 
		/// PersistenceManager.GetObjectContainer functions are used to construct ObjectContainers.
		/// </remarks>
		public ObjectContainerBase()
		{
		}

        /// <summary>
        /// If the XmlSerializer or Remoting is used, set the formatter after creating the object container.
        /// </summary>
        public IFormatter Formatter { get; set; }

		// Internal remark: The MarshalingString property should be the only
		// public property or variable, which is not marked with [XmlIgnore] 
		// because only this string should be transfered by the XmlSerializer.

		/// <summary>
		/// Public property to allow the XmlSerializer to marshal the object collection.
		/// </summary>
		/// <remarks>
		/// Do not set this property. Just get an ObjectContainer from your PersistenceManager
		/// and serialize it using either Soap/Binary Formatter or XmlSerializer. You can also use the 
		/// Marshaling string to transfer the Container using an object of your choice.
		/// Note: .Net Remoting will use the Formatters, .Net Web Services will use the XmlSerializer.
		/// </remarks>
		public virtual string MarshalingString
		{
			get 
			{
                if (Formatter == null)
                    throw new NDOException( 101, "Formatter is null. Please provide a value for the Formatter property." );

				return Serialize(Formatter); 
			}
			set 
			{
                if (Formatter == null)
                    throw new NDOException( 101, "Formatter is null. Please provide a value for the Formatter property." );

                Deserialize( value, Formatter); 
			}
		}

		/// <summary>
		/// Returns a list of the added root objects in the container. 
		/// </summary>
		/// <remarks>
		/// The container might contain
		/// child objects, which are referenced in the relations of the root objects. 
		/// The child objects can only be referenced through the root objects.
		/// </remarks>
		[XmlIgnore]
		public virtual IList RootObjects
		{
			get { return objects; }
		}

		/// <summary>
		/// Adds a range of objects to the objects list.
		/// </summary>
		/// <param name="l">The objects to add.</param>
		public void AddList(IList l)
		{
			this.objects.AddRange(l);
		}

		/// <summary>
		/// Adds an object to the objects list.
		/// </summary>
		/// <param name="o">The object to add.</param>
		public void AddObject(object o)
		{
			this.objects.Add(o);
		}

		/// <summary>
		/// Deserializes the objects collection from a string.
		/// </summary>
		/// <param name="value">The string to deserialize.</param>
		/// <param name="formatter">The formatter used for deserialization.</param>
		/// <remarks>
		/// The function determines, if the string is a base64 string. If that is the case,
		/// the string will be decoded befor using the Formatter.
		/// </remarks>
		public virtual void Deserialize(string value, IFormatter formatter)
		{
			var binaryFormat = value.StartsWith( "AAEAAAD/////" );
			MemoryStream ms = new MemoryStream();
			if (binaryFormat)
			{
				byte[] bytes = Convert.FromBase64String(value);
				ms.Write(bytes, 0, bytes.Length);
				ms.Flush();
			}
			else
			{
				StreamWriter sw = new StreamWriter(ms);
				sw.Write(value);
				sw.Flush();
			}
			ms.Seek(0, SeekOrigin.Begin);
			Deserialize(ms, formatter);
			ms.Close();
		}

		bool BufEqual(byte[] buf1, byte[] buf2)
		{
			if (buf1.Length != buf2.Length)
				return false;
			for(int i = 0; i < buf1.Length; i++)
				if (buf1[i] != buf2[i])
					return false;
			return true;
		}


		void Deserialize(Stream stream, IFormatter formatter)
		{
            formatter.Binder = new NDODeserializationBinder();
			object o = formatter.Deserialize(stream);
			this.objects = (ArrayList) o;
		}

		/// <summary>
		/// Serializes the objects collection to a string.
		/// </summary>
		/// <returns>The serialized string.</returns>
		/// <remarks>
		/// If binaryFormat is true, the results will be
		/// converted into a Base64 string.
		/// </remarks>
		public virtual string Serialize(IFormatter formatter)
		{
			string s;
			using ( MemoryStream ms = new MemoryStream() )
			{
				InnerSerialize( ms, formatter );

				ms.Seek( 0L, SeekOrigin.Begin );

                var binaryFormat = formatter.GetType().Name == "BinaryFormatter";

                if ( !binaryFormat )
				{
					StreamReader sr = new StreamReader( ms );
					s = sr.ReadToEnd();
					sr.Close();
				}
				else
				{
					byte[] buffer = new byte[ms.Length];
					ms.Read( buffer, 0, (int) ms.Length );
					s = Convert.ToBase64String( buffer );
				}
			}

			return s;
		}


		/// <summary>
		/// Serializes the object container to the specified stream.
		/// </summary>
		/// <param name="stream">A stream instance.</param>
		/// <param name="formatter">A formatter used for serialization.</param>
		public virtual void Serialize(Stream stream, IFormatter formatter)
		{
			InnerSerialize(stream, formatter);
		}

		/// <summary>
		/// Non overridable version of Serialize
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="formatter">A formatter used for serialization.</param>
		void InnerSerialize( Stream stream, IFormatter formatter )
		{
			formatter.Serialize( stream, this.objects );
			stream.Flush();
		}

        /// <summary>
        /// This function is used by the formatters and shouldn't be called by users.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            if (Formatter == null)
                throw new NDOException( 101, "Formatter is null. Please provide a value for the Formatter property." );

            info.AddValue( "objects", this.Serialize(Formatter) );
        }

        /// <summary>
        /// This constructor is used by the Soap/Binary formatters and shouldn't be called by users.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ObjectContainerBase( SerializationInfo info, StreamingContext context )
        {
            if (Formatter == null)
                throw new NDOException( 101, "Formatter is null. Please provide a value for the Formatter property." );

            string s = (string)info.GetValue( "objects", typeof( string ) );
            this.Deserialize( s, Formatter );
        }
    }
}

