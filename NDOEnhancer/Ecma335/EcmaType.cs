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
using System.Text.RegularExpressions;

namespace NDOEnhancer.Ecma335
{
    public class EcmaType : IEcmaDefinition
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

        string resolutionScope;

        public string ResolutionScope => resolutionScope;

        static string[] builtInTypes = 
        {
            "bool",            "char",            "float32",
            "float64",            "int8",            "int16",            "int32",            "int64",            "uint8",            "uint16",            "uint32",            "uint64",            "native int",            "native unsigned int",            "object",            "string",            "typedref",            "unsigned int8",            "unsigned int16",            "unsigned int32",            "unsigned int64",            "void"
        };

		public static Dictionary<string, Type> BuiltInTypesDict { get; set; } = new Dictionary<string, Type>()
        {
            {"bool", typeof(System.Boolean) },
            {"char", typeof(System.Char) },
            {"float32", typeof(System.Single) },
            {"float64", typeof(System.Double) },
            {"int8", typeof(System.Byte) },
            {"int16", typeof(System.Int16) },
            {"int32", typeof(System.Int32) },
            {"int64", typeof(System.Int64) },
            {"uint8", typeof(System.Byte) },
            {"uint16", typeof(System.UInt16) },
            {"uint32", typeof(System.UInt32) },
            {"uint64", typeof(System.UInt64) },
            {"string", typeof(System.String) }
        };

        static Regex arrayRegex = new Regex(@"^\[\d+\]", RegexOptions.Compiled);

		public bool Parse(string input)
        {
            int p = 0;
            while (char.IsWhiteSpace(input[p]))
                p++;

            this.nextTokenPosition = p;

            string s = input.Substring(p);
            if (ParseGenericArgs(s))
                goto suffix;
            if (ParseBuiltInTypes(s))
                goto suffix;
            if (ParseClassOrValueType(s))
                goto suffix;
            if (ParseMethodPointer(s))
                goto suffix;

            return false;

        suffix:
            // Look ahead, if a suffix appears. This happens in all situations where the
            // standard defines recursively "Type ::= Type suffix", where suffix may be 
            // arbitrary valid code.
            // Don't touch nextTokenPosition or content if you're not sure,
            // wether a suffix appears.
            p = this.nextTokenPosition;
            char c;
            while (char.IsWhiteSpace(c = input[p]))
                p++;
            s = input.Substring(p);
            if (c == '&' || c == '*')
            {
                s = s.Substring(0, p + 1 - nextTokenPosition);
                content += s;
                nextTokenPosition = p;
            }
            else if (c == '<')
            {
                content += input.Substring(nextTokenPosition, p - nextTokenPosition); // add blanks
                nextTokenPosition = p;
                if (!ParseGenericSuffix(s))
                    throw new EcmaILParserException("GenericArgumentList", input.Substring(0, p), input);
            }
            else if (c == '[')
            {
                var match = arrayRegex.Match(s);
                if (match.Success)
                {
                    var value = match.Value;
                    content += input.Substring( nextTokenPosition, p - nextTokenPosition ); // add blanks
                    content += value;
                    nextTokenPosition = p + value.Length;
                }
            }
            else if (s.StartsWith("modopt") || s.StartsWith("modreq"))
            {
                content += input.Substring(nextTokenPosition, p - nextTokenPosition); // add blanks
                nextTokenPosition = p;
                ParseMod(s);
            }
            else if (s.StartsWith("pinned"))
            {
                content += input.Substring(nextTokenPosition, p - nextTokenPosition); // add blanks
                nextTokenPosition = p;
                content += "pinned";
                nextTokenPosition += 6;
            }

            return true;
        }

        bool ParseClassOrValueType(string s)
        {
            int p = 0;
            if (s.StartsWith("class "))
            {
                content = "class ";
                p = content.Length;
            }
            else if (s.StartsWith("valuetype "))
            {
                content = "valuetype ";
                p = content.Length;
            }
            if (p == 0)
                return false;
            while (char.IsWhiteSpace(s[p]))
                p++;

            EcmaTypeRef typeRef = new EcmaTypeRef();
            if (!typeRef.Parse(s.Substring(p)))
                throw new EcmaILParserException("TypeRef", content, s.Substring(p));

            content += typeRef.Content;
            p += typeRef.NextTokenPosition;
            this.resolutionScope = typeRef.ResolutionScope;

            nextTokenPosition += p;

            return true;
        }

