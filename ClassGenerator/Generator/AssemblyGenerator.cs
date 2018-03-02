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
using System.IO;
using ClassGenerator;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.CodeDom;
using System.CodeDom.Compiler;
using ClassGenerator.SchemaAnalyzer;

namespace Generator
{
	/// <summary>
	/// Zusammenfassung für AssemblyGenerator.
	/// </summary>
	internal class AssemblyGenerator
	{
		DatabaseNode databaseNode;
		VsProject csProject;
		List<string> filesWithConflicts;

		public AssemblyGenerator(DatabaseNode databaseNode, VsProject csProject)
		{
			this.databaseNode = databaseNode;
			this.csProject = csProject;
		}

        public void GenerateCode()
        {
			this.filesWithConflicts = new List<string>();
            string errors = string.Empty;
			CodeDomProvider provider = null;
			if ( ApplicationController.Instance.AssemblyNode.Assembly.TargetLanguage == TargetLanguage.VB )
			{
				provider = new VBCodeProvider();
				Merge.CommentPrefix = "'";
			}
			else
			{
				provider = new CSharpCodeProvider();
				Merge.CommentPrefix = "//";
			}

			foreach (NDOTreeNode treenode in databaseNode.Nodes)
            {
                TableNode tn = treenode as TableNode;
                if (tn == null)
                    continue;
                if (!tn.Table.Skipped && 
					(tn.Table.MappingType == TableMappingType.MappedAsClass
                    || tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass))
                {
#if !DEBUG
                    try
                    {
#endif
                        string fileName = tn.Table.ClassName.ToString() + '.' + provider.FileExtension;
                        csProject.AddCsFile(fileName);
                        CodeCompileUnit cunit = new CodeCompileUnit();
						tn.UserData.Add("cunit", cunit);
						tn.UserData.Add("filename", Path.Combine(csProject.SourcePath, fileName));
                        new ClassGenerator(tn).GenerateCode(cunit);
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        errors += ex.Message + '\n';
                    }
#endif
                }

            }

			if (errors != string.Empty)
				goto hasErrors;

			foreach (NDOTreeNode treenode in databaseNode.Nodes)
            {
                TableNode tn = treenode as TableNode;
                if (tn == null)
                    continue;
                if (databaseNode.Database.IsXmlSchema && tn.Table.Skipped && 
					(tn.Table.MappingType == TableMappingType.MappedAsClass
                    || tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass))
                {
#if !DEBUG
                    try
                    {
#endif
                        new ClassGenerator(tn).AddSubelementsOfSkippedElement();
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        errors += ex.Message + '\n';
                    }
#endif
                }

            }

			if (errors != string.Empty)
				goto hasErrors;


            if (this.databaseNode.Database.DataSet.ExtendedProperties.ContainsKey("namespacewrapper"))
            {
                NamespaceWrapper nsw = (NamespaceWrapper)this.databaseNode.Database.DataSet.ExtendedProperties["namespacewrapper"];
#if !DEBUG
                try
                {
#endif

                string fileName = "NamespaceManager" + "." + provider.FileExtension;

                csProject.AddCsFile(fileName);

                fileName = Path.Combine(csProject.SourcePath, fileName);

                CodeCompileUnit cunit = new CodeCompileUnit();
                new NamespaceManagerGenerator(nsw).Generate(cunit, csProject.DefaultNamespace);
                CompileAndMergeCUnit(provider, cunit, fileName);
#if !DEBUG
                }
                catch (Exception ex)
                {
                    errors += ex.Message + '\n';
                }
#endif
            }




			foreach ( NDOTreeNode treenode in databaseNode.Nodes )
			{
				TableNode tn = treenode as TableNode;
				if ( tn == null )
					continue;
				if (tn.Table.Skipped)
					continue;
				if (tn.Table.MappingType == TableMappingType.NotMapped)
					continue;
				if (!tn.UserData.ContainsKey( "cunit" ))
					continue;
				CodeCompileUnit cunit = (CodeCompileUnit) tn.UserData["cunit"];
				string fileName = (string) tn.UserData["filename"];

				Merge.IgnoreSpaces = true;

                CompileAndMergeCUnit(provider, cunit, fileName);
			}

hasErrors:
            if (errors != string.Empty)
                throw new Exception("Errors occured while generating code:\n" + errors);
        }

        private void CompileAndMergeCUnit(CodeDomProvider provider, CodeCompileUnit cunit, string fileName)
        {
            MergeableFile mergeableFile = new MergeableFile(fileName, true);
            StreamWriter sw = new StreamWriter(mergeableFile.Stream);
            try
            {
                CodeGeneratorOptions cgo = new CodeGeneratorOptions();
                cgo.BracingStyle = "C";
                provider.GenerateCodeFromCompileUnit(cunit, sw, cgo);
                sw.Flush();
            }
            catch (Exception ex)
            {
                sw.Close();
                mergeableFile.Restore();
                throw ex;
            }
            if (mergeableFile.Write())
                this.filesWithConflicts.Add(fileName);

            sw.Close();
        }

		public List<string> FilesWithConflicts
		{
			get { return filesWithConflicts; }
		}

	}
}
