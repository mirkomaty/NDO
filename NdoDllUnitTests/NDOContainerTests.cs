using NDO.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoDllUnitTests
{
	[TestFixture]
	public class NDOContainerTests
	{
		[Test]
		public void SimpleRegistration()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>();
			container.RegisterType<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.Resolve<Driver>();
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar());
		}

		[Test]
		public void SimpleRegistrationWithOverride()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<Driver>();

			//Resolves dependencies and returns the Driver object 
			Driver drv = container.Resolve<Driver>(null, new ParameterOverride("car", new BMW()));
			Assert.AreEqual( "Running BMW - 1 mile", drv.RunCar() );
		}

		[Test]
		public void DoubleResolve()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>();

			var car1 = container.Resolve<ICar>();
			var car2 = container.Resolve<ICar>();

			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void DoubleResolveWithName()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>();

			var car1 = container.Resolve<ICar>("car1");
			var car2 = container.Resolve<ICar>("car1");
			var car3 = container.Resolve<ICar>("car2");

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void DoubleRegisterTypeWithName()
		{
			INDOContainer container = new NDOContainer();

			container.RegisterType<ICar, BMW>( "bmw" );
			container.RegisterType<ICar, Audi>( "audi" );

			var car1 = container.Resolve<ICar>( "bmw" );
			var car2 = container.Resolve<ICar>( "bmw" );
			var car3 = container.Resolve<ICar>( "audi" );

			Assert.AreSame( car1, car2 );
			Assert.AreNotSame( car1, car3 );
		}

		[Test]
		public void ResolveOrRegisterTypeWithOverride()
		{
			INDOContainer container = new NDOContainer();

			var drv1 = container.ResolveOrRegisterType<Driver>("bmw", new ParameterOverride("car", new BMW()));
			var drv2 = container.ResolveOrRegisterType<Driver>("audi", new ParameterOverride("car", new Audi()));
			Assert.AreEqual( "Running BMW - 1 mile", drv1.RunCar() );
			Assert.AreEqual( "Running Audi - 1 mile", drv2.RunCar() );
			drv1 = container.ResolveOrRegisterType<Driver>("bmw", new ParameterOverride("car", new BMW()));
			drv2 = container.ResolveOrRegisterType<Driver>("audi", new ParameterOverride("car", new Audi()));
			Assert.AreEqual( "Running BMW - 2 mile", drv1.RunCar() );
			Assert.AreEqual( "Running Audi - 2 mile", drv2.RunCar() );
		}
	}
}