        bool ParseBuiltInTypes(string s)
        {
            foreach (string t in builtInTypes)
            {
                if (s.StartsWith(t))
                {
                    nextTokenPosition += t.Length;
                    content = t;
                    return true;
                }
            }
            return false;
        }

        bool ParseGenericArgs(string s)
        {
            int p = 0;
            while (s[p] == '!')
            {
                p++;
            }
            if (p == 0)
                return false;
            while (char.IsWhiteSpace(s[p]))
                p++;
            if (char.IsDigit(s[p]))
            {
                while (char.IsDigit(s[p]))
                    p++;
            }
            else
            {
                // This seems to be a Microsoft extension to the standard
                // !Name instead of !int32
                EcmaDottedName dn = new EcmaDottedName();
                if (!dn.ParseId(s.Substring(p)))
                    throw new EcmaILParserException("Name", s.Substring(p), s);
                p += dn.NextTokenPosition;
            }
            content = s.Substring(0, p);
            nextTokenPosition += p;
            return true;
        }

        bool ParseMethodPointer(string s)
        {
            int p = 0;
            if (s.StartsWith("method"))
            {
                content += "method";
                p = content.Length;
            }
            if (p == 0) 
                return false;

            char c;
            while (char.IsWhiteSpace(c = s[p]))
            {
                p++;
                content += c;
            }

            EcmaCallConv callConv = new EcmaCallConv();
            if (!callConv.Parse(s.Substring(content.Length)))
                throw new EcmaILParserException("CallConv", content, s);

            content += callConv.Content;
            p += callConv.NextTokenPosition;

            EcmaType type = new EcmaType();
            if (!type.Parse(s.Substring(p)))
                throw new EcmaILParserException("Type", content, s);

            content += type.Content;
            p += type.NextTokenPosition;

            while (char.IsWhiteSpace(c = s[p]))
            {
                p++;
                content += c;
            }

            if (c != '*')
                throw new EcmaILParserException("*", content, s);

            content += c;

            while (char.IsWhiteSpace(c = s[p]))
            {
                p++;
                content += c;
            }

            EcmaParameterList parameterList = new EcmaParameterList();
            if (!parameterList.Parse(s.Substring(p)))
                throw new EcmaILParserException("ParameterList", content, s);

            content += parameterList.Content;
            p += parameterList.NextTokenPosition;

            nextTokenPosition += p;

            return true;

        }

        void ParseMod(string s)
        {
            int p = 6; // Length of "modopt" or "modreq"
            char c;

            while (char.IsWhiteSpace(c = s[p]))
                p++;

            if (c != '(')
                throw new EcmaILParserException("'('", s.Substring(0, 6), s);

            while (char.IsWhiteSpace(c = s[p]))
                p++;

            content += s.Substring(0, p);

            EcmaTypeRef typeRef = new EcmaTypeRef();
            if (!typeRef.Parse(s.Substring(p)))
                throw new EcmaILParserException("TypeRef", s.Substring(0, p), s);

            content += typeRef.Content;
            p += typeRef.NextTokenPosition;

            while (char.IsWhiteSpace(c = s[p]))
                p++;

            if (c != ')')
                throw new EcmaILParserException("'('", s.Substring(0, p), s);

            content += c;
        }

        bool ParseGenericSuffix(string s)
        {
            EcmaGenericParameter genPar = new EcmaGenericParameter();
            if (!genPar.Parse(s))
                return false;

            content += genPar.Content;
            nextTokenPosition += genPar.NextTokenPosition;
            return true;
        }
        
        public override string ToString()
        {
            return content;
        }
    }
}
/*
Type ::=
  ‘!’ Int32                                     -> ParseGenericArg| ‘!!’ Int32                                    -> ParseGenericArg
| bool
| char
| class TypeReference
| float32
| float64
| int8
| int16
| int32
| int64
| method CallConv Type ‘*’ 
      ‘(’ Parameters ‘)’
| native int
| native unsigned int
| object
| string
| Type ‘&’
| Type ‘*’
| Type ‘<’ GenArgs  ‘>’                         ->ParseGenericSuffix
| Type ‘[’ [ Bound [ ‘,’ Bound ]*] ‘]’
| Type modopt ‘(’ TypeReference ‘)’
| Type modreq ‘(’ TypeReference ‘)’
| Type pinned
| typedref
| valuetype TypeReference
| unsigned int8
| unsigned int16
| unsigned int32
| unsigned int64
| void
*/