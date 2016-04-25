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
	/// Summary description for ILPropertyElement.
	/// </summary>
	internal class ILPropertyElement : ILElement
	{
		public ILPropertyElement()
			: base( true )
		{
		}

		public ILPropertyElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		private string m_name;

		internal class ILPropertyElementType : ILElementType
		{
			public ILPropertyElementType()
				: base( ".property", typeof( ILPropertyElement ) )
			{
			}
		}

		internal class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILPropertyElement ) )
			{
			}

			public new ILPropertyElement
			getFirst()
			{
				return base.getFirst() as ILPropertyElement;				
			}

			public new ILPropertyElement
			getNext()
			{
				return base.getNext() as ILPropertyElement;
			}
		}

		private static ILElementType		m_elementType = new ILPropertyElementType();
		
		public static void
		initialize()
		{
		}

		public static ILPropertyElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		public string
		getName()
		{
			resolve();

			return m_name;
		}


		private void
		resolve()
		{
			if ( this.m_name == null )
			{
				string[] all = getAllLines().Split( ' ' );
				this.m_name = all[all.Length - 1].Replace( "()", string.Empty );
			}
		}

		public void
		addGetter( string firstLine )
		{
			addElement( new ILGetElement( firstLine ) );
		}

		public void
		addSetter( string firstLine )
		{
			addElement( new ILSetElement( firstLine ) );
		}
	}
}
