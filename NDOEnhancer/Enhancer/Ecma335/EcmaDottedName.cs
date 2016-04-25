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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NDOEnhancer.Ecma335
{
    class EcmaDottedName : IEcmaDefinition
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
            content = string.Empty;
            bool firstPassReady = false;

            Regex regex = new Regex(@"[A-Za-z_$@`?']");
            Match match = regex.Match(input.Substring(0, 1));
            if (!match.Success)
                return false;

            regex = new Regex(@"[A-Za-z_$@`?][A-Za-z0-9_$@`?]*|'[^']+'");

            do
            {
                if (firstPassReady)
                {
                    content += '.';
                    p++;
                }
                match = regex.Match(input.Substring(p));
                if (match.Success && match.Index == 0)
                {
                    content += match.Value;
                    p += match.Value.Length;
                }
                firstPassReady = true;
            }
            while (p < input.Length && input[p] == '.');
            nextTokenPosition = p;
            return true;
        }
    }
}
/*
 DottedName ::= Id [‘.’ Id]*
 Id ::= 
  ID
| SQSTRING


 
 ID is a contiguous string of characters which starts with either 
 an alphabetic character (A–Z, a–z) or one of “_”, “$”, “@”, 
 “`” (grave accent), or “?”, and is followed by any number of 
 alphanumeric characters  (A–Z, a–z, 0–9) or the characters “_”, “$”, “@”, “`” (grave accent), and “?”. 
*/