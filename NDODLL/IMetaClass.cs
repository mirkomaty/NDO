#if nix
//
// Copyright (c) 2002-2023 Mirko Matytschak 
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
	/// This interface is for internal use of the NDO Framework. Don't use it in your application code.
	/// </summary>
	public interface IMetaClass
	{

		/// <summary>
		/// Create a new object without constructor parameters.
		/// </summary>
		IPersistenceCapable CreateObject();

		/// <summary>
		/// Gets the ordinal number of a relation. Ordinal numbers are defined by the enhancer.
		/// </summary>
		/// <param name="fieldName">The relation field name</param>
		/// <returns>The ordinal number</returns>
		/// <remarks>Throws a NDOException, if the field doesn't exist.</remarks>
		int GetRelationOrdinal(string fieldName);

	}

	/// <summary>
	/// This interface is for internal use of the NDO Framework. Don't use it in your application code.
	/// </summary>
	public interface IMetaClass2 : IMetaClass
	{
		/// <summary>
		/// Create a new object.
		/// </summary>
		/// <param name="serviceProvider">The service provider used to resolve constructor parameters</param>
		/// <remarks>This method will be used for classes with constructor parameters</remarks>
		IPersistenceCapable CreateObject( IServiceProvider serviceProvider );
	}
}

#endif