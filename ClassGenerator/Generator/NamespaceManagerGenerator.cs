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
