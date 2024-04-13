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
using System.Collections.Generic;
using System.Linq;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILClassElement.
	/// </summary>
	internal class ILClassElement : ILElement
	{
		public ILClassElement()
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

		private string						m_name;
        private string                      m_genericArguments = string.Empty;
		private string						m_fullName = null;
		private string						m_baseFullName;
		private bool						m_isPublic			  = false;
		private bool						m_isPrivate			  = false;
		private bool						m_isSealed			  = false;
		private bool						m_isInterface		  = false;
		private bool						m_isValueType		  = false;
		private bool						m_isEnum			  = false;
		private bool						m_isAbstract		  = false;
        private string                      internalNamespace     = string.Empty;

		private static ILElementType		m_elementType		  = new ILClassElementType();
		private List<ILMethodElement>       m_methods;
		private List<ILFieldElement>		m_fields;

		public void AddImplements(string[] interfaceNames)
		{
			List<string> lines = new List<string>();
			int implPosition = -1;
			for ( int i = 0; i < this.LineCount; i++ )
			{
				string s = this.GetLine( i );
				if ( s.StartsWith( "implements" ) )
					implPosition = i;
				lines.Add( s );
			}

			if (implPosition == -1)
			{
				implPosition = lines.Count;
				AddLine("implements ");
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

			this.ClearLines();
			for ( int i = 0; i < implPosition; i++ )
				this.AddLine( lines[i] );
			remainingImplements.InsertRange( 0, existingImplements );
			for ( int i = 0; i < remainingImplements.Count; i++ )
			{
				string s = string.Empty;
				if ( i == 0 )
					s = "implements ";
				s += remainingImplements[i];
				if ( i < remainingImplements.Count - 1 )
					s += ", ";
				this.AddLine( s );
			}
		}

		private void Resolve()
		{
			if ( null != m_name )
				return;

			string[] words = SplitWords( GetLine( 0 ) );
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

			ILNamespaceElement nsElement = Owner as ILNamespaceElement;

            if (m_fullName == null) // In .NET > 2.0 .class contains the full name
            {
                string nsName = (null == nsElement ? "" : nsElement.getNamespaceName());
                if (nsName == "")
                {
                    ILClassElement clsElement = Owner as ILClassElement;
                    if (null == clsElement)
                        m_fullName = m_name;
                    else
                        m_fullName = clsElement.ClassFullName + "/" + m_name;
                }
                else
                {
                    m_fullName = nsName + "." + m_name;
                }
            }

			// basefullname

			string line = null;

			for ( i=0; i<LineCount; i++ )
			{
				line = GetLine( i );

				if ( line.StartsWith( "extends" ) )
					break;
			}

			if ( i < LineCount )
			{
				m_baseFullName = line.Substring( 7 );	//extends has 7 chars
				m_baseFullName = StripComment( m_baseFullName );

				if ( m_baseFullName.Equals( "[mscorlib]System.ValueType" ) || m_baseFullName.Equals( "[netstandard]System.ValueType" ))
					m_isValueType = true;
				else if ( m_baseFullName.Equals( "[mscorlib]System.Enum" ) || m_baseFullName.Equals( "[netstandard]System.Enum" ))
					m_isEnum = true;
			}

		}

		protected override void Unresolve()
		{
			m_name = null;
		}

		private void ComputeMethods()
		{
			m_methods = (from e in Elements let me = e as ILMethodElement where me != null select me).ToList();
		}

		public IEnumerable<ILMethodElement> Methods
		{
			get
			{
				if (m_methods == null)
				{
					ComputeMethods();
				}
				return m_methods;
			}
		}

		public void AppendMethod( ILMethodElement insertElement )
		{
			base.AppendElement( insertElement );
				m_methods.Add( insertElement );
		}

		public void AppendProperty( ILPropertyElement insertElement )
		{
			base.AppendElement( insertElement );
		}

		private void ComputeFields()
		{
			m_fields = ( from e in Elements let me = e as ILFieldElement where me != null select me ).ToList();
		}

		public IEnumerable<ILFieldElement> Fields
		{
			get
			{
				if (m_fields == null)
				{
					ComputeFields();
				}
				return m_fields;
			}
		}

		public ILMethodElement GetMethod(string name)
		{
			return (from me in Methods
					where me.Name == name 
					select me).FirstOrDefault();
		}


		public ILPropertyElement GetProperty( string name )
		{
			return ( from me in
				( from e in Elements where e is ILPropertyElement select (ILPropertyElement) e )
					 where me.Name == name
					 select me ).FirstOrDefault();
		}

		public bool
		IsPersistent(Type attrType)
		{
			return 
				( from me in
					from e in Elements where e is ILCustomElement select (ILCustomElement) e
					where me.IsAttribute( attrType )
				select me ).Any();
		}


		public bool IsPrivate => m_isPrivate;		
		public bool IsPublic=> m_isPublic;		
		public bool IsSealed=> m_isSealed;
		public bool IsInterface=> m_isInterface;
		public bool IsValueType=>m_isValueType;
		public bool IsEnum=>m_isEnum;
		public bool IsAbstract=>m_isAbstract;

		public override bool NeedsBlock => true;

		public string Name
		{
			get
			{
				Resolve();
				return m_name;
			}
		}

        public string ClassFullName
        {
			get
			{
				Resolve();
				return m_fullName;
			}
        }

        public string GenericArguments => m_genericArguments;       

        public string MappingName
        {
			get
			{
				Resolve();
				string pureName = m_fullName;
				if (pureName.StartsWith( "class " ))
					pureName = pureName.Substring( 6 );
				if (pureName.StartsWith( "valuetype " ))
					pureName = pureName.Substring( 10 );
				pureName = pureName.Replace( "'", string.Empty );
				int p = pureName.IndexOf('<');
				if (p > -1)
					return pureName.Substring( 0, p );

				return pureName;
			}
        }

		public string BaseFullName
		{
			get
			{
				Resolve();
				return m_baseFullName;
			}
		}

		public string[] GetInterfaceFullNames()
		{
			var arr = new List<string>();

			bool start = false;
			bool last  = false;
			for ( int i = 1; i < LineCount && ! last; i++ )
			{
				string line = GetLine( i ).Trim();
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

			return arr.ToArray();
		}

		public ILFieldElement
		AppendField( string firstLine )
		{
			var firstMethod = Methods.First();
			ILFieldElement field = new ILFieldElement( firstLine );
			firstMethod.InsertBefore( field );
			m_fields.Add( field );

			return field;
		}

		public Dictionary<string,string>
		GetAccessorProperties()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach ( ILMethodElement methodElement in Methods )
			{
				string methodName = methodElement.Name;
				if ( methodName.StartsWith ("get_") && !methodName.StartsWith ("get_NDO"))
				{					
					List<ILStatementElement> filteredStatements = new List<ILStatementElement>();

					foreach ( ILStatementElement statementElement in methodElement.Statements )
					{
						string line = statementElement.GetLine( 0 );
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
						string line = filteredStatements[0].GetLine(0);
						int p = line.LastIndexOf( "::" );
						if (p > -1)
						{
							p += 2;
							string key = line.Substring( p );
							if (key[0] == '\'' )
								key = key.Substring( 1, key.Length - 2 );
							if (!result.ContainsKey( key ))
								result.Add( key, methodName.Substring( 4 ) );
							//else
							//	Console.WriteLine( key + " " + methodName.Substring( 4 ) );
						}
					}
				}
			}

			return result;
		}
	}
}
