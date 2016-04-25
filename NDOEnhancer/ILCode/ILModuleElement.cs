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
	/// Summary description for ILModuleElement.
	/// </summary>
	internal class ILModuleElement : ILElement
	{
		public ILModuleElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILModuleElementType : ILElementType
		{
			public ILModuleElementType()
				: base( ".module", typeof( ILModuleElement ) )
			{
			}
		}

		internal class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILModuleElement ) )
			{
			}

			public new ILModuleElement
			getFirst()
			{
				return base.getFirst() as ILModuleElement;
			}

			public new ILModuleElement
			getNext()
			{
				return base.getNext() as ILModuleElement;
			}
		}

		private static ILElementType		m_elementType = new ILModuleElementType();
		
		private string						m_name;
		private bool						m_extern;

		public static void
		initialize()
		{
		}

		public static ILModuleElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		private void
		resolve()
		{
			if ( null != m_name )
				return;

			string[] words = splitWords( getLine( 0 ) );
			int		 count = words.Length;

			m_name = words[--count];

			int i;
			for ( i=1; i<count; i++ )
			{
				if ( words[i].Equals( "extern" ) )
				{
					m_extern = true;
				}
			}
		}

		protected override void
		unresolve()
		{
			m_name = null;
		}

		public string
		getName()
		{
			resolve();

			return m_name;
		}

		public bool
		isExtern()
		{
			resolve();
		
			return m_extern;
		}

		public void
		setName( string name )
		{
			foreach ( char c in name )
			{
				if ( ! Char.IsLetterOrDigit( c ) )
				{
					name = "'" + name + "'";
					break;
				}
			}

			replaceText( m_name, name );
			m_name = name;
		}
	}
}
