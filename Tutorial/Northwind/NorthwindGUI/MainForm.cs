using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Northwind;
using NDO;
using NDO.Linq;

namespace NorthwindGUI
{
    public partial class MainForm : Form
    {
        PersistenceManager pm;
        DateTime lastUIUpdate = DateTime.Now;

        List<Employee> employeeList = null;
        List<Customer> customerList = null;
        List<Order> orderList = null;
        List<OrderDetail> orderDetailList = null;
        List<Product> productList = null;

        public MainForm()
        {
            this.pm = new PersistenceManager();
            InitializeComponent();
            this.customerBindingSource.PositionChanged += new EventHandler(CustomerPositionChanged);
            this.employeeBindingSource.PositionChanged += new EventHandler(EmployeePositionChanged);
            this.orderBindingSource.PositionChanged += new EventHandler(OrderPositionChanged);
            this.customerBindingSource.DataSource = customerList;
            this.employeeBindingSource.DataSource = employeeList;
            this.orderBindingSource.DataSource = orderList;
            this.orderDetailBindingSource.DataSource = orderDetailList;
            this.productBindingSource.DataSource = productList;
            Application.Idle += new EventHandler(OnIdle);
        }


        #region Grid View Synchronizing
        void CustomerPositionChanged(object sender, EventArgs e)
        {
            
            NDOQuery<Order> q = new NDOQuery<Order>(pm, qh.customer.oid + Query.Op.Eq + Query.Placeholder(0));
            IPersistenceCapable pc = SelectedCustomer;
            if (pc != null)
            {
				this.orderList = from o in this.pm.Objects<Order>() where o.Oid<object>() == pc.NDOObjectId.Id.Value select o;
                q.Parameters.Add(pc.NDOObjectId.Id.Value);
                this.orderList = q.Execute();
                orderBindingSource.DataSource = this.orderList;
            }
        }

        void EmployeePositionChanged(object sender, EventArgs e)
        {
            Order.QueryHelper qh = new Order.QueryHelper();
            NDOQuery<Order> q = new NDOQuery<Order>(pm, qh.employee.oid + Query.Op.Eq + Query.Placeholder(0));
            IPersistenceCapable pc = SelectedEmployee;
            if (pc != null)
            {
                q.Parameters.Add(pc.NDOObjectId.Id.Value);
                orderBindingSource.DataSource = this.orderList = q.Execute();
            }
        }

        void OrderPositionChanged(object sender, EventArgs e)
        {
            NDOQuery<OrderDetail> q = new NDOQuery<OrderDetail>(pm, "order.oid" + Query.Op.Eq + Query.Placeholder(0));
            IPersistenceCapable pc = SelectedOrder;
            if (pc != null)
            {
                q.Parameters.Add(SelectedOrder.NDOObjectId.Id.Value);
                orderDetailBindingSource.DataSource = this.orderDetailList = q.Execute();
            }
        }
        #endregion

        #region UI Update
        void OnIdle(object sender, EventArgs e)
        {
            if (DateTime.Now - lastUIUpdate < TimeSpan.FromMilliseconds(250.0))
                return;
            lastUIUpdate = DateTime.Now;
            string tab = this.tabControl1.SelectedTab.Text;
            this.btnAdd.Enabled =
                tab != "Products"
                && !(tab == "Order Details" && (SelectedProduct == null || SelectedOrder == null))
                && !(tab == "Orders" && (SelectedCustomer == null || SelectedEmployee == null));
            bool selObjEnabled =
                   (tab == "Customers" && SelectedCustomer != null)
                || (tab == "Employees" && SelectedEmployee != null)
                || (tab == "Orders" && SelectedOrder != null)
                || (tab == "Order Details" && SelectedOrderDetail != null)
                || (tab == "Products" && SelectedProduct != null);
            this.btnEdit.Enabled = selObjEnabled;
            this.btnDelete.Enabled = selObjEnabled;
        }
        #endregion

