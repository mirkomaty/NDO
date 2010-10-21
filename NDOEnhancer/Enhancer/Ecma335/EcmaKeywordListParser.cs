//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections.Generic;
using System.Text;

namespace NDOEnhancer.Ecma335
{
    class EcmaKeywordListParser : IEcmaDefinition
    {
        string[] keywords;

        public EcmaKeywordListParser(string[] keywords)
        {
            this.keywords = keywords;
        }

        public bool Parse(string input)
        {
            string s = input;
            bool doContinue = true;
            int p = 0;
            while (doContinue && s.Length > 0)
            {
                doContinue = false;
                for (int i = 0; i < this.keywords.Length; i++)
                {
                    if (s.StartsWith(this.keywords[i]))
                    {
                        p += this.keywords[i].Length;
                        foundKeywords.Add(this.keywords[i]);
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
            nextTokenPosition += p;

            return true;
        }

        List<string> foundKeywords = new List<string>();
        public List<string> FoundKeywords
        {
            get { return this.foundKeywords; }
        }

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

    }
}
