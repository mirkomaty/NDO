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
    class EcmaMethodHeader : IEcmaDefinition
    {
        bool isInstance;
        public bool IsInstance
        {
            get { return isInstance; }
        }

        //bool isVarArg;
        //public bool IsVarArg
        //{
        //    get { return isVarArg; }
        //}

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

        List<string> methodAttrs;
        public List<string> MethodAttrs
        {
            get { return methodAttrs; }
        }

        List<string> implAttributes;
        public List<string> ImplAttributes
        {
            get { return this.implAttributes; }
        }

        string methodName;
        public string MethodName
        {
            get { return methodName; }
        }

        string parameterList;
        public string ParameterList
        {
            get { return parameterList; }
        }

        string ilType;
        public string IlType
        {
            get { return ilType; }
        }

        string signature;
        public string Signature
        {
            get { return signature; }
        }

        string genericParameterList = string.Empty;
        public string GenericParameterList
        {
            get { return genericParameterList; }
        }

        public bool Parse(string input)
        {
            string methodKeyword = ".method";
            if (!input.StartsWith(methodKeyword))
                throw new EcmaILParserException(methodKeyword, "begin of method declaration", input);

            content = methodKeyword;

            int p = 7;
            char c;
            while (char.IsWhiteSpace(c = input[p]))
            {
                content += c;
                p++;
            }

            EcmaMethAttr methodAttr = new EcmaMethAttr();
            if (!methodAttr.Parse(input.Substring(p)))
                return false;

            this.methodAttrs = methodAttr.MethodAttrs;

            p += methodAttr.NextTokenPosition;
            this.content += methodAttr.Content;

            while (char.IsWhiteSpace(c = input[p]))
            {
                content += c;
                p++;
            }

            EcmaCallConv callConv = new EcmaCallConv();

            if (callConv.Parse(input.Substring(p)))
            {
                p += callConv.NextTokenPosition;
                this.content += callConv.Content;
                if (callConv.Content.StartsWith("instance"))
                    this.isInstance = true;

                this.methodAttrs.AddRange(callConv.FoundKeywords);

                while (char.IsWhiteSpace(c = input[p]))
                {
                    content += c;
                    p++;
                }
            }

            EcmaType type = new EcmaType();

            if (!type.Parse(input.Substring(p)))
                throw new EcmaILParserException("Type in MethodHeader", input.Substring(0, p), input);

            this.content += type.Content;
            this.ilType = type.Content;
            p += type.NextTokenPosition;

            while (char.IsWhiteSpace(c = input[p]))
            {
                content += c;
                p++;
            }

            string s = input.Substring(p);

            if (s.StartsWith("marshal"))
            {
                int q = 7;
                while (char.IsWhiteSpace(s[q]))
                    q++;
                if (s[q] != '(')
                    throw new EcmaILParserException("(", "marshal", input);

                q++;

                int count = 1;
                while (count > 0 && q < s.Length)
                {
                    if (s[q] == ')')
                        count--;
                    if (s[q] == '(')
                        count++;
                    q++;
                }
                if (count > 0)
                    throw new EcmaILParserException(")", s, input);

                content += s.Substring(0, q);

                p += q;
                while (char.IsWhiteSpace(c = input[p]))
                {
                    content += c;
                    p++;
                }
            }

            EcmaMethodName emethodName = new EcmaMethodName();
            if (!emethodName.Parse(input.Substring(p)))
                throw new EcmaILParserException("MethodName", input.Substring(0, p), input);

            content += emethodName.Content;
            p += emethodName.NextTokenPosition;
            this.methodName = emethodName.Content;

            while (char.IsWhiteSpace(c = input[p]))
            {
                content += c;
                p++;
            }

            if (input[p] == '<')
            {
                EcmaGenericParameter genPar = new EcmaGenericParameter();
                genPar.Parse(input.Substring(p));
                content += genPar.Content;
                this.genericParameterList = genPar.Content;
                p += genPar.NextTokenPosition;
                while (char.IsWhiteSpace(c = input[p]))
                {
                    content += c;
                    p++;
                }
            }

            EcmaParameterList parList = new EcmaParameterList();
            if (!parList.Parse(input.Substring(p)))
                throw new EcmaILParserException("ParameterList", input.Substring(0, p), input);

            content += parList.Content;
            p += parList.NextTokenPosition;
            this.parameterList = parList.Content;
            this.signature = this.methodName + this.genericParameterList + this.parameterList;

            while (char.IsWhiteSpace(c = input[p]))
            {
                content += c;
                p++;
            }

            EcmaImplAttr implAttr = new EcmaImplAttr();
            if (!implAttr.Parse(input.Substring(p)))
                throw new EcmaILParserException("at least one of the ImplAttrs like 'cil'", input.Substring(0, p), input);

            this.implAttributes = implAttr.ImplAttributes;
            this.content += implAttr.Content;
            p += implAttr.NextTokenPosition;

            this.nextTokenPosition = p;

            return true;

        }

    }
}
/*
MethodHeader ::=
  MethAttr* [ CallConv ] Type 
              [ marshal ‘(’ [ NativeType ] ‘)’ ] 
              MethodName [ ‘<’ GenPars‘>’ ] ‘(’ Parameters ‘)’ ImplAttr*

 
 NativeType ::=
‘[’ ‘]’

| bool

| float32

| float64

| [ unsigned ] int

| [ unsigned ] int8

| [ unsigned ] int16

| [ unsigned ] int32

| [ unsigned ] int64

| lpstr

| lpwstr

| method

| NativeType ‘[’ ‘]’
| NativeType ‘[’ Int32 ‘]’
| NativeType 
‘[’ ‘+’ Int32 ‘]’
| NativeType 
‘[’ Int32 ‘+’ Int32 ‘]’

| as any

| byvalstr

| custom ‘(’ QSTRING,
  QSTRING  ‘)’
| fixed array [  Int32  ]

| fixed sysstring 
[  Int32  ]
| lpstruct

| lptstr 

| struct
  
  
  
 */
