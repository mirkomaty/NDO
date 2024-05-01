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
    class EcmaMethodDecl : IEcmaDefinition
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
			if (!methodAttr.Parse( input.Substring( p ) ))
				return false;

			this.methodAttrs = methodAttr.MethodAttrs;

			p += methodAttr.NextTokenPosition;
			this.content += methodAttr.Content;

			while (char.IsWhiteSpace( c = input[p] ))
			{
				content += c;
				p++;
			}

			EcmaMethodHeader methodHeader = new EcmaMethodHeader();
            if (!methodHeader.Parse(input.Substring(p)))
                return false;

            this.methodAttrs.AddRange( methodHeader.CallingConventions );

            this.isInstance = methodHeader.IsInstance;
            this.implAttributes = methodHeader.ImplAttributes;
            this.ilType = methodHeader.IlType;
            this.genericParameterList = methodHeader.GenericParameterList;
            this.content += methodHeader.Content;
            this.methodName = methodHeader.MethodName;
            this.parameterList = methodHeader.ParameterList;
            this.signature = methodHeader.Signature;

            this.nextTokenPosition = p + methodHeader.NextTokenPosition;

            return true;

        }
    }
}
