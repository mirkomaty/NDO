
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

namespace NDOEnhancer.Ecma335
{
    public class EcmaTypeSpec : IEcmaDefinition
    {
        public string Content { get; private set; } = String.Empty;
        public string ResolutionScope { get; private set; } = String.Empty;

        public string TypenameWithoutScope { get; private set; } = String.Empty;

        public int NextTokenPosition { get; private set; } = 0;
    
        public bool Parse( string input )
        {
            int p = 0;
            if (input[0] == '[')
            {
                p = input.IndexOf(']');
                if (p == -1)
                    throw new EcmaILParserException( "]", "[", input );
                p++;
                Content = ResolutionScope = input.Substring( 0, p );
            }

            var slashedName = new EcmaSlashedName();
            var success = slashedName.Parse( input.Substring( p ) );
            if (!success)
                throw new Exception( $"TypeSpec should contain a SlashedName in: {input}" );

            Content += slashedName.Content;
            TypenameWithoutScope = slashedName.Content;
            NextTokenPosition = p + slashedName.NextTokenPosition;
            return true;
        }
    }
}

/*

typeSpec : className
| '[' name1 ']'
| '[' '.module' name1 ']'
| type
;

className : '[' name1 ']' slashedName
| '[' '.module' name1 ']' slashedName
| slashedName
;

slashedName : name1
| slashedName '/' name1
;


name1 : id => empty
| DOTTEDNAME
| name1 '.' name1
;

 */