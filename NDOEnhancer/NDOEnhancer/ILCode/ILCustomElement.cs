//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
using NDOEnhancer.Ecma335.Bytes;
using System;
using System.Collections.Generic;
using System.Linq;
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

		internal class AttributeInfo
		{
			public string TypeName { get; set; }
			public string AssemblyName { get; set; }
            public string[] ParamTypeNames { get; set; } = new string[]{};
			public object[] ParamValues { get; set; } = new object[]{};
			public Dictionary<string, object> NamedParams { get; set; } = new Dictionary<string, object>();
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
			IEnumerable<object> paramValues = new object[]{};
			Dictionary<string, object> namedParams = new Dictionary<string, object>();

			var bytes = ecmaCustomAttrDecl.Bytes;
			var signature = ecmaCustomAttrDecl.ParameterList;

			if (signature != "")
			{
				paramTypeNames = signature.Split( comma );
				Type[] paramTypes = new Type[paramTypeNames.Length];

				for (int i = 0; i < paramTypeNames.Length; i++)
				{
					string paramTypeName = paramTypeNames[i].Trim();
					paramTypeNames[i] = ILType.GetNetTypeName( paramTypeName );
					var paramType = Type.GetType( paramTypeNames[i] );
					if (paramType == null)
					{
						EcmaType.TryGetBuiltInType( paramTypeName, out paramType );
						if (paramType == null)
							throw new Exception( $"{ecmaCustomAttrDecl.TypeName}: Unknown type in attribute parameter list: {paramTypeName}, type: {( this.Owner as ILClassElement )?.Name ?? ""}" );
					}

					paramTypes[i] = paramType;
				}

				var ecmaBytes = new EcmaBytes();
				ecmaBytes.Parse( bytes, paramTypes, out paramValues, out namedParams );				

			}

			AttributeInfo   attr = new AttributeInfo();
			attr.TypeName = ecmaCustomAttrDecl.TypeName;
			attr.AssemblyName = ecmaCustomAttrDecl.ResolutionScope;
			attr.ParamTypeNames = paramTypeNames;
			attr.ParamValues = paramValues.ToArray();
			attr.NamedParams = namedParams;

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
