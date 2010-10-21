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
using System.Text;
using System.IO;
using ClassGenerator;
using System.CodeDom;
using ClassGenerator.SchemaAnalyzer;

namespace Generator
{
	/// <summary>
	/// Zusammenfassung für RelationGenerator.
	/// </summary>
	internal class RelationGenerator
	{
		RelationNode relationNode;
		StringBuilder sb = new StringBuilder();
        NamespaceWrapper namespaceWrapper;

        public RelationGenerator(RelationNode relationNode, NamespaceWrapper namespaceWrapper)
		{
			this.relationNode = relationNode;
            this.namespaceWrapper = namespaceWrapper;
		}

		private void Write(string s)
		{
			sb.Append(s);
		}

		string GenerateNestedXpathCode(string[] xpathArray, CodeMemberMethod saveMethod)
		{
			string currentName = "myElement";
			//XmlElement connectionsElement = parentDocument.CreateElement("Connections");
			//parentNode.AppendChild(connectionsElement);
			CodeVariableReferenceExpression parentDocument = new CodeVariableReferenceExpression("parentDocument");
			CodeTypeReference xmlElement = new CodeTypeReference("XmlElement");
			for ( int i = 0; i < xpathArray.Length - 1; i++ )
			{
				string lowerName = NamespaceWrapper.GetXpathFromQualifiedName(xpathArray[i]);
				lowerName = lowerName.Substring(0, 1).ToLower() + lowerName.Substring(1) + "Element";
				CodeMethodInvokeExpression createElement = new CodeMethodInvokeExpression(parentDocument, "CreateElement", new CodePrimitiveExpression(xpathArray[i]));
				CodeVariableDeclarationStatement newElementDecl = new CodeVariableDeclarationStatement(xmlElement, lowerName, createElement);
				saveMethod.Statements.Add(newElementDecl);

				CodeVariableReferenceExpression newElement = new CodeVariableReferenceExpression(lowerName);
				CodeVariableReferenceExpression parentElement = new CodeVariableReferenceExpression(currentName);
				CodeMethodInvokeExpression appendChild = new CodeMethodInvokeExpression(parentElement, "AppendChild", newElement);
				saveMethod.Statements.Add(appendChild);

				currentName = lowerName;
			}

			return currentName;
		}

		public void GenerateCode(CodeTypeDeclaration persClass, CodeConstructor xmlConstructor, CodeMemberMethod saveToXmlMethod)
		{
			Relation relation = relationNode.Relation;
			// This is for FkRelations, which must exist as entries in the
			// tree in order to delete the relation mapping.
			if (relation.RelationDirection == RelationDirection.DirectedToMe)
				return;

			if ((relation is ForeignFkRelation) && relationNode.RelatedTableNode.Table.Skipped)
				return;

			if (relation.IsElement && ((TableNode)relationNode.Parent).Table.Skipped)
				return;

			if (relation.IsElement && relationNode.RelatedTableNode.Table.Skipped)
				return;

            bool useGenerics = true;  // Should be definable by the user
            string foreignClassShort = relationNode.RelatedTableNode.Table.ClassName;

			string relatedTableName = relationNode.RelatedTableNode.Name;

            try 
			{
                CodeAttributeDeclaration ndoRelation = new CodeAttributeDeclaration("NDORelation");

			    if (!relation.IsElement)
                    ndoRelation.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(foreignClassShort)));

