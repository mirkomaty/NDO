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


using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace NDOEnhancer.Ecma335
{
    internal class EcmaParameterList : IEcmaDefinition
    {
        int nextTokenPosition;
        public int NextTokenPosition
        {
            get { return nextTokenPosition; }
        }
        string content;
        public string Content
        {
            get { return content; }
        }

        public bool Parse(string input)
        {
            if (input[0] != '(')
                return false;

            int count = 1;
            int p = 1;
            char c;
            while (count > 0)
            {
                if (p == input.Length)
                    return false;
                c = input[p];
                if (c == '(')
                    count++;
                if (c == ')')
                    count--;
                p++;
            }

            content = input.Substring(0, p);
            nextTokenPosition = p;
            return true;
        }

    }
}

/*
In case we need more information about the parameter list:

sigArgs0 : // EMPTY
| sigArgs1
;

sigArgs1: sigArg
| sigArgs1 ',' sigArg
;

sigArg: '...'
| paramAttr type
| paramAttr type id
| paramAttr type 'marshal' '(' nativeType ')'
| paramAttr type 'marshal' '(' nativeType ')'

paramAttr: { '[' 'in' | 'out' | 'opt'| int32 ']' }
*/
