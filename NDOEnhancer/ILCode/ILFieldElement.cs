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

namespace ILCode
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

		public class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof( ILFieldElement ) )
			{
			}

			public new ILFieldElement
			getNext()
			{
				return base.getNext() as ILFieldElement;
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

		private string						m_poetSlotType;
		private string						m_poetSlotName;
		private string						m_javaType;
		private string						m_javaClass;
		private string						m_javaSignature;
		
		public static void
		initialize()
		{
		}

		public static ILFieldElement.Iterator
		getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		private void
		resolve()
		{
			if ( null != m_name )
				return;

			string[] words = splitWords( getLine( 0 ) );
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

//			// find .custom elements after this .field element in my container
//			ILElementIterator iter = getOwner().getAllIterator();
//			int state = 0;
//			for ( ILElement elem = iter.getFirst(); null != elem && 2 != state; elem = iter.getNext() )
//			{
//				switch ( state )
//				{
//				case 0:
//					if ( elem == this )
//						state = 1;
//
//					break;
//				case 1:
//					ILCustomElement cusElem = elem as ILCustomElement;
//					if ( null == cusElem )
//					{
//						state = 2;
//					}
//					else if ( cusElem.isAttribute( TransientAttribute.getType() ) )
//					{
//						m_isTransient = true;
//						state = 2;
//					}
//					break;
//				}
//			}

			if ( m_ilType == "bool" )
			{
				m_javaType	   	= "boolean";
				m_javaSignature	= "Z";
				m_javaClass	   	= "java.lang.Boolean";
				m_poetSlotType	= "bool";
				m_poetSlotName 	= "Boolean";
			}
			else if ( m_ilType == "bool[]" )
			{
				m_javaType		= "boolean[]";
				m_javaSignature	= "[Z";
				m_poetSlotType 	= "object";
				m_poetSlotName	= "Object";
			}
			else if ( m_ilType == "int8" || m_ilType == "unsigned int8" )
			{
				m_javaType	   	= "byte";
				m_javaSignature	= "B";
				m_javaClass	   	= "java.lang.Byte";
				m_poetSlotType	= "int8";
				m_poetSlotName 	= "Byte";
			}
			else if ( m_ilType == "int8[]" || m_ilType == "unsigned int8[]" )
			{
				m_javaType	   	= "byte[]";
				m_javaSignature	= "[B";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "char" || m_ilType == "unsigned char" )
			{
				m_javaType	   	= "char";
				m_javaSignature	= "C";
				m_javaClass	   	= "java.lang.Character";
				m_poetSlotType	= "char";
				m_poetSlotName 	= "Char";
			}
			else if ( m_ilType == "char[]" || m_ilType == "unsigned char[]" )
			{
				m_javaType	   	= "char[]";
				m_javaSignature	= "[C";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "int16" || m_ilType == "unsigned int16" )
			{
				m_javaType	   	= "short";
				m_javaSignature	= "S";
				m_javaClass	   	= "java.lang.Short";
				m_poetSlotType	= "int16";
				m_poetSlotName 	= "Short";
			}
			else if ( m_ilType == "int16[]" || m_ilType == "unsigned int16[]" )
			{
				m_javaType	   	= "short[]";
				m_javaSignature	= "[S";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "int32" || m_ilType == "unsigned int32" )
			{
				m_javaType	   	= "int";
				m_javaSignature	= "I";
				m_javaClass	   	= "java.lang.Integer";
				m_poetSlotType	= "int32";
				m_poetSlotName 	= "Int";
			}
			else if ( m_ilType == "int32[]" || m_ilType == "unsigned int32[]" )
			{
				m_javaType	   	= "int[]";
				m_javaSignature	= "[I";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "int64" || m_ilType == "unsigned int64" )
			{
				m_javaType	   	= "long";
				m_javaSignature	= "J";
				m_javaClass	   	= "java.lang.Long";
				m_poetSlotType	= "int64";
				m_poetSlotName 	= "Long";
			}
			else if ( m_ilType == "int64[]" || m_ilType == "unsigned int64[]" )
			{
				m_javaType	   	= "long[]";
				m_javaSignature	= "[J";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "float32" )
			{
				m_javaType	   	= "float";
				m_javaSignature	= "F";
				m_javaClass		= "java.lang.Float";
				m_poetSlotType	= "float32";
				m_poetSlotName 	= "Float";
			}
			else if ( m_ilType == "float32[]" )
			{
				m_javaType	   	= "float[]";
				m_javaSignature	= "[F";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "float64" )
			{
				m_javaType	   	= "double";
				m_javaSignature	= "D";
				m_javaClass	   	= "java.lang.Double";
				m_poetSlotType	= "float64";
				m_poetSlotName 	= "Double";
			}
			else if ( m_ilType == "float64[]" )
			{
				m_javaType	   	= "double[]";
				m_javaSignature	= "[D";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "string" )
			{
				m_javaType	   	= "java.lang.String";
				m_javaSignature	= "java.lang.String";
				m_poetSlotType	= "string";
				m_poetSlotName 	= "String";
			}
			else if ( m_ilType == "string[]" )
			{
				m_javaType	   	= "java.lang.String[]";
				m_javaSignature	= "[Ljava.lang.String;";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "valuetype [mscorlib]System.DateTime" )
			{
				m_javaType	   	= "java.util.Date";
				m_javaSignature	= "System.DateTime";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else if ( m_ilType == "valuetype [mscorlib]System.DateTime[]" )
			{
				m_javaType	   	= "java.util.Date[]";
				m_javaSignature	= "[LSystem.DateTime;";
				m_poetSlotType 	= "object";
				m_poetSlotName 	= "Object";
			}
			else
			{
				m_javaType = m_ilType;

				if ( m_javaType.StartsWith( "class " ) )
					m_javaType = m_javaType.Substring( 6 ).Trim();
				else if ( m_javaType.StartsWith( "valuetype " ) )
					m_javaType = m_javaType.Substring( 10 ).Trim();

				if ( 0 == m_javaType.IndexOf( "[" ) )
					m_javaType = m_javaType.Substring( m_javaType.IndexOf( "]" ) + 1 ).Trim();

				if ( m_javaType.EndsWith( "[]" ) )
					m_javaSignature = "[L" + m_javaType.Substring( 0, m_javaType.Length - 2 ) + ";";
				else
					m_javaSignature = m_javaType;

				m_poetSlotType = "object";
				m_poetSlotName = "Object";
			}
		}

		protected override void
		unresolve()
		{
			m_name = null;
		}


		public bool
		isPersistent( Type transientAttribute )
		{
			resolve();

			// Hole das nächste Element im IL-Quelltext
			ILCustomElement cusElem = this.getSuccessor() as ILCustomElement;
			while (null != cusElem)
			{
				if (cusElem.isAttribute( transientAttribute ) )
				{
					return false;
				}
				cusElem = cusElem.getSuccessor() as ILCustomElement;
			}
			return true;
		}


		public ILCustomElement.AttributeInfo
		getAttributeInfo ( ILElementIterator allIter )
		{
			ILCustomElement cusElem = allIter.getNext() as ILCustomElement;
			if (null == cusElem ) return null;
			//TODO: Eigentlich müsste der Fall berücksichtigt werden,
			// wenn ein Feld mehrere Attribute hat.
			return cusElem.getAttributeInfo();
		}


		public bool
		isPrivate()
		{
			resolve();

			return m_isPrivate;
		}

		public bool
		isProtected()
		{
			resolve();

			return m_isProtected;
		}

		public bool
		isPublic()
		{
			resolve();

			return m_isPublic;
		}

		public bool
		isStatic()
		{
			resolve();

			return m_isStatic;
		}

		public bool
		isConst()
		{
			resolve();

			return m_isConst;
		}

		public bool
		isVolatile()
		{
			resolve();

			return m_isVolatile;
		}

		public string
		getName()
		{
			resolve();

			return m_name;
		}

		public bool
		isPrimitive()
		{
			resolve();

			return (null != m_javaClass && 0 < m_javaClass.Length);
		}

		public string
		getILType()
		{
			resolve();

			return m_ilType;
		}

		public string
		getILTypeName()
		{
			resolve();

			if ( m_ilType.StartsWith( "class" ) )
				return m_ilType.Substring( 5 ).Trim();
			else if ( m_ilType.StartsWith( "valuetype" ) )
				return m_ilType.Substring( 9 ).Trim();

			return m_ilType;
		}

		public string
		getPureTypeName()
		{
			resolve();

			int pos = m_ilType.IndexOf( ']' );

			if ( -1 < pos )
				return m_ilType.Substring( pos + 1 );

			return getILTypeName();
		}

		public string
		getPoetSlotType()
		{
			resolve();

			return m_poetSlotType;
		}

		public string
		getPoetSlotName()
		{
			resolve();

			return m_poetSlotName;
		}

		public string
		getJavaType()
		{
			resolve();

			return m_javaType;
		}

		public string
		getPoetType()
		{
			resolve();

			return m_poetSlotName;
		}

		public string
		getJavaClass()
		{
			resolve();

			return m_javaClass;
		}

		public string
		getJavaSignature()
		{
			resolve();

			return m_javaSignature;
		}
	}
}
