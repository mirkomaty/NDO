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

namespace NDOEnhancer.Ecma335
{
    internal class EcmaCallConv : IEcmaDefinition
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

        List<string> foundKeywords = new List<string>();
        public List<string> FoundKeywords
        {
            get { return foundKeywords; }
        }

        public bool Parse(string input)
        {
            // In the most cases of a static method no calling convention exists.
            // So make the test fast.
            char c = input[0];
            if (c != 'i'
                && c != 'e'
                && c != 'd'
                && c != 'u'
                && c != 'v'
               )
                return false;

            int p = 0;
            string s = input;

            if (input.StartsWith("instance"))
            {
                p = 8;
                foundKeywords.Add("instance");
                while (char.IsWhiteSpace(input[p]))
                    p++;

                s = input.Substring(p);
                if (s.StartsWith("explicit"))
                {
                    foundKeywords.Add("explicit");
                    p += 8;
                    while (char.IsWhiteSpace(input[p]))
                        p++;
                    s = input.Substring(p);
                }
            }

            if (s.StartsWith("unmanaged"))
            {
                foundKeywords.Add("unmanaged");
                p += 9;
                while (char.IsWhiteSpace(input[p]))
                    p++;

                s = input.Substring(p);
                bool unmanagedCompleted = false;

                if (s.StartsWith("cdecl"))
                {
                    foundKeywords.Add("cdecl");
                    p += 5;
                    while (char.IsWhiteSpace(input[p]))
                        p++;
                    unmanagedCompleted = true;
                }
                else if (s.StartsWith("fastcall"))
                {
                    foundKeywords.Add("fastcall");
                    p += 8;
                    while (char.IsWhiteSpace(input[p]))
                        p++;
                    unmanagedCompleted = true;
                }
                else if (s.StartsWith("stdcall"))
                {
                    foundKeywords.Add("stdcall");
                    p += 7;
                    while (char.IsWhiteSpace(input[p]))
                        p++;
                    unmanagedCompleted = true;
                }
                else if (s.StartsWith("thiscall"))
                {
                    foundKeywords.Add("thiscall");
                    p += 8;
                    while (char.IsWhiteSpace(input[p]))
                        p++;
                    unmanagedCompleted = true;
                }
                if (!unmanagedCompleted)
                    throw new EcmaILParserException("one of cdecl, fastcall, stdcall and thiscall", "unmanaged", input);
            }
            else if (s.StartsWith("vararg"))
            {
                foundKeywords.Add("vararg");
                p += 6;
                while (char.IsWhiteSpace(input[p]))
                    p++;
            }
            else if (s.StartsWith("default"))
            {
                foundKeywords.Add("default");
                p += 7;
                while (char.IsWhiteSpace(input[p]))
                    p++;
            }
            if (p == 0)
                return false;

            nextTokenPosition = p;
            content = input.Substring(0, p);

            return true;
        }

    }
}
/*
CallConv ::= [ instance [ explicit ]] [ CallKind ]

 * CallKind ::=
  default
| unmanaged cdecl
| unmanaged fastcall
| unmanaged stdcall
| unmanaged thiscall
| vararg
*/
