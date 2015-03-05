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
