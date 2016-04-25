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
	/// Objects of classes implementing this Interface will get notifications,
	/// if certain events occur
	/// </summary>
	public interface IPersistenceNotifiable
	{
		/// <summary>
		/// This function will be called by the Persistence Manger, if an object is about to be saved in a DataRow.
		/// </summary>
		/// <remarks>It is allowed to use persistent fields and relation fields in this function.</remarks>
		void OnSaving();
		/// <summary>
		/// This function will be called by the Persistence Manger, if an object has been loaded from a DataRow
		/// </summary>
		/// <remarks>It is allowed to use persistent fields in this function. It is not allowed to use relation fields in this function, since it would cause a recursion.</remarks>
		void OnLoaded();

	}
}
