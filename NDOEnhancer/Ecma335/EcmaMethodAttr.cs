
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
    public class EcmaMethAttr : IEcmaDefinition
    {
        static string[] keywords = 
            {
                "private",
                "public",
                "hidebysig",
                "newslot",
                "virtual",
                "abstract",
                "static",
                "specialname",
                "assembly",
                "family",
                "compilercontrolled",
                "famandassem",
                "famorassem",
                "final",
                "rtspecialname",
                "unmanagedexp",
                "reqsecobj",
                "privatescope",
                "pinvokeimpl",
                "strict"   // Bug #1905682
            };

        List<string> methodAttrs = new List<string>();
        public List<string> MethodAttrs
        {
            get { return methodAttrs; }
        }

        public bool Parse(string input)
        {
            int p = 0;
            string s = input;
            bool doContinue = true;

            while (doContinue && s.Length > 0)
            {
                doContinue = false;
                for (int i = 0; i < keywords.Length; i++)
                {
                    if (s.StartsWith(keywords[i]))
                    {
                        int methAttrStart = p;
                        p += keywords[i].Length;
                        if (keywords[i] == "pinvokeimpl")
                        {
                            while (char.IsWhiteSpace(input[p]))
                                p++;
                            if (input[p++] != '(')
                                throw new EcmaILParserException("(", "pinvokeimpl", input);
                            while (input[p] != ')')   // hopefully we'll have a closing bracket...
                                p++;
                            p++;
                        }
                        methodAttrs.Add(input.Substring(methAttrStart, p - methAttrStart));
                        while (p < input.Length && char.IsWhiteSpace(input[p]))
                            p++;
                        if (p < input.Length)
                        {
                            s = input.Substring(p);
                            doContinue = true;
                        }
                        break;
                    }
                }
            }

            if (p == 0)
                return false;

            content = input.Substring(0, p);
            nextTokenPosition = p;

            return true;

        }

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

    }
}
/*
MethAttr ::=
  abstract | assembly | compilercontrolled | famandassem | family | famorassem | final | hidebysig | newslot | pinvokeimpl ‘(’
    QSTRING [ as QSTRING ]
    PinvAttr* ‘)’
| private | public | rtspecialname | specialname | static | virtual | strict

| unmanagedexp 
| reqsecobj
  
  
PinvAttr ::=
  ansi
| autochar
| cdecl
| fastcall
| stdcall
| thiscall
| unicode
| platformapi
  
*/