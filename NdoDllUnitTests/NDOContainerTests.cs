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

			container.RegisterType<ICar, BMW>();
			container.RegisterType<ICar, Audi>();

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

		[Test]
		public void RegisterInstance()
		{
			INDOContainer container = new NDOContainer();

			var car1 = new BMW();
			container.RegisterInstance<ICar>( car1 );

			var car2 = container.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void RegisterInstanceWithName()
		{
			INDOContainer container = new NDOContainer();

			var bmw1 = new BMW();
			container.RegisterInstance<ICar>( bmw1, "bmw" );
			var audi1 = new Audi();
			container.RegisterInstance<ICar>( audi1, "audi" );

			var bmw2 = container.Resolve<ICar>("bmw");
			Assert.AreSame( bmw1, bmw2 );
			var audi2 = container.Resolve<ICar>("audi");
			Assert.AreSame( audi1, audi2 );
		}

		[Test]
		public void ResolveAndRegisterInstance()
		{
			INDOContainer container = new NDOContainer();
			var car1 = container.ResolveOrRegisterInstance<ICar>("bmw", _=>new BMW());
			var car2 = container.ResolveOrRegisterInstance<ICar>("bmw", _=>new BMW());
			Assert.AreSame( car1, car2 );

			var car3 = container.ResolveOrRegisterInstance<ICar>("ford", _=>new Ford());
			Assert.AreNotSame( car1, car3 );

			var car4 = container.ResolveOrRegisterInstance<ICar>("bmw2", _=>new BMW());
			Assert.AreNotSame( car1, car4 );
		}

		[Test]
		public void ResolveWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterType<ICar, BMW>();

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.AreSame( car1, car2 );

			container2.RegisterType<ICar, BMW>();
			car2 = container2.Resolve<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}

		[Test]
		public void ResolveInstanceWithChildContainer()
		{
			INDOContainer container1 = new NDOContainer();

			container1.RegisterInstance<ICar>(new BMW());

			var car1 = container1.Resolve<ICar>();


			var container2 = container1.CreateChildContainer();
			var car2 = container2.Resolve<ICar>();

			Assert.AreSame( car1, car2 );

			container2.RegisterInstance<ICar>(new BMW());
			car2 = container2.Resolve<ICar>();

			Assert.AreNotSame( car1, car2 );

			container2.Dispose();

			car2 = container1.Resolve<ICar>();
			Assert.AreSame( car1, car2 );
		}
	}
}
