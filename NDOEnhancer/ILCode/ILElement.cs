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
using System.Diagnostics;
using System.Collections;
using System.Text;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILElement.
	/// </summary>
	internal class ILElement
	{
		public ILElement()
		{
		}

		public ILElement( bool hasElements )
		{
			if ( hasElements )
				m_elements = new ArrayList();
		}

		public ILElement( string firstLine )
		{
			m_lines.Add( firstLine );
		}

		public ILElement( string firstLine, ILElement owner )
		{
			m_owner = owner;
			m_lines.Add( firstLine );
		}

		private ILElement				m_owner		= null;
		private ArrayList				m_lines		= new ArrayList();
		private ArrayList				m_elements	= null;
		private ArrayList				m_classElements = null;
		private string					m_assemblyName;

		private static ArrayList		m_elementTypes = new ArrayList();

		private enum					Status{ normal, escape, quote, ccomment, cppcomment };

		public static void
		addElementType( ILElementType elementType )
		{
			m_elementTypes.Add( elementType );
		}

		public ILElement
		getElement( string line )
		{
			foreach ( ILElementType elementType in m_elementTypes )
			{
				if ( elementType.isElement( line ) )
					return elementType.createElement( line, this );
			}

			return null;
		}

		public string getAllLines()
		{
			string result = "";
			foreach (string line in m_lines)
				result += line + " ";
			if ( result == string.Empty )
				return string.Empty;
            return result.Substring(0, result.Length - 1); ;
		}

		public ILElement
		findElement(string pattern)
		{
			foreach (ILElement el in m_elements)
			{
				if (el.getAllLines().IndexOf(pattern) > -1)
					return el;
			}
			return null;
		}


		public ILLocalsElement getLocals()
		{
			foreach(ILElement el in this.m_elements)
				if (el is ILLocalsElement)
					return (ILLocalsElement) el;
			return null;
		}


		public void
		parseSubElements( ILFile ilfile )
		{
			m_elements = new ArrayList();

			for ( string line = ilfile.popLine(); null != line; line = ilfile.popLine() )
			{
				if ( line.StartsWith( "}" ) )
					return;

				ILElement element = getElement( line );

				if ( null != element )
				{
					m_elements.Add( element );
					element.parse( ilfile );
				}
			}
		}

		public virtual void
		parse( ILFile ilfile )
		{
			string nextLine = ilfile.popLine();

			if ( null == nextLine )
				return;

			if ( nextLine.StartsWith( "." ) )
			{
				ilfile.pushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "IL_" ) )
			{
				ilfile.pushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "{" ) )
			{
				parseSubElements( ilfile );
			}
			else if ( nextLine.StartsWith( "}" ) )
			{
				ilfile.pushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "//" ) )
			{
				parse( ilfile );
			}
			else
			{
				m_lines.Add( nextLine );
				parse( ilfile );
			}
		}

		public void
		writeSubElements( ILFile ilfile, int level )
		{
			if ( null == m_elements )
				return;

			foreach ( ILElement element in m_elements )
			{
				element.write( ilfile, level );
			}
		}

		public void
		write( ILFile ilfile, int level )
		{
//			ilfile.writeLine( 0, "// " + GetType().Name );

			foreach ( string line in m_lines )
				ilfile.writeLine( level, line );

			if ( null == m_elements )
				return;

			ilfile.writeLine( level, "{" );

			writeSubElements( ilfile, level + 1 );

			ilfile.writeLine( level, "}" );
		}

		public ILElement
		getOwner()
		{
			return m_owner;
		}

		private void
		setOwner( ILElement owner )
		{
			m_owner = owner;
		}

		internal ILElement
		getRootOwner()
		{
			if ( null != m_owner )
				return m_owner.getRootOwner();
			else
				return this;
		}

		public int
		getLineCount()
		{
			return m_lines.Count;
		}

		public string
		getLine( int index )
		{
			return m_lines[index] as string;
		}

		protected void
		clearLines()
		{
			m_lines.Clear();
		}

		public void
		clearElements()
		{
			m_elements.Clear();
		}

		public void
		addLine( string line )
		{
			m_lines.Add( line );
		}

		public void
		appendLastLine( string text )
		{
			int lastIndex = m_lines.Count - 1;
			string line = m_lines[lastIndex] + text;

			m_lines[lastIndex] = line;
		}

		public void
		replaceText( string oldText, string newText )
		{
			for ( int i=0; i<getLineCount(); i++ )
			{
				string oldLine = getLine( i );
				string newLine = "";
				int	   index;

				if ( -1 < (index = oldLine.IndexOf( oldText )) )
				{
					while ( -1 < (index = oldLine.IndexOf( oldText )) )
					{
						bool wholeWord = true;
						if ( 0 < index && Char.IsLetterOrDigit( oldLine, index - 1 ) )
							wholeWord = false;
						if ( wholeWord && index + oldText.Length < oldLine.Length && Char.IsLetterOrDigit( oldLine, index + oldText.Length ) )
							wholeWord = false;

						if ( wholeWord )
						{
							newLine = newLine + oldLine.Substring( 0, index ) + newText;
							oldLine = oldLine.Substring( index + oldText.Length );
						}
						else
						{
							newLine = newLine + oldLine.Substring( 0, index + 1 );
							oldLine = oldLine.Substring( index + 1 );
						}
					}

					m_lines[i] = newLine + oldLine;

					unresolve();
				}
			}

			for ( int i=0; i<getSubElementCount(); i++ )
			{
				getElement( i ).replaceText( oldText, newText );
			}
		}

		protected virtual void
		unresolve()
		{
		}

		public void
		replaceTextOnce( string oldText, string newText )
		{
			for ( int i=0; i<getLineCount(); i++ )
			{
				string line = getLine( i );

				int pos = line.IndexOf( oldText );

				if ( -1 < pos )
				{
					line = line.Substring( 0, pos ) + newText + line.Substring( pos + oldText.Length );

					m_lines[i] = line;

					return;
				}
			}
		}

		public int
		getSubElementCount()
		{
			if ( null == m_elements )
				return 0;

			return m_elements.Count;
		}

		public ILElement
		getElement( int index )
		{
			if ( null == m_elements )
				return null;

			if ( index < 0 || m_elements.Count <= index )
				return null;

			return m_elements[index] as ILElement;
		}

		public bool
		insertBefore( ILElement insertElement, ILElement existingElement )
		{
			for ( int i=0; i<m_elements.Count; i++ )
			{
				if ( m_elements[i] == existingElement )
				{
					insertElement.setOwner( this );
					m_elements.Insert( i, insertElement );
					return true;
				}
			}

			return false;
		}

		public bool
		insertBefore( ILElement insertElement )
		{
			if ( null == getOwner() )
				return false;

			return getOwner().insertBefore( insertElement, this );
		}

		protected bool
		insertAfter( ILElement insertElement, ILElement existingElement )
		{
			for ( int i=0; i<m_elements.Count; i++ )
			{
				if ( m_elements[i] == existingElement )
				{
					insertElement.setOwner( this );
					if ( (i + 1) < m_elements.Count )
						m_elements.Insert( i + 1, insertElement );
					else
						m_elements.Add( insertElement );

					return true;
				}
			}

			return false;
		}

		public bool
		insertAfter( ILElement insertElement )
		{
			if ( null == getOwner() )
				return false;

			return getOwner().insertAfter( insertElement, this );
		}

		protected void
		remove( ILElement removeElement )
		{
			removeElement.setOwner( null );
			m_elements.Remove( removeElement );
			if (removeElement is ILClassElement)
				m_classElements.Remove(removeElement);
		}

		public void
		remove()
		{
			if ( null == getOwner() )
				return;

			getOwner().remove( this );
		}

		public void
		addElement( ILElement insertElement )
		{
			if ( null == m_elements )
				m_elements = new ArrayList();

			insertElement.setOwner( this );
			if (insertElement is ILClassElement)
			{
				if (m_classElements == null)
				{
					// Erst versuchen, vorhandene Elemente zu finden
					this.getAllClassElements();
					// Keine gefunden? Neue Arraylist anlegen
					if (m_classElements == null)
						m_classElements = new ArrayList();
				}
				m_classElements.Add(insertElement);
			}
			m_elements.Add( insertElement );
		}

		public IList
		getAllClassElements()
		{
			if (m_classElements != null)
				return m_classElements;

			ArrayList forwardClasses = new ArrayList();
			ArrayList classes		 = new ArrayList();

			getAllClassElements( forwardClasses, classes );
			m_classElements = classes;
			return classes;
		}

		internal void
		getAllClassElements( ArrayList forwardClasses, ArrayList classes )
		{
			if ( null == m_elements )
				return;

//			Debug.WriteLine("Type: " + this.GetType().Name);
//			Debug.Indent();
			for ( int i = 0; i < m_elements.Count; i++ )
			{
                ILClassElement classElement = m_elements[i] as ILClassElement;
                if (classElement != null)
				{
#if NET20
                    classes.Add(classElement);
#else
					string classname = classElement.getClassFullName();
//					Debug.Write(classname + " -> ");
					for ( int j = 0; j < forwardClasses.Count; j++ )
					{
						ILClassElement forwardClassElement = forwardClasses[j] as ILClassElement;
						string forwardClassname = forwardClassElement.getClassFullName();

						if ( classname.Equals( forwardClassname ) )
						{
							forwardClasses.RemoveAt( j );
							classElement.setForwardClassElement( forwardClassElement );
//							Debug.WriteLine(forwardClassname);

							break;
						}
					}

					if ( null == classElement.getForwardClassElement() )
					{
//						Debug.WriteLine("nix");
						forwardClasses.Add( classElement );
					}
					else
						classes.Add( classElement );
#endif
				}
				
				(m_elements[i] as ILElement).getAllClassElements( forwardClasses, classes );
			}
//			Debug.Unindent();
		}

		public ILStatementElement[]
		getAllStatementElements()
		{
			ArrayList statements = new ArrayList();

			getAllStatementElements( statements );

			return statements.ToArray( typeof( ILStatementElement) ) as ILStatementElement[];
		}

		internal void
		getAllStatementElements( ArrayList statements )
		{
			if ( null == m_elements )
				return;

			for ( int i=0; i<m_elements.Count; i++ )
			{
				ILClassElement classElement = m_elements[i] as ILClassElement;
				if ( null != classElement )
				{
					continue;
				}

				ILStatementElement statementElement = m_elements[i] as ILStatementElement;
				if ( null != statementElement )
				{
					statements.Add( statementElement );
					continue;
				}
				
				(m_elements[i] as ILElement).getAllStatementElements( statements );
			}
		}

		public ILElementIterator
		getAllIterator(bool recursive)
		{
			return new ILElementIterator( this, typeof( ILElement ), recursive );
		}

		public ILAssemblyElement.Iterator
		getAssemblyIterator()
		{
			return ILAssemblyElement.getIterator( this );
		}

		public ILCustomElement.Iterator
		getCustomIterator()
		{
			return ILCustomElement.getIterator( this );
		}

		public ILModuleElement.Iterator
		getModuleIterator()
		{
			return ILModuleElement.getIterator( this );
		}
		
		public ILPublickeytokenElement.Iterator
		getPublickeytokenIterator()
		{
			return ILPublickeytokenElement.getIterator( this );
		}
		
		public ILPublickeyElement.Iterator
		getPublickeyIterator()
		{
			return ILPublickeyElement.getIterator( this );
		}
		
		public ILNamespaceElement.Iterator
		getNamespaceIterator()
		{
			return ILNamespaceElement.getIterator( this );
		}

		public ILClassElement.Iterator
		getClassIterator()
		{
			return ILClassElement.getIterator( this );
		}

		public ILFieldElement.Iterator
		getFieldIterator()
		{
			return ILFieldElement.getIterator( this );
		}

		public ILPropertyElement.Iterator
		getPropertyIterator()
		{
			return ILPropertyElement.getIterator( this );
		}

		public ILMethodElement.Iterator
		getMethodIterator()
		{
			return ILMethodElement.getIterator( this );
		}

		public ILTryElement.Iterator
		getTryIterator()
		{
			return ILTryElement.getIterator( this );
		}

		public ILCatchElement.Iterator
		getCatchIterator()
		{
			return ILCatchElement.getIterator( this );
		}

		public ILStatementElement.Iterator
		getStatementIterator(bool recursive)
		{
			return ILStatementElement.getIterator( this, recursive );
		}

		public ILStatementElement.Iterator
		getStatementIterator()
		{
			return ILStatementElement.getIterator( this );
		}

		public ILLineElement.Iterator
		getLineIterator()
		{
			return ILLineElement.getIterator( this );
		}

		public ILUnknownElement.Iterator
		getUnknownElement()
		{
			return ILUnknownElement.getIterator( this );
		}

		protected string
		stripComment( string text )
		{
			if ( text.IndexOf( "/*" ) < 0 && text.IndexOf( "//" ) < 0 )	// no comment
			{
				return text.Trim();
			}
			else if ( text.IndexOf( "\"" ) < 0 )	// comment, but no quote
			{
				for ( ;; )
				{
					int start = text.IndexOf( "/*" );
					int end	  = text.IndexOf( "*/" );

					if ( start <= -1 || end <= start )
						break;

					text = (text.Substring( 0, start ) + text.Substring( end + 2 ));
				}

				int pos = text.IndexOf( "//" );
				if ( -1 < pos )
					text = text.Substring( 0, pos );

				return text.Trim();
			}
			else	// quote and perhaps a comment
			{
				StringBuilder newText	= new StringBuilder( text.Length );
				Status		  status	= Status.normal;
				Status		  oldStatus	= Status.normal;
				
				for ( int i=0; i<text.Length; i++ )
				{
					switch ( status )
					{
					case Status.normal:
						if ( '\\' == text[i] )
						{
							oldStatus = status;
							status	  = Status.escape;
							newText.Append( text[i] );
						}
						else if ( '\"' == text[i] )
						{
							status	= Status.quote;
							newText.Append( text[i] );
						}
						else if ( '/' == text[i] && (i+1) < text.Length && '*' == text[i+1] )
						{
							status = Status.ccomment;
							i++;
						}
						else if ( '/' == text[i] && (i+1) < text.Length && '/' == text[i+1] )
						{
							status = Status.cppcomment;
							return newText.ToString().Trim();
						}
						else
						{
							newText.Append( text[i] );
						}

						break;

					case Status.escape:
						status  = oldStatus;
						newText.Append( text[i] );

						break;

					case Status.quote:
						if ( '\"' == text[i] )
							status = Status.normal;
						
						newText.Append( text[i] );
						
						break;
					
					case Status.ccomment:
						if ( '*' == text[i] && (i+1) < text.Length && '/' == text[i+1] )
						{
							status = Status.normal;
							i++;
						}

						break;
					}
				}

				return newText.ToString().Trim();
			}
		}

        
		protected string[]
		splitWords( string line )
		{
			line = stripComment( line );
			line = line.Replace( "(", " ( " );
			line = line.Replace( ")", " ) " );
			line = line.Replace( "=", " = " );
			line = line.Replace( ",", " , " );

			string[] words = line.Split( new char[] { ' ', '\t' } );
			int		 count = 0;

			// strip empty words

			for ( int i=0; i<words.Length; i++ )
				if ( 0 < words[i].Length )
					count++;

			string[] realWords = new string[count];

			count = 0;
			for ( int i=0; i<words.Length; i++ )
				if ( 0 < words[i].Length )
					realWords[count++] = words[i];

			return realWords;
		}
        
		protected Type
		typeFromIL( string typeName )
		{
			typeName = typeName.Trim();

			if ( typeName == "bool" )
				return Type.GetType( "System.Boolean" );
			else if ( typeName == "byte" )
				return Type.GetType( "System.Byte" );
			else if ( typeName == "sbyte" )
				return Type.GetType( "System.SByte" );
			else if ( typeName == "char" )
				return Type.GetType( "System.Char" );
			else if ( typeName == "unsigned char" )
				return Type.GetType( "System.UChar" );
			else if ( typeName == "short" || typeName == "int16" )
				return Type.GetType( "System.Int16" );
			else if ( typeName == "unsigned int16" )
				return Type.GetType( "System.UInt16" );
			else if ( typeName == "int" || typeName == "int32" )
				return Type.GetType( "System.Int32" );
			else if ( typeName == "unsigned int32" )
				return Type.GetType( "System.UInt32" );
			else if ( typeName == "long" || typeName == "int64" )
				return Type.GetType( "System.Int64" );
			else if ( typeName == "unsigned int64" )
				return Type.GetType( "System.UInt64" );
			else if ( typeName == "float" || typeName == "single" )
				return Type.GetType( "System.Single" );
			else if ( typeName == "double" )
				return Type.GetType( "System.Double" );
			else if ( typeName == "string" )
				return Type.GetType( "System.String" );
			else if ( typeName[0] == '[' )
			{
				int	   end	   = typeName.IndexOf( "]", 1 );
				string assName = stripComment( typeName.Substring( 1, end - 1 ) );

				return Type.GetType( typeName.Substring( end + 1 ) + ", " + assName );
			}
			else
			{
				return Type.GetType( typeName );
			}
		}

		public virtual string
		makeUpperCaseName( string name )
		{
			string oldName = name;

			if ( name.StartsWith( "'" ) && name.EndsWith( "'" ) )
				name = name.Substring( 1, name.Length - 2 );

			if ( ! Char.IsLower( name[0] ) )
				return oldName;

			return Char.ToUpper( name[0] ) + name.Substring( 1 );
		}

		protected string
		makeFullType( string type )
		{
			string typetype;
			string typename;

			if ( type.StartsWith( "class" ) )
			{
				typetype = "class";
				typename = type.Substring( 5 ).Trim();
			}
			else if ( type.StartsWith( "valuetype" ) )
			{
				typetype = "valuetype";
				typename = type.Substring( 9 ).Trim();
			}
			else
			{
				return type;
			}

			if ( typename[0] == '[' )
				return type;

			return typetype + " [" + getAssemblyName() + "]" + typename;
		}

		public void
		setAssemblyName( string assemblyName )
		{
			m_assemblyName = assemblyName;

			if ( null == m_elements )
				return;

			foreach ( ILElement element in m_elements )
			{
				element.setAssemblyName( assemblyName );
			}
		}

		public string
		getAssemblyName()
		{
			return m_assemblyName;
		}

		public virtual void
		removeAssemblyReference( string assemblyName )
		{
			string reference = "[" + assemblyName + "]";

			for ( int i=0; i<m_lines.Count; i++ )
			{
				m_lines[i] = (m_lines[i] as string).Replace( reference, "" );
			}

			if ( null == m_elements )
				return;

			foreach ( ILElement element in m_elements )
			{
				element.removeAssemblyReference( assemblyName );
			}
		}

		public ILElement getSuccessor()
		{
			ILElement Owner = getOwner();
			return Owner.getSuccessorOf(this);
		}

		public ILElement getPredecessor()
		{
			ILElement Owner = getOwner();
			return Owner.getPredecessorOf(this);
		}

		public ILElement getPredecessorOf(ILElement elementToSearch)
		{
			ILElement lastEl = null;
			for (int i = 0; i < m_elements.Count; i++)
			{
				if (m_elements[i] == elementToSearch)
				{
					return lastEl;
				}
				lastEl = (ILElement) m_elements[i];
			}
			return null;
		}


		public ILElement 
		getSuccessorOf( ILElement elementToSearch )
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				if (m_elements[i] == elementToSearch)
				{
					if (i == m_elements.Count - 1) 
						return null;
					else
						return (ILElement) m_elements[i + 1];
				}
			}
			return null;
		}


		public static string 
		stripLabel(string s)
		{
			if (s.Trim().StartsWith("IL_"))
			{
				int pos = s.IndexOf( ':' );
				return s.Substring( pos + 1 ).Trim();
			}
			else
				return s;
		}

        public static string
        getLabel(string s)
        {
            if (!s.Trim().StartsWith("IL_"))
                return string.Empty;
            int pos = s.IndexOf(':');
            return s.Substring(0, pos + 1);
        }


	}
}
