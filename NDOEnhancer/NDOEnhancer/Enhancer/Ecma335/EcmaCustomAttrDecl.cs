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


using System.Globalization;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace NDOEnhancer.Ecma335
{
    class EcmaCustomAttrDecl : IEcmaDefinition
    {
        int nextTokenPosition;
        string content;
		byte[] bytes = { };
		string resolutionScope;
		string typeName;

        public int NextTokenPosition => nextTokenPosition;
        public string Content => content;

		public string ResolutionScope => resolutionScope;
		public string TypeName => typeName;

		public byte[] Bytes => bytes;

		string parameterList;
		public string ParameterList => parameterList;		

		public bool Parse(string input)
        {
			string methodKeyword = ".custom";
			if (!input.StartsWith( methodKeyword ))
				throw new EcmaILParserException( methodKeyword, "begin of custom declaration", input );

			content = methodKeyword;

			int p = 7;
			char c;
			while (char.IsWhiteSpace( c = input[p] ))
			{
				content += c;
				p++;
			}


			this.parameterList = methodHeader.ParameterList;

			p += methodHeader.NextTokenPosition;

			while ( p < input.Length && char.IsWhiteSpace( c = input[p] ))
			{
				content += c;
				p++;
			}

			if (p < input.Length && ( c = input[p] ) == '=')
			{
				content += c;
				p++;


				while (p < input.Length && char.IsWhiteSpace( c = input[p] ))
				{
					content += c;
					p++;
				}

				var remainder = input.Substring(p);

				var start = remainder.IndexOf( "(" );
				var end = remainder.IndexOf( ")", start );

				if (start < 0)
					throw new EcmaILParserException( "(", "=", input );
				if (end < 0)
					throw new EcmaILParserException( ")", "(", input );

				string byteText = remainder.Substring( start, end - start ).Trim();

				string[] byteStrings = byteText.Split( ' ' );
				this.bytes = new byte[byteStrings.Length];

				for (int i = 0; i < byteStrings.Length; i++)
				{
					this.bytes[i] = Byte.Parse( byteStrings[i], NumberStyles.HexNumber );
				}

				content += remainder;
				p += remainder.Length;
			}

			nextTokenPosition = p;
			return true;
		}
	}
}

/*
Short definition in Partition II:
CustomDecl := ctor [ '=' '(' bytes ')']

Complete definition

customAttrDecl : 
1   '.custom' customType
2 | '.custom' customType '=' compQstring
3 | customHead bytes ')'
4 | '.custom' '(' ownerType ')' customType
5 | '.custom' '(' ownerType ')' customType '=' compQstring
6 | customHeadWithOwner bytes ')'
;

customHead : '.custom' customType '=' '('
;

compQstring : QSTRING {'+' QSTRING}

QSTRING : "c-style string"

customHeadWithOwner : '.custom' '(' ownerType ')' customType '='
'('
;

customType : callConv type typeSpec '::' '.ctor' '(' sigArgs0 ')'
| callConv type '.ctor' '(' sigArgs0 ')'
;

ownerType : typeSpec
| memberRef
;

Sample:
        callConv type typeSpec                              ::.ctor sigArgs0
.custom instance void [mscorlib]System.CLSCompliantAttribute::.ctor (bool) = ( 01 00 01 00 00 )

1 '.custom' callConv type typeSpec '::' '.ctor' '(' sigArgs0 ')'
1 '.custom' callConv type '.ctor' '(' sigArgs0 ')'
2 '.custom' callConv type typeSpec '::' '.ctor' '(' sigArgs0 ')' '=' compQstring
-> .custom instance void [assembly]Lala.GagaAttribute::.ctor(type1 par1) = "lala"+"gaga"
2 '.custom' callConv type '.ctor' '(' sigArgs0 ')' '=' compQstring
-> .custom instance Lala.GagaAttribute::.ctor(type1 par1) = "lala"+"gaga"
			|customHead												 | bytes ')'
            |customType										   |
3 '.custom' callConv type typeSpec '::' '.ctor' '(' sigArgs0 ')' = '(' bytes ')'
3 '.custom' callConv type '.ctor' '(' sigArgs0 ')' = '(' bytes ')'
4 like 1, with optional '(' ownerType ')' after .custom



 */
