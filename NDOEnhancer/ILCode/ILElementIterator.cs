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
using System.Collections;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILElementIterator.
	/// </summary>
	internal class ILElementIterator
	{
		public ILElementIterator( ILElement element, Type elementType )
		{
			Init(element, elementType, -1, false);
		}

		public ILElementIterator( ILElement element, Type elementType, bool recursive )
		{
			Init(element, elementType, -1, true);
		}

		private ILElementIterator( ILElement element, Type elementType, int index )
		{
			Init(element, elementType, index, false);
		}

		private void Init(ILElement parent, Type elementType, int index, bool recursive)
		{
			m_recursive	  = recursive;
			m_elementType = elementType;
			m_index		  = index;
			elementList = new ArrayList();
			if (!recursive)
			{
				for ( int i = 0; i < parent.getSubElementCount(); i++ )
				{
					ILElement element = parent.getElement( i );

					if ( element.GetType().Equals( m_elementType )
						||	 element.GetType().IsSubclassOf( m_elementType ) )
						elementList.Add(element);
				}				
			}
			else
			{
				AddRecursive(parent);
			}
		}

		private void AddRecursive(ILElement parent)
		{
			for ( int i = 0; i < parent.getSubElementCount(); i++ )
			{
				ILElement element = parent.getElement( i );

				if ( element.GetType().Equals( m_elementType )
					||	 element.GetType().IsSubclassOf( m_elementType ) )
					elementList.Add(element);
				if (element.getSubElementCount() > 0)
					AddRecursive(element);
			}				
		}

		IList elementList;

		private Type				m_elementType;
		private int					m_index;
		private bool				m_recursive;

		public ILElement
		getFirst()
		{
			m_index = -1;
			return getNext();
		}

		public ILElement
		getPrev()
		{
			m_index--;
			if (m_index < 0)
				return null;
			return (ILElement) elementList[m_index];
		}


		Stack itStack = new Stack();

		public ILElement
		getNext()
		{
			m_index++;
			if (m_index >= elementList.Count)
				return null;  // Kein Element mehr
			return (ILElement) elementList[m_index];
		}

		public ILElement
		getLast()
		{
			m_index = elementList.Count;
			return getPrev();
		}

//		public ILElementIterator
//		asAllElementIterator()
//		{
//			return new ILElementIterator( m_element, typeof( ILElement ), m_index );
//		}
	}
}