                if (relation.IsComposite)
                    ndoRelation.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("RelationInfo"), "Composite")));

                if (!string.IsNullOrEmpty(relation.RelationName))
                    ndoRelation.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(relation.RelationName)));
    			

                string leftSide = null;
                if (relation.CodingStyle == CodingStyle.ArrayList)
				{
                    leftSide = "List";
				}
				else
				{
                    leftSide = "IList";
                }

                CodeMemberField cmf;
                if (!relation.IsElement)
                {
                    CodeTypeReference ctrLeftSide = new CodeTypeReference(leftSide);
                    CodeTypeReference ctrElement = new CodeTypeReference(foreignClassShort);
                    CodeTypeReference ctrRightSide = new CodeTypeReference("List");
                    ctrLeftSide.TypeArguments.Add(ctrElement);
                    ctrRightSide.TypeArguments.Add(ctrElement);
                    cmf = new CodeMemberField(ctrLeftSide, relation.FieldName);
                    cmf.InitExpression = new CodeObjectCreateExpression(ctrRightSide);
                    persClass.Members.Add(cmf);
					
					if ( xmlConstructor != null )
					{
						GenerateXmlLoadAndSaveCode( xmlConstructor, saveToXmlMethod, relation, foreignClassShort );
					}
                }
                else
                {
                    cmf = new CodeMemberField(foreignClassShort, relation.FieldName);
                    persClass.Members.Add(cmf);
                }
                cmf.CustomAttributes.Add(ndoRelation);
			}				
			catch (Exception ex)
			{
                persClass.Comments.Add(new CodeCommentStatement("!!!! Relation Generator error: " + ex.Message));
			}
           
        }

		private void GenerateXmlLoadAndSaveCode( CodeConstructor xmlConstructor, CodeMemberMethod saveToXmlMethod, Relation relation, string foreignClassShort )
		{			
			/*
				XmlNodeList nodeList = xmlNode.SelectNodes("SubNodeName");
				for(int i = 0; i < nodeList.Count; i = i + 1)
				{
					XmlNode subNode = nodeList[i];
					this.relationField.Add(new RelatedType(subNode));
				}
			*/
			TargetLanguage targetLanguage = ApplicationController.Instance.AssemblyNode.Assembly.TargetLanguage;
			string xpath = null;
			if ( relation.IsForeign )
			{
				xpath = ((ForeignFkRelation) relation).XPath;
			}
			else
				throw new Exception( "Internal Error 132 in RelationGenerator: Relation is not a ForeignFkRelation." );

			string nodeListName = relation.FieldName + "NodeList";
            CodeVariableReferenceExpression nsManager = new CodeVariableReferenceExpression("NamespaceManager");
            CodePropertyReferenceExpression nsInstance = new CodePropertyReferenceExpression(nsManager, "Instance");

			CodeVariableDeclarationStatement nodeListDecl = new CodeVariableDeclarationStatement( "XmlNodeList", nodeListName,
				new CodeMethodInvokeExpression( new CodeVariableReferenceExpression( "xmlNode" ), "SelectNodes", new CodePrimitiveExpression( xpath ), nsInstance ) );
			xmlConstructor.Statements.Add( nodeListDecl );

			CodeVariableReferenceExpression iExpr = new CodeVariableReferenceExpression( "i" );
			CodeFieldReferenceExpression relationField = new CodeFieldReferenceExpression( new CodeThisReferenceExpression(), relation.FieldName );

			CodeIterationStatement forLoop = new CodeIterationStatement(
			    new CodeAssignStatement( iExpr, new CodePrimitiveExpression( 0 ) ),
			    new CodeBinaryOperatorExpression(
			        iExpr,
			        CodeBinaryOperatorType.LessThan,
			        new CodeVariableReferenceExpression( nodeListName + ".Count" ) ),  // should work in C# and VB
			    new CodeAssignStatement(
			        iExpr,
			        new CodeBinaryOperatorExpression(
			            iExpr,
			            CodeBinaryOperatorType.Add,
			            new CodePrimitiveExpression( 1 ) ) ),
			    new CodeVariableDeclarationStatement( "XmlNode", "subNode",
			        new CodeIndexerExpression(
			            new CodeVariableReferenceExpression( nodeListName ),
			            iExpr ) ),
				new CodeExpressionStatement(
					new CodeMethodInvokeExpression( relationField, "Add",
						new CodeObjectCreateExpression( foreignClassShort,
							new CodeVariableReferenceExpression( "subNode" ) ) ) ) );
			xmlConstructor.Statements.Add( forLoop );

			/*
			for ( int i = 0; i < relatedContainer.Count; i = i + 1 )
			{ 
				RelatedType relObject = relContainer[i];
				relObject.Save(newElement, "Elementname", "Namespace");
			}
			*/
			string elementNodeName = null;
			string[] xpathParts = xpath.Split( '/' );
			if ( xpath.IndexOf( '/' ) > -1 )
				elementNodeName = GenerateNestedXpathCode( xpathParts, saveToXmlMethod );
			else
				elementNodeName = "myElement";

			string elementPath = xpathParts[xpathParts.Length - 1];
            string namespc = this.namespaceWrapper[NamespaceWrapper.GetPrefixFromQualifiedName(elementPath)];
            elementPath = NamespaceWrapper.GetXpathFromQualifiedName(elementPath);

            CodeExpression namespcExpr = new CodePrimitiveExpression(namespc);

			forLoop = new CodeIterationStatement(
			    new CodeAssignStatement( iExpr, new CodePrimitiveExpression( 0 ) ),
			    new CodeBinaryOperatorExpression(
			        iExpr,
			        CodeBinaryOperatorType.LessThan,
			        new CodePropertyReferenceExpression( relationField, "Count" ) ),
			    new CodeAssignStatement(
			        iExpr,
			        new CodeBinaryOperatorExpression(
			            iExpr,
			            CodeBinaryOperatorType.Add,
			            new CodePrimitiveExpression( 1 ) ) ),
			    new CodeVariableDeclarationStatement( foreignClassShort, "relObject",
			        new CodeIndexerExpression( relationField, iExpr ) ),
				new CodeExpressionStatement(
					new CodeMethodInvokeExpression(
						new CodeVariableReferenceExpression( "relObject" ), "Save",
							new CodeVariableReferenceExpression( elementNodeName ),
							new CodePrimitiveExpression( elementPath ), namespcExpr ) ) );

			saveToXmlMethod.Statements.Add( forLoop );
		}
	}
}
