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
