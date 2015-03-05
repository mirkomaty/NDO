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
	/// This attribute models the kind of relationship between two classes.
    /// <remarks>
	/// If there are several relations of one class to the same target class the relations 
	/// should be distinguished by a relation name.
	/// Example: a Car class can have two fields "seller" and "buyer" which are both related 
	/// to the Person class.
	/// 1:n relations have to be implemented with containers inherited from IList. In that
	/// case you have to specify the target type.
	/// If you need to build composite relations, use a constructor with the RelationInfo parameter.
    /// If you want to influence the mapping information, use the 
    /// <see cref="NDO.Mapping.Attributes.RelationAttribute">RelationAttribute</see> class, the 
    /// <see cref="NDO.Mapping.Attributes.ForeignKeyColumnAttribute">ForeignKeyColumnAttribute</see> class, 
    /// and the <see cref="NDO.Mapping.Attributes.ChildForeignKeyColumnAttribute">ChildForeignKeyColumnAttribute</see> class.
    /// </remarks>
    /// <seealso cref="NDO.Mapping.Attributes.RelationAttribute">RelationAttribute</seealso> 
    /// <seealso cref="NDO.Mapping.Attributes.ForeignKeyColumnAttribute">ForeignKeyColumnAttribute</seealso>
    /// <seealso cref="NDO.Mapping.Attributes.ChildForeignKeyColumnAttribute">ChildForeignKeyColumnAttribute</seealso>
    /// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]	
	public class NDORelationAttribute : System.Attribute
	{
		private Type type;  // System type of related field
		private RelationInfo ri; // Aggr./Comp. 
		private string name;

		/// <summary>
		/// Constructor
		/// If other relations to the same type exists, use a constructor with the relName parameter.
		/// </summary>
		/// <param name="t">Related type</param>
		/// <param name="ri">Type of relation - composition or assoziation</param>
		public NDORelationAttribute(Type t, RelationInfo ri)
		{
			this.type = t;
			this.ri = ri;
			this.name = string.Empty;
		}

		/// <summary>
		/// Use this constructor if several relations to the same type exist in your class.
		/// </summary>
		/// <param name="t">Related type</param>
		/// <param name="ri">Type of relation - composition or assoziation</param>
		/// <param name="relName">Relation name</param>
		public NDORelationAttribute(Type t, RelationInfo ri, string relName)
		{
			this.type = t;
			this.ri = ri;
			this.name = relName;
		}


		/// <summary>
		/// Constructor for assoziation relations. 
		/// If other relations to the same type exists, use a constructor with the relName parameter.
		/// </summary>
		/// <param name="t">Related type</param>
		public NDORelationAttribute(Type t)
		{
			this.type = t;
			this.ri = RelationInfo.Default;
			this.name = string.Empty;
		}

		/// <summary>
		/// Use this constructor if several relations to the same type exist in your class.
		/// </summary>
		/// <param name="t">Related Type</param>
		/// <param name="relName">Relation name</param>
		public NDORelationAttribute(Type t, string relName)
		{
			this.type = t;
			this.ri = RelationInfo.Default;
			this.name = relName;
		}

		/// <summary>
		/// Constructor to be used for 1:1 relations where the relation type is part of the code. 
		/// Use this constructor for a bidirectional Relation if there exists another relation to the same type in your class.
		/// </summary>
		/// <param name="ri"></param>
		/// <param name="relName"></param>
		public NDORelationAttribute(RelationInfo ri, string relName)
		{
			this.type = null;
			this.ri = ri;
			this.name = relName;
		}

		/// <summary>
		/// Use this constructor for 1:1 relations which are not composites.
		/// If other relations to the same type exists, use a constructor with the relName parameter.
		/// </summary>
		public NDORelationAttribute()
		{
			type = null;
			ri = RelationInfo.Default;
			name = string.Empty;
		}


		/// <summary>
		/// Use this constructor for 1:1 relations which need RelationInfo.
		/// If other relations to the same type exists, use a constructor with the relName parameter.
		/// </summary>
		public NDORelationAttribute(RelationInfo ri)
		{
			type = null;
			this.ri = ri;
			name = string.Empty;
		}


		/// <summary>
		/// Accessor for relation type info
		/// </summary>
		public RelationInfo Info
		{
			get {return this.ri;}
		}

		/// <summary>
		/// Retrieve related type
		/// </summary>
		public Type RelationType
		{
			get {return type;}
		}

		/// <summary>
		/// Accessor for relation name
		/// </summary>
		public string RelationName
		{
			get { return this.name; }
			set { name = value; }
		}
	}

}
