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
using System.Linq;
using System.Collections;
using NUnit.Framework;
using NDO;
using PureBusinessClasses;
using NDO.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NdoUnitTests
{
	[TestFixture]
	public class IntermediateClassTest : NDOTest
	{
		Order order;
		Product pr;
		
		[SetUp]
		public void Setup()
		{
		}

		void CreateProductAndOrder(PersistenceManager pm)
		{
			order = new Order();
			order.Date = DateTime.Now;
			pm.MakePersistent( order );

			pr = new Product();
			pr.Name = "Shirt";
			pm.MakePersistent( pr );
			pm.Save();
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.Delete(pm.GetClassExtent(typeof(Order)));
			pm.Save();
            pm.Delete(pm.GetClassExtent(typeof(Product)));
			pm.Save();
		}

		private void CreateOrderDetail()
		{
			OrderDetail od = order.NewOrderDetail(pr);
			od.Price =49m;
			Assert.That(od.NDOObjectId.Id is DependentKey, "Wrong type");
			DependentKey dk = (DependentKey) od.NDOObjectId.Id;
			Assert.That(dk.IsValid, "Dependent key not valid");
		}

		[Test]
		public void TestCreateSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateSave( pm );
		}

		public void CreateSave(PersistenceManager pm)
		{
			CreateProductAndOrder( pm );
			CreateOrderDetail();
			pm.Save();
			pm.UnloadCache();
			IList orders = pm.GetClassExtent(typeof(Order));
			Assert.That(1 ==  orders.Count );
			Order o = (Order) orders[0];
			Assert.That(1 ==  o.OrderDetails.Count() );
			OrderDetail od = (OrderDetail) o.OrderDetails.First();
			Assert.That(od.Product != null, "Product shouldn't be null");
		}

		[Test]
		public void WrongSetup()
		{
			var pm = PmFactory.NewPersistenceManager();
			order = new Order();
			order.Date = DateTime.Now.Date;
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
			var pm = PmFactory.NewPersistenceManager();
			CreateProductAndOrder( pm );
			CreateOrderDetail();
			OrderDetail od = (OrderDetail)order.OrderDetails.First();
			Assert.That(NDOObjectState.Created ==  od.NDOObjectState );
			order.RemoveOrderDetail( od );
			Assert.That(NDOObjectState.Transient ==  od.NDOObjectState );
		}

		[Test]
		public void RemoveRelatedObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateProductAndOrder( pm );
			CreateOrderDetail();
			OrderDetail od = (OrderDetail)order.OrderDetails.First();
			Assert.That(od.NDOObjectState == NDOObjectState.Created, "Wrong state #1");
			pm.Save();
			Assert.That(od.NDOObjectState == NDOObjectState.Persistent, "Wrong state #2");
			order.RemoveOrderDetail(od);
			Assert.That(od.NDOObjectState == NDOObjectState.Deleted, "Wrong state #3");
		}

		[Test]
		public void RemoveRelatedObjectSaveReload()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateProductAndOrder( pm );
			CreateOrderDetail();
			OrderDetail od = (OrderDetail)order.OrderDetails.First();
			Assert.That( od.NDOObjectState == NDOObjectState.Created, "Wrong state #1" );
			pm.Save();
			Assert.That( od.NDOObjectState == NDOObjectState.Persistent, "Wrong state #2" );
			order.RemoveOrderDetail( od );
			Assert.That( od.NDOObjectState == NDOObjectState.Deleted, "Wrong state #3" );
			pm.Save();
			pm.UnloadCache();
			order = pm.Objects<Order>().Single();
			Assert.That(0 ==  order.OrderDetails.Count() );
			Assert.That(0 ==  pm.Objects<OrderDetail>().Count );

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
			var pm = PmFactory.NewPersistenceManager();
			CreateSave(pm);
			Product p2 = new Product();
			p2.Name = "Trouser";
			pm.MakePersistent(p2);
			OrderDetail od = order.NewOrderDetail(p2);
			Assert.That(NDOObjectState.Created ==  od.NDOObjectState, "Wrong state");
			od.Price = 55;
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			Assert.That(2 ==  order.OrderDetails.Count(), "Wrong count");
		}


		[Test]
		public void Reload()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateSave(pm);
			OrderDetail od = (OrderDetail) order.OrderDetails.First();
			pm.UnloadCache();
			od = (OrderDetail) pm.FindObject(od.NDOObjectId);
			decimal d = od.Price;
		}

		[Test]
		public void Delete()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateSave( pm );
			pm.UnloadCache();
			var q = new NDOQuery<Order>(pm);
			var order = q.ExecuteSingle(true);
			order.RemoveOrderDetail( order.OrderDetails.First() );
			pm.Save();
			pm.UnloadCache();
			var count = pm.Objects<OrderDetail>().Count;
			Assert.That(0 ==  count );

			order = pm.Objects<Order>().Single(); // Throws, if count != 1
			Assert.That(0 ==  order.OrderDetails.Count() );
		}

		[Test]
		public void DeleteParent()
		{
			var pm = PmFactory.NewPersistenceManager();
			CreateSave(pm);
			pm.UnloadCache();
			IQuery q = new NDOQuery<Order>(pm);
			order = (Order) q.ExecuteSingle(true);
			pm.Delete(order);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(OrderDetail));
			Assert.That(0 ==  l.Count, "Wrong count #1");
			l = pm.GetClassExtent(typeof(Order));
			Assert.That(0 ==  l.Count, "Wrong count #2");
		}
	}
}
