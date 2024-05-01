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

#if nix
namespace NDOEnhancer.Ecma335
{
    class EcmaArrayIndex : IEcmaDefinition
    {
        int nextTokenPosition;
        string content;

        public int NextTokenPosition => nextTokenPosition;
        public string Content => content;

        public bool Parse(string input)
        {
            int p = 1;
            while (input[p] != ']' && p < input.Length - 1)
                p++;

            if (input[p] != ']')
                return false;

            p++;
            content = input.Substring(0, p);
            nextTokenPosition = p;

            return true;
        }
    }
}

/*
 ArrayIndex ::=
 ‘[’ [ Bound [ ‘,’ Bound ]*] ‘]’ 
  
 Bound ::=
  ‘...’
| Int32
| Int32 ‘...’
| Int32 ‘...’ Int32

 */
#endif