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
using System.Reflection;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILElementType.
	/// </summary>
	internal class ILElementType
	{
		public ILElementType( string keyWord, Type type )
		{
			m_keyWord = keyWord;
			m_type	  = type;

			ILFile.AddElementType( this );
		}

		private string		m_keyWord;
		private Type		m_type;

		public virtual bool
		IsElement( string firstLine )
		{
			if ( ! firstLine.StartsWith( m_keyWord ) )
				return false;
			
			if ( '.' != m_keyWord[0] )
				return true;

			if ( firstLine.Length == m_keyWord.Length )
				return true;

			return Char.IsWhiteSpace( firstLine[m_keyWord.Length] );
		}

		public ILElement
		CreateElement( string firstLine, ILElement owner )
		{
			if ( null == m_type )
				return null;

			Type[]   ptype = { typeof( string ), typeof( ILElement ) };
			object[] param = { firstLine, owner }; 
			ConstructorInfo ctor = m_type.GetConstructor( ptype );
			return  ctor.Invoke( param ) as ILElement;
		}
	}
}
