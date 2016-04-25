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

namespace NDO
{
	/// <summary>
	/// NDOObjectIdAttribute is used to declare a Field as ObjectId of the class. 
	/// This is useful if the business logic of a class must compute the oid value.
    /// Important Note: The NDOObjectIdAttribute class is obsolete. Use the class 
    /// OidColumnAttribute instead.
	/// </summary>
	/// <remarks>
    /// If your class code doesn't need access to the oid value, use an IDGenerationHandler
    /// instead of mapping the oid to a field.
    /// Sample:
    /// <code>
    /// [OidColumn(FieldName = "myOidField")]
    /// [NDOPersistent]
    /// public class MyPersistentClass
    /// {
    ///     string myOidField;  // will be mapped as Oid
    ///     ...
    /// }
    /// </code>
	/// <seealso cref="PersistenceManagerBase.IdGenerationEvent"/>
	/// <seealso cref="NDOOidTypeAttribute"/>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Obsolete("Use the OidColumnAttribute instead.")]
	public class NDOObjectIdAttribute : System.Attribute
	{
		/// <summary>
		/// Standard Constructor
		/// </summary>
		public NDOObjectIdAttribute()
		{
		}
	}
}
