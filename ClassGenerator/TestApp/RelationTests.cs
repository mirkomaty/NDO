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
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using System.Windows.Forms;
using ClassGenerator;
using ClassGenerator.AssemblyWizard;
using ClassGenerator.IntermediateTableWizard;
using ClassGenerator.IntermediateClassWizard;
using WizardBase;

namespace TestApp
{
	[TestFixture]
	public class RelationTests
	{
		TreeView allObjects = new TreeView();
		AssemblyWizModel model;

		[SetUp]
		public void Setup()
		{
			allObjects.Nodes.Clear();
			model = new AssemblyWizModel();
			model.ConnectionType = "SqlServer";
			model.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind;Data Source=NOTEBOOKZERO";
			model.ProjectDirectory = @"c:\MyNw";
			if (!Directory.Exists(model.ProjectDirectory))
			{
				Directory.CreateDirectory(model.ProjectDirectory);
			}
			else
			{
				foreach(string s in Directory.GetFiles(model.ProjectDirectory))
				{
					if (!s.EndsWith(".sln"))
						File.Delete(s);
				}
			}
			model.ProjectName = "Northwind";
			model.DefaultNamespace = "Northwind.Reverse";
			FillNodes();
		}

		void FillNodes()
		{
			ApplicationController.Instance.DatabaseNode = new DatabaseNode(model);
			allObjects.Nodes.Add(ApplicationController.Instance.DatabaseNode);
			ApplicationController.Instance.AssemblyNode = new AssemblyNode(model);
			allObjects.Nodes.Add(ApplicationController.Instance.AssemblyNode);
		}


		[TearDown]
		public void TearDown()
		{
		}

		[DisplayName("ForeignKeyWizController")]
#if DEBUG
		public class FkWizardController : GenericController
#else
		internal class FkWizardController : GenericController
#endif
		{
			public FkWizardController(string viewTypeName, System.Reflection.Assembly assy, string title) : base (viewTypeName, assy, title)
			{
			}
			public override DialogResult Run(IModel model)
			{
				ClassGenerator.ForeignKeyWizard.ForeignKeyWizModel fkWizModel = (ClassGenerator.ForeignKeyWizard.ForeignKeyWizModel) model;
				TableNode relTableNode = null;
				foreach(TableNode tn in fkWizModel.TableNodes)
					if (tn.Text == "Employees")
						relTableNode = tn;

				fkWizModel.RelationNode.RelatedTableNode = relTableNode;
				FkRelation fkRelation = (FkRelation) fkWizModel.RelationNode.Relation;
				fkRelation.RelationDirection = RelationDirection.Bidirectional;
				fkRelation.FieldName = "employee";
				fkRelation.ForeignCodingStyle = CodingStyle.IList;
				fkRelation.ForeignFieldName = "orders";
				return DialogResult.OK;
			}
		}

		[Test]
		public void CreateCodeEmployeeOrder()
		{
			TableNode tn;
			ColumnNode cn;
			PrepareEmployeeOrder(out tn, out cn);

			ApplicationController.Instance.GenerateAssembly();
		}


		private void PrepareEmployeeOrder(out TableNode tn, out ColumnNode cn)
		{
			ApplicationController.WizardControllerFactory = new WizardControllerFactory(this.GetType().Assembly, "TestApp.RelationTests+FkWizardController");
			tn = (TableNode) FindNode(ApplicationController.Instance.DatabaseNode.Nodes, "Employees");
			tn.MapClass(null, EventArgs.Empty);
			tn = (TableNode) FindNode(ApplicationController.Instance.DatabaseNode.Nodes, "Orders");
			tn.MapClass(null, EventArgs.Empty);
			cn = (ColumnNode) FindNode(tn.Nodes, "EmployeeID");
			cn.ChangeMapping(null, EventArgs.Empty);
			cn.MakeForeignKey(null, EventArgs.Empty);
		}

