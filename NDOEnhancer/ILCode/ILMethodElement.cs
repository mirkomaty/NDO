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
using NDOEnhancer.Ecma335;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILMethodElement.
	/// </summary>
	internal class ILMethodElement : ILElement
	{
		public ILMethodElement()
		{
		}

		public ILMethodElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILMethodElementType : ILElementType
		{
			public ILMethodElementType()
				: base( ".method", typeof( ILMethodElement ) )
			{
			}
		}

		private static ILElementType		m_elementType = new ILMethodElementType();
		
		private bool						m_isPrivate	  = false;
		private bool						m_isProtected = false;
		private bool						m_isPublic	  = false;
		private bool						m_isStatic	  = false;
		private bool						m_isNative	  = false;
		private bool						m_isAbstract  = false;
		private bool						m_isVirtual	  = false;
		private bool						m_isNew		  = false;
        private bool                        m_isPinvoke   = false;
		private bool						m_specialName = false;
		private string						m_ilType;
		private string						m_name;
		private string						m_signature;

		private List<string>					m_parameterTypes = new List<string>();
		private List<string>					m_parameterNames = new List<string>();

		private List<ILStatementElement> m_statements;

		public void
		addStatement( string firstLine )
		{
			AddElement( new ILStatementElement( firstLine ) );
		}

		public bool
		isConstructor()
		{
			Resolve();

			return m_name.Equals( ".ctor" );
		}

		public bool
		isStaticConstructor()
		{
			Resolve();

			return m_name.Equals( ".cctor" );
		}

		private void
		Resolve()
		{
			if ( null != m_name )
				return;

			string allLines = this.GetAllLines();

            EcmaMethodHeader methodHeader = new EcmaMethodHeader();
            if (!methodHeader.Parse(allLines))
                throw new Exception("Invalid Method Header: " + allLines);

            //TODO: This doesn't work well with generic types
            string[] words = SplitWords(methodHeader.ParameterList);
            int count;
            for (count = 0; count < words.Length; count++)
                if (words[count].Equals("("))
                    break;

            string paramType = "";
            string paramName = "";
            for (int i = count + 1; i < words.Length; i++)
            {
                if (words[i] == "," || words[i] == ")")
                {
                    if (0 < paramType.Length && 0 < paramName.Length)
                    {
                        m_parameterTypes.Add(paramType);
                        m_parameterNames.Add(paramName);
                    }

                    if (words[i] == ")")
                        break;

                    paramType = "";
                    paramName = "";
                }
                else
                {
                    paramType = (paramType + " " + paramName).Trim();
                    paramName = words[i];
                }
            }

			m_name = methodHeader.MethodName;

            m_ilType = methodHeader.IlType;

			foreach ( string methodAttr in methodHeader.MethodAttrs)
			{
                
				if ( methodAttr.Equals( "private" ) )
				{
					m_isPrivate = true;
				}
				else if ( methodAttr.Equals( "family" ) )
				{
					m_isProtected = true;
				}
				else if ( methodAttr.Equals( "public" ) )
				{
					m_isPublic = true;
				}
				else if ( methodAttr.Equals( "static" ) )
				{
					m_isStatic = true;
				}
				else if ( methodAttr.Equals( "abstract" ) )
				{
					m_isAbstract = true;
				}
				else if ( methodAttr.Equals( "virtual" ) )
				{
					m_isVirtual = true;
				}
				else if ( methodAttr.Equals( "newslot" ) )
				{
					m_isNew = true;
				}
				else if ( methodAttr.Equals( "specialname" ) )
				{
					m_specialName = true;
				}
                else if (methodAttr.Equals("instance"))
                {
                    m_isStatic = false;
                }
                else if (methodAttr.Equals("extern"))
                {
                    m_isNative = true;
                }
                else if (methodAttr.Equals("unmanaged"))
                {
                    m_isNative = true;
                }
                else if (methodAttr.StartsWith("pinvokeimpl"))
                {
                    m_isPinvoke = true;
                }
            }


			m_signature = methodHeader.Signature;
		}

		protected override void
		Unresolve()
		{
			m_name = null;
		}

		public override string
		MakeUpperCaseName( string name )
		{
			if ( isSpecialName() )
				return name;

			return base.MakeUpperCaseName( name );
		}

		public void
		MakeUpperCaseName()
		{
			string oldName = Name;
			string newName = MakeUpperCaseName( oldName );

			if ( oldName == newName )
				return;

			ReplaceTextOnce( oldName + "(", newName + "(" );

			m_name		= newName;
			m_signature	= newName + m_signature.Substring( m_signature.IndexOf( "(" ) );
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
		isNative()
		{
			Resolve();

			return m_isNative;
		}


        public bool
        isPinvoke()
        { 
            Resolve();
            return m_isPinvoke;
        }

		public bool
		isAbstract()
		{
			Resolve();

			return m_isAbstract;
		}

		public bool
		isVirtual()
		{
			Resolve();

			return m_isVirtual;
		}

		public bool
		isNew()
		{
			Resolve();

			return m_isNew;
		}

		public bool
		isSpecialName()
		{
			Resolve();

			return m_specialName;
		}

		public string
		Name
		{
			get
			{
				Resolve();
				return m_name;
			}
		}

		public string
		getILType()
		{
			Resolve();

			return m_ilType;
		}

		public int
		getParameterCount()
		{
			Resolve();

			return m_parameterTypes.Count;
		}

		public string
		Signature
		{
			get
			{
				Resolve();
				return m_signature;
			}
		}

		protected override bool InsertAfter( ILElement insertElement, ILElement existingElement )
		{
			if (base.InsertAfter( insertElement, existingElement ))
			{
				if (insertElement is ILStatementElement insStatement)
					m_statements.Add( insStatement );

				return true;
			}

			return false;
		}

		public override bool InsertBefore( ILElement insertElement, ILElement existingElement )
		{
			if (base.InsertBefore( insertElement, existingElement ))
			{
				if (insertElement is ILStatementElement insStatement)
				{
					var index = m_statements.IndexOf( insStatement );
					m_statements.Insert( index, insStatement );
				}

				return true;
			}

			return false;
		}

		public IEnumerable<ILStatementElement> Statements
		{
			get
			{
				if (m_statements != null)
					return m_statements;

				m_statements = (from e in Elements 
								let se = e as ILStatementElement 
								where se != null 
								select se).ToList();

				return m_statements;
			}
		}
	}
}
