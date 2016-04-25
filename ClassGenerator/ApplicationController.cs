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
using System.Data;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClassGenerator.ForeignKeyWizard;
using ClassGenerator.AssemblyWizard;
using ClassGenerator.IntermediateTableWizard;
using ClassGenerator.IntermediateClassWizard;
using ClassGenerator.AddPrimaryKeyWizard;
using WizardBase;
using Generator;
using NDO;
using NDOInterfaces;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für Controller.
	/// </summary>
#if DEBUG
	public class ApplicationController
#else
	internal class ApplicationController
#endif
	{
		static ApplicationController instance = new ApplicationController();
		public static ApplicationController Instance
		{
			get { return instance; }
		}

		static WizardControllerFactory wizardControllerFactory = new WizardControllerFactory(typeof(ApplicationController).Assembly, "");
		public static WizardControllerFactory WizardControllerFactory
		{
			get { return wizardControllerFactory; }
			set { wizardControllerFactory = value; }
		}

		DatabaseNode databaseNode;
		public DatabaseNode DatabaseNode
		{
			get { return databaseNode; }
			set { databaseNode = value; }
		}
		AssemblyNode assemblyNode;
		public AssemblyNode AssemblyNode
		{
			get { return assemblyNode; }
			set { assemblyNode = value; }
		}

		public void MappingChanged()
		{
			this.assemblyNode.Refresh();
		}

		string MakeCamelCase( string input )
		{
			string prefix = this.assemblyNode.Assembly.TargetLanguage == TargetLanguage.VB ? "_" : string.Empty;
			return prefix + input.Substring(0,1).ToLower() + input.Substring(1);
		}

        string MakePascalCase(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

		public void MakeForeignKey( DataRelation dataRelation )
		{
			// The FkRelation is owned by the child table, because it holds
			// the foreign key.
			// Therefore the XPath has to be stored in the ForeignFkRelation.
            string xpath = (string)dataRelation.ExtendedProperties["xpath"];

            TableNode ownTableNode = this.databaseNode[dataRelation.ChildTable.TableName];
			TableNode foreignTableNode = this.databaseNode[dataRelation.ParentTable.TableName];
			ColumnNode cn = ownTableNode[dataRelation.ChildColumns[0].ColumnName];
			FkRelation fkRelation = new FkRelation(cn.Text);
			RelationNode relationNode = new RelationNode(fkRelation, cn.Parent);
			ForeignKeyWizModel model = new ForeignKeyWizModel(relationNode, null);
			fkRelation.CodingStyle = CodingStyle.ArrayList;
			fkRelation.FieldName = MakeCamelCase(foreignTableNode.Text);
            fkRelation.RelationDirection = RelationDirection.DirectedToMe;// Bidirectional;
			relationNode.RelatedTableNode = foreignTableNode;
			fkRelation.RelatedTable = dataRelation.ParentTable.TableName;
			fkRelation.RelatedType = relationNode.RelatedTableNode.Table.ClassName;
			fkRelation.IsComposite = false;

            string singularFieldName = null;
            if (xpath != null)
                singularFieldName = MakePascalCase(xpath.Substring(xpath.LastIndexOf('/') + 1));
            else
                singularFieldName = MakePascalCase(ownTableNode.Text);

            singularFieldName = singularFieldName.Substring(singularFieldName.IndexOf(':') + 1);

            string foreignFieldName = MakeCamelCase(singularFieldName);
            if (foreignFieldName.EndsWith("y"))
            {
                char c = char.ToLower(foreignFieldName[foreignFieldName.Length - 2]);                
                if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
                    foreignFieldName += "s";
                else
                    foreignFieldName = foreignFieldName.Substring(0, foreignFieldName.Length - 1) + "ies";
            }
            else if (foreignFieldName.EndsWith("s"))
                foreignFieldName += "es";
            else if (foreignFieldName.EndsWith("f"))
                foreignFieldName = foreignFieldName.Substring(0, foreignFieldName.Length - 1) + "ves";
            else
                foreignFieldName += "s";
			fkRelation.ForeignFieldName = foreignFieldName;

			fkRelation.ForeignIsComposite = true;
			fkRelation.ForeignCodingStyle = CodingStyle.ArrayList;
			fkRelation.RelationName = string.Empty;
			MakeForeignKey(cn, model);
            fkRelation.ForeignRelation.SingularFieldName = MakePascalCase(singularFieldName);
			fkRelation.ForeignRelation.XPath = xpath;
		}

		public void MakeForeignKey(ColumnNode columnNode)
		{
			MakeForeignKey(columnNode, null);
		}

		public void MakeForeignKey(ColumnNode columnNode, ForeignKeyWizModel model)
		{
			FkRelation fkRelation = null;
			RelationNode relationNode = null;
			if ( model == null )
			{
				fkRelation = new FkRelation( columnNode.Name );
				relationNode = new RelationNode( fkRelation, columnNode.Parent );
			}
			else
			{
				fkRelation = (FkRelation) model.RelationNode.Relation;
				relationNode = model.RelationNode;
			}
			fkRelation.OwningTable = ((TableNode) relationNode.Parent).Table.Name;
			fkRelation.OwningType = ((TableNode) relationNode.Parent).Table.ClassName;
			IList tableNodes = new ArrayList();
			if ( model == null )
			{
				foreach(TableNode tnode in ((DatabaseNode)columnNode.Parent.Parent).TableNodes)
				{
					if (tnode.Table.MappingType == TableMappingType.MappedAsClass)
						tableNodes.Add(tnode);
				}
				model = new ForeignKeyWizModel( relationNode, tableNodes );
				IWizardController controller = ApplicationController.wizardControllerFactory.Create
					( "ForeignKeyWizController", "ForeignKeyWiz", "Foreign Key Wizard" );
				//controller.FrameSize = new Size( 544, 408 );
				DialogResult r = controller.Run( model );
				if ( r == DialogResult.OK )
				{
					MakeForeignKeyRelation( relationNode, columnNode, model );
				}
			}
			else
			{
					MakeForeignKeyRelation( relationNode, columnNode, model );
			}
		}


		private void MakeForeignKeyRelation(RelationNode relationNode, ColumnNode columnNode, ForeignKeyWizModel model)
		{
			FkRelation fkRelation = (FkRelation) relationNode.Relation;
			fkRelation.RelatedTable = relationNode.RelatedTableNode.Table.Name;
			fkRelation.RelatedType = relationNode.RelatedTableNode.Table.ClassName;
			TableNode tn = (TableNode) columnNode.Parent;
			//tn.Nodes. Remove(columnNode);
			columnNode.Remove();
			tn.Nodes.Add(relationNode);
			relationNode.OriginalColumnNode = columnNode;
			this.assemblyNode.Refresh();
			if (fkRelation.RelationDirection == RelationDirection.DirectedToMe 
				|| fkRelation.RelationDirection == RelationDirection.Bidirectional)
			{
				tn = databaseNode.FindTableNode(fkRelation.RelatedTable, true);					
				RelationNode rn = new RelationNode(fkRelation.ForeignRelation, tn);
				rn.RelatedTableNode = (TableNode) columnNode.Parent;
				tn.Nodes.Add(rn);
			}
		}


		public void DeleteRelation(RelationNode relationNode)
		{
			FkRelation fkRelation = relationNode.Relation as FkRelation;
			TableNode parentNode = null;
			if (fkRelation == null)
			{
				ForeignFkRelation ffkrel = relationNode.Relation as ForeignFkRelation;
				if (ffkrel == null)
					return;
				parentNode = (TableNode) this.databaseNode.FindNode(ffkrel.RelatedTable, typeof(TableNode));
				relationNode = parentNode.FindRelationNode(relationNode.Text, relationNode.Parent.Text);//(RelationNode) parentNode.FindNode(relationNode.Text, typeof(RelationNode));
				fkRelation = (FkRelation) relationNode.Relation;
				if (parentNode.Table.MappingType == TableMappingType.MappedAsIntermediateClass)
				{
					parentNode.UnmapIntermediateClass(null, EventArgs.Empty);
					return;
				}
			}
			else
			{
				parentNode = (TableNode) relationNode.Parent;
			}

			//parentNode.Nodes. Remove(relationNode);
			relationNode.Remove();
			parentNode.Nodes.Add(relationNode.OriginalColumnNode);
			
			Debug.Assert(fkRelation != null);
			if (fkRelation != null) 
			{
				if (fkRelation.RelationDirection != RelationDirection.DirectedFromMe)
				{
					TableNode tn = databaseNode.FindTableNode(fkRelation.RelatedTable, true);
					RelationNode nodeToRemove = tn.FindRelationNode(relationNode.Text, parentNode.Text); 
					if (nodeToRemove != null)
						nodeToRemove.Remove();
				}
			}
		}

		public void GenerateAssembly()
		{
			Assembly ass = assemblyNode.Assembly;
            string fullPath = Path.GetFullPath(ass.TargetDir);
            DirectoryInfo di = new DirectoryInfo(Path.GetPathRoot(fullPath));
            di.CreateSubdirectory(fullPath.Substring(3));
			string projExtension = ass.TargetLanguage == TargetLanguage.VB ? ".vbproj" : ".csproj";
            string projFile = Path.Combine(ass.TargetDir, ass.ProjectName + projExtension);
            string ndoProjFile = Path.ChangeExtension(projFile, ".ndoproj");
			VsProject vsProject = new VsProject(ass.TargetLanguage, projFile, ass.ProjectName, ass.TargetDir, ass.RootNamespace, databaseNode.Database.ConnectionType);
            NdoProject ndoProject = new NdoProject(ndoProjFile,
                Path.Combine(ass.TargetDir, "bin\\debug\\" + ass.ProjectName + ".dll"),
                Path.Combine(ass.TargetDir, "obj\\debug\\"),
                ass.TargetDir);
            ndoProject.Save();
			AssemblyGenerator assGen = new AssemblyGenerator(databaseNode, vsProject);
			assGen.GenerateCode();
			string filename = ass.TargetDir + "\\NDOMapping.xml";
			vsProject.AddXmlFile("NDOMapping.xml");
			new MappingGenerator(filename, databaseNode, assemblyNode).GenerateCode();
			vsProject.Save();
			List<string> filesWithConflicts = assGen.FilesWithConflicts;

			if (vsProject.HasConflicts)
				filesWithConflicts.Insert(0, projFile);
			if ( filesWithConflicts.Count > 0 )
			{
				string strConflicts = "Files with conflicts:\n\n";
				foreach ( string s in filesWithConflicts )
					strConflicts += s + '\n';
				strConflicts += "\nPlease resolve the conflicts, otherwise the project might not compile.";
				MessageBox.Show(strConflicts, "NDO Class Generator");
			}

			foreach (string oldFile in Directory.GetFiles(ass.TargetDir, "*.old"))
				File.Delete(oldFile);
		}

		public void MapIntermediateTable(TableNode tn)
		{
			IntermediateTable it = new IntermediateTable(tn.Text);
			IntermediateTableNode itn = new IntermediateTableNode(it, tn);

			IList tableNodes = databaseNode.TableNodes;
			tableNodes.Remove(tn);

			IntermediateTableWizardModel model = new IntermediateTableWizardModel(itn, tableNodes);

			IWizardController controller = ApplicationController.wizardControllerFactory.Create
				("IntermediateTableWizController", "IntTblWiz", "Intermediate Table Wizard");
			//controller.FrameSize = new Size(544, 416);
			DialogResult r = controller.Run(model);
			if (r == DialogResult.OK)
			{
				DatabaseNode parent = (DatabaseNode) tn.Parent;
				//tn.Parent.Nodes. Remove(tn);
				int index = parent.Nodes.IndexOf(tn);
				tn.Remove();
				tn.Parent.Nodes.Insert(index, itn);
				itn.TreeView.SelectedNode = itn;
				itn.Nodes.Add(tn);  // This ensures persisting the column nodes with file/save.
				RelatedTableInfo rti0 = it[0];
				RelatedTableInfo rti1 = it[1];
				TableNode relTn0 = null;
				TableNode relTn1 = null;
				RelationNode relNode0 = null;
				RelationNode relNode1 = null;
				foreach(TableNode loopTn in tableNodes)
				{
					if (loopTn.Text == rti0.Table)
					{
						relTn0 = loopTn;
						if (rti0.RelationDirection != RelationDirection.DirectedToMe)
						{
							ForeignIntermediateTableRelation fitr = new ForeignIntermediateTableRelation(rti0, tn.Text);
							relNode0 = new RelationNode(fitr, relTn0);
							relTn0.Nodes.Add(relNode0);
						}
					}
					if (loopTn.Text == rti1.Table)
					{
						relTn1 = loopTn;
						if (rti1.RelationDirection != RelationDirection.DirectedToMe)
						{
							ForeignIntermediateTableRelation fitr = new ForeignIntermediateTableRelation(rti1, tn.Text);
							relNode1 = new RelationNode(fitr, relTn1);
							relTn1.Nodes.Add(relNode1);
						}
					}
				}
				if (relNode0 != null)
					relNode0.RelatedTableNode = relTn1;
				if (relNode1 != null)
					relNode1.RelatedTableNode = relTn0;
			}
		}

		public bool MapIntermediateClass(TableNode tn)
		{
//			IntermediateClass ic = new IntermediateClass();
//			IntermediateClassNode icn = new IntermediateClassNode(ic, tn);

			IList tableNodes = new ArrayList();

			foreach(TableNode tnode in databaseNode.TableNodes)
			{
				if (tn.Text == tnode.Text)
					continue;
				if (tnode.Table.MappingType == TableMappingType.MappedAsClass)
					tableNodes.Add(tnode);
			}

			IntermediateClassWizardModel model = new IntermediateClassWizardModel(tn, tableNodes);

			IWizardController controller = ApplicationController.wizardControllerFactory.Create
				("IntermediateClassWizController", "IntClassWiz", "Intermediate Class Wizard");
			//controller.FrameSize = new Size(544, 500);
			model[0].RelationDirection = RelationDirection.Bidirectional;
			model[1].RelationDirection = RelationDirection.Bidirectional;
			DialogResult r = controller.Run(model);
			if (r == DialogResult.OK)
			{
				DatabaseNode parent = (DatabaseNode) tn.Parent;
				// Nothing to remove, because we use the original table node
				//				tn.Remove();
				//				tn.Parent.Nodes.Add(icn);
				for(int i = 0; i < 2; i++)
				{
					ColumnNode columnNode = (ColumnNode) tn.FindNode(model[i].ForeignKeyColumnName, typeof(ColumnNode));
					FkRelation fkr = new FkRelation(columnNode.Text);
					IntermediateClassInfo intermClInfo = model[i];
					fkr.FieldName = intermClInfo.OwnFieldName;
					fkr.ForeignCodingStyle = intermClInfo.CodingStyle;
					fkr.ForeignFieldName = intermClInfo.ForeignFieldName;
					fkr.ForeignIsComposite = false;
					fkr.IsComposite = false;
					fkr.OwningTable = tn.Text;
					fkr.OwningType = tn.Table.ClassName;
					fkr.RelatedTable = intermClInfo.Table;
					fkr.RelatedType = intermClInfo.Type;
					fkr.RelationDirection = intermClInfo.RelationDirection;
					fkr.RelationName = string.Empty;
					//ForeignFkRelation ffkr = fkr.ForeignRelation;
					RelationNode relationNode = new RelationNode(fkr, tn);
					relationNode.RelatedTableNode = (TableNode) databaseNode.FindNode(intermClInfo.Table);
					ForeignKeyWizModel fkwizModel = new ForeignKeyWizModel(relationNode, new ArrayList());

					tn.DualKeyRelations[i] = intermClInfo.OwnFieldName;

					MakeForeignKeyRelation(relationNode, columnNode, fkwizModel);
				}
				return true;
			}
			return false;
		}

		void RemoveRelationNodeFromTable(string tableName, string nodeName)
		{
			TableNode tn = (TableNode) databaseNode.FindNode(tableName);

			if (tn != null)
			{
				RelationNode relNode = (RelationNode) tn.FindNode(nodeName, typeof(RelationNode));
				if (relNode != null)
					relNode.Remove();
					//tn.Nodes. Remove(relNode);
			}
		}

		public void UnmapIntermediateTable(IntermediateTableNode itn)
		{
			TableNode tn = itn.OriginalTableNode;
			int index = databaseNode.Nodes.IndexOf(itn);
			tn.Remove(); // Was inserted as node in itn.
			itn.Remove();
			databaseNode.Nodes.Insert(index, tn);
			tn.TreeView.SelectedNode = tn;
			for (int tableNr = 0; tableNr < 2; tableNr++)
			{
				RelatedTableInfo rti = itn.IntermediateTable[tableNr];
				RemoveRelationNodeFromTable(rti.Table, rti.ForeignKeyColumnName);
			}
		}

		public void UnmapIntermediateClass(TableNode tn)
		{
			foreach(NDOTreeNode trn in tn.Nodes)
			{
				RelationNode rn = trn as RelationNode;
				if (rn == null)
					continue;
				rn.Remove();
				FkRelation fkRelation = rn.Relation as FkRelation;
				if (fkRelation != null)
				{
					if (fkRelation.RelationDirection != RelationDirection.DirectedFromMe)
					{
						TableNode relTn = databaseNode.FindTableNode(fkRelation.RelatedTable, true);
						RelationNode nodeToRemove = relTn.FindRelationNode(rn.Text, tn.Text); 
						if (nodeToRemove != null)
							nodeToRemove.Remove();
					}
				}
			}
		}

        public void AddPrimaryKeyColum(TableNode tn)
        {
            //try
            //{
                AddPrimaryKeyWiz wiz = new AddPrimaryKeyWiz();
                if (wiz.ShowDialog() != DialogResult.OK)
                    return;
                foreach (ColumnNode cn in tn.ColumnNodes)
                {
                    if (cn.IsPrimary)
                        cn.ChangePrimary(null, EventArgs.Empty);
                }
                ISqlGenerator generator = (ISqlGenerator)NDOProviderFactory.Instance.Generators[this.databaseNode.Database.ConnectionType];
                IProvider provider = NDOProviderFactory.Instance[generator.ProviderName];
                generator.Provider = provider;
                string columnString = CreateColumn(generator, provider, wiz.ColumnName, wiz.ColumnType, wiz.IsAutoNumbered);
                string alter = "ALTER TABLE " + generator.AddColumn() + " " + columnString;
                IDbConnection conn = provider.NewConnection(databaseNode.Database.ConnectionString);
                IDbCommand cmd = provider.NewSqlCommand(conn);
                cmd.CommandText = alter;
                MessageBox.Show(alter);
                //cmd.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error");
            //}
            //            sb.Append(generator.NullExpression(false));

        }


        protected string CreateColumn(ISqlGenerator generator, IProvider provider, string columnName, Type type, bool autoIncrement)
        {
            string name = provider.GetQuotedName(columnName);
            string columnType = null;
            string width = null;
            StringBuilder sb = new StringBuilder();

            columnType = generator.DbTypeFromType(type);

            int dl = provider.GetDefaultLength(type);
            if (dl != 0)
                width = dl.ToString();

            if (!generator.LengthAllowed(type))
                width = null;

            if (autoIncrement && generator.HasSpecialAutoIncrementColumnFormat)
                sb.Append(generator.AutoIncrementColumn(name, type, columnType, width));
            else if (generator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InColumn)
                sb.Append(generator.PrimaryKeyColumn(name, type, columnType, width));
            else if (width != null)
                sb.Append(name + " " + columnType + "(" + width + ")");
            else
                sb.Append(name + " " + columnType);

            sb.Append(" ");

            return sb.ToString();
        }



        public AssemblyWizModel ReCreateModel()
        {
            AssemblyWizModel model = new AssemblyWizModel();
            Database db = this.databaseNode.Database;
            model.ConnectionString = db.ConnectionString;
            model.ConnectionType = db.ConnectionType;
            model.IsXmlSchema = db.IsXmlSchema;
            model.OwnerName = db.OwnerName;
            model.XmlSchemaFile = db.XmlSchemaFile;

            Assembly ass = this.assemblyNode.Assembly;
            model.DefaultNamespace = ass.RootNamespace;
            model.ProjectDirectory = ass.TargetDir;
            model.ProjectName = ass.ProjectName;
            model.TargetLanguage = ass.TargetLanguage;
            model.UseClassField = ass.UseClassField;
            return model;
        }

		private ApplicationController()
		{
		}
	}
}
