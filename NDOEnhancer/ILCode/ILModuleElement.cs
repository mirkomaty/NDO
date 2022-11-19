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


using System;

namespace NDOEnhancer.ILCode
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

		private static ILElementType		m_elementType = new ILModuleElementType();
		
		private string						m_name;
		private bool						m_extern;

		private void
		Resolve()
		{
			if ( null != m_name )
				return;

			string[] words = SplitWords( GetLine( 0 ) );
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
		Unresolve()
		{
			m_name = null;
		}

		public bool
		isExtern()
		{
			Resolve();
		
			return m_extern;
		}
	}
}
