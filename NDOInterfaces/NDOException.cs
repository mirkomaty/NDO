//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Runtime.Serialization;

namespace NDO 
{
	/// <summary>
	/// This exception type will be thrown by the NDO framework, if errors occure.
	/// </summary>
	[Serializable]
	public class NDOException : Exception, ISerializable
	{
		int errorNumber;

		/// <summary>
		/// Gets the error number of the NDOException. See the <see href="ExceptionNumbers.html">documentation</see> for information about the error numbers.
		/// </summary>
		public int ErrorNumber
		{
			get { return errorNumber; }
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="errorNumber">A unique number, which denotes the cause of the exception.</param>
		/// <param name="s">The message string of the exception.</param>
		public NDOException(int errorNumber, string s) : base(s) 
		{
			this.errorNumber = errorNumber;
		}

		/// <summary>
		/// Constructs a NDOException object.
		/// </summary>
		/// <param name="errorNumber">The uniqe error code.</param>
		/// <param name="s">The message string of the exception.</param>
		/// <param name="innerException"></param>
		public NDOException(int errorNumber, string s, Exception innerException) : base(s, innerException) 
		{
			this.errorNumber = errorNumber;
		}

		/// <summary>
		/// This constructor is used while deserializing an NDOException object.
		/// </summary>
		/// <param name="info">The SerializationInfo containing the data, the object is serialized from.</param>
		/// <param name="context">The source if this serialization.</param>
		protected NDOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//			string key = (string) info.GetValue("Key", typeof(string));
			this.errorNumber = (int) info.GetValue("errorNumber", typeof(int));
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The SerializationInfo to populate with data. </param>
		/// <param name="context">The destination for this serialization.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorNumber", this.errorNumber);
		}

		/// <summary>
		/// Overridden. Creates and returns a string representation of the current exception.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string s = base.ToString ();
			int p = s.IndexOf(":");
			if (p > -1)
			{
				return s.Substring(0, p + 2) + "#" + errorNumber + " " + s.Substring(p + 2);
			}
			else
				return s;
		}

	}
}
