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
using System.Collections.Generic;
using NDOEnhancer;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILClassElement.
	/// </summary>
	internal class ILClassElement : ILElement
	{
		public ILClassElement()
			: base( true )
		{
		}

		public ILClassElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILClassElementType : ILElementType
		{
			public ILClassElementType()
				: base( ".class", typeof( ILClassElement ) )
			{
			}
		}

		internal class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILClassElement ) )
			{
			}

			public new ILClassElement
			getFirst()
			{
				return base.getFirst() as ILClassElement;
			}

			public new ILClassElement
			getNext()
			{
				return base.getNext() as ILClassElement;
			}
		}

		private string						m_name;
        private string                      m_genericArguments = string.Empty;
		private string						m_fullName = null;
		private string						m_baseFullName;
#if NET11
		private ILClassElement				m_forwardClassElement = null;
#endif
		private bool						m_isPublic			  = false;
		private bool						m_isPrivate			  = false;
		private bool						m_isSealed			  = false;
		private bool						m_isInterface		  = false;
		private bool						m_isValueType		  = false;
		private bool						m_isEnum			  = false;
		private bool						m_isAbstract		  = false;
        private string                      internalNamespace     = string.Empty;

		private static ILElementType		m_elementType		  = new ILClassElementType();
		
		public static void
		initialize()
		{
		}

		public static ILClassElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		public void AddImplements(string[] interfaceNames)
		{
			List<string> lines = new List<string>();
			int implPosition = -1;
			for ( int i = 0; i < this.getLineCount(); i++ )
			{
				string s = this.getLine( i );
				if ( s.StartsWith( "implements" ) )
					implPosition = i;
				lines.Add( s );
			}

			if (implPosition == -1)
			{
				implPosition = lines.Count;
				addLine("implements ");
			}
			
			string implString = string.Empty;
			for ( int i = implPosition; i < lines.Count; i++ )
			{
				if ( i == implPosition )
					implString += lines[i].Substring( 11 ) + ' '; // "implements "
				else
					implString += lines[i] + ' ';
			}
			implString = implString.Trim();
			string[] existingImplements = implString.Split( new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries );
			List<string> remainingImplements = new List<string>();
			foreach ( string impl1 in interfaceNames )
			{
				bool exists = false;
				foreach ( string impl2 in existingImplements )
				{
					if ( impl1 == impl2 )
					{
						exists = true;
						break;
					}
				}
				if (!exists)
					remainingImplements.Add( impl1 );
			}
			if ( remainingImplements.Count == 0 )
				return;

			this.clearLines();
			for ( int i = 0; i < implPosition; i++ )
				this.addLine( lines[i] );
			remainingImplements.InsertRange( 0, existingImplements );
			for ( int i = 0; i < remainingImplements.Count; i++ )
			{
				string s = string.Empty;
				if ( i == 0 )
					s = "implements ";
				s += remainingImplements[i];
				if ( i < remainingImplements.Count - 1 )
					s += ", ";
				this.addLine( s );
			}
		}

		private void
		resolve()
		{
			if ( null != m_name )
				return;

			string[] words = splitWords( getLine( 0 ) );
			int		 count = words.Length;

			m_name = words[--count];
            
            int p = m_name.LastIndexOf('.');
            if (p > -1)
            {
                m_fullName = m_name;
                this.internalNamespace = m_name.Substring(0, p);
                m_name = m_name.Substring(p + 1);
            }

            p = m_name.IndexOf('<');
            if (p > 1)
                m_genericArguments = m_name.Substring(p);

			int i;
			for ( i=1; i<count; i++ )
			{
				if ( words[i].Equals( "interface" ) )
				{
					m_isInterface = true;
				}
				else if ( words[i].Equals( "private" ) )
				{
					m_isPrivate = true;
				}
				else if ( words[i].Equals( "public" ) )
				{
					m_isPublic = true;
				}
				else if ( words[i].Equals( "abstract" ) )
				{
					m_isAbstract = true;
				}
				else if ( words[i].Equals( "sealed" ) )
				{
					m_isSealed = true;
				}
			}

			// fullname

			ILNamespaceElement nsElement = getOwner() as ILNamespaceElement;

            //TODO: Check, how the full name should be determined for nested classes
            if (m_fullName == null) // In .NET 2.0 .class contains the full name
            {
                string nsName = (null == nsElement ? "" : nsElement.getNamespaceName());
                if (nsName == "")
                {
                    ILClassElement clsElement = getOwner() as ILClassElement;
                    if (null == clsElement)
                        m_fullName = m_name;
                    else
                        m_fullName = clsElement.getClassFullName() + "/" + m_name;
                }
                else
                {
                    m_fullName = nsName + "." + m_name;
                }
            }

			// basefullname

			string line = null;

			for ( i=0; i<getLineCount(); i++ )
			{
				line = getLine( i );

				if ( line.StartsWith( "extends" ) )
					break;
			}

			if ( i < getLineCount() )
			{
				m_baseFullName = line.Substring( 7 );	//extends has 7 chars
				m_baseFullName = stripComment( m_baseFullName );

				if ( m_baseFullName.Equals( "[mscorlib]System.ValueType" ) || m_baseFullName.Equals( "[netstandard]System.ValueType" ))
					m_isValueType = true;
				else if ( m_baseFullName.Equals( "[mscorlib]System.Enum" ) || m_baseFullName.Equals( "[netstandard]System.Enum" ))
					m_isEnum = true;
			}

		}

		protected override void
		unresolve()
		{
			m_name = null;
		}


#if NET11
		public void
		setForwardClassElement( ILClassElement forwardClassElement )
		{
			m_forwardClassElement = forwardClassElement;
		}

		public ILClassElement
		getForwardClassElement()
		{
			return m_forwardClassElement;
		}