        #region Button handlers
        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (this.tabControl1.SelectedTab.Text)
            {
                case "Customers":
                    this.customerList = new ObjectSearcher<Customer>(pm).SearchObjects();
                    if (customerList != null)
                        this.customerBindingSource.DataSource = customerList;
                    break;
                case "Employees":
                    this.employeeList = new ObjectSearcher<Employee>(pm).SearchObjects();
                    if (employeeList != null)
                        this.employeeBindingSource.DataSource = employeeList;
                    break;
                case "Orders":
                    this.orderList = new ObjectSearcher<Order>(pm).SearchObjects();
                    if (orderList != null)
                        this.orderBindingSource.DataSource = orderList;
                    break;
                case "Order Details":
                    // is automatically searched, if the position of the order changes
                    break;
                case "Products":
                    this.productList = new ObjectSearcher<Product>(pm).SearchObjects();
                    if (productList != null)
                        this.productBindingSource.DataSource = productList;
                    break;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            object displayedObject = null;
            switch (this.tabControl1.SelectedTab.Text)
            {
                case "Customers":
                    displayedObject = SelectedCustomer;
                    break;
                case "Employees":
                    displayedObject = SelectedEmployee;
                    break;
                case "Orders":
                    displayedObject = SelectedOrder;
                    break;
                case "Order Details":
                    displayedObject = SelectedOrderDetail;
                    break;
                case "Products":
                    displayedObject = SelectedProduct;
                    break;

            }
            if (displayedObject != null)
            {
                // In a real world application it would make more sense to write specialized 
                // Forms for creating and editing the different objects. In this forms you can
                // do things like showing the customer fotos or querying for related objects 
                // as it is the case with the shipper of an order.
                ObjectPropertyForm opf = new ObjectPropertyForm(displayedObject, false);
                if (opf.ShowDialog() == DialogResult.OK)
                    pm.Save();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                object displayedObject = null;
                BindingSource list = null;
                bool isComposite = false;
                switch (this.tabControl1.SelectedTab.Text)
                {
                    case "Customers":
                        displayedObject = SelectedCustomer;
                        list = customerBindingSource;
                        break;
                    case "Employees":
                        displayedObject = SelectedEmployee;
                        list = employeeBindingSource;
                        break;
                    case "Orders":
                        pm.VerboseMode = true;
                        pm.LogAdapter = new NDO.Logging.DebugLogAdapter();
                        Order order = SelectedOrder;
                        // In a composite we have to remove an object from the parent's
                        // object list
                        order.Customer.RemoveOrder(order);
                        displayedObject = SelectedOrder;
                        list = orderBindingSource;
                        isComposite = true;
                        break;
                    case "Order Details":
                        displayedObject = SelectedOrderDetail;
                        list = orderDetailBindingSource;
                        break;
                    case "Products":
                        list = productBindingSource;
                        displayedObject = SelectedProduct;
                        break;
                }
                if (displayedObject != null)
                {
                    if (MessageBox.Show("Do you really want to delete this " + displayedObject.GetType().Name + " object?", "Object deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (!isComposite)
                            pm.Delete(displayedObject);
                        pm.Save();
                        list.Remove(displayedObject);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                pm.Abort();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                object displayedObject = null;
                BindingSource list = null;
                switch (this.tabControl1.SelectedTab.Text)
                {
                    case "Customers":
                        displayedObject = new Customer();
                        this.pm.MakePersistent(displayedObject);
                        list = customerBindingSource;
                        break;
                    case "Employees":
                        displayedObject = new Employee();
                        this.pm.MakePersistent(displayedObject);
                        list = employeeBindingSource;
                        break;
                    case "Orders":
                        Customer c = SelectedCustomer;
                        Employee emp = SelectedEmployee;
                        if (c != null && emp != null)
                        {
                            Order o = c.NewOrder();
                            o.Employee = emp;

                            o.OrderDate = DateTime.Now.Date;
                            o.RequiredDate = DateTime.Now.Date + TimeSpan.FromDays(14);
                            o.ShippedDate = DateTime.Now.Date;

                            o.ShipName = c.CompanyName;
                            o.ShipCity = c.City;
                            o.ShipCountry = c.Country;
                            o.ShipAddress = c.Address;
                            o.Freight = 374m;
                            o.Shipper = this.DefaultShipper;
                            o.ShipPostalCode = c.PostalCode;
                            o.ShipRegion = c.Region;
                            displayedObject = o;

                            list = orderBindingSource;
                        }
                        break;
                    case "Order Details":
                        OrderDetail od = new OrderDetail();
                        Product p = SelectedProduct;
                        od.Product = p;
                        od.Order = SelectedOrder;
                        od.Quantity = 1;
                        od.UnitPrice = p.UnitPrice;
                        displayedObject = od;
                        pm.MakePersistent(od);
                        list = orderDetailBindingSource;
                        break;
                    case "Products":
                        MessageBox.Show("Sorry, adding products is not implemented.");
                        break;
                }
                if (displayedObject != null)
                {
                    ObjectPropertyForm opf = new ObjectPropertyForm(displayedObject, false);
                    if (opf.ShowDialog() == DialogResult.OK)
                    {
                        pm.Save();
                        list.Add(displayedObject);
                    }
                    else
                    {
                        pm.Abort();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                pm.Abort();
            }
        }

        /// <summary>
        /// Because we have no UI to select shippers, this gets us a shipper object
        /// which can be used to create orders.
        /// </summary>
        Shipper DefaultShipper
        {
            get
            {
                try
                {
                    NDOQuery<Shipper> q = new NDOQuery<Shipper>(pm, "oid = {0}");
                    q.Parameters.Add(1);
                    return q.ExecuteSingle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                return null;
            }
        }

        #endregion

        #region Determining Selected Objects 
        Customer SelectedCustomer
        {
            get 
            {
                if (!(customerBindingSource.DataSource is List<Customer>))
                    return null;
                List<Customer> customers = (List<Customer>)customerBindingSource.DataSource;
                if (customers == null || customers.Count == 0)
                    return null;
                return (Customer)customers[customerBindingSource.Position];
            }
        }
        Employee SelectedEmployee
        {
            get
            {
                if (!(employeeBindingSource.DataSource is List<Employee>))
                    return null;
                List<Employee> employees = (List<Employee>)employeeBindingSource.DataSource;
                if (employees == null || employees.Count == 0)
                    return null;
                return employees[employeeBindingSource.Position];
            }
        }
        Order SelectedOrder
        {
            get
            {
                if (!(orderBindingSource.DataSource is List<Order>))
                    return null;
                List<Order> orders = (List<Order>)orderBindingSource.DataSource;
                if (orders == null || orders.Count == 0)
                    return null;
                return orders[orderBindingSource.Position];
            }
        }
        OrderDetail SelectedOrderDetail
        {
            get
            {
                if (!(orderDetailBindingSource.DataSource is List<OrderDetail>))
                    return null;
                List<OrderDetail> orderDetails = (List<OrderDetail>)orderDetailBindingSource.DataSource;
                if (orderDetails == null || orderDetails.Count == 0)
                    return null;
                return orderDetails[orderDetailBindingSource.Position];
            }
        }
        Product SelectedProduct
        {
            get
            {
                if (!(productBindingSource.DataSource is List<Product>))
                    return null;
                List<Product> products = (List<Product>)productBindingSource.DataSource;
                if (products == null || products.Count == 0)
                    return null;
                return products[productBindingSource.Position];
            }
        }

        #endregion


    }
}