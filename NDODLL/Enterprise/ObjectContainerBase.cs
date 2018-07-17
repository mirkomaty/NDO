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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Remoting.Messaging;
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

		[NonSerialized]
		private SerializationFlags serFlags;

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
				return Serialize(); 
			}
			set 
			{ 
				Deserialize(value); 
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether serializing uses the Binary or the Soap formatter.
		/// </summary>
		/// <remarks>
		/// This property is a wrapper around the UseBinaryFormat flag of the SerFlags property.
		/// BinaryFormat is false by default. Before deserializing from a not seekable stream, which 
		/// was serialized using the Binary Formatter, set UseBinaryFormat explicitly to true.
		/// </remarks>
		/// <value><c>true</c> if the Binary Formatter should be used.</value>
		[XmlIgnore]
		public bool BinaryFormat
		{
			get { return (serFlags & SerializationFlags.UseBinaryFormat) != 0; }
			set 
			{ 
				if (value) 
					serFlags |= SerializationFlags.UseBinaryFormat; 
				else
					serFlags &= ~SerializationFlags.UseBinaryFormat;
			}
		}


		/// <summary>
		/// Gets or sets the flags, which determine, how the search for child objects is conducted 
		/// and what formatter should be used.
		/// </summary>
		/// <remarks>
		/// The ObjectContainerBase ignores all flags except UseBinaryFormat. Other classes, 
		/// derived from ObjectContainerBase use the other flags.
		/// </remarks>
		/// <value>The flags.</value>
		[XmlIgnore]
		public SerializationFlags SerFlags
		{
			get { return serFlags; }
			set { serFlags = value; }
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
		/// <remarks>
		/// Only available in the NDO Enterprise Edition.
		/// The function determines, if the string is a soap string. If that is the case,
		/// the string will be deserialized using the SoapFormatter. Otherwise, the string
		/// will be considered as a Base64 string. It will be converted to a byte array and 
		/// deserialized using the BinaryFormatter.
		/// </remarks>
		public virtual void Deserialize(string value)
		{
			this.BinaryFormat = !value.StartsWith("<SOAP");
			MemoryStream ms = new MemoryStream();
			if (BinaryFormat)
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
			Deserialize(ms, false);
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


		void Deserialize(Stream stream, bool testForBinary)
		{
			if (testForBinary && stream.CanSeek)
			{
				long pos = stream.Position;
				byte[] buf1 = new byte[5];
				stream.Read(buf1, 0, 5);
				byte[] buf2 = System.Text.Encoding.ASCII.GetBytes("<SOAP");
				this.BinaryFormat = !BufEqual(buf1, buf2);
				stream.Seek(pos, SeekOrigin.Begin);
			}

			IRemotingFormatter formatter;
			if (this.BinaryFormat)
				formatter = new BinaryFormatter();
			else
				formatter = new SoapFormatter();
            formatter.Binder = new NDODeserializationBinder();
			object o = formatter.Deserialize(stream);
			this.objects = (ArrayList) o;
		}

		/// <summary>
		/// Deserializes the object container from the specified stream.
		/// </summary>
		/// <remarks>
		/// Note: If the stream is seekable (CanSeek returns true), NDO will determine automatically,
		/// if the Soap or the binary Formatter should be used. Otherwise NDO will use the BinaryFormat
		/// property to determine, what formatter should be used. BinaryFormat
		/// is false by default. So, if a binary stream is not seekable, set the BinaryFormat property
		/// explicitly before calling Deserialize.
		/// </remarks>
		/// <param name="stream">A stream instance.</param>
		public void Deserialize(Stream stream)
		{
			Deserialize(stream, true);
		}

		/// <summary>
		/// Serializes the objects collection to a string.
		/// </summary>
		/// <returns>The serialized string.</returns>
		/// <remarks>
		/// If binaryFormat is true, the BinaryFormatter will be used and the results will be
		/// converted into a Base64 string. Otherwise the SoapFormatter will be used.
		/// </remarks>
		public virtual string Serialize()
		{
			string s;
			using ( MemoryStream ms = new MemoryStream() )
			{
				InnerSerialize( ms );

				ms.Seek( 0, SeekOrigin.Begin );
				if ( !BinaryFormat )
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
				ms.Close();
			}
			return s;
		}


		/// <summary>
		/// Serializes the object container to the specified stream.
		/// </summary>
		/// <param name="stream">A stream instance.</param>
		public virtual void Serialize(Stream stream)
		{
			InnerSerialize(stream);
		}

		/// <summary>
		/// Non overridable version of Serialize
		/// </summary>
		/// <param name="stream"></param>
		void InnerSerialize( Stream stream )
		{
			IRemotingFormatter formatter;
			if ( this.BinaryFormat )
				formatter = new BinaryFormatter();
			else
				formatter = new SoapFormatter();
			formatter.Serialize( stream, this.objects );
			stream.Flush();
		}

		/// <summary>
		/// This function is used by the Soap/Binary formatters and shouldn't be called by users.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("objects", this.Serialize());
		}

		/// <summary>
		/// This constructor is used by the Soap/Binary formatters and shouldn't be called by users.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ObjectContainerBase(SerializationInfo info, StreamingContext context)
		{
			string s = (string) info.GetValue("objects", typeof(string));
			this.Deserialize(s);
		}

	}
}

