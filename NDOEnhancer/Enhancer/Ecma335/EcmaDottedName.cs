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