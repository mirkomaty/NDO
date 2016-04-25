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
using System.Text;
using System.Windows.Forms;
using NDO;
using System.ComponentModel;


namespace NDO.DataBinding
{
    /// <summary>
    /// Helper class to persist Add and Delete operations on bound lists. Use this component instead of the 
    /// System.Windows.Forms.BindingSource component. This component is located in the NDO.DataBinding.dll.
    /// </summary>
    /// <remarks>
    /// The DataSource of the BindingSource should be a List&lt;T&gt;, where T is a persistent type.
    /// 
    /// Note: You can install this component in Visual Studio. Right Click into the Toolbox window and choose
    /// "Choose Elements...". In the Choose Toolbox Elements dialog click on "Browse". Browse for this dll and enter it.
    /// The NDOBindingSource will appear in the Toolbox and works just like an ordinary BindingSource.
    /// 
    /// Sample:
    /// <code>
    /// public class Form1 : System.Windows.Form
    /// {
    ///     PersistenceManager pm = new PersistenceManager();
    /// 
    ///     public Form1()
    ///     {
    ///         InitializeComponents();
    ///         this.employeeBindingSource.PersistenceManager = this.pm;
    ///         this.employeeBindingSource.DataSource = new NDOQuery&lt;Employee&gt;(pm).Execute();
    ///         this.bindingNavigator.DataSource = employeeBindingSource;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public class NDOBindingSource : BindingSource
    {
        PersistenceManager pm;
        /// <summary>
        /// Sets the PersistenceManager of the binding source.
        /// </summary>
        public PersistenceManager PersistenceManager
        {
            get { return pm; }
            set { pm = value; }
        }

        /// <summary>
        /// Initializes a new NDOBindingSource object.
        /// </summary>
        /// <param name="container">The container the BindingSource should be added to.</param>
        public NDOBindingSource(IContainer container) : base(container)
        {
        }

        /// <summary>
        /// Initializes a new NDOBindingSource object.
        /// </summary>
        public NDOBindingSource() : base()
        {
        }

        /// <summary>
        /// Initializes a new NDOBindingSource object.
        /// </summary>
        /// <param name="dataSource">The data source, the BindingSource should represent.</param>
        /// <param name="dataMember">The name of a column or specific list in the DataSource, the BindingSource should be bound to.</param>
        public NDOBindingSource(object dataSource, string dataMember) : base(dataSource, dataMember)
        { 
        }

        /// <summary>
        /// Adds a new object to the DataSource, the BindingSource is bound to. 
        /// The object should be a persistent object. If it is in the transient state it will be made persistent.
        /// </summary>
        /// <param name="value">The object to be added.</param>
        /// <returns>The index of the created object in the DataSource.</returns>
        public override int Add(object value)
        {
            // Allows adding persistent objects, so make it persistent only,
            // if it is not yet persistent.
            IPersistenceCapable pc = (IPersistenceCapable)value;
            if (pc.NDOObjectState == NDOObjectState.Transient)
            {
                this.pm.MakePersistent(pc);
                this.pm.Save();
            }
            return base.Add(value);
        }

        /// <summary>
        /// Creates a new object, makes it persistent, and adds it to the DataSource.
        /// </summary>
        /// <returns></returns>
        public override object AddNew()
        {
            object o = base.AddNew();
            pm.MakePersistent(o);
            pm.Save();
            return o;
        }

        /// <summary>
        /// Removes an object from the DataSource and deletes it using the PersistenceManager. 
        /// This method will be called if the NDOBindingSource is bound to a BindingNavigator and the
        /// Remove button has been pressed.
        /// </summary>
        /// <param name="index">The index of the object to be deleted.</param>
        public override void RemoveAt(int index)
        {
            object o = base[index];
            pm.Delete(o);
            pm.Save();
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes the given object from the DataSource and deletes it using the PersistenceManager.
        /// </summary>
        /// <param name="value">The object to be deleted.</param>
        public override void Remove(object value)
        {
            pm.Delete(value);
            pm.Save();
            base.Remove(value);
        }

    }
}
