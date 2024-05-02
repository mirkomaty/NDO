
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

using System;

namespace NDOEnhancer.Ecma335
{
    public class EcmaCompQString : IEcmaDefinition
    {
        int nextTokenPosition;
        public int NextTokenPosition
        {
            get { return nextTokenPosition; }
        }
        string content = String.Empty;
        public string Content
        {
            get { return content; }
        }

        public bool Parse( string input )
        {
            var p = 0;
            if (input[p] != '"')
                return false;
            bool firstRun = true;

            do
            {
                if (!firstRun)
                    content += input[p++];
                if (input[p] != '"')
                    throw new EcmaILParserException( "leading \"", "+", input );
                content += input[p++];
                var q = input.IndexOf('"', p + 1);
                if (q == -1)
                    throw new EcmaILParserException( "trailing \"", "leading\"", input );
                var length = q - p + 1;
                content += input.Substring( p, length );
                p += length;
            } while (p < input.Length && input[p] == '+');

            nextTokenPosition = p;
            return true;
        }
    }
}
