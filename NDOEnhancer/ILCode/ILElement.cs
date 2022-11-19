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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILElement.
	/// </summary>
	internal class ILElement
	{
		public ILElement()
		{
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

		private ILElement						m_owner		= null;
		private List<string>					m_lines		= new List<string>();
		private List<ILElement>					m_elements  = new List<ILElement>();
		private List<ILClassElement>			m_classElements = null;
		private string							m_assemblyName;
		private IEnumerable<ILCustomElement>	m_customElements = null;

		private static List<ILElementType>		m_elementTypes = new List<ILElementType>();

		private enum					Status{ normal, escape, quote, ccomment, cppcomment };

		public static void AddElementType( ILElementType elementType )
		{
			if (!m_elementTypes.Any( et => et.GetType() == elementType.GetType() ))
				m_elementTypes.Add( elementType );
		}

		public ILElement
		GetElement( string line )
		{
			foreach ( ILElementType elementType in m_elementTypes )
			{
				if ( elementType.IsElement( line ) )
					return elementType.CreateElement( line, this );
			}

			return null;
		}

		public string GetAllLines()
		{
			string result = "";
			foreach (string line in m_lines)
				result += line + " ";
			if ( result == string.Empty )
				return string.Empty;
            return result.Substring(0, result.Length - 1); ;
		}

		public ILElement
		FindElement(string pattern)
		{
			foreach (ILElement el in m_elements)
			{
				if (el.GetAllLines().IndexOf(pattern) > -1)
					return el;
			}
			return null;
		}


		public ILLocalsElement GetLocals()
		{
			foreach(ILElement el in this.m_elements)
				if (el is ILLocalsElement)
					return (ILLocalsElement) el;
			return null;
		}


		public void
		ParseSubElements( ILFile ilfile )
		{
			m_elements = new List<ILElement>();

			for ( string line = ilfile.PopLine(); null != line; line = ilfile.PopLine() )
			{
				if ( line.StartsWith( "}" ) )
					return;

				ILElement element = GetElement( line );

				if ( null != element )
				{
					m_elements.Add( element );
					element.Parse( ilfile );
				}
			}
		}

		public virtual void
		Parse( ILFile ilfile )
		{
			string nextLine = ilfile.PopLine();

			if ( null == nextLine )
				return;

			if ( nextLine.StartsWith( "." ) )
			{
				ilfile.PushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "IL_" ) )
			{
				ilfile.PushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "{" ) )
			{
				ParseSubElements( ilfile );
			}
			else if ( nextLine.StartsWith( "}" ) )
			{
				ilfile.PushLine( nextLine );
			}
			else if ( nextLine.StartsWith( "//" ) )
			{
				Parse( ilfile );
			}
			else
			{
				m_lines.Add( nextLine );
				Parse( ilfile );
			}
		}

		public void
		WriteSubElements( ILFile ilfile, int level, bool isNetStandard )
		{
			foreach ( ILElement element in m_elements )
			{
				element.Write( ilfile, level, isNetStandard );
			}
		}

		public void
		Write( ILFile ilfile, int level, bool isNetStandard )
		{
			//			ilfile.writeLine( 0, "// " + GetType().Name );

			foreach (string line in m_lines)
			{
				var outputLine = line;
				// even if the dll is a netstandard dll we analyze it
				// using the .NET Fx, which has dependencies into the mscorlib.
				if (isNetStandard)
					outputLine = line.Replace( "[mscorlib]", "[netstandard]" );
				ilfile.writeLine( level, outputLine );
			}

			// each element having a block will have at least one subelement
			if ( m_elements.Count == 0 )
				return;

			ilfile.writeLine( level, "{" );

			WriteSubElements( ilfile, level + 1, isNetStandard );

			ilfile.writeLine( level, "}" );
		}

		public ILElement Owner
		{
			get => this.m_owner;
			set => this.m_owner = value;
		}

		internal ILElement
		RootOwner
		{
			get
			{
				if (null != m_owner)
					return m_owner.RootOwner;
				else
					return this;
			}
		}

		public int LineCount => m_lines.Count;

		public string
		GetLine( int index )
		{
			return m_lines[index] as string;
		}

		protected void
		ClearLines()
		{
			m_lines.Clear();
		}

		public void
		ClearElements()
		{
			m_elements.Clear();
		}

		public void
		AddLine( string line )
		{
			m_lines.Add( line );
		}

		public void
		AppendLastLine( string text )
		{
			int lastIndex = m_lines.Count - 1;
			string line = m_lines[lastIndex] + text;

			m_lines[lastIndex] = line;
		}

		public void
		ReplaceText( string oldText, string newText )
		{
			for ( int i = 0; i < LineCount; i++ )
			{
				string oldLine = GetLine( i );
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

					Unresolve();
				}
			}

			for ( int i=0; i<GetSubElementCount(); i++ )
			{
				GetElement( i ).ReplaceText( oldText, newText );
			}
		}

		protected virtual void
		Unresolve()
		{
		}

		public void
		ReplaceTextOnce( string oldText, string newText )
		{
			for ( int i = 0; i < LineCount; i++ )
			{
				string line = GetLine( i );

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
		GetSubElementCount()
		{
			return m_elements.Count;
		}

		public ILElement
		GetElement( int index )
		{
			return m_elements[index];
		}

		public virtual bool
		InsertBefore( ILElement insertElement, ILElement existingElement )
		{
			for ( int i=0; i<m_elements.Count; i++ )
			{
				if ( m_elements[i] == existingElement )
				{
					insertElement.Owner = this;
					m_elements.Insert( i, insertElement );
					return true;
				}
			}

			return false;
		}

		public bool
		InsertBefore( ILElement insertElement )
		{
			if ( null == m_owner )
				return false;

			return m_owner.InsertBefore( insertElement, this );
		}

		protected virtual bool
		InsertAfter( ILElement insertElement, ILElement existingElement )
		{
			for ( int i=0; i<m_elements.Count; i++ )
			{
				if ( m_elements[i] == existingElement )
				{
					insertElement.Owner = this;
					if ( (i + 1) < m_elements.Count )
						m_elements.Insert( i + 1, insertElement );
					else
						m_elements.Add( insertElement );

					return true;
				}
			}

			return false;
		}

		protected virtual void AppendElement(ILElement insertElement)
		{
			m_elements.Add( insertElement );
		}

		public bool
		InsertAfter( ILElement insertElement )
		{
			if ( null == m_owner )
				return false;

			return m_owner.InsertAfter( insertElement, this );
		}

		protected void
		Remove( ILElement removeElement )
		{
			removeElement.Owner = null;
			m_elements.Remove( removeElement );
			if (removeElement is ILClassElement clel)
				m_classElements.Remove(clel);
		}

		public void
		Remove()
		{
			if ( null == m_owner )
				return;

			m_owner.Remove( this );
		}

		public void
		AddElement( ILElement insertElement )
		{
			insertElement.Owner = this;
			if (insertElement is ILClassElement classElement)
			{
				if (m_classElements == null)
				{
					// try to get class elements from m_elements
					GetAllClassElements();
				}
				m_classElements.Add(classElement);
			}
			m_elements.Add( insertElement );
		}

		public IEnumerable<ILClassElement>
		GetAllClassElements()
		{
			if (m_classElements != null)
				return m_classElements;

			var classes		 = new List<ILClassElement>();

			GetAllClassElements( classes );
			m_classElements = classes;
			return classes;
		}

		internal void
		GetAllClassElements( List<ILClassElement> classes )
		{
//			Debug.WriteLine("Type: " + this.GetType().Name);
//			Debug.Indent();
			foreach ( var element in m_elements  )
			{
                ILClassElement classElement = element as ILClassElement;
                if (classElement != null)
				{
                    classes.Add(classElement);
				}
				
				element.GetAllClassElements( classes );
			}
//			Debug.Unindent();
		}

		protected string
		StripComment( string text )
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
		SplitWords( string line )
		{
			line = StripComment( line );
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
		TypeFromIL( string typeName )
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
				string assName = StripComment( typeName.Substring( 1, end - 1 ) );

				return Type.GetType( typeName.Substring( end + 1 ) + ", " + assName );
			}
			else
			{
				return Type.GetType( typeName );
			}
		}

		public virtual string
		MakeUpperCaseName( string name )
		{
			string oldName = name;

			if ( name.StartsWith( "'" ) && name.EndsWith( "'" ) )
				name = name.Substring( 1, name.Length - 2 );

			if ( ! Char.IsLower( name[0] ) )
				return oldName;

			return Char.ToUpper( name[0] ) + name.Substring( 1 );
		}

		protected string
		MakeFullType( string type )
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

			return typetype + " [" + m_assemblyName + "]" + typename;
		}

		public string
		AssemblyName
		{
			get=> m_assemblyName;
			set
			{
				m_assemblyName = value;

				foreach (ILElement element in m_elements)
				{
					element.AssemblyName = value;
				}

			}
		}

		public virtual void
		RemoveAssemblyReference( string assemblyName )
		{
			string reference = "[" + assemblyName + "]";

			for ( int i=0; i<m_lines.Count; i++ )
			{
				m_lines[i] = (m_lines[i] as string).Replace( reference, "" );
			}

			foreach ( ILElement element in m_elements )
			{
				element.RemoveAssemblyReference( assemblyName );
			}
		}

		public ILElement GetSuccessor()
		{
			return m_owner.GetSuccessorOf(this);
		}

		public ILElement GetPredecessor()
		{
			return m_owner.GetPredecessorOf(this);
		}

		public ILElement GetPredecessorOf(ILElement elementToSearch)
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
		GetSuccessorOf( ILElement elementToSearch )
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
		StripLabel(string s)
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
        GetLabel(string s)
        {
            if (!s.Trim().StartsWith("IL_"))
                return string.Empty;
            int pos = s.IndexOf(':');
            return s.Substring(0, pos + 1);
        }

		public IEnumerable<ILElement> Elements => m_elements;

		public IEnumerable<ILElement> GetElementsRecursive()
		{
			List<ILElement> elements = new List<ILElement>();
			GetElementsRecursive( elements );
			return elements;
		}

		public void GetElementsRecursive(List<ILElement> elements)
		{
			elements.AddRange( m_elements );
			foreach (var el in m_elements)
			{
				el.GetElementsRecursive( elements );
			}
		}

		//TODO .event elements currently don't have an own element type.
		static Type[] childCustomOwners = {typeof(ILClassElement), typeof(ILAssemblyElement), typeof(ILPropertyElement) };
		//TODO: parameters need to have an own ILParamElement, to manage custom attributes
		//      currently we can omit this type because NDO doesnt use or alter custom attributes of params.
		//		note: Return values are handled like params at position 0.
		//		note: Generic params are special forms of params (.param type T)
		static Type[] memberCustomOwners = {typeof(ILModuleElement), typeof(ILFieldElement)};

		public IEnumerable<ILCustomElement> CustomElements
		{
			get
			{
				if (m_customElements != null)
					return m_customElements;

				Type t = GetType();
				if (childCustomOwners.Contains( t ))
					return ComputeChildCustomElements();
				if (memberCustomOwners.Contains( t ))
					return ComputeMemberCustomElements();

				// return an empty list for owners, which cannot have CAs
				return m_customElements = new List<ILCustomElement>();

			}
		}

		/// <summary>
		/// Compute all custom elements of an element, if custom elements
		/// are defined as child elements of an element, as is in .assembly and .class.
		/// </summary>
		private IEnumerable<ILCustomElement> ComputeChildCustomElements()
		{
			var customElements = new List<ILCustomElement>();
			m_customElements = customElements;
			// custom elements are the very first child elements
			// If there is a child element of another type, it may have
			// its own custom element. The declaration follows immediately after
			// the child element declaration.
			foreach (var childElement in m_elements)
			{
				if (childElement is ILCustomElement customElement)
				{
					customElements.Add( customElement );
				}
				else
				{
					break;
				}
			}

			return m_customElements;
		}

		/// <summary>
		/// Get all custom elements of an element, if custom elements
		/// are defined for class members like .field or .property.
		/// </summary>
		private IEnumerable<ILCustomElement> ComputeMemberCustomElements()
		{
			var customElements = new List<ILCustomElement>();
			m_customElements = customElements;
			// custom elements of members follow immediately after
			// the element declaration.
			IEnumerator<ILElement> elements = m_owner.Elements.GetEnumerator();
			elements.MoveNext();
			while (elements.Current != null && elements.Current != this)
				elements.MoveNext();

			if (elements.Current == null)
				return m_customElements; // empty

			// Current points now to the .field or .property declaration
			elements.MoveNext();	// Move to the next element, which should be the first .custom element
									// if there are any .custom elements

			while (elements.Current != null && elements.Current is ILCustomElement customElement)
			{
				customElements.Add( customElement );
				elements.MoveNext();
			}

			return m_customElements;
		}
	}
}