#endif

		public ILMethodElement getMethod(string name)
		{
			ILMethodElement.Iterator iter = getMethodIterator();
			for ( ILMethodElement meth = iter.getFirst(); meth != null; meth = iter.getNext() )
			{
				if ( meth.getName() == name )
					return meth;
			}

			return null;
		}


		public ILPropertyElement getProperty( string name )
		{
			ILPropertyElement.Iterator iter = getPropertyIterator();
			for ( ILPropertyElement meth = iter.getFirst(); meth != null; meth = iter.getNext() )
			{
				if ( meth.getName() == name )
					return meth;
			}

			return null;
		}


		public ILNamespaceElement
		getNamespace()
		{
			ILNamespaceElement nsElem = getOwner() as ILNamespaceElement;

			if ( null != nsElem )
				return nsElem;
			
            ILClassElement clsElem = getOwner() as ILClassElement;
			
			if ( null == clsElem )
				return null;

			return clsElem.getNamespace();
		}


		public bool
		isPersistent(Type attrType)
		{
			ILCustomElement.Iterator cstmIter = getCustomIterator();

			// Durchlaufe alle Custom Elements der Klasse
			for ( ILCustomElement cstmElem = cstmIter.getNext(); null != cstmElem; cstmElem = cstmIter.getNext() )
			{
				if ( cstmElem.isAttribute( attrType ) )
					return true;
			}
			return false;
		}


		public bool
		isPrivate()
		{
			resolve();

			return m_isPrivate;
		}

		public bool
		isPublic()
		{
			resolve();

			return m_isPublic;
		}

		public bool
		isSealed()
		{
			resolve();

			return m_isSealed;
		}

		public bool
		isInterface()
		{
			resolve();

			return m_isInterface;
		}

		public bool
		isValueType()
		{
			resolve();

			return m_isValueType;
		}

		public bool
		isEnum()
		{
			resolve();

			return m_isEnum;
		}

		public bool
		isAbstract()
		{
			resolve();

			return m_isAbstract;
		}

		public string
		getName()
		{
			resolve();

			return m_name;
		}

        public string
        getClassFullName()
        {
            resolve();

            return m_fullName;
        }

        public string getGenericArguments()
        {
            return m_genericArguments;
        }


        public string getMappingName()
        {
            resolve();
            string pureName = m_fullName;
            if (pureName.StartsWith("class "))
                pureName = pureName.Substring(6);
            if (pureName.StartsWith("valuetype "))
                pureName = pureName.Substring(10);
            pureName = pureName.Replace("'", string.Empty);
            int p = pureName.IndexOf('<');
            if (p > -1)
                return pureName.Substring(0, p);

            return pureName;
        }

		public string
		getBaseFullName()
		{
			resolve();

			return m_baseFullName;
		}

		public string[]
		getInterfaceFullNames()
		{
			ArrayList arr = new ArrayList();

			bool start = false;
			bool last  = false;
			for ( int i=1; i<getLineCount() && ! last; i++ )
			{
				string line = getLine( i ).Trim();
				if ( ! start )
				{
					if ( line.StartsWith( "implements" ) )
					{
						start = true;
						line = line.Substring( 10 ).Trim();
					}
				}

				if ( start )
				{
					if ( line.EndsWith( "," ) )
						line = line.Substring( 0, line.Length - 1 ).Trim();
					else
						last = true;

					arr.Add( line );
				}
			}

			return arr.ToArray( typeof( string ) ) as string[];
		}

		public ILFieldElement
		insertFieldBefore( string firstLine, ILMethodElement method )
		{
			ILFieldElement field = new ILFieldElement( firstLine );
			method.insertBefore( field );

			return field;
		}

		public Dictionary<string,string>
		findAccessorProperties()
		{
			ILMethodElement.Iterator iter = getMethodIterator();
			Dictionary<string, string> result = new Dictionary<string, string>();
			for ( ILMethodElement meth = iter.getFirst(); meth != null; meth = iter.getNext() )
			{
				string methodName = meth.getName();
				if ( methodName.StartsWith ("get_") && !methodName.StartsWith ("get_NDO"))
				{					
					List<ILStatementElement> filteredStatements = new List<ILStatementElement>();
					ILStatementElement.Iterator statementIter = meth.getStatementIterator(true);
					for ( ILStatementElement statementElement = statementIter.getNext(); null != statementElement; statementElement = statementIter.getNext() )
					{
						string line = statementElement.getLine( 0 );
						if (line.IndexOf( " nop" ) > -1)
							continue;
						if (line.IndexOf( " ldarg.0" ) > -1)
							continue;
						if (line.IndexOf( " stloc.0" ) > -1)
							continue;
						if (line.IndexOf( " ldloc.0" ) > -1)
							continue;
						if (line.IndexOf( " br.s" ) > -1)
							continue;
						if (line.IndexOf( " ret" ) > -1)
							continue;
						if (line.IndexOf( "ldfld" ) == -1)  // The only remaining line should be the ldfld line
						{
							filteredStatements.Clear();
							break;
						}
						filteredStatements.Add(statementElement);
					}
					if (filteredStatements.Count == 1)
					{
						string line = filteredStatements[0].getLine(0);
						int p = line.LastIndexOf( "::" );
						if (p > -1)
						{
							p += 2;
							string key = line.Substring( p );
							if (key[0] == '\'' )
								key = key.Substring( 1, key.Length - 2 );
							if (!result.ContainsKey( key ))
								result.Add( key, methodName.Substring( 4 ) );
							else
								Console.WriteLine( key + " " + methodName.Substring( 4 ) );
						}
					}
				}
			}
			return result;
		}
	}
}
