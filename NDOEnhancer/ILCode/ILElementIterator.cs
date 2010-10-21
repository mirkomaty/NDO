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
