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
using System.ComponentModel;

namespace NDO.Mapping.Attributes
{
	/// <summary>
	/// This attribute is used to place hints for the mapping information of fields in the source code
	/// </summary>
	/// <remarks>Note: This attribute is used like the <see cref="ColumnAttribute">ColumnAttribute</see>. The properties defined with FieldAttribute apply only to fields.</remarks>
	[AttributeUsage( AttributeTargets.Field )]
	public class FieldAttribute : Attribute
	{
		private bool? encrypted;

		/// <summary>
		/// True if the values should be stored encrypted.
		/// </summary>
		/// <remarks>This property affects the parent <see cref="Field">Field</see> of a column.</remarks>
		[Description( "True if the values should be stored encrypted." )]
		public bool Encrypted
		{
			get => this.encrypted.HasValue ? this.encrypted.Value : false;
			set => this.encrypted = value;
		}

		/// <summary>
		/// Initializes a given column to the values defined in this ColumnAttribute.
		/// </summary>
		/// <param name="field">The field to be initialized.</param>
		public void SetFieldValues( Field field )
		{
			if (this.encrypted.HasValue)
				field.Encrypted = this.encrypted.Value;
		}

		/// <summary>
		/// Initializes a given column to the values defined in this ColumnAttribute.
		/// </summary>
		/// <param name="field">The field to be initialized.</param>
		public void RemapField( Field field )
		{
			if (this.encrypted.HasValue && !field.Encrypted)
				field.Encrypted = this.encrypted.Value;
		}
	}
}
