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
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

// using com.poet.util;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILCustomElement.
	/// </summary>
	internal class ILCustomElement : ILElement
	{
		public ILCustomElement()
			: base( false )
		{
		}

		public ILCustomElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}
/*
		public ILCustomElement(string typeName, object[] parameters)
			: base ( false )
		{
			string param = makeParameters(parameters);
			//this.addLine
		}
*/
		internal class ILCustomElementType : ILElementType
		{
			public ILCustomElementType()
				: base( ".custom", typeof (ILCustomElement) )
			{
			}
		}

		internal class Iterator : ILElementIterator
		{
			public Iterator( ILElement element )
				: base( element, typeof (ILCustomElement) )
			{
			}

			public new ILCustomElement
				getNext()
			{
				return base.getNext() as ILCustomElement;
			}
		}

		private static ILElementType		m_elementType = new ILCustomElementType();
		
		public static void
			initialize()
		{
		}

		public static ILCustomElement.Iterator
			getIterator( ILElement element )
		{
			return new Iterator( element );
		}

		public bool
			isAttribute( Type type )
		{
			if ( null == type )
				return false;

			string firstLine = getLine( 0 );

			return (-1 < firstLine.IndexOf( type.FullName ));
		}

/*
		private string
			makeParameters(object[] parameters)
		{
			return null;
		}
*/
		private object
			readParam( byte[] bytes, Type type, ref int pos )
		{
			if ( type.FullName == "System.String" || type.FullName == "System.Type")
			{
				string para;
				int len = PackedLength.Read(bytes, ref pos);
				if (len == -1)
					para = null;
				else
					para = new System.Text.UTF8Encoding().GetString(bytes, pos + 1, len);
				//string para = new string( chars, pos + 1, bytes[pos]);
				pos += 1 + bytes[pos];
				if (para != null && para != string.Empty)
				{
					if (para[para.Length - 1] == '\0')
						para = para.Substring(0, para.Length - 1);
				}
				return para;
			}
			else if ( type == typeof(System.Boolean) )
			{
				bool para = Convert.ToBoolean( bytes[pos] );
				pos += 1;
				return para;
			}
			else if ( type == typeof(System.Char) )
			{
				char para = Convert.ToChar( bytes[pos+1] * 256 + bytes[pos] );
				pos += 2;
				return para;
			}
			else if ( type == typeof(System.Int16) )
			{
				short para = Convert.ToInt16( bytes[pos+1] * 256 + bytes[pos] );
				pos += 2;
				return para;
			}
			else if ( type == typeof(System.Int32) )
			{
				int para = ((bytes[pos+3] * 256 + bytes[pos+2]) * 256 + bytes[pos+1]) * 256 + bytes[pos];
				pos += 4;
				return para;
			}
			else if ( type.FullName == "NDO.RelationInfo" )
			{
				short para = Convert.ToInt16( bytes[pos+1] * 256 + bytes[pos] );
				pos += 2;
				return (NDO.RelationInfo) para;
			}
			
			NDOEnhancer.MessageAdapter ma = new NDOEnhancer.MessageAdapter();
			ma.ShowError("Unknown type in attribute parameter list: " + type.FullName );
			
			return null;
		}


		internal class AttributeInfo
		{
			public string Name;
			public string AssemblyName;
			public string[] ParamTypeNames = new string[]{};
			public object[] ParamValues = new object[]{};
		}


		public AttributeInfo
			getAttributeInfo()
		{
			string text = "";

			for ( int i=0; i<getLineCount(); i++ )
			{
				string line = getLine( i );
				
				int cmt = line.IndexOf( "//" );
				if ( -1 < cmt )
					line = line.Substring( 0, cmt ).Trim();
				
				text += " " + line;
			}

			int start, end;

			// assembly name

			start = text.IndexOf( "[" ) + 1;
			end   = text.IndexOf( "]", start );
			string assName = stripComment( text.Substring( start, end - start ) );

			// type name

			start = text.IndexOf( "]" ) + 1;
			end	  = text.IndexOf( "::", start );
			string typeName = stripComment( text.Substring( start, end - start ) );

			// constructor signature

			start = text.IndexOf( "(" ) + 1;
			end	  = text.IndexOf( ")", start );
			string signature = stripComment( text.Substring( start, end - start ) );

			// parameter bytes

			start = text.IndexOf( "= (" ) + 3;
			end	  = text.IndexOf( ")", start );
			string byteText	= text.Substring( start, end - start ).Trim();

			char[]   spc		 = { ' ' };
			string[] byteStrings = byteText.Split( spc );
			byte[]   bytes		 = new byte[byteStrings.Length];
			//			char[]	 chars		 = new char[byteStrings.Length];

			for ( int i=0; i<byteStrings.Length; i++ )
			{
				bytes[i] = Byte.Parse( byteStrings[i], NumberStyles.HexNumber );
				//				chars[i] = Convert.ToChar( bytes[i] );
			}

//			char[] chars = new System.Text.UTF8Encoding().GetChars(bytes);
			
			//			Type attributeType = Type.GetType( typeName + ", " + assName );
			//			if ( null == attributeType )
			//				return null;

			char   comma =  ',';
			string[] paramTypeNames = new string[]{};
			object[] paramValues = new object[]{};
			Type[] paramTypes = new Type[]{};
			//CustomAttrib starts with a Prolog – an unsigned int16, with value 0x0001
			int pos = 2;
			if (signature != "")
			{
				paramTypeNames = signature.Split( comma );
				paramTypes	   = new Type[paramTypeNames.Length];
				paramValues	   = new Object[paramTypeNames.Length];

				for ( int i=0; i<paramTypeNames.Length; i++ )
				{
					string paramTypeName = paramTypeNames[i].Trim();
					if ( paramTypeName == "string" || paramTypeName.IndexOf("System.String") > -1)
					{
						paramTypeNames[i] = "System.String";
						paramTypes[i] = typeof(string);
					}
					else if ( paramTypeName == "bool" )
					{
						paramTypes[i] = typeof(bool);
						paramTypeNames[i] = "System.Boolean";
					}
					else if ( paramTypeName == "char" )
					{
						paramTypes[i] = typeof(char);
						paramTypeNames[i] = "System.Char";
					}
					else if ( paramTypeName == "short" || paramTypeName == "int16" )
					{
						paramTypeNames[i] = "System.Int16";
						paramTypes[i] = typeof(System.Int16);
					}
					else if ( paramTypeName == "int" || paramTypeName == "int32")
					{
						paramTypeNames[i] = "System.Int32";
						paramTypes[i] = typeof(System.Int32);
					}
						// Achtung: System.Type wird als string abgespeichert!!!
					else if ( paramTypeName.IndexOf("[mscorlib") > -1 && paramTypeName.IndexOf("System.Type") > -1 )
					{
						paramTypes[i] = typeof(System.String);
						paramTypeNames[i] = "System.Type";						
					}
					else if ( paramTypeName == "valuetype [NDO]NDO.RelationInfo" )
					{
						paramTypes[i] = typeof(NDO.RelationInfo);
						paramTypeNames[i] = "NDO.RelationInfo, NDO";						
					}
					else
						throw new Exception("Relation Attribute: Unknown type in attribute parameter list: " + paramTypeName);

					//paramTypes[i]  = Type.GetType( paramTypeNames[i] );
					paramValues[i] = readParam( bytes, paramTypes[i], ref pos );
				}
			}
			//			ConstructorInfo ci	 = attributeType.GetConstructor( paramTypes );
			AttributeInfo	attr = new AttributeInfo();
			attr.Name = typeName;
			attr.AssemblyName = assName;
			attr.ParamTypeNames = paramTypeNames;
			attr.ParamValues = paramValues;

			//TODO: sollten jemals benannte Parameter dazukommen
			//TODO: müssten wir hier einigen Code hinzufügen

			//			short count = (short) readParam( bytes, chars, Type.GetType( "System.Int16" ), ref pos );
			//
			//			for ( short i=0; i<count; i++ )
			//			{
			//				pos += 2;
			//
			//				string propName = readParam( bytes, chars, Type.GetType( "System.String" ), ref pos ) as string;
			//
			//				PropertyInfo pi = attributeType.GetProperty( propName );
			//
			//				object propVal = readParam( bytes, chars, pi.PropertyType, ref pos );
			//
			//				pi.SetValue( attr, propVal, null );
			//			}

			return attr;
		}

		public void
			replaceLines( string firstLine )
		{
			clearLines();

			addLine( firstLine );
		}

	
	}
}
