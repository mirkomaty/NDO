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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using ClassGenerator;
using System.CodeDom;

namespace Generator
{
    /// <summary>
    /// Zusammenfassung für PropertyGenerator.
    /// </summary>
    internal class PropertyGenerator
    {
        RelationNode relationNode = null;
        ColumnNode columnNode = null;
        Relation relation;

        public PropertyGenerator(RelationNode rn)
        {
            this.relationNode = rn;
            this.relation = rn.Relation;
        }

        public PropertyGenerator(ColumnNode cn)
        {
            this.columnNode = cn;
        }

        public void GenerateCode(CodeTypeDeclaration persClass)
        {
			if (this.relationNode != null && (this.relation is ForeignFkRelation) && this.relationNode.RelatedTableNode.Table.Skipped)
				return;

			if (this.relationNode != null && this.relation.IsElement && ((TableNode)relationNode.Parent).Table.Skipped)
				return;

			if (this.relationNode != null && relation.IsElement && relationNode.RelatedTableNode.Table.Skipped)
				return;


            string typeStr;
            string name;
            bool isContainer = false;
            string summary = null;

            if (columnNode == null)  // We have a relation
            {
                isContainer = !relation.IsElement;
                string containerType = relation.CodingStyle.ToString();

                typeStr = isContainer ? containerType : relationNode.RelatedTableNode.Table.ClassName;
                name = relation.FieldName;
            }
            else
            {
                typeStr = columnNode.ShortType;
                name = columnNode.Column.FieldName;
                summary = columnNode.Column.Summary;
            }			

            string bigname;

            if (name.StartsWith("_"))
                bigname = name.Substring(1);
            else if (name.StartsWith("m_"))
                bigname = name.Substring(2);
            else
                bigname = name;
            bigname = bigname.Substring(0, 1).ToUpper() + bigname.Substring(1);

            string elementTyp = null;

			//CodeVariableReferenceExpression fieldReference = new CodeVariableReferenceExpression("this." + name);
			CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), name);

            if (isContainer)
            {
                elementTyp = relationNode.RelatedTableNode.Table.ClassName;
                string relationName;
                if (relation.RelationName != string.Empty)
                {
                    relationName = relation.RelationName;
                }
                else
                {
                    ForeignFkRelation foreignRelation = relation as ForeignFkRelation;
                    if (foreignRelation != null && foreignRelation.SingularFieldName != null)
                        relationName = foreignRelation.SingularFieldName;
                    else
                        relationName = elementTyp;
                }


                bool isComposite = (relation.IsComposite);
                string parameter = elementTyp.Substring(0, 1).ToLower();

                CodeMemberMethod cmm;
                cmm = new CodeMemberMethod();
				cmm.Attributes = MemberAttributes.Public;
                persClass.Members.Add(cmm);

                if (isComposite)
                {
                    cmm.Name = "New" + relationName;
                    cmm.ReturnType = new CodeTypeReference(elementTyp);
                    cmm.Statements.Add(new CodeVariableDeclarationStatement(elementTyp, parameter, new CodeObjectCreateExpression(elementTyp, new CodeExpression[] { })));
                    cmm.Statements.Add(new CodeMethodInvokeExpression(fieldReference, "Add", new CodeVariableReferenceExpression(parameter)));
                    cmm.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(parameter)));
                }
                else
                {
                    cmm.Name = "Add" + relationName;
                    cmm.Parameters.Add(new CodeParameterDeclarationExpression(elementTyp, parameter));
                    cmm.Statements.Add(new CodeMethodInvokeExpression(fieldReference, "Add", new CodeVariableReferenceExpression(parameter)));
                }

                cmm = new CodeMemberMethod();
				cmm.Attributes = MemberAttributes.Public;
                persClass.Members.Add(cmm);
                cmm.Parameters.Add(new CodeParameterDeclarationExpression(elementTyp, parameter));
                cmm.Name = "Remove" + relationName;
                CodeConditionStatement css = new CodeConditionStatement();
                css.Condition = new CodeMethodInvokeExpression(fieldReference, "Contains", new CodeVariableReferenceExpression(parameter));
                css.TrueStatements.Add(new CodeMethodInvokeExpression(fieldReference, "Remove", new CodeVariableReferenceExpression(parameter)));
                cmm.Statements.Add(css);
            }

            CodeMemberProperty cmp = new CodeMemberProperty();
            if (!string.IsNullOrEmpty(summary))
            {
                cmp.Comments.Add(new CodeCommentStatement("<summary>", true));
                cmp.Comments.Add(new CodeCommentStatement(summary, true));
                cmp.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
			cmp.Attributes = MemberAttributes.Public;
            cmp.HasGet = true;
            cmp.HasSet = true;
            cmp.Name = bigname;
            cmp.SetStatements.Add(new CodeAssignStatement(fieldReference, new CodeVariableReferenceExpression("value")));

            if (!isContainer)
            {
                persClass.Members.Add(cmp);
                cmp.Type = new CodeTypeReference(typeStr);
                cmp.GetStatements.Add(new CodeMethodReturnStatement(fieldReference));
            }
            else
            {
                CodeTypeReference containerType = null;
                if (relation.CodingStyle == CodingStyle.IList)
                    containerType = new CodeTypeReference("IList");
                else
                    containerType = new CodeTypeReference("List");
                containerType.TypeArguments.Add(new CodeTypeReference(elementTyp));
                persClass.Members.Add(cmp);
                cmp.Type = containerType;
                CodeTypeReference readOnlyList = new CodeTypeReference("NDOReadOnlyGenericList");
                readOnlyList.TypeArguments.Add(elementTyp);
                cmp.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(readOnlyList, fieldReference)));
            }
        }

    }
}
