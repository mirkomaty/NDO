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
using System.Text;
using ClassGenerator.SchemaAnalyzer;
using System.CodeDom;
using System.CodeDom.Compiler;

using System.Xml;




namespace Generator
{
    class NamespaceManagerGenerator
    {
        NamespaceWrapper namespaceWrapper;

        public NamespaceManagerGenerator(NamespaceWrapper nsw)
        {
            this.namespaceWrapper = nsw;
        }

        public void Generate(CodeCompileUnit cunit, string csNamespace)
        {
            CodeNamespace ns = new CodeNamespace(csNamespace);
            cunit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml"));
            CodeTypeDeclaration persClass = new CodeTypeDeclaration("NamespaceManager");
            ns.Types.Add(persClass);
            persClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            persClass.Comments.Add(new CodeCommentStatement("Use the static XmlNamespaceManager Instance of this class to provide namespaces for SelectNodes and SelectSingleNode calls." , true));
            persClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            CodeMemberField cmf = new CodeMemberField("XmlNamespaceManager", "Instance");
            cmf.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            persClass.Members.Add(cmf);

            CodeTypeConstructor cc = new CodeTypeConstructor();
            persClass.Members.Add(cc);

            CodeVariableReferenceExpression instance = new CodeVariableReferenceExpression("Instance");
            CodeObjectCreateExpression newNameTable = new CodeObjectCreateExpression("NameTable");
            CodeObjectCreateExpression newNsManager = new CodeObjectCreateExpression("XmlNamespaceManager", newNameTable);
            CodeAssignStatement instanceAssign = new CodeAssignStatement(instance, newNsManager);
            cc.Statements.Add(instanceAssign);

            foreach (XmlQualifiedName qn in this.namespaceWrapper.QualifiedNames)
            {
                CodePrimitiveExpression par1 = new CodePrimitiveExpression(qn.Name);
                CodePrimitiveExpression par2 = new CodePrimitiveExpression(qn.Namespace);
                CodeMethodInvokeExpression addNamespace = new CodeMethodInvokeExpression(instance, "AddNamespace", par1, par2);
                cc.Statements.Add(addNamespace);
            }
        }
    }
}
