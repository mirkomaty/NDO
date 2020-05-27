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
using System.Data.Common;
using System.Diagnostics;
using System.Collections;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using NDOInterfaces;
using PureBusinessClasses;
using NDO.Query;

namespace NdoUnitTests
{
	[TestFixture]
	public class IntermediateClassTest
	{
		PersistenceManager pm;

		Order order;
		Product pr;
		
		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			order = new Order();
			order.Date = DateTime.Now;
			pm.MakePersistent(order);

			pr = new Product();
			pr.Name = "Shirt";
			pm.MakePersistent(pr);
			pm.Save();
		}

		[TearDown]
		public void TearDown()
		{
			pm.Abort();
			pm.UnloadCache();
			pm.Delete(pm.GetClassExtent(typeof(Order)));
			pm.Save();
            pm.Delete(pm.GetClassExtent(typeof(Product)));
			pm.Save();
			Class cl = pm.NDOMapping.FindClass(typeof(OrderDetail));
			IProvider p = cl.Provider;
			string odname = cl.TableName;
            string[] arr = odname.Split('.');
            string tname = string.Empty;
            foreach (string name in arr)
                tname += p.GetQuotedName(name) + '.';
            tname = tname.Substring(0, tname.Length - 1);
            string sql = "Delete from " + tname;
            using (IDbConnection conn = p.NewConnection(pm.NDOMapping.FindConnection(cl).Name))
            {
                IDbCommand cmd = p.NewSqlCommand(conn);
                cmd.CommandText = sql;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
		}

		private void DoCreate()
		{
			OrderDetail od = order.NewOrderDetail(pr);
			od.Price =49m;
			Assert.That(od.NDOObjectId.Id is DependentKey, "Wrong type");
			DependentKey dk = (DependentKey) od.NDOObjectId.Id;
			Assert.That(dk.IsValid, "Dependent key not valid");
		}

		[Test]
		public void Create()
		{
//			Debug.WriteLine("Create");
			DoCreate();
		}

		[Test]
		public void CreateSave()
		{
//			Debug.WriteLine("CreateSave");
			DoCreate();
			pm.Save();
			pm.UnloadCache();
			IList orders = pm.GetClassExtent(typeof(Order));
			Assert.That(orders.Count == 1, "Wrong count");
			Order o = (Order) orders[0];
			Assert.That(o.OrderDetails.Count == 1, "Wrong count #2");
			OrderDetail od = (OrderDetail) o.OrderDetails[0];
			Assert.NotNull(od.Product, "Product shouldn't be null");
		}

		[Test]
		public void WrongSetup()
		{
			order = new Order();
			order.Date = DateTime.Now.Date;
			pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(order);
			int exnr = 0;
			try
			{
				order.NewOrderDetail(null);
			}
			catch (NDOException ex)
			{
				exnr = ex.ErrorNumber;
			}
			Assert.That(exnr == 41, "NDOException #41 should have been happened");
		}

		[Test]
		public void RemoveRelatedObject()
		{
			DoCreate();
			OrderDetail od = (OrderDetail)order.OrderDetails[0];
			Assert.That(od.NDOObjectState == NDOObjectState.Created, "Wrong state");
			order.RemoveOrderDetail(od);
			Assert.That(od.NDOObjectState == NDOObjectState.Transient, "Wrong state");
		}

		[Test]
		public void RemoveRelatedObjectSave()
		{
			DoCreate();
			OrderDetail od = (OrderDetail)order.OrderDetails[0];
			Assert.That(od.NDOObjectState == NDOObjectState.Created, "Wrong state");
			pm.Save();
			Assert.That(od.NDOObjectState == NDOObjectState.Persistent, "Wrong state");
			order.RemoveOrderDetail(od);
			Assert.That(od.NDOObjectState == NDOObjectState.Deleted, "Wrong state");
		}

		[Test]
		public void RemoveRelatedObjectSaveReload()
		{
			DoCreate();
			OrderDetail od = (OrderDetail)order.OrderDetails[0];
			Assert.That(od.NDOObjectState == NDOObjectState.Created, "Wrong state");
			pm.Save();
			Assert.That(od.NDOObjectState == NDOObjectState.Persistent, "Wrong state");
			order.RemoveOrderDetail(od);
			Assert.That(od.NDOObjectState == NDOObjectState.Deleted, "Wrong state");
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			Assert.That(order.OrderDetails.Count == 0, "Wrong count");
			IList l = pm.GetClassExtent(typeof(OrderDetail));
			Assert.That(order.OrderDetails.Count == 0, "Wrong count #2");

		}

        [Test]
        public void InsertWithTransientOrder()
        {
            Order o2 = new Order();
            o2.Date = DateTime.Now;
            OrderDetail od = o2.NewOrderDetail(this.pr);
            // Nothing should happen, since o2 is transient and is
            // not under the control of the pm.
        }


		[Test]
		public void InsertWithTransientProduct()
		{
			Product p2 = new Product();
			p2.Name = "Trouser";
			int exnr = 0;
			try
			{
				OrderDetail od = order.NewOrderDetail(p2);
			}
			catch (NDOException ex)
			{
				exnr = ex.ErrorNumber;
			}
			Assert.That(exnr == 59, "Exception 59 should have been occured");
		}


		[Test]
		public void Insert()
		{
			CreateSave();
			Product p2 = new Product();
			p2.Name = "Trouser";
			pm.MakePersistent(p2);
			OrderDetail od = order.NewOrderDetail(p2);
			Assert.AreEqual(NDOObjectState.Created, od.NDOObjectState, "Wrong state");
			od.Price = 55;
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			Assert.AreEqual(2, order.OrderDetails.Count, "Wrong count");
		}


		[Test]
		public void Reload()
		{
			CreateSave();
			OrderDetail od = (OrderDetail) order.OrderDetails[0];
			pm.UnloadCache();
			od = (OrderDetail) pm.FindObject(od.NDOObjectId);
			decimal d = od.Price;
		}

		[Test]
		public void Delete()
		{
			CreateSave();
			pm.UnloadCache();
			IQuery q = new NDOQuery<OrderDetail>(pm);
			OrderDetail od = (OrderDetail) q.ExecuteSingle(true);
			pm.Delete(od);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(OrderDetail));
			Assert.AreEqual(0, l.Count, "Wrong count #1");

			q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			Assert.AreEqual(0, order.OrderDetails.Count, "Wrong count #2");
		}

		[Test]
		public void DeleteParent()
		{
			CreateSave();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			pm.Delete(order);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(OrderDetail));
			Assert.AreEqual(0, l.Count, "Wrong count #1");
			l = pm.GetClassExtent(typeof(Order));
			Assert.AreEqual(0, l.Count, "Wrong count #2");
		}
	}
}
