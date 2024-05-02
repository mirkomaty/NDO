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


using NDOEnhancer.Ecma335;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILCustomElement.
	/// </summary>
	internal class ILCustomElement : ILElement
	{
		/*
      .custom instance void [NDO]NDO.Linq.ServerFunctionAttribute::.ctor(string) = ( 01 00 15 4D 49 4E 5F 41 43 54 49 56 45 5F 52 4F   // ...MIN_ACTIVE_RO
                                                                                     57 56 45 52 53 49 4F 4E 00 00 )                   // WVERSION..		 
		 */

		public ILCustomElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		internal class ILCustomElementType : ILElementType
		{
			public ILCustomElementType()
				: base( ".custom", typeof( ILCustomElement ) )
			{
			}
		}

		private static ILElementType        m_elementType = new ILCustomElementType();
		private AttributeInfo attributeInfo = null;


		private void Resolve()
		{
			if (this.attributeInfo == null)
				this.attributeInfo = GetAttributeInfo();
		}

		public bool
		IsAttribute( Type type )
		{
			Resolve();
			return this.attributeInfo.TypeName == type.FullName;
		}


		private object
		ReadParam( byte[] bytes, Type type, ref int pos )
		{
			if (type.FullName == "System.String" || type.FullName == "System.Type")
			{
				string para;
				int len = PackedLength.Read(bytes, ref pos);
				if (len == -1)
					para = null;
				else
					para = new System.Text.UTF8Encoding().GetString( bytes, pos + 1, len );

				pos += 1 + bytes[pos];

				if (para != null && para != string.Empty)
				{
					if (para[para.Length - 1] == '\0')
						para = para.Substring( 0, para.Length - 1 );
				}

				return para;
			}


			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			IntPtr addressInPinnedObject = (handle.AddrOfPinnedObject() + pos);
			object returnedObject = Marshal.PtrToStructure(addressInPinnedObject, type);
			handle.Free();
			pos += Marshal.SizeOf( type );
			return returnedObject;
			//else if (type == typeof( System.Boolean ))
			//{
			//	bool para = Convert.ToBoolean( bytes[pos] );
			//	pos += 1;
			//	return para;
			//}
			//else if (type == typeof( System.Byte ))
			//{
			//	byte para = bytes[pos];
			//	pos += 1;
			//	return para;
			//}
			//else if (type == typeof( System.Char ))
			//{
			//	char para = Convert.ToChar( bytes[pos+1] * 256 + bytes[pos] );
			//	pos += 2;
			//	return para;
			//}
			//else if (type == typeof( System.Int16 ))
			//{
			//	short para = Convert.ToInt16( bytes[pos+1] * 256 + bytes[pos] );
			//	pos += 2;
			//	return para;
			//}
			//else if (type == typeof( System.Int32 ))
			//{
			//	int para = ((bytes[pos+3] * 256 + bytes[pos+2]) * 256 + bytes[pos+1]) * 256 + bytes[pos];
			//	pos += 4;
			//	return para;
			//}
			//else if (typeof(System.Enum).IsAssignableFrom( type ) )
			//{
			//	// enums can be "derived" from any integral number type
			//	var underlyingType = Enum.GetUnderlyingType(type);
			//	var size = Marshal.SizeOf( underlyingType );

			//	// fortunately we can use a long value for all types of enums
			//	long integralValue = 0;
			//	for (int i = size-1; i >= 0; i--)
			//	{
			//		integralValue += bytes[pos+i];
			//		if (i > 0)
			//			integralValue *= 256;
			//	}

			//	pos += size;

			//	// convert the integral value into an enum of the given type
			//	return Enum.ToObject( type, integralValue );
			//}

			//MessageAdapter ma = new MessageAdapter();
			//ma.ShowError( $"Relation Attribute: Unknown type in attribute parameter list: {type.FullName}, owner type: {( this.Owner as ILClassElement )?.Name ?? "-"}" );

			//return null;
		}


		internal class AttributeInfo
		{
			public string TypeName;
			public string AssemblyName;
			public string[] ParamTypeNames = new string[]{};
			public object[] ParamValues = new object[]{};
		}


		public AttributeInfo
			GetAttributeInfo()
		{
			string text = "";

			for (int i = 0; i < LineCount; i++)
			{
				string line = GetLine( i );

				int cmt = line.IndexOf( "//" );
				if (-1 < cmt)
					line = line.Substring( 0, cmt ).Trim();

				text += " " + line;
			}

			text = text.Trim();

			var ecmaCustomAttrDecl = new EcmaCustomAttrDecl();
			ecmaCustomAttrDecl.Parse( text );

			char   comma =  ',';
			string[] paramTypeNames = new string[]{};
			object[] paramValues = new object[]{};

			// CustomAttributes parameters start with a Prolog – an unsigned int16 with the value 0x0001
			int pos = 2;
			var bytes = ecmaCustomAttrDecl.Bytes;
			var signature = ecmaCustomAttrDecl.ParameterList;

			BinaryReader br = new BinaryReader(new MemoryStream());
			if (signature != "")
			{
				paramTypeNames = signature.Split( comma );
				Type paramType;
				paramValues = new Object[paramTypeNames.Length];

				for (int i = 0; i < paramTypeNames.Length; i++)
				{
					string paramTypeName = paramTypeNames[i].Trim();
					paramTypeNames[i] = ILType.GetNetTypeName( paramTypeName );
					paramType = Type.GetType( paramTypeNames[i] );
					if (paramType == null)
					{
						EcmaType.BuiltInTypesDict.TryGetValue( paramTypeName, out paramType );
						if (paramType == null)
							throw new Exception( $"{ecmaCustomAttrDecl.TypeName}: Unknown type in attribute parameter list: {paramTypeName}, type: {( this.Owner as ILClassElement )?.Name ?? ""}" );
					}

					paramValues[i] = ReadParam( bytes, paramType, ref pos );
				}
			}

			AttributeInfo   attr = new AttributeInfo();
			attr.TypeName = ecmaCustomAttrDecl.TypeName;
			attr.AssemblyName = ecmaCustomAttrDecl.ResolutionScope;
			attr.ParamTypeNames = paramTypeNames;
			attr.ParamValues = paramValues;

			this.attributeInfo = attr;
			return attr;
		}

		public void
			replaceLines( string firstLine )
		{
			ClearLines();

			AddLine( firstLine );
		}


	}
}