		[Test]
		public void TestRemoveBidirectionalRelation()
		{
			TableNode tn;
			ColumnNode cn;
			// Returns the Orders table node
			PrepareEmployeeOrder(out tn, out cn);

			Assertion.Assert(!cn.Column.IsMapped);
			Assertion.Assert(!cn.Column.IsMapped);

			NDOTreeNode treeNode = (NDOTreeNode) FindNode(tn.Nodes, "EmployeeID");
			Assertion.AssertNotNull(treeNode);
			Assertion.Assert("Wrong type #1", treeNode is RelationNode);
			RelationNode rn = (RelationNode) treeNode;
			TableNode relTn = rn.RelatedTableNode;
			// tn is Orders, relTn is Employees
			treeNode = (NDOTreeNode) FindNode(relTn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNotNull(treeNode);
			RelationNode rn2 = (RelationNode) treeNode;
			Assertion.Assert(rn2.Relation is ForeignFkRelation);
			rn.DeleteFkRelation(null, EventArgs.Empty);

			treeNode = (NDOTreeNode) FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
			treeNode = (NDOTreeNode) FindNode(relTn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
		}

		[Test]
		public void UnmapClassDeleteRelations1()
		{
			TableNode tn;
			ColumnNode cn;
			PrepareEmployeeOrder(out tn, out cn);
			//tn is the Orders table
			tn.UnmapClass(null, EventArgs.Empty);
			TreeNode treeNode = FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
			tn = (TableNode) FindNode(ApplicationController.Instance.DatabaseNode.Nodes, "Employees");
			treeNode = FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
		}

		[Test]
		public void UnmapClassDeleteRelations2()
		{
			TableNode tn;
			ColumnNode cn;
			PrepareEmployeeOrder(out tn, out cn);
			//tn is the Orders table, find Employees
			tn = (TableNode) FindNode(ApplicationController.Instance.DatabaseNode.Nodes, "Employees");
			tn.UnmapClass(null, EventArgs.Empty);
			TreeNode treeNode = FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
			tn = (TableNode) FindNode(ApplicationController.Instance.DatabaseNode.Nodes, "Orders");
			treeNode = FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull(treeNode);
		}



		
		public class IntermediateTableWizController : GenericController
		{
			public IntermediateTableWizController(string viewTypeName, System.Reflection.Assembly assy, string title) : base (viewTypeName, assy, title)
			{
			}
			public override DialogResult Run(IModel model)
			{
				IntermediateTableWizardModel intTWizModel = (IntermediateTableWizardModel) model;
				TableNode empTableNode = null;
				TableNode terrTableNode = null;
				foreach(TableNode tn in intTWizModel.TableNodes)
				{
					if (tn.Text == "Employees")
						empTableNode = tn;
					if (tn.Text == "Territories")
						terrTableNode = tn;
				}
				Assertion.AssertNotNull(empTableNode);
				Assertion.AssertNotNull(terrTableNode);
				IntermediateTable intTable = intTWizModel.IntermediateTableNode.IntermediateTable;
				intTable[0].CodingStyle = CodingStyle.IList;
				intTable[0].RelationDirection = RelationDirection.Bidirectional;
//				intTable[0].RelationDirection = RelationDirection.DirectedToMe;
				intTable[0].IsComposite = false;
				intTable[0].IsElement = false;
				intTable[0].RelationName = "Hi";
				intTable[0].Table = empTableNode.Table.Name;
				intTable[0].Type = empTableNode.Table.ClassName;
				intTable[0].FieldName = "territories";
				intTable[0].ForeignKeyColumnName = "EmployeeID";
				intTable[0].ChildForeignKeyColumnName = "TerritoryID";

				intTable[1].CodingStyle = CodingStyle.IList;
				intTable[1].RelationDirection = RelationDirection.Bidirectional;
//				intTable[1].RelationDirection = RelationDirection.DirectedFromMe;
				intTable[1].IsComposite = false;
				intTable[1].IsElement = false;
				intTable[1].RelationName = "Hi";
				intTable[1].Table = terrTableNode.Table.Name;
				intTable[1].Type = terrTableNode.Table.ClassName;
				intTable[1].FieldName = "employees";
				intTable[1].ForeignKeyColumnName = "TerritoryID";
				intTable[1].ChildForeignKeyColumnName = "EmployeeID";
				return DialogResult.OK;				
			}
		}

		[Test]
		public void CreateCodeEmployeeTerritories()
		{
			TableNode tn;
			PrepareEmployeeTerritories(out tn);
			ApplicationController.Instance.GenerateAssembly();
		}

		[Test]
		public void CreateCodeEmployeeTerritoriesAfterUnmap()
		{
			TableNode tn;
			PrepareEmployeeTerritories(out tn);
			DatabaseNode dbn = (DatabaseNode) tn.Parent;
			IntermediateTableNode itn = (IntermediateTableNode) FindNode(dbn.Nodes, "EmployeeTerritories", typeof(IntermediateTableNode));
			itn.UnmapIntermediateTable(null, EventArgs.Empty);
			ApplicationController.Instance.GenerateAssembly();
			// There shouldn't be relations in the code
		}


		void PrepareEmployeeTerritories(out TableNode tn)
		{
			ApplicationController.WizardControllerFactory = new WizardControllerFactory(this.GetType().Assembly, "TestApp.RelationTests+IntermediateTableWizController");
			// Find Territories, map primary key, since it's not automatically detected,
			// and map class
			TreeNodeCollection nodes = ApplicationController.Instance.DatabaseNode.Nodes;

			tn = (TableNode) FindNode(nodes, "Territories");
			ColumnNode cn = (ColumnNode) FindNode(tn.Nodes, "TerritoryID");
			cn.ChangePrimary(null, EventArgs.Empty);
			tn.MapClass(null, EventArgs.Empty);
			tn.Table.ClassName = "Territory";

			// Find Employees and map class
			tn = (TableNode) FindNode(nodes, "Employees");
			tn.MapClass(null, EventArgs.Empty);
			tn.Table.ClassName = "Employee";

			tn = (TableNode) FindNode(nodes, "EmployeeTerritories");
			DatabaseNode dbn = (DatabaseNode) tn.Parent;
			Assertion.AssertNotNull("dbn shouldn't be null", dbn);
			tn.MapIntermediateTable(null, EventArgs.Empty);
		}

		[Test]
		public void TestIntermediateTable()
		{
			TableNode tn;
			PrepareEmployeeTerritories(out tn);

			DatabaseNode dbn = (DatabaseNode) tn.Parent;

			IntermediateTableNode itn = (IntermediateTableNode) FindNode(dbn.Nodes, "EmployeeTerritories", typeof(IntermediateTableNode));
			Assertion.AssertNotNull("IntermediateTableNode not found", itn);
			Assertion.AssertEquals("Wrong original table", tn, itn.OriginalTableNode);
			Assertion.AssertEquals("Wrong image", itn.SelectedImageIndex, 13);

			tn = (TableNode) FindNode(dbn.Nodes, "Employees");
			RelationNode rn = (RelationNode) FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNotNull("Relation node should exist #1", rn);

			tn = (TableNode) FindNode(dbn.Nodes, "Territories");
			rn = (RelationNode) FindNode(tn.Nodes, "TerritoryID", typeof(RelationNode));
			Assertion.AssertNotNull("Relation node should exist #2", rn);

			// Now unmap and test
			itn.UnmapIntermediateTable(null, EventArgs.Empty);

			itn = (IntermediateTableNode) FindNode(dbn.Nodes, "EmployeeTerritories", typeof(IntermediateTableNode));
			Assertion.AssertNull("itn should be null", itn);
			tn = (TableNode) FindNode(dbn.Nodes, "EmployeeTerritories", typeof(TableNode));
			Assertion.AssertNotNull("Table node should be back", tn);

			tn = (TableNode) FindNode(dbn.Nodes, "Employees");
			rn = (RelationNode) FindNode(tn.Nodes, "EmployeeID", typeof(RelationNode));
			Assertion.AssertNull("Relation node shouldn't be there", rn);

			tn = (TableNode) FindNode(dbn.Nodes, "Territories");
			rn = (RelationNode) FindNode(tn.Nodes, "TerritoryID", typeof(RelationNode));
			Assertion.AssertNull("Relation node shouldn't be there #2", rn);
		}


		public class IntermediateClassWizController : GenericController
		{
			public IntermediateClassWizController(string viewTypeName, System.Reflection.Assembly assy, string title) : base (viewTypeName, assy, title)
			{
			}
			public override DialogResult Run(IModel model)
			{
				IntermediateClassWizardModel intCWizModel = (IntermediateClassWizardModel) model;
				
				// intCWizModel.TableNode is set by the ApplicationController

				// 0 is the relation to Orders
				// 1 is the relation to Product
				intCWizModel[0].CodingStyle = CodingStyle.IList;
				intCWizModel[0].OwnFieldName = "order";
				intCWizModel[0].ForeignFieldName = "orderDetail";
				intCWizModel[0].ForeignKeyColumnName = "OrderID";
				intCWizModel[0].RelationDirection = RelationDirection.Bidirectional;
				intCWizModel[0].Table = "Orders";
				intCWizModel[0].Type = "Order";
				
				intCWizModel[1].CodingStyle = CodingStyle.IList;
				intCWizModel[1].OwnFieldName = "product";
				intCWizModel[1].ForeignFieldName = "orderDetail";
				intCWizModel[1].ForeignKeyColumnName = "ProductID";
				intCWizModel[1].RelationDirection = RelationDirection.DirectedFromMe;
				intCWizModel[1].Table = "Products";
				intCWizModel[1].Type = "Product";

				return DialogResult.OK;				
			}
		}

		void PrepareOrderDetails(out TableNode tn)
		{
			ApplicationController.WizardControllerFactory = new WizardControllerFactory(this.GetType().Assembly, "TestApp.RelationTests+IntermediateClassWizController");
			TreeNodeCollection nodes = ApplicationController.Instance.DatabaseNode.Nodes;
			tn = (TableNode) FindNode(nodes, "Orders");
			tn.MapClass(null, EventArgs.Empty);
			tn.Table.ClassName = "Order";
			tn = (TableNode) FindNode(nodes, "Products");
			tn.MapClass(null, EventArgs.Empty);
			tn.Table.ClassName = "Product";
			tn = (TableNode) FindNode(nodes, "Order Details");
			DatabaseNode dbn = (DatabaseNode) tn.Parent;
			Assertion.AssertNotNull("dbn shouldn't be null", dbn);

			tn.MapIntermediateClass(null, EventArgs.Empty);
			tn.Table.ClassName = "OrderDetail";
		}


		[Test]
		public void TestIntermediateClassRemove2()
		{
			TableNode tn;
			PrepareOrderDetails(out tn);

			TreeNodeCollection nodes = ApplicationController.Instance.DatabaseNode.Nodes;

			TableNode tnOrders = (TableNode) FindNode(nodes, "Orders");
			TableNode tnProducts = (TableNode) FindNode(nodes, "Products");
			tnProducts.UnmapClass(null, EventArgs.Empty);
			
			Assertion.Assert("Mapping type wrong #1", tn.Table.MappingType == TableMappingType.NotMapped);

			RelationNode rn = tn.FindRelationNode("OrderID", "Orders");
			Assertion.AssertNull("Relation to Orders should be removed", rn);
			rn = tn.FindRelationNode("ProductID", "Products");
			Assertion.AssertNull("Relation to Products should be removed", rn);

			rn = tnOrders.FindRelationNode("OrderID", "Order Details");
			Assertion.AssertNull("Relation should be removed", rn);
//			ApplicationController.Instance.GenerateAssembly();
		}


		[Test]
		public void TestIntermediateClassRemove()
		{
			TableNode tn;
			PrepareOrderDetails(out tn);

			TreeNodeCollection nodes = ApplicationController.Instance.DatabaseNode.Nodes;

			TableNode tnOrders = (TableNode) FindNode(nodes, "Orders");
			tnOrders.UnmapClass(null, EventArgs.Empty);
			TableNode tnProducts = (TableNode) FindNode(nodes, "Products");

			Assertion.Assert("Mapping type wrong #1", tn.Table.MappingType == TableMappingType.NotMapped);

			RelationNode rn = tn.FindRelationNode("OrderID", "Orders");
			Assertion.AssertNull("Relation to Orders should be removed", rn);
			rn = tn.FindRelationNode("ProductID", "Products");
			Assertion.AssertNull("Relation to Products should be removed", rn);

			rn = tnOrders.FindRelationNode("OrderID", "Order Details");
			Assertion.AssertNull("Relation should be removed", rn);
		}

		[Test]
		public void TestIntermediateClass()
		{
			TableNode tn;
			PrepareOrderDetails(out tn);
			Assertion.Assert("Mapping type wrong #1", tn.Table.MappingType == TableMappingType.MappedAsIntermediateClass);
			TreeNodeCollection nodes = ApplicationController.Instance.DatabaseNode.Nodes;
			object o = FindNode(nodes, tn.Text);
			Assertion.AssertNotNull("Intermediate Class '" + tn.Text + "' not found", o);
			Assertion.AssertSame("Node should be the same table node", tn, o);
			RelationNode rn = tn.FindRelationNode("OrderID", "Orders");
			Assertion.AssertNotNull("Relation to Orders should be there", rn);
			Assertion.Assert("Wrong relation type", rn.Relation is FkRelation);
			rn = tn.FindRelationNode("ProductID", "Products");
			Assertion.AssertNotNull("Relation to Products should be there", rn);
			Assertion.Assert("Wrong relation type", rn.Relation is FkRelation);

			TableNode tnOrders = (TableNode) FindNode(nodes, "Orders");
			rn = tnOrders.FindRelationNode("OrderID", "Order Details");
			Assertion.AssertNotNull("OrderID relation not found", rn);
			TableNode tnProducts = (TableNode) FindNode(nodes, "Products");
//			foreach(NDOTreeNode trn in tnProducts.Nodes)
//			{
//				RelationNode rn2 = trn as RelationNode;
//				if (rn2 != null)
//				{
//					Debug.WriteLine(rn2.Text + " " + rn2.Relation.GetType().Name);
//					ForeignFkRelation fkr = rn2.Relation as ForeignFkRelation;
//					if (fkr != null)
//						Debug.WriteLine(fkr.RelatedTable);
//				}
//			}
			rn = tnProducts.FindRelationNode("ProductID", "Order Details");
			Assertion.AssertNull("Relation is directed", rn);
			tn.UnmapIntermediateClass(null, EventArgs.Empty);

			Assertion.Assert("Mapping type wrong #1", tn.Table.MappingType == TableMappingType.NotMapped);

			rn = tn.FindRelationNode("OrderID", "Orders");
			Assertion.AssertNull("Relation to Orders should be removed", rn);
			rn = tn.FindRelationNode("ProductID", "Products");
			Assertion.AssertNull("Relation to Products should be removed", rn);

			rn = tnOrders.FindRelationNode("OrderID", "Order Details");
			Assertion.AssertNull("Relation should be removed", rn);
		}

		[Test]
		public void CreateCodeOrderDetails()
		{
			TableNode tn;
			PrepareOrderDetails(out tn);
			ApplicationController.Instance.GenerateAssembly();
		}




		TreeNode FindNode(TreeNodeCollection container, string name)
		{
			foreach(TreeNode tn in container)
				if (tn.Text == name)
					return tn;
			return null;
		}

		TreeNode FindNode(TreeNodeCollection container, string name, Type targetType)
		{
			foreach(TreeNode tn in container)
				if (tn.Text == name && targetType.IsAssignableFrom(tn.GetType()))
					return tn;
			return null;
		}

	}
}
