﻿using NDO;
using NUnit.Framework;
using PureBusinessClasses.DITests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace NdoUnitTests
{
	[TestFixture]
	public class DITests
	{
		class DIParameter : IDIParameter
		{
			public string GetString()
			{
				return "Teststring";
			}
		}


		[SetUp]
		public void Setup()
		{
			NDO.Configuration.NDOContainer.Instance.RegisterType<IDIParameter, DIParameter>();
		}
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.Delete( pm.Objects<ClassWithDIConstructor>().ResultTable );
		}

		[Test]
		public void FindObject_provides_DIParameter()
		{
			var pm = PmFactory.NewPersistenceManager();
			var testObject = (ClassWithDIConstructor)pm.FindObject( typeof( ClassWithDIConstructor ), 0 );
			Assert.AreEqual( "Teststring", testObject.DiField );
		}

		[Test]
		public void Query_provides_DIParameter()
		{
			var pm = PmFactory.NewPersistenceManager();
			var testObject = new ClassWithDIConstructor(new DIParameter());
			testObject.PersistentField = "PersistentFieldValue";
			pm.MakePersistent( testObject );
			pm.Save();
			pm.UnloadCache();
			testObject = pm.Objects<ClassWithDIConstructor>().Single();
			Assert.AreEqual( "PersistentFieldValue", testObject.PersistentField );
			Assert.AreEqual( "Teststring", testObject.DiField );
			pm.Delete( testObject );
			pm.Save();
		}
	}
}
