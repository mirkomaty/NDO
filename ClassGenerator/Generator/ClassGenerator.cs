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
using System.Xml;
using System.Collections;
using System.IO;
using ClassGenerator;

using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using ClassGenerator.SchemaAnalyzer;

namespace Generator
{
	/// <summary>
	/// Zusammenfassung für ClassGenerator.
	/// </summary>
	internal class ClassGenerator
	{
		TableNode tableNode;
		bool generateXmlConstructor;
		TargetLanguage targetLanguage;
        NamespaceWrapper namespaceWrapper = null;

		public ClassGenerator(TableNode tableNode)
		{
			this.tableNode = tableNode;
            Database database = ((DatabaseNode)tableNode.Parent).Database;
			this.generateXmlConstructor = database.IsXmlSchema;
            if (this.generateXmlConstructor)
            {
                this.namespaceWrapper = (NamespaceWrapper)database.DataSet.ExtendedProperties["namespacewrapper"];
            }
			this.targetLanguage = ApplicationController.Instance.AssemblyNode.Assembly.TargetLanguage;
		}

		/// <summary>
		/// Quick & Dirty Hack to skip collection elements in the Xml Schema. If we have a schema like that:
		/// RootElement / Actions / Action
		/// the Action elements should be a 1:n child of RootElement.
		/// </summary>
		/// <param name="cunit"></param>
		public void AddSubelementsOfSkippedElement()
		{
			if ( !this.generateXmlConstructor )  // Generate subelements only, if we reverse engineer a Xml Schema
				return;

			TableNode parentNode = (TableNode) this.tableNode.UserData["parentnode"];
			AddSubelementsOfSkippedElement( parentNode, this.tableNode.Name );
		}

		void AddSubelementsOfSkippedElement( TableNode parentNode, string xpath )
		{
			if ( parentNode.Table.Skipped )
			{
				TableNode pparentNode = (TableNode) parentNode.UserData["parentnode"];
				string path = xpath + '/'+ parentNode.Name;
				AddSubelementsOfSkippedElement( pparentNode, path );
				return;
				//-------
			}

			CodeCompileUnit cunit = (CodeCompileUnit) parentNode.UserData["cunit"];
			CodeNamespace ns = cunit.Namespaces[0];
			CodeTypeDeclaration persClass = ns.Types[0];
			CodeConstructor xmlConstructor = null;
			CodeMemberMethod xmlSaveMethod = null;

			foreach ( CodeTypeMember member in persClass.Members )
			{
				if ( member.Name == ".ctor" )
				{
					CodeConstructor cc = (CodeConstructor) member;
					if ( cc.Parameters.Count > 0 )
					{
						xmlConstructor = cc;
					}
				}
				else if ( member.Name == "Save" )
				{
					CodeMemberMethod cmm = (CodeMemberMethod) member;
					if ( cmm.Parameters.Count == 1 && cmm.Parameters[0].Type.BaseType == "XmlNode" )
						xmlSaveMethod = cmm;
				}
				if ( xmlConstructor != null && xmlSaveMethod != null )
					break;
			}

			/*
						XmlElement subElement = parentNode.OwnerDocument.CreateElement("MyElement");
						newElement.AppendChild(subElement);
						newElement = subElement;
			*/
            int p = xpath.LastIndexOf(':');
            string namespc = this.namespaceWrapper[NamespaceWrapper.GetPrefixFromQualifiedName(xpath)];
            string elementName = NamespaceWrapper.GetXpathFromQualifiedName(xpath);

			CodePrimitiveExpression myElementName = new CodePrimitiveExpression( elementName );
            CodePrimitiveExpression myNamespace = new CodePrimitiveExpression(namespc);
			CodeVariableReferenceExpression newElement = new CodeVariableReferenceExpression( "myElement" );
			CodeVariableReferenceExpression subElement = new CodeVariableReferenceExpression( "subElement" );
			CodeVariableReferenceExpression ownerDoc = new CodeVariableReferenceExpression( "parentNode.OwnerDocument" );
            CodeMethodInvokeExpression createElementCall;
            if (namespc != null)
			    createElementCall = new CodeMethodInvokeExpression( ownerDoc, "CreateElement", myElementName,  myNamespace );
            else
			    createElementCall = new CodeMethodInvokeExpression( ownerDoc, "CreateElement", myElementName );
			CodeVariableDeclarationStatement newElementAssign = new CodeVariableDeclarationStatement(new CodeTypeReference("XmlElement"), "subElement", createElementCall );
			xmlSaveMethod.Statements.Add( newElementAssign );

			CodeMethodInvokeExpression appendChild = new CodeMethodInvokeExpression( newElement, "AppendChild", subElement );
			xmlSaveMethod.Statements.Add( appendChild );

			CodeAssignStatement backAssign = new CodeAssignStatement( newElement, subElement );
			xmlSaveMethod.Statements.Add( backAssign );
			/*
			xmlNode = xmlNode.SelectSingleNode("xpath");
			*/
			CodeVariableReferenceExpression xmlNode = new CodeVariableReferenceExpression( "xmlNode" );
			CodePrimitiveExpression xPathExpr = new CodePrimitiveExpression( xpath );
            CodeVariableReferenceExpression nsManager = new CodeVariableReferenceExpression("NamespaceManager");
            CodePropertyReferenceExpression nsInstance = new CodePropertyReferenceExpression(nsManager, "Instance");
			CodeMethodInvokeExpression selectNodes = new CodeMethodInvokeExpression( xmlNode, "SelectSingleNode", xPathExpr, nsInstance );
			CodeAssignStatement xmlNodeAssign = new CodeAssignStatement( xmlNode, selectNodes );
			xmlConstructor.Statements.Add( xmlNodeAssign );

			GenerateMembers( persClass, xmlConstructor, xmlSaveMethod );


			// xmlNode = xmlNode.ParentNode;
			CodePropertyReferenceExpression parentNodeProperty = new CodePropertyReferenceExpression( xmlNode, "ParentNode" );
			xmlNodeAssign = new CodeAssignStatement(xmlNode, parentNodeProperty);
			xmlConstructor.Statements.Add( xmlNodeAssign );

			// newElement = (XmlElement) newElement.ParentNode;
			parentNodeProperty = new CodePropertyReferenceExpression( newElement, "ParentNode" );
			CodeCastExpression xmlElementCast = new CodeCastExpression( "XmlElement", parentNodeProperty );
			backAssign = new CodeAssignStatement( newElement, xmlElementCast );
			xmlSaveMethod.Statements.Add( backAssign );
		}

