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
using NUnit.Framework;
using PureBusinessClasses;
using NDO;
using NDO.Mapping;
using NDO.Query;

namespace NdoUnitTests
{
    [TestFixture]
    public class CompositePartListTests
    {
        PersistenceManager pm;
        [SetUp]
        public void Setup()
        {
            pm = PmFactory.NewPersistenceManager();
        }

        [TearDown]
        public void TearDown()
        {
            pm.Delete(pm.GetClassExtent(typeof(SnmpDevice)));
            pm.Save();
            pm.Delete(pm.GetClassExtent(typeof(Device)));
            pm.Save();
        }

        [Test]
        public void TestBaseOnly()
        {
            Device root = new Device();
            root.Name = "root";
            pm.MakePersistent(root);
            pm.Save();

            Device child1 = root.NewDevice(typeof(Device));
            child1.Name = "1";

            Device child2 = root.NewDevice(typeof(Device));
            child2.Name = "2";

            pm.Save();

            Device child11 = child1.NewDevice(typeof(Device));
            child11.Name = "11";

            Device child12 = child1.NewDevice(typeof(Device));
            child12.Name = "12";

            Device child21 = child2.NewDevice(typeof(Device));
            child21.Name = "21";

            Device child22 = child2.NewDevice(typeof(Device));
            child22.Name = "22";

            pm.Save();

            NDOQuery<Device> q = new NDOQuery<Device>(pm);
            decimal c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);

            Assert.AreEqual(7m, c, "Count wrong: ");

            q = new NDOQuery<Device>(pm, "name = {0}");
            q.Parameters.Add("root");

            root = q.ExecuteSingle(true);

            Assert.AreEqual(2, root.Subdevices.Count, "Child Count wrong #1: ");
            foreach (Device d in root.Subdevices)
            {
                Assert.That(d.Name == "1" || d.Name == "2", "Name wrong #1");
            }

            pm.Delete(root);
            pm.Save();
            pm.UnloadCache();

            q = new NDOQuery<Device>(pm);
            c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);
            Assert.AreEqual(0m, c, "Count wrong: ");

        }


        [Test]
        public void TestDerivedOnly()
        {
            Device root = new SnmpDevice();
            root.Name = "root";
            pm.MakePersistent(root);
            pm.Save();

            Device child1 = root.NewDevice(typeof(SnmpDevice));
            child1.Name = "1";

            Device child2 = root.NewDevice(typeof(SnmpDevice));
            child2.Name = "2";

            pm.Save();

            Device child11 = child1.NewDevice(typeof(SnmpDevice));
            child11.Name = "11";

            Device child12 = child1.NewDevice(typeof(SnmpDevice));
            child12.Name = "12";

            Device child21 = child2.NewDevice(typeof(SnmpDevice));
            child21.Name = "21";

            Device child22 = child2.NewDevice(typeof(SnmpDevice));
            child22.Name = "22";

            pm.Save();

            NDOQuery<Device> q = new NDOQuery<Device>(pm);
            decimal c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);

            Assert.AreEqual(7m, c, "Count wrong: ");

            q = new NDOQuery<Device>(pm, "name = {0}");
            q.Parameters.Add("root");

            root = q.ExecuteSingle(true);

            Assert.AreEqual(2, root.Subdevices.Count, "Child Count wrong #1: ");
            foreach (Device d in root.Subdevices)
            {
                Assert.That(d.Name == "1" || d.Name == "2", "Name wrong #1");
            }

            pm.Delete(root);
            pm.Save();
            pm.UnloadCache();

            q = new NDOQuery<Device>(pm);
            c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);
            Assert.AreEqual(0, c, "Count wrong: ");
        }


        [Test]
        public void TestMixed()
        {
            Device root = new Device();
            root.Name = "root";
            pm.MakePersistent(root);
            pm.Save();

            Device child1 = root.NewDevice(typeof(SnmpDevice));
            child1.Name = "1";

            Device child2 = root.NewDevice(typeof(Device));
            child2.Name = "2";

            pm.Save();

            Device child11 = child1.NewDevice(typeof(Device));
            child11.Name = "11";

            Device child12 = child1.NewDevice(typeof(SnmpDevice));
            child12.Name = "12";

            Device child21 = child2.NewDevice(typeof(Device));
            child21.Name = "21";

            Device child22 = child2.NewDevice(typeof(SnmpDevice));
            child22.Name = "22";

            pm.Save();

            NDOQuery<Device> q = new NDOQuery<Device>(pm);
            decimal c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);

            Assert.AreEqual(7m, c, "Count wrong: ");

            q = new NDOQuery<Device>(pm, "name = {0}");
            q.Parameters.Add("root");

            root = q.ExecuteSingle(true);

            Assert.AreEqual(2, root.Subdevices.Count, "Child Count wrong #1: ");
            foreach (Device d in root.Subdevices)
            {
                Assert.That(d.Name == "1" || d.Name == "2", "Name wrong #1");
            }

            pm.Delete(root);
            pm.Save();
            pm.UnloadCache();

            q = new NDOQuery<Device>(pm);
            c = (decimal)q.ExecuteAggregate("name", AggregateType.Count);
            Assert.AreEqual(0m, c, "Count wrong: ");
        }


        [Test]
        public void TestMapping()
        {
            Class cl = pm.NDOMapping.FindClass(typeof(Device));
            Relation r = cl.FindRelation("subdevices");
            Assert.That(r.Bidirectional == false, "Relation shouldn't be bidirectional #1");
            Assert.Null(r.ForeignRelation, "No foreign Relation should appear #1");
            cl = pm.NDOMapping.FindClass(typeof(SnmpDevice));
            r = cl.FindRelation("subdevices");
            Assert.That(r.Bidirectional == false, "Relation shouldn't be bidirectional #1");
            Assert.Null(r.ForeignRelation, "No foreign Relation should appear #1");
        }

    }
}
