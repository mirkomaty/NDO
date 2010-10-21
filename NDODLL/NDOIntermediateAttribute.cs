//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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


#if nix
using System;

namespace NDO
{
	/// <summary>
	/// This attribute is used to mark classes, which don't have a ID column by themselfes, 
	/// but are identified as intermediate objects of other classes.
	/// </summary>
	/// <remarks>
	/// Oids of Intermediate classes are mapped to two relations of element multiplicity.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]	
	public class NDOIntermediateAttribute : System.Attribute
	{
		string relationFieldName1;
		/// <summary>
		/// Gets the field name of the first of the two relations, which define the identity of the object.
		/// </summary>
		public string RelationFieldName1
		{
			get { return relationFieldName1; }
		}
		string relationFieldName2;
		/// <summary>
		/// Gets the field name of the second the two relations, which define the identity of the object.
		/// </summary>
		public string RelationFieldName2
		{
			get { return relationFieldName2; }
		}
		
		/// <summary>
		/// Constructs a NDOIntermediate class attribute.
		/// </summary>
		/// <param name="relationFieldName1">Field name of the first relation, which is used to map the oid.</param>
		/// <param name="relationFieldName2">Field name of the second relation, which is used to map the oid.</param>
		/// <remarks>
		/// Example:
		/// <code>
		/// [NDOPersistent, NDOIntermediate("order", "product")]
		/// class OrderDetail
		/// {
		///		[NDORelation]
		///		Order order;		// Defines the parent relation
		///		[NDORelation]
		///		Product product;	// Defines the child relation
		///		...
		///	}
		/// </code>
		/// The relations must have a cardinality of 1 (Multiplicity = Element). 
		/// Intermediate classes must not have other relations.
		/// </remarks>
		public NDOIntermediateAttribute(string relationFieldName1, string relationFieldName2)
		{
			this.relationFieldName1 = relationFieldName1;
			this.relationFieldName2 = relationFieldName2;
		}
	}
}
#endif