
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

namespace NDOEnhancer.Ecma335
{
    public class EcmaGenericParameter : IEcmaDefinition
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

        List<string> elements = new List<string>();
        public List<string> Elements
        {
            get { return elements; }
        }

        public bool Parse(string input)
        {
            if (input[0] != '<')
                return false;

            int count = 1;
            int p = 1;
            char c;
            int startpos = p;
            while (count > 0)
            {
                if (p == input.Length)
                    return false;
                c = input[p];
                if (c == ',' && count == 1)
                {
                    elements.Add(input.Substring(startpos, p - startpos).Trim());
                    startpos = p + 1;
                    while (char.IsWhiteSpace(input[startpos]))
                        startpos++;
                }
                else if (c == '<')
                {
                    count++;
                }
                else if (c == '>')
                {
                    if (count == 1)
                        elements.Add(input.Substring(startpos, p - startpos).Trim());
                    count--;
                }
                p++;
            }

            content = input.Substring(0, p);
            nextTokenPosition = p;
            return true;
        }

    }
}