		public void GenerateCode(CodeCompileUnit cunit)
		{
            CodeNamespace ns = new CodeNamespace(tableNode.Table.Namespace);
            cunit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			if ( this.generateXmlConstructor )
			{
				ns.Imports.Add( new CodeNamespaceImport( "System.Xml" ) );
				ns.Imports.Add(new CodeNamespaceImport("NDO.Xml"));
			}
            ns.Imports.Add(new CodeNamespaceImport("NDO"));

            CodeTypeDeclaration persClass = new CodeTypeDeclaration(tableNode.Table.ClassName);
            ns.Types.Add(persClass);
            persClass.CustomAttributes.Add(GetNDOPersistentAttribute());
            if (!string.IsNullOrEmpty(tableNode.Table.Summary))
            {
                persClass.Comments.Add(new CodeCommentStatement("<summary>", true));
                persClass.Comments.Add(new CodeCommentStatement(tableNode.Table.Summary, true));
                persClass.Comments.Add(new CodeCommentStatement("</summary>", true));
            }

			if (tableNode.Table.NdoOidType != string.Empty)
                persClass.CustomAttributes.Add(GetNDOOidAttribute(tableNode.Table.NdoOidType));

            // Add empty constructor
            CodeConstructor cc = new CodeConstructor() ;
            cc.Attributes = MemberAttributes.Public ;
            persClass.Members.Add(cc);

			CodeConstructor xmlConstructor = null;
			CodeMemberMethod saveXmlMethod = null;

			// If we analyze an Xml schema we need a constructor with an Xml node as parameter
			// and a Save(XmlNode xn) method.
			if (this.generateXmlConstructor)
			{
				xmlConstructor = new CodeConstructor();
				xmlConstructor.Parameters.Add(new CodeParameterDeclarationExpression("XmlNode", "xmlNode"));
				xmlConstructor.Attributes = MemberAttributes.Public;
				persClass.Members.Add(xmlConstructor);
                xmlConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
                xmlConstructor.Comments.Add(new CodeCommentStatement("Constructs an object of type " + this.tableNode.Name + " and initializes it using an XmlNode.", true));
                xmlConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
                xmlConstructor.Comments.Add(new CodeCommentStatement(@"<param name=""xmlNode"">The XmlNode containing all information to initialize the object.</param>", true));

                CodeThisReferenceExpression thisExpr = new CodeThisReferenceExpression();
                CodeVariableReferenceExpression parentNode = new CodeVariableReferenceExpression("parentNode");
#if DontGenerateDefaultSaveMethod
				// Default Save Method, which can be used for the 
				saveXmlMethod = new CodeMemberMethod();
				saveXmlMethod.Name = "Save";
				saveXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression("XmlNode", "parentNode"));
				saveXmlMethod.Attributes = MemberAttributes.Public;
				persClass.Members.Add(saveXmlMethod);
				/*
				 Save(parentNode, "MyElementName");
				 */
				CodeThisReferenceExpression thisExpr = new CodeThisReferenceExpression();
				CodeVariableReferenceExpression parentNode = new CodeVariableReferenceExpression("parentNode");
				CodeMethodInvokeExpression saveInvoke = new CodeMethodInvokeExpression(thisExpr, "Save", parentNode, new CodePrimitiveExpression(this.tableNode.Name));
                saveXmlMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement("This method is the default save method to be used to store document root objects.", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement(@"<param name=""parentNode"">The parent node of the current element.</param>", true));               

				saveXmlMethod.Statements.Add(saveInvoke);
#endif
				saveXmlMethod = new CodeMemberMethod();
				saveXmlMethod.Name = "Save";
				saveXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression("XmlNode", "parentNode"));
				saveXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "elementName"));
                saveXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "elementNamespace"));
                //saveXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression("string", "nsPrefix"));
				saveXmlMethod.Attributes = MemberAttributes.Public;
				persClass.Members.Add(saveXmlMethod);
                saveXmlMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement("This method will be used for non-root objects.", true));
				saveXmlMethod.Comments.Add(new CodeCommentStatement("Note that a complex type can be saved by different parent nodes using different element names.", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement(@"<param name=""parentNode"">The parent node of the current element.</param>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement(@"<param name=""elementName"">The name of the current element.</param>", true));
                saveXmlMethod.Comments.Add(new CodeCommentStatement(@"<param name=""elementNamespace"">If a namespace is used, the namespace, otherwise null.</param>", true));

/*
						XmlDocument parentDocument = parentNode.OwnerDocument;
						XmlElement myElement = parentDocument.CreateElement("MyElement");
						parentNode.AppendChild(myElement);
*/
				CodeTypeReference xmlDocument = new CodeTypeReference("XmlDocument");
				CodeVariableDeclarationStatement parentDocumentDecl = new CodeVariableDeclarationStatement(xmlDocument, "parentDocument", new CodeAssignAsExpression("parentNode", "XmlDocument", this.targetLanguage));
				saveXmlMethod.Statements.Add(parentDocumentDecl);
				CodeVariableReferenceExpression parentDocument = new CodeVariableReferenceExpression("parentDocument");
				CodeVariableReferenceExpression ownerDoc = new CodeVariableReferenceExpression("parentNode.OwnerDocument");
				CodeAssignStatement parentDocumentAssign = new CodeAssignStatement(parentDocument, ownerDoc);
				CodeCheckForNullExpression equalsNull = new CodeCheckForNullExpression(parentDocument, this.targetLanguage);
				CodeConditionStatement ifNull = new CodeConditionStatement(equalsNull, parentDocumentAssign);
				saveXmlMethod.Statements.Add(ifNull);

				CodeVariableReferenceExpression myElement = new CodeVariableReferenceExpression("elementName");
                CodeVariableReferenceExpression myNamespace = new CodeVariableReferenceExpression("elementNamespace");
                CodeMethodInvokeExpression createElementCall = new CodeMethodInvokeExpression(parentDocument, "CreateElement", myElement, myNamespace);
				CodeVariableDeclarationStatement newElementDecl = new CodeVariableDeclarationStatement("XmlElement", "myElement", createElementCall);
				saveXmlMethod.Statements.Add(newElementDecl);

				CodeMethodInvokeExpression appendChild = new CodeMethodInvokeExpression(parentNode, "AppendChild", new CodeVariableReferenceExpression("myElement"));
				saveXmlMethod.Statements.Add(appendChild);
				int i = 0;
				foreach(RelationNode rn in tableNode.RelationNodes)
				    if (!rn.Relation.IsElement)
				        i++;
				if ( i > 0 )
				{
				    CodeVariableDeclarationStatement int_i = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof (int)), "i");
				    saveXmlMethod.Statements.Add(int_i);
				    xmlConstructor.Statements.Add(int_i);
				}
			}

			GenerateMembers( persClass, xmlConstructor, saveXmlMethod );

		}

		private void GenerateMembers( CodeTypeDeclaration persClass, CodeConstructor xmlConstructor, CodeMemberMethod saveXmlMethod )
		{
			foreach ( ColumnNode cn in tableNode.ColumnNodes )
			{
				if ( cn.IsMapped ) // is mapped
				{
					CodeMemberField field = new CodeMemberField( cn.ShortType, cn.Column.FieldName );
					persClass.Members.Add( field );
					if ( cn.IsPrimary )
						field.CustomAttributes.Add( new CodeAttributeDeclaration( "NDOObjectId" ) );
					if ( generateXmlConstructor )
						GenerateXmlSaveCodeForMember( xmlConstructor, saveXmlMethod, cn, field );
				}
			}

			foreach ( RelationNode rn in tableNode.RelationNodes )
			{
				new RelationGenerator( rn, this.namespaceWrapper ).GenerateCode( persClass, xmlConstructor, saveXmlMethod );
			}

			foreach ( ColumnNode cn in tableNode.ColumnNodes )
			{
				if ( cn.IsMapped )
					new PropertyGenerator( cn ).GenerateCode( persClass );
			}

			foreach ( RelationNode rn in tableNode.RelationNodes )
			{
				if ( rn.Relation.RelationDirection == RelationDirection.DirectedToMe )
					continue;
				new PropertyGenerator( rn ).GenerateCode( persClass );
			}
		}

		private static void GenerateXmlSaveCodeForMember( CodeConstructor xmlConstructor, CodeMemberMethod saveXmlMethod, ColumnNode columnNode, CodeMemberField field )
		{
			bool isElement = false;
			if (columnNode.UserData.ContainsKey("schemaType"))
				isElement = columnNode.UserData["schemaType"].ToString() == "Element";

			string setMethodName = null;
			string getMethodName = null;
			if ( isElement )
			{
				setMethodName = "SetElement";
				getMethodName = "GetElement";
			}
			else
			{
				setMethodName = "SetAttribute";
				getMethodName = "GetAttribute";
			}

			Type t = Type.GetType( columnNode.Column.Type );
			CodeTypeOfExpression fieldType = null;
			if ( t == null )
				fieldType = new CodeTypeOfExpression( t );
			else
				fieldType = new CodeTypeOfExpression( columnNode.Column.Type );
			CodePrimitiveExpression attrName = new CodePrimitiveExpression( columnNode.Name );
			CodeFieldReferenceExpression thisField = new CodeFieldReferenceExpression( new CodeThisReferenceExpression(), field.Name );

			/*
			this.field = (fieldType) XmlHelper.GetAttribute(xmlNode, "AttrName", typeof(fieldType));
			*/
			CodeAssignStatement cas = new CodeAssignStatement( thisField,
				new CodeCastExpression( columnNode.Column.Type,
					new CodeMethodInvokeExpression(
						new CodeVariableReferenceExpression( "XmlHelper" ), getMethodName,
							new CodeVariableReferenceExpression( "xmlNode" ), attrName, fieldType ) ) );

			xmlConstructor.Statements.Add( cas );
			/*
			XmlHelper.SetAttribute(newElement, "AttrName", this.field, typeof(fieldType));
			 */
			CodeVariableReferenceExpression xmlHelper = new CodeVariableReferenceExpression( "XmlHelper" );
			CodeVariableReferenceExpression newElement = new CodeVariableReferenceExpression( "myElement" );
			CodeMethodInvokeExpression setAttributeCall = new CodeMethodInvokeExpression( xmlHelper, setMethodName, newElement, attrName, thisField, fieldType );
			saveXmlMethod.Statements.Add( new CodeExpressionStatement( setAttributeCall ) );
		}


        CodeAttributeDeclaration GetNDOPersistentAttribute()
        {
            CodeAttributeDeclaration cad = new CodeAttributeDeclaration("NDOPersistent");
            return cad;
        }
        CodeAttributeDeclaration GetNDOOidAttribute(string type)
        {
            CodeAttributeDeclaration cad = new CodeAttributeDeclaration("NDOOidType");
            cad.Arguments.Add(GetTypeArg(type));
            return cad;
        }
        public static CodeAttributeArgument GetTypeArg(string typeName)
        {
            return new CodeAttributeArgument(new CodeTypeOfExpression(typeName));
        }

	}
}
