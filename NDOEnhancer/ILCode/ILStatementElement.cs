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
	/// Summary description for ILStatementElement.
	/// </summary>
	internal class ILStatementElement : ILElement
	{
		public ILStatementElement()
			: base()
		{
		}

		public ILStatementElement( string firstLine )
			: base( firstLine )
		{
		}

		public ILStatementElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILStatementElementType : ILElementType
		{
			public ILStatementElementType()
				: base( "IL_", typeof( ILStatementElement ) )
			{
			}
		}

		public class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILStatementElement ) )
			{
			}

			public Iterator( ILElement element, bool recursive )
				: base( element, typeof( ILStatementElement ), recursive )
			{
			}

			public new ILStatementElement
			getNext()
			{
				return base.getNext() as ILStatementElement;
			}
		}

		private static ILElementType		m_elementType = new ILStatementElementType();

		private string						m_name;
		private string						m_signature;
		
		public static void
		initialize()
		{
		}

		public static ILStatementElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		public static ILStatementElement.Iterator
		getIterator( ILElement element, bool recursive )
		{
			return new Iterator( element, recursive );
		}

		public void
		setFirstLine( string firstLine )
		{
            string label = getLabel(this.getLine(0)) + "  ";
			clearLines();
			addLine( label + firstLine );
		}

		private string[]
		dropEmptyWords( string[] words )
		{
			int i;
			int j = 0;
			for ( i=0; i<words.Length; i++ )
				if ( null != words[i] && 0 < words[i].Length )
					j++;

			string[] ret = new string[j];

			j = 0;
			for ( i=0; i<words.Length; i++ )
				if ( null != words[i] && 0 < words[i].Length )
					ret[j++] = words[i];

			return ret;
		}

		private bool
		isMultilineStatement( string name )
		{
			string[] names = { "switch", "call", "callvirt" };

			foreach ( string test in names )
				if ( test == name )
					return true;

			return false;
		}

		public override void
		parse( ILFile ilfile )
		{
			string[] words = dropEmptyWords( getLine( 0 ).Split( new char[] { '\t', ' ' } ) );

			if ( 2 < words.Length && isMultilineStatement( words[1] ) )
			{
				string line = getLine( 0 );

				while ( -1 == line.IndexOf( ')' ) )
				{
					line = ilfile.popLine();
					addLine( line );
				}

				return;
			}

			base.parse( ilfile );
		}

		public string
		getName()
		{
			if ( null != m_name )
				return m_name;

			string[] words = dropEmptyWords( getLine( 0 ).Split( new char[] { '\t', ' ' } ) );

			if ( words[0].EndsWith( ":" ) )
				m_name = words[1];
			else
				m_name = words[0];

			return m_name;
		}

		public bool
		isCallStatement()
		{
			string name = getName();

			return (name == "call") || (name == "callvirt");
		}

		public string
		getCallSignature()
		{
			if ( null != m_signature )
				return m_signature;

			string allLines = "";
			for ( int i=0; i<getLineCount(); i++ )
				allLines = allLines + " " + getLine( i );

			string[] words = splitWords( allLines );
			int		 count;
			for ( count=0; count<words.Length; count++ )
				if ( words[count].Equals( "(" ) )
					break;

			string	  paramType		 = "";
			ArrayList parameterTypes = new ArrayList();

			for ( int i=count+1; i<words.Length; i++ )
			{
				if ( words[i] == "," || words[i] == ")" )
				{
					if ( 0 < paramType.Length )
						parameterTypes.Add( paramType );
					
					if ( words[i] == ")" )
						break;

					paramType = "";
				}
				else
				{
					paramType = (paramType + " " + words[i]).Trim();
				}
			}

			string fullname = words[--count];

			m_signature = fullname + "(";

			for ( int i=0; i<parameterTypes.Count; i++ )
			{
				if ( 0 < i )
					m_signature = m_signature + ", ";

				string type = makeFullType( parameterTypes[i] as string );

				m_signature = m_signature + type;
			}

			m_signature = m_signature + ")";

			return m_signature;
		}

		public void
		makeUpperNameSignature()
		{
			getCallSignature();		// let it resolve

			int pos1 = m_signature.IndexOf( "::" );

			if ( pos1 < 0 )
				return;

			int pos2 = m_signature.IndexOf( "(", pos1 );

			if ( pos2 < 0 )
				return;

			string oldName = m_signature.Substring( pos1 + 2, pos2 - pos1 - 2 );
			string newName = makeUpperCaseName( oldName );

			if ( oldName == newName )
				return;

			replaceTextOnce( "::" + oldName + "(", "::" + newName + "(" );

			m_signature = m_signature.Substring( 0, pos1 + 2 ) + newName + m_signature.Substring( pos2 );
		}
	}
}
