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

		private static ILElementType		m_elementType = new ILStatementElementType();

		private string						m_name;
		private string						m_signature;
		
		public void
		setFirstLine( string firstLine )
		{
            string label = GetLabel(this.GetLine(0)) + "  ";
			ClearLines();
			AddLine( label + firstLine );
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
		Parse( ILFile ilfile )
		{
			string[] words = dropEmptyWords( GetLine( 0 ).Split( new char[] { '\t', ' ' } ) );

			if ( 2 < words.Length && isMultilineStatement( words[1] ) )
			{
				string line = GetLine( 0 );

				while ( -1 == line.IndexOf( ')' ) )
				{
					line = ilfile.popLine();
					AddLine( line );
				}

				return;
			}

			base.Parse( ilfile );
		}

		public string
		getName()
		{
			if ( null != m_name )
				return m_name;

			string[] words = dropEmptyWords( GetLine( 0 ).Split( new char[] { '\t', ' ' } ) );

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
			for ( int i=0; i<GetLineCount(); i++ )
				allLines = allLines + " " + GetLine( i );

			string[] words = SplitWords( allLines );
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

				string type = MakeFullType( parameterTypes[i] as string );

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
			string newName = MakeUpperCaseName( oldName );

			if ( oldName == newName )
				return;

			ReplaceTextOnce( "::" + oldName + "(", "::" + newName + "(" );

			m_signature = m_signature.Substring( 0, pos1 + 2 ) + newName + m_signature.Substring( pos2 );
		}
	}
}
