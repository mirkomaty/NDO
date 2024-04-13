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


using System;
using System.Collections.Generic;
using System.Text;

namespace NDOEnhancer.Ecma335
{
    class EcmaTypeReferenceName : IEcmaDefinition
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
            int p = 0;
            EcmaDottedName dottedName = new EcmaDottedName();
            if (!dottedName.Parse(input))
                return false;

            p = dottedName.NextTokenPosition;

            while (input[p] == '/')
            {
                p++;
                if (!dottedName.Parse(input.Substring(p)))
                    throw new EcmaILParserException("DottedName", input.Substring(0, p), input);
                p += dottedName.NextTokenPosition;
            }
            content = input.Substring(0, p);
            nextTokenPosition = p;

            return true;
        }
    }
}
