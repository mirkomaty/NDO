using NDO;
using NDO.Mapping;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NdoUnitTests
{
	[TestFixture]
	public class TypeCodeTest
	{
		PersistenceManager pm;

		[SetUp]
		public void Setup()
		{
			this.pm = PmFactory.NewPersistenceManager();
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void TestIfAllPersistentTypesHaveATypeCode()
		{
			foreach (Class cls in pm.NDOMapping.Classes)
			{
				if (!cls.IsAbstract)
					Assert.That( cls.TypeCode != 0, cls.FullName + " does not have a TypeCode" );
			}
		}
	}
}