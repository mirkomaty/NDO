using NDO;
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

		PersistenceManager pm;

		[SetUp]
		public void Setup()
		{
			this.pm = PmFactory.NewPersistenceManager();
//			this.pm.ConfigContainer.RegisterType<IDIParameter, DIParameter>();
			NDO.Configuration.NDOContainer.Instance.RegisterType<IDIParameter, DIParameter>();
		}

		[Test]
		public void FindObject_provides_DIParameter()
		{
			var testObject = (ClassWithDIConstructor)this.pm.FindObject( typeof( ClassWithDIConstructor ), 0 );
			Assert.AreEqual( "Teststring", testObject.DiField );
		}

		[Test]
		public void Query_provides_DIParameter()
		{
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
