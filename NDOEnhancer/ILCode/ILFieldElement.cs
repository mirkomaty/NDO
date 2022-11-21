//
// Copyright (c) 2002-2022 Mirko Matytschak 
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


using NDOEnhancer;
using System;
using System.Linq;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILFieldElement.
	/// </summary>
	internal class ILFieldElement : ILElement
	{
		public ILFieldElement()
			: base()
		{
		}

		public ILFieldElement( string firstLine )
			: base( firstLine )
		{
		}

		public ILFieldElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILFieldElementType : ILElementType
		{
			public ILFieldElementType()
				: base( ".field", typeof( ILFieldElement ) )
			{
			}
		}

		private static ILElementType		m_elementType = new ILFieldElementType();

		private bool						m_isPublic	  = false;
		private bool						m_isProtected = false;
		private bool						m_isPrivate	  = false;
		private bool						m_isStatic	  = false;
		private bool						m_isConst	  = false;
		private bool						m_isVolatile  = false;
		private string						m_ilType;
		private string						m_name;

		private void
		Resolve()
		{
			if ( null != m_name )
				return;

			string[] words = SplitWords( GetLine( 0 ) );
			int		 count;
			for ( count=0; count<words.Length; count++ )
				if ( words[count].Equals( "=" ) )
					break;

			m_name = words[--count];

			m_ilType = words[--count];
			if ( words[count - 1] == "unsigned"
			||   words[count - 1] == "class"
			||   words[count - 1] == "interface"
			||   words[count - 1] == "valuetype"
			||   words[count - 1] == "enum" )
				m_ilType = words[--count] + " " + m_ilType;

			for ( int i=1; i<count; i++ )
			{
				if ( words[i].Equals( "private" ) )
				{
					m_isPrivate = true;
				}
				else if ( words[i].Equals( "family" ) )
				{
					m_isProtected = true;
				}
				else if ( words[i].Equals( "public" ) )
				{
					m_isPublic = true;
				}
				else if ( words[i].Equals( "static" ) )
				{
					m_isStatic = true;
				}
				else if ( words[i].Equals( "literal" ) )
				{
					m_isConst = true;
				}
				else if ( words[i].Equals( "modreq" ) )
				{
					if ( (i+2) < count && -1 <= words[i+2].IndexOf( "System.Runtime.CompilerServices.IsVolatile" ) )
					{
						m_isVolatile = true;
					}
				}
			}
		}

		protected override void
		Unresolve()
		{
			m_name = null;
		}


		public bool
		IsPersistent()
		{
			Resolve();

			return CustomElements.Where( c => c.GetAttributeInfo().TypeName == "NDO.NDOTransientAttribute" ).Any();
		}

		public bool
		isPrivate()
		{
			Resolve();

			return m_isPrivate;
		}

		public bool
		isProtected()
		{
			Resolve();

			return m_isProtected;
		}

		public bool
		isPublic()
		{
			Resolve();

			return m_isPublic;
		}

		public bool
		isStatic()
		{
			Resolve();

			return m_isStatic;
		}

		public bool
		isConst()
		{
			Resolve();

			return m_isConst;
		}

		public bool
		isVolatile()
		{
			Resolve();

			return m_isVolatile;
		}

		public string
		getName()
		{
			Resolve();

			return m_name;
		}

		public string
		getILType()
		{
			Resolve();

			return m_ilType;
		}

		public string
		getILTypeName()
		{
			Resolve();

			if ( m_ilType.StartsWith( "class" ) )
				return m_ilType.Substring( 5 ).Trim();
			else if ( m_ilType.StartsWith( "valuetype" ) )
				return m_ilType.Substring( 9 ).Trim();

			return m_ilType;
		}

		public string
		getPureTypeName()
		{
			Resolve();

			int pos = m_ilType.IndexOf( ']' );

			if ( -1 < pos )
				return m_ilType.Substring( pos + 1 );

			return getILTypeName();
		}

	}
}
