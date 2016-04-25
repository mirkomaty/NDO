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

namespace ILCode
{
	/// <summary>
	/// Summary description for ILPublickeytokenElement.
	/// </summary>
	internal class ILPublickeytokenElement : ILElement
	{
		public ILPublickeytokenElement()
			: base( false )
		{
		}

		public ILPublickeytokenElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILPublickeytokenElementType : ILElementType
		{
			public ILPublickeytokenElementType()
				: base( ".publickeytoken", typeof( ILPublickeytokenElement ) )
			{
			}
		}

		internal class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILPublickeytokenElement ) )
			{
			}

			public new ILPublickeytokenElement
			getFirst()
			{
				return base.getFirst() as ILPublickeytokenElement;
			}

			public new ILPublickeytokenElement
			getNext()
			{
				return base.getNext() as ILPublickeytokenElement;
			}
		}

		private static ILElementType		m_elementType = new ILPublickeytokenElementType();
		
		public static void
		initialize()
		{
		}

		public static ILPublickeytokenElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}
	}
}
