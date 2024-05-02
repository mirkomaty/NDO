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


using System.Globalization;
using System;

namespace NDOEnhancer.Ecma335
{
    public class EcmaCustomAttrDecl : IEcmaDefinition
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

		public string OwnerType { get; private set; } = String.Empty;

		void EatWhitespace(string input, ref int p)
		{
			char c;
            while (p < input.Length && char.IsWhiteSpace( c = input[p] ))
            {
                content += c;
                p++;
            }
        }

        public bool Parse(string input)
        {
			string methodKeyword = ".custom";
			if (!input.StartsWith( methodKeyword ))
				throw new EcmaILParserException( methodKeyword, "begin of custom declaration", input );

			content = methodKeyword;

			int p = 7;
			char c;

			EatWhitespace( input, ref p );

			if (input[p] == '(')
			{
				content += '(';
				p++;
				var ownerTypeSpec = new EcmaTypeSpec();
				if (!ownerTypeSpec.Parse( input.Substring( p ) ))
					throw new EcmaILParserException( "TypeSpec", "(", input );
				p += ownerTypeSpec.NextTokenPosition;
				content += ownerTypeSpec.Content;
				this.OwnerType = ownerTypeSpec.Content;
				if (input[p] != ')')
                    throw new EcmaILParserException( ")", "( TypeSpec", input );
				content += ')';
				p++;
            }

            EatWhitespace( input, ref p );

            // usually 'instance'
            EcmaCallConv callConv = new EcmaCallConv();
			if (!callConv.Parse( input.Substring( p ) ))
				throw new EcmaILParserException( "CallConv", ".custom", input );

            content += callConv.Content;
			p += callConv.NextTokenPosition;

            EatWhitespace( input, ref p );
            
			// This is the .ctor return type, which seems to be always void.
            EcmaType returnType = new EcmaType();
			if (!returnType.Parse( input.Substring( p ) ))
                throw new EcmaILParserException( "Type", callConv.Content, input );

			content += returnType.Content;
			p += returnType.NextTokenPosition;

            EatWhitespace( input, ref p );

			if (input[p] != '.') // from .ctor
			{
				EcmaTypeSpec typeSpec = new EcmaTypeSpec();
				if (!typeSpec.Parse( input.Substring( p ) ))
                    throw new EcmaILParserException( "TypeSpec", returnType.Content, input );

				this.resolutionScope = typeSpec.ResolutionScope;
				this.typeName = typeSpec.Content.Substring(this.resolutionScope.Length);
				this.content += typeSpec.Content;
				p += typeSpec.NextTokenPosition;

				if (input.Substring(p, 2) != "::")
                    throw new EcmaILParserException( "::", typeSpec.Content, input );

				p += 2;
				this.content += "::";
            }

			if (input.Substring( p, 5 ) != ".ctor")
				throw new Exception( "CustomAttribute must contain a .ctor declaration in: " + input );

			p += 5;
			content += ".ctor";

			EatWhitespace( input, ref p );

            EcmaParameterList parList = new EcmaParameterList();
            if (!parList.Parse( input.Substring( p ) ))
                throw new EcmaILParserException( "ParameterList", input.Substring( 0, p ), input );

            content += parList.Content;
            p += parList.NextTokenPosition;
			// strip off the brackets
            this.parameterList = parList.Content.Substring(1, parList.Content.Length - 2);

			EatWhitespace( input, ref p );

			if (p < input.Length && ( c = input[p] ) == '=')
			{
				content += c;
				p++;

				EatWhitespace( input, ref p );

				var remainder = input.Substring(p);

                // TODO: We should implement compQString as an alternative here.
                // However, there is currently no application for it
                //if (remainder[0] == '"')
                //{
                //	var compQString = new EcmaCompQString();
                //	compQString.Parse( remainder );
                //	var qstring = compQString.Content;
                //	// don't know what to do with qstring.
                //}

                var start = remainder.IndexOf( "(" );
				var end = remainder.IndexOf( ")", start );

				if (start < 0)
					throw new EcmaILParserException( "(", "=", input );
				if (end < 0)
					throw new EcmaILParserException( ")", "(", input );

				start++;

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
Summary:
'.custom' ['(' ownerType ')'] callConv type [typeSpec '::'] '.ctor' '(' sigArgs0 ')' ['=' '(' bytes ')' | compQstring]

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

customHeadWithOwner : '.custom' '(' ownerType ')' customType '=' '('
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
5 like 2, with optional '(' ownerType ')' after .custom
6 '.custom' '(' ownerType ')' callConv type typeSpec '::' '.ctor' '(' sigArgs0 ')' '=' '(' bytes ')'
6 '.custom' '(' ownerType ')' callConv type '.ctor' '(' sigArgs0 ')' '=' '(' bytes ')'



 */
